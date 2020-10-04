'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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

Imports mangosVB.Common.Globals
Imports mangosVB.Shared

Public Module WS_PlayerData

    Public Class CharacterObject
        Inherits BaseUnit
        Implements IDisposable

        'Connection Information
        Public client As ClientClass
        Public Access As AccessLevel = AccessLevel.Player
        Public LogoutTimer As Timer
        Public FullyLoggedIn As Boolean = False
        Public LoginMap As UInteger = 0
        Public LoginTransport As ULong = 0L

        'Character Information
        Public TargetGUID As ULong = 0
        Public Model_Native As Integer = 0
        Public cPlayerFlags As PlayerFlags = 0
        '                                                    <<0                <<8             <<16                <<24
        Public cPlayerBytes As Integer = 0                  'Skin,              Face,           HairStyle,          HairColor
        Public cPlayerBytes2 As Integer = &H200EE00         'FacialHair,        ?,              BankSlotsAvailable, RestState
        Public cPlayerBytes3 As Integer = 0                 'Gender,            Alchohol,       Defender?,          LastWeekHonorRank
        Public cPlayerFieldBytes As Integer = &HEEE00000    '?,                 ComboPoints,    ActionBar,          HighestHonorRank
        Public cPlayerFieldBytes2 As Integer = 0            'HonorBar

        'cPlayerBytes subfields
        '(Skin + (CType(Face, Integer) << 8) + (CType(HairStyle, Integer) << 16) + (CType(HairColor, Integer) << 24))
        Public Property HairColor() As Byte
            Get
                Return (cPlayerBytes And &HFF000000) >> 24
            End Get
            Set(ByVal value As Byte)
                cPlayerBytes = ((cPlayerBytes And &HFFFFFF) Or (CInt(value) << 24))
            End Set
        End Property

        Public Property HairStyle() As Byte
            Get
                Return (cPlayerBytes And &HFF0000) >> 16
            End Get
            Set(ByVal value As Byte)
                cPlayerBytes = ((cPlayerBytes And &HFF00FFFF) Or (CInt(value) << 16))
            End Set
        End Property

        Public Property Face() As Byte
            Get
                Return (cPlayerBytes And &HFF00) >> 8
            End Get
            Set(ByVal value As Byte)
                cPlayerBytes = ((cPlayerBytes And &HFFFF00FF) Or (CInt(value) << 8))
            End Set
        End Property

        Public Property Skin() As Byte
            Get
                Return (cPlayerBytes And &HFF) >> 0
            End Get
            Set(ByVal value As Byte)
                cPlayerBytes = ((cPlayerBytes And &HFFFFFF00) Or (CInt(value) << 0))
            End Set
        End Property

        'cPlayerBytes2 subfields
        '(FacialHair + (&HEE << 8) + (CType(Items_AvailableBankSlots, Integer) << 16) + (CType(RestState, Integer) << 24))
        Public Property RestState() As XPSTATE
            Get
                Return (cPlayerBytes2 And &HFF000000) >> 24
            End Get
            Set(ByVal value As XPSTATE)
                cPlayerBytes2 = ((cPlayerBytes2 And &HFFFFFF) Or (CInt(value) << 24))
            End Set
        End Property

        Public Property Items_AvailableBankSlots() As Byte
            Get
                Return (cPlayerBytes2 And &HFF0000) >> 16
            End Get
            Set(ByVal value As Byte)
                cPlayerBytes2 = ((cPlayerBytes2 And &HFF00FFFF) Or (CInt(value) << 16))
            End Set
        End Property

        Public Property FacialHair() As Byte
            Get
                Return (cPlayerBytes2 And &HFF) >> 0
            End Get
            Set(ByVal value As Byte)
                cPlayerBytes2 = ((cPlayerBytes2 And &HFFFFFF00) Or (CInt(value) << 0))
            End Set
        End Property

        'cPlayerBytes3 subfields
        '(CInt(Gender) Or (CInt(HonorRank) << 24UI))
        Public Overrides Property Gender() As Genders
            Get
                Return (cBytes0 And &HFF0000) >> 16
            End Get
            Set(ByVal value As Genders)
                cBytes0 = ((cBytes0 And &HFF00FFFF) Or (CInt(value) << 16))
                cPlayerBytes3 = ((cPlayerBytes3 And &HFFFFFF00) Or (CInt(value) << 0))
            End Set
        End Property

        Public Property HonorRank() As PlayerHonorRank
            Get
                Return (cPlayerBytes3 And &HFF000000) >> 24
            End Get
            Set(ByVal value As PlayerHonorRank)
                cPlayerBytes3 = ((cPlayerBytes3 And &HFFFFFF) Or (CInt(value) << 24))
            End Set
        End Property

        'cPlayerFieldBytes subfields
        Public Property HonorHighestRank() As PlayerHonorRank
            Get
                Return (cPlayerFieldBytes And &HFF000000) >> 24
            End Get
            Set(ByVal value As PlayerHonorRank)
                cPlayerFieldBytes = ((cPlayerFieldBytes And &HFFFFFF) Or (CInt(value) << 24))
            End Set
        End Property

        'cPlayerFieldBytes2 subfields
        Public Property HonorBar() As Byte
            Get
                Return (cPlayerFieldBytes2 And &HFF) >> 0
            End Get
            Set(ByVal value As Byte)
                cPlayerFieldBytes2 = ((cPlayerFieldBytes2 And &HFFFFFF00) Or (CInt(value) << 0))
            End Set
        End Property

        Public Rage As New TStatBar(1, 1, 0)
        Public Energy As New TStatBar(1, 1, 0)
        Public Strength As New TStat
        Public Agility As New TStat
        Public Stamina As New TStat
        Public Intellect As New TStat
        Public Spirit As New TStat
        Public Faction As Short = FactionTemplates.None

        'Combat related
        Public attackState As TAttackTimer = New TAttackTimer(Me)
        Public attackSelection As BaseObject = Nothing
        Public attackSheathState As SHEATHE_SLOT = SHEATHE_SLOT.SHEATHE_NONE
        Public Disarmed As Boolean

        ' Miscellaneous Information
        Public MenuNumber As Integer = 0

        Public ReadOnly Property GetTarget() As BaseUnit
            Get
                If GuidIsCreature(TargetGUID) Then Return WORLD_CREATUREs(TargetGUID)
                If GuidIsPlayer(TargetGUID) Then Return CHARACTERs(TargetGUID)
                If GuidIsPet(TargetGUID) Then Return WORLD_CREATUREs(TargetGUID)
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property CanShootRanged() As Boolean
            Get
                Return (AmmoID > 0 AndAlso Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) AndAlso Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).IsRanged AndAlso Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).IsBroken = False AndAlso ItemCOUNT(AmmoID) > 0)
            End Get
        End Property

        Public ReadOnly Property GetRageConversion() As Single
            ' From http://www.wowwiki.com/Formulas:Rage_generation
            Get
                Return 0.0091107836 * Level * Level + 3.225598133 * Level + 4.2652911
            End Get
        End Property

        Public ReadOnly Property GetHitFactor(Optional ByVal MainHand As Boolean = True, Optional ByVal Critical As Boolean = False) As Single
            Get
                Dim HitFactor As Single = 1.75
                If MainHand Then HitFactor *= 2
                If Critical Then HitFactor *= 2
                Return HitFactor
            End Get
        End Property

        Public ReadOnly Property GetCriticalWithSpells() As Single
            ' From http://www.wowwiki.com/Spell_critical_strike
            ' TODO: Need to add SpellCritical Value in this format -- (Intellect/80 '82 for Warlocks) + (Spell Critical Strike Rating/22.08) + Class Specific Constant
            ' How do you generate the base spell crit rating... and then we can fix the formula
            Get
                Select Case Classe
                    Case Classes.CLASS_DRUID
                        Return Fix(Intellect.Base / 80 + 1.85F)
                    Case Classes.CLASS_MAGE
                        Return Fix(Intellect.Base / 80 + 0.91F)
                    Case Classes.CLASS_PRIEST
                        Return Fix(Intellect.Base / 80 + 1.24F)
                    Case Classes.CLASS_WARLOCK
                        Return Fix(Intellect.Base / 82 + 1.701F)
                    Case Classes.CLASS_PALADIN
                        Return Fix(Intellect.Base / 80 + 3.336F)
                    Case Classes.CLASS_SHAMAN
                        Return Fix(Intellect.Base / 80 + 2.2F)
                    Case Classes.CLASS_HUNTER
                        Return Fix(Intellect.Base / 80 + 3.6F)
                    Case Else
                        'CLASS_ROGUE
                        'CLASS_WARRIOR
                        Return 0
                End Select

            End Get
        End Property

        Public spellCasted() As CastSpellParameters = {Nothing, Nothing, Nothing, Nothing}
        Public spellCastManaRegeneration As Byte = 0
        Public spellCanDualWeild As Boolean = False
        Public healing As New TDamageBonus
        Public spellDamage(6) As TDamageBonus
        Public spellCriticalRating As Integer = 0
        Public combatCanDualWield As Boolean = False
        Public combatBlock As Integer = 0
        Public combatBlockValue As Integer = 0
        Public combatParry As Integer = 0
        Public combatCrit As Integer = 0
        Public combatDodge As Integer = 0
        Public Damage As New TDamage
        Public RangedDamage As New TDamage
        Public OffHandDamage As New TDamage
        Public ReadOnly Property BaseUnarmedDamage() As Integer
            Get
                Return (AttackPower + AttackPowerMods) * 0.071428571428571425
            End Get
        End Property

        Public ReadOnly Property BaseRangedDamage() As Integer
            Get
                Return (AttackPowerRanged + AttackPowerModsRanged) * 0.071428571428571425
            End Get
        End Property

        Public ReadOnly Property AttackPower() As Integer
            ' From http://www.wowwiki.com/Attack_power
            Get
                Select Case Classe
                    Case Classes.CLASS_WARRIOR, Classes.CLASS_PALADIN
                        Return (Level * 3 + Strength.Base * 3 - 20)
                    Case Classes.CLASS_SHAMAN
                        Return (Level * 2 + Strength.Base * 2 - 20)
                    Case Classes.CLASS_MAGE, Classes.CLASS_PRIEST, Classes.CLASS_WARLOCK
                        Return (Strength.Base - 10)
                    Case Classes.CLASS_ROGUE, Classes.CLASS_HUNTER
                        Return (Level * 2 + Strength.Base + Agility.Base - 20)
                    Case Classes.CLASS_DRUID
                        If ShapeshiftForm = ShapeshiftForm.FORM_CAT Then
                            Return (Level * 2 + Strength.Base * 2 + Agility.Base - 20)
                        ElseIf ShapeshiftForm = ShapeshiftForm.FORM_BEAR Or ShapeshiftForm = ShapeshiftForm.FORM_DIREBEAR Then
                            Return (Level * 3 + Strength.Base * 2 - 20)
                        ElseIf ShapeshiftForm = ShapeshiftForm.FORM_MOONKIN Then
                            Return (Level * 1.5 + Agility.Base + Strength.Base * 2 - 20)
                        Else
                            Return (Strength.Base * 2 - 20)
                        End If
                End Select
            End Get
        End Property

        Public ReadOnly Property AttackPowerRanged() As Integer
            ' From http://www.wowwiki.com/Attack_power
            Get
                Select Case Classe
                    Case Classes.CLASS_WARRIOR, Classes.CLASS_ROGUE
                        Return (Level + Agility.Base - 10)
                    Case Classes.CLASS_HUNTER
                        Return (Level * 2 + Agility.Base - 10)
                    Case Classes.CLASS_PALADIN, Classes.CLASS_SHAMAN, Classes.CLASS_MAGE, Classes.CLASS_PRIEST, Classes.CLASS_WARLOCK
                        Return (Agility.Base - 10)
                    Case Classes.CLASS_DRUID
                        If ShapeshiftForm = ShapeshiftForm.FORM_CAT Or ShapeshiftForm = ShapeshiftForm.FORM_BEAR Or ShapeshiftForm = ShapeshiftForm.FORM_DIREBEAR Or ShapeshiftForm = ShapeshiftForm.FORM_MOONKIN Then
                            Return 0
                        Else
                            Return (Agility.Base - 10)
                        End If
                End Select
            End Get
        End Property

        Public ReadOnly Property AttackTime(ByVal index As WeaponAttackType) As Short
            Get
                Return Fix(AttackTimeBase(index) * AttackTimeMods(index))
            End Get
        End Property

        Public AttackTimeBase() As Short = {2000, 0, 0}
        Public AttackTimeMods() As Single = {1.0F, 1.0F, 1.0F}

        'Item Bonuses
        Public ManaRegenerationModifier As Single = Config.ManaRegenerationRate
        Public LifeRegenerationModifier As Single = Config.HealthRegenerationRate
        Public ManaRegenBonus As Integer = 0
        Public ManaRegenPercent As Single = 1
        Public ManaRegen As Integer = 0
        Public ManaRegenInterrupt As Integer = 0
        Public LifeRegenBonus As Integer = 0
        Public RageRegenBonus As Integer = 0

        Public Function GetStat(ByVal Type As Byte) As Short
            Select Case Type
                Case 0
                    Return Strength.Base
                Case 1
                    Return Agility.Base
                Case 2
                    Return Stamina.Base
                Case 3
                    Return Intellect.Base
                Case 4
                    Return Spirit.Base
                Case Else
                    Return 0
            End Select
        End Function

        Public Sub UpdateManaRegen()
            If FullyLoggedIn = False Then Exit Sub
            Dim PowerRegen As Single = Math.Sqrt(Intellect.Base) * 1 'GetOCTRegenMP()
            If Single.IsNaN(PowerRegen) Then PowerRegen = 1 'Clear an invalid PowerRegen
            PowerRegen *= ManaRegenPercent
            Dim PowerRegenMP5 As Single = (ManaRegenBonus / 5)
            Dim PowerRegenInterrupt As Integer = 0

            For i As Integer = 0 To MAX_AURA_EFFECTs - 1
                If Not ActiveSpells(i) Is Nothing Then
                    For j As Byte = 0 To 2
                        If Not ActiveSpells(i).Aura_Info(j) Is Nothing Then
                            If ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraEffects_Names.SPELL_AURA_MOD_MANA_REGEN_FROM_STAT Then
                                PowerRegenMP5 += GetStat(ActiveSpells(i).Aura_Info(j).MiscValue) * ActiveSpells(i).Aura_Info(j).GetValue(Level) / 500.0F

                            ElseIf ActiveSpells(i).SpellID = 34074 AndAlso ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraEffects_Names.SPELL_AURA_PERIODIC_DUMMY Then
                                PowerRegenMP5 += (ActiveSpells(i).Aura_Info(j).GetValue(Level) * Intellect.Base / 500.0F) + (Level * 35 / 100)

                            ElseIf ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraEffects_Names.SPELL_AURA_MOD_MANA_REGEN_INTERRUPT Then
                                PowerRegenInterrupt += ActiveSpells(i).Aura_Info(j).GetValue(Level)
                            End If
                        End If
                    Next
                End If
            Next

            If PowerRegenInterrupt > 100 Then PowerRegenInterrupt = 100

            PowerRegenInterrupt = (PowerRegenMP5 + PowerRegen * PowerRegenInterrupt / 100.0F)
            PowerRegen = CInt(PowerRegenMP5 + PowerRegen)
            ManaRegen = PowerRegen
            ManaRegenInterrupt = PowerRegenInterrupt
        End Sub

        'Temporaly variables
        Public Spell_Language As LANGUAGES = -1

        'Pets
        Public Pet As PetObject = Nothing

        'Honor And Arena
        Public HonorPoints As Integer = 0
        Public StandingLastWeek As Integer = 0
        Public HonorKillsLifeTime As Integer = 0
        Public DishonorKillsLifeTime As Integer = 0
        Public HonorPointsLastWeek As Integer = 0
        Public HonorPointsThisWeek As Integer = 0
        Public HonorPointsYesterday As Integer = 0
        Public HonorKillsLastWeek As Integer = 0
        Public HonorKillsThisWeek As Integer = 0
        Public HonorKillsYesterday As Short = 0
        Public HonorKillsToday As Short = 0
        Public DishonorKillsToday As Short = 0

        Public Sub HonorSaveAsNew()
            CharacterDatabase.Update("INSERT INTO characters_honor (char_guid)  VALUES (" & GUID & ");")
        End Sub

        'Done: Player Honor Save
        Public Sub HonorSave()
            Dim honor As String = "UPDATE characters_honor SET"

            honor = honor & ", honor_points =" & HonorPoints
            honor = honor & ", kills_honor =" & HonorKillsLifeTime
            honor = honor & ", kills_dishonor =" & DishonorKillsLifeTime
            honor = honor & ", honor_yesterday =" & HonorPointsYesterday
            honor = honor & ", honor_thisWeek =" & HonorPointsThisWeek
            honor = honor & ", kills_thisWeek =" & HonorKillsThisWeek
            honor = honor & ", kills_today =" & HonorKillsToday
            honor = honor & ", kills_dishonortoday =" & DishonorKillsToday

            honor = honor + String.Format(" WHERE char_guid = ""{0}"";", GUID)
            CharacterDatabase.Update(honor)
        End Sub

        'Done: Player Honor Load
        Public Sub HonorLoad()
            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query(String.Format("SELECT * FROM characters_honor WHERE char_guid = {0};", GUID), MySQLQuery)
            If MySQLQuery.Rows.Count = 0 Then
                Log.WriteLine(LogType.FAILED, "Unable to get SQLDataBase honor info for character [GUID={0:X}]", GUID)
                Exit Sub
            End If

            HonorPoints = MySQLQuery.Rows(0).Item("honor_points")
            HonorRank = MySQLQuery.Rows(0).Item("honor_rank")
            HonorHighestRank = MySQLQuery.Rows(0).Item("honor_hightestRank")
            StandingLastWeek = MySQLQuery.Rows(0).Item("standing_lastweek")
            HonorKillsLifeTime = MySQLQuery.Rows(0).Item("kills_honor")
            DishonorKillsLifeTime = MySQLQuery.Rows(0).Item("kills_dishonor")
            HonorPointsLastWeek = MySQLQuery.Rows(0).Item("honor_lastWeek")
            HonorPointsThisWeek = MySQLQuery.Rows(0).Item("honor_thisWeek")
            HonorPointsYesterday = MySQLQuery.Rows(0).Item("honor_yesterday")
            HonorKillsLastWeek = MySQLQuery.Rows(0).Item("kills_lastWeek")
            HonorKillsThisWeek = MySQLQuery.Rows(0).Item("kills_thisWeek")
            HonorKillsYesterday = MySQLQuery.Rows(0).Item("kills_yesterday")
            HonorKillsToday = MySQLQuery.Rows(0).Item("kills_today")
            DishonorKillsToday = MySQLQuery.Rows(0).Item("kills_dishonortoday")

            MySQLQuery.Dispose()
        End Sub

        Public Sub HonorLog(ByVal honorPoints As Integer, ByVal victimGUID As ULong, ByVal victimRank As Integer)
            'GUID = 0 : You have been awarded %h honor points.
            'GUID <>0 : %p dies, honorable kill Rank: %r (Estimated Honor Points: %h)

            Dim packet As New PacketClass(OPCODES.SMSG_PVP_CREDIT)
            Try
                packet.AddInt32(honorPoints)
                packet.AddUInt64(victimGUID)
                packet.AddInt32(victimRank)
                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Copper As UInteger = 0
        Public Name As String = ""

        Public ActionButtons As New Dictionary(Of Byte, TActionButton)
        Public TaxiZones As BitArray = New BitArray(8 * 32, False)
        Public TaxiNodes As New Queue(Of Integer)
        Public ZonesExplored() As UInteger = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

        Public WalkSpeed As Single = UNIT_NORMAL_WALK_SPEED
        Public RunSpeed As Single = UNIT_NORMAL_RUN_SPEED
        Public RunBackSpeed As Single = UNIT_NORMAL_WALK_BACK_SPEED
        Public SwimSpeed As Single = UNIT_NORMAL_SWIM_SPEED
        Public SwimBackSpeed As Single = UNIT_NORMAL_SWIM_BACK_SPEED
        Public TurnRate As Single = UNIT_NORMAL_TURN_RATE

        Public charMovementFlags As Integer = 0
        Public ZoneID As Integer = 0
        Public AreaID As Integer = 0
        Public bindpoint_positionX As Single = 0
        Public bindpoint_positionY As Single = 0
        Public bindpoint_positionZ As Single = 0
        Public bindpoint_map_id As Integer = 0
        Public bindpoint_zone_id As Integer = 0
        Public DEAD As Boolean = False

        Public ReadOnly Property ClassMask() As UInteger
            Get
                Return (1 << Classe - 1)
            End Get
        End Property

        Public ReadOnly Property RaceMask() As UInteger
            Get
                Return (1 << Race - 1)
            End Get
        End Property

        Public Overrides ReadOnly Property IsDead() As Boolean
            Get
                Return DEAD
            End Get
        End Property

        Public ReadOnly Property isMoving() As Boolean
            Get
                Return (movementFlagsMask And charMovementFlags)
            End Get
        End Property

        Public ReadOnly Property isTurning() As Boolean
            Get
                Return (TurningFlagsMask And charMovementFlags)
            End Get
        End Property

        Public ReadOnly Property isMovingOrTurning() As Boolean
            Get
                Return (movementOrTurningFlagsMask And charMovementFlags)
            End Get
        End Property

        Public Property isPvP() As Boolean
            Get
                Return (cUnitFlags And UnitFlags.UNIT_FLAG_PVP)
            End Get
            Set(ByVal Enabled As Boolean)
                If Enabled Then
                    cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_PVP
                Else
                    cUnitFlags = cUnitFlags And (Not UnitFlags.UNIT_FLAG_PVP)
                End If
            End Set
        End Property

        Public ReadOnly Property isResting() As Boolean
            Get
                Return (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_RESTING)
            End Get
        End Property

        Public exploreCheckQueued_ As Boolean = False
        Public outsideMapID_ As Boolean = False
        Public antiHackSpeedChanged_ As Integer = 0

        Public underWaterTimer As TDrowningTimer = Nothing
        Public underWaterBreathing As Boolean = False
        Public lootGUID As ULong = 0
        Public repopTimer As TRepopTimer = Nothing
        Public tradeInfo As TTradeInfo = Nothing
        Public corpseGUID As ULong = 0
        Public corpseMapID As Integer = 0
        Public corpseCorpseType As CorpseType = CorpseType.CORPSE_BONES
        Public corpsePositionX As Single = 0
        Public corpsePositionY As Single = 0
        Public corpsePositionZ As Single = 0
        Public resurrectGUID As ULong = 0
        Public resurrectMapID As Integer = 0
        Public resurrectPositionX As Single = 0
        Public resurrectPositionY As Single = 0
        Public resurrectPositionZ As Single = 0
        Public resurrectHealth As Integer = 0
        Public resurrectMana As Integer = 0

        Public guidsForRemoving_Lock As New ReaderWriterLock
        Public guidsForRemoving As New List(Of ULong)
        Public creaturesNear As New List(Of ULong)
        Public playersNear As New List(Of ULong)
        Public gameObjectsNear As New List(Of ULong)
        Public dynamicObjectsNear As New List(Of ULong)
        Public corpseObjectsNear As New List(Of ULong)
        Public inCombatWith As New List(Of ULong)
        Public lastPvpAction As Integer = 0

        Public Overrides Function IsFriendlyTo(ByRef Unit As BaseUnit) As Boolean
            If Unit Is Me Then Return True

            If TypeOf Unit Is CharacterObject Then
                With CType(Unit, CharacterObject)
                    If .GM Then Return True
                    If DuelPartner IsNot Nothing AndAlso DuelPartner Is Unit Then Return False
                    If .DuelPartner IsNot Nothing AndAlso .DuelPartner Is Me Then Return False
                    If IsInGroup AndAlso .IsInGroup AndAlso Group Is .Group Then Return True
                    If HaveFlags(cPlayerFlags, PlayerFlags.PLAYER_FLAGS_FFA_PVP) AndAlso HaveFlags(.cPlayerFlags, PlayerFlags.PLAYER_FLAGS_FFA_PVP) Then Return False
                    If Team = .Team Then Return True
                    Return Not .isPvP
                End With
            ElseIf TypeOf Unit Is CreatureObject Then
                With CType(Unit, CreatureObject)
                    If GetReputation(.Faction) < ReputationRank.Friendly Then Return False
                    If GetReaction(.Faction) < TReaction.NEUTRAL Then Return False

                    'TODO: At war with faction?
                End With
            End If

            Return True
        End Function

        Public Overrides Function IsEnemyTo(ByRef Unit As BaseUnit) As Boolean
            If Unit Is Me Then Return False

            If TypeOf Unit Is CharacterObject Then
                With CType(Unit, CharacterObject)
                    If .GM Then Return False
                    If DuelPartner IsNot Nothing AndAlso DuelPartner Is Unit Then Return True
                    If .DuelPartner IsNot Nothing AndAlso .DuelPartner Is Me Then Return True
                    If IsInGroup AndAlso .IsInGroup AndAlso Group Is .Group Then Return False
                    If HaveFlags(cPlayerFlags, PlayerFlags.PLAYER_FLAGS_FFA_PVP) AndAlso HaveFlags(.cPlayerFlags, PlayerFlags.PLAYER_FLAGS_FFA_PVP) Then Return True
                    If Team = .Team Then Return False
                    Return .isPvP
                End With
            ElseIf TypeOf Unit Is CreatureObject Then
                With CType(Unit, CreatureObject)
                    If GetReputation(.Faction) < ReputationRank.Neutral Then Return True
                    If GetReaction(.Faction) < TReaction.NEUTRAL Then Return True

                    'TODO: At war with faction?
                End With
            End If

            Return False
        End Function

        Public ReadOnly Property IsInCombat() As Boolean
            Get
                Return (inCombatWith.Count > 0 OrElse (timeGetTime("") - lastPvpAction) < DEFAULT_PVP_COMBAT_TIME)
            End Get
        End Property

        Public Sub AddToCombat(ByVal Unit As BaseUnit)
            If TypeOf Unit Is CharacterObject Then
                lastPvpAction = timeGetTime("")
            Else
                If inCombatWith.Contains(Unit.GUID) Then Exit Sub

                inCombatWith.Add(Unit.GUID)
            End If
            CheckCombat()
        End Sub

        Public Sub RemoveFromCombat(ByVal Unit As BaseUnit)
            If Not inCombatWith.Contains(Unit.GUID) Then Exit Sub

            inCombatWith.Remove(Unit.GUID)
            CheckCombat()
        End Sub

        'NOTE: This function removes combat if there's no one else in your combat array
        Public Sub CheckCombat()
            If (cUnitFlags And UnitFlags.UNIT_FLAG_IN_COMBAT) Then
                If IsInCombat Then Exit Sub

                SetPlayerOutOfCombat(Me)
            Else
                If Not IsInCombat Then Exit Sub

                SetPlayerInCombat(Me)
            End If
        End Sub

        Public Overrides Function CanSee(ByRef objCharacter As BaseObject) As Boolean
            If GUID = objCharacter.GUID Then Return False
            If instance <> objCharacter.instance Then Return False
            If objCharacter.MapID <> MapID Then Return False

            If TypeOf objCharacter Is CreatureObject Then
                If Not CType(objCharacter, CreatureObject).aiScript Is Nothing Then
                    If CType(objCharacter, CreatureObject).aiScript.State = AIState.AI_RESPAWN Then Return False
                End If
            ElseIf TypeOf objCharacter Is GameObjectObject Then
                If CType(objCharacter, GameObjectObject).Despawned Then Return False
            End If

            Dim distance As Single = GetDistance(Me, objCharacter)

            'DONE: See party members
            If (Group IsNot Nothing) AndAlso (TypeOf objCharacter Is CharacterObject) Then
                If (CType(objCharacter, CharacterObject).Group Is Group) Then
                    If distance > objCharacter.VisibleDistance Then Return False Else Return True
                End If
            End If

            'DONE: Check dead
            If DEAD AndAlso corpseGUID <> 0UL Then
                'DONE: See only dead
                If corpseGUID = objCharacter.GUID Then Return True
                If GetDistance(objCharacter, corpsePositionX, corpsePositionY, corpsePositionZ) < objCharacter.VisibleDistance Then
                    'DONE: GM and DEAD invisibility
                    If objCharacter.Invisibility > CanSeeInvisibility Then Return False
                    'DONE: Stealth Detection
                    If objCharacter.Invisibility = InvisibilityLevel.STEALTH AndAlso (distance < GetStealthDistance(objCharacter)) Then Return True
                    'DONE: Check invisibility
                    If objCharacter.Invisibility = InvisibilityLevel.INIVISIBILITY AndAlso objCharacter.Invisibility_Value > CanSeeInvisibility_Invisibility Then Return False
                    If objCharacter.Invisibility = InvisibilityLevel.STEALTH AndAlso CanSeeStealth = False Then Return False
                    Return True
                End If
                If objCharacter.Invisibility <> InvisibilityLevel.DEAD Then Return False
            ElseIf Invisibility = InvisibilityLevel.INIVISIBILITY Then
                'DONE: See only invisible, or people who can see invisibility
                If objCharacter.Invisibility <> InvisibilityLevel.INIVISIBILITY Then
                    If objCharacter.CanSeeInvisibility_Invisibility >= Invisibility_Value Then Return True
                    Return False
                End If
                If Invisibility_Value < objCharacter.Invisibility_Value Then Return False
            Else
                'DONE: GM and DEAD invisibility
                If objCharacter.Invisibility > CanSeeInvisibility Then Return False
                'DONE: Stealth Detection
                If objCharacter.Invisibility = InvisibilityLevel.STEALTH AndAlso (distance < GetStealthDistance(objCharacter)) Then Return True
                'DONE: Check invisibility
                If objCharacter.Invisibility = InvisibilityLevel.INIVISIBILITY AndAlso objCharacter.Invisibility_Value > CanSeeInvisibility_Invisibility Then Return False
                If objCharacter.Invisibility = InvisibilityLevel.STEALTH AndAlso CanSeeStealth = False Then Return False
            End If

            'DONE: Check distance
            If distance > objCharacter.VisibleDistance Then Return False
            Return True
        End Function

        Public TutorialFlags() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

        'Updating
        Private UpdateMask As New BitArray(FIELD_MASK_SIZE_PLAYER, False)
        Private UpdateData As New Hashtable
        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As Integer)
            UpdateMask.Set(pos, True)
            UpdateData(pos) = (value)
        End Sub

        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As UInteger)
            UpdateMask.Set(pos, True)
            UpdateData(pos) = (value)
        End Sub

        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As Long)
            UpdateMask.Set(pos, True)
            UpdateMask.Set(pos + 1, True)
            UpdateData(pos) = (CType((value And UInteger.MaxValue), Integer))
            UpdateData(pos + 1) = (CType(((value >> 32) And UInteger.MaxValue), Integer))
        End Sub

        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As ULong)
            UpdateMask.Set(pos, True)
            UpdateMask.Set(pos + 1, True)
            UpdateData(pos) = (CType((value And UInteger.MaxValue), UInteger))
            UpdateData(pos + 1) = (CType(((value >> 32) And UInteger.MaxValue), UInteger))
        End Sub

        Public Sub SetUpdateFlag(ByVal pos As Integer, ByVal value As Single)
            UpdateMask.Set(pos, True)
            UpdateData(pos) = (value)
        End Sub

        Public Sub SendOutOfRangeUpdate()
            Dim GUIDs() As ULong

            guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
            GUIDs = guidsForRemoving.ToArray()
            guidsForRemoving.Clear()
            guidsForRemoving_Lock.ReleaseWriterLock()

            If GUIDs.Length > 0 Then
                Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                Try
                    packet.AddInt32(1)      'Operations.Count
                    packet.AddInt8(0)
                    packet.AddInt8(ObjectUpdateType.UPDATETYPE_OUT_OF_RANGE_OBJECTS)
                    packet.AddInt32(GUIDs.Length)

                    For Each g As ULong In GUIDs
                        packet.AddPackGUID(g)
                    Next

                    client.Send(packet)
                Finally
                    packet.Dispose()
                End Try
            End If
        End Sub

        Public Sub SendUpdate()
            Dim updateCount As Integer = 1 + Items.Count
            If OnTransport IsNot Nothing Then updateCount += 1

            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            Try
                packet.AddInt32(updateCount)
                packet.AddInt8(0)

                'DONE: If character is on a transport, create the transport right here
                If OnTransport IsNot Nothing Then
                    Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
                    OnTransport.FillAllUpdateFlags(tmpUpdate, Me)
                    tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, OnTransport)
                    tmpUpdate.Dispose()

                    gameObjectsNear.Add(OnTransport.GUID)
                    OnTransport.SeenBy.Add(GUID)
                End If

                PrepareUpdate(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF)

                For Each tmpItem As KeyValuePair(Of Byte, ItemObject) In Items
                    Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
                    tmpItem.Value.FillAllUpdateFlags(tmpUpdate)
                    tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, (tmpItem.Value))
                    tmpUpdate.Dispose()

                    'DONE: Update Items In bag
                    If tmpItem.Value.ItemInfo.IsContainer Then
                        tmpItem.Value.SendContainedItemsUpdate(client, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT)
                    End If
                Next

                packet.CompressUpdatePacket()
                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
            'DONE: Create everyone on the transport if we are located on one
            If (OnTransport IsNot Nothing) AndAlso (TypeOf OnTransport Is TransportObject) Then
                CType(OnTransport, TransportObject).CreateEveryoneOnTransport(Me)
            End If
        End Sub

        Public Sub SendItemUpdate(ByVal Item As ItemObject)
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            Try
                packet.AddInt32(1)      'Operations.Count
                packet.AddInt8(0)

                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
                Item.FillAllUpdateFlags(tmpUpdate)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Item)
                tmpUpdate.Dispose()

                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub SendInventoryUpdate()
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            Try
                packet.AddInt32(1)      'Operations.Count
                packet.AddInt8(0)

                For i As Byte = 0 To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                    If Items.ContainsKey(i) Then
                        SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, Items(i).GUID)
                        If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + (i * PLAYER_VISIBLE_ITEM_SIZE), Items(i).ItemEntry)

                            'SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_1 + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)           'ITEM_FIELD_ENCHANTMENT
                            'SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_2 + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)           'ITEM_FIELD_ENCHANTMENT + 3
                            'SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_3 + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)           'ITEM_FIELD_ENCHANTMENT + 6
                            'SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_4 + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)           'ITEM_FIELD_ENCHANTMENT + 9
                            'SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_5 + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)           'ITEM_FIELD_ENCHANTMENT + 12
                            'SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_6 + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)           'ITEM_FIELD_ENCHANTMENT + 15
                            'SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_7 + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)           'ITEM_FIELD_ENCHANTMENT + 18
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + (i * PLAYER_VISIBLE_ITEM_SIZE), 0)   'ITEM_FIELD_RANDOM_PROPERTIES_ID
                        End If
                    Else
                        SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, CType(0, Long))
                        If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + i * PLAYER_VISIBLE_ITEM_SIZE, 0)
                        End If
                    End If
                Next

                PrepareUpdate(packet, ObjectUpdateType.UPDATETYPE_VALUES)

                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub SendItemAndCharacterUpdate(ByVal Item As ItemObject, Optional ByVal UPDATETYPE As Integer = ObjectUpdateType.UPDATETYPE_VALUES)
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
            Try
                packet.AddInt32(2)      'Operations.Count
                packet.AddInt8(0)

                'DONE: Send to self
                Item.FillAllUpdateFlags(tmpUpdate)
                tmpUpdate.AddToPacket(packet, UPDATETYPE, Item)

                For i As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                    If Items.ContainsKey(i) Then
                        SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, Items(i).GUID)
                        If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + (i * PLAYER_VISIBLE_ITEM_SIZE), Items(i).ItemEntry)
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + (i * PLAYER_VISIBLE_ITEM_SIZE), Items(i).RandomProperties)   'ITEM_FIELD_RANDOM_PROPERTIES_ID
                        End If
                    Else
                        SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, CType(0, ULong))
                        If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + i * PLAYER_VISIBLE_ITEM_SIZE, 0)
                        End If
                    End If
                Next

                PrepareUpdate(packet, ObjectUpdateType.UPDATETYPE_VALUES)

                client.Send(packet)
            Finally
                packet.Dispose()
                tmpUpdate.Dispose()
            End Try

            'DONE: Send to others
            For i As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                If Items.ContainsKey(i) Then
                    SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + (i * PLAYER_VISIBLE_ITEM_SIZE), Items(i).ItemEntry)
                    SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + (i * PLAYER_VISIBLE_ITEM_SIZE), Items(i).RandomProperties)   'ITEM_FIELD_RANDOM_PROPERTIES_ID
                Else
                    SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + i * PLAYER_VISIBLE_ITEM_SIZE, 0)
                End If
            Next
            SendCharacterUpdate(True, True)
        End Sub

        Public Sub SendCharacterUpdate(Optional ByVal toNear As Boolean = True, Optional ByVal notMe As Boolean = False)
            If UpdateData.Count = 0 Then Exit Sub

            'DONE: Send to near
            If toNear AndAlso SeenBy.Count > 0 Then
                Dim forOthers As New UpdateClass With {
                    .UpdateData = UpdateData.Clone,
                    .UpdateMask = UpdateMask.Clone
                }

                Dim packetForOthers As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                Try
                    packetForOthers.AddInt32(1)       'Operations.Count
                    packetForOthers.AddInt8(0)
                    forOthers.AddToPacket(packetForOthers, ObjectUpdateType.UPDATETYPE_VALUES, Me)
                    SendToNearPlayers(packetForOthers)
                Finally
                    packetForOthers.Dispose()
                End Try
            End If
            If notMe Then Exit Sub
            If client Is Nothing Then Exit Sub

            'DONE: Send to me
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            Try
                packet.AddInt32(1)       'Operations.Count
                packet.AddInt8(0)
                PrepareUpdate(packet, ObjectUpdateType.UPDATETYPE_VALUES)
                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub                                      'Sends update for character to him and near players

        Public Sub FillStatsUpdateFlags()
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Life.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1, Mana.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER2, Rage.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER4, Energy.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER5, 0)

            SetUpdateFlag(EPlayerFields.PLAYER_BLOCK_PERCENTAGE, combatBlock)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MINDAMAGE, Damage.Minimum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXDAMAGE, Damage.Maximum + BaseUnarmedDamage)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MINOFFHANDDAMAGE, OffHandDamage.Minimum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXOFFHANDDAMAGE, OffHandDamage.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MINRANGEDDAMAGE, RangedDamage.Minimum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXRANGEDDAMAGE, RangedDamage.Maximum + BaseRangedDamage)

            SetUpdateFlag(EUnitFields.UNIT_FIELD_BASEATTACKTIME, AttackTime(0))
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGEDATTACKTIME, AttackTime(1))
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGEDATTACKTIME, AttackTime(2))

            SetUpdateFlag(EPlayerFields.PLAYER_BLOCK_PERCENTAGE, GetBasePercentBlock(Me, 0))
            SetUpdateFlag(EPlayerFields.PLAYER_DODGE_PERCENTAGE, GetBasePercentDodge(Me, 0))
            SetUpdateFlag(EPlayerFields.PLAYER_PARRY_PERCENTAGE, GetBasePercentParry(Me, 0))
            SetUpdateFlag(EPlayerFields.PLAYER_CRIT_PERCENTAGE, GetBasePercentCrit(Me, 0))

            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Copper)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT0, Strength.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT1, Agility.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT2, Stamina.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT3, Intellect.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT4, Spirit.Base)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT0, CType(Strength.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT1, CType(Agility.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT2, CType(Stamina.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT3, CType(Intellect.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT4, CType(Spirit.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT0, CType(Strength.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT1, CType(Agility.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT2, CType(Stamina.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT3, CType(Intellect.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT4, CType(Spirit.NegativeBonus, Integer))

            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_PHYSICAL, Resistances(DamageTypes.DMG_PHYSICAL).RealBase)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_HOLY, Resistances(DamageTypes.DMG_HOLY).RealBase)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_FIRE, Resistances(DamageTypes.DMG_FIRE).RealBase)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_NATURE, Resistances(DamageTypes.DMG_NATURE).RealBase)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_FROST, Resistances(DamageTypes.DMG_FROST).RealBase)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_SHADOW, Resistances(DamageTypes.DMG_SHADOW).RealBase)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_ARCANE, Resistances(DamageTypes.DMG_ARCANE).RealBase)

            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_PHYSICAL, Resistances(DamageTypes.DMG_PHYSICAL).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_HOLY, Resistances(DamageTypes.DMG_HOLY).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_FIRE, Resistances(DamageTypes.DMG_FIRE).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_NATURE, Resistances(DamageTypes.DMG_NATURE).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_FROST, Resistances(DamageTypes.DMG_FROST).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_SHADOW, Resistances(DamageTypes.DMG_SHADOW).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_ARCANE, Resistances(DamageTypes.DMG_ARCANE).PositiveBonus)

            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_PHYSICAL, Resistances(DamageTypes.DMG_PHYSICAL).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_HOLY, Resistances(DamageTypes.DMG_HOLY).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_FIRE, Resistances(DamageTypes.DMG_FIRE).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_NATURE, Resistances(DamageTypes.DMG_NATURE).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_FROST, Resistances(DamageTypes.DMG_FROST).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_SHADOW, Resistances(DamageTypes.DMG_SHADOW).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_ARCANE, Resistances(DamageTypes.DMG_ARCANE).NegativeBonus)
        End Sub                                     'Used for this player's stats updates

        Public Sub FillAllUpdateFlags()

            SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
            SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, 25)
            SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, Size)

            If Pet IsNot Nothing Then SetUpdateFlag(EUnitFields.UNIT_FIELD_SUMMON, Pet.GUID)

            SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, Mana.Current)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER2, Rage.Current)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER4, Energy.Current)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER5, 0)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Life.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1, Mana.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER2, Rage.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER4, Energy.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER5, 0)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_BASE_HEALTH, Life.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_BASE_MANA, Mana.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_LEVEL, Level)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_FACTIONTEMPLATE, Faction)

            SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS_2, 0)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT0, Strength.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT1, Agility.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT2, Stamina.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT3, Spirit.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAT4, Intellect.Base)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT0, CType(Strength.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT1, CType(Agility.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT2, CType(Stamina.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT3, CType(Intellect.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT4, CType(Spirit.PositiveBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT0, CType(Strength.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT1, CType(Agility.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT2, CType(Stamina.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT3, CType(Intellect.NegativeBonus, Integer))
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT4, CType(Spirit.NegativeBonus, Integer))
            SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_0, cBytes0)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, cBytes1)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_2, cBytes2)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, Model)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_NATIVEDISPLAYID, Model_Native)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, Mount)
            SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, cDynamicFlags)

            SetUpdateFlag(EPlayerFields.PLAYER_BYTES, cPlayerBytes)
            SetUpdateFlag(EPlayerFields.PLAYER_BYTES_2, cPlayerBytes2)
            SetUpdateFlag(EPlayerFields.PLAYER_BYTES_3, cPlayerBytes3)

            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_WATCHED_FACTION_INDEX, WatchedFactionIndex)

            SetUpdateFlag(EPlayerFields.PLAYER_XP, XP)
            SetUpdateFlag(EPlayerFields.PLAYER_NEXT_LEVEL_XP, XPTable(Level))
            SetUpdateFlag(EPlayerFields.PLAYER_REST_STATE_EXPERIENCE, RestBonus)

            SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BYTES, cPlayerFieldBytes)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BYTES2, cPlayerFieldBytes2)

            SetUpdateFlag(EUnitFields.UNIT_FIELD_BOUNDINGRADIUS, BoundingRadius)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_COMBATREACH, CombatReach)

            SetUpdateFlag(EPlayerFields.PLAYER_CHARACTER_POINTS1, TalentPoints)
            'SetUpdateFlag(EPlayerFields.PLAYER_CHARACTER_POINTS2, 0)

            SetUpdateFlag(EPlayerFields.PLAYER_GUILDID, GuildID)
            SetUpdateFlag(EPlayerFields.PLAYER_GUILDRANK, GuildRank)

            SetUpdateFlag(EUnitFields.UNIT_FIELD_MINDAMAGE, Damage.Minimum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXDAMAGE, Damage.Maximum + BaseUnarmedDamage)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_BASEATTACKTIME, AttackTime(WeaponAttackType.BASE_ATTACK))
            SetUpdateFlag(EUnitFields.UNIT_FIELD_BASEATTACKTIME + 1, AttackTime(WeaponAttackType.OFF_ATTACK))
            SetUpdateFlag(EUnitFields.UNIT_MOD_CAST_SPEED, 1.0F)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER, AttackPower)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER, AttackPowerRanged)
            SetUpdateFlag(EPlayerFields.PLAYER_CRIT_PERCENTAGE, GetBasePercentCrit(Me, 0))
            SetUpdateFlag(EPlayerFields.PLAYER_RANGED_CRIT_PERCENTAGE, GetBasePercentCrit(Me, 0))
            'SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_HEALING_DONE_POS, healing.PositiveBonus)

            For i As Byte = 0 To 6
                'SetUpdateFlag(EPlayerFields.PLAYER_SPELL_CRIT_PERCENTAGE1 + i, CType(0, Single))
                SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_POS + i, spellDamage(i).PositiveBonus)
                SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG + i, spellDamage(i).NegativeBonus)
                SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT + i, spellDamage(i).Modifier)
            Next

            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_PHYSICAL, Resistances(DamageTypes.DMG_PHYSICAL).Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_HOLY, Resistances(DamageTypes.DMG_HOLY).Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_FIRE, Resistances(DamageTypes.DMG_FIRE).Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_NATURE, Resistances(DamageTypes.DMG_NATURE).Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_FROST, Resistances(DamageTypes.DMG_FROST).Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_SHADOW, Resistances(DamageTypes.DMG_SHADOW).Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_ARCANE, Resistances(DamageTypes.DMG_ARCANE).Base)

            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_PHYSICAL, Resistances(DamageTypes.DMG_PHYSICAL).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_HOLY, Resistances(DamageTypes.DMG_HOLY).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_FIRE, Resistances(DamageTypes.DMG_FIRE).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_NATURE, Resistances(DamageTypes.DMG_NATURE).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_FROST, Resistances(DamageTypes.DMG_FROST).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_SHADOW, Resistances(DamageTypes.DMG_SHADOW).PositiveBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + DamageTypes.DMG_ARCANE, Resistances(DamageTypes.DMG_ARCANE).PositiveBonus)

            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_PHYSICAL, Resistances(DamageTypes.DMG_PHYSICAL).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_HOLY, Resistances(DamageTypes.DMG_HOLY).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_FIRE, Resistances(DamageTypes.DMG_FIRE).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_NATURE, Resistances(DamageTypes.DMG_NATURE).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_FROST, Resistances(DamageTypes.DMG_FROST).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_SHADOW, Resistances(DamageTypes.DMG_SHADOW).NegativeBonus)
            'SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + DamageTypes.DMG_ARCANE, Resistances(DamageTypes.DMG_ARCANE).NegativeBonus)

            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Copper)

            For Each Skill As KeyValuePair(Of Integer, TSkill) In Skills
                SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + SkillsPositions(Skill.Key) * 3, Skill.Key)                                    'skill1.Id
                SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + SkillsPositions(Skill.Key) * 3 + 1, Skill.Value.GetSkill)      'CType((skill1.CurrentVal(Me) + (skill1.Cap(Me) << 16)), Integer)
                SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + SkillsPositions(Skill.Key) * 3 + 2, Skill.Value.Bonus)         'skill1.Bonus
            Next

            SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGEDATTACKTIME, AttackTime(WeaponAttackType.RANGED_ATTACK))
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MINOFFHANDDAMAGE, OffHandDamage.Minimum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXOFFHANDDAMAGE, OffHandDamage.Maximum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STRENGTH, Strength.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_AGILITY, Agility.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_STAMINA, Stamina.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_SPIRIT, Spirit.Base)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_INTELLECT, Intellect.Base)

            SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER_MODS, AttackPowerMods)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MODS, AttackPowerModsRanged)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MINRANGEDDAMAGE, RangedDamage.Minimum)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXRANGEDDAMAGE, RangedDamage.Maximum + BaseRangedDamage)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER_MULTIPLIER, 0.0F)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER, 0.0F)

            For i As Byte = 0 To QuestInfo.QUEST_SLOTS
                If TalkQuests(i) Is Nothing Then
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_1 + i * 3, 0) 'ID
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + i * 3, 0) 'State
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + i * 3 + 1, 0) 'Timer
                Else
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_1 + i * 3, TalkQuests(i).ID)
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + i * 3, TalkQuests(i).GetProgress)
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + i * 3 + 1, 0) 'Timer
                End If
            Next i

            SetUpdateFlag(EPlayerFields.PLAYER_BLOCK_PERCENTAGE, GetBasePercentBlock(Me, 0))
            SetUpdateFlag(EPlayerFields.PLAYER_DODGE_PERCENTAGE, GetBasePercentDodge(Me, 0))
            SetUpdateFlag(EPlayerFields.PLAYER_PARRY_PERCENTAGE, GetBasePercentParry(Me, 0))

            For i As Byte = 0 To PLAYER_EXPLORED_ZONES_SIZE
                SetUpdateFlag(EPlayerFields.PLAYER_EXPLORED_ZONES_1 + i, ZonesExplored(i))
            Next i

            'SetUpdateFlag(EPlayerFields.PLAYER_FIELD_PVP_MEDALS, 0)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_LIFETIME_HONORBALE_KILLS, HonorKillsLifeTime)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_LIFETIME_DISHONORBALE_KILLS, DishonorKillsLifeTime)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_SESSION_KILLS, HonorKillsToday + (CInt(DishonorKillsToday) << 16))
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_THIS_WEEK_KILLS, HonorKillsThisWeek)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_LAST_WEEK_KILLS, HonorKillsLastWeek)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_YESTERDAY_KILLS, HonorKillsYesterday)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_THIS_WEEK_CONTRIBUTION, HonorPointsThisWeek)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_LAST_WEEK_CONTRIBUTION, HonorPointsLastWeek)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_YESTERDAY_CONTRIBUTION, HonorPointsYesterday)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_LAST_WEEK_RANK, StandingLastWeek)

            For i As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                If Items.ContainsKey(i) Then
                    If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                        SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + (i * PLAYER_VISIBLE_ITEM_SIZE), Items(i).ItemEntry)

                        'DONE: Include enchantment info
                        For Each Enchant As KeyValuePair(Of Byte, TEnchantmentInfo) In Items(i).Enchantments
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + 1 + Enchant.Key * 3 + i * PLAYER_VISIBLE_ITEM_SIZE, Enchant.Value.ID)
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + 2 + Enchant.Key * 3 + i * PLAYER_VISIBLE_ITEM_SIZE, Enchant.Value.Charges) 'Correct?
                            SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + 3 + Enchant.Key * 3 + i * PLAYER_VISIBLE_ITEM_SIZE, Enchant.Value.Duration) 'Correct?
                        Next
                        SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + i * PLAYER_VISIBLE_ITEM_SIZE, Items(i).RandomProperties)
                    End If
                    SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, Items(i).GUID)
                Else
                    If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                        SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + i * PLAYER_VISIBLE_ITEM_SIZE, 0)
                        SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + i * PLAYER_VISIBLE_ITEM_SIZE, 0)
                    End If
                    SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, 0)
                End If
            Next

            SetUpdateFlag(EPlayerFields.PLAYER_AMMO_ID, AmmoID)

            For i As Byte = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing Then
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_AURA + i, ActiveSpells(i).SpellID)
                End If
            Next
            For i As Byte = 0 To MAX_AURA_EFFECT_FLAGs - 1
                SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAFLAGS + i, ActiveSpells_Flags(i))
            Next
            For i As Byte = 0 To MAX_AURA_EFFECT_LEVELSs - 1
                SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAAPPLICATIONS + i, ActiveSpells_Count(i))
                SetUpdateFlag(EUnitFields.UNIT_FIELD_AURALEVELS + i, ActiveSpells_Level(i))
            Next

        End Sub                                       'Used for this player's update packets

        Public Sub FillAllUpdateFlags(ByRef Update As UpdateClass)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, Size)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, 25)

            If Pet IsNot Nothing Then SetUpdateFlag(EUnitFields.UNIT_FIELD_SUMMON, Pet.GUID)

            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, Mana.Current)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER2, Rage.Current)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER4, Energy.Current)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER5, 0)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Life.Maximum)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1, Mana.Maximum)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER2, Rage.Maximum)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER4, Energy.Maximum)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER5, 0)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_LEVEL, Level)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_FACTIONTEMPLATE, Faction)

            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_0, cBytes0)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, cBytes1)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_2, cBytes2)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, Model)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_NATIVEDISPLAYID, Model_Native)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, Mount)

            Update.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, cDynamicFlags)

            Update.SetUpdateFlag(EPlayerFields.PLAYER_BYTES, cPlayerBytes)
            Update.SetUpdateFlag(EPlayerFields.PLAYER_BYTES_2, cPlayerBytes2)
            Update.SetUpdateFlag(EPlayerFields.PLAYER_BYTES_3, cPlayerBytes3)

            Update.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)

            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BOUNDINGRADIUS, BoundingRadius)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_COMBATREACH, CombatReach)
            Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_TARGET, TargetGUID)

            Update.SetUpdateFlag(EPlayerFields.PLAYER_GUILDID, GuildID)
            Update.SetUpdateFlag(EPlayerFields.PLAYER_GUILDRANK, GuildRank)

            For i As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                If Items.ContainsKey(i) Then
                    If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                        Update.SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + (i * PLAYER_VISIBLE_ITEM_SIZE), Items(i).ItemEntry)

                        'DONE: Include enchantment info
                        'For Each Enchant As KeyValuePair(Of Byte, TEnchantmentInfo) In Items(i).Enchantments
                        '    Update.SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0_1 + Enchant.Key + i * PLAYER_VISIBLE_ITEM_SIZE, Enchant.Value.ID)
                        'Next
                        Update.SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + i * PLAYER_VISIBLE_ITEM_SIZE, Items(i).RandomProperties)
                    End If
                    Update.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, Items(i).GUID)
                Else
                    If i < EquipmentSlots.EQUIPMENT_SLOT_END Then
                        Update.SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + i * PLAYER_VISIBLE_ITEM_SIZE, 0)
                        Update.SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + i * PLAYER_VISIBLE_ITEM_SIZE, 0)
                    End If
                    Update.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + i * 2, 0)
                End If
            Next

            For i As Byte = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing Then
                    Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURA + i, ActiveSpells(i).SpellID)
                End If
            Next
            For i As Byte = 0 To MAX_AURA_EFFECT_FLAGs - 1
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAFLAGS + i, ActiveSpells_Flags(i))
            Next
            For i As Byte = 0 To MAX_AURA_EFFECT_LEVELSs - 1
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAAPPLICATIONS + i, ActiveSpells_Count(i))
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURALEVELS + i, ActiveSpells_Level(i))
            Next

        End Sub                                       'Used for other players' update packets

        Public Sub PrepareUpdate(ByRef packet As PacketClass, Optional ByVal UPDATETYPE As Integer = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT)
            packet.AddInt8(UPDATETYPE)
            packet.AddPackGUID(GUID)

            If UPDATETYPE = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Or UPDATETYPE = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF Then
                packet.AddInt8(ObjectTypeID.TYPEID_PLAYER)
            End If

            If UPDATETYPE = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Or UPDATETYPE = ObjectUpdateType.UPDATETYPE_MOVEMENT Or UPDATETYPE = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF Then
                Dim flags2 As Integer = &H2000
                If OnTransport IsNot Nothing Then
                    flags2 = flags2 Or MovementFlags.MOVEMENTFLAG_ONTRANSPORT
                End If

                packet.AddInt8(&H71) 'flags
                packet.AddInt32(flags2) 'flags2
                packet.AddInt32(MsTime)
                packet.AddSingle(positionX)
                packet.AddSingle(positionY)
                packet.AddSingle(positionZ)
                packet.AddSingle(orientation)

                If (flags2 And MovementFlags.MOVEMENTFLAG_ONTRANSPORT) Then
                    packet.AddUInt64(OnTransport.GUID)
                    packet.AddSingle(transportX)
                    packet.AddSingle(transportY)
                    packet.AddSingle(transportZ)
                    packet.AddSingle(orientation)
                End If

                packet.AddInt32(0) 'Unk
                packet.AddInt32(0) 'Unk
                packet.AddInt32(0) 'AttackCycle?
                packet.AddInt32(0) 'TimeID?
                packet.AddInt32(0) 'VictimGUID?

                packet.AddSingle(WalkSpeed)
                packet.AddSingle(RunSpeed)
                packet.AddSingle(RunBackSpeed)
                packet.AddSingle(SwimSpeed)
                packet.AddSingle(SwimBackSpeed)
                packet.AddSingle(TurnRate)

                packet.AddUInt32(&H2F)
            End If

            If UPDATETYPE = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT Or UPDATETYPE = ObjectUpdateType.UPDATETYPE_VALUES Or UPDATETYPE = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF Then
                Dim UpdateCount As Integer = 0
                For i As Integer = 0 To UpdateMask.Count - 1
                    If UpdateMask.Get(i) Then UpdateCount = i
                Next

                packet.AddInt8((UpdateCount + 32) \ 32)
                packet.AddBitArray(UpdateMask, CType((UpdateCount + 32) \ 32, Byte) * 4)      'OK Flags are Int32, so to byte -> *4
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
        End Sub

        'Packets and Events
        Public Property AFK() As Boolean
            Get
                Return (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_AFK)
            End Get
            Set(ByVal Value As Boolean)
                If Value Then
                    cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_AFK
                    ClsWorldServer.Cluster.ClientSetChatFlag(client.Index, ChatFlag.FLAGS_AFK)
                Else
                    cPlayerFlags = cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_AFK)
                    ClsWorldServer.Cluster.ClientSetChatFlag(client.Index, 0)
                End If
            End Set
        End Property

        Public Property DND() As Boolean
            Get
                Return (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_DND)
            End Get
            Set(ByVal Value As Boolean)
                If Value Then
                    cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_DND
                    ClsWorldServer.Cluster.ClientSetChatFlag(client.Index, ChatFlag.FLAGS_DND)
                Else
                    cPlayerFlags = cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_DND)
                    ClsWorldServer.Cluster.ClientSetChatFlag(client.Index, 0)
                End If
            End Set
        End Property

        Public Property GM() As Boolean
            Get
                Return (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_GM)
            End Get
            Set(ByVal Value As Boolean)
                If Value Then
                    cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_GM
                    ClsWorldServer.Cluster.ClientSetChatFlag(client.Index, ChatFlag.FLAGS_GM)
                Else
                    cPlayerFlags = cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_GM)
                    ClsWorldServer.Cluster.ClientSetChatFlag(client.Index, 0)
                End If
            End Set
        End Property

        'Chat
        Public Sub SendChatMessage(ByRef Sender As CharacterObject, ByVal Message As String, ByVal msgType As ChatMsg, ByVal msgLanguage As Integer, Optional ByVal ChannelName As String = "Global", Optional ByVal SendToMe As Boolean = False)
            Dim packet As PacketClass = BuildChatMessage(Sender.GUID, Message, msgType, msgLanguage, GetChatFlag(Sender), ChannelName)

            SendToNearPlayers(packet, , SendToMe)
            packet.Dispose()
        End Sub

        Public Sub CommandResponse(ByVal Message As String)
            Dim Messages() As String = Message.Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
            If Messages.Length = 0 Then
                ReDim Messages(0)
                Messages(0) = Message
            End If
            For Each Msg As String In Messages
                Dim packet As PacketClass = BuildChatMessage(SystemGUID, Msg, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL)
                client.Send(packet)
                packet.Dispose()
            Next
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_MESSAGECHAT", client.IP, client.Port)
        End Sub

        Public Sub SystemMessage(ByVal Message As String)
            SendMessageSystem(client, Message)
        End Sub

        'Spell/Skill/Talents System
        Public TalentPoints As Byte = 0
        Public AmmoID As Integer = 0
        Public AmmoDPS As Single = 0.0
        Public AmmoMod As Single = 1.0
        Public AutoShotSpell As Integer = 0
        Public NonCombatPet As CreatureObject = Nothing
        Public TotemSlot(0 To 3) As ULong
        Public Skills As New Dictionary(Of Integer, TSkill)
        Public SkillsPositions As New Dictionary(Of Integer, Short)
        Public Spells As New Dictionary(Of Integer, CharacterSpell)

        Public MindControl As BaseUnit = Nothing
        Public Sub CastOnSelf(ByVal SpellID As Integer)
            If WS_Spells.SPELLs.ContainsKey(SpellID) = False Then Exit Sub
            Dim Targets As New SpellTargets
            Targets.SetTarget_UNIT(Me)
            Dim castParams As New CastSpellParameters(Targets, Me, SpellID)
            castParams.Cast(Nothing)
        End Sub

        Public Sub ApplySpell(ByVal SpellID As Integer)
            If WS_Spells.SPELLs.ContainsKey(SpellID) = False Then Exit Sub
            Dim t As New SpellTargets
            t.SetTarget_SELF(Me)
            If WS_Spells.SPELLs(SpellID).CanCast(Me, t, False) = SpellFailedReason.SPELL_NO_ERROR Then
                WS_Spells.SPELLs(SpellID).Apply(Me, t)
            End If
        End Sub

        Public Sub ProhibitSpellSchool(ByVal School As Integer, ByVal Time As Integer)
            Dim packet As New PacketClass(OPCODES.SMSG_SPELL_COOLDOWN)
            Try
                packet.AddInt32(GUID)

                Dim curTime As UInteger = GetTimestamp(Now)
                For Each Spell As KeyValuePair(Of Integer, CharacterSpell) In Spells
                    Dim SpellInfo As SpellInfo = WS_Spells.SPELLs(Spell.Key)

                    If SpellInfo.School = School AndAlso (Spell.Value.Cooldown < curTime OrElse (Spell.Value.Cooldown - curTime) < Time) Then
                        packet.AddInt32(Spell.Key)
                        packet.AddInt32(Time)

                        Spell.Value.Cooldown = curTime + Time
                        Spell.Value.CooldownItem = 0
                    End If
                Next

                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Function FinishAllSpells(Optional ByVal OK As Boolean = False) As Boolean
            Dim result1 As Boolean = FinishSpell(CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL, OK)
            Dim result2 As Boolean = FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL, OK)
            Dim result3 As Boolean = FinishSpell(CurrentSpellTypes.CURRENT_GENERIC_SPELL, OK)

            Return (result1 OrElse result2 OrElse result3)
        End Function

        Public Function FinishSpell(ByVal SpellType As CurrentSpellTypes, Optional ByVal OK As Boolean = False) As Boolean
            If SpellType = CurrentSpellTypes.CURRENT_CHANNELED_SPELL Then
                SendChannelUpdate(Me, 0)
            End If
            If client.Character.spellCasted(SpellType) Is Nothing Then Return False
            If client.Character.spellCasted(SpellType).Finished Then Return False

            client.Character.spellCasted(SpellType).State = SpellCastState.SPELL_STATE_IDLE
            client.Character.spellCasted(SpellType).Stopped = True

            If SpellType = CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL Then
                client.Character.AutoShotSpell = 0
                client.Character.attackState.AttackStop()
            Else
                Dim SpellID As Integer = spellCasted(SpellType).SpellID
                WS_Spells.SPELLs(SpellID).SendInterrupted(0, client.Character)

                If Not OK Then
                    SendCastResult(SpellFailedReason.SPELL_FAILED_INTERRUPTED, client, SpellID)
                End If

                client.Character.RemoveAuraBySpell(SpellID)

                'DONE: Remove dynamic objects created with spell
                Dim DynamicObjects() As DynamicObjectObject = client.Character.dynamicObjects.ToArray()
                For Each tmpDO As DynamicObjectObject In DynamicObjects
                    If tmpDO.SpellID = SpellID Then
                        tmpDO.Delete()
                        client.Character.dynamicObjects.Remove(tmpDO)
                        Exit For
                    End If
                Next

                'DONE: Remove game objects created with spell
                Dim GameObjects() As GameObjectObject = client.Character.gameObjects.ToArray()
                For Each tmpGO As GameObjectObject In GameObjects
                    If tmpGO.CreatedBySpell = SpellID Then
                        tmpGO.Destroy(tmpGO)
                        client.Character.gameObjects.Remove(tmpGO)
                        Exit For
                    End If
                Next
            End If

            Return True
        End Function

        Public Sub LearnSpell(ByVal SpellID As Integer)
            If Spells.ContainsKey(SpellID) Then Exit Sub
            If Not WS_Spells.SPELLs.ContainsKey(SpellID) Then Exit Sub
            Spells.Add(SpellID, New CharacterSpell(SpellID, 1, 0, 0))

            'DONE: Save it to the database
            CharacterDatabase.Update(String.Format("INSERT INTO characters_spells (guid, spellid, active, cooldown, cooldownitem) VALUES ({0},{1},{2},{3},{4});", GUID, SpellID, 1, 0, 0))

            If client Is Nothing Then Exit Sub
            Dim SMSG_LEARNED_SPELL As New PacketClass(OPCODES.SMSG_LEARNED_SPELL)
            Try
                SMSG_LEARNED_SPELL.AddInt32(SpellID)
                client.Send(SMSG_LEARNED_SPELL)
            Finally
                SMSG_LEARNED_SPELL.Dispose()
            End Try

            Dim t As New SpellTargets
            t.SetTarget_SELF(Me)

            If WS_Spells.SPELLs(SpellID).IsPassive Then
                'DONE: Apply passive spell we don't have
                If HavePassiveAura(SpellID) = False AndAlso WS_Spells.SPELLs(SpellID).CanCast(Me, t, False) = SpellFailedReason.SPELL_NO_ERROR Then
                    WS_Spells.SPELLs(SpellID).Apply(Me, t)
                End If
            End If

            'DONE: Deactivate old ranks
            If Not WS_Spells.SPELLs(SpellID).CanStackSpellRank Then
                If SpellChains.ContainsKey(SpellID) Then
                    If Spells.ContainsKey(SpellChains(SpellID)) Then
                        Spells(SpellChains(SpellID)).Active = 0 'NOTE: Deactivate spell, don't remove it

                        'DONE: Save it to the database
                        CharacterDatabase.Update(String.Format("UPDATE characters_spells SET active = 0 WHERE guid = {0} AND spellid = {1};", GUID, SpellID))

                        Dim packet As New PacketClass(OPCODES.SMSG_SUPERCEDED_SPELL)
                        Try
                            packet.AddInt32(SpellChains(SpellID))
                            packet.AddInt32(SpellID)
                            client.Send(packet)
                        Finally
                            packet.Dispose()
                        End Try
                    End If
                End If
            End If

            Dim maxSkill As Integer = If(Level > DEFAULT_MAX_LEVEL, DEFAULT_MAX_LEVEL * 5, Level * 5)
            Select Case SpellID
                Case 4036 ' SKILL_ENGINERING
                    LearnSpell(3918)
                    LearnSpell(3919)
                    LearnSpell(3920)
                Case 3908 ' SKILL_TAILORING
                    LearnSpell(2387)
                    LearnSpell(2963)
                Case 7411 ' SKILL_ENCHANTING
                    LearnSpell(7418)
                    LearnSpell(7421)
                    LearnSpell(13262)
                Case 2259 ' SKILL_ALCHEMY
                    LearnSpell(2329)
                    LearnSpell(7183)
                    LearnSpell(2330)
                Case 2018 ' SKILL_BLACKSMITHING
                    LearnSpell(2663)
                    LearnSpell(12260)
                    LearnSpell(2660)
                    LearnSpell(3115)
                Case 2108 ' SKILL_LEATHERWORKING
                    LearnSpell(2152)
                    LearnSpell(9058)
                    LearnSpell(9059)
                    LearnSpell(2149)
                    LearnSpell(7126)
                    LearnSpell(2881)
                Case 2550 ' SKILL_COOKING
                    LearnSpell(818)
                    LearnSpell(2540)
                    LearnSpell(2538)
                Case 3273 ' SKILL_FIRST_AID
                    LearnSpell(3275)
                Case 7620 ' SKILL_FISHING
                    LearnSpell(7738)
                Case 2575 ' SKILL_MINING
                    LearnSpell(2580)
                    LearnSpell(2656)
                    LearnSpell(2657)
                Case 2366 ' SKILL_HERBALISM
                    LearnSpell(2383)
                Case 264 ' WEAPON_BOWS
                    If Not HaveSpell(75) Then LearnSpell(2480)
                    LearnSkill(SKILL_IDs.SKILL_BOWS, 1, maxSkill)
                Case 266 ' WEAPON_GUNS
                    If Not HaveSpell(75) Then LearnSpell(2480)
                    LearnSkill(SKILL_IDs.SKILL_GUNS, 1, maxSkill)
                Case 5011 ' WEAPON_CROSSBOWS
                    If Not HaveSpell(75) Then LearnSpell(7919)
                    LearnSkill(SKILL_IDs.SKILL_CROSSBOWS, 1, maxSkill)
                Case 2567 ' WEAPON_THROWN
                    LearnSpell(2764)
                    LearnSkill(SKILL_IDs.SKILL_THROWN, 1, maxSkill)
                Case 5009 ' WEAPON_WANDS
                    LearnSpell(5019)
                    LearnSkill(SKILL_IDs.SKILL_WANDS, 1, maxSkill)
                Case 9078 ' ARMOR_CLOTH
                    LearnSkill(SKILL_IDs.SKILL_CLOTH)
                Case 9077 ' ARMOR_LEATHER
                    LearnSkill(SKILL_IDs.SKILL_LEATHER)
                Case 8737 ' ARMOR_MAIL
                    LearnSkill(SKILL_IDs.SKILL_MAIL)
                Case 750 ' ARMOR_PLATE
                    LearnSkill(SKILL_IDs.SKILL_PLATE_MAIL)
                Case 9116 ' ARMOR_SHIELD
                    LearnSkill(SKILL_IDs.SKILL_SHIELD)
                Case 674 ' DUAL_WIELD
                    LearnSkill(SKILL_IDs.SKILL_DUAL_WIELD)
                Case 196 ' WEAPON_AXES
                    LearnSkill(SKILL_IDs.SKILL_AXES, 1, maxSkill)
                Case 197 ' WEAPON_TWOHAND_AXES
                    LearnSkill(SKILL_IDs.SKILL_TWO_HANDED_AXES, 1, maxSkill)
                Case 227 ' WEAPON_STAVES
                    LearnSkill(SKILL_IDs.SKILL_STAVES, 1, maxSkill)
                Case 198 ' WEAPON_MACES
                    LearnSkill(SKILL_IDs.SKILL_MACES, 1, maxSkill)
                Case 199 ' WEAPON_TWOHAND_MACES
                    LearnSkill(SKILL_IDs.SKILL_TWO_HANDED_MACES, 1, maxSkill)
                Case 201 ' WEAPON_SWORDS
                    LearnSkill(SKILL_IDs.SKILL_SWORDS, 1, maxSkill)
                Case 202 ' WEAPON_TWOHAND_SWORDS
                    LearnSkill(SKILL_IDs.SKILL_TWO_HANDED_SWORDS, 1, maxSkill)
                Case 1180 ' WEAPON_DAGGERS
                    LearnSkill(SKILL_IDs.SKILL_DAGGERS, 1, maxSkill)
                Case 15590 ' WEAPON_FIST_WEAPONS
                    LearnSkill(SKILL_IDs.SKILL_FIST_WEAPONS, 1, maxSkill)
                Case 200 ' WEAPON_POLEARMS
                    LearnSkill(SKILL_IDs.SKILL_POLEARMS, 1, maxSkill)
                Case 3386 ' WEAPON_SPEARS
                    LearnSkill(SKILL_IDs.SKILL_SPEARS, 1, maxSkill)
                Case 2842 ' OTHER_POISONS
                    LearnSkill(SKILL_IDs.SKILL_POISONS, 1, maxSkill)
                Case 668 ' LANGUAGE_COMMON
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_COMMON, 300, 300)
                Case 669 ' LANGUAGE_ORCISH
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_ORCISH, 300, 300)
                Case 670 ' LANGUAGE_TAURAHE
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_TAURAHE, 300, 300)
                Case 671 ' LANGUAGE_DARNASSIAN
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_DARNASSIAN, 300, 300)
                Case 672 ' LANGUAGE_DWARVEN
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_DWARVEN, 300, 300)
                Case 813 ' LANGUAGE_THALASSIAN
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_THALASSIAN, 300, 300)
                Case 814 ' LANGUAGE_DRACONIC
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_DRACONIC, 300, 300)
                Case 815 ' LANGUAGE_DEMON_TONGUE
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_DEMON_TONGUE, 300, 300)
                Case 816 ' LANGUAGE_TITAN
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_TITAN, 300, 300)
                Case 817 ' LANGUAGE_OLD_TONGUE
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_OLD_TONGUE, 300, 300)
                Case 7340 ' LANGUAGE_GNOMISH
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_GNOMISH, 300, 300)
                Case 7341 ' LANGUAGE_TROLL
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_TROLL, 300, 300)
                Case 17737 ' LANGUAGE_GUTTERSPEAK
                    LearnSkill(SKILL_IDs.SKILL_LANGUAGE_GUTTERSPEAK, 300, 300)
            End Select
        End Sub

        Public Sub UnLearnSpell(ByVal SpellID As Integer)
            If Not Spells.ContainsKey(SpellID) Then Exit Sub
            Spells.Remove(SpellID)

            'DONE: Save it to the database
            CharacterDatabase.Update(String.Format("DELETE FROM characters_spells WHERE guid = {0} AND spellid = {1};", GUID, SpellID))

            Dim SMSG_REMOVED_SPELL As New PacketClass(OPCODES.SMSG_REMOVED_SPELL)
            Try
                SMSG_REMOVED_SPELL.AddInt32(SpellID)
                client.Send(SMSG_REMOVED_SPELL)
            Finally
                SMSG_REMOVED_SPELL.Dispose()
            End Try

            'DONE: Remove Aura by this spell
            client.Character.RemoveAuraBySpell(SpellID)
        End Sub

        Public Function HaveSpell(ByVal SpellID As Integer) As Boolean
            Return Spells.ContainsKey(SpellID)
        End Function

        Public Sub LearnSkill(ByVal SkillID As Integer, Optional ByVal Current As Short = 1, Optional ByVal Maximum As Short = 1)
            If Skills.ContainsKey(SkillID) Then

                'DONE: Already know this skill, just increase value
                CType(Skills(SkillID), TSkill).Base = Maximum
                If Current <> 1 Then CType(Skills(SkillID), TSkill).Current = Current

            Else

                'DONE: Learn this skill as new
                'TODO: Needs to be tidied up
                Dim i As Short = 0
                For i = 0 To PLAYER_SKILL_INFO_SIZE
                    If Not SkillsPositions.ContainsValue(i) Then
                        Exit For
                    End If
                Next

                If i > PLAYER_SKILL_INFO_SIZE Then Exit Sub

                SkillsPositions.Add(SkillID, i)
                Skills.Add(SkillID, New TSkill(Current, Maximum))
            End If

            If client Is Nothing Then Exit Sub

            'DONE: Set update parameters
            SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + SkillsPositions(SkillID) * 3, SkillID)                            'skill1.Id
            SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + SkillsPositions(SkillID) * 3 + 1, Skills(SkillID).GetSkill)       'CType((skill1.CurrentVal(Me) + (skill1.Cap(Me) << 16)), Integer)
            SendCharacterUpdate()
        End Sub

        Public Function HaveSkill(ByVal SkillID As Integer, Optional ByVal SkillValue As Integer = 0) As Boolean
            If Skills.ContainsKey(SkillID) Then
                Return Skills(SkillID).Current >= SkillValue
            Else
                Return False
            End If
        End Function

        Public Sub UpdateSkill(ByVal SkillID As Integer, Optional ByVal SpeedMod As Single = 0)
            If SkillID = 0 Then Exit Sub
            If Skills(SkillID).Current >= Skills(SkillID).Maximum Then Exit Sub

            If ((Skills(SkillID).Current / Skills(SkillID).Maximum) - SpeedMod) < Rnd.NextDouble Then
                Skills(SkillID).Increment()
                SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + SkillsPositions(SkillID) * 3 + 1, Skills(SkillID).GetSkill)
                SendCharacterUpdate()
            End If
        End Sub

        'XP and Level Managment
        Public RestBonus As Integer = 0
        Public XP As Integer = 0
        Public Sub SetLevel(ByVal SetToLevel As Byte)
            Dim TotalXp As Integer = 0
            'TODO: If it's a level decrease, decrease stats etc instead of increasing them
            If Level > SetToLevel Then Exit Sub

            For i As Short = Level To SetToLevel - 1
                TotalXp += XPTable(i)
            Next

            AddXP(TotalXp, 0, 0, False)
        End Sub

        Public Sub AddXP(ByVal Ammount As Integer, ByVal RestedBonus As Integer, Optional ByVal VictimGUID As ULong = 0, Optional ByVal LogIt As Boolean = True)
            If Ammount <= 0 Then Exit Sub

            If Level < DEFAULT_MAX_LEVEL Then

                XP = XP + Ammount
                If LogIt Then LogXPGain(Ammount, RestedBonus, VictimGUID, 1.0F)

                'Update rested state if needed
                If RestedBonus > 0 Then
                    If RestBonus <= 0 Then
                        RestBonus = 0
                        RestState = XPSTATE.Normal
                    End If
                    SetUpdateFlag(EPlayerFields.PLAYER_BYTES_2, cPlayerBytes2)
                    SetUpdateFlag(EPlayerFields.PLAYER_REST_STATE_EXPERIENCE, RestBonus)
                End If

