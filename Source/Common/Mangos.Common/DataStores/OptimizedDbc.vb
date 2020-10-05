'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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
Imports Mangos.Common.Enums

Namespace DataStores
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
        Public Overrides ReadOnly Property Item(ByVal row As Integer, ByVal column As Integer, Optional ByVal valueType As GlobalEnum.DBCValueType = DBCValueType.DBC_INTEGER) As Object
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
End Namespace
