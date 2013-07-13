Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip.Compression.Streams

Public Module ZLib

    Public Function Compress(ByVal b As Byte(), ByVal offset As Integer, ByVal len As Integer) As Byte()
        Dim buffer2() As Byte

        Try
            Dim outputStream As New MemoryStream
            Dim compressordStream As New DeflaterOutputStream(outputStream)
            compressordStream.Write(b, offset, len)
            compressordStream.Flush()
            compressordStream.Close()
            buffer2 = outputStream.ToArray
        Catch e As Exception
            buffer2 = Nothing
        End Try

        Return buffer2
    End Function

    Public Function DeCompress(ByVal b As Byte()) As Byte()
        Dim buffer2() As Byte = Nothing
        Dim writeBuffer(Short.MaxValue) As Byte

        Dim decopressorStream As New InflaterInputStream(New MemoryStream(b))
        Try
            Dim bytesRead As Integer = decopressorStream.Read(writeBuffer, 0, writeBuffer.Length)
            If (bytesRead > 0) Then
                buffer2 = New Byte(bytesRead - 1) {}
                Buffer.BlockCopy(writeBuffer, 0, buffer2, 0, bytesRead)
            End If
            decopressorStream.Flush()
            decopressorStream.Close()
        Catch e As Exception
            buffer2 = Nothing
        End Try

        Return buffer2
    End Function

End Module