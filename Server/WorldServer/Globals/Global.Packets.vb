'
' Copyright (C) 2013 - 2014 getMaNGOS <http://www.getmangos.eu>
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


Public Module Packets
    ''' <summary>
    ''' Dumps the packet.
    ''' </summary>
    ''' <param name="data">The data.</param>
    ''' <param name="client">The client.</param>
    ''' <param name="start">The start.</param>
    ''' <returns></returns>
    Public Sub DumpPacket(ByVal data() As Byte, Optional ByRef client As ClientClass = Nothing, Optional ByVal start As Integer = 0)
        '#If DEBUG Then
        Dim j As Integer
        Dim buffer As String = ""
        Try
            If client Is Nothing Then
                buffer = buffer + String.Format("DEBUG: Packet Dump{0}", vbNewLine)
            Else
                buffer = buffer + String.Format("[{0}:{1}] DEBUG: Packet Dump - Length={2}{3}", client.IP, client.Port, data.Length - start, vbNewLine)
            End If

            If (data.Length - start) Mod 16 = 0 Then
                For j = start To data.Length - 1 Step 16
                    buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                    buffer += " |  " & Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
                Next
            Else
                For j = start To data.Length - 1 - 16 Step 16
                    buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                    buffer += " |  " & Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
                Next

                buffer += "|  " & BitConverter.ToString(data, j, (data.Length - start) Mod 16).Replace("-", " ")
                buffer += New String(" ", (16 - (data.Length - start) Mod 16) * 3)
                buffer += " |  " & Text.Encoding.ASCII.GetString(data, j, (data.Length - start) Mod 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?")
                buffer += New String(" ", 16 - (data.Length - start) Mod 16)
                buffer += " |" & vbNewLine
            End If

            Log.WriteLine(LogType.DEBUG, buffer, Nothing)
            '#End If
        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Error dumping packet: {0}{1}", vbNewLine, e.ToString)
        End Try
    End Sub

    Public Class UpdateClass
        Implements IDisposable

        'Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
        'packet.AddInt32(OPERATIONS_COUNT)
        'packet.AddInt8(0)

        Public UpdateMask As BitArray
        Public UpdateData As New Hashtable

        Public Sub New(Optional ByVal max As Integer = FIELD_MASK_SIZE_PLAYER)
            UpdateMask = New BitArray(max, False)
        End Sub

        ''' <summary>
        ''' Sets the update flag.
        ''' </summary>
        ''' <param name="pos">The pos.</param>
        ''' <param name="value">The value.</param>
        ''' <returns></returns>
        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As Integer)
            UpdateMask.Set(pos, True)
            UpdateData(pos) = (value)
        End Sub

        ''' <summary>
        ''' Sets the update flag.
        ''' </summary>
        ''' <param name="pos">The pos.</param>
        ''' <param name="index">The index.</param>
        ''' <param name="value">The value.</param>
        ''' <returns></returns>
        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal index As Integer, ByVal value As Byte)
            UpdateMask.Set(pos, True)
            If UpdateData.ContainsKey(pos) Then
                UpdateData(pos) = (CType(UpdateData(pos), Integer) Or (CInt(value) << (8 * index)))
            Else
                UpdateData(pos) = (CInt(value) << (8 * index))
            End If
        End Sub

        ''' <summary>
        ''' Sets the update flag.
        ''' </summary>
        ''' <param name="pos">The pos.</param>
        ''' <param name="value">The value.</param>
        ''' <returns></returns>
        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As UInteger)
            UpdateMask.Set(pos, True)
            UpdateData(pos) = (value)
        End Sub

        ''' <summary>
        ''' Sets the update flag.
        ''' </summary>
        ''' <param name="pos">The pos.</param>
        ''' <param name="value">The value.</param>
        ''' <returns></returns>
        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As Long)
            UpdateMask.Set(pos, True)
            UpdateMask.Set(pos + 1, True)
            UpdateData(pos) = (CType((value And UInteger.MaxValue), Integer))
            UpdateData(pos + 1) = (CType(((value >> 32) And UInteger.MaxValue), Integer))
        End Sub

        ''' <summary>
        ''' Sets the update flag.
        ''' </summary>
        ''' <param name="pos">The pos.</param>
        ''' <param name="value">The value.</param>
        ''' <returns></returns>
        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As ULong)
            UpdateMask.Set(pos, True)
            UpdateMask.Set(pos + 1, True)
            UpdateData(pos) = (CType((value And UInteger.MaxValue), UInteger))
            UpdateData(pos + 1) = (CType(((value >> 32) And UInteger.MaxValue), UInteger))
        End Sub

        ''' <summary>
        ''' Sets the update flag.
        ''' </summary>
        ''' <param name="pos">The pos.</param>
        ''' <param name="value">The value.</param>
        ''' <returns></returns>
        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As Single)
            UpdateMask.Set(pos, True)
            UpdateData(pos) = (value)
        End Sub

        ''' <summary>
        ''' Adds to packet.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="updateType">Type of the update.</param>
        ''' <param name="updateObject">The update object.</param>
        ''' <returns></returns>
        Public Sub AddToPacket(ByRef packet As PacketClass, ByVal updateType As ObjectUpdateType, ByRef updateObject As CreatureObject)
            packet.AddInt8(updateType)
            packet.AddPackGUID(updateObject.GUID)

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Then
                packet.AddInt8(ObjectTypeID.TYPEID_UNIT)
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_MOVEMENT Then
                'TODO: If creature is moving when this packet is created, send it here with help of movementflags?

                packet.AddInt8(&H70)
                packet.AddInt32(&H800000)  'movementflags
                packet.AddInt32(msTime)
                packet.AddSingle(updateObject.positionX)
                packet.AddSingle(updateObject.positionY)
                packet.AddSingle(updateObject.positionZ)
                packet.AddSingle(updateObject.orientation)
                packet.AddSingle(0)

                packet.AddSingle(CREATURESDatabase(updateObject.ID).WalkSpeed)
                packet.AddSingle(CREATURESDatabase(updateObject.ID).RunSpeed)
                packet.AddSingle(UNIT_NORMAL_SWIM_BACK_SPEED)
                packet.AddSingle(UNIT_NORMAL_SWIM_SPEED)
                packet.AddSingle(UNIT_NORMAL_WALK_BACK_SPEED)
                packet.AddSingle(UNIT_NORMAL_TURN_RATE)

                packet.AddUInt32(1)
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_VALUES Then
                Dim updateCount As Integer = 0
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then updateCount = i
                Next

                packet.AddInt8((updateCount + 32) \ 32)
                packet.AddBitArray(UpdateMask, CType((updateCount + 32) \ 32, Byte) * 4)      'OK Flags are Int32, so to byte -> *4
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then
                        If TypeOf UpdateData(i) Is UInteger Then
                            packet.AddUInt32(UpdateData(i))
                        ElseIf TypeOf UpdateData(i) Is Single Then
                            packet.AddSingle(UpdateData(i))
                        Else
                            packet.AddInt32(UpdateData(i))
                        End If
                    End If
                Next

                UpdateMask.SetAll(False)
            End If

            If TypeOf packet Is UpdatePacketClass Then CType(packet, UpdatePacketClass).UpdatesCount += 1
        End Sub

        ''' <summary>
        ''' Adds to packet.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="updateType">Type of the update.</param>
        ''' <param name="updateObject">The update object.</param>
        ''' <returns></returns>
        Public Sub AddToPacket(ByRef packet As PacketClass, ByVal updateType As ObjectUpdateType, ByRef updateObject As CharacterObject)
            packet.AddInt8(updateType)
            packet.AddPackGUID(updateObject.GUID)

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF Then
                packet.AddInt8(ObjectTypeID.TYPEID_PLAYER)
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF OrElse updateType = ObjectUpdateType.UPDATETYPE_MOVEMENT Then
                Dim flags2 As Integer = updateObject.charMovementFlags And &HFF
                If updateObject.OnTransport IsNot Nothing Then
                    flags2 = flags2 Or MovementFlags.MOVEMENTFLAG_ONTRANSPORT
                End If

                packet.AddInt8(&H70)        'flags
                packet.AddInt32(flags2)     'flags 2
                packet.AddInt32(msTime)
                packet.AddSingle(updateObject.positionX)
                packet.AddSingle(updateObject.positionY)
                packet.AddSingle(updateObject.positionZ)
                packet.AddSingle(updateObject.orientation)

                If (flags2 And MovementFlags.MOVEMENTFLAG_ONTRANSPORT) Then
                    packet.AddUInt64(updateObject.OnTransport.GUID)
                    packet.AddSingle(updateObject.transportX)
                    packet.AddSingle(updateObject.transportY)
                    packet.AddSingle(updateObject.transportZ)
                    packet.AddSingle(updateObject.orientation)
                End If

                packet.AddInt32(0)          'Last fall time

                packet.AddSingle(updateObject.WalkSpeed)
                packet.AddSingle(updateObject.RunSpeed)
                packet.AddSingle(updateObject.RunBackSpeed)
                packet.AddSingle(updateObject.SwimSpeed)
                packet.AddSingle(updateObject.SwimBackSpeed)
                packet.AddSingle(updateObject.TurnRate)

                packet.AddUInt32(GuidLow(updateObject.GUID))
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_VALUES OrElse updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF Then
                Dim updateCount As Integer = 0
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then updateCount = i
                Next

                packet.AddInt8((updateCount + 32) \ 32)
                packet.AddBitArray(UpdateMask, CType((updateCount + 32) \ 32, Byte) * 4)      'OK Flags are Int32, so to byte -> *4
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then
                        If TypeOf UpdateData(i) Is UInteger Then
                            packet.AddUInt32(UpdateData(i))
                        ElseIf TypeOf UpdateData(i) Is Single Then
                            packet.AddSingle(UpdateData(i))
                        Else
                            packet.AddInt32(UpdateData(i))
                        End If
                    End If
                Next

                UpdateMask.SetAll(False)
            End If

            If TypeOf packet Is UpdatePacketClass Then CType(packet, UpdatePacketClass).UpdatesCount += 1
        End Sub

        ''' <summary>
        ''' Adds to packet.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="updateType">Type of the update.</param>
        ''' <param name="updateObject">The update object.</param>
        ''' <returns></returns>
        Public Sub AddToPacket(ByRef packet As PacketClass, ByVal updateType As ObjectUpdateType, ByRef updateObject As ItemObject)
            packet.AddInt8(updateType)
            packet.AddPackGUID(updateObject.GUID)

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Then
                If ITEMDatabase(updateObject.ItemEntry).ContainerSlots > 0 Then
                    packet.AddInt8(ObjectTypeID.TYPEID_CONTAINER)
                Else
                    packet.AddInt8(ObjectTypeID.TYPEID_ITEM)
                End If
                packet.AddInt8(&H18)
                packet.AddUInt64(updateObject.GUID)
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Or updateType = ObjectUpdateType.UPDATETYPE_VALUES Then
                Dim updateCount As Integer = 0
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then updateCount = i
                Next

                packet.AddInt8((updateCount + 32) \ 32)
                packet.AddBitArray(UpdateMask, CType((updateCount + 32) \ 32, Byte) * 4)      'OK Flags are Int32, so to byte -> *4
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then
                        If TypeOf UpdateData(i) Is UInteger Then
                            packet.AddUInt32(UpdateData(i))
                        ElseIf TypeOf UpdateData(i) Is Single Then
                            packet.AddSingle(UpdateData(i))
                        Else
                            packet.AddInt32(UpdateData(i))
                        End If
                    End If
                Next

                UpdateMask.SetAll(False)
            End If

            If TypeOf packet Is UpdatePacketClass Then CType(packet, UpdatePacketClass).UpdatesCount += 1
        End Sub

        ''' <summary>
        ''' Adds to packet.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="updateType">Type of the update.</param>
        ''' <param name="updateObject">The update object.</param>
        ''' <returns></returns>
        Public Sub AddToPacket(ByRef packet As PacketClass, ByVal updateType As ObjectUpdateType, ByRef updateObject As GameObjectObject)
            packet.AddInt8(updateType)
            packet.AddPackGUID(updateObject.GUID)

            Select Case updateObject.Type
                Case GameObjectType.GAMEOBJECT_TYPE_DUEL_ARBITER, GameObjectType.GAMEOBJECT_TYPE_TRAP, GameObjectType.GAMEOBJECT_TYPE_FLAGDROP, GameObjectType.GAMEOBJECT_TYPE_FLAGSTAND
                    updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF
            End Select

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF Then
                packet.AddInt8(ObjectTypeID.TYPEID_GAMEOBJECT)

                'packet.AddInt8(&H58)
                If updateObject.Type = GameObjectType.GAMEOBJECT_TYPE_TRANSPORT OrElse updateObject.Type = GameObjectType.GAMEOBJECT_TYPE_MO_TRANSPORT Then
                    packet.AddInt8(&H52)
                Else
                    packet.AddInt8(&H50)
                End If

                If updateObject.Type = GameObjectType.GAMEOBJECT_TYPE_MO_TRANSPORT Then
                    packet.AddSingle(0)
                    packet.AddSingle(0)
                    packet.AddSingle(0)
                    packet.AddSingle(updateObject.orientation)
                Else
                    packet.AddSingle(updateObject.positionX)
                    packet.AddSingle(updateObject.positionY)
                    packet.AddSingle(updateObject.positionZ)
                    packet.AddSingle(updateObject.orientation)
                End If

                'packet.AddUInt64(updateObject.GUID)
                packet.AddUInt32(GuidHigh(updateObject.GUID))

                If updateObject.Type = GameObjectType.GAMEOBJECT_TYPE_TRANSPORT OrElse updateObject.Type = GameObjectType.GAMEOBJECT_TYPE_MO_TRANSPORT Then
                    packet.AddInt32(msTime)
                End If
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF OrElse updateType = ObjectUpdateType.UPDATETYPE_VALUES Then
                Dim updateCount As Integer = 0
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then updateCount = i
                Next

                packet.AddInt8((updateCount + 32) \ 32)
                packet.AddBitArray(UpdateMask, CType((updateCount + 32) \ 32, Byte) * 4)      'OK Flags are Int32, so to byte -> *4
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then
                        If TypeOf UpdateData(i) Is UInteger Then
                            packet.AddUInt32(UpdateData(i))
                        ElseIf TypeOf UpdateData(i) Is Single Then
                            packet.AddSingle(UpdateData(i))
                        Else
                            packet.AddInt32(UpdateData(i))
                        End If
                    End If
                Next

                UpdateMask.SetAll(False)
            End If

            If TypeOf packet Is UpdatePacketClass Then CType(packet, UpdatePacketClass).UpdatesCount += 1
        End Sub

        ''' <summary>
        ''' Adds to packet.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="updateType">Type of the update.</param>
        ''' <param name="updateObject">The update object.</param>
        ''' <returns></returns>
        Public Sub AddToPacket(ByRef packet As PacketClass, ByVal updateType As ObjectUpdateType, ByRef updateObject As DynamicObjectObject)
            packet.AddInt8(updateType)
            packet.AddPackGUID(updateObject.GUID)

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF Then
                packet.AddInt8(ObjectTypeID.TYPEID_DYNAMICOBJECT)

                packet.AddInt8(&H58)
                packet.AddSingle(updateObject.positionX)
                packet.AddSingle(updateObject.positionY)
                packet.AddSingle(updateObject.positionZ)
                packet.AddSingle(updateObject.orientation)
                packet.AddUInt64(updateObject.GUID)
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT OrElse updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF OrElse updateType = ObjectUpdateType.UPDATETYPE_VALUES Then
                Dim updateCount As Integer = 0
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then updateCount = i
                Next

                packet.AddInt8((updateCount + 32) \ 32)
                packet.AddBitArray(UpdateMask, CType((updateCount + 32) \ 32, Byte) * 4)      'OK Flags are Int32, so to byte -> *4
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then
                        If TypeOf UpdateData(i) Is UInteger Then
                            packet.AddUInt32(UpdateData(i))
                        ElseIf TypeOf UpdateData(i) Is Single Then
                            packet.AddSingle(UpdateData(i))
                        Else
                            packet.AddInt32(UpdateData(i))
                        End If
                    End If
                Next

                UpdateMask.SetAll(False)
            End If

            If TypeOf packet Is UpdatePacketClass Then CType(packet, UpdatePacketClass).UpdatesCount += 1
        End Sub

        ''' <summary>
        ''' Adds to packet.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="updateType">Type of the update.</param>
        ''' <param name="updateObject">The update object.</param>
        ''' <returns></returns>
        Public Sub AddToPacket(ByRef packet As PacketClass, ByVal updateType As ObjectUpdateType, ByRef updateObject As CorpseObject)
            packet.AddInt8(updateType)
            packet.AddPackGUID(updateObject.GUID)

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Then
                packet.AddInt8(ObjectTypeID.TYPEID_CORPSE)

                packet.AddInt8(&H58)
                packet.AddSingle(updateObject.positionX)
                packet.AddSingle(updateObject.positionY)
                packet.AddSingle(updateObject.positionZ)
                packet.AddSingle(updateObject.orientation)
                packet.AddUInt64(updateObject.GUID)
                'packet.AddInt32(1)
            End If

            If updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Or updateType = ObjectUpdateType.UPDATETYPE_VALUES Then
                Dim updateCount As Integer = 0
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then updateCount = i
                Next

                packet.AddInt8((updateCount + 32) \ 32)
                packet.AddBitArray(UpdateMask, CType((updateCount + 32) \ 32, Byte) * 4)      'OK Flags are Int32, so to byte -> *4
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then
                        If TypeOf UpdateData(i) Is UInteger Then
                            packet.AddUInt32(UpdateData(i))
                        ElseIf TypeOf UpdateData(i) Is Single Then
                            packet.AddSingle(UpdateData(i))
                        Else
                            packet.AddInt32(UpdateData(i))
                        End If
                    End If
                Next

                UpdateMask.SetAll(False)
            End If

            If TypeOf packet Is UpdatePacketClass Then CType(packet, UpdatePacketClass).UpdatesCount += 1
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
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
    End Class

