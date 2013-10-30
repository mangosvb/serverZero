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

Imports ICSharpCode.SharpZipLib.Zip.Compression.Streams
Imports System.IO
Imports System.Runtime.CompilerServices
Imports mangosVB.Common.BaseWriter


Public Module Global_Zip

    <MethodImplAttribute(MethodImplOptions.Synchronized)> _
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
            '            BaseWriter.Log.WriteLine(LogType.FAILED, "ZIP: {0}", e.Message)
            buffer2 = Nothing
        End Try

        Return buffer2
    End Function

    <MethodImplAttribute(MethodImplOptions.Synchronized)> _
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
            'Log.WriteLine(LogType.FAILED, "ZIP: {0}", e.Message)
            buffer2 = Nothing
        End Try

        Return buffer2
    End Function


End Module
