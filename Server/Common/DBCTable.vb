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

Public Class DBCTable
    Public results As New DataTable

    Public Sub New(ByRef DbcDatabase As SQL, ByVal dbcTable As String)
        DbcDatabase.Update("SET NAMES 'utf8';")

        DbcDatabase.Query(String.Format("SELECT * FROM " & dbcTable), results)
    End Sub

    Public Overridable ReadOnly Property Item(ByVal field As Object, Optional ByVal valueType As DBCValueType = DBCValueType.DBC_INTEGER) As Object
        Get
            Dim buffer As Byte()

            Select Case valueType
                Case DBCValueType.DBC_FLOAT
                    buffer = BitConverter.GetBytes(CType(field, Single))
                    Return BitConverter.ToSingle(buffer, 0)
                Case DBCValueType.DBC_INTEGER
                    buffer = BitConverter.GetBytes(CType(field, Integer))
                    Return BitConverter.ToInt32(buffer, 0)
                Case DBCValueType.DBC_STRING
                    buffer = BitConverter.GetBytes(CType(field, Char))

                    Dim strByte As Byte
                    Dim strResult As String
                    strByte = 0
                    strResult = ""
                    Dim i As Integer
                    For i = 0 To buffer.Length
                        strByte = buffer.GetValue(i)
                        strResult &= Chr(strByte)
                        If strByte = 0 Then Exit For
                    Next

                    Return strResult
                Case Else
                    Throw New ApplicationException("DBCTable: Undefined DBC field type.")
            End Select
        End Get
    End Property

End Class
