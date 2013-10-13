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
'TODO:
'       MpqHuffman
'       MpqWavCompression
'       PKDecompress
'
Imports System.Runtime.InteropServices
Imports System.Collections
Imports System.IO
Imports System.ComponentModel
Imports System
Imports ICSharpCode.SharpZipLib.BZip2
Imports ICSharpCode.SharpZipLib.Zip.Compression.Streams

Namespace MPQ

    Public Class MPQArchive
        Implements IDisposable

        Public Const UINT32_MAX As Integer = &HFFFFFFFF
        Public Const UINT32_MIN As Integer = 0

        Shared Sub New()
            sStormBuffer = BuildStormBuffer()
        End Sub
        Public Sub New(ByVal SourceStream As Stream)
            mStream = SourceStream
            Init()
        End Sub
        Public Sub New(ByVal Filename As String)
            mStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read)
            Init()
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                If (Not mStream Is Nothing) Then
                    mStream.Close()
                End If
            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Private Sub Init()
            If Not LocateMpqHeader() Then
                Throw New Exception("Unable to find MPQ header")
            End If

            Dim br As New BinaryReader(mStream)
            mBlockSize = (&H200 << mHeader.BlockSize)

            'Load hash table
            mStream.Seek(mHeader.HashTablePos, SeekOrigin.Begin)
            Dim hashdata As Byte() = br.ReadBytes(CInt((mHeader.HashTableSize * MpqHash.Size)))
            DecryptTable(hashdata, "(hash table)")

            Dim br2 As New BinaryReader(New MemoryStream(hashdata))
            ReDim mHashes(mHeader.HashTableSize - 1)

            For i As Integer = 0 To mHeader.HashTableSize - 1
                mHashes(i) = New MpqHash(br2)
            Next

            'Load block table
            mStream.Seek(mHeader.BlockTablePos, SeekOrigin.Begin)
            Dim blockdata As Byte() = br.ReadBytes(mHeader.BlockTableSize * MpqBlock.Size)
            MPQArchive.DecryptTable(blockdata, "(block table)")

            br2 = New BinaryReader(New MemoryStream(blockdata))
            ReDim mBlocks(mHeader.BlockTableSize - 1)

            For i As Integer = 0 To mHeader.BlockTableSize - 1
                mBlocks(i) = New MpqBlock(br2, mHeaderOffset)
            Next

        End Sub
        Private Function LocateMpqHeader() As Boolean
            Dim br As New BinaryReader(mStream)

            'In .mpq files the header will be at the start of the file
            'In .exe files, it will be at a multiple of 0x200
            For i As Long = 0 To (mStream.Length - MpqHeader.Size) - 1 Step &H200
                mStream.Seek(i, SeekOrigin.Begin)
                mHeader = New MpqHeader(br)
                If (mHeader.ID = MpqHeader.MpqId) Then
                    mHeaderOffset = i
                    mHeader.HashTablePos += mHeaderOffset
                    mHeader.BlockTablePos += mHeaderOffset
                    If (mHeader.DataOffset = &H6D9E4B86) Then
                        'then this is a protected archive
                        mHeader.DataOffset = (MpqHeader.Size + i)
                    End If
                    Return True
                End If
            Next

            Return False
        End Function
        Public Function OpenFile(ByVal Filename As String) As MpqStream
            Dim hash As MpqHash = Me.GetHashEntry(Filename)
            Dim blockIndex As Long = hash.BlockIndex

            If (blockIndex = UINT32_MAX) Then
                Throw New FileNotFoundException(("File not found: " & Filename))
            End If

            Dim block As MpqBlock = mBlocks(blockIndex)
            Return New MpqStream(Me, block)
        End Function
        Public Function FileExists(ByVal Filename As String) As Boolean
            Dim hash As MpqHash = GetHashEntry(Filename)
            Return (hash.BlockIndex <> UINT32_MAX)
        End Function

        Friend ReadOnly Property BaseStream() As Stream
            Get
                Return mStream
            End Get
        End Property
        Friend ReadOnly Property BlockSize() As Integer
            Get
                Return mBlockSize
            End Get
        End Property

        Private Function GetHashEntry(ByVal Filename As String) As MpqHash
            Dim index As Long = MPQArchive.HashString(Filename, 0)
            index = (index And (mHeader.HashTableSize - 1))
            Dim name1 As Long = MPQArchive.HashString(Filename, &H100)
            Dim name2 As Long = MPQArchive.HashString(Filename, &H200)

            Dim i As Long = index
            Do While (i < Me.mHashes.Length)
                Dim hash As MpqHash = mHashes(i)
                If ((hash.Name1 = name1) AndAlso (hash.Name2 = name2)) Then
                    Return hash
                End If

                i += 1
            Loop

            Dim nullhash As MpqHash = New MpqHash
            nullhash.BlockIndex = UINT32_MAX
            Return nullhash
        End Function
        Friend Shared Function HashString(ByVal Input As String, ByVal Offset As Integer) As Long
            Dim seed1 As Long = &H7FED7FED
            Dim seed2 As Long = &HEEEEEEEE

            For Each c As Char In Input
                Dim val As Long = Asc(Char.ToUpper(c))

                seed1 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes((sStormBuffer(Offset + val) Xor CType((seed1 + seed2) And &HFFFFFFFF, Long))), 0))
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes((val + seed1 + seed2 + (seed2 << 5) + 3)), 0))
            Next
            Return seed1
        End Function

        'Used for Hash Tables and Block Tables
        Friend Shared Sub DecryptBlock(ByVal Data As Byte(), ByVal Seed1 As Long)
            Dim seed2 As Long = &HEEEEEEEE

            'NB: If the block is not an even multiple of 4,
            'the remainder is not encrypted
            Dim i As Integer = 0
            Do While (i < (Data.Length - 3))
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(seed2 + MPQArchive.sStormBuffer(&H400 + (Seed1 And &HFF))), 0))
                Dim result As Long = Convert.ToInt64(BitConverter.ToUInt32(Data, i))
                result = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(result Xor (Seed1 + seed2)), 0))

                Seed1 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((CType(Not Seed1, Long) << 21) + &H11111111) Or (Seed1 >> 11)), 0))
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((result + seed2) + (seed2 << 5)) + 3), 0))
                If BitConverter.IsLittleEndian Then
                    Data(i) = CByte(result And &HFF)
                    Data(i + 1) = CByte((result >> 8) And &HFF)
                    Data(i + 2) = CByte((result >> 16) And &HFF)
                    Data(i + 3) = CByte((result >> 24) And &HFF)
                Else
                    Data(i + 3) = CByte(result And &HFF)
                    Data(i + 2) = CByte((result >> 8) And &HFF)
                    Data(i + 1) = CByte((result >> 16) And &HFF)
                    Data(i) = CByte((result >> 24) And &HFF)
                End If

                i += 4
            Loop

        End Sub
        Friend Shared Sub DecryptBlock(ByVal Data As Long(), ByVal Seed1 As Long)
            Dim seed2 As Long = &HEEEEEEEE

            Dim i As Integer = 0
            Do While (i < Data.Length)
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(seed2 + MPQArchive.sStormBuffer(&H400 + (Seed1 And &HFF))), 0))
                Dim result As Long = Data(i)
                result = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(result Xor (Seed1 + seed2)), 0))

                Seed1 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((Not Seed1 << 21) + &H11111111) Or (Seed1 >> 11)), 0))
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((result + seed2) + (seed2 << 5)) + 3), 0))
                Data(i) = result

                i += 1
            Loop
        End Sub
        Friend Shared Sub DecryptTable(ByVal Data As Byte(), ByVal Key As String)
            DecryptBlock(Data, HashString(Key, &H300))
        End Sub

        'This function calculates the encryption key based on
        'some assumptions we can make about the headers for encrypted files
        Friend Shared Function DetectFileSeed(ByVal Data As Long(), ByVal Decrypted As Long) As Long
            Dim value0 As Long = Data(0)
            Dim value1 As Long = Data(1)
            Dim temp As Long = ((value0 Xor Decrypted) - &HEEEEEEEE)

            Dim i As Integer = 0
            Do While (i < &H100)
                Dim seed1 As Long = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(temp - MPQArchive.sStormBuffer((&H400 + i))), 0))
                Dim seed2 As Long = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(&HEEEEEEEE + MPQArchive.sStormBuffer(&H400 + (seed1 And &HFF))), 0))
                Dim result As Long = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(value0 Xor (seed1 + seed2)), 0))

                If (result = Decrypted) Then
                    'Test this result against the 2nd value

                    Dim saveSeed As Long = seed1
                    seed1 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((Not seed1 << 21) + &H11111111) Or (seed1 >> 11)), 0))
                    seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((result + seed2) + (seed2 << 5)) + 3), 0))
                    seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(seed2 + MPQArchive.sStormBuffer(&H400 + (seed1 And &HFF))), 0))
                    result = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(value1 Xor (seed1 + seed2)), 0))
                    If ((result And &HFFFF0000) = 0) Then
                        Return saveSeed
                    End If
                End If

                i += 1
            Loop

            Return 0
        End Function
        Private Shared Function BuildStormBuffer() As Long()
            Dim seed As Long = &H100001
            Dim result As Long() = New Long(&H500 - 1) {}

            For index1 As Long = 0 To &H100 - 1
                Dim index2 As Integer = index1
                For i As Long = 0 To 5 - 1
                    seed = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((seed * 125) + 3) Mod &H2AAAAB), 0))
                    Dim temp As Long = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes((seed And &HFFFF) << 16), 0))
                    seed = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((seed * 125) + 3) Mod &H2AAAAB), 0))
                    result(index2) = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(temp Or (seed And &HFFFF)), 0))

                    index2 += &H100
                Next
            Next

            Return result
        End Function

