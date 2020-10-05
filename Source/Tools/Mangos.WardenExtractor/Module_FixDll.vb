'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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

Public Module Module_FixDll

#Region "FixDll"
    'http://www.openrce.org/reference_library/files/reference/PE%20Format.pdf
    Public Sub FixNormalDll(ByRef Data() As Byte)
        Dim ms As New MemoryStream
        Dim bw As New BinaryWriter(ms)

        Dim ms2 As New MemoryStream(Data)
        Dim br As New BinaryReader(ms2)

        Dim Sections As New List(Of Section)
        Dim CurrentPosition As Integer = &H400
        Dim ImportAddress As Integer = br.ReadInt32()
        Dim ImportUnk As Integer = br.ReadInt32()
        Dim ExportAddress As Integer = br.ReadInt32()
        Dim ExportUnk As Integer = br.ReadInt32()

        br.BaseStream.Position = 36
        Dim numSections As Integer = br.ReadInt32()
        Dim BufferPosition As Integer = &H4E
        Dim i As Integer = 40
        Dim j As Integer = 0
        Do
            Dim virtualaddress As Integer = br.ReadInt32()
            Dim len As Integer = br.ReadInt32()
            Dim type As Integer = br.ReadInt32()
            Dim name As String = ""
            Dim characteristics As Integer = 0

            If type = 2 Then
                name = ".rdata"
                characteristics = &H40000040I
                '40000000 + 40 (can be read, initialized data)
            ElseIf type = 4 Then
                name = ".data"
                characteristics = &HC0000040I
                '80000000 + 40000000 + 40 (Can be written to, can be read, initialized data)
            ElseIf type = 32 Then
                name = ".text"
                characteristics = &H60000020I
                '40000000 + 20000000 + 20 (can be read, can be executed as code, contains executable code)
            Else
                'Can't?!
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[ERROR] Invalid section in dll.")
                Exit Sub
            End If

            Dim tmpPos As Long = br.BaseStream.Position
            Dim newSection As New Section
            newSection.Name = name
            newSection.Address = virtualaddress
            newSection.Characteristics = characteristics
            newSection.Size = len
            newSection.Data = GetSectionData(br, BufferPosition, name, virtualaddress, len, characteristics)
            newSection.Pointer = CurrentPosition
            If type = 32 Then
                newSection.RawSize = &H6E00
            ElseIf type = 4 Then
                newSection.RawSize = &H200
            ElseIf type = 2 Then
                newSection.RawSize = &H800
            Else
                newSection.RawSize = len
            End If
            Sections.Add(newSection)
            br.BaseStream.Position = tmpPos

            BufferPosition += len
            CurrentPosition += newSection.RawSize

            j += 1
            i += 12
        Loop While j < numSections

        ms2.Close()
        ms2.Dispose()
        ms2 = Nothing
        br = Nothing

        'TODO: Other headers!
        Dim ExportSection As New Section
        ExportSection.Name = ".edata"
        ExportSection.Address = ExportAddress
        ExportSection.Characteristics = &H40000040
        '40000000 + 40 (can be read, initialized data)
        ExportSection.Size = &H8D
        ExportSection.Data = New Byte() {} 'TODO!!
        ExportSection.Pointer = CurrentPosition
        ExportSection.RawSize = &H200
        CurrentPosition += ExportSection.RawSize
        Sections.Add(ExportSection)

        Dim ImportSection As New Section
        ImportSection.Name = ".idata"
        ImportSection.Address = ImportAddress
        ImportSection.Characteristics = &H42000040
        '40000000 + 2000000 + 40 (can be read, can be discarded, initialized data)
        ImportSection.Size = &H3C
        ImportSection.Data = New Byte() {} 'TODO!!
        ImportSection.Pointer = CurrentPosition
        ImportSection.RawSize = &H200
        CurrentPosition += ImportSection.RawSize
        Sections.Add(ImportSection)

        Dim RelocSection As New Section
        RelocSection.Name = ".reloc"
        RelocSection.Address = &HC000
        RelocSection.Characteristics = &H42000040
        '40000000 + 2000000 + 40 (can be read, can be discarded, initialized data)
        RelocSection.Size = &H2F4
        RelocSection.Data = New Byte() {} 'TODO!!
        RelocSection.Pointer = CurrentPosition
        RelocSection.RawSize = &H400
        CurrentPosition += RelocSection.RawSize
        Sections.Add(RelocSection)

        Dim Null(1024) As Byte
        For i = 0 To Null.Length - 1
            Null(i) = 0
        Next

        'Start Header
        bw.Write(23117I)
        bw.Write(Null, 0, 56)
        bw.Write(64I)
        'IMAGE_NT_HEADERS
        bw.Write(17744I) 'Signature
        '-IMAGE_FILE_HEADER
        bw.Write(&H14CS) 'Machine
        bw.Write(CShort(Sections.Count)) 'Number of sections
        bw.Write(0I) 'Timestamp
        bw.Write(0I) 'Pointer to symbol table
        bw.Write(0I) 'Number of symbols
        bw.Write(224S) 'Optional header size
        bw.Write(&H2102S) 'Characteristics
        '-IMAGE_OPTIONAL_HEADER
        bw.Write(&H10BS) 'Magic
        bw.Write(CByte(7)) 'MajorLinkerVersion
        bw.Write(CByte(10)) 'MinorLinkerVersion
        bw.Write(&H6E00I) 'SizeOfCode
        bw.Write(0I) 'SizeOfInitializedData
        bw.Write(0I) 'SizeOfUninitializedData
        bw.Write(&H0I) 'AddressEntryPoint
        bw.Write(&H1000I) 'BaseOfCode
        bw.Write(&H8000I) 'BaseOfData
        bw.Write(&H400000I) 'ImageBase
        bw.Write(&H1000I) 'SectionAlignment
        bw.Write(&H200I) 'FileAlignment
        bw.Write(4S) 'MajorOperatingSystemVersion
        bw.Write(0S) 'MinorOperatingSystemVersion
        bw.Write(1S) 'MajorImageVersion
        bw.Write(0S) 'MinorImageVersion
        bw.Write(4S) 'MajorSubsystemVersion
        bw.Write(0S) 'MinorSubsystemVersion
        bw.Write(0I) 'Win32VersionValue
        bw.Write(&HD000I) 'SizeOfImage
        bw.Write(&H400I) 'SizeOfHeaders
        bw.Write(0I) 'CheckSum
        bw.Write(2S) 'Subsystem
        bw.Write(0S) 'DllCharacteristics
        bw.Write(&H100000I) 'SizeOfStackReverse
        bw.Write(&H1000I) 'SizeOfStackCommit
        bw.Write(&H100000I) 'SizeOfHeapReverse
        bw.Write(&H1000I) 'SizeOfHeapCommit
        bw.Write(0I) 'LoaderFlags
        bw.Write(&H10I) 'NumberOfRvaAndSizes
        '--IMAGE_DIRECTORY_ENTRY_EXPORT
        bw.Write(&HA000I) 'VirtualAddress
        bw.Write(&H8DI) 'Size
        '--IMAGE_DIRECTORY_ENTRY_IMPORT
        bw.Write(&HB000I) 'VirtualAddress
        bw.Write(&H3CI) 'Size
        '--IMAGE_DIRECTORY_ENTRY_RESOURCE
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_EXCEPTION
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_SECURITY
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_BASERELOC
        bw.Write(&HC000I) 'VirtualAddress
        bw.Write(&H2F4I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_DEBUG
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_COPYRIGHT
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_GLOBALPTR
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_TLS
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_IAT
        bw.Write(&H8000I) 'VirtualAddress
        bw.Write(&H80I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTION
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size
        '--IMAGE_DIRECTORY_ENTRY_RESERVED
        bw.Write(0I) 'VirtualAddress
        bw.Write(0I) 'Size

        'IMAGE_SECTION_HEADER
        For Each tmpSection As Section In Sections
            bw.Write(GetName(tmpSection.Name)) 'Name (8 bytes, null-padded)
            bw.Write(tmpSection.Size) 'VirtualSize
            bw.Write(tmpSection.Address) 'VirtualAddress
            bw.Write(tmpSection.RawSize) 'SizeOfRawData
            bw.Write(tmpSection.Pointer) 'PointerToRawData
            bw.Write(0I) 'PointerToRelocations
            bw.Write(0I) 'PointerToLinenumbers
            bw.Write(0S) 'NumberOfRelocations
            bw.Write(0S) 'NumberOfLinenumbers
            bw.Write(tmpSection.Characteristics) 'Characteristics
        Next
        'End Header

        'Section data
        For Each tmpSection As Section In Sections
            bw.BaseStream.Position = tmpSection.Pointer
            bw.Write(tmpSection.Data, 0, tmpSection.Data.Length)
        Next

        'To make sure the file gets this long
        If bw.BaseStream.Position <> CurrentPosition Then
            bw.BaseStream.Position = CLng(CurrentPosition - 1)
            bw.Write(CByte(0))
        End If

        Dim DllData() As Byte = ms.ToArray

        Data = DllData
        ms.Close()
        ms.Dispose()
        ms = Nothing
        bw = Nothing
    End Sub

    Public Sub FixWardenDll(ByRef Data() As Byte)

    End Sub

    Public Function GetName(ByVal Name As String) As ULong
        Dim bBytes(7) As Byte
        For i As Integer = 0 To 7
            If (i + 1) > Name.Length Then Exit For
            bBytes(i) = Asc(Name(i))
        Next
        Return BitConverter.ToUInt64(bBytes, 0)
    End Function

    Public Function GetSectionData(ByRef br As BinaryReader, ByVal Position As Integer, ByVal Name As String, ByVal Address As Integer, ByVal Length As Integer, ByVal characteristics As Integer) As Byte()
        br.BaseStream.Position = CLng(Position)
        Dim tmpBytes() As Byte = br.ReadBytes(Length)

        If (characteristics And &H20) Then
            Dim InstructionPos As Integer = 0
            Dim InstrPos As Integer = 0
            Do
                Try
                    InstrPos = InstructionPos
                    Dim Instr As Instruction = ParseInstuction(tmpBytes, InstructionPos)
                    If Instr Is Nothing Then Continue Do

                    If Instr.Opcode = &HA1 Then
                        Instr.DisplacementData(2) = &H40
                        Instr.Place(tmpBytes, InstrPos)
                    ElseIf Instr.Opcode = &HC6 AndAlso Instr.ModRmData = &H5 Then
                        Instr.DisplacementData(2) = &H40
                        Instr.Place(tmpBytes, InstrPos)
                    ElseIf Instr.Opcode = &H80 Then
                        Instr.DisplacementData(2) = &H40
                        Instr.Place(tmpBytes, InstrPos)
                    ElseIf Instr.Opcode = &HFF AndAlso (Instr.ModRmData = &H15 OrElse Instr.ModRmData = &H25) Then
                        Instr.DisplacementData(2) = &H40
                        Instr.Place(tmpBytes, InstrPos)
                    ElseIf Instr.Opcode = &H8B AndAlso Instr.ModRmData = &HD Then
                        Instr.DisplacementData(2) = &H40
                        Instr.Place(tmpBytes, InstrPos)
                    ElseIf Instr.Opcode = &HC7 AndAlso Instr.ModRmData = &H0 Then
                        Instr.ImmediateData(2) = &H40
                        Instr.Place(tmpBytes, InstrPos)
                    ElseIf Instr.Opcode = &HC7 AndAlso Instr.ModRmData = &H5 Then
                        Instr.DisplacementData(2) = &H40

                        If Instr.ImmediateData(0) <> &HFF Then
                            Dim tmpBytes2() As Byte = tmpBytes
                            ReDim Preserve tmpBytes(tmpBytes.Length + 1)
                            Array.Copy(tmpBytes2, InstructionPos - 2, tmpBytes, InstructionPos, tmpBytes2.Length - (InstructionPos - 2))
                            Instr.ImmediateData(0) = 0
                            Instr.ImmediateData(1) = 0
                            Instr.ImmediateData(2) = 0
                            Instr.ImmediateData(3) = 0
                        End If

                        Instr.Place(tmpBytes, InstrPos)
                    ElseIf InstrPos >= (&HB60 - &H400) Then
                        'MsgBox(Hex(Instr.Opcode) & " : " & Instr.ToString)
                    End If
                Catch
                    Exit Do
                End Try
            Loop While InstructionPos < tmpBytes.Length
        End If

        Return tmpBytes
    End Function

    Public Class Section
        Public Name As String
        Public Address As Integer
        Public Characteristics As Integer
        Public Size As Integer
        Public Data() As Byte

        Public Pointer As Integer
        Public RawSize As Integer
    End Class
#End Region

#Region "IA32 Parser"
    Private Function ParseInstuction(ByRef Data() As Byte, ByRef Position As Integer) As Instruction
        Dim bOpcode As Byte = 0

        Dim newInstruction As New Instruction

        If Data(Position) = 0 Then
            Position += 1
            Return Nothing
        End If

        Dim StartAt As Integer = Position
        For i As Integer = 0 To 3
            If Not IsPrefix(Data(Position)) Then Exit For
            newInstruction.Prefix(i) = Data(Position)
            Position += 1
        Next
        newInstruction.InitPrefix()

        bOpcode = Data(Position)
        Position += 1
        If bOpcode = &HF Then
            newInstruction.Opcode = Data(Position)
            newInstruction.Two_Bytes = True
            Position += 1
        Else
            newInstruction.Opcode = bOpcode
        End If

        newInstruction.ExtendedOpcode = IsExtendedOpcode(newInstruction)
        newInstruction.ModrefReq = IsModrefRequired(newInstruction)

        newInstruction.DisplacementSize = FindDisplacementDataSize(newInstruction)
        newInstruction.ImmediateSize = FindImmediateDataSize(newInstruction)

        If newInstruction.ModrefReq Then
            newInstruction.ModRmData = Data(Position)

            If (newInstruction.ModRmData And MOD_FIELD_MASK) <> MOD_FIELD_NO_SIB AndAlso (newInstruction.ModRmData And RM_FIELD_MASK) = RM_FIELD_SIB Then
                Position += 1
                newInstruction.SibData = Data(Position)
                newInstruction.SibAccompanies = True
            End If

            newInstruction.DisplacementSize = FindDisplacementDataSize2(newInstruction)
            newInstruction.ImmediateSize = FindImmediateDataSize(newInstruction)

            Position += 1
        End If

        If newInstruction.DisplacementSize > 0 Then
            For i As Integer = 0 To newInstruction.DisplacementSize - 1
                newInstruction.DisplacementData(i) = Data(Position + i)
            Next

            Position += newInstruction.DisplacementSize
        End If

        If newInstruction.ImmediateSize > 0 Then
            For i As Integer = 0 To newInstruction.ImmediateSize - 1
                newInstruction.ImmediateData(i) = Data(Position + i)
            Next

            Position += newInstruction.ImmediateSize
        End If

        newInstruction.OrigSize = Position - StartAt

        Return newInstruction
    End Function

    Private Function IsPrefix(ByVal bByte As Byte) As Boolean
        Select Case bByte
            Case &HF3, &HF2, &HF0, &H2E, &H36, &H3E, &H26, &H64, &H65, &H66, &H67
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Function IsExtendedOpcode(ByRef Instr As Instruction) As Integer
        If Not Instr.Two_Bytes Then
            Select Case Instr.Opcode
                Case &H80, &H81, &H82, &H83
                    Return 1
                Case &HC0, &HC1, &HD0, &HD1, &HD2, &HD3
                    Return 2
                Case &HF6, &HF7
                    Return 3
                Case &HFE
                    Return 4
                Case &HFF
                    Return 5
                Case Else
                    Return 0
            End Select
        Else
            Select Case Instr.Opcode
                Case &H0
                    Return 6
                Case &H1
                    Return 7
                Case &HC7
                    Return 9
                Case &H71, &H72, &H73
                    Return &HA
                Case Else
                    Return 0
            End Select
        End If
    End Function

    Private Function IsModrefRequired(ByRef Instr As Instruction) As Boolean
        If Instr.Two_Bytes Then
            Return IsBitSetInTable(Instr.Opcode, TWO_BYTE_OPCODE_MODREF_REQUIREMENT)
        Else
            Return IsBitSetInTable(Instr.Opcode, ONE_BYTE_OPCODE_MODREF_REQUIREMENT)
        End If
    End Function

    Private Function FindDisplacementDataSize(ByRef Instr As Instruction) As Integer
        Dim address_size_is_32 As Boolean = False

        If Instr.EffectiveAddressSize() = 32 Then address_size_is_32 = True

        If Instr.Opcode = &H9A OrElse Instr.Opcode = &HEA Then
            Instr.FullDisplacement = True
            Return If(address_size_is_32, 6, 4)
        End If

        If Not Instr.Two_Bytes Then
            If IsBitSetInTable(Instr.Opcode, ONE_BYTE_OPCODE_DISPLACEMENT_SIZE_BYTE) Then
                Return 1
            End If

            If IsBitSetInTable(Instr.Opcode, ONE_BYTE_OPCODE_DISPLACEMENT_SIZE_VARIABLE) Then
                Return If(address_size_is_32, 4, 2)
            End If
        Else
            If IsBitSetInTable(Instr.Opcode, TWO_BYTE_OPCODE_DISPLACEMENT_SIZE_BYTE) Then
                Return 1
            End If

            If IsBitSetInTable(Instr.Opcode, TWO_BYTE_OPCODE_DISPLACEMENT_SIZE_VARIABLE) Then
                Return If(address_size_is_32, 4, 2)
            End If
        End If

        Return 0
    End Function

    Private Function FindImmediateDataSize(ByRef Instr As Instruction) As Integer
        Dim operand_size_32 As Boolean = False

        If Instr.EffectiveOperandSize() = 32 Then operand_size_32 = True

        If Instr.Opcode = &HC2 OrElse Instr.Opcode = &HCA Then
            Return 2
        ElseIf Instr.Opcode = &HC8 Then
            Return 3
        End If

        If Not Instr.Two_Bytes Then
            If IsBitSetInTable(Instr.Opcode, ONE_BYTE_OPCODE_IMMEDIATE_SIZE_BYTE) Then
                Return 1
            End If

            If IsBitSetInTable(Instr.Opcode, ONE_BYTE_OPCODE_IMMEDIATE_SIZE_VARIABLE) Then
                Return If(operand_size_32, 4, 2)
            End If
        Else
            If IsBitSetInTable(Instr.Opcode, TWO_BYTE_OPCODE_IMMEDIATE_SIZE_BYTE) Then
                Return 1
            End If

            If IsBitSetInTable(Instr.Opcode, TWO_BYTE_OPCODE_IMMEDIATE_SIZE_VARIABLE) Then
                Return If(operand_size_32, 4, 2)
            End If
        End If

        Return 0
    End Function

    Private Function FindDisplacementDataSize2(ByRef Instr As Instruction) As Integer
        Dim address_size_is_32 As Boolean = False

        Dim DisplacementSize As Integer = 0
        If Instr.EffectiveOperandSize() = 32 Then address_size_is_32 = True

        Select Case (Instr.ModRmData And MOD_FIELD_MASK)
            Case MOD_FIELD_00
                If address_size_is_32 Then
                    If (Instr.ModRmData And RM_FIELD_MASK) = &H5 Then
                        DisplacementSize = 4
                    End If
                Else
                    If (Instr.ModRmData And RM_FIELD_MASK) = &H6 Then
                        DisplacementSize = 2
                    End If
                End If
            Case MOD_FIELD_01
                DisplacementSize = 1
            Case MOD_FIELD_10
                DisplacementSize = If(address_size_is_32, 4, 2)
            Case MOD_FIELD_11
                DisplacementSize = 0
        End Select

        Return DisplacementSize
    End Function

    Private Function FindImmediateDataSize2(ByRef Instr As Instruction) As Integer
        Dim operand_size_32 As Boolean = False

        Dim immediate_size As Integer = 0
        If Instr.EffectiveOperandSize() = 32 Then operand_size_32 = True

        Select Case Instr.ExtendedOpcode
            Case 1
                If Instr.Opcode = &H81 Then
                    immediate_size = If(operand_size_32, 4, 2)
                Else
                    immediate_size = 1
                End If
            Case 2
                If Instr.Opcode = &HC0 OrElse Instr.Opcode = &HC1 Then
                    immediate_size = 1
                End If
            Case 3
                If (Instr.ModRmData And OPCODE_FIELD_MASK) = 0 Then
                    If Instr.Opcode = &HF6 Then
                        immediate_size = 1
                    ElseIf Instr.Opcode = &HF6 Then
                        immediate_size = If(operand_size_32, 4, 2)
                    Else
                        Return 0
                    End If
                End If
            Case 8
                immediate_size = 1
            Case &HA
                immediate_size = 1
            Case Else
                immediate_size = 0
        End Select

        Return immediate_size
    End Function

    Private Function IsBitSetInTable(ByVal bOpcode As Byte, ByVal Table() As UShort) As Boolean
        Dim row_index As Byte = (&HF0 And bOpcode)
        row_index = (row_index >> 4)
        Dim col_index As Byte = (&HF And bOpcode)

        Dim databits As UShort = Table(row_index)
        Dim the_bit As UInteger

        Select Case col_index
            Case &H0
                the_bit = (databits And &H8000)
            Case &H1
                the_bit = (databits And &H4000)
            Case &H2
                the_bit = (databits And &H2000)
            Case &H3
                the_bit = (databits And &H1000)
            Case &H4
                the_bit = (databits And &H800)
            Case &H5
                the_bit = (databits And &H400)
            Case &H6
                the_bit = (databits And &H200)
            Case &H7
                the_bit = (databits And &H100)
            Case &H8
                the_bit = (databits And &H80)
            Case &H9
                the_bit = (databits And &H40)
            Case &HA
                the_bit = (databits And &H20)
            Case &HB
                the_bit = (databits And &H10)
            Case &HC
                the_bit = (databits And &H8)
            Case &HD
                the_bit = (databits And &H4)
            Case &HE
                the_bit = (databits And &H2)
            Case &HF
                the_bit = (databits And &H1)
            Case Else
                Return False
        End Select

        Return (the_bit > 0)
    End Function

    Public Class Instruction
        Public Prefix() As Byte = {0, 0, 0, 0}
        Public Opcode As Byte = 0
        Public Two_Bytes As Boolean = False
        Public ExtendedOpcode As Integer = 0
        Public ModrefReq As Boolean = False
        Public ModRmData As Byte = 0
        Public SibData As Byte = 0
        Public SibAccompanies As Boolean = False
        Public AddressSizeOverwritten As Boolean = False
        Public OperandSizeOverwritten As Boolean = False
        Public FullDisplacement As Boolean = False
        Public DisplacementSize As Integer = 0
        Public ImmediateSize As Integer = 0

        Public DisplacementData() As Byte = {0, 0, 0, 0, 0, 0}
        Public ImmediateData() As Byte = {0, 0, 0, 0}

        Public OrigSize As Integer = 0

        Public Sub InitPrefix()
            For i As Integer = 0 To 3
                If IsPrefix(Prefix(i)) = False Then Exit For

                If Prefix(i) = &H67 Then
                    AddressSizeOverwritten = True
                ElseIf Prefix(i) = &H66 Then
                    OperandSizeOverwritten = True
                End If
            Next
        End Sub

        Public ReadOnly Property EffectiveAddressSize() As Integer
            Get
                If AddressSizeOverwritten Then Return 16
                Return 32
            End Get
        End Property

        Public ReadOnly Property EffectiveOperandSize() As Integer
            Get
                If OperandSizeOverwritten Then Return 16
                Return 32
            End Get
        End Property

        Public Function GetBytes() As Byte()
            Dim newData As New MemoryStream
            For i As Integer = 0 To 3
                If IsPrefix(Prefix(i)) = False Then Exit For
                newData.WriteByte(Prefix(i))
            Next

            If Two_Bytes Then newData.WriteByte(&HF)
            newData.WriteByte(Opcode)

            If ModrefReq Then
                newData.WriteByte(ModRmData)

                If (ModRmData And MOD_FIELD_MASK) <> MOD_FIELD_NO_SIB AndAlso (ModRmData And RM_FIELD_MASK) = RM_FIELD_SIB Then
                    newData.WriteByte(SibData)
                End If
            End If

            If DisplacementSize > 0 Then
                For i As Integer = 0 To DisplacementSize - 1
                    newData.WriteByte(DisplacementData(i))
                Next
            End If

            If ImmediateSize > 0 Then
                For i As Integer = 0 To ImmediateSize - 1
                    newData.WriteByte(ImmediateData(i))
                Next
            End If

            Return newData.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return BitConverter.ToString(GetBytes).Replace("-", " ")
        End Function

        Public Sub Place(ByRef Data() As Byte, ByVal Index As Integer)
            Dim tmpBytes() As Byte = GetBytes()

            If tmpBytes.Length <> OrigSize Then
                Dim tmpBytes2() As Byte = Data

                If tmpBytes.Length > OrigSize Then ReDim Preserve Data(Data.Length + (tmpBytes.Length - OrigSize) - 1)

                Array.Copy(tmpBytes2, Index + OrigSize, Data, Index + tmpBytes.Length, tmpBytes2.Length - (Index + OrigSize))

                If tmpBytes.Length < OrigSize Then ReDim Preserve Data(Data.Length - (OrigSize - tmpBytes.Length) - 1)
            End If

            Array.Copy(tmpBytes, 0, Data, Index, tmpBytes.Length)
        End Sub
    End Class

#Region "Consts"
    Public ONE_BYTE_OPCODE_MODREF_REQUIREMENT() As UShort = {&HF0F0, &HF0F0, &HF0F0, &HF0F0, &H0, &H0, &H3050, &H0, &HFFFF, &H0, &H0, &H0, &HCF00, &HF000, &H0, &H303}
    Public TWO_BYTE_OPCODE_MODREF_REQUIREMENT() As UShort = {&HB000, &H0, &HF000, &HF000, &HFFFF, &H0, &HFFF3, &H7E03, &H0, &HFFFF, &H1C1D, &HFF3F, &HC100, &H74DD, &H64DD, &H74EE}
    Public ONE_BYTE_OPCODE_DISPLACEMENT_SIZE_BYTE() As UShort = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &HFFFF, &H0, &H0, &HA000, &H0, &H0, &H0, &HF010, &H0}
    Public ONE_BYTE_OPCODE_DISPLACEMENT_SIZE_VARIABLE() As UShort = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H5000, &H0, &H0, &H0, &HC0, &H0}
    Public TWO_BYTE_OPCODE_DISPLACEMENT_SIZE_BYTE() As UShort = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}
    Public TWO_BYTE_OPCODE_DISPLACEMENT_SIZE_VARIABLE() As UShort = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &HFFFF, &H0, &H0, &H0, &H0, &H0, &H0, &H0}
    Public ONE_BYTE_OPCODE_IMMEDIATE_SIZE_BYTE() As UShort = {&H808, &H808, &H808, &H808, &H0, &H0, &H30, &H0, &H0, &H0, &H80, &HFF00, &H200, &H0, &HF00, &H0}
    Public ONE_BYTE_OPCODE_IMMEDIATE_SIZE_VARIABLE() As UShort = {&H404, &H404, &H404, &H404, &H0, &H0, &HC0, &H0, &H0, &H0, &H40, &HFF, &H100, &H0, &H0, &H0}
    Public TWO_BYTE_OPCODE_IMMEDIATE_SIZE_BYTE() As UShort = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H808, &H0, &H0, &H0, &H0, &H0}
    Public TWO_BYTE_OPCODE_IMMEDIATE_SIZE_VARIABLE() As UShort = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}

    Public Const MOD_FIELD_NO_SIB As Byte = &HC0
    Public Const MOD_FIELD_MASK As Byte = &HC0
    Public Const RM_FIELD_SIB As Byte = &H4
    Public Const RM_FIELD_MASK As Byte = &H7
    Public Const MOD_FIELD_00 As Byte = &H0
    Public Const MOD_FIELD_01 As Byte = &H40
    Public Const MOD_FIELD_10 As Byte = &H80
    Public Const MOD_FIELD_11 As Byte = &HC0
    Public Const OPCODE_FIELD_MASK As Byte = &H38
#End Region
#End Region

End Module
