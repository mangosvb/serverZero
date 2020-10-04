'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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

Imports MangosVB.Shared

Namespace DBC
    <Description("DBC wrapper class using optimizations for reading row by row.")> _
    Public Class OptimizedDbc
        Inherits BaseDBC

        Protected TmpRow() As Byte
        Protected TmpRowRead As Integer = -1

        <Description("Open filename for reading and initialize internals.")> _
        Public Sub New(ByVal fileName As String)
            MyBase.New(FileName)

            ReDim tmpRow(RowLength)
        End Sub
        <Description("Open filename for reading and initialize internals.")> _
        Public Sub New(ByVal stream As Stream)
            MyBase.New(Stream)

            ReDim tmpRow(RowLength)
        End Sub
        Protected Sub ReadRow(ByVal row As Integer)
            tmpOffset = 20 + Row * RowLength
            If fs.Position <> tmpOffset Then fs.Seek(tmpOffset, SeekOrigin.Begin)
            fs.Read(tmpRow, 0, RowLength)

            tmpRowRead = Row
        End Sub

        <Description("Access to item by row and column.")> _
        Public Overrides ReadOnly Property Item(ByVal row As Integer, ByVal column As Integer, Optional ByVal valueType As DBCValueType = DBCValueType.DBC_INTEGER) As Object
            Get
                If row >= Rows Then Throw New ApplicationException("DBC: Row index outside file definition.")
                If column >= Columns Then Throw New ApplicationException("DBC: Column index outside file definition.")

                If TmpRowRead <> row Then ReadRow(row)

                Array.Copy(TmpRow, column * 4, buffer, 0, 4)

                Select Case ValueType
                    Case DBCValueType.DBC_INTEGER
                        Return BitConverter.ToInt32(buffer, 0)
                    Case DBCValueType.DBC_FLOAT
                        Return BitConverter.ToSingle(buffer, 0)
                    Case DBCValueType.DBC_STRING
                        Dim offset As Integer = BitConverter.ToInt32(buffer, 0)
                        fs.Seek(20 + Rows * RowLength + offset, SeekOrigin.Begin)

                        Dim strByte As Byte = 0
                        Dim strResult As String = ""
                        Do
                            strByte = fs.ReadByte()
                            strResult &= Chr(strByte)
                        Loop While strByte <> 0

                        Return strResult
                    Case Else
                        Throw New ApplicationException("DBC: Undefined DBC field type.")
                End Select

            End Get
        End Property

    End Class

    <Description("DBC wrapper class using buffered stream for file access.")> _
    Public Class BufferedDbc
        Inherits BaseDBC
        Implements IDisposable

        Protected bs As BufferedStream

        <Description("Open filename for reading and initialize internals.")> _
        Public Sub New(ByVal fileName As String)
            MyBase.New(FileName)

            bs = New BufferedStream(fs)
        End Sub

        <Description("Open filename for reading and initialize internals.")> _
        Public Sub New(ByVal stream As Stream)
            MyBase.New(Stream)

            bs = New BufferedStream(fs)
        End Sub

        <Description("Close file and dispose the dbc reader.")> _
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                fs.Close()
                bs.Close()
                bs.Dispose()

                '                MyBase.Dispose()
            End If
            _disposedValue = True
        End Sub

        <Description("Access to item by row and column.")> _
        Public Overrides ReadOnly Property Item(ByVal row As Integer, ByVal column As Integer, Optional ByVal valueType As DBCValueType = DBCValueType.DBC_INTEGER) As Object
            Get
                If row >= Rows Then Throw New ApplicationException("DBC: Row index outside file definition.")
                If column >= Columns Then Throw New ApplicationException("DBC: Column index outside file definition.")

                tmpOffset = 20 + row * RowLength + column * 4
                If bs.Position <> tmpOffset Then bs.Seek(tmpOffset, SeekOrigin.Begin)
                bs.Read(buffer, 0, 4)

                Select Case valueType
                    Case DBCValueType.DBC_FLOAT
                        Return BitConverter.ToSingle(buffer, 0)
                    Case DBCValueType.DBC_INTEGER
                        Return BitConverter.ToInt32(buffer, 0)
                    Case DBCValueType.DBC_STRING
                        Dim offset As Integer = BitConverter.ToInt32(buffer, 0)
                        bs.Seek(20 + Rows * RowLength + offset, SeekOrigin.Begin)

                        Dim strByte As Byte = 0
                        Dim strResult As String = ""
                        Do
                            strByte = bs.ReadByte()
                            If strByte <> 0 Then strResult &= Chr(strByte)
                        Loop While strByte <> 0

                        Return strResult
                    Case Else
                        Throw New ApplicationException("DBCReader: Undefined DBC field type.")
                End Select
            End Get
        End Property

    End Class

    <Description("DBC wrapper class.")> _
    Public Class BaseDbc
        Implements IDisposable

        'Variables
        Protected Fs As Stream

        <Description("Header information: File type.")> Public FType As String = ""
        <Description("Header information: Rows contained in the file.")> Public Rows As Integer = 0
        <Description("Header information: Columns for each row.")> Public Columns As Integer = 0
        <Description("Header information: Bytes ocupied by each row.")> Public RowLength As Integer = 0
        <Description("Header information: Strings data block length.")> Public StringPartLength As Integer = 0

        Protected Buffer(3) As Byte
        Protected TmpOffset As Long = 0

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Default Functions
        <Description("Close file and dispose the dbc reader.")> _
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                fs.Close()
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
        Public Sub New(ByVal fileName As String)
            Fs = New FileStream(FileName, FileMode.Open, FileAccess.Read)

            ReadHeader()
        End Sub
        <Description("Open filename for reading and initialize internals.")> _
        Public Sub New(ByVal Stream As Stream)
            fs = Stream

            ReadHeader()
        End Sub

        Protected Sub ReadHeader()
            Try
                fs.Read(buffer, 0, 4)
                fType = Text.Encoding.ASCII.GetString(buffer)
                fs.Read(buffer, 0, 4)
                Rows = BitConverter.ToInt32(buffer, 0)
                fs.Read(buffer, 0, 4)
                Columns = BitConverter.ToInt32(buffer, 0)
                fs.Read(buffer, 0, 4)
                RowLength = BitConverter.ToInt32(buffer, 0)
                fs.Read(buffer, 0, 4)
                StringPartLength = BitConverter.ToInt32(buffer, 0)

            Catch e As Exception
                Throw New ApplicationException("DBC: File could not be read.")
            Finally
                If fType <> "WDBC" Then Throw New ApplicationException("DBC: Not valid DBC file format.")
            End Try
        End Sub

        <Description("Access to item by row and column.")> _
        Public Overridable ReadOnly Property Item(ByVal row As Integer, ByVal column As Integer, Optional ByVal valueType As DBCValueType = DBCValueType.DBC_INTEGER) As Object
            Get
                If row >= Rows Then Throw New ApplicationException("DBC: Row index outside file definition.")
                If column >= Columns Then Throw New ApplicationException("DBC: Column index outside file definition.")

                tmpOffset = 20 + row * RowLength + column * 4
                If fs.Position <> tmpOffset Then fs.Seek(tmpOffset, SeekOrigin.Begin)
                fs.Read(buffer, 0, 4)

                Select Case valueType
                    Case DBCValueType.DBC_FLOAT
                        Return BitConverter.ToSingle(buffer, 0)
                    Case DBCValueType.DBC_INTEGER
                        Return BitConverter.ToInt32(buffer, 0)
                    Case DBCValueType.DBC_STRING
                        Dim offset As Integer = BitConverter.ToInt32(buffer, 0)
                        fs.Seek(20 + Rows * RowLength + offset, SeekOrigin.Begin)

                        Dim strByte As Byte
                        Dim strResult As String
                        strByte = 0
                        strResult = ""
                        Do
                            strByte = fs.ReadByte()
                            strResult &= Chr(strByte)
                        Loop While strByte <> 0

                        Return strResult
                    Case Else
                        Throw New ApplicationException("DBCReader: Undefined DBC field type.")
                End Select
            End Get
        End Property
        <Description("Return formated DBC header information.")> _
        Public ReadOnly Property GetFileInformation() As String
            Get
                Return String.Format("DBC: {0}:{1}x{2}:{3}:{4}", fType, Rows, Columns, RowLength, StringPartLength)
            End Get
        End Property
    End Class
End Namespace