#Region "FileInfoSupport"
        Public Class FileInfo
            Public CompressedSize As Long
            <CLSCompliant(False)> Public Flags As MpqFileFlags
            Public Name As String
            Public UncompressedSize As Long
        End Class

        <Description("Gets or sets the external list file. This setting overrides any list file contained in the archive")> _
        Public Property ExternalListFile() As String
            Get
                Return _ExternalListFile
            End Get
            Set(ByVal value As String)
                _ExternalListFile = value
            End Set
        End Property
        Private _ExternalListFile As String = Nothing

        <Description("Returns a collection of file infos for the archive. The archive must contain ""ListFileName"" (default ""(listfile)"") for this to work.")> _
        Public ReadOnly Property Files() As FileInfo()
            Get
                If (_Files Is Nothing) Then
                    Try
                        Dim stm As Stream = Nothing

                        'Open list file stream - either external or internal
                        If ((Not ExternalListFile Is Nothing) AndAlso (Not ExternalListFile Is "")) Then
                            stm = New FileStream(ExternalListFile, FileMode.Open, FileAccess.Read, FileShare.Read)
                        Else
                            stm = OpenFile("(listfile)")
                        End If

                        Dim stream2 As Stream = stm
                        Dim reader As StreamReader = New StreamReader(stm)

                        Dim data As String = reader.ReadLine
                        Dim filesList As New ArrayList

                        Do While (Not data Is Nothing)
                            Dim hash As MpqHash = GetHashEntry(data)
                            Dim block As MpqBlock = mBlocks(hash.BlockIndex)

                            'initialize and add new FileInfo
                            Dim fi As New FileInfo
                            fi.Name = data
                            fi.Flags = block.Flags
                            fi.UncompressedSize = block.FileSize
                            fi.CompressedSize = block.CompressedSize

                            filesList.Add(fi)
                            data = reader.ReadLine
                        Loop
                        _Files = filesList.ToArray(GetType(FileInfo))

                        reader.Close()
                        '                        stream2.Close()

                    Catch e As FileNotFoundException
                        Throw New NotSupportedException("Error: the archive contains no listfile")
                    End Try
                End If

                Return _Files
            End Get
        End Property
        <CLSCompliant(False)> Protected _Files As FileInfo()