#Region "Packets.ArrayBased"
    Public Class PacketClass
        Implements IDisposable

        Public Data() As Byte
        Public Offset As Integer = 4

        'Public ReadOnly Property Length() As Integer
        '    Get
        '        Return (Data(1) + (Data(0) * 256))
        '    End Get
        'End Property
        Public ReadOnly Property OpCode() As OPCODES
            Get
                Return (Data(2) + (Data(3) * 256))
            End Get
        End Property

        Public Sub New(ByVal opcode As OPCODES)
            ReDim Data(3)
            Data(0) = 0
            Data(1) = 0
            Data(2) = CType(opcode, Short) Mod 256
            Data(3) = CType(opcode, Short) \ 256
        End Sub

        Public Sub New(ByRef rawdata() As Byte)
            Data = rawdata
            rawdata.CopyTo(Data, 0)
        End Sub

        ''' <summary>
        ''' Compresses the update packet.
        ''' </summary>
        ''' <returns></returns>
        Public Sub CompressUpdatePacket()
            If OpCode <> OPCODES.SMSG_UPDATE_OBJECT Then Exit Sub 'Wrong packet type
            If Data.Length < 200 Then Exit Sub 'Too small packet

            Dim uncompressedSize As Integer = Data.Length
            Dim compressedBuffer() As Byte = Compress(Data, 4, Data.Length - 4)
            If compressedBuffer.Length = 0 Then Exit Sub

            ReDim Data(3)
            Data(0) = 0
            Data(1) = 0
            Data(2) = CType(OPCODES.SMSG_COMPRESSED_UPDATE_OBJECT, Short) Mod 256
            Data(3) = CType(OPCODES.SMSG_COMPRESSED_UPDATE_OBJECT, Short) \ 256

            AddInt32(uncompressedSize)
            AddByteArray(compressedBuffer)
            UpdateLength() 'Update packet size
        End Sub

        ''' <summary>
        ''' Adds the bit array.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <param name="arraryLen">The len.</param>
        ''' <returns></returns>
        Public Sub AddBitArray(ByVal buffer As BitArray, ByVal arraryLen As Integer)
            ReDim Preserve Data(Data.Length - 1 + arraryLen)

            Dim bufferarray(CType((buffer.Length + 8) / 8, Byte)) As Byte

            buffer.CopyTo(bufferarray, 0)
            Array.Copy(bufferarray, 0, Data, Data.Length - arraryLen, arraryLen)
        End Sub

        ''' <summary>
        ''' Adds the int8.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <param name="position">The position.</param>
        ''' <returns></returns>
        Public Sub AddInt8(ByVal buffer As Byte, Optional ByVal position As Integer = 0)
            If position <= 0 OrElse position >= Data.Length Then
                position = Data.Length
                ReDim Preserve Data(Data.Length)
            End If
            Data(position) = buffer
        End Sub

        ''' <summary>
        ''' Adds the int16.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <param name="position">The position.</param>
        ''' <returns></returns>
        Public Sub AddInt16(ByVal buffer As Short, Optional ByVal position As Integer = 0)
            If position <= 0 OrElse position >= Data.Length Then
                position = Data.Length
                ReDim Preserve Data(Data.Length + 1)
            End If
            Data(position) = buffer And 255
            Data(position + 1) = (buffer >> 8) And 255
        End Sub

        ''' <summary>
        ''' Adds the int32.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <param name="position">The position.</param>
        ''' <returns></returns>
        Public Sub AddInt32(ByVal buffer As Integer, Optional ByVal position As Integer = 0)
            If position <= 0 OrElse position > (Data.Length - 3) Then
                position = Data.Length
                ReDim Preserve Data(Data.Length + 3)
            End If

            Data(position) = buffer And 255
            Data(position + 1) = (buffer >> 8) And 255
            Data(position + 2) = (buffer >> 16) And 255
            Data(position + 3) = (buffer >> 24) And 255
        End Sub

        ''' <summary>
        ''' Adds the int64.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <param name="position">The position.</param>
        ''' <returns></returns>
        Public Sub AddInt64(ByVal buffer As Long, Optional ByVal position As Integer = 0)
            If position <= 0 OrElse position > (Data.Length - 7) Then
                position = Data.Length
                ReDim Preserve Data(Data.Length + 7)
            End If

            Data(position) = buffer And 255
            Data(position + 1) = (buffer >> 8) And 255
            Data(position + 2) = (buffer >> 16) And 255
            Data(position + 3) = (buffer >> 24) And 255
            Data(position + 4) = (buffer >> 32) And 255
            Data(position + 5) = (buffer >> 40) And 255
            Data(position + 6) = (buffer >> 48) And 255
            Data(position + 7) = (buffer >> 56) And 255
        End Sub

        ''' <summary>
        ''' Adds the string.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <returns></returns>
        Public Sub AddString(ByVal buffer As String)
            If IsDBNull(buffer) Or buffer = "" Then
                AddInt8(0)
            Else
                Dim bytes As Byte() = Text.Encoding.UTF8.GetBytes(buffer.ToCharArray)

                ReDim Preserve Data(Data.Length + bytes.Length)

                For i As Integer = 0 To bytes.Length - 1
                    Data(Data.Length - 1 - bytes.Length + i) = bytes(i)
                Next i

                Data(Data.Length - 1) = 0
            End If
        End Sub

        ''' <summary>
        ''' Adds the string2.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <returns></returns>
        Public Sub AddString2(ByVal buffer As String)
            If IsDBNull(buffer) Or buffer = "" Then
                AddInt8(0)
            Else
                Dim bytes As Byte() = Text.Encoding.UTF8.GetBytes(buffer.ToCharArray)

                ReDim Preserve Data(Data.Length + bytes.Length)

                Data(Data.Length - 1 - bytes.Length) = bytes.Length
                For i As Integer = 0 To bytes.Length - 1
                    Data(Data.Length - bytes.Length + i) = bytes(i)
                Next i
            End If
        End Sub

        ''' <summary>
        ''' Adds the single.
        ''' </summary>
        ''' <param name="buffer2">The buffer2.</param>
        ''' <returns></returns>
        Public Sub AddSingle(ByVal buffer2 As Single)
            Dim buffer1 As Byte() = BitConverter.GetBytes(buffer2)
            ReDim Preserve Data(Data.Length + buffer1.Length - 1)
            Buffer.BlockCopy(buffer1, 0, Data, Data.Length - buffer1.Length, buffer1.Length)
        End Sub

        ''' <summary>
        ''' Adds the byte array.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <returns></returns>
        Public Sub AddByteArray(ByVal buffer() As Byte)
            Dim tmp As Integer = Data.Length
            ReDim Preserve Data(Data.Length + buffer.Length - 1)
            Array.Copy(buffer, 0, Data, tmp, buffer.Length)
        End Sub

        ''' <summary>
        ''' Adds the pack GUID.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <returns></returns>
        Public Sub AddPackGUID(ByVal buffer As ULong)
            Dim guid() As Byte = BitConverter.GetBytes(buffer)
            Dim flags As New BitArray(8)
            Dim offsetStart As Integer = Data.Length
            Dim offsetNewSize As Integer = offsetStart

            For i As Byte = 0 To 7
                flags(i) = (guid(i) <> 0)
                If flags(i) Then offsetNewSize += 1
            Next

            ReDim Preserve Data(offsetNewSize)

            flags.CopyTo(Data, offsetStart)
            offsetStart += 1

            For i As Byte = 0 To 7
                If flags(i) Then
                    Data(offsetStart) = guid(i)
                    offsetStart += 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' Adds the U int16.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <returns></returns>
        Public Sub AddUInt16(ByVal buffer As UShort)
            ReDim Preserve Data(Data.Length + 1)

            Data(Data.Length - 2) = buffer And 255
            Data(Data.Length - 1) = (buffer >> 8) And 255
        End Sub

        ''' <summary>
        ''' Adds the U int32.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <returns></returns>
        Public Sub AddUInt32(ByVal buffer As UInteger)
            ReDim Preserve Data(Data.Length + 3)

            Data(Data.Length - 4) = buffer And 255
            Data(Data.Length - 3) = (buffer >> 8) And 255
            Data(Data.Length - 2) = (buffer >> 16) And 255
            Data(Data.Length - 1) = (buffer >> 24) And 255
        End Sub

        ''' <summary>
        ''' Adds the U int64.
        ''' </summary>
        ''' <param name="buffer">The buffer.</param>
        ''' <param name="position">The position.</param>
        ''' <returns></returns>
        Public Sub AddUInt64(ByVal buffer As ULong, Optional ByVal position As Integer = 0)
            Dim dBuffer As Byte() = BitConverter.GetBytes(buffer)
            Dim valueConverted As Long = BitConverter.ToInt64(dBuffer, 0)
            AddInt64(valueConverted, position)
        End Sub

        ''' <summary>
        ''' Updates the length.
        ''' </summary>
        ''' <returns></returns>
        Public Sub UpdateLength()
            If Data(0) <> 0 Or Data(1) <> 0 Then Exit Sub
            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256
        End Sub

        ''' <summary>
        ''' Gets the int8.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetInt8() As Byte
            Offset = Offset + 1
            Return Data(Offset - 1)
        End Function

        ''' <summary>
        ''' Gets the int16.
        ''' </summary>
        ''' <returns></returns>
        '''
        Public Function GetInt16() As Short
            Dim num1 As Short = BitConverter.ToInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function

        ''' <summary>
        ''' Gets the int32.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetInt32() As Integer
            Dim num1 As Integer = BitConverter.ToInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function

        ''' <summary>
        ''' Gets the int64.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetInt64() As Long
            Dim num1 As Long = BitConverter.ToInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function

        ''' <summary>
        ''' Gets the float.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetFloat() As Single
            Dim single1 As Single = BitConverter.ToSingle(Data, Offset)
            Offset = (Offset + 4)
            Return single1
        End Function

        'Public Function GetFloat(ByVal Offset As Integer) As Single
        '    Dim single1 As Single = BitConverter.ToSingle(Data, Offset)
        '    Offset = (Offset + 4)
        '    Return single1
        'End Function

        ''' <summary>
        ''' Gets the string.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetString() As String
            Dim start As Integer = Offset
            Dim i As Integer = 0

            While Data(start + i) <> 0
                i = i + 1
                Offset = Offset + 1
            End While
            Offset = Offset + 1

            Return EscapeString(Text.Encoding.UTF8.GetString(Data, start, i))
        End Function

        ''' <summary>
        ''' Gets the string2.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetString2() As String
            Dim thisLength As Integer = Data(Offset)
            Dim start As Integer = Offset + 1
            Offset += thisLength + 1

            Return EscapeString(Text.Encoding.UTF8.GetString(Data, start, thisLength))
        End Function

        ''' <summary>
        ''' Gets the U int16.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetUInt16() As UShort
            Dim num1 As UShort = BitConverter.ToUInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function

        ''' <summary>
        ''' Gets the U int32.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetUInt32() As UInteger
            Dim num1 As UInteger = BitConverter.ToUInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function

        ''' <summary>
        ''' Gets the U int64.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetUInt64() As ULong
            Dim num1 As ULong = BitConverter.ToUInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function

        ''' <summary>
        ''' Gets the pack GUID.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetPackGuid() As ULong
            Dim flags As Byte = Data(Offset)
            Dim guid() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
            Offset += 1

            If (flags And 1) = 1 Then
                guid(0) = Data(Offset)
                Offset += 1
            End If
            If (flags And 2) = 2 Then
                guid(1) = Data(Offset)
                Offset += 1
            End If
            If (flags And 4) = 4 Then
                guid(2) = Data(Offset)
                Offset += 1
            End If
            If (flags And 8) = 8 Then
                guid(3) = Data(Offset)
                Offset += 1
            End If
            If (flags And 16) = 16 Then
                guid(4) = Data(Offset)
                Offset += 1
            End If
            If (flags And 32) = 32 Then
                guid(5) = Data(Offset)
                Offset += 1
            End If
            If (flags And 64) = 64 Then
                guid(6) = Data(Offset)
                Offset += 1
            End If
            If (flags And 128) = 128 Then
                guid(7) = Data(Offset)
                Offset += 1
            End If

            Return BitConverter.ToUInt64(guid, 0)
        End Function

        'Public Function GetPackGUID(ByVal Offset As Integer) As ULong
        '    Dim flags As Byte = Data(Offset)
        '    Dim GUID() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
        '    Offset += 1

        '    If (flags And 1) = 1 Then
        '        GUID(0) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 2) = 2 Then
        '        GUID(1) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 4) = 4 Then
        '        GUID(2) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 8) = 8 Then
        '        GUID(3) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 16) = 16 Then
        '        GUID(4) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 32) = 32 Then
        '        GUID(5) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 64) = 64 Then
        '        GUID(6) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 128) = 128 Then
        '        GUID(7) = Data(Offset)
        '        Offset += 1
        '    End If

        ''' <summary>
        ''' Gets the byte array.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetByteArray() As Byte()
            Dim lengthLoc As Integer = Data.Length - Offset
            If lengthLoc <= 0 Then Return New Byte() {}
            Return GetByteArray(lengthLoc)
        End Function

        ''' <summary>
        ''' Gets the byte array.
        ''' </summary>
        ''' <param name="lengthLoc">The length loc.</param>
        ''' <returns></returns>
        Private Function GetByteArray(ByVal lengthLoc As Integer) As Byte()
            If Offset + lengthLoc > Data.Length Then lengthLoc = Data.Length - Offset
            If lengthLoc <= 0 Then Return New Byte() {}
            Dim tmpBytes(lengthLoc - 1) As Byte
            Array.Copy(Data, Offset, tmpBytes, 0, tmpBytes.Length)
            Offset += tmpBytes.Length

            Return tmpBytes
        End Function

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
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
    End Class
    Public Class UpdatePacketClass
        Inherits PacketClass

        ''' <summary>
        ''' Gets or sets the updates count.
        ''' </summary>
        ''' <value>The updates count.</value>
        Public Property UpdatesCount() As Integer
            Get
                Return BitConverter.ToInt32(Data, 4)
            End Get
            Set(ByVal value As Integer)
                Data(4) = value And 255
                Data(5) = (value >> 8) And 255
                Data(6) = (value >> 16) And 255
                Data(7) = (value >> 24) And 255
            End Set
        End Property

        Public Sub New()
            MyBase.New(OPCODES.SMSG_UPDATE_OBJECT)

            AddInt32(0)
            AddInt8(0)
        End Sub

        'Public Sub Compress()
        'End Sub
    End Class
