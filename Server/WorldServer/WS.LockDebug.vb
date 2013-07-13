Imports System.IO

Public Class ReaderWriterLock_Debug
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

        Dim writeThread As New Thread(AddressOf WriteLoop)
        writeThread.Name = "WriteLoop, ReaderWriterLock_Debug - " & s
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
                writer.WriteLine(Str)
                i += 1
            Loop
            If i > 0 Then writer.Flush()

            Thread.Sleep(100)
        End While
    End Sub

End Class