#End Region

        Private mBlocks() As MpqBlock
        Private mBlockSize As Integer
        Private mHashes() As MpqHash
        Private mHeader As MpqHeader
        Private mHeaderOffset As Long
        Private mStream As Stream
        Private Shared sStormBuffer As Long()

    End Class

    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure MpqBlock

        Public Sub New(ByVal br As BinaryReader, ByVal HeaderOffset As Long)
            FilePos = (Convert.ToInt64(br.ReadUInt32) + HeaderOffset)
            CompressedSize = Convert.ToInt64(br.ReadUInt32)
            FileSize = Convert.ToInt64(br.ReadUInt32)
            Flags = Convert.ToInt64(br.ReadUInt32)
        End Sub

        Public ReadOnly Property IsCompressed() As Boolean
            Get
                Return ((Flags And MpqFileFlags.MPQ_Compressed) <> 0)
            End Get
        End Property
        Public ReadOnly Property IsEncrypted() As Boolean
            Get
                Return ((Flags And MpqFileFlags.MPQ_Encrypted) <> 0)
            End Get
        End Property

        Public CompressedSize As Long
        Public FilePos As Long
        Public FileSize As Long
        Public Flags As MpqFileFlags
        Public Const Size As Long = 16
    End Structure
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure MpqHash
        Public Sub New(ByVal br As BinaryReader)
            Name1 = Convert.ToInt64(br.ReadUInt32)
            Name2 = Convert.ToInt64(br.ReadUInt32)
            Locale = Convert.ToInt64(br.ReadUInt32)
            BlockIndex = Convert.ToInt64(br.ReadUInt32)
        End Sub

        Public ReadOnly Property IsValid() As Boolean
            Get
                Return ((Name1 <> MPQArchive.UINT32_MAX) AndAlso (Name2 <> MPQArchive.UINT32_MAX))
            End Get
        End Property

        Public BlockIndex As Long
        Public Locale As Long
        Public Name1 As Long
        Public Name2 As Long
        Public Const Size As Long = 16
    End Structure
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure MpqHeader

        Public Sub New(ByVal br As BinaryReader)
            ID = Convert.ToInt64(br.ReadUInt32)
            DataOffset = Convert.ToInt64(br.ReadUInt32)
            ArchiveSize = Convert.ToInt64(br.ReadUInt32)
            Offs0C = Convert.ToInt32(br.ReadUInt16)
            BlockSize = Convert.ToInt32(br.ReadUInt16)
            HashTablePos = Convert.ToInt64(br.ReadUInt32)
            BlockTablePos = Convert.ToInt64(br.ReadUInt32)
            HashTableSize = Convert.ToInt64(br.ReadUInt32)
            BlockTableSize = Convert.ToInt64(br.ReadUInt32)
        End Sub

        Public ArchiveSize As Long
        Public BlockSize As Integer
        Public BlockTablePos As Long
        Public BlockTableSize As Long
        Public DataOffset As Long
        Public HashTablePos As Long
        Public HashTableSize As Long
        Public ID As Long
        Public Const MpqId As Long = 441536589
        Public Offs0C As Integer
        Public Const Size As Long = 32
    End Structure

    'A Stream based class for reading a file from an MPQ file
    Public Class MpqStream
        Inherits Stream

        Friend Sub New(ByVal File As MPQArchive, ByVal Block As MpqBlock)
            mCurrentBlockIndex = -1
            mBlock = Block
            mStream = File.BaseStream
            mBlockSize = File.BlockSize
            If mBlock.IsCompressed Then
                LoadBlockPositions()
            End If
        End Sub
        Private Sub BufferData()
            Dim requiredblock As Integer = mPosition \ mBlockSize
            If (requiredblock <> mCurrentBlockIndex) Then
                Dim expectedlength As Integer = CInt(Math.Min(Length - (requiredblock * mBlockSize), mBlockSize))
                mCurrentData = LoadBlock(requiredblock, expectedlength)
                mCurrentBlockIndex = requiredblock
            End If
        End Sub
        Private Shared Function BZip2Decompress(ByVal Data As Stream, ByVal ExpectedLength As Integer) As Byte()
            Dim output As New MemoryStream
            'BZip2.Decompress(Data, output)
            BZip2Decompress(Data, ExpectedLength)
            Return output.ToArray
        End Function
        Private Shared Function DecompressMulti(ByVal Input As Byte(), ByVal OutputLength As Integer) As Byte()
            Dim sinput As Stream = New MemoryStream(Input)
            Dim comptype As Byte = sinput.ReadByte

            'BZip2
            If ((comptype And 16) <> 0) Then
                Dim result As Byte() = BZip2Decompress(sinput, OutputLength)
                comptype = CByte((comptype And 239))
                If (comptype = 0) Then
                    Return result
                End If
                sinput = New MemoryStream(result)
            End If

            'PKLib
            If ((comptype And 8) <> 0) Then
                Dim result As Byte() = PKDecompress(sinput, OutputLength)
                comptype = CByte((comptype And 247))
                If (comptype = 0) Then
                    Return result
                End If
                sinput = New MemoryStream(result)
            End If

            'ZLib
            If ((comptype And 2) <> 0) Then
                Dim result As Byte() = ZlibDecompress(sinput, OutputLength)
                comptype = CByte((comptype And 253))
                If (comptype = 0) Then
                    Return result
                End If
                sinput = New MemoryStream(result)
            End If

            If ((comptype And 1) <> 0) Then
                'Dim result As Byte() = MpqHuffman.Decompress(sinput)
                'comptype = CByte((comptype And 254))
                'If (comptype = 0) Then
                '    Return result
                'End If
                'sinput = New MemoryStream(result)
            End If
            If ((comptype And 128) <> 0) Then
                'Dim result As Byte() = MpqWavCompression.Decompress(sinput, 2)
                'comptype = CByte((comptype And 127))
                'If (comptype = 0) Then
                '    Return result
                'End If
                'sinput = New MemoryStream(result)
            End If
            If ((comptype And 64) <> 0) Then
                'Dim result As Byte() = MpqWavCompression.Decompress(sinput, 1)
                'comptype = CByte((comptype And 191))
                'If (comptype = 0) Then
                '    Return result
                'End If
                'sinput = New MemoryStream(result)
            End If

            Throw New Exception(String.Format("Unhandled compression flags: 0x{0:X}", comptype))
        End Function
        Public Overrides Sub Flush()
        End Sub
        Private Function LoadBlock(ByVal BlockIndex As Integer, ByVal ExpectedLength As Integer) As Byte()
            Dim offset As Long
            Dim toread As Integer

            If mBlock.IsCompressed Then
                offset = mBlockPositions(BlockIndex)
                toread = CInt((mBlockPositions((BlockIndex + 1)) - offset))
            Else
                offset = CType(BlockIndex, Long) * CType(mBlockSize, Long)
                toread = ExpectedLength
            End If
            offset += mBlock.FilePos

            Dim data As Byte() = New Byte(toread - 1) {}
            SyncLock mStream
                mStream.Seek(CLng(offset), SeekOrigin.Begin)
                mStream.Read(data, 0, toread)
            End SyncLock

            If (mBlock.IsEncrypted AndAlso (mBlock.FileSize > 3)) Then
                If (mSeed1 = 0) Then
                    Throw New Exception("Unable to determine encryption key")
                End If
                MPQArchive.DecryptBlock(data, (mSeed1 + CType(BlockIndex, Long)))
            End If

            If (mBlock.IsCompressed AndAlso (data.Length <> ExpectedLength)) Then
                If (mBlock.Flags And MpqFileFlags.MPQ_CompressedMulti) <> 0 Then
                    data = DecompressMulti(data, ExpectedLength)
                Else
                    data = PKDecompress(New MemoryStream(data), ExpectedLength)
                End If
            End If

            Return data
        End Function
        Private Sub LoadBlockPositions()
            Dim blockposcount As Integer = (CInt((((mBlock.FileSize + mBlockSize) - 1) / CLng(mBlockSize))) + 1)

            mBlockPositions = New Long(blockposcount - 1) {}
            SyncLock mStream
                mStream.Seek(CLng(mBlock.FilePos), SeekOrigin.Begin)
                Dim br As New BinaryReader(mStream)
                Dim i As Integer = 0
                Do While (i < blockposcount)
                    mBlockPositions(i) = Convert.ToInt64(br.ReadUInt32)
                    i += 1
                Loop
            End SyncLock

            Dim blockpossize As Long = CType((blockposcount * 4), Long)
            If (((mBlock.Flags And MpqFileFlags.MPQ_FileHasMetadata) = CType(0, MpqFileFlags)) AndAlso (mBlockPositions(0) <> blockpossize)) Then
                mBlock.Flags = (mBlock.Flags Or MpqFileFlags.MPQ_Encrypted)
            End If

            If mBlock.IsEncrypted Then
                If (mSeed1 = 0) Then
                    mSeed1 = MPQArchive.DetectFileSeed(mBlockPositions, blockpossize)
                    If (mSeed1 = 0) Then
                        Throw New Exception("Unable to determine encyption seed")
                    End If
                End If
                MPQArchive.DecryptBlock(mBlockPositions, mSeed1)
                mSeed1 = (mSeed1 + 1)   'Add 1 because the first block is the offset list
            End If

        End Sub
        Private Shared Function PKDecompress(ByVal Data As Stream, ByVal ExpectedLength As Integer) As Byte()
            'TODO: PKLib
            'Dim pk As New PKLibDecompress(Data)
            'Return pk.Explode(ExpectedLength)
            Return Nothing
        End Function
        Public Overrides Function Read(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer) As Integer
            Dim toread As Integer = Count
            Dim readtotal As Integer = 0

            While (toread > 0)
                Dim readVal As Integer = ReadInternal(Buffer, Offset, toread)
                If (readVal = 0) Then
                    Return readtotal
                End If
                readtotal += readVal
                Offset += readVal
                toread -= readVal
            End While

            Return readtotal
        End Function
        Public Overrides Function ReadByte() As Integer
            If (mPosition >= Length) Then
                Return -1
            End If
            BufferData()

            Dim localposition As Integer = CInt((mPosition Mod CLng(mBlockSize)))
            Me.mPosition += 1
            Return mCurrentData(localposition)
        End Function
        Private Function ReadInternal(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer) As Integer

            'Avoid reading past the contents of the file
            If (mPosition >= Length) Then
                Return 0
            End If

            BufferData()

            Dim localposition As Integer = mPosition Mod mBlockSize
            Dim bytestocopy As Integer = Math.Min((mCurrentData.Length - localposition), Count)
            If (bytestocopy <= 0) Then
                Return 0
            End If

            Array.Copy(mCurrentData, localposition, Buffer, Offset, bytestocopy)

            mPosition += bytestocopy
            Return bytestocopy

        End Function
        Public Overrides Function Seek(ByVal Offset As Long, ByVal Origin As SeekOrigin) As Long
            Dim target As Long

            Select Case Origin
                Case SeekOrigin.Begin
                    target = Offset
                Case SeekOrigin.Current
                    target = (Position + Offset)
                Case SeekOrigin.End
                    target = (Length + Offset)
                Case Else
                    Throw New ArgumentException("Origin", "Invalid SeekOrigin")
            End Select

            If (target < 0) Then
                Throw New ArgumentOutOfRangeException("Attmpted to Seek before the beginning of the stream")
            End If
            If (target >= Me.Length) Then
                Throw New ArgumentOutOfRangeException("Attmpted to Seek beyond the end of the stream")
            End If
            mPosition = target
            Return mPosition
        End Function
        Public Overrides Sub SetLength(ByVal Value As Long)
            Throw New NotSupportedException("SetLength is not supported")
        End Sub
        Public Overrides Sub Write(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer)
            Throw New NotSupportedException("Writing is not supported")
        End Sub
        Private Shared Function ZlibDecompress(ByVal Data As Stream, ByVal ExpectedLength As Integer) As Byte()
            'This assumes that Zlib won't be used in combination with another compression type

            Dim Output As Byte() = New Byte(ExpectedLength - 1) {}
            Dim s As Stream = New InflaterInputStream(Data)
            Dim Offset As Integer = 0
            Do While True
                Dim size As Integer = s.Read(Output, Offset, ExpectedLength)
                If (size = 0) Then
                    Return Output
                End If
                Offset = (Offset + size)
            Loop

            Return New Byte() {}
        End Function

        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return True
            End Get
        End Property
        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return True
            End Get
        End Property
        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property Length() As Long
            Get
                Return CLng(Me.mBlock.FileSize)
            End Get
        End Property
        Public Overrides Property Position() As Long
            Get
                Return mPosition
            End Get
            Set(ByVal value As Long)
                Seek(value, SeekOrigin.Begin)
            End Set
        End Property

        Private mBlock As MpqBlock
        Private mBlockPositions As Long()
        Private mBlockSize As Integer
        Private mCurrentBlockIndex As Integer = -1
        Private mCurrentData As Byte()
        Private mPosition As Long
        Private mSeed1 As Long
        Private mStream As Stream
    End Class

    <CLSCompliant(False)> _
    Public Enum MpqFileFlags As Long
        MPQ_Changed = 1                     '&H00000001
        MPQ_Protected = 2                   '&H00000002
        MPQ_CompressedPK = 256              '&H00000100
        MPQ_CompressedMulti = 512           '&H00000200
        MPQ_Compressed = 65280              '&H0000FF00
        MPQ_Encrypted = 65536               '&H00010000
        MPQ_FixSeed = 131072                '&H00020000
        MPQ_SingleUnit = 16777216           '&H01000000
        MPQ_Unknown_02000000 = 33554432     '&H02000000 - The file is only 1 byte long and its name is a hash
        MPQ_FileHasMetadata = 67108864      '&H04000000 - Indicates the file has associted metadata.
        MPQ_Exists = 2147483648             '&H80000000
    End Enum

End Namespace
