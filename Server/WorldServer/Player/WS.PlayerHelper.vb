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

Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common.Global_Constants

Public Module WS_PlayerHelper

    Public Class TSkill
        Private _Current As Int16 = 0
        Public Bonus As Int16 = 0
        Public Base As Int16 = 300
        Public Sub New(ByVal CurrentVal As Int16, Optional ByVal MaximumVal As Int16 = 375)
            Current = CurrentVal
            Base = MaximumVal
        End Sub

        Public Sub Increment(Optional ByVal Incrementator As Int16 = 1)
            If (Current + Incrementator) < Base Then
                Current = Current + Incrementator
            Else
                Current = Base
            End If
        End Sub

        Public ReadOnly Property Maximum() As Integer
            Get
                Return Base
            End Get
        End Property

        Public ReadOnly Property MaximumWithBonus() As Integer
            Get
                Return Base + Bonus
            End Get
        End Property

        Public Property Current() As Int16
            Get
                Return _Current
            End Get
            Set(ByVal Value As Int16)
                If Value <= Maximum Then _Current = Value
            End Set
        End Property

        Public ReadOnly Property CurrentWithBonus() As Int16
            Get
                Return _Current + Bonus
            End Get
        End Property

        Public ReadOnly Property GetSkill() As Integer
            Get
                Return CType((_Current + (CType(Base + Bonus, Integer) << 16)), Integer)
            End Get
        End Property
    End Class

    Public Class TStatBar
        Private _Current As Integer = 0
        Public Bonus As Integer = 0
        Public Base As Integer = 0
        Public Modifier As Single = 1
        Public Sub Increment(Optional ByVal Incrementator As Integer = 1)
            If (Current + Incrementator) < (Bonus + Base) Then
                Current = Current + Incrementator
            Else
                Current = Maximum
            End If
        End Sub

        Public Sub New(ByVal CurrentVal As Integer, ByVal BaseVal As Integer, ByVal BonusVal As Integer)
            _Current = CurrentVal
            Bonus = BonusVal
            Base = BaseVal
        End Sub

        Public ReadOnly Property Maximum() As Integer
            Get
                Return (Bonus + Base) * Modifier
            End Get
        End Property

        Public Property Current() As Integer
            Get
                Return _Current * Modifier
            End Get
            Set(ByVal Value As Integer)
                If Value <= Me.Maximum Then _Current = Value Else _Current = Me.Maximum
                If _Current < 0 Then _Current = 0
            End Set
        End Property
    End Class

    Public Class TStat
        Public Base As Integer = 0
        Public PositiveBonus As Short = 0
        Public NegativeBonus As Short = 0
        Public BaseModifier As Single = 1
        Public Modifier As Single = 1
        Public Property RealBase() As Integer
            Get
                Return (Base - PositiveBonus + NegativeBonus)
            End Get
            Set(ByVal value As Integer)
                Base = Base - PositiveBonus + NegativeBonus
                Base = value
                Base = Base + PositiveBonus - NegativeBonus
            End Set
        End Property

        Public Sub New(Optional ByVal BaseValue As Byte = 0, Optional ByVal PosValue As Byte = 0, Optional ByVal NegValue As Byte = 0)
            Base = BaseValue
            PositiveBonus = PosValue
            PositiveBonus = NegValue
        End Sub
    End Class

    Public Class TDamageBonus
        Public PositiveBonus As Integer = 0
        Public NegativeBonus As Integer = 0
        Public Modifier As Single = 1
        Public ReadOnly Property Value() As Integer
            Get
                Return (PositiveBonus - NegativeBonus) * Modifier
            End Get
        End Property

        Public Sub New(Optional ByVal PosValue As Byte = 0, Optional ByVal NegValue As Byte = 0)
            PositiveBonus = PosValue
            PositiveBonus = NegValue
        End Sub
    End Class

    Public Class THonor
        Public CharGUID As ULong = 0
        Public HonorPounts As Short = 0                 '! MAX=1000 ?
        Public HonorRank As Byte = 0
        Public HonorHightestRank As Byte = 0
        Public Standing As Integer = 0

        Public HonorLastWeek As Integer = 0
        Public HonorThisWeek As Integer = 0
        Public HonorYesterday As Integer = 0

        Public KillsLastWeek As Integer = 0
        Public KillsThisWeek As Integer = 0
        Public KillsYesterday As Integer = 0

        Public KillsHonorableToday As Integer = 0
        Public KillsDisHonorableToday As Integer = 0
        Public KillsHonorableLifetime As Integer = 0
        Public KillsDisHonorableLifetime As Integer = 0

        Public Sub Save()
            Dim tmp As String = "UPDATE characters_honor SET"

            tmp = tmp & " honor_points=""" & HonorPounts & """"
            tmp = tmp & ", honor_rank=" & HonorRank
            tmp = tmp & ", honor_hightestRank=" & HonorHightestRank
            tmp = tmp & ", honor_standing=" & Standing
            tmp = tmp & ", honor_lastWeek=" & HonorLastWeek
            tmp = tmp & ", honor_thisWeek=" & HonorThisWeek
            tmp = tmp & ", honor_yesterday=" & HonorYesterday
            tmp = tmp & ", kills_lastWeek=" & KillsLastWeek
            tmp = tmp & ", kills_thisWeek=" & KillsThisWeek
            tmp = tmp & ", kills_yesterday=" & KillsYesterday
            tmp = tmp & ", kills_dishonorableToday=" & KillsDisHonorableToday
            tmp = tmp & ", kills_honorableToday=" & KillsHonorableToday
            tmp = tmp & ", kills_dishonorableLifetime=" & KillsDisHonorableLifetime
            tmp = tmp & ", kills_honorableLifetime=" & KillsHonorableLifetime

            tmp = tmp + String.Format(" WHERE char_guid = ""{0}"";", CharGUID)
            CharacterDatabase.Update(tmp)
        End Sub

        Public Sub Load(ByVal GUID As ULong)

        End Sub

        Public Sub SaveAsNew(ByVal GUID As ULong)

        End Sub
    End Class

    Public Class TReputation
        '1:"AtWar" clickable but not checked
        '3:"AtWar" clickable and checked
        Public Flags As Integer = 0
        Public Value As Integer = 0
    End Class

    Public Class TActionButton
        Public ActionType As Byte = 0
        Public ActionMisc As Byte = 0
        Public Action As Integer = 0
        Public Sub New(ByVal Action_ As Integer, ByVal Type_ As Byte, ByVal Misc_ As Byte)
            ActionType = Type_
            ActionMisc = Misc_
            Action = Action_
        End Sub
    End Class

    Public Class TDrowningTimer
        Implements IDisposable

        Private DrowningTimer As Timer = Nothing
        Public DrowningValue As Integer = 70000
        Public DrowningDamage As Byte = 1
        Public CharacterGUID As ULong = 0

        Public Sub New(ByRef Character As CharacterObject)
            CharacterGUID = Character.GUID
            Character.StartMirrorTimer(MirrorTimer.DROWNING, 70000)
            DrowningTimer = New Timer(AddressOf Character.HandleDrowning, Nothing, 2000, 1000)
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                If IsNothing(DrowningTimer) = False Then
                    DrowningTimer.Dispose()
                    DrowningTimer = Nothing
                End If
                If CHARACTERs.ContainsKey(CharacterGUID) Then CHARACTERs(CharacterGUID).StopMirrorTimer(1)
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

    Public Class TRepopTimer
        Implements IDisposable

        Private RepopTimer As Timer = Nothing
        Public Character As CharacterObject = Nothing

        Public Sub New(ByRef Character As CharacterObject)
            Me.Character = Character
            RepopTimer = New Timer(AddressOf Repop, Nothing, 360000, 360000)
        End Sub
        Public Sub Repop(ByVal Obj As Object)
            CharacterRepop(Character.client)
            Character.repopTimer = Nothing
            Me.Dispose()
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                RepopTimer.Dispose()
                RepopTimer = Nothing
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

    Public Sub SendBindPointUpdate(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim SMSG_BINDPOINTUPDATE As New PacketClass(OPCODES.SMSG_BINDPOINTUPDATE)
        Try
            SMSG_BINDPOINTUPDATE.AddSingle(Character.bindpoint_positionX)
            SMSG_BINDPOINTUPDATE.AddSingle(Character.bindpoint_positionY)
            SMSG_BINDPOINTUPDATE.AddSingle(Character.bindpoint_positionZ)
            SMSG_BINDPOINTUPDATE.AddInt32(Character.bindpoint_map_id)
            SMSG_BINDPOINTUPDATE.AddInt32(Character.bindpoint_zone_id)
            client.Send(SMSG_BINDPOINTUPDATE)
        Finally
            SMSG_BINDPOINTUPDATE.Dispose()
        End Try
    End Sub

    Public Sub Send_SMSG_SET_REST_START(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim SMSG_SET_REST_START As New PacketClass(OPCODES.SMSG_SET_REST_START)
        Try
            SMSG_SET_REST_START.AddInt32(msTime)
            client.Send(SMSG_SET_REST_START)
        Finally
            SMSG_SET_REST_START.Dispose()
        End Try
    End Sub

    Public Sub SendTutorialFlags(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim SMSG_TUTORIAL_FLAGS As New PacketClass(OPCODES.SMSG_TUTORIAL_FLAGS)
        Try
            '[8*Int32] or [32 Bytes] or [256 Bits Flags] Total!!!
            'SMSG_TUTORIAL_FLAGS.AddInt8(0)
            'SMSG_TUTORIAL_FLAGS.AddInt8(Character.TutorialFlags.Length)
            SMSG_TUTORIAL_FLAGS.AddByteArray(Character.TutorialFlags)
            client.Send(SMSG_TUTORIAL_FLAGS)
        Finally
            SMSG_TUTORIAL_FLAGS.Dispose()
        End Try
    End Sub

    Public Sub SendFactions(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim packet As New PacketClass(OPCODES.SMSG_INITIALIZE_FACTIONS)
        Try
            packet.AddInt32(64)
            For i As Byte = 0 To 63
                packet.AddInt8(Character.Reputation(i).Flags)                               'Flags
                packet.AddInt32(Character.Reputation(i).Value)                              'Standing
            Next i

            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    Public Sub SendActionButtons(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim packet As New PacketClass(OPCODES.SMSG_ACTION_BUTTONS)
        Try
            For i As Byte = 0 To 119    'or 480 ?
                If Character.ActionButtons.ContainsKey(i) Then
                    packet.AddUInt16(Character.ActionButtons(i).Action)
                    packet.AddInt8(Character.ActionButtons(i).ActionType)
                    packet.AddInt8(Character.ActionButtons(i).ActionMisc)
                Else
                    packet.AddInt32(0)
                End If
            Next

            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    Public Sub SendInitWorldStates(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Character.ZoneCheck()
        Dim NumberOfFields As UShort = 0
        Select Case Character.ZoneID
            Case 0, 1, 4, 8, 10, 11, 12, 36, 38, 40, 41, 51, 267, 1519, 1537, 2257, 2918
                NumberOfFields = 6
            Case 2597
                NumberOfFields = 81
            Case 3277
                NumberOfFields = 14
            Case 3358, 3820
                NumberOfFields = 38
            Case 3483
                NumberOfFields = 22
            Case 3519
                NumberOfFields = 36
            Case 3521
                NumberOfFields = 35
            Case 3698, 3702, 3968
                NumberOfFields = 9
            Case 3703
                NumberOfFields = 9
            Case Else
                NumberOfFields = 10
        End Select

        Dim packet As New PacketClass(OPCODES.SMSG_INIT_WORLD_STATES)
        Try
            packet.AddUInt32(Character.MapID)
            packet.AddInt32(Character.ZoneID)
            packet.AddInt32(Character.AreaID)
            packet.AddUInt16(NumberOfFields)
            packet.AddUInt32(&H8D8)
            packet.AddUInt32(&H0)
            packet.AddUInt32(&H8D7)
            packet.AddUInt32(&H0)
            packet.AddUInt32(&H8D6)
            packet.AddUInt32(&H0)
            packet.AddUInt32(&H8D5)
            packet.AddUInt32(&H0)
            packet.AddUInt32(&H8D4)
            packet.AddUInt32(&H0)
            packet.AddUInt32(&H8D3)
            packet.AddUInt32(&H0)
            Select Case Character.ZoneID
                Case 1, 11, 12, 38, 40, 51, 1519, 1537, 2257
                    Exit Select
                Case 2597 'AV
                    'TODO
                Case 3277 'WSG
                    'TODO
                Case 3358 'AB
                    'TODO
                Case 3820 'Eye of the Storm
                    'TODO
                Case 3968 'Ruins of Lordaeron Arena
                    packet.AddUInt32(&HBB8)
                    packet.AddUInt32(&H0)
                    packet.AddUInt32(&HBB9)
                    packet.AddUInt32(&H0)
                    packet.AddUInt32(&HBBA)
                    packet.AddUInt32(&H0)
                    Exit Select
                Case Else
                    packet.AddUInt32(&H914)
                    packet.AddUInt32(&H0)
                    packet.AddUInt32(&H913)
                    packet.AddUInt32(&H0)
                    packet.AddUInt32(&H912)
                    packet.AddUInt32(&H0)
                    packet.AddUInt32(&H915)
                    packet.AddUInt32(&H0)
            End Select
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    Public Sub SendInitialSpells(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim packet As New PacketClass(OPCODES.SMSG_INITIAL_SPELLS)
        Try
            packet.AddInt8(0)
            Dim countPos As Integer = packet.Data.Length
            packet.AddInt16(0) 'Updated later

            Dim spellCount As Integer = 0
            Dim spellCooldowns As New Dictionary(Of Integer, KeyValuePair(Of UInteger, Integer))
            For Each Spell As KeyValuePair(Of Integer, CharacterSpell) In Character.Spells
                If Spell.Value.Active = 1 Then
                    packet.AddUInt16(Spell.Key) 'SpellID
                    packet.AddInt16(0) 'Unknown
                    spellCount += 1

                    If Spell.Value.Cooldown > 0UI Then
                        spellCooldowns.Add(Spell.Key, New KeyValuePair(Of UInteger, Integer)(Spell.Value.Cooldown, 0))
                    End If
                End If
            Next
            packet.AddInt16(spellCount, countPos)

            spellCount = 0
            countPos = packet.Data.Length
            packet.AddInt16(0) 'Updated later

            For Each Cooldown As KeyValuePair(Of Integer, KeyValuePair(Of UInteger, Integer)) In spellCooldowns
                If SPELLs.ContainsKey(Cooldown.Key) = False Then Continue For

                packet.AddUInt16(Cooldown.Key) 'SpellID

                Dim timeLeft As Integer = 0
                If Cooldown.Value.Key > GetTimestamp(Now) Then
                    timeLeft = (Cooldown.Value.Key - GetTimestamp(Now)) * 1000
                End If

                packet.AddUInt16(Cooldown.Value.Value) 'CastItemID
                packet.AddUInt16(SPELLs(Cooldown.Key).Category) 'SpellCategory
                If SPELLs(Cooldown.Key).CategoryCooldown > 0 Then
                    packet.AddInt32(0) 'SpellCooldown
                    packet.AddInt32(timeLeft) 'CategoryCooldown
                Else
                    packet.AddInt32(timeLeft) 'SpellCooldown
                    packet.AddInt32(0) 'CategoryCooldown
                End If

                spellCount += 1
            Next
            packet.AddInt16(spellCount, countPos)

            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    Public Sub InitializeTalentSpells(ByVal objCharacter As CharacterObject)
        Dim t As New SpellTargets
        t.SetTarget_SELF(CType(objCharacter, CharacterObject))

        For Each Spell As KeyValuePair(Of Integer, CharacterSpell) In objCharacter.Spells
            If SPELLs.ContainsKey(Spell.Key) AndAlso (SPELLs(Spell.Key).IsPassive) Then
                'DONE: Add passive spell we don't have
                'DONE: Remove passive spells we can't have anymore
                If objCharacter.HavePassiveAura(Spell.Key) = False AndAlso SPELLs(Spell.Key).CanCast(objCharacter, t, False) = SpellFailedReason.SPELL_NO_ERROR Then
                    SPELLs(Spell.Key).Apply(CType(objCharacter, CharacterObject), t)
                ElseIf objCharacter.HavePassiveAura(Spell.Key) AndAlso SPELLs(Spell.Key).CanCast(objCharacter, t, False) <> SpellFailedReason.SPELL_NO_ERROR Then
                    objCharacter.RemoveAuraBySpell(Spell.Key)
                End If
            End If
        Next
    End Sub

End Module