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

Imports System.IO
Imports System.Threading

Namespace Server

    Public Class ReaderWriterLock_Debug
        Implements IDisposable

        Private ID As String
        Private file As FileStream
        Private writer As StreamWriter
        Private lock As ReaderWriterLock
        Private WriteQueue As New Queue(Of String)

        Public Sub New(ByVal s As String)
            ID = s
            file = New FileStream(String.Format("ReaderWriterLock_Debug_{0}_{1}.log", ID, Now.Ticks), FileMode.Create)
            writer = New StreamWriter(file)
            lock = New ReaderWriterLock

            Dim st As New StackTrace
            Dim sf As StackFrame() = st.GetFrames
            WriteLine("NewLock " & ID & " from:")
            For Each frame As StackFrame In sf
                WriteLine(vbTab & frame.GetMethod.Name)
            Next

            WriteLine("NewLock " & ID)

            Dim writeThread As New Thread(AddressOf WriteLoop) With {
                    .Name = "WriteLoop, ReaderWriterLock_Debug - " & s
                    }

            writeThread.Start()
        End Sub

        Public Sub AcquireReaderLock(ByVal t As Integer)
            Dim st As New StackTrace
            Dim sf As StackFrame() = st.GetFrames
            WriteLine("AcquireReaderLock " & ID & " from:")
            For Each frame As StackFrame In sf
                WriteLine(vbTab & frame.GetMethod.Name)
            Next

            lock.AcquireReaderLock(t)
        End Sub
        Public Sub ReleaseReaderLock()
            Try
                lock.ReleaseReaderLock()

                Dim st As New StackTrace
                Dim sf As StackFrame() = st.GetFrames
                WriteLine("ReleaseReaderLock " & ID & " from:")
                For Each frame As StackFrame In sf
                    WriteLine(vbTab & frame.GetMethod.Name)
                Next
            Catch ex As Exception
                WriteLine("ReleaseReaderLock " & ID & " is not freed!")
            End Try
        End Sub

        Public Sub AcquireWriterLock(ByVal t As Integer)
            Dim st As New StackTrace
            Dim sf As StackFrame() = st.GetFrames
            WriteLine("AcquireWriterLock " & ID & " from:")
            For Each frame As StackFrame In sf
                WriteLine(vbTab & frame.GetMethod.Name)
            Next

            lock.AcquireWriterLock(t)
        End Sub
        Public Sub ReleaseWriterLock()
            Try
                lock.ReleaseWriterLock()

                Dim st As New StackTrace
                Dim sf As StackFrame() = st.GetFrames
                WriteLine("ReleaseWriterLock " & ID & " from:")
                For Each frame As StackFrame In sf
                    WriteLine(vbTab & frame.GetMethod.Name)
                Next
            Catch ex As Exception
                WriteLine("ReleaseWriterLock " & ID & " is not freed!")
            End Try
        End Sub

        Public Function IsWriterLockHeld() As Boolean
            Return lock.IsWriterLockHeld
        End Function

        Public Function IsReaderLockHeld() As Boolean
            Return lock.IsReaderLockHeld
        End Function

        Public Sub WriteLine(ByVal str As String)
            SyncLock WriteQueue
                WriteQueue.Enqueue(str)
            End SyncLock
        End Sub

        Private Sub WriteLoop()
            Dim i As Integer = 0
            Dim str As String = ""
            While True
                i = 0
                Do While WriteQueue.Count > 0
                    SyncLock WriteQueue
                        str = WriteQueue.Dequeue
                    End SyncLock
                    writer.WriteLine(str)
                    i += 1
                Loop
                If i > 0 Then writer.Flush()

                Thread.Sleep(100)
            End While
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                writer.Dispose()
                file.Dispose()
            End If
            _disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End NameSpace