'
' Copyright (C) 2013-2021 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.ComponentModel
Imports System.IO
Imports Mangos.Common.Enums.Global

Namespace DataStores
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

        <Description("Access to item by row and column.")>
        Public Overrides ReadOnly Property Item(ByVal row As Integer, ByVal column As Integer, Optional ByVal valueType As DBCValueType = DBCValueType.DBC_INTEGER) As Object
            Get
                If row >= Rows Then Throw New ApplicationException("DBC: Row index outside file definition.")
                If column >= Columns Then Throw New ApplicationException("DBC: Column index outside file definition.")

                TmpOffset = 20 + row * RowLength + column * 4
                If bs.Position <> TmpOffset Then bs.Seek(TmpOffset, SeekOrigin.Begin)
                bs.Read(Buffer, 0, 4)

                Select Case valueType
                    Case DBCValueType.DBC_FLOAT
                        Return BitConverter.ToSingle(Buffer, 0)
                    Case DBCValueType.DBC_INTEGER
                        Return BitConverter.ToInt32(Buffer, 0)
                    Case DBCValueType.DBC_STRING
                        Dim offset As Integer = BitConverter.ToInt32(Buffer, 0)
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
End NameSpace