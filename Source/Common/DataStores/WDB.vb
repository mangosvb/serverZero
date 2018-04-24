'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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
    Public Class BaseWdb
        Implements IDisposable

        'Variables
        Protected Fs As FileStream
        Protected Bs As BufferedStream
        Public Buffer(3) As Byte

        Public FType As String = ""
        Public FBuild As Integer = 0
        Public FLocale As String = ""
        Public FUnk1 As Integer = 0
        Public FUnk2 As Integer = 0

        Public FIndex As New Hashtable

#Region "IDisposable Support"

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        <Description("Close file and dispose the wdb reader.")>
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

        ''' <summary>
        ''' Initializes a new instance of the <see cref="BaseWDB" /> class.
        ''' </summary>
        ''' <param name="filename">The filename.</param>
        <Description("Open filename for reading and initialize internals.")>
        Public Sub New(ByVal filename As String)
            Try
                Fs = New FileStream(filename, FileMode.Open, FileAccess.Read)
                Bs = New BufferedStream(Fs)

                Fs.Read(Buffer, 0, 4)
                fType = Text.Encoding.ASCII.GetString(buffer)
                Fs.Read(Buffer, 0, 4)
                fBuild = BitConverter.ToInt32(buffer, 0)
                Fs.Read(Buffer, 0, 4)
                fLocale = Text.Encoding.Default.GetString(buffer)

                Fs.Read(Buffer, 0, 4)
                fUnk1 = BitConverter.ToInt32(buffer, 0)
                Fs.Read(Buffer, 0, 4)
                fUnk2 = BitConverter.ToInt32(buffer, 0)

                'Do indexing of offsets
                Dim id As Integer = - 1
                Dim recordSize As Integer = 0
                While id <> 0
                    Fs.Read(Buffer, 0, 4)
                    id = BitConverter.ToInt32(buffer, 0)

                    Fs.Read(Buffer, 0, 4)
                    recordSize = BitConverter.ToInt32(buffer, 0)

                    fIndex.Add(id, fs.Position)

                    Fs.Seek(recordSize, SeekOrigin.Current)
                End While

            Catch e As Exception
                Console.WriteLine(e.ToString)
                Throw New ApplicationException("DBC: File could not be openned.")
            Finally

            End Try
        End Sub

        ''' <summary>
        ''' Access to item by index and id.
        ''' </summary>
        ''' <value></value>
        <Description("Access to item by index and id.")>
        Public ReadOnly Property Item(ByVal id As Integer, ByVal index As Integer, ByVal valueType As WDBValueType) As Object
            Get
                If FIndex.ContainsKey(id) Then Throw New ApplicationException("WDB: ID not found in file.")

                Bs.Seek(FIndex(id) + index, SeekOrigin.Begin)

                Select Case valueType
                    Case WDBValueType.WDB_FLOAT
                        Bs.Read(Buffer, 0, 4)
                        Return BitConverter.ToSingle(Buffer, 0)
                    Case WDBValueType.WDB_INTEGER
                        Bs.Read(Buffer, 0, 4)
                        Return BitConverter.ToInt32(Buffer, 0)
                    Case WDBValueType.WDB_BYTE
                        Return Bs.ReadByte()
                    Case WDBValueType.WDB_STRING
                        Dim strByte As Byte = 0
                        Dim strResult As String = ""
                        Do
                            strByte = Fs.ReadByte()
                            strResult &= Chr(strByte)
                        Loop While strByte <> 0

                        Return strResult
                    Case Else
                        Throw New ApplicationException("WDBReader: Undefined WDB field type.")
                End Select
            End Get
        End Property

        ''' <summary>
        ''' Search if this item id is present in the file.
        ''' </summary>
        ''' <value></value>
        <Description("Search if this item id is present in the file.")>
        Public ReadOnly Property Contains(ByVal id As Integer) As Boolean
            Get
                fIndex.ContainsKey(id)
            End Get
        End Property

        ''' <summary>
        ''' Returns found records in the wdb file.
        ''' </summary>
        ''' <value>The get records.</value>
        <Description("Returns found records in the wdb file.")>
        Public ReadOnly Property GetRecords() As Integer
            Get
                Return fIndex.Count
            End Get
        End Property
    End Class
End Namespace
