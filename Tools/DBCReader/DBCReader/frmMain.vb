'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System
Imports System.IO
Imports System.Object

Public Class frmMain

    Private IsFloat As New List(Of Integer)
    Private IsString As New List(Of Integer)

    Private StringData() As Byte = {}

    Private Sub cmdBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowse.Click
        Dim fdlg As OpenFileDialog = New OpenFileDialog()
        fdlg.Title = "Which DBC You Want to View"
        fdlg.Filter = "DBC File (*.dbc)|*.dbc"
        fdlg.FilterIndex = 2
        fdlg.RestoreDirectory = True
        If fdlg.ShowDialog() = DialogResult.OK Then
            txtFile.Text = fdlg.FileName

            IsFloat.Clear()
            IsString.Clear()
            DBCData.Clear()
            cmbColumn.Items.Clear()
            Dim fs As New FileStream(fdlg.FileName, FileMode.Open, FileAccess.Read)
            Dim s As New BinaryReader(fs)
            s.BaseStream.Seek(0, SeekOrigin.Begin)
            Dim Buffer() As Byte = s.ReadBytes(FileLen(fdlg.FileName))
            HandleDBCData(Buffer)
            Buffer = Nothing
            s.Close()
        End If
    End Sub

    Private Sub HandleDBCData(ByVal Data() As Byte)
        Dim DBCType As String = Chr(Data(0)) & Chr(Data(1)) & Chr(Data(2)) & Chr(Data(3))
        Dim Rows As Integer = BitConverter.ToInt32(Data, 4)
        Dim Columns As Integer = BitConverter.ToInt32(Data, 8)
        Dim RowLength As Integer = BitConverter.ToInt32(Data, 12)
        Dim StringPartLength As Integer = BitConverter.ToInt32(Data, 16)

        If DBCType <> "WDBC" Then MsgBox("This file is not a DBC file.", MsgBoxStyle.Critical, "Error") : Exit Sub
        If Rows <= 0 Or Columns <= 0 Or RowLength <= 0 Then MsgBox("This file is not a DBC file.", MsgBoxStyle.Critical, "Error") : Exit Sub

        Dim i As Integer, j As Integer, tmpOffset As Integer
        Dim tmpStr(Columns - 1) As String, AmtZero(Columns - 1) As Integer, foundStrings(Columns - 1) As List(Of Integer)

        For i = 0 To Columns - 1
            Dim tmpColumn As New System.Windows.Forms.ColumnHeader
            tmpColumn.Text = CStr(i)
            tmpColumn.Width = 90
            DBCData.Columns.Add(tmpColumn)
            cmbColumn.Items.Add(CStr(i))

            AmtZero(i) = 0
            foundStrings(i) = New List(Of Integer)
        Next

        'Check if any column uses floats instead of uint32's
        'Code below doesn't work at the moment, flags are in some cases counted as floats
        Dim tmpSng As Single, tmpString As String = "", notFloat As New List(Of Integer)
        For i = 0 To 99
            If i > (Rows - 1) Then Exit For
            For j = 0 To Columns - 1
                If notFloat.Contains(j) = False Then
                    tmpOffset = 20 + i * RowLength + j * 4
                    tmpSng = Math.Abs(BitConverter.ToSingle(Data, tmpOffset))
                    If tmpSng < 50000 Then 'Only allow floats to be between 0 and 50000 (negative and positive)
                        tmpString = CStr(tmpSng).Replace(",", ".")
                        tmpString = tmpString.Substring(tmpString.IndexOf(".") + 1)
                        If CStr(tmpSng).Replace(",", ".").IndexOf(".") = -1 OrElse (tmpString.Length >= 1 And tmpString.Length <= 6) Then 'Only allow a minimum of 1 decimal and a maximum of 5 decimals
                            If IsFloat.Contains(j) = False Then IsFloat.Add(j)
                        ElseIf IsFloat.Contains(j) Then
                            IsFloat.Remove(j)
                            notFloat.Add(j)
                        End If
                    End If
                End If
            Next
        Next

        'Check if any column is a string
        Dim tmpInt As Integer, notString As New List(Of Integer)
        If StringPartLength > 0 Then
            For i = 0 To 99
                If i > (Rows - 1) Then Exit For
                For j = 0 To Columns - 1
                    If notString.Contains(j) = False AndAlso IsFloat.Contains(j) = False Then
                        tmpOffset = 20 + i * RowLength + j * 4
                        tmpInt = BitConverter.ToInt32(Data, tmpOffset)

                        If tmpInt >= 0 And tmpInt < StringPartLength Then
                            tmpOffset = 20 + Rows * RowLength + tmpInt
                            If tmpInt > 0 AndAlso Data(tmpOffset - 1) > 0 Then
                                If IsString.Contains(j) Then IsString.Remove(j)
                                notString.Add(j)
                                Continue For
                            End If
                            If tmpInt > 0 Then
                                tmpString = GetString(Data, tmpOffset)
                            Else
                                AmtZero(j) += 1
                            End If
                            If tmpInt = 0 OrElse IsValidString(tmpString) Then
                                If IsString.Contains(j) = False Then IsString.Add(j)
                                If foundStrings(j).Contains(tmpInt) = False Then foundStrings(j).Add(tmpInt)
                            ElseIf IsString.Contains(j) Then
                                IsString.Remove(j)
                                notString.Add(j)
                            End If
                        ElseIf IsString.Contains(j) Then
                            IsString.Remove(j)
                            notString.Add(j)
                        End If
                    End If
                Next
            Next
        End If

        For i = 0 To Columns - 1
            If IsString.Contains(i) Then
                If IsString.Contains(i) AndAlso AmtZero(i) > 90 Then IsString.Remove(i) : Continue For
                If foundStrings(i).Count < IIf(Rows < 10, 2, 5) Then IsString.Remove(i) : Continue For
            End If
        Next
        Application.DoEvents()

        Dim tmpTag(Columns - 1) As Integer
        For i = 0 To Rows - 1
            For j = 0 To Columns - 1
                tmpTag(j) = 0

                tmpOffset = 20 + i * RowLength + j * 4
                If IsFloat.Contains(j) Then
                    tmpStr(j) = CStr(BitConverter.ToSingle(Data, tmpOffset))
                ElseIf IsString.Contains(j) Then
                    tmpOffset = BitConverter.ToInt32(Data, tmpOffset)
                    tmpTag(j) = tmpOffset
                    tmpStr(j) = GetString(Data, 20 + Rows * RowLength + tmpOffset)
                Else
                    tmpStr(j) = CStr(BitConverter.ToInt32(Data, tmpOffset))
                End If
            Next

            With DBCData.Items.Add(tmpStr(0))
                If Columns > 1 Then
                    For j = 1 To Columns - 1
                        .SubItems.Add(tmpStr(j)).Tag = tmpTag(j)
                    Next j
                End If
            End With

            If (i Mod 100) = 0 Then
                ProgressBar.Value = CInt(((i + 1) / Rows) * 100)
            End If
        Next

        ReDim StringData(StringPartLength - 1)
        Array.Copy(Data, 20 + Rows * RowLength, StringData, 0, StringData.Length)

        ProgressBar.Value = 0
    End Sub

    Private Function IsValidString(ByVal str As String) As Boolean
        Dim chars() As Char = str.ToCharArray
        Dim accepted() As Char = " ():.,'-*_?\/<>;$%".ToCharArray
        For i As Integer = 0 To chars.Length - 1
            If (chars(i) < "A"c OrElse chars(i) > "z"c) AndAlso (chars(i) < "0"c OrElse chars(i) > "9"c) Then
                For j = 0 To accepted.Length - 1
                    If chars(i) = accepted(j) Then Exit For
                    If j = accepted.Length - 1 Then
                        Return False
                    End If
                Next
            End If
        Next
        Return True
    End Function

    Private Sub DBCData_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles DBCData.ColumnClick
        Dim i As Integer, tmpInt As Integer, tmpSng As Single, Buffer(3) As Byte, A_Float As Boolean, A_String As Boolean
        Dim FailString As Boolean = False, DoneChange As Boolean = False, tmpInt2 As Integer
        If DBCData.Items.Count = 0 Then Exit Sub

        A_Float = IsFloat.Contains(e.Column)
        A_String = (A_Float = False AndAlso IsString.Contains(e.Column) = True)
        For i = 0 To DBCData.Items.Count - 1
            If A_Float Then 'To string or int if it's not possible
                tmpSng = CSng(DBCData.Items(i).SubItems(e.Column).Text)
                Buffer = BitConverter.GetBytes(tmpSng)
                tmpInt = BitConverter.ToInt32(Buffer, 0)

                If FailString = False AndAlso tmpInt > 0 AndAlso tmpInt < StringData.Length - 4 Then
                    DBCData.Items(i).SubItems(e.Column).Text = GetString(StringData, tmpInt)
                    DBCData.Items(i).SubItems(e.Column).Tag = tmpInt
                Else
                    If DoneChange = False Then
                        DoneChange = True
                        FailString = True
                        For j = 0 To i - 1
                            tmpInt2 = CStr(DBCData.Items(j).SubItems(e.Column).Tag)
                            DBCData.Items(j).SubItems(e.Column).Tag = CInt(0)
                            DBCData.Items(j).SubItems(e.Column).Text = tmpInt2
                        Next

                        IsFloat.Remove(e.Column)
                        IsString.Add(e.Column)
                    End If

                    DBCData.Items(i).SubItems(e.Column).Text = tmpInt
                End If
            ElseIf A_String Then 'To int
                tmpInt = CStr(DBCData.Items(i).SubItems(e.Column).Tag)
                DBCData.Items(i).SubItems(e.Column).Tag = CInt(0)
                DBCData.Items(i).SubItems(e.Column).Text = tmpInt
            Else 'To float
                tmpInt = CInt(DBCData.Items(i).SubItems(e.Column).Text)
                Buffer = BitConverter.GetBytes(tmpInt)
                DBCData.Items(i).SubItems(e.Column).Text = BitConverter.ToSingle(Buffer, 0)
            End If

            ProgressBar.Value = CInt(((i + 1) / DBCData.Items.Count) * 100)
        Next i
        If FailString = True Then A_Float = False : A_String = True

        If A_Float Then
            IsFloat.Remove(e.Column)
            IsString.Add(e.Column)
        ElseIf A_String Then
            IsString.Remove(e.Column)
        Else
            IsFloat.Add(e.Column)
        End If

        ProgressBar.Value = 0
    End Sub

    Private Function GetString(ByRef Data() As Byte, ByVal Index As Integer) As String
        Dim tmpStr As String = ""
        Dim i As Integer
        For i = Index To Data.Length - 1
            If Data(i) = 0 Then Exit For
        Next
        If i = Index Then Return ""
        Return System.Text.ASCIIEncoding.ASCII.GetString(Data, Index, i - Index)
    End Function

    Private Sub cmdSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSearch.Click
        If cmbColumn.Items.Count = 0 Then MsgBox("To be able to search you must first open up a DBC File!", MsgBoxStyle.Exclamation, "Unable to search") : Exit Sub
        If cmbColumn.SelectedItem Is Nothing Then MsgBox("You must select a column to search in first!", MsgBoxStyle.Exclamation, "Unable to search") : Exit Sub

        Dim sQuery As String = txtQuery.Text
        Dim column As Integer = cmbColumn.SelectedItem
        Dim start As Integer = 0
        If DBCData.SelectedItems.Count > 0 Then
            start = Integer.MaxValue - 1
            For Each item As ListViewItem In DBCData.SelectedItems
                If item.Index < start Then start = item.Index
            Next
            start += 1
        End If

        Dim is_string As Boolean = IsString.Contains(column)

        For i As Integer = start To DBCData.Items.Count - 1
            Dim sItem As String = DBCData.Items(i).SubItems(column).Text
            If is_string AndAlso sItem.IndexOf(sQuery, StringComparison.OrdinalIgnoreCase) >= 0 Then
                DBCData.SelectedItems.Clear()
                DBCData.Items(i).Selected = True
                DBCData.Items(i).EnsureVisible()
                Exit Sub
            ElseIf sItem = sQuery Then
                DBCData.SelectedItems.Clear()
                DBCData.Items(i).Selected = True
                DBCData.Items(i).EnsureVisible()
                Exit Sub
            End If
        Next

        MsgBox("No result for that search was found!" & vbNewLine & vbNewLine & "Do note that the search starts from your current selection.", MsgBoxStyle.Information, "No result found")
    End Sub

    Private Sub txtQuery_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtQuery.KeyDown
        If e.KeyCode = Keys.Enter Then
            cmdSearch.PerformClick()
        End If
    End Sub

End Class
