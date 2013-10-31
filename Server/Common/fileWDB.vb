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
Imports System.IO
Imports System.ComponentModel

Namespace WDB
    Public Class BaseWDB
        Implements IDisposable

        'Variables
        Protected fs As FileStream
        Protected bs As BufferedStream
        Public buffer(3) As Byte

        Public fType As String = ""
        Public fBuild As Integer = 0
        Public fLocale As String = ""
        Public fUnk1 As Integer = 0
        Public fUnk2 As Integer = 0

        Public fIndex As New Hashtable

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        <Description("Close file and dispose the wdb reader.")> _
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                fs.Close()
                bs.Close()
            End If
            _disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        <Description("Open filename for reading and initialize internals.")> _
        Public Sub New(ByVal FileName As String)
            Try
                fs = New FileStream(FileName, FileMode.Open, FileAccess.Read)
                bs = New BufferedStream(fs)

                fs.Read(buffer, 0, 4)
                fType = Text.Encoding.ASCII.GetString(buffer)
                fs.Read(buffer, 0, 4)
                fBuild = BitConverter.ToInt32(buffer, 0)
                fs.Read(buffer, 0, 4)
                fLocale = Text.Encoding.Default.GetString(buffer)

                fs.Read(buffer, 0, 4)
                fUnk1 = BitConverter.ToInt32(buffer, 0)
                fs.Read(buffer, 0, 4)
                fUnk2 = BitConverter.ToInt32(buffer, 0)

                'Do indexing of offsets
                Dim ID As Integer = -1
                Dim RecordSize As Integer = 0
                While ID <> 0
                    fs.Read(buffer, 0, 4)
                    ID = BitConverter.ToInt32(buffer, 0)

                    fs.Read(buffer, 0, 4)
                    RecordSize = BitConverter.ToInt32(buffer, 0)

                    fIndex.Add(ID, fs.Position)

                    fs.Seek(RecordSize, SeekOrigin.Current)
                End While

            Catch e As Exception
                Console.WriteLine(e.ToString)
                Throw New ApplicationException("DBC: File could not be openned.")
            Finally

            End Try

        End Sub

        <Description("Access to item by index and id.")> _
        Public ReadOnly Property Item(ByVal ID As Integer, ByVal Index As Integer, ByVal ValueType As WDBValueType) As Object
            Get
                If fIndex.ContainsKey(ID) Then Throw New ApplicationException("WDB: ID not found in file.")

                bs.Seek(fIndex(ID) + Index, SeekOrigin.Begin)

                Select Case ValueType
                    Case WDBValueType.WDB_FLOAT
                        bs.Read(buffer, 0, 4)
                        Return BitConverter.ToSingle(buffer, 0)
                    Case WDBValueType.WDB_INTEGER
                        bs.Read(buffer, 0, 4)
                        Return BitConverter.ToInt32(buffer, 0)
                    Case WDBValueType.WDB_BYTE
                        Return bs.ReadByte()
                    Case WDBValueType.WDB_STRING
                        Dim strByte As Byte = 0
                        Dim strResult As String = ""
                        Do
                            strByte = fs.ReadByte()
                            strResult &= Chr(strByte)
                        Loop While strByte <> 0

                        Return strResult
                    Case Else
                        Throw New ApplicationException("WDBReader: Undefined WDB field type.")
                End Select
            End Get
        End Property
        <Description("Search if this item id is present in the file.")> _
        Public ReadOnly Property Contains(ByVal ID As Integer) As Boolean
            Get
                fIndex.ContainsKey(ID)
            End Get
        End Property
        <Description("Returns found records in the wdb file.")> _
        Public ReadOnly Property GetRecords() As Integer
            Get
                Return fIndex.Count
            End Get
        End Property
    End Class

    Public Enum WDBValueType
        WDB_STRING
        WDB_INTEGER
        WDB_BYTE
        WDB_FLOAT
    End Enum
End Namespace