#End Region
#Region "Packets.MemoryStreamBased"

    '    Public Class PacketClassNew
    '        Implements IDisposable

    '        Public Offset As Integer = 4
    '        Public Length As Integer = 0
    '        Public ms As MemoryStream
    '        Public bw As BinaryWriter
    '        Public br As BinaryReader

    '        Public ReadOnly Property OpCode() As OPCODES
    '            Get
    '                ms.Seek(2, SeekOrigin.Begin)
    '                Return br.ReadUInt16.ToString
    '            End Get
    '        End Property
    '        Public Property Data() As Byte()
    '            Get
    '                Return ms.ToArray
    '            End Get
    '            Set(ByVal Value As Byte())
    '                ms.Close()
    '                br.Close()
    '                bw.Close()
    '                ms = New MemoryStream(Value.Length)
    '                bw = New BinaryWriter(ms, System.Text.Encoding.UTF8)
    '                br = New BinaryReader(ms, System.Text.Encoding.UTF8)
    '                Length = Value.Length - 2
    '                bw.Write(Value)
    '            End Set
    '        End Property

    '        Public Sub New(ByVal opcode As OPCODES)
    '            ms = New MemoryStream(12)
    '            bw = New BinaryWriter(ms, System.Text.Encoding.UTF8)
    '            br = New BinaryReader(ms, System.Text.Encoding.UTF8)

    '            Length = 2
    '            bw.Write(CType(Length, Int16))
    '            bw.Write(CType(opcode, Int16))
    '        End Sub
    '        Public Sub New(ByRef rawms() As Byte)
    '            ms = New MemoryStream(12)
    '            bw = New BinaryWriter(ms, System.Text.Encoding.UTF8)
    '            br = New BinaryReader(ms, System.Text.Encoding.UTF8)

    '            bw.Write(rawms)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length = br.ReadInt16

    '            ms.Seek(Offset, SeekOrigin.Begin)
    '        End Sub

    '#Region "IDisposable Support"
    '        Private _disposedValue As Boolean ' To detect redundant calls

    '        ' IDisposable
    '        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
    '            If Not _disposedValue Then
    '                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
    '                ' TODO: set large fields to null.
    '                bw.Close()
    '                br.Close()
    '                ms.Close()
    '            End If
    '            _disposedValue = True
    '        End Sub

    '        ' This code added by Visual Basic to correctly implement the disposable pattern.
    '        Public Sub Dispose() Implements IDisposable.Dispose
    '            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '            Dispose(True)
    '            GC.SuppressFinalize(Me)
    '        End Sub
    '#End Region

    '        Public Sub AddBitArray(ByVal buffer As BitArray, ByVal Len As Integer)
    '            Dim bufferarray(CType((buffer.Length + 8) / 8, Byte)) As Byte
    '            buffer.CopyTo(bufferarray, 0)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(bufferarray, 0, Len)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += Len
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddInt8(ByVal buffer As Byte)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(buffer)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += 1
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddInt16(ByVal buffer As Short)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(buffer)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += 2
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddInt32(ByVal buffer As Integer)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(buffer)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += 4
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddInt64(ByVal buffer As Long)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(buffer)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += 8
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddString(ByVal buffer As String)
    '            Dim Bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(buffer.ToCharArray)

    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(Bytes)
    '            bw.Write(CType(0, Byte))

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += Bytes.Length + 1
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddDouble(ByVal buffer As Double)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(buffer)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += 8
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddSingle(ByVal buffer As Single)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(buffer)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += 4
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddByteArray(ByVal buffer() As Byte)
    '            ms.Seek(0, SeekOrigin.End)
    '            bw.Write(buffer)

    '            ms.Seek(0, SeekOrigin.Begin)
    '            Length += buffer.Length
    '            bw.Write(CType(Length, Int16))
    '        End Sub
    '        Public Sub AddPackGUID(ByVal buffer As ULong)
    '            Dim GUID() As Byte = BitConverter.GetBytes(buffer)
    '            Dim flags As New BitArray(8)
    '            Dim flagsByte(1) As Byte
    '            Length += 1

    '            flags(0) = (GUID(0) <> 0)
    '            flags(1) = (GUID(1) <> 0)
    '            flags(2) = (GUID(2) <> 0)
    '            flags(3) = (GUID(3) <> 0)
    '            flags(4) = (GUID(4) <> 0)
    '            flags(5) = (GUID(5) <> 0)
    '            flags(6) = (GUID(6) <> 0)
    '            flags(7) = (GUID(7) <> 0)

    '            If flags(0) Then Length += 1
    '            If flags(1) Then Length += 1
    '            If flags(2) Then Length += 1
    '            If flags(3) Then Length += 1
    '            If flags(4) Then Length += 1
    '            If flags(5) Then Length += 1
    '            If flags(6) Then Length += 1
    '            If flags(7) Then Length += 1

    '            ms.Seek(0, SeekOrigin.End)
    '            flags.CopyTo(flagsByte, 0)
    '            bw.Write(flagsByte(0))
    '            If flags(0) Then bw.Write(GUID(0))
    '            If flags(1) Then bw.Write(GUID(1))
    '            If flags(2) Then bw.Write(GUID(2))
    '            If flags(3) Then bw.Write(GUID(3))
    '            If flags(4) Then bw.Write(GUID(4))
    '            If flags(5) Then bw.Write(GUID(5))
    '            If flags(6) Then bw.Write(GUID(6))
    '            If flags(7) Then bw.Write(GUID(7))

    '            ms.Seek(0, SeekOrigin.Begin)
    '            bw.Write(CType(Length, Int16))
    '        End Sub

    '        Public Function GetInt8() As Byte
    '            Return br.ReadByte()
    '        End Function
    '        'Public Function GetInt8(ByVal Offset As Integer) As Byte
    '        '    ms.Seek(Offset, SeekOrigin.Begin)
    '        '    Return br.ReadByte()
    '        'End Function
    '        Public Function GetInt16() As Short
    '            Return br.ReadInt16
    '        End Function
    '        'Public Function GetInt16(ByVal Offset As Integer) As Short
    '        '    ms.Seek(Offset, SeekOrigin.Begin)
    '        '    Return br.ReadInt16
    '        'End Function
    '        Public Function GetInt32() As Integer
    '            Return br.ReadInt32
    '        End Function
    '        'Public Function GetInt32(ByVal Offset As Integer) As Integer
    '        '    ms.Seek(Offset, SeekOrigin.Begin)
    '        '    Return br.ReadInt32
    '        'End Function
    '        Public Function GetInt64() As Long
    '            Return br.ReadInt64
    '        End Function
    '        'Public Function GetInt64(ByVal Offset As Integer) As Long
    '        '    ms.Seek(Offset, SeekOrigin.Begin)
    '        '    Return br.ReadInt64
    '        'End Function
    '        Public Function GetFloat() As Single
    '            Return br.ReadSingle
    '        End Function
    '        'Public Function GetFloat(ByVal Offset As Integer) As Single
    '        '    ms.Seek(Offset, SeekOrigin.Begin)
    '        '    Return br.ReadSingle
    '        'End Function
    '        Public Function GetDouble() As Double
    '            Return br.ReadDouble
    '        End Function
    '        'Public Function GetDouble(ByVal Offset As Integer) As Double
    '        '    ms.Seek(Offset, SeekOrigin.Begin)
    '        '    Return br.ReadDouble
    '        'End Function
    '        Public Function GetString() As String
    '            Dim tmpString As New System.Text.StringBuilder
    '            Dim tmpChar As Char = br.ReadChar()
    '            Dim tmpEndChar As Char = System.Text.Encoding.UTF8.GetString(New Byte() {0})

    '            While tmpChar <> tmpEndChar
    '                tmpString.Append(tmpChar)
    '                tmpChar = br.ReadChar()
    '            End While

    '            Return tmpString.ToString
    '        End Function
    '        'Public Function GetString(ByVal Offset As Integer) As String
    '        '    ms.Seek(Offset, SeekOrigin.Begin)
    '        '    Dim tmpString As New System.Text.StringBuilder
    '        '    Dim tmpChar As Char = br.ReadChar()
    '        '    Dim tmpEndChar As Char = System.Text.Encoding.UTF8.GetString(New Byte() {0})

    '        '    While tmpChar <> tmpEndChar
    '        '        tmpString.Append(tmpChar)
    '        '        tmpChar = br.ReadChar()
    '        '    End While

    '        '    Return tmpString.ToString
    '        'End Function
    '        'Public Function GetPackGUID() As ULong
    '        '    Dim flags As Byte = br.ReadByte
    '        '    Dim GUID() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
    '        '    Offset += 1

    '        '    If (flags And 1) = 1 Then GUID(0) = br.ReadByte
    '        '    If (flags And 2) = 2 Then GUID(1) = br.ReadByte
    '        '    If (flags And 4) = 4 Then GUID(2) = br.ReadByte
    '        '    If (flags And 8) = 8 Then GUID(3) = br.ReadByte
    '        '    If (flags And 16) = 16 Then GUID(4) = br.ReadByte
    '        '    If (flags And 32) = 32 Then GUID(5) = br.ReadByte
    '        '    If (flags And 64) = 64 Then GUID(6) = br.ReadByte
    '        '    If (flags And 128) = 128 Then GUID(7) = br.ReadByte

    '        '    Return CType(BitConverter.ToUInt64(GUID, 0), ULong)
    '        'End Function
    '        '    Public Function GetPackGUID(ByVal Offset As Integer) As ULong
    '        '        ms.Seek(Offset, SeekOrigin.Begin)
    '        '        Dim flags As Byte = br.ReadByte
    '        '        Dim GUID() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
    '        '        Offset += 1

    '        '        If (flags And 1) = 1 Then GUID(0) = br.ReadByte
    '        '        If (flags And 2) = 2 Then GUID(1) = br.ReadByte
    '        '        If (flags And 4) = 4 Then GUID(2) = br.ReadByte
    '        '        If (flags And 8) = 8 Then GUID(3) = br.ReadByte
    '        '        If (flags And 16) = 16 Then GUID(4) = br.ReadByte
    '        '        If (flags And 32) = 32 Then GUID(5) = br.ReadByte
    '        '        If (flags And 64) = 64 Then GUID(6) = br.ReadByte
    '        '        If (flags And 128) = 128 Then GUID(7) = br.ReadByte

    '        '        Return CType(BitConverter.ToUInt64(GUID, 0), ULong)
    '        '    End Function

    '        'End Class
    '        'Public Class UpdatePacketClassNew
    '        '    Inherits PacketClassNew

    '        '    Public Property UpdatesCount() As Integer
    '        '        Get
    '        '            ms.Seek(4, SeekOrigin.Begin)
    '        '            Return br.ReadInt32
    '        '        End Get
    '        '        Set(ByVal Value As Integer)
    '        '            ms.Seek(4, SeekOrigin.Begin)
    '        '            bw.Write(Value)
    '        '        End Set
    '        '    End Property
    '        Public Sub New()
    '            '            MyBase.New(OPCODES.SMSG_UPDATE_OBJECT)
    '            MyBase.New()

    '            AddInt32(0)
    '            AddInt8(0)
    '        End Sub

    '        'Public Sub Compress()
    '        'End Sub
    '    End Class
#End Region
End Module