CheckXPAgain:
                If XP >= XPTable(Level) Then
                    XP -= XPTable(Level)
                    Level = Level + 1

                    GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_LEVEL

                    'DONE: Send update to cluster
                    ClsWorldServer.Cluster.ClientUpdate(client.Index, ZoneID, Level)

                    Dim oldLife As Integer = Life.Maximum
                    Dim oldMana As Integer = Mana.Maximum
                    Dim oldStrength As Integer = Strength.Base
                    Dim oldAgility As Integer = Agility.Base
                    Dim oldStamina As Integer = Stamina.Base
                    Dim oldIntellect As Integer = Intellect.Base
                    Dim oldSpirit As Integer = Spirit.Base
                    CalculateOnLevelUP(Me)
                    Dim SMSG_LEVELUP_INFO As New PacketClass(OPCODES.SMSG_LEVELUP_INFO)
                    Try
                        SMSG_LEVELUP_INFO.AddInt32(Level)
                        SMSG_LEVELUP_INFO.AddInt32(Life.Maximum - oldLife)
                        SMSG_LEVELUP_INFO.AddInt32(Mana.Maximum - oldMana)
                        SMSG_LEVELUP_INFO.AddInt32(0)
                        SMSG_LEVELUP_INFO.AddInt32(0)
                        SMSG_LEVELUP_INFO.AddInt32(0)
                        SMSG_LEVELUP_INFO.AddInt32(0)
                        SMSG_LEVELUP_INFO.AddInt32(0)
                        SMSG_LEVELUP_INFO.AddInt32(0)
                        SMSG_LEVELUP_INFO.AddInt32(Strength.Base - oldStrength)
                        SMSG_LEVELUP_INFO.AddInt32(Agility.Base - oldAgility)
                        SMSG_LEVELUP_INFO.AddInt32(Stamina.Base - oldStamina)
                        SMSG_LEVELUP_INFO.AddInt32(Intellect.Base - oldIntellect)
                        SMSG_LEVELUP_INFO.AddInt32(Spirit.Base - oldSpirit)
                        If client IsNot Nothing Then client.Send(SMSG_LEVELUP_INFO)
                    Finally
                        SMSG_LEVELUP_INFO.Dispose()
                    End Try

                    Life.Current = Life.Maximum
                    Mana.Current = Mana.Maximum

                    Resistances(DamageTypes.DMG_PHYSICAL).Base += (Agility.Base - oldAgility) * 2

                    SetUpdateFlag(EPlayerFields.PLAYER_XP, XP)
                    SetUpdateFlag(EPlayerFields.PLAYER_NEXT_LEVEL_XP, XPTable(Level))
                    SetUpdateFlag(EPlayerFields.PLAYER_CHARACTER_POINTS1, TalentPoints)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_LEVEL, Level)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_STRENGTH, Strength.Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_AGILITY, Agility.Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_STAMINA, Stamina.Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_SPIRIT, Spirit.Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_INTELLECT, Intellect.Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_BASE_HEALTH, Life.Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, Mana.Current)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_BASE_MANA, Mana.Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Life.Maximum)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1, Mana.Maximum)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER, AttackPower)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER_MODS, AttackPowerMods)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER, AttackPowerRanged)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MODS, AttackPowerModsRanged)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_PHYSICAL, Resistances(DamageTypes.DMG_PHYSICAL).Base)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MINDAMAGE, Damage.Minimum)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXDAMAGE, CType(Damage.Maximum + (AttackPower + AttackPowerMods) * 0.071428571428571425, Single))
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MINOFFHANDDAMAGE, OffHandDamage.Minimum)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXOFFHANDDAMAGE, OffHandDamage.Maximum)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MINRANGEDDAMAGE, RangedDamage.Minimum)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXRANGEDDAMAGE, RangedDamage.Maximum + BaseRangedDamage)

                    For Each Skill As KeyValuePair(Of Integer, TSkill) In Skills
                        SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + SkillsPositions(Skill.Key) * 3 + 1, Skill.Value.GetSkill)       'CType((skill1.CurrentVal(Me) + (skill1.Cap(Me) << 16)), Integer)
                    Next

                    If client IsNot Nothing Then UpdateManaRegen()
                Else
                    If client IsNot Nothing Then SetUpdateFlag(EPlayerFields.PLAYER_XP, XP)
                End If

                'We just dinged more than one level
                If XP >= XPTable(Level) AndAlso Level < DEFAULT_MAX_LEVEL Then GoTo CheckXPAgain

                'Fix if we add very big number XP
                If XP > XPTable(Level) Then XP = XPTable(Level)

                If client IsNot Nothing Then SendCharacterUpdate()
                SaveCharacter()
            End If
        End Sub

        'Item Managment
        Public Items As New Dictionary(Of Byte, ItemObject)
        Public Sub ItemADD(ByVal ItemEntry As Integer, ByVal dstBag As Byte, ByVal dstSlot As Byte, Optional ByVal Count As Integer = 1)
            Dim tmpItem As New ItemObject(ItemEntry, GUID)
            'DONE: Check for unique
            If tmpItem.ItemInfo.Unique > 0 AndAlso ItemCOUNT(ItemEntry) > tmpItem.ItemInfo.Unique Then tmpItem.Delete() : Exit Sub
            'DONE: Check for max stacking
            If Count > tmpItem.ItemInfo.Stackable Then Count = tmpItem.ItemInfo.Stackable
            tmpItem.StackCount = Count
            If dstBag = 255 And dstSlot = 255 Then
                ItemADD_AutoSlot(tmpItem)
            Else
                ItemSETSLOT(tmpItem, dstBag, dstSlot)
            End If
            If dstBag = 0 And dstSlot < InventorySlots.INVENTORY_SLOT_BAG_END AndAlso client IsNot Nothing Then UpdateAddItemStats(tmpItem, dstSlot)
        End Sub

        Public Sub ItemREMOVE(ByVal srcBag As Byte, ByVal srcSlot As Byte, ByVal Destroy As Boolean, ByVal Update As Boolean)
            If srcBag = 0 Then
                If srcSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then
                    If srcSlot < EquipmentSlots.EQUIPMENT_SLOT_END Then SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + srcSlot * PLAYER_VISIBLE_ITEM_SIZE, 0)
                    UpdateRemoveItemStats(Items(srcSlot), srcSlot)
                End If
                SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + srcSlot * 2, 0)

                CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", ITEM_SLOT_NULL, ITEM_BAG_NULL, Items(srcSlot).GUID - GUID_ITEM))
                If Destroy Then Items(srcSlot).Delete()
                Items.Remove(srcSlot)
                If Update Then SendCharacterUpdate()
            Else
                CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", ITEM_SLOT_NULL, ITEM_BAG_NULL, Items(srcBag).Items(srcSlot).GUID - GUID_ITEM))
                If Destroy Then Items(srcBag).Items(srcSlot).Delete()
                Items(srcBag).Items.Remove(srcSlot)
                If Update Then SendItemUpdate(Items(srcBag))
            End If
        End Sub

        Public Sub ItemREMOVE(ByVal itemGuid As ULong, ByVal Destroy As Boolean, ByVal Update As Boolean)
            'DONE: Search in inventory
            For slot As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                If Items.ContainsKey(slot) Then
                    If Items(slot).GUID = itemGuid Then

                        CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", ITEM_SLOT_NULL, ITEM_BAG_NULL, Items(slot).GUID - GUID_ITEM))
                        If slot < InventorySlots.INVENTORY_SLOT_BAG_END Then
                            If slot < EquipmentSlots.EQUIPMENT_SLOT_END Then SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + slot * PLAYER_VISIBLE_ITEM_SIZE, 0)
                            UpdateRemoveItemStats(Items(slot), slot)
                        End If
                        SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + slot * 2, 0)

                        If Destroy Then Items(slot).Delete()
                        Items.Remove(slot)
                        If Update Then SendCharacterUpdate(True)
                        Exit Sub

                    End If
                End If
            Next slot

            'DONE: Search in bags
            For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_1 To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                If Items.ContainsKey(bag) Then

                    'DONE: Search this bag
                    Dim slot As Byte = 0
                    For slot = 0 To Items(bag).ItemInfo.ContainerSlots - 1
                        If Items(bag).Items.ContainsKey(slot) Then

                            If Items(bag).Items(slot).GUID = itemGuid Then
                                CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", ITEM_SLOT_NULL, ITEM_BAG_NULL, Items(bag).Items(slot).GUID - GUID_ITEM))

                                If Destroy Then Items(bag).Items(slot).Delete()
                                Items(bag).Items.Remove(slot)
                                If Update Then SendItemUpdate(Items(bag))
                                Exit Sub
                            End If
                        End If
                    Next slot
                End If
            Next

            Throw New ApplicationException("Unable to remove item because character doesn't have it in inventory or bags.")
        End Sub

        Public Function ItemADD(ByRef Item As ItemObject) As Boolean
            Dim tmpEntry As Integer = Item.ItemEntry
            Dim tmpCount As Byte = Item.StackCount
            'DONE: Check for max stack
            If tmpCount > Item.ItemInfo.Stackable Then tmpCount = Item.ItemInfo.Stackable
            'DONE: Check for unique
            If Item.ItemInfo.Unique > 0 AndAlso ItemCOUNT(Item.ItemEntry) >= Item.ItemInfo.Unique Then Return False
            'DONE: Add the item
            If ItemADD_AutoSlot(Item) AndAlso (Not client Is Nothing) Then
                'DONE: Fire quest event to check for if this item is required for quest
                'TODO: This needs to be fired BEFORE the client has the item in the bag...
                'NOTE: Not only quest items are needed for quests
                ALLQUESTS.OnQuestItemAdd(Me, tmpEntry, tmpCount)

                Return True
            End If
            Return False
        End Function

        Public BuyBackTimeStamp(0 To ((BuyBackSlots.BUYBACK_SLOT_END - BuyBackSlots.BUYBACK_SLOT_START) - 1)) As Integer

        Public Sub ItemADD_BuyBack(ByRef Item As ItemObject)
            Dim i As Byte, Slot As Byte, eSlot As Byte, OldestTime As Integer, OldestSlot As Byte
            Slot = ITEM_SLOT_NULL
            OldestTime = GetTimestamp(Now)
            OldestSlot = ITEM_SLOT_NULL
            For i = BuyBackSlots.BUYBACK_SLOT_START To BuyBackSlots.BUYBACK_SLOT_END - 1
                If Items.ContainsKey(i) = False OrElse BuyBackTimeStamp(i - BuyBackSlots.BUYBACK_SLOT_START) = 0 Then 'Woho we found a empty slot to use!
                    If Slot = ITEM_SLOT_NULL Then Slot = i
                Else 'If not let's find out the oldest item in the buyback
                    If BuyBackTimeStamp(i - BuyBackSlots.BUYBACK_SLOT_START) < OldestTime Then
                        OldestTime = BuyBackTimeStamp(i - BuyBackSlots.BUYBACK_SLOT_START)
                        OldestSlot = i
                    End If
                End If
            Next
            If Slot = ITEM_SLOT_NULL Then 'We never found a empty slot so let's just remove the oldest item
                If OldestSlot <> ITEM_SLOT_NULL Then Exit Sub 'Somehow it all got very wrong o_O
                ItemREMOVE(0, OldestSlot, True, True)
                Slot = OldestSlot
            End If
            'Now we have a empty slow so let's just put our item there
            eSlot = Slot - BuyBackSlots.BUYBACK_SLOT_START
            BuyBackTimeStamp(eSlot) = GetTimestamp(Now)
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BUYBACK_TIMESTAMP_1 + eSlot, BuyBackTimeStamp(eSlot))
            SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BUYBACK_PRICE_1 + eSlot, Item.ItemInfo.SellPrice * Item.StackCount)
            ItemSETSLOT(Item, 0, Slot)
        End Sub

        Public Function ItemADD_AutoSlot(ByRef Item As ItemObject) As Boolean

            If Item.ItemInfo.Stackable > 1 Then
                'DONE: Search for stackable in special bags
                If Item.ItemInfo.BagFamily = ITEM_BAG.KEYRING OrElse Item.ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_KEY Then
                    For slot As Byte = KeyRingSlots.KEYRING_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                        If Items.ContainsKey(slot) AndAlso Items(slot).ItemEntry = Item.ItemEntry AndAlso Items(slot).StackCount < Items(slot).ItemInfo.Stackable Then
                            Dim stacked As Integer = Items(slot).ItemInfo.Stackable - Items(slot).StackCount
                            If stacked >= Item.StackCount Then
                                Items(slot).StackCount += Item.StackCount
                                Item.Delete()
                                Item = Items(slot)
                                Items(slot).Save()
                                SendItemUpdate(Items(slot))
                                Return True
                            ElseIf stacked > 0 Then
                                Items(slot).StackCount += stacked
                                Item.StackCount -= stacked
                                Items(slot).Save()
                                Item.Save()
                                SendItemUpdate(Items(slot))
                                SendItemUpdate(Item)
                                Return ItemADD_AutoSlot(Item)
                            End If
                        End If
                    Next
                ElseIf Item.ItemInfo.BagFamily <> 0 Then
                    For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                        If Items.ContainsKey(bag) AndAlso Items(bag).ItemInfo.SubClass <> ITEM_SUBCLASS.ITEM_SUBCLASS_BAG Then
                            If (Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_SOUL_BAG AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.SOUL_SHARD) OrElse
                            (Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_HERB_BAG AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.HERB) OrElse
                            (Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_ENCHANTING_BAG AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.ENCHANTING) OrElse
                            (Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_QUIVER AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.ARROW) OrElse
                            (Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_AMMO_POUCH AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.BULLET) Then
                                For Each slot As KeyValuePair(Of Byte, ItemObject) In Items(bag).Items
                                    If slot.Value.ItemEntry = Item.ItemEntry AndAlso slot.Value.StackCount < slot.Value.ItemInfo.Stackable Then
                                        Dim stacked As Integer = slot.Value.ItemInfo.Stackable - slot.Value.StackCount
                                        If stacked >= Item.StackCount Then
                                            slot.Value.StackCount += Item.StackCount
                                            Item.Delete()
                                            Item = slot.Value
                                            slot.Value.Save()
                                            SendItemUpdate(slot.Value)
                                            SendItemUpdate(Items(bag))
                                            Return True
                                        ElseIf stacked > 0 Then
                                            slot.Value.StackCount += stacked
                                            Item.StackCount -= stacked
                                            slot.Value.Save()
                                            Item.Save()
                                            SendItemUpdate(slot.Value)
                                            SendItemUpdate(Item)
                                            SendItemUpdate(Items(bag))
                                            Return ItemADD_AutoSlot(Item)
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    Next
                End If
                'DONE: Search for stackable in main bag
                For slot As Byte = InventoryPackSlots.INVENTORY_SLOT_ITEM_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                    If Items.ContainsKey(slot) AndAlso Items(slot).ItemEntry = Item.ItemEntry AndAlso Items(slot).StackCount < Items(slot).ItemInfo.Stackable Then
                        Dim stacked As Integer = Items(slot).ItemInfo.Stackable - Items(slot).StackCount
                        If stacked >= Item.StackCount Then
                            Items(slot).StackCount += Item.StackCount
                            Item.Delete()
                            Item = Items(slot)
                            Items(slot).Save()
                            SendItemUpdate(Items(slot))
                            Return True
                        ElseIf stacked > 0 Then
                            Items(slot).StackCount += stacked
                            Item.StackCount -= stacked
                            Items(slot).Save()
                            Item.Save()
                            SendItemUpdate(Items(slot))
                            SendItemUpdate(Item)
                            Return ItemADD_AutoSlot(Item)
                        End If
                    End If
                Next
                'DONE: Search for stackable in bags
                For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                    If Items.ContainsKey(bag) Then

                        For Each slot As KeyValuePair(Of Byte, ItemObject) In Items(bag).Items
                            If slot.Value.ItemEntry = Item.ItemEntry AndAlso slot.Value.StackCount < slot.Value.ItemInfo.Stackable Then
                                Dim stacked As Integer = slot.Value.ItemInfo.Stackable - slot.Value.StackCount
                                If stacked >= Item.StackCount Then
                                    slot.Value.StackCount += Item.StackCount
                                    Item.Delete()
                                    Item = slot.Value
                                    slot.Value.Save()
                                    SendItemUpdate(slot.Value)
                                    SendItemUpdate(Items(bag))
                                    Return True
                                ElseIf stacked > 0 Then
                                    slot.Value.StackCount += stacked
                                    Item.StackCount -= stacked
                                    slot.Value.Save()
                                    Item.Save()
                                    SendItemUpdate(slot.Value)
                                    SendItemUpdate(Item)
                                    SendItemUpdate(Items(bag))
                                    Return ItemADD_AutoSlot(Item)
                                End If
                            End If
                        Next
                    End If
                Next
            End If

            If Item.ItemInfo.BagFamily = ITEM_BAG.KEYRING OrElse Item.ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_KEY Then
                'DONE: Insert as keyring
                For slot As Byte = KeyRingSlots.KEYRING_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                    If Not Items.ContainsKey(slot) Then
                        Return ItemSETSLOT(Item, 0, slot)
                    End If
                Next
            ElseIf Item.ItemInfo.BagFamily <> 0 Then
                'DONE: Insert in free special bag
                For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                    If Items.ContainsKey(bag) AndAlso Items(bag).ItemInfo.SubClass <> ITEM_SUBCLASS.ITEM_SUBCLASS_BAG Then
                        If (Items(bag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONTAINER AndAlso Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_SOUL_BAG AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.SOUL_SHARD) OrElse
                        (Items(bag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONTAINER AndAlso Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_HERB_BAG AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.HERB) OrElse
                        (Items(bag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONTAINER AndAlso Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_ENCHANTING_BAG AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.ENCHANTING) OrElse
                        (Items(bag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_QUIVER AndAlso Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_QUIVER AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.ARROW) OrElse
                        (Items(bag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_QUIVER AndAlso Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_AMMO_POUCH AndAlso Item.ItemInfo.BagFamily = ITEM_BAG.BULLET) Then
                            For slot As Byte = 0 To Items(bag).ItemInfo.ContainerSlots - 1
                                If Not Items(bag).Items.ContainsKey(slot) Then
                                    Return ItemSETSLOT(Item, bag, slot)
                                End If
                            Next
                        End If
                    End If
                Next
            End If

            'DONE: Insert as new item in inventory
            For slot As Byte = InventoryPackSlots.INVENTORY_SLOT_ITEM_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                If Not Items.ContainsKey(slot) Then
                    Return ItemSETSLOT(Item, 0, slot)
                End If
            Next
            'DONE: Insert as new item in bag
            For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                If Items.ContainsKey(bag) AndAlso Items(bag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_BAG Then
                    For slot As Byte = 0 To Items(bag).ItemInfo.ContainerSlots - 1
                        If (Not Items(bag).Items.ContainsKey(slot)) AndAlso ItemCANEQUIP(Item, bag, slot) = InventoryChangeFailure.EQUIP_ERR_OK Then
                            Return ItemSETSLOT(Item, bag, slot)
                        End If
                    Next
                End If
            Next

            'DONE: Send error, not free slot
            SendInventoryChangeFailure(Me, InventoryChangeFailure.EQUIP_ERR_INVENTORY_FULL, 0, 0)
            Return False
        End Function

        Public Function ItemADD_AutoBag(ByRef Item As ItemObject, ByVal dstBag As Byte) As Boolean
            If dstBag = 0 Then
                If Item.ItemInfo.Stackable > 1 Then
                    'DONE: Search for stackable in main bag
                    For slot As Byte = InventoryPackSlots.INVENTORY_SLOT_ITEM_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                        If Items(slot).ItemEntry = Item.ItemEntry AndAlso Items(slot).StackCount < Items(slot).ItemInfo.Stackable Then
                            Dim stacked As Byte = Items(slot).ItemInfo.Stackable - Items(slot).StackCount
                            If stacked >= Item.StackCount Then
                                Items(slot).StackCount += Item.StackCount
                                Item.Delete()
                                Item = Items(slot)
                                Items(slot).Save()
                                SendItemUpdate(Items(slot))
                                Return True
                            ElseIf stacked > 0 Then
                                Items(slot).StackCount += stacked
                                Item.StackCount -= stacked
                                Items(slot).Save()
                                Item.Save()
                                SendItemUpdate(Items(slot))
                                SendItemUpdate(Item)
                                Return ItemADD_AutoBag(Item, dstBag)
                            End If
                        End If
                    Next
                End If
                'DONE: Insert as keyring
                If Item.ItemInfo.BagFamily = ITEM_BAG.KEYRING Then
                    For slot As Byte = KeyRingSlots.KEYRING_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                        If Not Items.ContainsKey(slot) Then
                            Return ItemSETSLOT(Item, 0, slot)
                        End If
                    Next
                End If
                'DONE: Insert as new item in inventory
                For slot As Byte = InventoryPackSlots.INVENTORY_SLOT_ITEM_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                    If Not Items.ContainsKey(slot) Then
                        Return ItemSETSLOT(Item, 0, slot)
                    End If
                Next

            Else
                If Items.ContainsKey(dstBag) Then
                    If (Items(dstBag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONTAINER AndAlso Items(dstBag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_SOUL_BAG AndAlso Item.ItemInfo.BagFamily <> ITEM_BAG.SOUL_SHARD) OrElse
                        (Items(dstBag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONTAINER AndAlso Items(dstBag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_HERB_BAG AndAlso Item.ItemInfo.BagFamily <> ITEM_BAG.HERB) OrElse
                        (Items(dstBag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONTAINER AndAlso Items(dstBag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_ENCHANTING_BAG AndAlso Item.ItemInfo.BagFamily <> ITEM_BAG.ENCHANTING) OrElse
                        (Items(dstBag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_QUIVER AndAlso Items(dstBag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_QUIVER AndAlso Item.ItemInfo.BagFamily <> ITEM_BAG.ARROW) OrElse
                        (Items(dstBag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_QUIVER AndAlso Items(dstBag).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_BULLET AndAlso Item.ItemInfo.BagFamily <> ITEM_BAG.BULLET) Then
                        Log.WriteLine(LogType.DEBUG, "{0} - {1} - {2}", Items(dstBag).ItemInfo.ObjectClass, Items(dstBag).ItemInfo.SubClass, Item.ItemInfo.BagFamily)
                        SendInventoryChangeFailure(Me, InventoryChangeFailure.EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG, Item.GUID, 0)
                        Return False
                    End If

                    If Item.ItemInfo.Stackable > 1 Then
                        'DONE: Search for stackable in bag
                        For Each i As KeyValuePair(Of Byte, ItemObject) In Items(dstBag).Items
                            If i.Value.ItemEntry = Item.ItemEntry AndAlso i.Value.StackCount < i.Value.ItemInfo.Stackable Then
                                Dim stacked As Byte = i.Value.ItemInfo.Stackable - i.Value.StackCount
                                If stacked >= Item.StackCount Then
                                    i.Value.StackCount += Item.StackCount
                                    Item.Delete()
                                    Item = i.Value
                                    i.Value.Save()
                                    SendItemUpdate(i.Value)
                                    Return True
                                ElseIf stacked > 0 Then
                                    i.Value.StackCount += stacked
                                    Item.StackCount -= stacked
                                    i.Value.Save()
                                    Item.Save()
                                    SendItemUpdate(i.Value)
                                    SendItemUpdate(Item)
                                    Return ItemADD_AutoBag(Item, dstBag)
                                End If
                            End If
                        Next
                    End If
                    'DONE: Insert as new item in bag
                    For slot As Byte = 0 To Items(dstBag).ItemInfo.ContainerSlots - 1
                        If (Not Items(dstBag).Items.ContainsKey(slot)) AndAlso ItemCANEQUIP(Item, dstBag, slot) = InventoryChangeFailure.EQUIP_ERR_OK Then
                            Return ItemSETSLOT(Item, dstBag, slot)
                        End If
                    Next

                End If
            End If

            'DONE: Send error, not free slot
            SendInventoryChangeFailure(Me, InventoryChangeFailure.EQUIP_ERR_BAG_FULL, Item.GUID, 0)
            Return False
        End Function

        Public Function ItemSETSLOT(ByRef Item As ItemObject, ByVal dstBag As Byte, ByVal dstSlot As Byte) As Boolean
            If Item.ItemInfo.Bonding = ITEM_BONDING_TYPE.BIND_WHEN_PICKED_UP AndAlso Item.IsSoulBound = False Then Item.SoulbindItem()
            If (Item.ItemInfo.Bonding = ITEM_BONDING_TYPE.BIND_UNK_QUESTITEM1 OrElse Item.ItemInfo.Bonding = ITEM_BONDING_TYPE.BIND_UNK_QUESTITEM2) AndAlso Item.IsSoulBound = False Then Item.SoulbindItem()
            If dstBag = 0 Then
                'DONE: Bind a nonbinded BIND WHEN PICKED UP item or a nonbinded quest item
                'DONE: Put in inventory
                Items(dstSlot) = Item
                CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1}, item_stackCount = {2} WHERE item_guid = {3};", dstSlot, GUID, Item.StackCount, Item.GUID - GUID_ITEM))

                SetUpdateFlag(EPlayerFields.PLAYER_FIELD_INV_SLOT_HEAD + dstSlot * 2, Item.GUID)
                If dstSlot < EquipmentSlots.EQUIPMENT_SLOT_END Then
                    SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + dstSlot * PLAYER_VISIBLE_ITEM_SIZE, Item.ItemEntry)
                    'For Each Enchant As KeyValuePair(Of Byte, TEnchantmentInfo) In Item.Enchantments
                    '   SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_0 + Enchant.Key + dstSlot * PLAYER_VISIBLE_ITEM_SIZE, Enchant.Value.ID)
                    'Next
                    SetUpdateFlag(EPlayerFields.PLAYER_VISIBLE_ITEM_1_PROPERTIES + dstSlot * PLAYER_VISIBLE_ITEM_SIZE, Item.RandomProperties)
                    'DONE: Bind a nonbinded BIND WHEN EQUIPPED item
                    If Item.ItemInfo.Bonding = ITEM_BONDING_TYPE.BIND_WHEN_EQUIPED AndAlso Item.IsSoulBound = False Then Item.SoulbindItem()
                End If
            Else
                'DONE: Put in bag
                Items(dstBag).Items(dstSlot) = Item
                CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1}, item_stackCount = {2} WHERE item_guid = {3};", dstSlot, Items(dstBag).GUID, Item.StackCount, Item.GUID - GUID_ITEM))
            End If

            'DONE: Send updates
            If client IsNot Nothing Then
                SendItemAndCharacterUpdate(Item, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT)
                If dstBag > 0 Then SendItemUpdate(Items(dstBag))
            End If
            Return True
        End Function

        Public Function ItemCOUNT(ByVal ItemEntry As Integer, Optional ByVal Equipped As Boolean = False) As Integer
            Dim count As Integer = 0

            'DONE: Search in inventory
            Dim EndSlot As Byte = InventoryPackSlots.INVENTORY_SLOT_ITEM_END
            If Equipped Then EndSlot = EquipmentSlots.EQUIPMENT_SLOT_END
            For slot As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To EndSlot - 1
                If Items.ContainsKey(slot) Then
                    If Items(slot).ItemEntry = ItemEntry Then count += Items(slot).StackCount
                End If
            Next slot
            If Equipped Then Return count

            'DONE: Search in keyring
            For slot As Byte = KeyRingSlots.KEYRING_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                If Items.ContainsKey(slot) Then
                    If Items(slot).ItemEntry = ItemEntry Then count += Items(slot).StackCount
                End If
            Next slot

            'DONE: Search in bags
            For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_1 To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                If Items.ContainsKey(bag) Then

                    'DONE: Search this bag
                    Dim slot As Byte = 0
                    For slot = 0 To Items(bag).ItemInfo.ContainerSlots - 1
                        If Items(bag).Items.ContainsKey(slot) Then
                            If Items(bag).Items(slot).ItemEntry = ItemEntry Then count += Items(bag).Items(slot).StackCount
                        End If
                    Next slot
                End If
            Next

            Return count
        End Function

        Public Function ItemCONSUME(ByVal ItemEntry As Integer, ByVal Count As Integer) As Boolean
            'DONE: Search in inventory
            For slot As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                If Items.ContainsKey(slot) Then
                    If Items(slot).ItemEntry = ItemEntry Then

                        If Items(slot).StackCount <= Count Then
                            Count -= Items(slot).StackCount
                            ItemREMOVE(0, slot, True, True)
                            If Count <= 0 Then Return True
                        Else
                            Items(slot).StackCount -= Count
                            Items(slot).Save(False)
                            SendItemUpdate(Items(slot))
                            Return True
                        End If

                    End If
                End If
            Next slot

            'DONE: Search in keyring slot
            For slot As Byte = KeyRingSlots.KEYRING_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                If Items.ContainsKey(slot) Then
                    If Items(slot).ItemEntry = ItemEntry Then

                        If Items(slot).StackCount <= Count Then
                            Count -= Items(slot).StackCount
                            ItemREMOVE(0, slot, True, True)
                            If Count <= 0 Then Return True
                        Else
                            Items(slot).StackCount -= Count
                            Items(slot).Save(False)
                            SendItemUpdate(Items(slot))
                            Return True
                        End If

                    End If
                End If
            Next slot

            'DONE: Search in bags
            For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_1 To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                If Items.ContainsKey(bag) Then

                    'DONE: Search this bag
                    Dim slot As Byte = 0
                    For slot = 0 To Items(bag).ItemInfo.ContainerSlots - 1
                        If Items(bag).Items.ContainsKey(slot) Then
                            If Items(bag).Items(slot).ItemEntry = ItemEntry Then

                                If Items(bag).Items(slot).StackCount <= Count Then
                                    Count -= Items(bag).Items(slot).StackCount
                                    ItemREMOVE(bag, slot, True, True)
                                    If Count <= 0 Then Return True
                                Else
                                    Items(bag).Items(slot).StackCount -= Count
                                    Items(bag).Items(slot).Save(False)
                                    SendItemUpdate(Items(bag).Items(slot))
                                    Return True
                                End If

                            End If
                        End If
                    Next slot
                End If
            Next

            Return False
        End Function

        Public Function ItemFREESLOTS() As Integer
            Dim foundFreeSlots As Integer = 0

            'DONE Find space in main bag
            For slot As Byte = InventoryPackSlots.INVENTORY_SLOT_ITEM_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                If Not Items.ContainsKey(slot) Then
                    foundFreeSlots += 1
                End If
            Next slot

            'DONE: Find space in other bags
            For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                If Items.ContainsKey(bag) Then
                    For slot As Byte = 0 To Items(bag).ItemInfo.ContainerSlots - 1
                        If Not Items(bag).Items.ContainsKey(slot) Then
                            foundFreeSlots += 1
                        End If
                    Next slot
                End If
            Next bag

            Return foundFreeSlots
        End Function

        Public Function ItemCANEQUIP(ByVal Item As ItemObject, ByVal dstBag As Byte, ByVal dstSlot As Byte) As InventoryChangeFailure
            'DONE: if dead then EQUIP_ERR_YOU_ARE_DEAD
            If DEAD Then Return InventoryChangeFailure.EQUIP_ERR_YOU_ARE_DEAD

            Dim ItemInfo As ItemInfo = Item.ItemInfo

            Try
                If dstBag = 0 Then
                    'DONE: items in inventory
                    Select Case dstSlot
                        Case Is < EquipmentSlots.EQUIPMENT_SLOT_END
                            If ItemInfo.IsContainer Then
                                Return InventoryChangeFailure.EQUIP_ERR_ITEM_CANT_BE_EQUIPPED
                            End If

                            If Not HaveFlag(ItemInfo.AvailableClasses, Classe - 1) Then
                                Return InventoryChangeFailure.EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM
                            End If
                            If Not HaveFlag(ItemInfo.AvailableRaces, Race - 1) Then
                                Return InventoryChangeFailure.EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM2
                            End If
                            If ItemInfo.ReqLevel > Level Then
                                Return InventoryChangeFailure.EQUIP_ERR_YOU_MUST_REACH_LEVEL_N
                            End If

                            Dim tmp As Boolean = False
                            For Each SlotVal As Byte In ItemInfo.GetSlots
                                If dstSlot = SlotVal Then tmp = True
                            Next
                            If Not tmp Then Return InventoryChangeFailure.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT

                            If dstSlot = EquipmentSlots.EQUIPMENT_SLOT_MAINHAND AndAlso ItemInfo.InventoryType = INVENTORY_TYPES.INVTYPE_TWOHAND_WEAPON AndAlso Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) Then
                                Return InventoryChangeFailure.EQUIP_ERR_CANT_EQUIP_WITH_TWOHANDED
                            End If
                            If dstSlot = EquipmentSlots.EQUIPMENT_SLOT_OFFHAND AndAlso Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) Then
                                If Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).ItemInfo.InventoryType = INVENTORY_TYPES.INVTYPE_TWOHAND_WEAPON Then
                                    Return InventoryChangeFailure.EQUIP_ERR_CANT_EQUIP_WITH_TWOHANDED
                                End If
                            End If
                            If dstSlot = EquipmentSlots.EQUIPMENT_SLOT_OFFHAND AndAlso ItemInfo.InventoryType = INVENTORY_TYPES.INVTYPE_WEAPON Then
                                If Not Skills.ContainsKey(SKILL_IDs.SKILL_DUAL_WIELD) Then Return InventoryChangeFailure.EQUIP_ERR_CANT_DUAL_WIELD
                            End If

                            If ItemInfo.GetReqSkill <> 0 Then
                                If Not Skills.ContainsKey(ItemInfo.GetReqSkill) Then Return InventoryChangeFailure.EQUIP_ERR_NO_REQUIRED_PROFICIENCY
                            End If
                            If ItemInfo.GetReqSpell <> 0 Then
                                If Not Spells.ContainsKey(ItemInfo.GetReqSpell) Then Return InventoryChangeFailure.EQUIP_ERR_NO_REQUIRED_PROFICIENCY
                            End If
                            If ItemInfo.ReqSkill <> 0 Then
                                If Not Skills.ContainsKey(ItemInfo.ReqSkill) Then Return InventoryChangeFailure.EQUIP_ERR_NO_REQUIRED_PROFICIENCY
                                If Skills(ItemInfo.ReqSkill).Current < ItemInfo.ReqSkillRank Then Return InventoryChangeFailure.EQUIP_ERR_SKILL_ISNT_HIGH_ENOUGH
                            End If
                            If ItemInfo.ReqSpell <> 0 Then
                                If Not Spells.ContainsKey(ItemInfo.ReqSpell) Then Return InventoryChangeFailure.EQUIP_ERR_NO_REQUIRED_PROFICIENCY
                            End If
                            'NOTE: Not used anymore in new honor system
                            If ItemInfo.ReqHonorRank <> 0 Then
                                If HonorHighestRank < ItemInfo.ReqHonorRank Then Return InventoryChangeFailure.EQUIP_ITEM_RANK_NOT_ENOUGH
                            End If
                            If ItemInfo.ReqFaction <> 0 Then
                                If client.Character.GetReputation(ItemInfo.ReqFaction) <= ItemInfo.ReqFactionLevel Then Return InventoryChangeFailure.EQUIP_ITEM_REPUTATION_NOT_ENOUGH
                            End If

                            Return InventoryChangeFailure.EQUIP_ERR_OK

                        Case Is < InventorySlots.INVENTORY_SLOT_BAG_END
                            If Not ItemInfo.IsContainer Then Return InventoryChangeFailure.EQUIP_ERR_NOT_A_BAG
                            If Not Item.IsFree Then Return InventoryChangeFailure.EQUIP_ERR_NONEMPTY_BAG_OVER_OTHER_BAG
                            Return InventoryChangeFailure.EQUIP_ERR_OK

                        Case Is < InventoryPackSlots.INVENTORY_SLOT_ITEM_END
                            If ItemInfo.IsContainer Then
                                'DONE: Move only empty bags
                                If Item.IsFree Then
                                    Return InventoryChangeFailure.EQUIP_ERR_OK
                                Else
                                    Return InventoryChangeFailure.EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS
                                End If
                            End If
                            Return InventoryChangeFailure.EQUIP_ERR_OK

                        Case Is < BankItemSlots.BANK_SLOT_ITEM_END
                            If ItemInfo.IsContainer Then
                                'DONE: Move only empty bags
                                If Item.IsFree Then
                                    Return InventoryChangeFailure.EQUIP_ERR_OK
                                Else
                                    Return InventoryChangeFailure.EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS
                                End If
                            End If
                            Return InventoryChangeFailure.EQUIP_ERR_OK

                        Case Is < BankBagSlots.BANK_SLOT_BAG_END
                            If dstSlot >= (BankBagSlots.BANK_SLOT_BAG_START + Items_AvailableBankSlots) Then Return InventoryChangeFailure.EQUIP_ERR_MUST_PURCHASE_THAT_BAG_SLOT
                            If Not ItemInfo.IsContainer Then Return InventoryChangeFailure.EQUIP_ERR_NOT_A_BAG
                            If Not Item.IsFree Then Return InventoryChangeFailure.EQUIP_ERR_NONEMPTY_BAG_OVER_OTHER_BAG
                            Return InventoryChangeFailure.EQUIP_ERR_OK

                        Case Is < KeyRingSlots.KEYRING_SLOT_END
                            If ItemInfo.BagFamily <> ITEM_BAG.KEYRING AndAlso ItemInfo.ObjectClass <> ITEM_CLASS.ITEM_CLASS_KEY Then Return InventoryChangeFailure.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT
                            Return InventoryChangeFailure.EQUIP_ERR_OK

                        Case Else
                            Return InventoryChangeFailure.EQUIP_ERR_ITEM_CANT_BE_EQUIPPED
                    End Select
                Else
                    'DONE: Items in bags
                    If Not Items.ContainsKey(dstBag) Then Return InventoryChangeFailure.EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG
                    If ItemInfo.IsContainer Then
                        If Item.IsFree Then
                            Return InventoryChangeFailure.EQUIP_ERR_OK
                        Else
                            Return InventoryChangeFailure.EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS
                        End If
                    End If

                    If Items(dstBag).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_QUIVER Then
                        If ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_PROJECTILE Then
                            If Items(dstBag).ItemInfo.SubClass <> ItemInfo.SubClass Then
                                'Inserting Ammo in not proper AmmoType bag
                                Return InventoryChangeFailure.EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG
                            Else
                                'Inserting Ammo in proper AmmoType bag
                                Return InventoryChangeFailure.EQUIP_ERR_OK
                            End If
                        Else
                            Return InventoryChangeFailure.EQUIP_ERR_ONLY_AMMO_CAN_GO_HERE
                        End If
                    Else
                        Return InventoryChangeFailure.EQUIP_ERR_OK
                    End If

                End If
            Catch err As Exception
                Log.WriteLine(LogType.FAILED, "[{0}:{1}] Unable to equip item. {2} {3}", client.IP, client.Port, Environment.NewLine, err.ToString)
                Return InventoryChangeFailure.EQUIP_ERR_CANT_DO_RIGHT_NOW
            End Try
        End Function

        Public Function ItemSTACK(ByVal srcBag As Byte, ByVal srcSlot As Byte, ByVal dstBag As Byte, ByVal dstSlot As Byte) As Boolean
            Dim srcItem As ItemObject = Nothing
            Dim dstItem As ItemObject = Nothing
            If srcBag <> 0 Then
                srcItem = Items(srcBag).Items(srcSlot)
            Else
                srcItem = Items(srcSlot)
            End If
            If dstBag <> 0 Then
                dstItem = Items(dstBag).Items(dstSlot)
            Else
                dstItem = Items(dstSlot)
            End If

            'DONE: If already full, just swap
            If srcItem.StackCount = dstItem.ItemInfo.Stackable Or dstItem.StackCount = dstItem.ItemInfo.Stackable Then Return False

            'DONE: Same item types -> stack if not full, else just swap !Nooo, else fill
            If (srcItem.ItemEntry = dstItem.ItemEntry) AndAlso (dstItem.StackCount + srcItem.StackCount) <= dstItem.ItemInfo.Stackable Then
                dstItem.StackCount += srcItem.StackCount
                ItemREMOVE(srcBag, srcSlot, True, True)

                SendItemUpdate(dstItem)
                If dstBag > 0 Then SendItemUpdate(Items(dstBag))
                dstItem.Save(False)
                Return True
            End If
            'DONE: Same item types, but bigger than max count -> fill destination
            If (srcItem.ItemEntry = dstItem.ItemEntry) Then
                srcItem.StackCount -= dstItem.ItemInfo.Stackable - dstItem.StackCount
                dstItem.StackCount = dstItem.ItemInfo.Stackable

                SendItemUpdate(dstItem)
                If dstBag > 0 Then SendItemUpdate(Items(dstBag))
                SendItemUpdate(srcItem)
                If srcBag > 0 Then SendItemUpdate(Items(srcBag))
                srcItem.Save(False)
                dstItem.Save(False)
                Return True
            End If
            Return False
        End Function

        Public Sub ItemSPLIT(ByVal srcBag As Byte, ByVal srcSlot As Byte, ByVal dstBag As Byte, ByVal dstSlot As Byte, ByVal Count As Integer)
            Dim srcItem As ItemObject = Nothing
            Dim dstItem As ItemObject = Nothing

            'DONE: Get source item
            If srcBag = 0 Then
                If Not client.Character.Items.ContainsKey(srcSlot) Then
                    Dim EQUIP_ERR_ITEM_NOT_FOUND As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                    Try
                        EQUIP_ERR_ITEM_NOT_FOUND.AddInt8(InventoryChangeFailure.EQUIP_ERR_ITEM_NOT_FOUND)
                        EQUIP_ERR_ITEM_NOT_FOUND.AddUInt64(0)
                        EQUIP_ERR_ITEM_NOT_FOUND.AddUInt64(0)
                        EQUIP_ERR_ITEM_NOT_FOUND.AddInt8(0)
                        client.Send(EQUIP_ERR_ITEM_NOT_FOUND)
                    Finally
                        EQUIP_ERR_ITEM_NOT_FOUND.Dispose()
                    End Try
                    Exit Sub
                End If
                srcItem = Items(srcSlot)
            Else
                If Not client.Character.Items(srcBag).Items.ContainsKey(srcSlot) Then
                    Dim EQUIP_ERR_ITEM_NOT_FOUND As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                    Try
                        EQUIP_ERR_ITEM_NOT_FOUND.AddInt8(InventoryChangeFailure.EQUIP_ERR_ITEM_NOT_FOUND)
                        EQUIP_ERR_ITEM_NOT_FOUND.AddUInt64(0)
                        EQUIP_ERR_ITEM_NOT_FOUND.AddUInt64(0)
                        EQUIP_ERR_ITEM_NOT_FOUND.AddInt8(0)
                        client.Send(EQUIP_ERR_ITEM_NOT_FOUND)
                    Finally
                        EQUIP_ERR_ITEM_NOT_FOUND.Dispose()
                    End Try
                    Exit Sub
                End If
                srcItem = Items(srcBag).Items(srcSlot)
            End If

            'DONE: Get destination item
            If dstBag = 0 Then
                If Items.ContainsKey(dstSlot) Then dstItem = Items(dstSlot)
            Else
                If Items(dstBag).Items.ContainsKey(dstSlot) Then dstItem = Items(dstBag).Items(dstSlot)
            End If

            If dstSlot = 255 Then
                Dim notHandledYet As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                Try
                    notHandledYet.AddInt8(InventoryChangeFailure.EQUIP_ERR_COULDNT_SPLIT_ITEMS)
                    notHandledYet.AddUInt64(srcItem.GUID)
                    notHandledYet.AddUInt64(dstItem.GUID)
                    notHandledYet.AddInt8(0)
                    client.Send(notHandledYet)
                Finally
                    notHandledYet.Dispose()
                End Try
                Exit Sub
            End If

            If Count = srcItem.StackCount Then
                ItemSWAP(srcBag, srcSlot, dstBag, dstSlot)
                Exit Sub
            End If

            If Count > srcItem.StackCount Then
                Dim EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                Try
                    EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT.AddInt8(InventoryChangeFailure.EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT)
                    EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT.AddUInt64(srcItem.GUID)
                    EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT.AddUInt64(0)
                    EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT.AddInt8(0)
                    client.Send(EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT)
                Finally
                    EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT.Dispose()
                End Try
                Exit Sub
            End If

            'DONE: Create new item if needed
            If dstItem Is Nothing Then
                srcItem.StackCount -= Count

                Dim tmpItem As New ItemObject(srcItem.ItemEntry, GUID) With {
                    .StackCount = Count
                }

                dstItem = tmpItem
                tmpItem.Save()
                ItemSETSLOT(tmpItem, dstBag, dstSlot)

                Dim SMSG_UPDATE_OBJECT As New UpdatePacketClass
                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
                Try
                    tmpItem.FillAllUpdateFlags(tmpUpdate)
                    tmpUpdate.AddToPacket((SMSG_UPDATE_OBJECT), ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, tmpItem)
                    client.Send((SMSG_UPDATE_OBJECT))
                Finally
                    SMSG_UPDATE_OBJECT.Dispose()
                    tmpUpdate.Dispose()
                End Try
                SendItemUpdate(srcItem)
                SendItemUpdate(dstItem)
                If srcBag <> 0 Then
                    SendItemUpdate(Items(srcBag))
                    Items(srcBag).Save(False)
                End If
                If dstBag <> 0 Then
                    SendItemUpdate(Items(dstBag))
                    Items(dstBag).Save(False)
                End If
                srcItem.Save(False)
                dstItem.Save(False)
                Exit Sub
            End If

            'DONE: Split
            If srcItem.ItemEntry = dstItem.ItemEntry Then
                If (dstItem.StackCount + Count) <= dstItem.ItemInfo.Stackable Then
                    srcItem.StackCount -= Count
                    dstItem.StackCount += Count

                    SendItemUpdate(srcItem)
                    SendItemUpdate(dstItem)
                    If srcBag <> 0 Then
                        SendItemUpdate(Items(srcBag))
                        Items(srcBag).Save(False)
                    End If
                    If dstBag <> 0 Then
                        SendItemUpdate(Items(dstBag))
                        Items(dstBag).Save(False)
                    End If
                    srcItem.Save(False)
                    dstItem.Save(False)

                    Dim EQUIP_ERR_OK As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                    Try
                        EQUIP_ERR_OK.AddInt8(InventoryChangeFailure.EQUIP_ERR_OK)
                        EQUIP_ERR_OK.AddUInt64(srcItem.GUID)
                        EQUIP_ERR_OK.AddUInt64(dstItem.GUID)
                        EQUIP_ERR_OK.AddInt8(0)
                        client.Send(EQUIP_ERR_OK)
                    Finally
                        EQUIP_ERR_OK.Dispose()
                    End Try
                    Exit Sub
                End If
            End If

            Dim response As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
            Try
                response.AddInt8(InventoryChangeFailure.EQUIP_ERR_COULDNT_SPLIT_ITEMS)
                response.AddUInt64(srcItem.GUID)
                response.AddUInt64(dstItem.GUID)
                response.AddInt8(0)
                client.Send(response)
            Catch
                response.Dispose()
            End Try
        End Sub

        Public Sub ItemSWAP(ByVal srcBag As Byte, ByVal srcSlot As Byte, ByVal dstBag As Byte, ByVal dstSlot As Byte)
            'DONE: Disable when dead, attackTarget<>0
            If DEAD Then
                SendInventoryChangeFailure(Me, InventoryChangeFailure.EQUIP_ERR_YOU_ARE_DEAD, ItemGetGUID(srcBag, srcSlot), ItemGetGUID(dstBag, dstSlot))
                Exit Sub
            End If

            Dim errCode As Byte = InventoryChangeFailure.EQUIP_ERR_ITEMS_CANT_BE_SWAPPED

            'Disable moving the bag into same bag
            If (srcBag = 0 AndAlso srcSlot = dstBag AndAlso dstBag > 0) OrElse (dstBag = 0 AndAlso dstSlot = srcBag AndAlso srcBag > 0) Then
                SendInventoryChangeFailure(Me, errCode, Items(srcSlot).GUID, 0)
                Exit Sub
            End If

            Try
                If srcBag > 0 AndAlso dstBag > 0 Then
                    'DONE: Betwen Bags Moving
                    If Not Items(srcBag).Items.ContainsKey(srcSlot) Then
                        errCode = InventoryChangeFailure.EQUIP_ERR_SLOT_IS_EMPTY
                    Else
                        errCode = ItemCANEQUIP(Items(srcBag).Items(srcSlot), dstBag, dstSlot)
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK AndAlso Items(dstBag).Items.ContainsKey(dstSlot) Then
                            errCode = ItemCANEQUIP(Items(dstBag).Items(dstSlot), srcBag, srcSlot)
                        End If

                        'DONE: Moving item
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK Then

                            If Not Items(dstBag).Items.ContainsKey(dstSlot) Then
                                If Not Items(srcBag).Items.ContainsKey(srcSlot) Then
                                    Items(dstBag).Items.Remove(dstSlot)
                                    Items(srcBag).Items.Remove(srcSlot)
                                Else
                                    Items(dstBag).Items(dstSlot) = Items(srcBag).Items(srcSlot)
                                    Items(srcBag).Items.Remove(srcSlot)
                                End If
                            Else
                                If Not Items(srcBag).Items.ContainsKey(srcSlot) Then
                                    Items(srcBag).Items(srcSlot) = Items(dstBag).Items(dstSlot)
                                    Items(dstBag).Items.Remove(dstSlot)
                                Else
                                    If ItemSTACK(srcBag, srcSlot, dstBag, dstSlot) Then Exit Sub
                                    Dim tmp As ItemObject = Items(dstBag).Items(dstSlot)
                                    Items(dstBag).Items(dstSlot) = Items(srcBag).Items(srcSlot)
                                    Items(srcBag).Items(srcSlot) = tmp
                                    tmp = Nothing
                                End If
                            End If

                            SendItemUpdate(Items(srcBag))
                            If dstBag <> srcBag Then
                                SendItemUpdate(Items(dstBag))
                            End If

                            CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", dstSlot, Items(dstBag).GUID, Items(dstBag).Items(dstSlot).GUID - GUID_ITEM))
                            If Items(srcBag).Items.ContainsKey(srcSlot) Then CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", srcSlot, Items(srcBag).GUID, Items(srcBag).Items(srcSlot).GUID - GUID_ITEM))
                        End If
                    End If

                ElseIf srcBag > 0 Then
                    'DONE: from Bag to Inventory
                    If Not Items(srcBag).Items.ContainsKey(srcSlot) Then
                        errCode = InventoryChangeFailure.EQUIP_ERR_SLOT_IS_EMPTY
                    Else
                        errCode = ItemCANEQUIP(Items(srcBag).Items(srcSlot), dstBag, dstSlot)
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK AndAlso Items.ContainsKey(dstSlot) Then
                            errCode = ItemCANEQUIP(Items(dstSlot), srcBag, srcSlot)
                        End If

                        'DONE: Moving item
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK Then

                            If Not Items.ContainsKey(dstSlot) Then
                                If Not Items(srcBag).Items.ContainsKey(srcSlot) Then
                                    Items.Remove(dstSlot)
                                    Items(srcBag).Items.Remove(srcSlot)
                                Else
                                    Items(dstSlot) = Items(srcBag).Items(srcSlot)
                                    Items(srcBag).Items.Remove(srcSlot)
                                    If dstSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateAddItemStats(Items(dstSlot), dstSlot)
                                End If
                            Else
                                If Not Items(srcBag).Items.ContainsKey(srcSlot) Then
                                    Items(srcBag).Items(srcSlot) = Items(dstSlot)
                                    Items.Remove(dstSlot)
                                    If dstSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateRemoveItemStats(Items(srcBag).Items(srcSlot), dstSlot)
                                Else
                                    If ItemSTACK(srcBag, srcSlot, dstBag, dstSlot) Then Exit Sub
                                    Dim tmp As ItemObject = Items(dstSlot)
                                    Items(dstSlot) = Items(srcBag).Items(srcSlot)
                                    Items(srcBag).Items(srcSlot) = tmp
                                    If dstSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then
                                        UpdateAddItemStats(Items(dstSlot), dstSlot)
                                        UpdateRemoveItemStats(Items(srcBag).Items(srcSlot), dstSlot)
                                    End If
                                    tmp = Nothing
                                End If
                            End If

                            SendItemAndCharacterUpdate(Items(srcBag))
                            CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", dstSlot, GUID, Items(dstSlot).GUID - GUID_ITEM))
                            If Items(srcBag).Items.ContainsKey(srcSlot) Then CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", srcSlot, Items(srcBag).GUID, Items(srcBag).Items(srcSlot).GUID - GUID_ITEM))
                        End If
                    End If

                ElseIf dstBag > 0 Then
                    'DONE: from Inventory to Bag
                    If Not Items.ContainsKey(srcSlot) Then
                        errCode = InventoryChangeFailure.EQUIP_ERR_SLOT_IS_EMPTY
                    Else
                        errCode = ItemCANEQUIP(Items(srcSlot), dstBag, dstSlot)
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK AndAlso Items(dstBag).Items.ContainsKey(dstSlot) Then
                            errCode = ItemCANEQUIP(Items(dstBag).Items(dstSlot), srcBag, srcSlot)
                        End If

                        'DONE: Moving item
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK Then

                            If Not Items(dstBag).Items.ContainsKey(dstSlot) Then
                                If Not Items.ContainsKey(srcSlot) Then
                                    Items(dstBag).Items.Remove(dstSlot)
                                    Items.Remove(srcSlot)
                                Else
                                    Items(dstBag).Items(dstSlot) = Items(srcSlot)
                                    Items.Remove(srcSlot)
                                    If srcSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateRemoveItemStats(Items(dstBag).Items(dstSlot), srcSlot)
                                End If
                            Else
                                If Not Items.ContainsKey(srcSlot) Then
                                    Items(srcSlot) = Items(dstBag).Items(dstSlot)
                                    Items(dstBag).Items.Remove(dstSlot)
                                    If srcSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateAddItemStats(Items(srcSlot), srcSlot)
                                Else
                                    If ItemSTACK(srcBag, srcSlot, dstBag, dstSlot) Then Exit Sub
                                    Dim tmp As ItemObject = Items(dstBag).Items(dstSlot)
                                    Items(dstBag).Items(dstSlot) = Items(srcSlot)
                                    Items(srcSlot) = tmp
                                    If srcSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then
                                        UpdateAddItemStats(Items(srcSlot), srcSlot)
                                        UpdateRemoveItemStats(Items(dstBag).Items(dstSlot), srcSlot)
                                    End If
                                    tmp = Nothing
                                End If
                            End If

                            SendItemAndCharacterUpdate(Items(dstBag))
                            CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", dstSlot, Items(dstBag).GUID, Items(dstBag).Items(dstSlot).GUID - GUID_ITEM))
                            If Items.ContainsKey(srcSlot) Then CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", srcSlot, GUID, Items(srcSlot).GUID - GUID_ITEM))
                        End If
                    End If

                Else
                    'DONE: Inventory Moving
                    If Not Items.ContainsKey(srcSlot) Then
                        errCode = InventoryChangeFailure.EQUIP_ERR_SLOT_IS_EMPTY
                    Else
                        errCode = ItemCANEQUIP(Items(srcSlot), dstBag, dstSlot)
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK AndAlso Items.ContainsKey(dstSlot) Then
                            errCode = ItemCANEQUIP(Items(dstSlot), srcBag, srcSlot)
                        End If

                        'DONE: Moving item
                        If errCode = InventoryChangeFailure.EQUIP_ERR_OK Then

                            If Not Items.ContainsKey(dstSlot) Then
                                If Not Items.ContainsKey(srcSlot) Then
                                    Items.Remove(dstSlot)
                                    Items.Remove(srcSlot)
                                Else
                                    Items(dstSlot) = Items(srcSlot)
                                    Items.Remove(srcSlot)
                                    If dstSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateAddItemStats(Items(dstSlot), dstSlot)
                                    If srcSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateRemoveItemStats(Items(dstSlot), srcSlot)
                                End If
                            Else
                                If Not Items.ContainsKey(srcSlot) Then
                                    Items(srcSlot) = Items(dstSlot)
                                    Items.Remove(dstSlot)
                                    If dstSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateRemoveItemStats(Items(srcSlot), dstSlot)
                                    If srcSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateAddItemStats(Items(srcSlot), srcSlot)
                                Else
                                    If ItemSTACK(srcBag, srcSlot, dstBag, dstSlot) Then Exit Sub
                                    Dim tmp As ItemObject = Items(dstSlot)
                                    Items(dstSlot) = Items(srcSlot)
                                    Items(srcSlot) = tmp
                                    If dstSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then
                                        UpdateAddItemStats(Items(dstSlot), dstSlot)
                                        UpdateRemoveItemStats(Items(srcSlot), dstSlot)
                                    End If
                                    If srcSlot < InventorySlots.INVENTORY_SLOT_BAG_END Then
                                        UpdateAddItemStats(Items(srcSlot), srcSlot)
                                        UpdateRemoveItemStats(Items(dstSlot), srcSlot)
                                    End If
                                    tmp = Nothing
                                End If
                            End If

                            SendItemAndCharacterUpdate(Items(dstSlot))
                            CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", dstSlot, GUID, Items(dstSlot).GUID - GUID_ITEM))
                            If Items.ContainsKey(srcSlot) Then CharacterDatabase.Update(String.Format("UPDATE characters_inventory SET item_slot = {0}, item_bag = {1} WHERE item_guid = {2};", srcSlot, GUID, Items(srcSlot).GUID - GUID_ITEM))
                        End If
                    End If
                End If

            Catch err As Exception
                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] Unable to swap items. {2}{3}", client.IP, client.Port, Environment.NewLine, err.ToString)
            Finally

                If errCode <> InventoryChangeFailure.EQUIP_ERR_OK Then
                    SendInventoryChangeFailure(Me, errCode, ItemGetGUID(srcBag, srcSlot), ItemGetGUID(dstBag, dstSlot))
                End If
            End Try
        End Sub

        Public Function ItemGET(ByVal srcBag As Byte, ByVal srcSlot As Byte) As ItemObject
            If srcBag = 0 Then
                If Items.ContainsKey(srcSlot) Then Return Items(srcSlot)
            Else
                If Items.ContainsKey(srcBag) AndAlso Items(srcBag).Items IsNot Nothing AndAlso Items(srcBag).Items.ContainsKey(srcSlot) Then Return Items(srcBag).Items(srcSlot)
            End If

            Return Nothing
        End Function

        Public Function ItemGETByGUID(ByVal GUID As ULong) As ItemObject
            Dim srcBag As Byte, srcSlot As Byte
            srcSlot = client.Character.ItemGetSLOTBAG(GUID, srcBag)
            If srcSlot = ITEM_SLOT_NULL Then Return Nothing
            Return ItemGET(srcBag, srcSlot)
        End Function
        Public Function ItemGetGUID(ByVal srcBag As Byte, ByVal srcSlot As Byte) As ULong
            If srcBag = 0 Then
                If Items.ContainsKey(srcSlot) Then Return Items(srcSlot).GUID
            Else
                If Items.ContainsKey(srcBag) AndAlso Items(srcBag).Items IsNot Nothing AndAlso Items(srcBag).Items.ContainsKey(srcSlot) Then Return Items(srcBag).Items(srcSlot).GUID
            End If

            Return 0
        End Function

        Public Function ItemGetSLOTBAG(ByVal GUID As ULong, ByRef bag As Byte) As Byte

            For slot As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                If Items.ContainsKey(slot) AndAlso Items(slot).GUID = GUID Then
                    bag = 0
                    Return slot
                End If
            Next
            For slot As Byte = KeyRingSlots.KEYRING_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                If Items.ContainsKey(slot) AndAlso Items(slot).GUID = GUID Then
                    bag = 0
                    Return slot
                End If
            Next
            For bag = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                If Items.ContainsKey(bag) Then
                    For Each item As KeyValuePair(Of Byte, ItemObject) In Items(bag).Items
                        If item.Value.GUID = GUID Then Return item.Key
                    Next
                End If
            Next

            bag = ITEM_SLOT_NULL
            Return ITEM_SLOT_NULL
        End Function

        Public Sub UpdateAddItemStats(ByRef Item As ItemObject, ByVal slot As Byte)
            'TODO: Fill in the other item stat types also
            For i As Byte = 0 To 9
                Select Case Item.ItemInfo.ItemBonusStatType(i)
                    Case ITEM_STAT_TYPE.HEALTH
                        Life.Bonus += Item.ItemInfo.ItemBonusStatValue(i)
                    Case ITEM_STAT_TYPE.AGILITY
                        Agility.Base += Item.ItemInfo.ItemBonusStatValue(i)
                        Agility.PositiveBonus += Item.ItemInfo.ItemBonusStatValue(i)
                        Resistances(DamageTypes.DMG_PHYSICAL).Base += Item.ItemInfo.ItemBonusStatValue(i) * 2
                    Case ITEM_STAT_TYPE.STRENGTH
                        Strength.Base += Item.ItemInfo.ItemBonusStatValue(i)
                        Strength.PositiveBonus += Item.ItemInfo.ItemBonusStatValue(i)
                    Case ITEM_STAT_TYPE.INTELLECT
                        Intellect.Base += Item.ItemInfo.ItemBonusStatValue(i)
                        Intellect.PositiveBonus += Item.ItemInfo.ItemBonusStatValue(i)
                        Life.Bonus += Item.ItemInfo.ItemBonusStatValue(i) * 15
                    Case ITEM_STAT_TYPE.SPIRIT
                        Spirit.Base += Item.ItemInfo.ItemBonusStatValue(i)
                        Spirit.PositiveBonus += Item.ItemInfo.ItemBonusStatValue(i)
                    Case ITEM_STAT_TYPE.STAMINA
                        Stamina.Base += Item.ItemInfo.ItemBonusStatValue(i)
                        Stamina.PositiveBonus += Item.ItemInfo.ItemBonusStatValue(i)
                        Life.Bonus += Item.ItemInfo.ItemBonusStatValue(i) * 10
                    Case ITEM_STAT_TYPE.BLOCK
                        combatBlockValue += Item.ItemInfo.ItemBonusStatValue(i)
                End Select
            Next

            For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                Resistances(i).Base += Item.ItemInfo.Resistances(i)
            Next

            combatBlockValue += Item.ItemInfo.Block

            If Item.ItemInfo.Delay > 0 Then
                If slot = EquipmentSlots.EQUIPMENT_SLOT_RANGED Then
                    AttackTimeBase(2) = Item.ItemInfo.Delay
                ElseIf slot = EquipmentSlots.EQUIPMENT_SLOT_MAINHAND Then
                    AttackTimeBase(0) = Item.ItemInfo.Delay
                ElseIf slot = EquipmentSlots.EQUIPMENT_SLOT_OFFHAND Then
                    AttackTimeBase(1) = Item.ItemInfo.Delay
                End If
            End If

            'DONE: Add the equip spells to the character
            For i As Byte = 0 To 4
                If Item.ItemInfo.Spells(i).SpellID > 0 Then
                    If WS_Spells.SPELLs.ContainsKey(Item.ItemInfo.Spells(i).SpellID) Then
                        Dim SpellInfo As SpellInfo = WS_Spells.SPELLs(Item.ItemInfo.Spells(i).SpellID)
                        If Item.ItemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.ON_EQUIP Then
                            ApplySpell(Item.ItemInfo.Spells(i).SpellID)
                        ElseIf Item.ItemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.USE Then
                            'DONE: Show item cooldown when equipped
                            Dim cooldown As New PacketClass(OPCODES.SMSG_ITEM_COOLDOWN)
                            Try
                                cooldown.AddUInt64(Item.GUID)
                                cooldown.AddInt32(Item.ItemInfo.Spells(i).SpellID)
                                client.Send(cooldown)
                            Finally
                                cooldown.Dispose()
                            End Try
                        End If
                    End If
                End If
            Next i

            'DONE: Bind item to player
            If Item.ItemInfo.Bonding = ITEM_BONDING_TYPE.BIND_WHEN_EQUIPED AndAlso Not Item.IsSoulBound Then Item.SoulbindItem()

            'DONE: Cancel any spells that are being casted while equipping an item
            FinishAllSpells()

            For Each Enchant As KeyValuePair(Of Byte, TEnchantmentInfo) In Item.Enchantments
                Item.AddEnchantBonus(Enchant.Key, Me)
            Next

            CalculateMinMaxDamage(Me, WeaponAttackType.BASE_ATTACK)
            CalculateMinMaxDamage(Me, WeaponAttackType.OFF_ATTACK)
            CalculateMinMaxDamage(Me, WeaponAttackType.RANGED_ATTACK)

            If ManaType = ManaTypes.TYPE_MANA OrElse Classe = Classes.CLASS_DRUID Then UpdateManaRegen()
            FillStatsUpdateFlags()
        End Sub

        Public Sub UpdateRemoveItemStats(ByRef Item As ItemObject, ByVal slot As Byte)
            'TODO: Add the other item stat types here also
            For i As Byte = 0 To 9
                Select Case Item.ItemInfo.ItemBonusStatType(i)
                    Case ITEM_STAT_TYPE.HEALTH
                        Life.Bonus -= Item.ItemInfo.ItemBonusStatValue(i)
                    Case ITEM_STAT_TYPE.AGILITY
                        Agility.Base -= Item.ItemInfo.ItemBonusStatValue(i)
                        Agility.PositiveBonus -= Item.ItemInfo.ItemBonusStatValue(i)
                        Resistances(DamageTypes.DMG_PHYSICAL).Base -= Item.ItemInfo.ItemBonusStatValue(i) * 2
                    Case ITEM_STAT_TYPE.STRENGTH
                        Strength.Base -= Item.ItemInfo.ItemBonusStatValue(i)
                        Strength.PositiveBonus -= Item.ItemInfo.ItemBonusStatValue(i)
                    Case ITEM_STAT_TYPE.INTELLECT
                        Intellect.Base -= Item.ItemInfo.ItemBonusStatValue(i)
                        Intellect.PositiveBonus -= Item.ItemInfo.ItemBonusStatValue(i)
                        Mana.Bonus -= Item.ItemInfo.ItemBonusStatValue(i) * 15
                    Case ITEM_STAT_TYPE.SPIRIT
                        Spirit.Base -= Item.ItemInfo.ItemBonusStatValue(i)
                        Spirit.PositiveBonus -= Item.ItemInfo.ItemBonusStatValue(i)
                    Case ITEM_STAT_TYPE.STAMINA
                        Stamina.Base -= Item.ItemInfo.ItemBonusStatValue(i)
                        Stamina.PositiveBonus -= Item.ItemInfo.ItemBonusStatValue(i)
                        Life.Bonus -= Item.ItemInfo.ItemBonusStatValue(i) * 10
                    Case ITEM_STAT_TYPE.BLOCK
                        combatBlockValue -= Item.ItemInfo.ItemBonusStatValue(i)
                End Select
            Next

            For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                Resistances(i).Base -= Item.ItemInfo.Resistances(i)
            Next

            combatBlockValue -= Item.ItemInfo.Block

            If Item.ItemInfo.Delay > 0 Then
                If slot = EquipmentSlots.EQUIPMENT_SLOT_RANGED Then
                    AttackTimeBase(2) = 0
                ElseIf slot = EquipmentSlots.EQUIPMENT_SLOT_MAINHAND Then
                    If Classe = Classes.CLASS_ROGUE Then AttackTimeBase(0) = 1900 Else AttackTimeBase(0) = 2000
                ElseIf slot = EquipmentSlots.EQUIPMENT_SLOT_OFFHAND Then
                    AttackTimeBase(1) = 0
                End If
            End If

            'DONE: Remove the equip spells to the character
            For i As Byte = 0 To 4
                If Item.ItemInfo.Spells(i).SpellID > 0 Then
                    If WS_Spells.SPELLs.ContainsKey(Item.ItemInfo.Spells(i).SpellID) Then
                        Dim SpellInfo As SpellInfo = WS_Spells.SPELLs(Item.ItemInfo.Spells(i).SpellID)
                        If Item.ItemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.ON_EQUIP Then
                            RemoveAuraBySpell(Item.ItemInfo.Spells(i).SpellID)
                        End If
                    End If
                End If
            Next i

            For Each Enchant As KeyValuePair(Of Byte, TEnchantmentInfo) In Item.Enchantments
                Item.RemoveEnchantBonus(Enchant.Key)
            Next

            CalculateMinMaxDamage(Me, WeaponAttackType.BASE_ATTACK)
            CalculateMinMaxDamage(Me, WeaponAttackType.OFF_ATTACK)
            CalculateMinMaxDamage(Me, WeaponAttackType.RANGED_ATTACK)

            If ManaType = ManaTypes.TYPE_MANA OrElse Classe = Classes.CLASS_DRUID Then UpdateManaRegen()
            FillStatsUpdateFlags()
        End Sub

        'Creature Interactions
        Public Sub SendGossip(ByVal cGUID As ULong, ByVal cTextID As Integer, Optional ByRef Menu As GossipMenu = Nothing, Optional ByRef qMenu As QuestMenu = Nothing)
            Dim SMSG_GOSSIP_MESSAGE As PacketClass = New PacketClass(OPCODES.SMSG_GOSSIP_MESSAGE)
            Try
                SMSG_GOSSIP_MESSAGE.AddUInt64(cGUID)
                SMSG_GOSSIP_MESSAGE.AddInt32(cTextID)
                If Menu Is Nothing Then
                    SMSG_GOSSIP_MESSAGE.AddInt32(0)
                Else
                    SMSG_GOSSIP_MESSAGE.AddInt32(Menu.Menus.Count)
                    Dim index As Integer = 0
                    While index < Menu.Menus.Count
                        SMSG_GOSSIP_MESSAGE.AddInt32(index)
                        SMSG_GOSSIP_MESSAGE.AddInt8(Menu.Icons(index))
                        SMSG_GOSSIP_MESSAGE.AddInt8(Menu.Coded(index))
                        SMSG_GOSSIP_MESSAGE.AddString(Menu.Menus(index))
                        index += 1
                    End While
                End If

                If qMenu Is Nothing Then
                    SMSG_GOSSIP_MESSAGE.AddInt32(0)
                Else
                    SMSG_GOSSIP_MESSAGE.AddInt32(qMenu.Names.Count)
                    Dim index As Integer = 0
                    While index < qMenu.Names.Count
                        SMSG_GOSSIP_MESSAGE.AddInt32(qMenu.IDs(index))
                        SMSG_GOSSIP_MESSAGE.AddInt32(qMenu.Icons(index))
                        SMSG_GOSSIP_MESSAGE.AddInt32(qMenu.Levels(index))
                        SMSG_GOSSIP_MESSAGE.AddString(qMenu.Names(index))
                        index += 1
                    End While
                End If

                client.Send(SMSG_GOSSIP_MESSAGE)
            Finally
                SMSG_GOSSIP_MESSAGE.Dispose()
            End Try
        End Sub

        Public Sub SendGossipComplete()
            Dim SMSG_GOSSIP_COMPLETE As PacketClass = New PacketClass(OPCODES.SMSG_GOSSIP_COMPLETE)
            Try
                client.Send(SMSG_GOSSIP_COMPLETE)
            Finally
                SMSG_GOSSIP_COMPLETE.Dispose()
            End Try
        End Sub

        Public Sub SendPointOfInterest(ByVal x As Single, ByVal y As Single, ByVal icon As Integer, ByVal flags As Integer, ByVal data As Integer, ByVal name As String)
            Dim SMSG_GOSSIP_POI As PacketClass = New PacketClass(OPCODES.SMSG_GOSSIP_POI)
            Try
                SMSG_GOSSIP_POI.AddInt32(flags)
                SMSG_GOSSIP_POI.AddSingle(x)
                SMSG_GOSSIP_POI.AddSingle(y)
                SMSG_GOSSIP_POI.AddInt32(icon)
                SMSG_GOSSIP_POI.AddInt32(data)
                SMSG_GOSSIP_POI.AddString(name)
                client.Send(SMSG_GOSSIP_POI)
            Finally
                SMSG_GOSSIP_POI.Dispose()
            End Try
        End Sub

        Public Sub SendTalking(ByVal TextID As Integer)

            If NPCTexts.ContainsKey(TextID) = False Then
                Dim tmpText As New NPCText(TextID)
                'The New does a an add to the .Containskey collection above
            End If

            'DONE: Load TextID
            Dim response As New PacketClass(OPCODES.SMSG_NPC_TEXT_UPDATE)
            Try
                response.AddInt32(TextID)

                If NPCTexts(TextID).Count = 0 Then
                    response.AddInt32(0)
                    response.AddString(NPCTexts(TextID).TextLine1(0))
                    response.AddString(NPCTexts(TextID).TextLine2(0))
                Else
                    For i As Integer = 0 To 7
                        response.AddSingle(NPCTexts(TextID).Probability(i))     'Probability
                        response.AddString(NPCTexts(TextID).TextLine1(i))       'Text1
                        If NPCTexts(TextID).TextLine2(i) = "" Then
                            response.AddString(NPCTexts(TextID).TextLine1(i))   'Text2
                        Else
                            response.AddString(NPCTexts(TextID).TextLine2(i))   'Text2
                        End If
                        response.AddInt32(NPCTexts(TextID).Language(i))         'Language
                        response.AddInt32(NPCTexts(TextID).EmoteDelay1(i))      'Emote1.Delay
                        response.AddInt32(NPCTexts(TextID).Emote1(i))           'Emote1.Emote
                        response.AddInt32(NPCTexts(TextID).EmoteDelay2(i))      'Emote2.Delay
                        response.AddInt32(NPCTexts(TextID).Emote2(i))           'Emote2.Emote
                        response.AddInt32(NPCTexts(TextID).EmoteDelay3(i))      'Emote3.Delay
                        response.AddInt32(NPCTexts(TextID).Emote3(i))           'Emote3.Emote
                    Next
                End If

                client.Send(response)
            Finally
                response.Dispose()
            End Try
        End Sub

        Public Sub BindPlayer(ByVal cGUID As ULong)
            bindpoint_positionX = positionX
            bindpoint_positionY = positionY
            bindpoint_positionZ = positionZ
            bindpoint_map_id = MapID
            bindpoint_zone_id = ZoneID
            SaveCharacter()

            Dim SMSG_BINDPOINTUPDATE As New PacketClass(OPCODES.SMSG_BINDPOINTUPDATE)
            Try
                SMSG_BINDPOINTUPDATE.AddSingle(bindpoint_positionX)
                SMSG_BINDPOINTUPDATE.AddSingle(bindpoint_positionY)
                SMSG_BINDPOINTUPDATE.AddSingle(bindpoint_positionZ)
                SMSG_BINDPOINTUPDATE.AddInt32(bindpoint_map_id)
                SMSG_BINDPOINTUPDATE.AddInt32(bindpoint_zone_id)
                client.Send(SMSG_BINDPOINTUPDATE)
            Finally
                SMSG_BINDPOINTUPDATE.Dispose()
            End Try

            Dim SMSG_PLAYERBOUND As New PacketClass(OPCODES.SMSG_PLAYERBOUND)
            Try
                SMSG_PLAYERBOUND.AddUInt64(cGUID)
                SMSG_PLAYERBOUND.AddInt32(bindpoint_zone_id)
                client.Send(SMSG_PLAYERBOUND)
            Finally
                SMSG_PLAYERBOUND.Dispose()
            End Try
        End Sub

        'Character Movement
        Public Sub Teleport(ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As Integer)
            If MapID <> map Then
                Transfer(posX, posY, posZ, ori, map)
                Exit Sub
            End If

            Log.WriteLine(LogType.INFORMATION, "World: Player Teleport: X[{0}], Y[{1}], Z[{2}], O[{3}]", posX, posY, posZ, ori)

            charMovementFlags = 0

            Dim packet As New PacketClass(OPCODES.MSG_MOVE_TELEPORT_ACK)
            Try
                packet.AddPackGUID(GUID)
                packet.AddInt32(0)              'Counter
                packet.AddInt32(0)              'Movement flags
                packet.AddInt32(MsTime)
                packet.AddSingle(posX)
                packet.AddSingle(posY)
                packet.AddSingle(posZ)
                packet.AddSingle(ori)
                packet.AddInt32(0)
                client.Send(packet)
            Finally
                packet.Dispose()
            End Try

            positionX = posX
            positionY = posY
            positionZ = posZ
            orientation = ori

            MoveCell(Me)
            UpdateCell(Me)

            client.Character.ZoneID = AreaTable(GetAreaFlag(posX, posY, client.Character.MapID)).Zone
        End Sub

        Public Sub Transfer(ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As Integer)
            Log.WriteLine(LogType.INFORMATION, "World: Player Transfer: X[{0}], Y[{1}], Z[{2}], O[{3}], MAP[{4}]", posX, posY, posZ, ori, map)

            Dim p As New PacketClass(OPCODES.SMSG_TRANSFER_PENDING)
            Try
                p.AddInt32(map)
                If OnTransport IsNot Nothing Then
                    p.AddInt32(OnTransport.ID)      'Only if on transport
                    p.AddInt32(OnTransport.MapID)   'Only if on transport
                End If
                client.Send(p)
            Finally
                p.Dispose()
            End Try
            'Actions Here
            RemoveFromWorld(Me)

            If (OnTransport IsNot Nothing) AndAlso (TypeOf OnTransport Is TransportObject) Then
                CType(OnTransport, TransportObject).RemovePassenger(Me)
            End If

            client.Character.charMovementFlags = 0
            client.Character.positionX = posX
            client.Character.positionY = posY
            client.Character.positionZ = posZ
            client.Character.orientation = ori
            client.Character.MapID = map
            client.Character.Save()

            'Do global transfer
            ClsWorldServer.ClientTransfer(client.Index, posX, posY, posZ, ori, map)
        End Sub

        Public Sub ZoneCheck()
            Dim ZoneFlag As Integer = GetAreaFlag(positionX, positionY, MapID)
            If AreaTable.ContainsKey(ZoneFlag) = False Then
                Log.WriteLine(LogType.WARNING, "Zone Flag {0} does not exist.", ZoneFlag)
                Exit Sub
            End If
            AreaID = AreaTable(ZoneFlag).ID
            If AreaTable(ZoneFlag).Zone = 0 Then
                ZoneID = AreaTable(ZoneFlag).ID
            Else
                ZoneID = AreaTable(ZoneFlag).Zone
            End If

            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_ZONE

            'DONE: Set rested in citys
            If AreaTable(ZoneFlag).IsCity Then
                If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_RESTING) = 0 AndAlso Level < DEFAULT_MAX_LEVEL Then
                    cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_RESTING
                    SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)
                    SendCharacterUpdate()
                End If
            Else
                If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_RESTING) Then
                    cPlayerFlags = cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_RESTING)
                    SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)
                    SendCharacterUpdate()
                End If
            End If
            'DONE: Sanctuary turns players into blue and not attackable
            If AreaTable(ZoneFlag).IsSanctuary Then
                If (cUnitFlags And UnitFlags.UNIT_FLAG_NON_PVP_PLAYER) < UnitFlags.UNIT_FLAG_NON_PVP_PLAYER Then
                    cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_NON_PVP_PLAYER
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                    SendCharacterUpdate()
                End If
            Else
                If (cUnitFlags And UnitFlags.UNIT_FLAG_NON_PVP_PLAYER) = UnitFlags.UNIT_FLAG_NON_PVP_PLAYER Then
                    cUnitFlags = cUnitFlags And (Not UnitFlags.UNIT_FLAG_NON_PVP_PLAYER)
                    cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_ATTACKABLE 'To still be able to attack neutral
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                    SendCharacterUpdate()
                End If

                'DONE: Activate Arena PvP (Can attack people from your own faction)
                If AreaTable(ZoneFlag).IsArena Then
                    If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_PVP_TIMER) = 0 Then
                        cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_PVP_TIMER
                        SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)
                        SendCharacterUpdate()

                        GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS
                    End If
                Else
                    'DONE: Activate PvP
                    'TODO: Only for PvP realms
                    If AreaTable(ZoneFlag).IsMyLand(Me) = False Then
                        If (cUnitFlags And UnitFlags.UNIT_FLAG_PVP) = 0 Then
                            cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_PVP
                            SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                            SendCharacterUpdate()

                            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS
                        End If
                    Else
                        'TODO: It takes 5 minutes before the PVP flag wears off
                        If (cUnitFlags And UnitFlags.UNIT_FLAG_PVP) Then
                            cUnitFlags = cUnitFlags And (Not UnitFlags.UNIT_FLAG_PVP)
                            SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                            SendCharacterUpdate()

                            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS
                        End If
                    End If
                End If
            End If
        End Sub

        Public Sub ZoneCheckInstance()
            Dim ZoneFlag As Integer = GetAreaFlag(positionX, positionY, MapID)
            If AreaTable.ContainsKey(ZoneFlag) = False Then
                Log.WriteLine(LogType.WARNING, "Zone Flag {0} does not exist.", ZoneFlag)
                Exit Sub
            End If
            AreaID = AreaTable(ZoneFlag).ID
            If AreaTable(ZoneFlag).Zone = 0 Then
                If ZoneID = 0 Then
                    ZoneID = AreaTable(ZoneFlag).ID
                Else
                    ZoneID = AreaTable(ZoneFlag).Zone
                End If
            End If
            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_ZONE

            'DONE: Set rested in citys
            If AreaTable(ZoneFlag).IsCity Then
                If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_RESTING) = 0 AndAlso Level < DEFAULT_MAX_LEVEL Then
                    cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_RESTING
                    SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)
                    SendCharacterUpdate()
                End If
            Else
                If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_RESTING) Then
                    cPlayerFlags = cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_RESTING)
                    SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)
                    SendCharacterUpdate()
                End If
            End If
            'DONE: Sanctuary turns players into blue and not attackable
            If AreaTable(ZoneFlag).IsSanctuary Then
                If (cUnitFlags And UnitFlags.UNIT_FLAG_NON_PVP_PLAYER) < UnitFlags.UNIT_FLAG_NON_PVP_PLAYER Then
                    cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_NON_PVP_PLAYER
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                    SendCharacterUpdate()
                End If
            Else
                If (cUnitFlags And UnitFlags.UNIT_FLAG_NON_PVP_PLAYER) = UnitFlags.UNIT_FLAG_NON_PVP_PLAYER Then
                    cUnitFlags = cUnitFlags And (Not UnitFlags.UNIT_FLAG_NON_PVP_PLAYER)
                    cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_ATTACKABLE 'To still be able to attack neutral
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                    SendCharacterUpdate()
                End If

                'DONE: Activate Arena PvP (Can attack people from your own faction)
                If AreaTable(ZoneFlag).IsArena Then
                    If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_PVP_TIMER) = 0 Then
                        cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_PVP_TIMER
                        SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, cPlayerFlags)
                        SendCharacterUpdate()

                        GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS
                    End If
                Else
                    'DONE: Activate PvP
                    'TODO: Only for PvP realms
                    If AreaTable(ZoneFlag).IsMyLand(Me) = False Then
                        If (cUnitFlags And UnitFlags.UNIT_FLAG_PVP) = 0 Then
                            cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_PVP
                            SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                            SendCharacterUpdate()

                            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS
                        End If
                    Else
                        'TODO: It takes 5 minutes before the PVP flag wears off
                        If (cUnitFlags And UnitFlags.UNIT_FLAG_PVP) Then
                            cUnitFlags = cUnitFlags And (Not UnitFlags.UNIT_FLAG_PVP)
                            SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                            SendCharacterUpdate()

                            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS
                        End If
                    End If
                End If
            End If
        End Sub

        'Public Sub ChangeSpeed(ByVal Type As ChangeSpeedType, ByVal NewSpeed As Single)
        '    Dim packet As PacketClass = Nothing
        '    Try
        '        Select Case Type
        '            Case ChangeSpeedType.RUN
        '                RunSpeed = NewSpeed
        '                packet = New PacketClass(OPCODES.MSG_MOVE_SET_RUN_SPEED)
        '            Case ChangeSpeedType.RUNBACK
        '                RunBackSpeed = NewSpeed
        '                packet = New PacketClass(OPCODES.MSG_MOVE_SET_RUN_BACK_SPEED)
        '            Case ChangeSpeedType.SWIM
        '                SwimSpeed = NewSpeed
        '                packet = New PacketClass(OPCODES.MSG_MOVE_SET_SWIM_SPEED)
        '            Case ChangeSpeedType.SWIMBACK
        '                SwimSpeed = NewSpeed
        '                packet = New PacketClass(OPCODES.MSG_MOVE_SET_SWIM_BACK_SPEED)
        '            Case ChangeSpeedType.TURNRATE
        '                TurnRate = NewSpeed
        '                packet = New PacketClass(OPCODES.MSG_MOVE_SET_TURN_RATE)
        '        End Select

        '        'DONE: Send to nearby players
        '        packet.AddPackGUID(Client.Character.GUID)
        '        packet.AddInt32(0) 'Movement flags
        '        packet.AddInt32(msTime)
        '        packet.AddSingle(positionX)
        '        packet.AddSingle(positionY)
        '        packet.AddSingle(positionZ)
        '        packet.AddSingle(orientation)
        '        packet.AddInt32(0) 'Unk flag
        '        packet.AddSingle(NewSpeed)
        '        client.Character.SendToNearPlayers(packet)
        '    Finally
        '        packet.Dispose()
        '    End Try
        '        End Sub

        Public Sub ChangeSpeedForced(ByVal Type As ChangeSpeedType, ByVal NewSpeed As Single)
            antiHackSpeedChanged_ += 1
            Dim packet As PacketClass = Nothing
            Try
                Select Case Type
                    Case ChangeSpeedType.RUN
                        packet = New PacketClass(OPCODES.SMSG_FORCE_RUN_SPEED_CHANGE)
                        RunSpeed = NewSpeed
                    Case ChangeSpeedType.RUNBACK
                        packet = New PacketClass(OPCODES.SMSG_FORCE_RUN_BACK_SPEED_CHANGE)
                        RunBackSpeed = NewSpeed
                    Case ChangeSpeedType.SWIM
                        packet = New PacketClass(OPCODES.SMSG_FORCE_SWIM_SPEED_CHANGE)
                        SwimSpeed = NewSpeed
                    Case ChangeSpeedType.SWIMBACK
                        packet = New PacketClass(OPCODES.SMSG_FORCE_SWIM_BACK_SPEED_CHANGE)
                        SwimBackSpeed = NewSpeed
                    Case ChangeSpeedType.TURNRATE
                        packet = New PacketClass(OPCODES.SMSG_FORCE_TURN_RATE_CHANGE)
                        TurnRate = NewSpeed
                    Case Else
                        Exit Sub
                End Select
                packet.AddPackGUID(GUID)
                packet.AddInt32(antiHackSpeedChanged_)
                packet.AddSingle(NewSpeed)
                client.Character.SendToNearPlayers(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub SetWaterWalk()
            Dim SMSG_MOVE_WATER_WALK As New PacketClass(OPCODES.SMSG_MOVE_WATER_WALK)
            Try
                SMSG_MOVE_WATER_WALK.AddPackGUID(GUID)
                SMSG_MOVE_WATER_WALK.AddInt32(0)
                SendToNearPlayers(SMSG_MOVE_WATER_WALK)
            Finally
                SMSG_MOVE_WATER_WALK.Dispose()
            End Try
        End Sub

        Public Sub SetLandWalk()
            Dim SMSG_MOVE_LAND_WALK As New PacketClass(OPCODES.SMSG_MOVE_LAND_WALK)
            Try
                SMSG_MOVE_LAND_WALK.AddPackGUID(GUID)
                SMSG_MOVE_LAND_WALK.AddInt32(0)
                SendToNearPlayers(SMSG_MOVE_LAND_WALK)
            Finally
                SMSG_MOVE_LAND_WALK.Dispose()
            End Try
        End Sub

        Public Sub SetMoveRoot()
            Dim SMSG_FORCE_MOVE_ROOT As New PacketClass(OPCODES.SMSG_FORCE_MOVE_ROOT)
            Try
                SMSG_FORCE_MOVE_ROOT.AddPackGUID(GUID)
                SMSG_FORCE_MOVE_ROOT.AddInt32(0)
                client.Send(SMSG_FORCE_MOVE_ROOT)
            Finally
                SMSG_FORCE_MOVE_ROOT.Dispose()
            End Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FORCE_MOVE_ROOT", client.IP, client.Port)
        End Sub

        Public Sub SetMoveUnroot()
            Dim SMSG_FORCE_MOVE_UNROOT As New PacketClass(OPCODES.SMSG_FORCE_MOVE_UNROOT)
            Try
                SMSG_FORCE_MOVE_UNROOT.AddPackGUID(GUID)
                SMSG_FORCE_MOVE_UNROOT.AddInt32(0)
                SendToNearPlayers(SMSG_FORCE_MOVE_UNROOT)
            Finally
                SMSG_FORCE_MOVE_UNROOT.Dispose()
            End Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FORCE_MOVE_UNROOT", client.IP, client.Port)
        End Sub

        Public Sub StartMirrorTimer(ByVal Type As MirrorTimer, ByVal MaxValue As Integer)
            Dim SMSG_START_MIRROR_TIMER As New PacketClass(OPCODES.SMSG_START_MIRROR_TIMER)
            Try
                SMSG_START_MIRROR_TIMER.AddInt32(Type)
                SMSG_START_MIRROR_TIMER.AddInt32(MaxValue)
                SMSG_START_MIRROR_TIMER.AddInt32(MaxValue)
                SMSG_START_MIRROR_TIMER.AddInt32(-1)
                SMSG_START_MIRROR_TIMER.AddInt32(0)
                SMSG_START_MIRROR_TIMER.AddInt8(0)

                client.Send(SMSG_START_MIRROR_TIMER)
            Finally
                SMSG_START_MIRROR_TIMER.Dispose()
            End Try
        End Sub

        Public Sub ModifyMirrorTimer(ByVal Type As MirrorTimer, ByVal MaxValue As Integer, ByVal CurrentValue As Integer, ByVal Regen As Integer)
            'TYPE: 0 = fartigua 1 = breath 2 = fire
            Dim SMSG_START_MIRROR_TIMER As New PacketClass(OPCODES.SMSG_START_MIRROR_TIMER)
            Try
                SMSG_START_MIRROR_TIMER.AddInt32(Type)
                SMSG_START_MIRROR_TIMER.AddInt32(CurrentValue)
                SMSG_START_MIRROR_TIMER.AddInt32(MaxValue)
                SMSG_START_MIRROR_TIMER.AddInt32(Regen)
                SMSG_START_MIRROR_TIMER.AddInt32(0)
                SMSG_START_MIRROR_TIMER.AddInt8(0)

                client.Send(SMSG_START_MIRROR_TIMER)
            Finally
                SMSG_START_MIRROR_TIMER.Dispose()
            End Try
        End Sub

        Public Sub StopMirrorTimer(ByVal Type As MirrorTimer)
            Dim SMSG_STOP_MIRROR_TIMER As New PacketClass(OPCODES.SMSG_STOP_MIRROR_TIMER)
            Try
                SMSG_STOP_MIRROR_TIMER.AddInt32(Type)

                client.Send(SMSG_STOP_MIRROR_TIMER)
            Finally
                SMSG_STOP_MIRROR_TIMER.Dispose()
            End Try
            'If Type = 1 And (Not (underWaterTimer Is Nothing)) Then
            '    underWaterTimer.Dispose()
            '    underWaterTimer = Nothing
            'End If
        End Sub

        Public Sub HandleDrowning(ByVal state As Object)
            Try
                If positionZ > (GetWaterLevel(positionX, positionY, MapID) - 1.6) Then
                    underWaterTimer.DrowningValue += 2000
                    If underWaterTimer.DrowningValue > 70000 Then underWaterTimer.DrowningValue = 70000
                    ModifyMirrorTimer(MirrorTimer.DROWNING, 70000, underWaterTimer.DrowningValue, 2)
                Else
                    underWaterTimer.DrowningValue -= 1000
                    If underWaterTimer.DrowningValue < 0 Then
                        underWaterTimer.DrowningValue = 0
                        LogEnvironmentalDamage(EnvironmentalDamage.DAMAGE_DROWNING, Fix(0.1F * Life.Maximum * underWaterTimer.DrowningDamage))
                        DealDamage(Fix(0.1F * Life.Maximum * underWaterTimer.DrowningDamage))
                        underWaterTimer.DrowningDamage = underWaterTimer.DrowningDamage * 2
                        If DEAD Then
                            underWaterTimer.Dispose()
                            underWaterTimer = Nothing
                        End If
                    End If
                    ModifyMirrorTimer(MirrorTimer.DROWNING, 70000, underWaterTimer.DrowningValue, -1)
                End If
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error at HandleDrowning():", e.ToString)
                If underWaterTimer IsNot Nothing Then underWaterTimer.Dispose()
                underWaterTimer = Nothing
            End Try
        End Sub

        'Reputation
        Public WatchedFactionIndex As Byte = &HFF
        Public Reputation(63) As TReputation
        Public Sub InitializeReputation(ByVal FactionID As Integer)
            If FactionInfo(FactionID).VisibleID > -1 Then
                Reputation(FactionInfo(FactionID).VisibleID).Value = 0
                If Reputation(FactionInfo(FactionID).VisibleID).Flags = 0 Then
                    Reputation(FactionInfo(FactionID).VisibleID).Flags = 1
                End If
            End If
        End Sub

        Public Function GetReaction(ByVal FactionID As Integer) As TReaction
            If FactionTemplatesInfo.ContainsKey(FactionID) = False OrElse FactionTemplatesInfo.ContainsKey(Faction) = False Then Return TReaction.NEUTRAL

            'DONE: Neutral to everyone
            If FactionTemplatesInfo(FactionID).enemyMask = 0 AndAlso FactionTemplatesInfo(FactionID).friendMask = 0 AndAlso
            FactionTemplatesInfo(FactionID).enemyFaction1 = 0 And FactionTemplatesInfo(FactionID).enemyFaction2 = 0 AndAlso
            FactionTemplatesInfo(FactionID).enemyFaction3 = 0 AndAlso FactionTemplatesInfo(FactionID).enemyFaction4 = 0 Then Return TReaction.NEUTRAL

            'DONE: Neutral to your faction
            If FactionTemplatesInfo(FactionID).enemyMask = 0 AndAlso FactionTemplatesInfo(FactionID).friendMask = 0 AndAlso
            FactionTemplatesInfo(FactionID).enemyFaction1 <> Faction And FactionTemplatesInfo(FactionID).enemyFaction2 <> Faction AndAlso
            FactionTemplatesInfo(FactionID).enemyFaction3 <> Faction AndAlso FactionTemplatesInfo(FactionID).enemyFaction4 <> Faction Then Return TReaction.NEUTRAL

            'DONE: Hostile to any players
            If FactionTemplatesInfo(FactionID).enemyMask And FactionMasks.FACTION_MASK_PLAYER Then Return TReaction.HOSTILE

            'DONE: Friendly to your faction
            If FactionTemplatesInfo(FactionID).friendFaction1 = Faction OrElse FactionTemplatesInfo(FactionID).friendFaction2 = Faction OrElse
            FactionTemplatesInfo(FactionID).friendFaction3 = Faction OrElse FactionTemplatesInfo(FactionID).friendFaction4 = Faction Then Return TReaction.FIGHT_SUPPORT

            'DONE: Friendly to your faction mask
            If FactionTemplatesInfo(FactionID).friendMask And FactionTemplatesInfo(Faction).ourMask Then Return TReaction.FIGHT_SUPPORT

            'DONE: Hostile to your faction
            If FactionTemplatesInfo(FactionID).enemyFaction1 = Faction OrElse FactionTemplatesInfo(FactionID).enemyFaction2 = Faction OrElse
            FactionTemplatesInfo(FactionID).enemyFaction3 = Faction OrElse FactionTemplatesInfo(FactionID).enemyFaction4 = Faction Then Return TReaction.HOSTILE

            'DONE: Hostile to your faction mask
            If FactionTemplatesInfo(FactionID).enemyMask And FactionTemplatesInfo(Faction).ourMask Then Return TReaction.HOSTILE

            'DONE: Hostile by reputation
            Dim Rank As ReputationRank = GetReputation(FactionTemplatesInfo(FactionID).FactionID)
            If Rank <= ReputationRank.Hostile Then
                Return TReaction.HOSTILE
            ElseIf Rank >= ReputationRank.Revered Then
                Return TReaction.FIGHT_SUPPORT
            ElseIf Rank >= ReputationRank.Friendly Then
                Return TReaction.FRIENDLY
            Else
                Return TReaction.NEUTRAL
            End If
        End Function

        Public Function GetReputationValue(ByVal FactionTemplateID As Integer) As Integer
            If Not FactionTemplatesInfo.ContainsKey(FactionTemplateID) Then Return ReputationRank.Neutral
            Dim FactionID As Integer = FactionTemplatesInfo(FactionTemplateID).FactionID
            If Not FactionInfo.ContainsKey(FactionID) Then Return ReputationRank.Neutral
            If FactionInfo(FactionID).VisibleID = -1 Then Return ReputationRank.Neutral

            Dim points As Integer
            If HaveFlag(FactionInfo(FactionID).flags(0), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(0)
            ElseIf HaveFlag(FactionInfo(FactionID).flags(1), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(1)
            ElseIf HaveFlag(FactionInfo(FactionID).flags(2), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(2)
            ElseIf HaveFlag(FactionInfo(FactionID).flags(3), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(3)
            Else
                points = 0
            End If

            If Reputation(FactionInfo(FactionID).VisibleID).Flags > 0 Then
                points = points + Reputation(FactionInfo(FactionID).VisibleID).Value
            End If
            Return points
        End Function

        Public Function GetReputation(ByVal FactionTemplateID As Integer) As ReputationRank
            If Not FactionTemplatesInfo.ContainsKey(FactionTemplateID) Then Return ReputationRank.Neutral
            Dim FactionID As Integer = FactionTemplatesInfo(FactionTemplateID).FactionID
            If Not FactionInfo.ContainsKey(FactionID) Then Return ReputationRank.Neutral
            If FactionInfo(FactionID).VisibleID = -1 Then Return ReputationRank.Neutral

            Dim points As Integer
            If HaveFlag(FactionInfo(FactionID).flags(0), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(0)
            ElseIf HaveFlag(FactionInfo(FactionID).flags(1), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(1)
            ElseIf HaveFlag(FactionInfo(FactionID).flags(2), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(2)
            ElseIf HaveFlag(FactionInfo(FactionID).flags(3), Race - 1) Then
                points = FactionInfo(FactionID).rep_stats(3)
            Else
                points = 0
            End If

            If Reputation(FactionInfo(FactionID).VisibleID).Flags > 0 Then
                points = points + Reputation(FactionInfo(FactionID).VisibleID).Value
            End If

            Select Case points
                Case Is > ReputationPoints.Revered
                    Return ReputationRank.Exalted
                Case Is > ReputationPoints.Honored
                    Return ReputationRank.Revered
                Case Is > ReputationPoints.Friendly
                    Return ReputationRank.Honored
                Case Is > ReputationPoints.Neutral
                    Return ReputationRank.Friendly
                Case Is > ReputationPoints.Unfriendly
                    Return ReputationRank.Neutral
                Case Is > ReputationPoints.Hostile
                    Return ReputationRank.Unfriendly
                Case Is > ReputationPoints.Hated
                    Return ReputationRank.Hostile
                Case Else
                    Return ReputationRank.Hated
            End Select
        End Function

        Public Sub SetReputation(ByVal FactionID As Integer, ByVal Value As Integer)
            If FactionInfo(FactionID).VisibleID = -1 Then Exit Sub

            Reputation(FactionInfo(FactionID).VisibleID).Value += Value

            If (Reputation(FactionInfo(FactionID).VisibleID).Flags And 1) = 0 Then
                Reputation(FactionInfo(FactionID).VisibleID).Flags = Reputation(FactionInfo(FactionID).VisibleID).Flags Or 1
            End If

            If Not client Is Nothing Then
                Dim packet As New PacketClass(OPCODES.SMSG_SET_FACTION_STANDING)
                Try
                    packet.AddInt32(Reputation(FactionInfo(FactionID).VisibleID).Flags)
                    packet.AddInt32(FactionInfo(FactionID).VisibleID)
                    packet.AddInt32(Reputation(FactionInfo(FactionID).VisibleID).Value)
                    client.Send(packet)
                Finally
                    packet.Dispose()
                End Try
            End If
        End Sub

        Public Function GetDiscountMod(ByVal FactionID As Integer) As Single
            Dim Rank As ReputationRank = GetReputation(FactionID)
            If Rank >= ReputationRank.Honored Then Return 0.9F
            Return 1.0F
        End Function

        'Death
        Public Overrides Sub Die(ByRef Attacker As BaseUnit)
            'NOTE: Do this first to prevent problems
            DEAD = True
            corpseGUID = 0UL

            If Attacker IsNot Nothing AndAlso TypeOf Attacker Is CreatureObject Then
                If CType(Attacker, CreatureObject).aiScript IsNot Nothing Then CType(Attacker, CreatureObject).aiScript.OnKill(Me)
            End If

            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS

            For Each uGuid As ULong In inCombatWith
                If GuidIsPlayer(uGuid) AndAlso CHARACTERs.ContainsKey(uGuid) Then
                    'DONE: Remove combat from players who had you in combat
                    CHARACTERs(uGuid).RemoveFromCombat(Me)
                End If
            Next
            inCombatWith.Clear()

            'DONE: Check if player is in duel
            If IsInDuel Then
                DEAD = False
                DuelComplete(DuelPartner, Me)
                Exit Sub
            End If

            'DONE: Remove all spells when you die
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing Then
                    RemoveAura(i, ActiveSpells(i).SpellCaster, , False)
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_AURA + i, 0)
                End If
            Next

            'DONE: Save as DEAD (GHOST)!
            repopTimer = New TRepopTimer(Me)
            cDynamicFlags = DynamicFlags.UNIT_DYNFLAG_DEAD
            cUnitFlags = 8          'player death animation, also can be used with cDynamicFlags

            SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, 0)
            SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
            SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, cDynamicFlags)
            SendCharacterUpdate(True)

            'DONE: 10% Durability lost, and only if the killer is a creature or you died by enviromental damage
            If Attacker Is Nothing OrElse TypeOf Attacker Is CreatureObject Then
                For i As Byte = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                    If Items.ContainsKey(i) Then Items(i).ModifyDurability(0.1F, client)
                Next
                Dim SMSG_DURABILITY_DAMAGE_DEATH As New PacketClass(OPCODES.SMSG_DURABILITY_DAMAGE_DEATH)
                Try
                    client.Send(SMSG_DURABILITY_DAMAGE_DEATH)
                Finally
                    SMSG_DURABILITY_DAMAGE_DEATH.Dispose()
                End Try
            End If

            'DONE: Save the character
            Save()
        End Sub

        Public Sub SendDeathReleaseLoc(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal MapID As Integer)
            'Show spirit healer position on minimap
            Dim p As New PacketClass(OPCODES.CMSG_REPOP_REQUEST)
            Try
                p.AddInt32(MapID)
                p.AddSingle(x)
                p.AddSingle(y)
                p.AddSingle(z)
                client.Send(p)
            Finally
                p.Dispose()
            End Try
        End Sub

        'Combat
        Public Overrides Sub DealDamage(ByVal Damage As Integer, Optional ByRef Attacker As BaseUnit = Nothing)
            'DONE: Check for dead
            If DEAD Then Exit Sub

            'DONE: Break some spells when taking any damage
            RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_DAMAGE)

            If spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL) IsNot Nothing Then
                With spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL)
                    If .Finished = False Then
                        If (.SpellInfo.interruptFlags And SpellInterruptFlags.SPELL_INTERRUPT_FLAG_DAMAGE) Then
                            FinishAllSpells()
                        ElseIf (.SpellInfo.interruptFlags And SpellInterruptFlags.SPELL_INTERRUPT_FLAG_PUSH_BACK) Then
                            .Delay()
                        End If
                    End If
                End With
            End If

            If Attacker IsNot Nothing Then
                'DONE: Add into combat if not already
                If Not inCombatWith.Contains(Attacker.GUID) Then
                    inCombatWith.Add(Attacker.GUID)
                    CheckCombat()
                    SendCharacterUpdate()
                End If

                'DONE: Add the attacker into combat if not already
                If TypeOf Attacker Is CharacterObject AndAlso CType(Attacker, CharacterObject).inCombatWith.Contains(GUID) = False Then
                    CType(Attacker, CharacterObject).inCombatWith.Add(GUID)
                    If (CType(Attacker, CharacterObject).cUnitFlags And UnitFlags.UNIT_FLAG_IN_COMBAT) = 0 Then
                        CType(Attacker, CharacterObject).cUnitFlags = CType(Attacker, CharacterObject).cUnitFlags Or UnitFlags.UNIT_FLAG_IN_COMBAT
                        CType(Attacker, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, CType(Attacker, CharacterObject).cUnitFlags)
                        CType(Attacker, CharacterObject).SendCharacterUpdate()
                    End If
                End If

                'DONE: Fight support by NPCs
                For Each cGUID As ULong In creaturesNear.ToArray
                    If WORLD_CREATUREs.ContainsKey(cGUID) AndAlso WORLD_CREATUREs(cGUID).aiScript IsNot Nothing AndAlso WORLD_CREATUREs(cGUID).isGuard Then
                        If WORLD_CREATUREs(cGUID).IsDead = False AndAlso WORLD_CREATUREs(cGUID).aiScript.InCombat() = False Then
                            If inCombatWith.Contains(cGUID) Then Continue For
                            If GetReaction(WORLD_CREATUREs(cGUID).Faction) = TReaction.FIGHT_SUPPORT AndAlso GetDistance(WORLD_CREATUREs(cGUID), Me) <= WORLD_CREATUREs(cGUID).AggroRange(Me) Then
                                WORLD_CREATUREs(cGUID).aiScript.OnGenerateHate(Attacker, Damage)
                            End If
                        End If
                    End If
                Next
            End If

            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_HP
            If Not Invulnerable Then Life.Current -= Damage

            If Life.Current = 0 Then
                Die(Attacker)
                Exit Sub
            Else
                SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
                SendCharacterUpdate()
            End If

            'TODO: Need a better generation for Range
            'http://www.wowwiki.com/Formulas:Rage_generation
            If Classe = Classes.CLASS_WARRIOR OrElse (Classe = Classes.CLASS_DRUID AndAlso (ShapeshiftForm = ShapeshiftForm.FORM_BEAR OrElse ShapeshiftForm = ShapeshiftForm.FORM_DIREBEAR)) Then
                Rage.Increment(Fix(2.5 * Damage / GetRageConversion))
                SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + ManaTypes.TYPE_RAGE, Rage.Current)
                SendCharacterUpdate(True)
            End If
        End Sub

        Public Overrides Sub Heal(ByVal Damage As Integer, Optional ByRef Attacker As BaseUnit = Nothing)
            If DEAD Then Exit Sub

            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_HP

            'TODO: Healing generates thread on the NPCs that has this character in their combat array

            Life.Current += Damage
            SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
            SendCharacterUpdate()
        End Sub

        Public Overrides Sub Energize(ByVal Damage As Integer, ByVal Power As ManaTypes, Optional ByRef Attacker As BaseUnit = Nothing)
            If DEAD Then Exit Sub

            GroupUpdateFlag = GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_POWER

            Select Case Power
                Case ManaTypes.TYPE_MANA
                    If Mana.Current = Mana.Maximum Then Exit Sub
                    Mana.Current += Damage
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, Mana.Current)
                Case ManaTypes.TYPE_RAGE
                    If Rage.Current = Rage.Maximum Then Exit Sub
                    Rage.Current += Damage
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER2, Rage.Current)
                Case ManaTypes.TYPE_ENERGY
                    If Energy.Current = Energy.Maximum Then Exit Sub
                    Energy.Current += Damage
                    SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER4, Energy.Current)
                Case Else
                    Exit Sub
            End Select
            SendCharacterUpdate()
        End Sub

        'System
        Public Sub Logout(Optional ByVal StateObj As Object = Nothing)
            Try
                LogoutTimer.Dispose()
                LogoutTimer = Nothing
            Catch
            End Try

            'DONE: Spawn corpse and remove repop timer if present
            If repopTimer IsNot Nothing Then
                repopTimer.Dispose()
                repopTimer = Nothing
                'DONE: Spawn Corpse
                Dim myCorpse As New CorpseObject(Me)
                myCorpse.AddToWorld()
                myCorpse.Save()
            End If

            'DONE: Leave local group
            If IsInGroup Then
                Group.LocalMembers.Remove(GUID)
                If Group.LocalMembers.Count = 0 Then
                    Group.Dispose()
                    Group = Nothing
                End If
            End If

            'DONE: Leave transports
            If (OnTransport IsNot Nothing) AndAlso (TypeOf OnTransport Is TransportObject) Then
                CType(OnTransport, TransportObject).RemovePassenger(Me)
            End If

            'DONE: Cancel duels
            If DuelPartner IsNot Nothing Then
                If DuelPartner.DuelArbiter = DuelArbiter Then
                    DuelComplete(DuelPartner, Me)
                ElseIf WORLD_GAMEOBJECTs.ContainsKey(DuelArbiter) Then
                    WORLD_GAMEOBJECTs(DuelArbiter).Destroy(WORLD_GAMEOBJECTs(DuelArbiter))
                End If
            End If

            'DONE: Disconnect the client
            Dim SMSG_LOGOUT_COMPLETE As New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE)
            Try
                client.Send(SMSG_LOGOUT_COMPLETE)
                SMSG_LOGOUT_COMPLETE.Dispose()
                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_LOGOUT_COMPLETE", client.IP, client.Port)

                client.Character = Nothing
                Log.WriteLine(LogType.USER, "Character {0} logged off.", Name)

                client.Delete()
                client = Nothing
            Finally
                Dispose()
            End Try
        End Sub

        Public Sub Login()
            'DONE: Setting instance ID
            InstanceMapEnter(Me)

            'Set player to transport
            SetOnTransport()

            'DONE: If we have changed map
            If MapID <> LoginMap Then
                Log.WriteLine(LogType.DEBUG, "Spawned on wrong map [{0}], transferring to [{1}].", LoginMap, MapID)
                Transfer(positionX, positionY, positionZ, orientation, MapID)
                Exit Sub
            End If

            'Loading map cell if not loaded
            GetMapTile(positionX, positionY, CellX, CellY)
            Try
                If Maps(MapID).Tiles(CellX, CellY) Is Nothing Then MAP_Load(CellX, CellY, MapID)
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "Failed loading maps at character logging in.{0}{1}", Environment.NewLine, ex.ToString())
            End Try

            'DONE: SMSG_BINDPOINTUPDATE
            SendBindPointUpdate(client, Me)

            'TODO: SMSG_SET_REST_START
            Send_SMSG_SET_REST_START(client, Me)

            'DONE: SMSG_TUTORIAL_FLAGS
            SendTutorialFlags(client, Me)

            'DONE: SMSG_SET_PROFICIENCY
            SendProficiencies()

            'TODO: SMSG_UPDATE_AURA_DURATION

            'DONE: SMSG_INITIAL_SPELLS
            SendInitialSpells(client, Me)
            'DONE: SMSG_INITIALIZE_FACTIONS
            SendFactions(client, Me)
            'DONE: SMSG_ACTION_BUTTONS
            SendActionButtons(client, Me)
            'DONE: SMSG_INIT_WORLD_STATES
            SendInitWorldStates(client, Me)

            'DONE: SMSG_UPDATE_OBJECT for ourself
            Life.Current = Life.Maximum
            Mana.Current = Mana.Maximum
            FillAllUpdateFlags()
            SendUpdate()

            'DONE: Adding to World
            AddToWorld(Me)

            'DONE: Enable client moving
            SendTimeSyncReq(client)

            'DONE: Send update on aura durations
            UpdateAuraDurations()

            FullyLoggedIn = True
            UpdateManaRegen()
        End Sub

        Public Sub UpdateAuraDurations()
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing Then
                    Dim SMSG_UPDATE_AURA_DURATION As New PacketClass(OPCODES.SMSG_UPDATE_AURA_DURATION)
                    Try
                        SMSG_UPDATE_AURA_DURATION.AddInt8(i)
                        SMSG_UPDATE_AURA_DURATION.AddInt32(ActiveSpells(i).SpellDuration)
                        client.Send(SMSG_UPDATE_AURA_DURATION)
                    Finally
                        SMSG_UPDATE_AURA_DURATION.Dispose()
                    End Try
                End If
            Next i
        End Sub

        Public Sub SetOnTransport()
            If LoginTransport = 0UL Then Exit Sub

            Log.WriteLine(LogType.DEBUG, "Spawning on transport.")
            Dim TransportGUID As ULong = LoginTransport
            LoginTransport = 0UL
            If TransportGUID > 0 Then
                If GuidIsMoTransport(TransportGUID) AndAlso WORLD_TRANSPORTs.ContainsKey(TransportGUID) Then
                    OnTransport = WORLD_TRANSPORTs(TransportGUID)
                    WORLD_TRANSPORTs(TransportGUID).AddPassenger(Me)
                    transportX = positionX
                    transportY = positionY
                    transportZ = positionZ
                    positionX = OnTransport.positionX
                    positionY = OnTransport.positionY
                    positionZ = OnTransport.positionZ
                    MapID = OnTransport.MapID
                ElseIf GuidIsTransport(TransportGUID) Then
                    If WORLD_GAMEOBJECTs.ContainsKey(TransportGUID) Then
                        OnTransport = WORLD_GAMEOBJECTs(TransportGUID)
                        transportX = positionX
                        transportY = positionY
                        transportZ = positionZ
                        positionX = OnTransport.positionX
                        positionY = OnTransport.positionY
                        positionZ = OnTransport.positionZ
                    Else
                        Log.WriteLine(LogType.CRITICAL, "Spawning new transport!")
                        Dim newGameobject As New GameObjectObject(TransportGUID - GUID_TRANSPORT)
                        newGameobject.AddToWorld()
                        OnTransport = newGameobject
                        transportX = positionX
                        transportY = positionY
                        transportZ = positionZ
                        positionX = OnTransport.positionX
                        positionY = OnTransport.positionY
                        positionZ = OnTransport.positionZ
                    End If
                Else
                    Log.WriteLine(LogType.CRITICAL, "Character logging in on non-existant transport [{0}].", TransportGUID - GUID_MO_TRANSPORT)
                    AllGraveYards.GoToNearestGraveyard(Me, True, False)
                    OnTransport = Nothing
                End If
            Else
                OnTransport = Nothing
            End If
        End Sub

        Public Sub SendProficiencies()
            Dim ProficiencyFlags As Integer = 0
            If HaveSpell(9125) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_MISC) 'Here using spell "Generic"
            If HaveSpell(9078) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_CLOTH)
            If HaveSpell(9077) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_LEATHER)
            If HaveSpell(8737) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_MAIL)
            If HaveSpell(750) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_PLATE)
            If HaveSpell(9124) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_BUCKLER)
            If HaveSpell(9116) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_SHIELD)
            If HaveSpell(27762) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_LIBRAM)
            If HaveSpell(27763) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_TOTEM)
            If HaveSpell(27764) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_IDOL)
            SendProficiency(client, ITEM_CLASS.ITEM_CLASS_ARMOR, ProficiencyFlags)

            ProficiencyFlags = 0
            If HaveSpell(196) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_AXE)
            If HaveSpell(197) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_AXE)
            If HaveSpell(264) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_BOW)
            If HaveSpell(266) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_GUN)
            If HaveSpell(198) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_MACE)
            If HaveSpell(199) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_MACE)
            If HaveSpell(200) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_POLEARM)
            If HaveSpell(201) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_SWORD)
            If HaveSpell(202) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_SWORD)
            'If Spells.Contains() Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_WEAPON_obsolete)
            If HaveSpell(227) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_STAFF)
            If HaveSpell(262) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_WEAPON_EXOTIC)
            If HaveSpell(263) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_WEAPON_EXOTIC2)
            If HaveSpell(15590) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_FIST_WEAPON)
            If HaveSpell(2382) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_MISC_WEAPON) 'Here using spell "Generic"
            If HaveSpell(1180) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_DAGGER)
            If HaveSpell(2567) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_THROWN)
            If HaveSpell(3386) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_SPEAR)
            If HaveSpell(5011) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW)
            If HaveSpell(5009) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_WAND)
            If HaveSpell(7738) Then ProficiencyFlags += (1 << ITEM_SUBCLASS.ITEM_SUBCLASS_FISHING_POLE)
            SendProficiency(client, ITEM_CLASS.ITEM_CLASS_WEAPON, ProficiencyFlags)
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                'WARNING: Do not save character here!!!

                'DONE: Remove buyback items when logged out
                CharacterDatabase.Update(String.Format("DELETE FROM characters_inventory WHERE item_bag = {0} AND item_slot >= {1} AND item_slot <= {2}", GUID, 69, 80 - 1))

                If Not underWaterTimer Is Nothing Then underWaterTimer.Dispose()

                'DONE: Spawn corpse and remove repop timer if present
                If repopTimer IsNot Nothing Then
                    repopTimer.Dispose()
                    repopTimer = Nothing
                    'DONE: Spawn Corpse
                    Dim myCorpse As New CorpseObject(Me)
                    myCorpse.Save()
                    myCorpse.AddToWorld()
                End If

                'DONE: Remove non-combat pets
                If NonCombatPet IsNot Nothing Then NonCombatPet.Destroy()

                'DONE: Leave local group
                If IsInGroup Then
                    Group.LocalMembers.Remove(GUID)
                    If Group.LocalMembers.Count = 0 Then
                        Group.Dispose()
                        Group = Nothing
                    End If
                End If

                CHARACTERs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                CHARACTERs.Remove(GUID)
                CHARACTERs_Lock.ReleaseWriterLock()

                If FullyLoggedIn Then RemoveFromWorld(Me)

                Log.WriteLine(LogType.USER, "Character {0} disposed.", Name)

                For Each Item As KeyValuePair(Of Byte, ItemObject) In Items
                    'DONE: Dispose items in bags (done in Item.Dispose)
                    Item.Value.Dispose()
                Next

                attackState.Dispose()

                If Not client Is Nothing Then client.Character = Nothing
                If Not LogoutTimer Is Nothing Then LogoutTimer.Dispose()
                LogoutTimer = Nothing

                GC.Collect()
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

        Public Sub Initialize()
            CanSeeInvisibility_Stealth = 0
            CanSeeInvisibility_Invisibility = 0
            Model_Native = Model

            If CreatureModel.ContainsKey(Model) Then
                BoundingRadius = CreatureModel(Model).BoundingRadius
                CombatReach = CreatureModel(Model).CombatReach
            End If

            'If Classe = Classes.CLASS_WARRIOR Then Me.ShapeshiftForm = WS_Spells.ShapeshiftForm.FORM_BATTLESTANCE
            If Classe = Classes.CLASS_WARRIOR Then ApplySpell(2457)

            Resistances(DamageTypes.DMG_PHYSICAL).Base += Agility.Base * 2
            Damage.Type = DamageTypes.DMG_PHYSICAL
            Damage.Minimum += 1
            RangedDamage.Type = DamageTypes.DMG_PHYSICAL
            RangedDamage.Minimum += 1
            'TODO: Calculate base dodge, parry, block

            If Access >= AccessLevel.GameMaster Then GM = True

            'DONE: Set ammo automatically
            If Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) AndAlso Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_WEAPON AndAlso
                (Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_BOW OrElse
                 Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW OrElse
                 Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_GUN) Then

                Dim AmmoType As ITEM_SUBCLASS = ITEM_SUBCLASS.ITEM_SUBCLASS_ARROW
                If Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_GUN Then AmmoType = ITEM_SUBCLASS.ITEM_SUBCLASS_BULLET

                For i As Byte = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                    If Items.ContainsKey(i) AndAlso Items(i).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_QUIVER Then
                        For Each slot As KeyValuePair(Of Byte, ItemObject) In Items(i).Items
                            If slot.Value.ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_PROJECTILE AndAlso slot.Value.ItemInfo.SubClass = AmmoType Then
                                Dim CanUse As InventoryChangeFailure = CanUseAmmo(Me, slot.Value.ItemEntry)
                                If CanUse = InventoryChangeFailure.EQUIP_ERR_OK Then
                                    AmmoID = slot.Value.ItemEntry
                                    AmmoDPS = ITEMDatabase(AmmoID).Damage(0).Minimum
                                    CalculateMinMaxDamage(Me, WeaponAttackType.RANGED_ATTACK)

                                    GoTo DoneAmmo
                                End If
                            End If
                        Next
                    End If
                Next
