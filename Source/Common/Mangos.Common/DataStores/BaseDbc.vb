Imports System.ComponentModel
Imports System.IO

Namespace DBC
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
End NameSpace