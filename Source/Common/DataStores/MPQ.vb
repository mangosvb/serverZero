'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getmangos.eu>
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
Imports System.IO
Imports System.ComponentModel
Imports ICSharpCode.SharpZipLib.BZip2
Imports ICSharpCode.SharpZipLib.Zip.Compression.Streams

Namespace MPQ

    Public Class MPQArchive
        Implements IDisposable


        Shared Sub New()
            sStormBuffer = BuildStormBuffer()
        End Sub
        Public Sub New(ByVal SourceStream As Stream)
            _mStream = SourceStream
            Init()
        End Sub
        Public Sub New(ByVal Filename As String)
            _mStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read)
            Init()
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                If (Not _mStream Is Nothing) Then
                    _mStream.Close()
                End If
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

        Private Sub Init()
            If Not LocateMpqHeader() Then
                Throw New Exception("Unable to find MPQ header")
            End If

            Dim br As New BinaryReader(_mStream)
            _mBlockSize = (&H200 << _mHeader.BlockSize)

            'Load hash table
            _mStream.Seek(_mHeader.HashTablePos, SeekOrigin.Begin)
            Dim hashdata As Byte() = br.ReadBytes(_mHeader.HashTableSize * MpqHash.Size)
            DecryptTable(hashdata, "(hash table)")

            Dim br2 As New BinaryReader(New MemoryStream(hashdata))
            ReDim _mHashes(_mHeader.HashTableSize - 1)

            For i As Integer = 0 To _mHeader.HashTableSize - 1
                _mHashes(i) = New MpqHash(br2)
            Next

            'Load block table
            _mStream.Seek(_mHeader.BlockTablePos, SeekOrigin.Begin)
            Dim blockdata As Byte() = br.ReadBytes(_mHeader.BlockTableSize * MpqBlock.Size)
            DecryptTable(blockdata, "(block table)")

            br2 = New BinaryReader(New MemoryStream(blockdata))
            ReDim _mBlocks(_mHeader.BlockTableSize - 1)

            For i As Integer = 0 To _mHeader.BlockTableSize - 1
                _mBlocks(i) = New MpqBlock(br2, _mHeaderOffset)
            Next

        End Sub
        Private Function LocateMpqHeader() As Boolean
            Dim br As New BinaryReader(_mStream)

            'In .mpq files the header will be at the start of the file
            'In .exe files, it will be at a multiple of 0x200
            For i As Long = 0 To (_mStream.Length - MpqHeaderSize) - 1 Step &H200
                _mStream.Seek(i, SeekOrigin.Begin)
                _mHeader = New MpqHeader(br)
                If (_mHeader.ID = MpqId) Then
                    _mHeaderOffset = i
                    _mHeader.HashTablePos += _mHeaderOffset
                    _mHeader.BlockTablePos += _mHeaderOffset
                    If (_mHeader.DataOffset = &H6D9E4B86) Then
                        'then this is a protected archive
                        _mHeader.DataOffset = (MpqHeaderSize + i)
                    End If
                    Return True
                End If
            Next

            Return False
        End Function
        Public Function OpenFile(ByVal filename As String) As MpqStream
            Dim hash As MpqHash = GetHashEntry(filename)
            Dim blockIndex As Long = hash.BlockIndex

            If (blockIndex = UINT32_MAX) Then
                Throw New FileNotFoundException(("File not found: " & filename))
            End If

            Dim block As MpqBlock = _mBlocks(blockIndex)
            Return New MpqStream(Me, block)
        End Function
        Public Function FileExists(ByVal filename As String) As Boolean
            Dim hash As MpqHash = GetHashEntry(filename)
            Return (hash.BlockIndex <> UINT32_MAX)
        End Function

        Friend ReadOnly Property BaseStream() As Stream
            Get
                Return _mStream
            End Get
        End Property
        Friend ReadOnly Property BlockSize() As Integer
            Get
                Return _mBlockSize
            End Get
        End Property

        Private Function GetHashEntry(ByVal filename As String) As MpqHash
            Dim index As Long = HashString(filename, 0)
            index = (index And (_mHeader.HashTableSize - 1))
            Dim name1 As Long = HashString(filename, &H100)
            Dim name2 As Long = HashString(filename, &H200)

            Dim i As Long = index
            Do While (i < _mHashes.Length)
                Dim hash As MpqHash = _mHashes(i)
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

            For Each objCharacter As Char In Input
                Dim val As Long = Asc(Char.ToUpper(objCharacter))

                seed1 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes((sStormBuffer(Offset + val) Xor (seed1 + seed2) And &HFFFFFFFF)), 0))
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
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(seed2 + sStormBuffer(&H400 + (Seed1 And &HFF))), 0))
                Dim result As Long = Convert.ToInt64(BitConverter.ToUInt32(Data, i))
                result = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(result Xor (Seed1 + seed2)), 0))

                Seed1 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes((((Not Seed1) << 21) + &H11111111) Or (Seed1 >> 11)), 0))
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((result + seed2) + (seed2 << 5)) + 3), 0))
                If BitConverter.IsLittleEndian Then
                    Data(i) = result And &HFF
                    Data(i + 1) = (result >> 8) And &HFF
                    Data(i + 2) = (result >> 16) And &HFF
                    Data(i + 3) = (result >> 24) And &HFF
                Else
                    Data(i + 3) = result And &HFF
                    Data(i + 2) = (result >> 8) And &HFF
                    Data(i + 1) = (result >> 16) And &HFF
                    Data(i) = (result >> 24) And &HFF
                End If

                i += 4
            Loop

        End Sub
        Friend Shared Sub DecryptBlock(ByVal Data As Long(), ByVal Seed1 As Long)
            Dim seed2 As Long = &HEEEEEEEE

            Dim i As Integer = 0
            Do While (i < Data.Length)
                seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(seed2 + sStormBuffer(&H400 + (Seed1 And &HFF))), 0))
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
                Dim seed1 As Long = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(temp - sStormBuffer((&H400 + i))), 0))
                Dim seed2 As Long = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(&HEEEEEEEE + sStormBuffer(&H400 + (seed1 And &HFF))), 0))
                Dim result As Long = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(value0 Xor (seed1 + seed2)), 0))

                If (result = Decrypted) Then
                    'Test this result against the 2nd value

                    Dim saveSeed As Long = seed1
                    seed1 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((Not seed1 << 21) + &H11111111) Or (seed1 >> 11)), 0))
                    seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(((result + seed2) + (seed2 << 5)) + 3), 0))
                    seed2 = Convert.ToInt64(BitConverter.ToUInt32(BitConverter.GetBytes(seed2 + sStormBuffer(&H400 + (seed1 And &HFF))), 0))
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

        <Description("Gets or sets the external list file. This setting overrides any list file contained in the archive")>
        Public Property ExternalListFile() As String
            Get
                Return _ExternalListFile
            End Get
            Set(ByVal value As String)
                _ExternalListFile = value
            End Set
        End Property
        Private _ExternalListFile As String = Nothing

        <Description("Returns a collection of file infos for the archive. The archive must contain ""ListFileName"" (default ""(listfile)"") for this to work.")>
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
                            Dim block As MpqBlock = _mBlocks(hash.BlockIndex)

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

        Private _mBlocks() As MpqBlock
        Private _mBlockSize As Integer
        Private _mHashes() As MpqHash
        Private _mHeader As MpqHeader
        Private _mHeaderOffset As Long
        Private _mStream As Stream
        Private Shared sStormBuffer As Long()

    End Class

    <StructLayout(LayoutKind.Sequential)>
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

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure MpqHash
        Public Sub New(ByVal br As BinaryReader)
            Name1 = Convert.ToInt64(br.ReadUInt32)
            Name2 = Convert.ToInt64(br.ReadUInt32)
            Locale = Convert.ToInt64(br.ReadUInt32)
            BlockIndex = Convert.ToInt64(br.ReadUInt32)
        End Sub

        Public ReadOnly Property IsValid() As Boolean
            Get
                Return ((Name1 <> UINT32_MAX) AndAlso (Name2 <> UINT32_MAX))
            End Get
        End Property

        Public BlockIndex As Long
        Public Locale As Long
        Public Name1 As Long
        Public Name2 As Long
        Public Const Size As Long = 16
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure MpqHeader
        Public ArchiveSize As Long
        Public BlockSize As Integer
        Public BlockTablePos As Long
        Public BlockTableSize As Long
        Public DataOffset As Long
        Public HashTablePos As Long
        Public HashTableSize As Long
        Public ID As Long
        Public Offs0C As Integer

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

    End Structure

    'A Stream based class for reading a file from an MPQ file
    Public Class MpqStream
        Inherits Stream

        Private _mBlock As MpqBlock
        Private _mBlockPositions As Long()
        Private _mBlockSize As Integer
        Private _mCurrentBlockIndex As Integer = -1
        Private _mCurrentData As Byte()
        Private _mPosition As Long
        Private _mSeed1 As Long
        Private _mStream As Stream

        ''' <summary>
        ''' Initializes a new instance of the <see cref="MpqStream" /> class.
        ''' </summary>
        ''' <param name="file">The file.</param>
        ''' <param name="block">The block.</param>
        Friend Sub New(ByVal file As MPQArchive, ByVal block As MpqBlock)
            _mCurrentBlockIndex = -1
            _mBlock = block
            _mStream = file.BaseStream
            _mBlockSize = file.BlockSize
            If _mBlock.IsCompressed Then
                LoadBlockPositions()
            End If
        End Sub

        ''' <summary>
        ''' Buffers the data.
        ''' </summary>
        ''' <returns></returns>
        Private Sub BufferData()
            Dim requiredblock As Integer = _mPosition \ _mBlockSize
            If (requiredblock <> _mCurrentBlockIndex) Then
                Dim expectedlength As Integer = Math.Min(Length - (requiredblock * _mBlockSize), _mBlockSize)
                _mCurrentData = LoadBlock(requiredblock, expectedlength)
                _mCurrentBlockIndex = requiredblock
            End If
        End Sub

        ''' <summary>
        ''' decompress the  Bzip2 memorystream. TODO: WTF is this doing, calling itself recursively - surely a guaranteed stack overflow
        ''' </summary>
        ''' <param name="data">The data.</param>
        ''' <param name="expectedLength">The expected length.</param>
        ''' <returns></returns>
        Private Shared Function BZip2Decompress(ByVal data As Stream, ByVal expectedLength As Integer) As Byte()
            'TODO: Original code here
            'Dim output As New MemoryStream
            ''BZip2.Decompress(data, output, True)
            'BZip2Decompress(data, expectedLength)
            'Return output.ToArray

            'TODO: New Code here, apart from expectedLength - this seems more reasonable
            Dim output As New MemoryStream
            BZip2.Decompress(data, output, True)
            'BZip2Decompress(data, expectedLength)
            Return output.ToArray
        End Function

        ''' <summary>
        ''' Decompresses the memorystream chunk.
        ''' </summary>
        ''' <param name="inputValue">The input value.</param>
        ''' <param name="outputLength">Length of the output.</param>
        ''' <returns></returns>
        Private Shared Function DecompressMulti(ByVal inputValue As Byte(), ByVal outputLength As Integer) As Byte()
            Dim sinput As Stream = New MemoryStream(inputValue)
            Dim comptype As Byte = sinput.ReadByte

            'BZip2
            If ((comptype And 16) <> 0) Then
                Dim result As Byte() = BZip2Decompress(sinput, outputLength)
                comptype = comptype And 239
                If (comptype = 0) Then
                    Return result
                End If
                sinput = New MemoryStream(result)
            End If

            'PKLib
            If ((comptype And 8) <> 0) Then
                Dim result As Byte() = PKDecompress(sinput, outputLength)
                comptype = comptype And 247
                If (comptype = 0) Then
                    Return result
                End If
                sinput = New MemoryStream(result)
            End If

            'ZLib
            If ((comptype And 2) <> 0) Then
                Dim result As Byte() = ZlibDecompress(sinput, outputLength)
                comptype = comptype And 253
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

            If _mBlock.IsCompressed Then
                offset = _mBlockPositions(BlockIndex)
                toread = _mBlockPositions((BlockIndex + 1)) - offset
            Else
                offset = BlockIndex * CType(_mBlockSize, Long)
                toread = ExpectedLength
            End If
            offset += _mBlock.FilePos

            Dim data As Byte() = New Byte(toread - 1) {}
            SyncLock _mStream
                _mStream.Seek(offset, SeekOrigin.Begin)
                _mStream.Read(data, 0, toread)
            End SyncLock

            If (_mBlock.IsEncrypted AndAlso (_mBlock.FileSize > 3)) Then
                If (_mSeed1 = 0) Then
                    Throw New Exception("Unable to determine encryption key")
                End If
                MPQArchive.DecryptBlock(data, (_mSeed1 + BlockIndex))
            End If

            If (_mBlock.IsCompressed AndAlso (data.Length <> ExpectedLength)) Then
                If (_mBlock.Flags And MpqFileFlags.MPQ_CompressedMulti) <> 0 Then
                    data = DecompressMulti(data, ExpectedLength)
                Else
                    data = PKDecompress(New MemoryStream(data), ExpectedLength)
                End If
            End If

            Return data
        End Function
        Private Sub LoadBlockPositions()
            Dim blockposcount As Integer = (CInt((((_mBlock.FileSize + _mBlockSize) - 1) / _mBlockSize)) + 1)

            _mBlockPositions = (New Long(blockposcount - 1) {})
            SyncLock _mStream
                _mStream.Seek(_mBlock.FilePos, SeekOrigin.Begin)
                Dim br As New BinaryReader(_mStream)
                Dim i As Integer = 0
                Do While (i < blockposcount)
                    _mBlockPositions(i) = Convert.ToInt64(br.ReadUInt32)
                    i += 1
                Loop
            End SyncLock

            Dim blockpossize As Long = blockposcount * 4
            If (((_mBlock.Flags And MpqFileFlags.MPQ_FileHasMetadata) = CType(0, MpqFileFlags)) AndAlso (_mBlockPositions(0) <> blockpossize)) Then
                _mBlock.Flags = (_mBlock.Flags Or MpqFileFlags.MPQ_Encrypted)
            End If

            If _mBlock.IsEncrypted Then
                If (_mSeed1 = 0) Then
                    _mSeed1 = MPQArchive.DetectFileSeed(_mBlockPositions, blockpossize)
                    If (_mSeed1 = 0) Then
                        Throw New Exception("Unable to determine encyption seed")
                    End If
                End If
                MPQArchive.DecryptBlock(_mBlockPositions, _mSeed1)
                _mSeed1 = (_mSeed1 + 1)   'Add 1 because the first block is the offset list
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
            If (_mPosition >= Length) Then
                Return -1
            End If
            BufferData()

            Dim localposition As Integer = _mPosition Mod _mBlockSize
            _mPosition += 1
            Return _mCurrentData(localposition)
        End Function
        Private Function ReadInternal(ByVal Buffer As Byte(), ByVal Offset As Integer, ByVal Count As Integer) As Integer

            'Avoid reading past the contents of the file
            If (_mPosition >= Length) Then
                Return 0
            End If

            BufferData()

            Dim localposition As Integer = _mPosition Mod _mBlockSize
            Dim bytestocopy As Integer = Math.Min((_mCurrentData.Length - localposition), Count)
            If (bytestocopy <= 0) Then
                Return 0
            End If

            Array.Copy(_mCurrentData, localposition, Buffer, Offset, bytestocopy)

            _mPosition += bytestocopy
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
            If (target >= Length) Then
                Throw New ArgumentOutOfRangeException("Attmpted to Seek beyond the end of the stream")
            End If
            _mPosition = target
            Return _mPosition
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
                Return _mBlock.FileSize
            End Get
        End Property
        Public Overrides Property Position() As Long
            Get
                Return _mPosition
            End Get
            Set(ByVal value As Long)
                Seek(value, SeekOrigin.Begin)
            End Set
        End Property

    End Class
End Namespace