DoneAmmo:
            End If
        End Sub

        Public Sub New()
            MyBase.New()
            Level = 1
            UpdateMask.SetAll(False)

            For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                spellDamage(i) = New TDamageBonus
                Resistances(i) = New TStat
            Next

        End Sub

        Public Sub New(ByRef ClientVal As ClientClass, ByVal GuidVal As ULong)
            'DONE: Add space for passive auras
            ReDim ActiveSpells(MAX_AURA_EFFECTs - 1)

            'DONE: Initialize Defaults
            client = ClientVal
            GUID = GuidVal
            client.Character = Me

            For i As Integer = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                spellDamage(i) = New TDamageBonus
                Resistances(i) = New TStat
            Next

            'DONE: Get character info from DB
            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query(String.Format("SELECT * FROM characters WHERE char_guid = {0}; UPDATE characters SET char_online = 1 WHERE char_guid = {0};", GUID), MySQLQuery)
            If MySQLQuery.Rows.Count = 0 Then
                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] Unable to get SQLDataBase info for character [GUID={2:X}]", client.IP, client.Port, GUID)
                Dispose()
                Exit Sub
            End If

            'DONE: Get BindPoint Coords
            bindpoint_positionX = MySQLQuery.Rows(0).Item("bindpoint_positionX")
            bindpoint_positionY = MySQLQuery.Rows(0).Item("bindpoint_positionY")
            bindpoint_positionZ = MySQLQuery.Rows(0).Item("bindpoint_positionZ")
            bindpoint_map_id = MySQLQuery.Rows(0).Item("bindpoint_map_id")
            bindpoint_zone_id = MySQLQuery.Rows(0).Item("bindpoint_zone_id")

            'DONE: Get CharCreate Vars
            MyBase.Race = CType(MySQLQuery.Rows(0).Item("char_race"), Byte)
            Classe = CType(MySQLQuery.Rows(0).Item("char_class"), Byte)
            Gender = CType(MySQLQuery.Rows(0).Item("char_gender"), Byte)
            Skin = MySQLQuery.Rows(0).Item("char_skin")
            Face = MySQLQuery.Rows(0).Item("char_face")
            HairStyle = MySQLQuery.Rows(0).Item("char_hairStyle")
            HairColor = MySQLQuery.Rows(0).Item("char_hairColor")
            FacialHair = MySQLQuery.Rows(0).Item("char_facialHair")
            MyBase.ManaType = CType(MySQLQuery.Rows(0).Item("char_manaType"), Byte)
            Life.Base = CType(MySQLQuery.Rows(0).Item("char_life"), Short)
            Life.Current = Life.Maximum
            Mana.Base = CType(MySQLQuery.Rows(0).Item("char_mana"), Short)
            Mana.Current = Mana.Maximum
            Rage.Base = 1000
            Rage.Current = 0
            Energy.Base = 100
            Energy.Current = Energy.Maximum
            XP = MySQLQuery.Rows(0).Item("char_xp")

            If CharRaces.ContainsKey(MySQLQuery.Rows(0).Item("char_race")) Then
                Faction = CharRaces(MySQLQuery.Rows(0).Item("char_race")).FactionID
                If Gender = Genders.GENDER_MALE Then
                    Model = CharRaces(MySQLQuery.Rows(0).Item("char_race")).ModelMale
                Else
                    Model = CharRaces(MySQLQuery.Rows(0).Item("char_race")).ModelFemale
                End If
            End If
            If Model = 0 Then Model = GetRaceModel(Race, Gender)

            'DONE: Get Rested Bonus XP and Rest State
            RestBonus = MySQLQuery.Rows(0).Item("char_xp_rested")
            If RestBonus > 0 Then RestState = XPSTATE.Rested

            'DONE: Get Guild Info
            GuildID = MySQLQuery.Rows(0).Item("char_guildId")
            GuildRank = MySQLQuery.Rows(0).Item("char_guildRank")

            'DONE: Get all other vars
            Name = CType(MySQLQuery.Rows(0).Item("char_name"), String)
            Level = MySQLQuery.Rows(0).Item("char_level")
            Access = ClientVal.Access
            Copper = MySQLQuery.Rows(0).Item("char_copper")
            positionX = MySQLQuery.Rows(0).Item("char_positionX")
            positionY = MySQLQuery.Rows(0).Item("char_positionY")
            positionZ = MySQLQuery.Rows(0).Item("char_positionZ")
            orientation = MySQLQuery.Rows(0).Item("char_orientation")
            ZoneID = MySQLQuery.Rows(0).Item("char_zone_id")
            MapID = MySQLQuery.Rows(0).Item("char_map_id")
            LoginMap = MapID
            Strength.Base = CType(MySQLQuery.Rows(0).Item("char_strength"), Short)
            Agility.Base = CType(MySQLQuery.Rows(0).Item("char_agility"), Short)
            Stamina.Base = CType(MySQLQuery.Rows(0).Item("char_stamina"), Short)
            Intellect.Base = CType(MySQLQuery.Rows(0).Item("char_intellect"), Short)
            Spirit.Base = CType(MySQLQuery.Rows(0).Item("char_spirit"), Short)
            TalentPoints = MySQLQuery.Rows(0).Item("char_talentpoints")
            Items_AvailableBankSlots = MySQLQuery.Rows(0).Item("char_bankSlots")
            WatchedFactionIndex = MySQLQuery.Rows(0).Item("char_watchedFactionIndex")

            LoginTransport = MySQLQuery.Rows(0).Item("char_transportGuid")

            Dim tmp() As String

            Dim SpellQuery As New DataTable
            'ToDo: Need better string to query the data correctly. An ugly method.
            CharacterDatabase.Query(String.Format("UPDATE characters_spells SET cooldown = 0, cooldownitem = 0 WHERE guid = {0} AND cooldown > 0 AND cooldown < {1}; 
                SELECT * FROM characters_spells WHERE guid = {0}; 
                UPDATE characters_spells SET cooldown = 0, cooldownitem = 0 WHERE guid = {0} AND cooldown > 0 AND cooldown < {1};", GUID, GetTimestamp(Now)), SpellQuery)

            'DONE: Get SpellList
            For Each Spell As DataRow In SpellQuery.Rows
                Spells.Add(Spell.Item("spellid"),
                    New CharacterSpell(Spell.Item("spellid"),
                                       Spell.Item("active"),
                                       Spell.Item("cooldown"),
                                       Spell.Item("cooldownitem")))
            Next
            SpellQuery.Clear()

            'DONE: Get SkillList -> Saved as STRING like "SkillID1:Current:Maximum SkillID2:Current:Maximum SkillID3:Current:Maximum"
            tmp = Split(CType(MySQLQuery.Rows(0).Item("char_skillList"), String), " ")
            If tmp.Length > 0 Then
                For i As Integer = 0 To tmp.Length - 1
                    If Trim(tmp(i)) <> "" Then
                        Dim tmp2() As String = Split(tmp(i), ":")
                        If tmp2.Length = 3 Then
                            Skills(tmp2(0)) = New TSkill(tmp2(1), tmp2(2))
                            SkillsPositions(tmp2(0)) = i
                        End If
                    End If
                Next i
            End If

            'DONE: Get AuraList
            tmp = Split(CType(MySQLQuery.Rows(0).Item("char_auraList"), String), " ")
            If tmp.Length > 0 Then
                Dim currentTimestamp As UInteger = GetTimestamp(Now)
                For i As Integer = 0 To tmp.Length - 1
                    If Trim(tmp(i)) <> "" Then
                        Dim tmp2() As String = Split(tmp(i), ":")
                        If tmp2.Length = 3 Then
                            Dim AuraSlot As Integer = tmp2(0)
                            Dim AuraSpellID As Integer = tmp2(1)
                            Dim AuraExpire As Long = tmp2(2)
                            If AuraSlot < 0 OrElse AuraSlot >= MAX_AURA_EFFECTs_VISIBLE Then Continue For 'Not acceptable slot
                            If WS_Spells.SPELLs.ContainsKey(AuraSpellID) = False Then Continue For 'Non-existant spell

                            If ActiveSpells(AuraSlot) Is Nothing Then
                                Dim duration As Integer = 0
                                If AuraExpire = 0L Then 'Infinite duration aura
                                    duration = SPELL_DURATION_INFINITE
                                ElseIf AuraExpire < 0L Then 'Duration paused during offline-time
                                    duration = -AuraExpire
                                Else 'Duration continued during offline-time
                                    If currentTimestamp >= AuraExpire Then Continue For 'Duration has expired
                                    duration = (AuraExpire - currentTimestamp) * 1000
                                End If

                                ActiveSpells(AuraSlot) = New BaseActiveSpell(AuraSpellID, duration) With {
                                    .SpellCaster = Nothing
                                }

                                SetAura(AuraSpellID, AuraSlot, duration, False)
                            End If
                        End If
                    End If
                Next i
            End If

            'DONE: Get TutorialFlags -> Saved as STRING like "Flag1 Flag2 Flag3"
            tmp = Split(CType(MySQLQuery.Rows(0).Item("char_tutorialFlags"), String), " ")
            If tmp.Length > 0 Then
                For i As Integer = 0 To tmp.Length - 1
                    If Trim(tmp(i)) <> "" Then TutorialFlags(i) = tmp(i)
                Next i
            End If

            'DONE: Get TaxiFlags -> Saved as STRING like "Flag1 Flag2 Flag3"
            tmp = Split(CType(MySQLQuery.Rows(0).Item("char_taxiFlags"), String), " ")
            If tmp.Length > 0 Then
                For i As Integer = 0 To tmp.Length - 1
                    If Trim(tmp(i)) <> "" Then
                        For j As Byte = 0 To 7
                            If (tmp(i) And (1 << j)) Then
                                TaxiZones.Set((i * 8) + j, True)
                            End If
                        Next j
                    End If
                Next i
            End If

            'DONE: Get ZonesExplored -> Saved as STRING like "Flag1 Flag2 Flag3"
            tmp = Split(CType(MySQLQuery.Rows(0).Item("char_mapExplored"), String), " ")
            If tmp.Length > 0 Then
                For i As Integer = 0 To tmp.Length - 1
                    If Trim(tmp(i)) <> "" Then ZonesExplored(i) = UInteger.Parse(tmp(i))
                Next i
            End If

            'DONE: Get ActionButtons -> Saved as STRING like "Button1:Action1:Type1:Misc1 Button2:Action2:Type2:Misc2"
            tmp = Split(CType(MySQLQuery.Rows(0).Item("char_actionBar"), String), " ")
            If tmp.Length > 0 Then
                For i As Integer = 0 To tmp.Length - 1
                    If Trim(tmp(i)) <> "" Then
                        Dim tmp2() As String
                        tmp2 = Split(tmp(i), ":")
                        ActionButtons(tmp2(0)) = New TActionButton(tmp2(1), tmp2(2), tmp2(3))
                    End If
                Next i
            End If

            'DONE: Get ReputationPoints -> Saved as STRING like "Flags1:Standing1 Flags2:Standing2"
            tmp = Split(CType(MySQLQuery.Rows(0).Item("char_reputation"), String), " ")
            For i As Integer = 0 To 63
                Dim tmp2() As String
                tmp2 = Split(tmp(i), ":")
                Reputation(i) = New TReputation With {
                    .Flags = Trim(tmp2(0)),
                    .Value = Trim(tmp2(1))
                }
            Next

            'DONE: Get playerflags from force restrictions
            Dim ForceRestrictions As UInteger = MySQLQuery.Rows(0).Item("force_restrictions")
            If (ForceRestrictions And ForceRestrictionFlags.RESTRICT_HIDECLOAK) Then cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_HIDE_CLOAK
            If (ForceRestrictions And ForceRestrictionFlags.RESTRICT_HIDEHELM) Then cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_HIDE_HELM

            'DONE: Get Items
            MySQLQuery.Clear()
            CharacterDatabase.Query(String.Format("SELECT * FROM characters_inventory WHERE item_bag = {0};", GUID), MySQLQuery)
            For Each row As DataRow In MySQLQuery.Rows
                If row.Item("item_slot") <> ITEM_SLOT_NULL Then
                    Dim tmpItem As ItemObject = LoadItemByGUID(CType(row.Item("item_guid"), Long), Me, (CType(row.Item("item_slot"), Byte) < EquipmentSlots.EQUIPMENT_SLOT_END))
                    Items(row.Item("item_slot")) = tmpItem
                    If CType(row.Item("item_slot"), Byte) < InventorySlots.INVENTORY_SLOT_BAG_END Then UpdateAddItemStats(tmpItem, row.Item("item_slot"))
                End If
            Next

            'DONE: Get Honor Point
            HonorLoad()

            'DONE: Load quests in progress
            ALLQUESTS.LoadQuests(Me)

            'DONE: Initialize Internal fields
            Initialize()

            'DONE: Load current pet if any
            LoadPet(Me)

            'DONE: Load corpse if present
            MySQLQuery.Clear()
            CharacterDatabase.Query(String.Format("SELECT * FROM corpse WHERE player = {0};", GUID), MySQLQuery)
            If MySQLQuery.Rows.Count > 0 Then
                corpseGUID = MySQLQuery.Rows(0).Item("guid") + GUID_CORPSE
                corpseMapID = MySQLQuery.Rows(0).Item("map")
                corpsePositionX = MySQLQuery.Rows(0).Item("position_x")
                corpsePositionY = MySQLQuery.Rows(0).Item("position_y")
                corpsePositionZ = MySQLQuery.Rows(0).Item("position_z")

                'DONE: If you logout before releasing your corpse you will now go to the graveyard
                If positionX = corpsePositionX AndAlso positionY = corpsePositionY AndAlso positionZ = corpsePositionZ AndAlso MapID = corpseMapID Then
                    AllGraveYards.GoToNearestGraveyard(Me, False, False)
                End If

                'DONE: Make Dead
                DEAD = True
                cPlayerFlags = cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_DEAD

                'DONE: Update to see only dead
                Invisibility = InvisibilityLevel.DEAD
                CanSeeInvisibility = InvisibilityLevel.DEAD

                SetWaterWalk()

                'DONE: Set Auras
                If client.Character.Race = Races.RACE_NIGHT_ELF Then
                    client.Character.ApplySpell(20584)
                Else
                    client.Character.ApplySpell(8326)
                End If

                Mana.Current = 0
                Rage.Current = 0
                Energy.Current = 0
                Life.Current = 1
                cUnitFlags = UnitFlags.UNIT_FLAG_ATTACKABLE
                cDynamicFlags = 0
            Else
                'DONE: Calculate the bonus health and mana from stats
                Life.Bonus = ((Stamina.Base - 18) * 10)
                Mana.Bonus = ((Intellect.Base - 18) * 15)
                Life.Current = Life.Maximum
                Mana.Current = Life.Maximum
            End If

        End Sub

        Public Sub SaveAsNewCharacter(ByVal Account_ID As Integer)
            'Only for creating New Character
            Dim tmpCMD As String = "INSERT INTO characters (account_id"
            Dim tmpValues As String = " VALUES (" & Account_ID
            Dim temp As New ArrayList

            tmpCMD = tmpCMD & ", char_name"
            tmpValues = tmpValues & ", """ & Name & """"
            tmpCMD = tmpCMD & ", char_race"
            tmpValues = tmpValues & ", " & Race
            tmpCMD = tmpCMD & ", char_class"
            tmpValues = tmpValues & ", " & Classe
            tmpCMD = tmpCMD & ", char_gender"
            tmpValues = tmpValues & ", " & Gender
            tmpCMD = tmpCMD & ", char_skin"
            tmpValues = tmpValues & ", " & Skin
            tmpCMD = tmpCMD & ", char_face"
            tmpValues = tmpValues & ", " & Face
            tmpCMD = tmpCMD & ", char_hairStyle"
            tmpValues = tmpValues & ", " & HairStyle
            tmpCMD = tmpCMD & ", char_hairColor"
            tmpValues = tmpValues & ", " & HairColor
            tmpCMD = tmpCMD & ", char_facialHair"
            tmpValues = tmpValues & ", " & FacialHair
            tmpCMD = tmpCMD & ", char_level"
            tmpValues = tmpValues & ", " & Level
            tmpCMD = tmpCMD & ", char_manaType"
            tmpValues = tmpValues & ", " & ManaType

            tmpCMD = tmpCMD & ", char_mana"
            tmpValues = tmpValues & ", " & Mana.Base
            tmpCMD = tmpCMD & ", char_rage"
            tmpValues = tmpValues & ", " & Rage.Base
            tmpCMD = tmpCMD & ", char_energy"
            tmpValues = tmpValues & ", " & Energy.Base
            tmpCMD = tmpCMD & ", char_life"
            tmpValues = tmpValues & ", " & Life.Base

            tmpCMD = tmpCMD & ", char_positionX"
            tmpValues = tmpValues & ", " & Trim(Str(positionX))
            tmpCMD = tmpCMD & ", char_positionY"
            tmpValues = tmpValues & ", " & Trim(Str(positionY))
            tmpCMD = tmpCMD & ", char_positionZ"
            tmpValues = tmpValues & ", " & Trim(Str(positionZ))
            tmpCMD = tmpCMD & ", char_map_id"
            tmpValues = tmpValues & ", " & MapID
            tmpCMD = tmpCMD & ", char_zone_id"
            tmpValues = tmpValues & ", " & ZoneID
            tmpCMD = tmpCMD & ", char_orientation"
            tmpValues = tmpValues & ", " & Trim(Str(orientation))
            tmpCMD = tmpCMD & ", bindpoint_positionX"
            tmpValues = tmpValues & ", " & Trim(Str(bindpoint_positionX))
            tmpCMD = tmpCMD & ", bindpoint_positionY"
            tmpValues = tmpValues & ", " & Trim(Str(bindpoint_positionY))
            tmpCMD = tmpCMD & ", bindpoint_positionZ"
            tmpValues = tmpValues & ", " & Trim(Str(bindpoint_positionZ))
            tmpCMD = tmpCMD & ", bindpoint_map_id"
            tmpValues = tmpValues & ", " & bindpoint_map_id
            tmpCMD = tmpCMD & ", bindpoint_zone_id"
            tmpValues = tmpValues & ", " & bindpoint_zone_id

            tmpCMD = tmpCMD & ", char_copper"
            tmpValues = tmpValues & ", " & Copper
            tmpCMD = tmpCMD & ", char_xp"
            tmpValues = tmpValues & ", " & XP
            tmpCMD = tmpCMD & ", char_xp_rested"
            tmpValues = tmpValues & ", " & RestBonus

            'char_skillList
            temp.Clear()
            For Each Skill As KeyValuePair(Of Integer, TSkill) In Skills
                temp.Add(String.Format("{0}:{1}:{2}", Skill.Key, Skill.Value.Current, Skill.Value.Maximum))
            Next
            tmpCMD = tmpCMD & ", char_skillList"
            tmpValues = tmpValues & ", """ & Join(temp.ToArray, " ") & """"

            tmpCMD = tmpCMD & ", char_auraList"
            tmpValues = tmpValues & ", """""

            'char_tutorialFlags
            temp.Clear()
            For Each Flag As Byte In TutorialFlags
                temp.Add(Flag)
            Next
            tmpCMD = tmpCMD & ", char_tutorialFlags"
            tmpValues = tmpValues & ", """ & Join(temp.ToArray, " ") & """"

            'char_mapExplored
            temp.Clear()
            For Each Flag As Byte In ZonesExplored
                temp.Add(Flag)
            Next
            tmpCMD = tmpCMD & ", char_mapExplored"
            tmpValues = tmpValues & ", """ & Join(temp.ToArray, " ") & """"

            'char_reputation
            temp.Clear()
            For Each Reputation_Point As TReputation In Reputation
                temp.Add(Reputation_Point.Flags & ":" & Reputation_Point.Value)
            Next
            tmpCMD = tmpCMD & ", char_reputation"
            tmpValues = tmpValues & ", """ & Join(temp.ToArray, " ") & """"

            'char_actionBar
            temp.Clear()
            For Each ActionButton As KeyValuePair(Of Byte, TActionButton) In ActionButtons
                temp.Add(String.Format("{0}:{1}:{2}:{3}", ActionButton.Key, ActionButton.Value.Action, ActionButton.Value.ActionType, ActionButton.Value.ActionMisc))
            Next
            tmpCMD = tmpCMD & ", char_actionBar"
            tmpValues = tmpValues & ", """ & Join(temp.ToArray, " ") & """"

            tmpCMD = tmpCMD & ", char_strength"
            tmpValues = tmpValues & ", " & Strength.RealBase
            tmpCMD = tmpCMD & ", char_agility"
            tmpValues = tmpValues & ", " & Agility.RealBase
            tmpCMD = tmpCMD & ", char_stamina"
            tmpValues = tmpValues & ", " & Stamina.RealBase
            tmpCMD = tmpCMD & ", char_intellect"
            tmpValues = tmpValues & ", " & Intellect.RealBase
            tmpCMD = tmpCMD & ", char_spirit"
            tmpValues = tmpValues & ", " & Spirit.RealBase

            Dim ForceRestrictions As UInteger = 0
            If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_HIDE_CLOAK) Then ForceRestrictions = ForceRestrictions Or ForceRestrictionFlags.RESTRICT_HIDECLOAK
            If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_HIDE_HELM) Then ForceRestrictions = ForceRestrictions Or ForceRestrictionFlags.RESTRICT_HIDEHELM
            tmpCMD = tmpCMD & ", force_restrictions"
            tmpValues = tmpValues & ", " & ForceRestrictions

            tmpCMD = tmpCMD & ") " & tmpValues & ");"
            CharacterDatabase.Update(tmpCMD)

            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters WHERE char_name = '{0}';", Name), MySQLQuery)
            GUID = CType(MySQLQuery.Rows(0).Item("char_guid"), Long)

            HonorSaveAsNew()
        End Sub

        Public Sub Save()
            SaveCharacter()

            For Each Item As KeyValuePair(Of Byte, ItemObject) In Items
                Item.Value.Save()
            Next
        End Sub

        Public Sub SaveCharacter()
            Dim tmp As String = "UPDATE characters SET"

            tmp = tmp & " char_name=""" & Name & """"
            tmp = tmp & ", char_race=" & Race
            tmp = tmp & ", char_class=" & Classe
            tmp = tmp & ", char_gender=" & Gender
            tmp = tmp & ", char_skin=" & Skin
            tmp = tmp & ", char_face=" & Face
            tmp = tmp & ", char_hairStyle=" & HairStyle
            tmp = tmp & ", char_hairColor=" & HairColor
            tmp = tmp & ", char_facialHair=" & FacialHair
            tmp = tmp & ", char_level=" & Level
            tmp = tmp & ", char_manaType=" & ManaType

            tmp = tmp & ", char_life=" & Life.Base
            tmp = tmp & ", char_rage=" & Rage.Base
            tmp = tmp & ", char_mana=" & Mana.Base
            tmp = tmp & ", char_energy=" & Energy.Base

            tmp = tmp & ", char_strength=" & Strength.RealBase
            tmp = tmp & ", char_agility=" & Agility.RealBase
            tmp = tmp & ", char_stamina=" & Stamina.RealBase
            tmp = tmp & ", char_intellect=" & Intellect.RealBase
            tmp = tmp & ", char_spirit=" & Spirit.RealBase

            tmp = tmp & ", char_map_id=" & MapID
            tmp = tmp & ", char_zone_id=" & ZoneID
            If OnTransport IsNot Nothing Then
                tmp = tmp & ", char_positionX=" & Trim(Str(transportX))
                tmp = tmp & ", char_positionY=" & Trim(Str(transportY))
                tmp = tmp & ", char_positionZ=" & Trim(Str(transportZ))
                tmp = tmp & ", char_orientation=" & Trim(Str(transportO))
                tmp = tmp & ", char_transportGuid=" & Trim(Str(OnTransport.GUID))
            Else
                tmp = tmp & ", char_positionX=" & Trim(Str(positionX))
                tmp = tmp & ", char_positionY=" & Trim(Str(positionY))
                tmp = tmp & ", char_positionZ=" & Trim(Str(positionZ))
                tmp = tmp & ", char_orientation=" & Trim(Str(orientation))
                tmp = tmp & ", char_transportGuid=0"
            End If
            tmp = tmp & ", bindpoint_positionX=" & Trim(Str(bindpoint_positionX))
            tmp = tmp & ", bindpoint_positionY=" & Trim(Str(bindpoint_positionY))
            tmp = tmp & ", bindpoint_positionZ=" & Trim(Str(bindpoint_positionZ))
            tmp = tmp & ", bindpoint_map_id=" & bindpoint_map_id
            tmp = tmp & ", bindpoint_zone_id=" & bindpoint_zone_id

            tmp = tmp & ", char_copper=" & Copper
            tmp = tmp & ", char_xp=" & XP
            tmp = tmp & ", char_xp_rested=" & RestBonus

            tmp = tmp & ", char_guildId=" & GuildID
            tmp = tmp & ", char_guildRank=" & GuildRank

            Dim temp As New ArrayList

            'char_skillList
            temp.Clear()
            For Each Skill As KeyValuePair(Of Integer, TSkill) In Skills
                temp.Add(String.Format("{0}:{1}:{2}", Skill.Key, Skill.Value.Current, Skill.Value.Maximum))
            Next
            tmp = tmp & ", char_skillList=""" & Join(temp.ToArray, " ") & """"

            'char_auraList
            temp.Clear()
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing AndAlso (ActiveSpells(i).SpellDuration = SPELL_DURATION_INFINITE OrElse ActiveSpells(i).SpellDuration > 10000) Then 'If the aura exists and if it's worth saving
                    Dim expire As Long = 0L
                    If ActiveSpells(i).SpellDuration <> SPELL_DURATION_INFINITE Then expire = GetTimestamp(Now) + (ActiveSpells(i).SpellDuration \ 1000)
                    'TODO: If Not_Tick_While_Offline Then expire = -ActiveSpells(i).SpellDuration
                    temp.Add(String.Format("{0}:{1}:{2}", i, ActiveSpells(i).SpellID, expire))
                End If
            Next
            tmp = tmp & ", char_auraList=""" & Join(temp.ToArray, " ") & """"

            'char_tutorialFlags
            temp.Clear()
            For Each Flag As Byte In TutorialFlags
                temp.Add(Flag)
            Next
            tmp = tmp & ", char_tutorialFlags=""" & Join(temp.ToArray, " ") & """"

            'char_taxiFlags
            temp.Clear()
            Dim TmpArray(31) As Byte
            TaxiZones.CopyTo(TmpArray, 0)
            For Each Flag As Byte In TmpArray
                temp.Add(Flag)
            Next
            tmp = tmp & ", char_taxiFlags=""" & Join(temp.ToArray, " ") & """"

            'char_mapExplored
            temp.Clear()
            For Each Flag As UInteger In ZonesExplored
                temp.Add(Flag)
            Next
            tmp = tmp & ", char_mapExplored=""" & Join(temp.ToArray, " ") & """"

            'char_reputation
            temp.Clear()
            For Each Reputation_Point As TReputation In Reputation
                temp.Add(Reputation_Point.Flags & ":" & Reputation_Point.Value)
            Next
            tmp = tmp & ", char_reputation=""" & Join(temp.ToArray, " ") & """"

            'char_actionBar
            temp.Clear()
            For Each ActionButton As KeyValuePair(Of Byte, TActionButton) In ActionButtons
                temp.Add(String.Format("{0}:{1}:{2}:{3}", ActionButton.Key, ActionButton.Value.Action, ActionButton.Value.ActionType, ActionButton.Value.ActionMisc))
            Next
            tmp = tmp & ", char_actionBar=""" & Join(temp.ToArray, " ") & """"

            tmp = tmp & ", char_talentpoints=" & TalentPoints

            Dim ForceRestrictions As UInteger = 0
            If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_HIDE_CLOAK) Then ForceRestrictions = ForceRestrictions Or ForceRestrictionFlags.RESTRICT_HIDECLOAK
            If (cPlayerFlags And PlayerFlags.PLAYER_FLAGS_HIDE_HELM) Then ForceRestrictions = ForceRestrictions Or ForceRestrictionFlags.RESTRICT_HIDEHELM
            tmp = tmp & ", force_restrictions=" & ForceRestrictions

            tmp = tmp + String.Format(" WHERE char_guid = ""{0}"";", GUID)
            CharacterDatabase.Update(tmp)
        End Sub

        Public Sub SavePosition()
            Dim tmp As String = "UPDATE characters SET"

            tmp = tmp & ", char_positionX=" & Trim(Str(positionX))
            tmp = tmp & ", char_positionY=" & Trim(Str(positionY))
            tmp = tmp & ", char_positionZ=" & Trim(Str(positionZ))
            tmp = tmp & ", char_map_id=" & MapID

            tmp = tmp + String.Format(" WHERE char_guid = ""{0}"";", GUID)
            CharacterDatabase.Update(tmp)
        End Sub

        'Party/Raid
        Public Group As Group = Nothing
        Public GroupUpdateFlag As UInteger = 0
        Public ReadOnly Property IsInGroup() As Boolean
            Get
                Return (Group IsNot Nothing)
            End Get
        End Property

        Public ReadOnly Property IsInRaid() As Boolean
            Get
                Return ((Group IsNot Nothing) AndAlso (Group.Type = GroupType.RAID))
            End Get
        End Property

        Public ReadOnly Property IsGroupLeader() As Boolean
            Get
                Return (Group.Leader = GUID)
            End Get
        End Property

        Public Sub GroupUpdate()
            If Group Is Nothing Then Exit Sub
            If GroupUpdateFlag = 0 Then Exit Sub
            Dim Packet As PacketClass = BuildPartyMemberStats(Me, GroupUpdateFlag)
            GroupUpdateFlag = 0
            If Not Packet Is Nothing Then Group.Broadcast(Packet)
        End Sub

        Public Sub GroupUpdate(ByVal Flag As Integer)
            If Group Is Nothing Then Exit Sub
            Dim Packet As PacketClass = BuildPartyMemberStats(Me, Flag)
            If Not Packet Is Nothing Then Group.Broadcast(Packet)
        End Sub

        'Guilds
        Public GuildID As UInteger = 0
        Public GuildRank As Byte = 0
        Public GuildInvited As Integer = 0
        Public GuildInvitedBy As Integer = 0
        Public ReadOnly Property IsInGuild() As Boolean
            Get
                Return GuildID <> 0
            End Get
        End Property

        'Duel
        Public DuelArbiter As ULong = 0
        Public DuelPartner As CharacterObject = Nothing
        Public DuelOutOfBounds As Byte = DUEL_COUNTER_DISABLED
        Public ReadOnly Property IsInDuel() As Boolean
            Get
                Return (Not (DuelPartner Is Nothing))
            End Get
        End Property

        Public Sub StartDuel()
            Thread.Sleep(DUEL_COUNTDOWN)
            If DuelArbiter = 0 Then Exit Sub
            If DuelPartner Is Nothing Then Exit Sub

            'DONE: Do updates
            SetUpdateFlag(EPlayerFields.PLAYER_DUEL_TEAM, 1)
            DuelPartner.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_TEAM, 2)
            DuelPartner.SendCharacterUpdate(True)
            SendCharacterUpdate(True)
        End Sub

        'NPC Talking and Quests
        Public TalkMenuTypes As New ArrayList
        Public TalkQuests(QuestInfo.QUEST_SLOTS) As WS_QuestsBase
        Public QuestsCompleted As New List(Of Integer)
        Public TalkCurrentQuest As WS_QuestInfo = Nothing
        Public Function TalkAddQuest(ByRef Quest As WS_QuestInfo) As Boolean
            For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                If TalkQuests(i) Is Nothing Then
                    'DONE: Initialize quest info
                    ALLQUESTS.CreateQuest(TalkQuests(i), Quest)

                    'DONE: Initialize quest
                    If TypeOf TalkQuests(i) Is WS_QuestsBaseScripted Then
                        CType(TalkQuests(i), WS_QuestsBaseScripted).OnQuestStart(Me)
                    Else
                        TalkQuests(i).Initialize(Me)
                    End If

                    TalkQuests(i).Slot = i

                    Dim updateDataCount As Integer = UpdateData.Count
                    Dim questState As Integer = TalkQuests(i).GetProgress

                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_1 + i * 3, TalkQuests(i).ID)
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + i * 3, questState)
                    SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + i * 3 + 1, 0) 'Timer

                    CharacterDatabase.Update(String.Format("INSERT INTO characters_quests (char_guid, quest_id, quest_status) VALUES ({0}, {1}, {2});", GUID, TalkQuests(i).ID, questState))

                    SendCharacterUpdate(updateDataCount <> 0)
                    Return True
                End If
            Next

            Return False
        End Function

        Public Function TalkDeleteQuest(ByVal QuestSlot As Byte) As Boolean
            If TalkQuests(QuestSlot) Is Nothing Then
                Return False
            Else
                If TypeOf TalkQuests(QuestSlot) Is WS_QuestsBaseScripted Then CType(TalkQuests(QuestSlot), WS_QuestsBaseScripted).OnQuestCancel(Me)

                Dim updateDataCount As Integer = UpdateData.Count

                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_1 + QuestSlot * 3, 0)
                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + QuestSlot * 3, 0)
                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + QuestSlot * 3 + 1, 0)

                CharacterDatabase.Update(String.Format("DELETE  FROM characters_quests WHERE char_guid = {0} AND quest_id = {1};", GUID, TalkQuests(QuestSlot).ID))
                TalkQuests(QuestSlot) = Nothing

                SendCharacterUpdate(updateDataCount <> 0)
                Return True
            End If
        End Function

        Public Function TalkCompleteQuest(ByVal QuestSlot As Byte) As Boolean
            If TalkQuests(QuestSlot) Is Nothing Then
                Return False
            Else
                If TypeOf TalkQuests(QuestSlot) Is WS_QuestsBaseScripted Then CType(TalkQuests(QuestSlot), WS_QuestsBaseScripted).OnQuestComplete(Me)
                Dim updateDataCount As Integer = UpdateData.Count

                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_1 + QuestSlot * 3, 0)
                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + QuestSlot * 3, 0)
                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + QuestSlot * 3 + 1, 0)

                QuestsCompleted.Add(TalkQuests(QuestSlot).ID)
                CharacterDatabase.Update(String.Format("UPDATE characters_quests SET quest_status = -1 WHERE char_guid = {0} AND quest_id = {1};", GUID, TalkQuests(QuestSlot).ID))
                TalkQuests(QuestSlot) = Nothing

                'SendCharacterUpdate(updateDataCount <> 0)
                Return True
            End If
        End Function

        Public Function TalkUpdateQuest(ByVal QuestSlot As Byte) As Boolean
            If TalkQuests(QuestSlot) Is Nothing Then
                Return False
            Else
                Dim updateDataCount As Integer = UpdateData.Count
                Dim tmpState As Integer = TalkQuests(QuestSlot).GetState
                Dim tmpProgress As Integer = TalkQuests(QuestSlot).GetProgress
                Dim tmpTimer As Integer = 0
                If TalkQuests(QuestSlot).TimeEnd > 0 Then tmpTimer = TalkQuests(QuestSlot).TimeEnd - GetTimestamp(Now)
                CharacterDatabase.Update(String.Format("UPDATE characters_quests SET quest_status = {2} WHERE char_guid = {0} AND quest_id = {1};", GUID, TalkQuests(QuestSlot).ID, tmpProgress))

                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + QuestSlot * 3, tmpProgress)
                SetUpdateFlag(EPlayerFields.PLAYER_QUEST_LOG_1_2 + QuestSlot * 3 + 1, 0) 'Timer
                SendCharacterUpdate(updateDataCount <> 0)

                Return True
            End If
        End Function

        Public Function TalkCanAccept(ByRef Quest As WS_QuestInfo) As Boolean

            Dim DBResult As New DataTable
            CharacterDatabase.Query(String.Format("SELECT quest_status FROM characters_quests WHERE char_guid = {0} AND quest_id = {1} LIMIT 1;", GUID, Quest.ID), DBResult)
            If DBResult.Rows.Count > 0 Then
                Dim status As Integer = DBResult.Rows(0).Item("quest_status")

                If status = -1 Then ' Quest is completed
                    Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                    Try
                        packet.AddInt32(QuestInvalidError.INVALIDREASON_COMPLETED_QUEST)
                        client.Send(packet)
                    Finally
                        packet.Dispose()
                    End Try
                Else
                    Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                    Try
                        packet.AddInt32(QuestInvalidError.INVALIDREASON_HAVE_QUEST)
                        client.Send(packet)
                    Finally
                        packet.Dispose()
                    End Try
                End If
                Return False
            End If

            If Quest.RequiredRace <> 0 AndAlso (Quest.RequiredRace And (1 << (Race - 1))) = 0 Then
                Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                Try
                    packet.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_RACE)
                    client.Send(packet)
                Finally
                    packet.Dispose()
                End Try
                Return False
            End If

            If Quest.RequiredClass <> 0 AndAlso (Quest.RequiredClass And (1 << (Classe - 1))) = 0 Then
                'TODO: Find constant for INVALIDREASON_DONT_HAVE_CLASS if exists
                Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                Try
                    packet.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ)
                    client.Send(packet)
                Finally
                    packet.Dispose()
                End Try
                Return False
            End If

            If Quest.RequiredTradeSkill <> 0 AndAlso Not Skills.ContainsKey(Quest.RequiredTradeSkill) Then
                'TODO: Find constant for INVALIDREASON_DONT_HAVE_SKILL if exists
                Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                Try
                    packet.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ)
                    client.Send(packet)
                Finally
                    packet.Dispose()
                End Try
                Return False
            End If

            'TODO: Check requirements for reputation
            'TODO: Check requirements for honor?

            Return True
        End Function

        Public Function IsQuestCompleted(ByVal QuestID As Integer) As Boolean
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT quest_id FROM characters_quests WHERE char_guid = {0} AND quest_status = -1 AND quest_id = {1};", GUID, QuestID), q)

            Return q.Rows.Count <> 0
        End Function

        Public Function IsQuestInProgress(ByVal QuestID As Integer) As Boolean
            For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                If Not TalkQuests(i) Is Nothing Then
                    If TalkQuests(i).ID = QuestID Then Return True
                End If
            Next

            Return False
        End Function

        'Helper Funtions
        Public Sub LogXPGain(ByVal Ammount As Integer, ByVal Rested As Integer, ByVal VictimGUID As ULong, ByVal Group As Single)
            Dim SMSG_LOG_XPGAIN As New PacketClass(OPCODES.SMSG_LOG_XPGAIN)
            Try
                SMSG_LOG_XPGAIN.AddUInt64(VictimGUID)

                'Total XP
                SMSG_LOG_XPGAIN.AddInt32(Ammount)

                If VictimGUID <> 0 Then
                    'XP from kill
                    SMSG_LOG_XPGAIN.AddInt8(0)
                Else
                    'XP from other source
                    SMSG_LOG_XPGAIN.AddInt8(1)
                End If

                'Rested XP
                SMSG_LOG_XPGAIN.AddInt32(Ammount - Rested)

                'Group bonus percent, 100% for none (1.0F)
                SMSG_LOG_XPGAIN.AddSingle(Group)

                client.Send(SMSG_LOG_XPGAIN)
            Finally
                SMSG_LOG_XPGAIN.Dispose()
            End Try
        End Sub

        Public Sub LogHonorGain(ByVal Ammount As Integer, Optional ByVal VictimGUID As ULong = 0, Optional ByVal VictimRANK As Byte = 0)
            Dim SMSG_PVP_CREDIT As New PacketClass(OPCODES.SMSG_PVP_CREDIT)
            Try
                SMSG_PVP_CREDIT.AddInt32(Ammount)
                SMSG_PVP_CREDIT.AddUInt64(VictimGUID)
                SMSG_PVP_CREDIT.AddInt32(VictimRANK)
                client.Send(SMSG_PVP_CREDIT)
            Finally
                SMSG_PVP_CREDIT.Dispose()
            End Try
        End Sub

        Public Sub LogLootItem(ByVal Item As ItemObject, ByVal ItemCount As Byte, ByVal Recieved As Boolean, ByVal Created As Boolean)
            Dim response As New PacketClass(OPCODES.SMSG_ITEM_PUSH_RESULT)
            Try
                response.AddUInt64(GUID)
                response.AddInt32(Recieved) '0 = Looted, 1 = From NPC?
                response.AddInt32(Created) '0 = Recieved, 1 = Created
                response.AddInt32(1) 'Unk, always 1
                response.AddInt8(Item.GetBagSlot)
                If Item.StackCount = ItemCount Then
                    response.AddInt32(Item.GetSlot) 'Item Slot (When added to stack: 0xFFFFFFFF)
                Else 'Added to stack
                    response.AddInt32(&HFFFFFFFF)
                End If
                response.AddInt32(Item.ItemEntry)
                response.AddInt32(Item.SuffixFactor)
                response.AddInt32(Item.RandomProperties)
                response.AddInt32(ItemCount) 'Count of items
                response.AddInt32(Me.ItemCOUNT(Item.ItemEntry)) 'Count of items in inventory
                client.SendMultiplyPackets(response)
                If IsInGroup Then Group.Broadcast(response)
            Finally
                response.Dispose()
            End Try
        End Sub

        Public Sub LogEnvironmentalDamage(ByVal dmgType As DamageTypes, ByVal Damage As Integer)
            Dim SMSG_ENVIRONMENTALDAMAGELOG As New PacketClass(OPCODES.SMSG_ENVIRONMENTALDAMAGELOG)
            Try
                SMSG_ENVIRONMENTALDAMAGELOG.AddUInt64(GUID)
                SMSG_ENVIRONMENTALDAMAGELOG.AddInt8(dmgType)
                SMSG_ENVIRONMENTALDAMAGELOG.AddInt32(Damage)
                SMSG_ENVIRONMENTALDAMAGELOG.AddInt32(0)
                SMSG_ENVIRONMENTALDAMAGELOG.AddInt32(0)

                SendToNearPlayers(SMSG_ENVIRONMENTALDAMAGELOG)
            Finally
                SMSG_ENVIRONMENTALDAMAGELOG.Dispose()
            End Try
        End Sub

        Public ReadOnly Property IsHorde() As Boolean
            Get
                Select Case Race
                    Case Races.RACE_DWARF, Races.RACE_GNOME, Races.RACE_HUMAN, Races.RACE_NIGHT_ELF
                        Return False
                    Case Else
                        Return True
                End Select
            End Get
        End Property

        Public ReadOnly Property Team() As Integer
            Get
                Select Case Race
                    Case Races.RACE_DWARF, Races.RACE_GNOME, Races.RACE_HUMAN, Races.RACE_NIGHT_ELF
                        Return 469
                    Case Else
                        Return 67
                End Select
            End Get
        End Property

        Public Function GetStealthDistance(ByRef objCharacter As BaseUnit) As Single
            Dim VisibleDistance As Single = 10.5 - (Invisibility_Value / 100)
            VisibleDistance += objCharacter.Level - CInt(Level)
            VisibleDistance += (objCharacter.CanSeeInvisibility_Stealth - Invisibility_Bonus) / 5
            Return VisibleDistance
        End Function

        'Warden AntiCheat Engine
        Public WardenData As New WardenData
    End Class
End Module
