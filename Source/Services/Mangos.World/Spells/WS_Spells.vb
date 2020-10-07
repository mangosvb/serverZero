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

Imports System.Threading
Imports Mangos.Common
Imports Mangos.Common.Enums.Faction
Imports Mangos.Common.Enums.GameObject
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Item
Imports Mangos.Common.Enums.Player
Imports Mangos.Common.Enums.Spell
Imports Mangos.Common.Enums.Unit
Imports Mangos.Common.Globals
Imports Mangos.World.DataStores
Imports Mangos.World.Globals
Imports Mangos.World.Handlers
Imports Mangos.World.Loots
Imports Mangos.World.Maps
Imports Mangos.World.Objects
Imports Mangos.World.Player
Imports Mangos.World.Server

Namespace Spells

    Public Class WS_Spells

#Region "WS.Spells.Framework"

        'WARNING: Use only with SPELLs()
        Public Class SpellInfo
            Public ID As Integer = 0
            Public School As Integer = 0
            Public Category As Integer = 0
            Public DispellType As Integer = 0
            Public Mechanic As Integer = 0

            Public Attributes As Integer = 0
            Public AttributesEx As Integer = 0
            Public AttributesEx2 As Integer = 0
            Public RequredCasterStance As Integer = 0
            Public ShapeshiftExclude As Integer = 0
            Public Target As Integer = 0
            Public TargetCreatureType As Integer = 0
            Public FocusObjectIndex As Integer = 0
            Public FacingCasterFlags As Integer = 0
            Public CasterAuraState As Integer = 0
            Public TargetAuraState As Integer = 0
            Public ExcludeCasterAuraState As Integer = 0
            Public ExcludeTargetAuraState As Integer = 0

            Public SpellCastTimeIndex As Integer = 0
            Public CategoryCooldown As Integer = 0
            Public SpellCooldown As Integer = 0

            Public interruptFlags As Integer = 0
            Public auraInterruptFlags As Integer = 0
            Public channelInterruptFlags As Integer = 0
            Public procFlags As Integer = 0
            Public procChance As Integer = 0
            Public procCharges As Integer = 0
            Public maxLevel As Integer = 0
            Public baseLevel As Integer = 0
            Public spellLevel As Integer = 0
            Public maxStack As Integer = 0

            Public DurationIndex As Integer = 0

            Public powerType As Integer = 0
            Public manaCost As Integer = 0
            Public manaCostPerlevel As Integer = 0
            Public manaPerSecond As Integer = 0
            Public manaPerSecondPerLevel As Integer = 0
            Public manaCostPercent As Integer = 0

            Public rangeIndex As Integer = 0
            Public Speed As Single = 0
            Public modalNextSpell As Integer = 0

            Public Totem() As Integer = {0, 0}
            Public TotemCategory() As Integer = {0, 0}
            Public Reagents() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
            Public ReagentsCount() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}

            Public EquippedItemClass As Integer = 0
            Public EquippedItemSubClass As Integer = 0
            Public EquippedItemInventoryType As Integer = 0

            Public SpellEffects() As SpellEffect = {Nothing, Nothing, Nothing}

            Public MaxTargets As Integer = 0
            Public RequiredAreaID As Integer = 0

            Public SpellVisual As Integer = 0
            Public SpellPriority As Integer = 0
            Public AffectedTargetLevel As Integer = 0
            Public SpellIconID As Integer = 0
            Public ActiveIconID As Integer = 0
            Public SpellNameFlag As Integer = 0
            Public Rank As String = ""
            Public RankFlags As Integer = 0
            Public StartRecoveryCategory As Integer = 0
            Public StartRecoveryTime As Integer = 0
            Public SpellFamilyName As Integer = 0
            Public SpellFamilyFlags As Integer = 0
            'Public MaxAffectedTargets As Integer = 0
            Public DamageType As Integer = 0
            Public Name As String = ""

            Public CustomAttributs As UInteger = 0UI

            Public ReadOnly Property SchoolMask() As SpellSchoolMask
                Get
                    Return (1 << School)
                End Get
            End Property

            Public ReadOnly Property GetDuration() As Integer
                Get
                    If _WS_Spells.SpellDuration.ContainsKey(DurationIndex) Then Return _WS_Spells.SpellDuration(DurationIndex)
                    Return 0
                End Get
            End Property

            Public ReadOnly Property GetRange() As Integer
                Get
                    If _WS_Spells.SpellRange.ContainsKey(rangeIndex) Then Return _WS_Spells.SpellRange(rangeIndex)
                    Return 0
                End Get
            End Property

            Public ReadOnly Property GetFocusObject() As String
                Get
                    If _WS_Spells.SpellFocusObject.ContainsKey(FocusObjectIndex) Then Return _WS_Spells.SpellFocusObject(FocusObjectIndex)
                    Return 0
                End Get
            End Property

            Public ReadOnly Property GetCastTime() As Integer
                Get
                    If _WS_Spells.SpellCastTime.ContainsKey(SpellCastTimeIndex) Then Return _WS_Spells.SpellCastTime(SpellCastTimeIndex)
                    Return 0
                End Get
            End Property

            Public ReadOnly Property GetManaCost(ByVal level As Integer, ByVal Mana As Integer) As Integer
                Get
                    Return manaCost + manaCostPerlevel * level + Mana * (manaCostPercent / 100)
                End Get
            End Property

            Public ReadOnly Property IsAura() As Boolean
                Get
                    If SpellEffects(0) IsNot Nothing AndAlso SpellEffects(0).ApplyAuraIndex <> 0 Then Return True
                    If SpellEffects(1) IsNot Nothing AndAlso SpellEffects(1).ApplyAuraIndex <> 0 Then Return True
                    If SpellEffects(2) IsNot Nothing AndAlso SpellEffects(2).ApplyAuraIndex <> 0 Then Return True
                    Return False
                End Get
            End Property

            Public ReadOnly Property IsAOE() As Boolean
                Get
                    If SpellEffects(0) IsNot Nothing AndAlso SpellEffects(0).IsAOE Then Return True
                    If SpellEffects(1) IsNot Nothing AndAlso SpellEffects(1).IsAOE Then Return True
                    If SpellEffects(2) IsNot Nothing AndAlso SpellEffects(2).IsAOE Then Return True
                    Return False
                End Get
            End Property

            Public ReadOnly Property IsDispell() As Boolean
                Get
                    If SpellEffects(0) IsNot Nothing AndAlso SpellEffects(0).ID = SpellEffects_Names.SPELL_EFFECT_DISPEL Then Return True
                    If SpellEffects(1) IsNot Nothing AndAlso SpellEffects(1).ID = SpellEffects_Names.SPELL_EFFECT_DISPEL Then Return True
                    If SpellEffects(2) IsNot Nothing AndAlso SpellEffects(2).ID = SpellEffects_Names.SPELL_EFFECT_DISPEL Then Return True
                    Return False
                End Get
            End Property

            Public ReadOnly Property IsPassive() As Boolean
                Get
                    Return (Attributes And SpellAttributes.SPELL_ATTR_PASSIVE) AndAlso ((AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_NEGATIVE) = 0)
                End Get
            End Property

            Public ReadOnly Property IsNegative() As Boolean
                Get
                    For i As Byte = 0 To 2
                        If SpellEffects(i) IsNot Nothing AndAlso SpellEffects(i).IsNegative Then Return True
                    Next
                    Return (AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_NEGATIVE)
                End Get
            End Property

            Public ReadOnly Property IsAutoRepeat() As Boolean
                Get
                    Return (AttributesEx2 And SpellAttributesEx2.SPELL_ATTR_EX2_AUTO_SHOOT)
                End Get
            End Property

            Public ReadOnly Property IsRanged() As Boolean
                Get
                    Return (DamageType = SpellDamageType.SPELL_DMG_TYPE_RANGED)
                End Get
            End Property

            Public ReadOnly Property IsMelee() As Boolean
                Get
                    Return (DamageType = SpellDamageType.SPELL_DMG_TYPE_MELEE)
                End Get
            End Property

            Public ReadOnly Property CanStackSpellRank() As Boolean
                Get
                    If Not _WS_Spells.SpellChains.ContainsKey(ID) OrElse _WS_Spells.SpellChains(ID) = 0 Then Return True

                    If powerType = ManaTypes.TYPE_MANA Then
                        If manaCost > 0 Then Return True
                        If manaCostPercent > 0 Then Return True
                        If manaCostPerlevel > 0 Then Return True
                        If manaPerSecond > 0 Then Return True
                        If manaPerSecondPerLevel > 0 Then Return True
                    End If
                    Return False
                End Get
            End Property

            Public Function GetTargets(ByRef Caster As WS_Base.BaseObject, ByVal Targets As SpellTargets, ByVal Index As Byte) As Dictionary(Of WS_Base.BaseObject, SpellMissInfo)
                Dim TargetsInfected As New List(Of WS_Base.BaseObject)

                Dim Ref As WS_Base.BaseUnit = Nothing
                If TypeOf Caster Is WS_Totems.TotemObject Then Ref = CType(Caster, WS_Totems.TotemObject).Caster
                If SpellEffects(Index) IsNot Nothing Then
                    For j As Byte = 0 To 1
                        Dim ImplicitTarget As SpellImplicitTargets = SpellEffects(Index).implicitTargetA
                        If j = 1 Then ImplicitTarget = SpellEffects(Index).implicitTargetB
                        _WorldServer.Log.WriteLine(LogType.DEBUG, "{0}: {1}", CStr(IIf(j = 1, "ImplicitTargetB", "ImplicitTargetA")), ImplicitTarget)
                        If ImplicitTarget = SpellImplicitTargets.TARGET_NOTHING Then Continue For
                        Select Case ImplicitTarget
                            Case SpellImplicitTargets.TARGET_ALL_ENEMY_IN_AREA, SpellImplicitTargets.TARGET_ALL_ENEMY_IN_AREA_INSTANT
                                Dim EnemyTargets As List(Of WS_Base.BaseUnit) = Nothing
                                If (Targets.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
                                    EnemyTargets = _WS_Spells.GetEnemyAtPoint(Caster, Targets.dstX, Targets.dstY, Targets.dstZ, SpellEffects(Index).GetRadius)
                                Else
                                    If TypeOf Caster Is WS_DynamicObjects.DynamicObjectObject Then
                                        EnemyTargets = _WS_Spells.GetEnemyAtPoint(CType(Caster, WS_DynamicObjects.DynamicObjectObject).Caster, Caster.positionX, Caster.positionY, Caster.positionZ, SpellEffects(Index).GetRadius)
                                    Else
                                        EnemyTargets = _WS_Spells.GetEnemyAtPoint(Caster, Caster.positionX, Caster.positionY, Caster.positionZ, SpellEffects(Index).GetRadius)
                                    End If
                                End If
                                For Each EnemyTarget As WS_Base.BaseUnit In EnemyTargets
                                    If Not TargetsInfected.Contains(EnemyTarget) Then TargetsInfected.Add(EnemyTarget)
                                Next
                            Case SpellImplicitTargets.TARGET_ALL_FRIENDLY_UNITS_AROUND_CASTER
                                Dim EnemyTargets As List(Of WS_Base.BaseUnit) = _WS_Spells.GetEnemyAroundMe(Caster, SpellEffects(Index).GetRadius, Ref)
                                For Each EnemyTarget As WS_Base.BaseUnit In EnemyTargets
                                    If Not TargetsInfected.Contains(EnemyTarget) Then TargetsInfected.Add(EnemyTarget)
                                Next
                            Case SpellImplicitTargets.TARGET_ALL_PARTY
                                Dim PartyTargets As List(Of WS_Base.BaseUnit) = _WS_Spells.GetPartyMembersAroundMe(Caster, 9999999)
                                For Each PartyTarget As WS_Base.BaseUnit In PartyTargets
                                    If Not TargetsInfected.Contains(PartyTarget) Then TargetsInfected.Add(PartyTarget)
                                Next
                            Case SpellImplicitTargets.TARGET_ALL_PARTY_AROUND_CASTER_2, SpellImplicitTargets.TARGET_AROUND_CASTER_PARTY, SpellImplicitTargets.TARGET_AREAEFFECT_PARTY
                                Dim PartyTargets As List(Of WS_Base.BaseUnit)
                                If TypeOf Caster Is WS_Totems.TotemObject Then
                                    PartyTargets = _WS_Spells.GetPartyMembersAtPoint(CType(Caster, WS_Totems.TotemObject).Caster, SpellEffects(Index).GetRadius, Caster.positionX, Caster.positionY, Caster.positionZ)
                                Else
                                    PartyTargets = _WS_Spells.GetPartyMembersAroundMe(Caster, SpellEffects(Index).GetRadius)
                                End If
                                For Each PartyTarget As WS_Base.BaseUnit In PartyTargets
                                    If Not TargetsInfected.Contains(PartyTarget) Then TargetsInfected.Add(PartyTarget)
                                Next
                            Case SpellImplicitTargets.TARGET_CHAIN_DAMAGE, SpellImplicitTargets.TARGET_CHAIN_HEAL
                                Dim UsedTargets As New List(Of WS_Base.BaseUnit)
                                Dim TargetUnit As WS_Base.BaseUnit = Nothing
                                If Not TargetsInfected.Contains(Targets.unitTarget) Then TargetsInfected.Add(Targets.unitTarget)
                                UsedTargets.Add(Targets.unitTarget)
                                TargetUnit = Targets.unitTarget

                                If SpellEffects(Index).ChainTarget > 1 Then
                                    For k As Byte = 2 To SpellEffects(Index).ChainTarget
                                        Dim EnemyTargets As List(Of WS_Base.BaseUnit) = _WS_Spells.GetEnemyAroundMe(TargetUnit, 10, Caster)
                                        TargetUnit = Nothing
                                        Dim LowHealth As Single = 1.01
                                        Dim TmpLife As Single = 0
                                        For Each tmpUnit As WS_Base.BaseUnit In EnemyTargets
                                            If UsedTargets.Contains(tmpUnit) = False Then
                                                TmpLife = (tmpUnit.Life.Current / tmpUnit.Life.Maximum)
                                                If TmpLife < LowHealth Then
                                                    LowHealth = TmpLife
                                                    TargetUnit = tmpUnit
                                                End If
                                            End If
                                        Next
                                        If TargetUnit IsNot Nothing Then
                                            If Not TargetsInfected.Contains(TargetUnit) Then TargetsInfected.Add(TargetUnit)
                                            UsedTargets.Add(TargetUnit)
                                        Else
                                            Exit For
                                        End If
                                    Next
                                End If
                            Case SpellImplicitTargets.TARGET_AROUND_CASTER_ENEMY
                                Dim EnemyTargets As List(Of WS_Base.BaseUnit) = _WS_Spells.GetEnemyAroundMe(Caster, SpellEffects(Index).GetRadius, Ref)
                                For Each EnemyTarget As WS_Base.BaseUnit In EnemyTargets
                                    If Not TargetsInfected.Contains(EnemyTarget) Then TargetsInfected.Add(EnemyTarget)
                                Next
                            Case SpellImplicitTargets.TARGET_DYNAMIC_OBJECT
                                If Targets.goTarget IsNot Nothing Then TargetsInfected.Add(Targets.goTarget)
                            Case SpellImplicitTargets.TARGET_INFRONT
                                If (CustomAttributs And SpellAttributesCustom.SPELL_ATTR_CU_CONE_BACK) Then
                                    Dim EnemyTargets As List(Of WS_Base.BaseUnit) = _WS_Spells.GetEnemyInBehindMe(Caster, SpellEffects(Index).GetRadius)
                                    For Each EnemyTarget As WS_Base.BaseUnit In EnemyTargets
                                        If Not TargetsInfected.Contains(EnemyTarget) Then TargetsInfected.Add(EnemyTarget)
                                    Next
                                ElseIf (CustomAttributs And SpellAttributesCustom.SPELL_ATTR_CU_CONE_LINE) Then
                                    'TODO!
                                Else
                                    Dim EnemyTargets As List(Of WS_Base.BaseUnit) = _WS_Spells.GetEnemyInFrontOfMe(Caster, SpellEffects(Index).GetRadius)
                                    For Each EnemyTarget As WS_Base.BaseUnit In EnemyTargets
                                        If Not TargetsInfected.Contains(EnemyTarget) Then TargetsInfected.Add(EnemyTarget)
                                    Next
                                End If
                            Case SpellImplicitTargets.TARGET_BEHIND_VICTIM
                                'TODO: Behind victim? What spells has this really?
                            Case SpellImplicitTargets.TARGET_GAMEOBJECT_AND_ITEM, SpellImplicitTargets.TARGET_SELECTED_GAMEOBJECT
                                If Targets.goTarget IsNot Nothing Then TargetsInfected.Add(Targets.goTarget)
                            Case SpellImplicitTargets.TARGET_SELF, SpellImplicitTargets.TARGET_SELF2, SpellImplicitTargets.TARGET_SELF_FISHING, SpellImplicitTargets.TARGET_MASTER, SpellImplicitTargets.TARGET_DUEL_VS_PLAYER
                                If Not TargetsInfected.Contains(Caster) Then TargetsInfected.Add(Caster)
                            Case SpellImplicitTargets.TARGET_RANDOM_RAID_MEMBER
                                'TODO
                            Case SpellImplicitTargets.TARGET_PET, SpellImplicitTargets.TARGET_MINION
                                'TODO
                            Case SpellImplicitTargets.TARGET_NONCOMBAT_PET
                                If TypeOf Caster Is WS_PlayerData.CharacterObject AndAlso CType(Caster, WS_PlayerData.CharacterObject).NonCombatPet IsNot Nothing Then TargetsInfected.Add(CType(Caster, WS_PlayerData.CharacterObject).NonCombatPet)
                            Case SpellImplicitTargets.TARGET_SINGLE_ENEMY, SpellImplicitTargets.TARGET_SINGLE_FRIEND_2, SpellImplicitTargets.TARGET_SELECTED_FRIEND, SpellImplicitTargets.TARGET_SINGLE_PARTY
                                If Not TargetsInfected.Contains(Targets.unitTarget) Then TargetsInfected.Add(Targets.unitTarget)
                            Case SpellImplicitTargets.TARGET_EFFECT_SELECT
                                'TODO: What is this? Used in warstomp.
                            Case Else
                                If Targets.unitTarget IsNot Nothing Then
                                    If Not TargetsInfected.Contains(Targets.unitTarget) Then TargetsInfected.Add(Targets.unitTarget)
                                Else
                                    If Not TargetsInfected.Contains(Caster) Then TargetsInfected.Add(Caster)
                                End If
                        End Select
                    Next

                    'DONE: If no targets were taken, use our target, or else the caster, but ONLY if spell doesn't have any target specifications
                    If SpellEffects(Index).implicitTargetA = SpellImplicitTargets.TARGET_NOTHING AndAlso SpellEffects(Index).implicitTargetB = SpellImplicitTargets.TARGET_NOTHING Then
                        If TargetsInfected.Count = 0 Then
                            If Targets.unitTarget IsNot Nothing Then
                                If Not TargetsInfected.Contains(Targets.unitTarget) Then TargetsInfected.Add(Targets.unitTarget)
                            Else
                                If Not TargetsInfected.Contains(Caster) Then TargetsInfected.Add(Caster)
                            End If
                        End If
                    End If

                    Return CalculateMisses(Caster, TargetsInfected, SpellEffects(Index))
                Else
                    Return New Dictionary(Of WS_Base.BaseObject, SpellMissInfo)
                End If
            End Function

            Public Function CalculateMisses(ByRef Caster As WS_Base.BaseObject, ByRef Targets As List(Of WS_Base.BaseObject), ByRef SpellEffect As SpellEffect) As Dictionary(Of WS_Base.BaseObject, SpellMissInfo)
                Dim newTargets As New Dictionary(Of WS_Base.BaseObject, SpellMissInfo)

                For Each Target As WS_Base.BaseObject In Targets
                    If (Not Target Is Caster) AndAlso (TypeOf Caster Is WS_Base.BaseUnit) AndAlso (TypeOf Target Is WS_Base.BaseUnit) Then
                        With CType(Target, WS_Base.BaseUnit)
                            If SpellEffect.Mechanic > 0 AndAlso (.MechanicImmunity And (1 << (SpellEffect.Mechanic - 1))) Then 'Immune to mechanic
                                newTargets.Add(Target, SpellMissInfo.SPELL_MISS_IMMUNE2)
                            ElseIf Not IsNegative Then 'Positive spells can't miss
                                newTargets.Add(Target, SpellMissInfo.SPELL_MISS_NONE)
                            ElseIf (AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_UNAFFECTED_BY_SCHOOL_IMMUNE) = 0 AndAlso (.SchoolImmunity And (1 << School)) Then 'Immune to school
                                newTargets.Add(Target, SpellMissInfo.SPELL_MISS_IMMUNE2)
                            ElseIf (TypeOf Target Is WS_Creatures.CreatureObject) AndAlso CType(Target, WS_Creatures.CreatureObject).Evade Then 'Creature is evading
                                newTargets.Add(Target, SpellMissInfo.SPELL_MISS_EVADE)
                            ElseIf (TypeOf Caster Is WS_PlayerData.CharacterObject) AndAlso CType(Caster, WS_PlayerData.CharacterObject).GM Then 'Only GM itself can cast on himself when in GM mode
                                'Don't even show up in misses
                            Else
                                Select Case DamageType
                                    Case SpellDamageType.SPELL_DMG_TYPE_NONE
                                        newTargets.Add(Target, SpellMissInfo.SPELL_MISS_NONE)
                                    Case SpellDamageType.SPELL_DMG_TYPE_MAGIC
                                        newTargets.Add(Target, .GetMagicSpellHitResult(Caster, Me))
                                    Case SpellDamageType.SPELL_DMG_TYPE_MELEE, SpellDamageType.SPELL_DMG_TYPE_RANGED
                                        newTargets.Add(Target, .GetMeleeSpellHitResult(Caster, Me))
                                End Select
                            End If
                        End With
                    Else
                        'DONE: Only units can be missed
                        newTargets.Add(Target, SpellMissInfo.SPELL_MISS_NONE)
                    End If
                Next

                Return newTargets
            End Function

            Public Function GetHits(ByRef Targets As Dictionary(Of WS_Base.BaseObject, SpellMissInfo)) As List(Of WS_Base.BaseObject)
                Dim targetHits As New List(Of WS_Base.BaseObject)
                For Each Target As KeyValuePair(Of WS_Base.BaseObject, SpellMissInfo) In Targets
                    If Target.Value = SpellMissInfo.SPELL_MISS_NONE Then
                        targetHits.Add(Target.Key)
                    End If
                Next
                Return targetHits
            End Function

            Public Sub InitCustomAttributes()
                'SpellAttributesCustom
                CustomAttributs = 0UI

                Dim auraSpell As Boolean = True
                For i As Integer = 0 To 2
                    If SpellEffects(i) IsNot Nothing AndAlso SpellEffects(i).ID <> SpellEffects_Names.SPELL_EFFECT_APPLY_AURA Then
                        auraSpell = False
                        Exit For
                    End If
                Next
                If auraSpell Then CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_AURA_SPELL

                If SpellFamilyName = SpellFamilyNames.SPELLFAMILY_PALADIN AndAlso (SpellFamilyFlags And &HC0000000) Then
                    If SpellEffects(0) IsNot Nothing Then SpellEffects(0).ID = SpellEffects_Names.SPELL_EFFECT_HEAL
                End If

                For i As Integer = 0 To 2
                    If SpellEffects(i) IsNot Nothing Then
                        Select Case SpellEffects(i).ApplyAuraIndex
                            Case AuraEffects_Names.SPELL_AURA_PERIODIC_DAMAGE, AuraEffects_Names.SPELL_AURA_PERIODIC_DAMAGE_PERCENT, AuraEffects_Names.SPELL_AURA_PERIODIC_LEECH
                                CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_AURA_DOT
                            Case AuraEffects_Names.SPELL_AURA_PERIODIC_HEAL, AuraEffects_Names.SPELL_AURA_OBS_MOD_HEALTH
                                CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_AURA_HOT
                            Case AuraEffects_Names.SPELL_AURA_MOD_ROOT
                                CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_AURA_CC Or SpellAttributesCustom.SPELL_ATTR_CU_MOVEMENT_IMPAIR
                            Case AuraEffects_Names.SPELL_AURA_MOD_DECREASE_SPEED
                                CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_MOVEMENT_IMPAIR
                        End Select

                        Select Case SpellEffects(i).ID
                            Case SpellEffects_Names.SPELL_EFFECT_SCHOOL_DAMAGE, SpellEffects_Names.SPELL_EFFECT_WEAPON_DAMAGE, SpellEffects_Names.SPELL_EFFECT_WEAPON_DAMAGE_NOSCHOOL, SpellEffects_Names.SPELL_EFFECT_WEAPON_PERCENT_DAMAGE, SpellEffects_Names.SPELL_EFFECT_HEAL
                                CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_DIRECT_DAMAGE
                            Case SpellEffects_Names.SPELL_EFFECT_CHARGE
                                If Speed = 0.0F AndAlso SpellFamilyName = 0 Then
                                    Speed = 42.0F 'Charge default speed
                                End If
                                CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_CHARGE
                        End Select
                    End If
                Next

                For i As Integer = 0 To 2
                    If SpellEffects(i) IsNot Nothing Then
                        Select Case SpellEffects(i).ApplyAuraIndex
                            Case AuraEffects_Names.SPELL_AURA_MOD_POSSESS, AuraEffects_Names.SPELL_AURA_MOD_CONFUSE, AuraEffects_Names.SPELL_AURA_MOD_CHARM, AuraEffects_Names.SPELL_AURA_MOD_FEAR, AuraEffects_Names.SPELL_AURA_MOD_STUN
                                CustomAttributs = (CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_AURA_CC) And (Not SpellAttributesCustom.SPELL_ATTR_CU_MOVEMENT_IMPAIR)
                        End Select
                    End If
                Next

                If SpellVisual = 3879 Then
                    CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_CONE_BACK
                End If

                Select Case ID
                    Case 26029 'Dark Glare
                        CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_CONE_LINE
                    Case 24340, 26558, 28884, 26789 'Meteor
                        CustomAttributs = CustomAttributs Or SpellAttributesCustom.SPELL_ATTR_CU_SHARE_DAMAGE
                    Case 8122, 8124, 10888, 10890, 12494 'Psychic Scream, Frostbite
                        Attributes = Attributes Or SpellAttributes.SPELL_ATTR_BREAKABLE_BY_DAMAGE
                End Select
            End Sub

            Public Sub Cast(ByRef castParams As CastSpellParameters)
                Try
                    Dim Caster As WS_Base.BaseObject = castParams.Caster
                    Dim Targets As SpellTargets = castParams.Targets

                    Dim CastFlags As Short = 2
                    If IsRanged Then CastFlags = CastFlags Or SpellCastFlags.CAST_FLAG_RANGED 'Ranged

                    Dim spellStart As New Packets.PacketClass(OPCODES.SMSG_SPELL_START)
                    'SpellCaster (If the spell is casted by a item, then it's the item guid here, else caster guid)
                    If castParams.Item IsNot Nothing Then
                        spellStart.AddPackGUID(castParams.Item.GUID)
                    Else
                        spellStart.AddPackGUID(castParams.Caster.GUID)
                    End If

                    spellStart.AddPackGUID(castParams.Caster.GUID) 'SpellCaster
                    spellStart.AddInt32(ID)
                    spellStart.AddInt16(CastFlags)
                    If castParams.InstantCast Then
                        spellStart.AddInt32(0)
                    Else
                        spellStart.AddInt32(GetCastTime)
                    End If
                    Targets.WriteTargets(spellStart)

                    'DONE: Write ammo to packet
                    If (CastFlags And SpellCastFlags.CAST_FLAG_RANGED) Then
                        WriteAmmoToPacket(spellStart, Caster)
                    End If

                    Caster.SendToNearPlayers(spellStart)
                    spellStart.Dispose()

                    'PREPEARING SPELL
                    castParams.State = SpellCastState.SPELL_STATE_PREPARING
                    castParams.Stopped = False
                    If TypeOf Caster Is WS_Creatures.CreatureObject Then
                        If Targets.unitTarget IsNot Nothing Then
                            CType(Caster, WS_Creatures.CreatureObject).TurnTo((Targets.unitTarget))
                        ElseIf (Targets.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
                            CType(Caster, WS_Creatures.CreatureObject).TurnTo(Targets.dstX, Targets.dstY)
                        End If
                    End If

                    'DONE: Log spell
                    Dim NeedSpellLog As Boolean = True
                    For i As Byte = 0 To 2
                        If SpellEffects(i) IsNot Nothing Then
                            If SpellEffects(i).ID = SpellEffects_Names.SPELL_EFFECT_SCHOOL_DAMAGE Then NeedSpellLog = False
                        End If
                    Next
                    If NeedSpellLog Then SendSpellLog(Caster, Targets)

                    'DONE: Wait for the castingtime
                    If castParams.InstantCast = False AndAlso GetCastTime > 0 Then
                        Thread.Sleep(GetCastTime)
                        'DONE: Delayed spells
                        Do While castParams.Delayed > 0
                            Dim delayTime As Integer = castParams.Delayed
                            castParams.Delayed = 0
                            Thread.Sleep(delayTime)
                        Loop
                    End If

                    If castParams.Stopped OrElse castParams.State <> SpellCastState.SPELL_STATE_PREPARING Then
                        'Has been interrupted, please abort
                        castParams.Dispose() 'Clean up when we don't need it anymore
                        Exit Sub
                    End If

                    'CASTING SPELL
                    castParams.State = SpellCastState.SPELL_STATE_CASTING

                    'DONE: Calculate the time it takes until the spell is at the target
                    Dim SpellTime As Integer = 0
                    Dim SpellDistance As Single = 0
                    If Speed > 0 Then
                        If (Targets.targetMask And SpellCastTargetFlags.TARGET_FLAG_UNIT) AndAlso Targets.unitTarget IsNot Nothing Then SpellDistance = _WS_Combat.GetDistance(Caster, Targets.unitTarget)
                        If (Targets.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) AndAlso (Targets.dstX <> 0 OrElse Targets.dstY <> 0 OrElse Targets.dstZ <> 0) Then SpellDistance = _WS_Combat.GetDistance(Caster, Targets.dstX, Targets.dstY, Targets.dstZ)
                        If (Targets.targetMask And SpellCastTargetFlags.TARGET_FLAG_OBJECT) AndAlso Targets.goTarget IsNot Nothing Then SpellDistance = _WS_Combat.GetDistance(Caster, Targets.goTarget)
                        If SpellDistance > 0 Then SpellTime = Fix(SpellDistance / Speed * 1000)
                    End If

                    'DONE: Do one more control to see if you still can cast the spell (only if it's not instant)
                    Dim SpellCastError As SpellFailedReason = SpellFailedReason.SPELL_NO_ERROR
                    If (castParams.InstantCast = False OrElse GetCastTime = 0) AndAlso TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        SpellCastError = CanCast(CType(Caster, WS_PlayerData.CharacterObject), Targets, False)
                        If SpellCastError <> SpellFailedReason.SPELL_NO_ERROR Then
                            _WS_Spells.SendCastResult(SpellCastError, CType(Caster, WS_PlayerData.CharacterObject).client, ID)
                            castParams.State = SpellCastState.SPELL_STATE_IDLE
                            castParams.Dispose() 'Clean up when we don't need it anymore
                            Exit Sub
                        End If
                    End If

                    'DONE: Get the targets
                    Dim TargetsInfected(0 To 2) As Dictionary(Of WS_Base.BaseObject, SpellMissInfo)
                    TargetsInfected(0) = GetTargets(Caster, Targets, 0)
                    TargetsInfected(1) = GetTargets(Caster, Targets, 1)
                    TargetsInfected(2) = GetTargets(Caster, Targets, 2)

                    'DONE: On next attack
                    If (Attributes And SpellAttributes.SPELL_ATTR_NEXT_ATTACK) OrElse (Attributes And SpellAttributes.SPELL_ATTR_NEXT_ATTACK2) Then
                        If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                            If CType(Caster, WS_PlayerData.CharacterObject).attackState.combatNextAttackSpell Then
                                _WS_Spells.SendCastResult(SpellFailedReason.SPELL_FAILED_SPELL_IN_PROGRESS, CType(Caster, WS_PlayerData.CharacterObject).client, ID)
                                castParams.Dispose() 'Clean up when we don't need it anymore
                                Exit Sub
                            End If

                            CType(Caster, WS_PlayerData.CharacterObject).attackState.combatNextAttackSpell = True
                            CType(Caster, WS_PlayerData.CharacterObject).attackState.combatNextAttack.WaitOne()
                        End If
                    End If

                    'Send cooldown, Drain power and reagents
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        With CType(Caster, WS_PlayerData.CharacterObject)
                            'DONE: Spell cooldown
                            SendSpellCooldown(CType(Caster, WS_PlayerData.CharacterObject))

                            'DONE: Get reagents
                            For i As Byte = 0 To 7
                                If Reagents(i) AndAlso ReagentsCount(i) Then
                                    .ItemCONSUME(Reagents(i), ReagentsCount(i))
                                End If
                            Next i

                            'DONE: Get arrows for ranged spells
                            If IsRanged Then
                                If .AmmoID > 0 Then .ItemCONSUME(.AmmoID, 1)
                            End If

                            'DONE: Get mana
                            Select Case powerType
                                Case ManaTypes.TYPE_MANA
                                    'DONE: Drain all power for some spells
                                    Dim ManaCost As Integer = 0
                                    If (AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_DRAIN_ALL_POWER) Then
                                        .Mana.Current = 0
                                        ManaCost = 1 'To add the 5 second rule :)
                                    Else
                                        ManaCost = GetManaCost(.Level, .Mana.Base)
                                        .Mana.Current -= ManaCost
                                    End If
                                    'DONE: 5 second rule
                                    If ManaCost > 0 Then
                                        .spellCastManaRegeneration = 5
                                        .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, .Mana.Current)
                                        .GroupUpdateFlag = .GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_POWER
                                        .SendCharacterUpdate()
                                    End If

                                Case ManaTypes.TYPE_RAGE
                                    'DONE: Drain all power for some spells
                                    If (AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_DRAIN_ALL_POWER) Then
                                        .Rage.Current = 0
                                    Else
                                        .Rage.Current -= GetManaCost(.Level, .Rage.Base)
                                    End If
                                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER2, .Rage.Current)
                                    .GroupUpdateFlag = .GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_POWER
                                    .SendCharacterUpdate()

                                Case ManaTypes.TYPE_HEALTH
                                    'DONE: Drain all power for some spells
                                    'TODO: If there are spells using it, should you die or end up with 1 hp?
                                    If (AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_DRAIN_ALL_POWER) Then
                                        .Life.Current = 1
                                    Else
                                        .Life.Current -= GetManaCost(.Level, .Life.Base)
                                    End If
                                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, .Life.Current)
                                    .GroupUpdateFlag = .GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_HP
                                    .SendCharacterUpdate()

                                Case ManaTypes.TYPE_ENERGY
                                    'DONE: Drain all power for some spells
                                    If (AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_DRAIN_ALL_POWER) Then
                                        .Energy.Current = 0
                                    Else
                                        .Energy.Current -= GetManaCost(.Level, .Energy.Base)
                                    End If
                                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER4, .Energy.Current)
                                    .GroupUpdateFlag = .GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_POWER
                                    .SendCharacterUpdate()

                            End Select

                            'DONE: Remove auras when casting a spell
                            .RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_CAST_SPELL)

                            'DONE: Check if the spell was needed for a quest
                            If Not (Targets.unitTarget Is Nothing) AndAlso TypeOf Targets.unitTarget Is WS_Creatures.CreatureObject Then
                                _WorldServer.ALLQUESTS.OnQuestCastSpell(CType(Caster, WS_PlayerData.CharacterObject), CType(Targets.unitTarget, WS_Creatures.CreatureObject), ID)
                            End If
                            If Not (Targets.goTarget Is Nothing) Then
                                _WorldServer.ALLQUESTS.OnQuestCastSpell(CType(Caster, WS_PlayerData.CharacterObject), CType(Targets.goTarget, WS_GameObjects.GameObjectObject), ID)
                            End If
                        End With

                    ElseIf TypeOf Caster Is WS_Creatures.CreatureObject Then
                        With CType(Caster, WS_Creatures.CreatureObject)
                            'DONE: Get mana from creatures
                            Select Case powerType
                                Case ManaTypes.TYPE_MANA
                                    .Mana.Current -= GetManaCost(.Level, .Mana.Base)

                                    'DONE: Send the update
                                    Dim updatePacket As New Packets.UpdatePacketClass
                                    Dim powerUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                                    powerUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, .Mana.Current)
                                    powerUpdate.AddToPacket(updatePacket, ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_Creatures.CreatureObject))
                                    .SendToNearPlayers(updatePacket)
                                    powerUpdate.Dispose()

                                Case ManaTypes.TYPE_HEALTH
                                    .Life.Current -= GetManaCost(.Level, .Life.Base)

                                    'DONE: Send the update
                                    Dim updatePacket As New Packets.UpdatePacketClass
                                    Dim powerUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                                    powerUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, .Life.Current)
                                    powerUpdate.AddToPacket(updatePacket, ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_Creatures.CreatureObject))
                                    .SendToNearPlayers(updatePacket)
                                    powerUpdate.Dispose()
                            End Select
                        End With
                    End If

                    castParams.State = SpellCastState.SPELL_STATE_FINISHED

                    'DONE: Send the cast result
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then _WS_Spells.SendCastResult(SpellFailedReason.SPELL_NO_ERROR, CType(Caster, WS_PlayerData.CharacterObject).client, ID)

                    'DONE: Send the GO message
                    Dim tmpTargets As New Dictionary(Of ULong, SpellMissInfo)
                    For i As Byte = 0 To 2
                        For Each tmpTarget As KeyValuePair(Of WS_Base.BaseObject, SpellMissInfo) In TargetsInfected(i)
                            If Not tmpTargets.ContainsKey(tmpTarget.Key.GUID) Then tmpTargets.Add(tmpTarget.Key.GUID, tmpTarget.Value)
                        Next
                    Next
                    SendSpellGO(Caster, Targets, tmpTargets, castParams.Item)

                    'DONE: Start channel if it's a channeling spell
                    If channelInterruptFlags <> 0 Then
                        castParams.State = SpellCastState.SPELL_STATE_CASTING

                        StartChannel(Caster, GetDuration, Targets)
                    End If

                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        If castParams.Item IsNot Nothing Then
                            'DONE: If this spell use charges then reduce by one when we cast this spell
                            If castParams.Item.ChargesLeft > 0 Then
                                castParams.Item.ChargesLeft -= 1
                                CType(Caster, WS_PlayerData.CharacterObject).SendItemUpdate(castParams.Item)
                            End If

                            'DONE: Consume the item
                            If castParams.Item.ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONSUMABLE Then
                                castParams.Item.StackCount -= 1
                                If castParams.Item.StackCount <= 0 Then
                                    Dim bag As Byte, slot As Byte
                                    slot = CType(Caster, WS_PlayerData.CharacterObject).ItemGetSLOTBAG(castParams.Item.GUID, bag)
                                    If bag <> _Global_Constants.ITEM_SLOT_NULL And slot <> _Global_Constants.ITEM_SLOT_NULL Then
                                        CType(Caster, WS_PlayerData.CharacterObject).ItemREMOVE(bag, slot, True, True)
                                    End If
                                Else
                                    CType(Caster, WS_PlayerData.CharacterObject).SendItemUpdate(castParams.Item)
                                End If
                            End If
                        End If
                    End If
                    If castParams.State = SpellCastState.SPELL_STATE_FINISHED Then castParams.State = SpellCastState.SPELL_STATE_IDLE

                    If SpellTime > 0 Then Thread.Sleep(SpellTime)

                    'APPLYING EFFECTS
                    Dim TargetHits(2) As List(Of WS_Base.BaseObject)
                    TargetHits(0) = GetHits(TargetsInfected(0))
                    TargetHits(1) = GetHits(TargetsInfected(1))
                    TargetHits(2) = GetHits(TargetsInfected(2))
                    For i As Byte = 0 To 2
                        If SpellEffects(i) IsNot Nothing Then
#If DEBUG Then
                            _WorldServer.Log.WriteLine(LogType.DEBUG, "DEBUG: Casting effect: {0}", SpellEffects(i).ID)
#End If

                            SpellCastError = _WS_Spells.SPELL_EFFECTs(SpellEffects(i).ID).Invoke(Targets, Caster, SpellEffects(i), ID, TargetHits(i), castParams.Item)
                            If SpellCastError <> SpellFailedReason.SPELL_NO_ERROR Then Exit For
                        End If
                    Next

                    If SpellCastError <> SpellFailedReason.SPELL_NO_ERROR Then
                        If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                            _WS_Spells.SendCastResult(SpellCastError, CType(Caster, WS_PlayerData.CharacterObject).client, ID)
                            SendInterrupted(0, Caster)
                            castParams.Dispose() 'Clean up when we don't need it anymore
                            Exit Sub
                        Else
                            SendInterrupted(0, Caster)
                            castParams.Dispose() 'Clean up when we don't need it anymore
                            Exit Sub
                        End If
                    End If

                    'DONE: If channel, wait for the duration before we finish it
                    If castParams.State = SpellCastState.SPELL_STATE_CASTING Then
                        If channelInterruptFlags <> 0 Then
                            Thread.Sleep(GetDuration)
                            castParams.State = SpellCastState.SPELL_STATE_IDLE
                        End If
                    End If
                Catch e As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "Error when casting spell.{0}", Environment.NewLine & e.ToString)
                End Try
                If castParams IsNot Nothing Then
                    Try
                        castParams.Dispose() 'Clean up when we don't need it anymore
                    Catch
                    End Try
                End If
            End Sub

            Public Sub Apply(ByRef caster As WS_Base.BaseObject, ByVal Targets As SpellTargets)
                Dim TargetsInfected(0 To 2) As List(Of WS_Base.BaseObject)
                TargetsInfected(0) = GetHits(GetTargets(caster, Targets, 0))
                TargetsInfected(1) = GetHits(GetTargets(caster, Targets, 1))
                TargetsInfected(2) = GetHits(GetTargets(caster, Targets, 2))

                If SpellEffects(0) IsNot Nothing Then _WS_Spells.SPELL_EFFECTs(SpellEffects(0).ID).Invoke(Targets, caster, SpellEffects(0), ID, TargetsInfected(0), Nothing)
                If SpellEffects(1) IsNot Nothing Then _WS_Spells.SPELL_EFFECTs(SpellEffects(1).ID).Invoke(Targets, caster, SpellEffects(1), ID, TargetsInfected(1), Nothing)
                If SpellEffects(2) IsNot Nothing Then _WS_Spells.SPELL_EFFECTs(SpellEffects(2).ID).Invoke(Targets, caster, SpellEffects(2), ID, TargetsInfected(2), Nothing)
            End Sub

            Public Function CanCast(ByRef Character As WS_PlayerData.CharacterObject, ByVal Targets As SpellTargets, ByVal FirstCheck As Boolean) As SpellFailedReason
                If Character.Spell_Silenced Then Return SpellFailedReason.SPELL_FAILED_SILENCED
                If (Character.cUnitFlags And UnitFlags.UNIT_FLAG_TAXI_FLIGHT) Then Return SpellFailedReason.SPELL_FAILED_ERROR
                If (Targets.unitTarget IsNot Nothing) AndAlso (Not Targets.unitTarget Is Character) Then
                    If (FacingCasterFlags And 1) Then
                        If _WS_Combat.IsInFrontOf(Character, (Targets.unitTarget)) = False Then Return SpellFailedReason.SPELL_FAILED_NOT_INFRONT
                    End If
                    If (FacingCasterFlags And 2) Then
                        If _WS_Combat.IsInBackOf(Character, (Targets.unitTarget)) = False Then Return SpellFailedReason.SPELL_FAILED_NOT_BEHIND
                    End If
                End If

                If Targets.unitTarget IsNot Nothing AndAlso (Not Targets.unitTarget Is Character) AndAlso (TypeOf Targets.unitTarget Is WS_PlayerData.CharacterObject) AndAlso CType(Targets.unitTarget, WS_PlayerData.CharacterObject).GM Then Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS

                If (Attributes And SpellAttributes.SPELL_ATTR_WHILE_DEAD) = 0 AndAlso Character.IsDead Then Return SpellFailedReason.SPELL_FAILED_CASTER_DEAD
                If (Attributes And SpellAttributes.SPELL_ATTR_NOT_WHILE_COMBAT) Then
                    If Character.IsInCombat Then Return SpellFailedReason.SPELL_FAILED_INTERRUPTED_COMBAT
                End If

                Dim StanceMask As Integer = 0
                If Character.ShapeshiftForm > ShapeshiftForm.FORM_NORMAL Then StanceMask = 1 << (Character.ShapeshiftForm - 1)
                If (StanceMask And ShapeshiftExclude) Then Return SpellFailedReason.SPELL_FAILED_NOT_SHAPESHIFT
                If (StanceMask And RequredCasterStance) = 0 Then
                    Dim actAsShifted As Boolean = False
                    If Character.ShapeshiftForm > ShapeshiftForm.FORM_NORMAL Then
                        Dim ShapeShift As WS_DBCDatabase.TSpellShapeshiftForm = _WS_DBCDatabase.FindShapeshiftForm(Character.ShapeshiftForm)
                        If ShapeShift IsNot Nothing Then
                            If (ShapeShift.Flags1 And 1) = 0 Then
                                actAsShifted = True
                            Else
                                actAsShifted = False
                            End If
                        Else
                            GoTo SkipShapeShift
                        End If
                    End If
                    If actAsShifted Then
                        If (Attributes And SpellAttributes.SPELL_ATTR_NOT_WHILE_SHAPESHIFTED) Then
                            Return SpellFailedReason.SPELL_FAILED_ONLY_SHAPESHIFT
                        ElseIf RequredCasterStance <> 0 Then
                            Return SpellFailedReason.SPELL_FAILED_ONLY_SHAPESHIFT
                        End If
                    Else
                        If (AttributesEx2 And SpellAttributesEx2.SPELL_ATTR_EX2_NOT_NEED_SHAPESHIFT) = 0 AndAlso RequredCasterStance <> 0 Then
                            Return SpellFailedReason.SPELL_FAILED_ONLY_SHAPESHIFT
                        End If
                    End If
                End If

SkipShapeShift:
                If (Attributes And SpellAttributes.SPELL_ATTR_REQ_STEALTH) And Character.Invisibility <> InvisibilityLevel.STEALTH Then
                    Return SpellFailedReason.SPELL_FAILED_ONLY_STEALTHED
                End If

                If (Character.charMovementFlags And _Global_Constants.movementFlagsMask) Then
                    If ((Character.charMovementFlags And MovementFlags.MOVEMENTFLAG_FALLING) = 0 OrElse SpellEffects(0).ID <> SpellEffects_Names.SPELL_EFFECT_STUCK) AndAlso (IsAutoRepeat OrElse (auraInterruptFlags And SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_NOT_SEATED)) Then
                        Return SpellFailedReason.SPELL_FAILED_MOVING
                    End If
                End If

                Dim ManaCost As Integer = GetManaCost(Character.Level, Character.Mana.Base)
                If ManaCost > 0 Then
                    If powerType <> Character.ManaType Then Return SpellFailedReason.SPELL_FAILED_NO_POWER
                    Select Case powerType
                        Case ManaTypes.TYPE_MANA
                            If ManaCost > Character.Mana.Current Then Return SpellFailedReason.SPELL_FAILED_NO_POWER
                        Case ManaTypes.TYPE_RAGE
                            If ManaCost > Character.Rage.Current Then Return SpellFailedReason.SPELL_FAILED_NO_POWER
                        Case ManaTypes.TYPE_HEALTH
                            If ManaCost > Character.Life.Current Then Return SpellFailedReason.SPELL_FAILED_NO_POWER
                        Case ManaTypes.TYPE_ENERGY
                            If ManaCost > Character.Energy.Current Then Return SpellFailedReason.SPELL_FAILED_NO_POWER
                        Case Else
                            Return SpellFailedReason.SPELL_FAILED_UNKNOWN
                    End Select
                End If

                If Not FirstCheck Then
                    If Mechanic <> 0 AndAlso Targets.unitTarget IsNot Nothing AndAlso (Targets.unitTarget.MechanicImmunity And (1 << (Mechanic - 1))) <> 0 Then Return SpellFailedReason.SPELL_FAILED_IMMUNE
                End If

                If EquippedItemClass > 0 AndAlso EquippedItemSubClass > 0 Then
                    If EquippedItemClass = ITEM_CLASS.ITEM_CLASS_WEAPON Then
                        Dim FoundItem As Boolean = False
                        For i As Integer = ITEM_SUBCLASS.ITEM_SUBCLASS_AXE To ITEM_SUBCLASS.ITEM_SUBCLASS_FISHING_POLE
                            If (EquippedItemSubClass And (1 << i)) Then
                                Select Case i
                                    Case ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_AXE, ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_MACE, ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_SWORD, ITEM_SUBCLASS.ITEM_SUBCLASS_STAFF, ITEM_SUBCLASS.ITEM_SUBCLASS_POLEARM, ITEM_SUBCLASS.ITEM_SUBCLASS_SPEAR, ITEM_SUBCLASS.ITEM_SUBCLASS_FISHING_POLE
                                        If Character.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).IsBroken = False AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).ItemInfo.ObjectClass = EquippedItemClass Then FoundItem = True : Exit For
                                    Case ITEM_SUBCLASS.ITEM_SUBCLASS_AXE, ITEM_SUBCLASS.ITEM_SUBCLASS_MACE, ITEM_SUBCLASS.ITEM_SUBCLASS_SWORD, ITEM_SUBCLASS.ITEM_SUBCLASS_FIST_WEAPON, ITEM_SUBCLASS.ITEM_SUBCLASS_DAGGER
                                        If Character.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).IsBroken = False AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).ItemInfo.ObjectClass = EquippedItemClass Then FoundItem = True : Exit For
                                        If Character.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).IsBroken = False AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).ItemInfo.ObjectClass = EquippedItemClass Then FoundItem = True : Exit For
                                    Case ITEM_SUBCLASS.ITEM_SUBCLASS_BOW, ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW, ITEM_SUBCLASS.ITEM_SUBCLASS_GUN, ITEM_SUBCLASS.ITEM_SUBCLASS_WAND, ITEM_SUBCLASS.ITEM_SUBCLASS_THROWN
                                        If Character.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).IsBroken = False AndAlso Character.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.ObjectClass = EquippedItemClass Then
                                            If i = ITEM_SUBCLASS.ITEM_SUBCLASS_BOW OrElse i = ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW OrElse i = ITEM_SUBCLASS.ITEM_SUBCLASS_GUN Then
                                                If Character.AmmoID = 0 Then Return SpellFailedReason.SPELL_FAILED_NO_AMMO
                                                If Character.ItemCOUNT(Character.AmmoID) = 0 Then Return SpellFailedReason.SPELL_FAILED_NO_AMMO
                                            ElseIf i = ITEM_SUBCLASS.ITEM_SUBCLASS_THROWN Then
                                                If Character.ItemCOUNT(Character.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemEntry) Then Return SpellFailedReason.SPELL_FAILED_NO_AMMO
                                            End If
                                            FoundItem = True : Exit For
                                        End If
                                End Select
                            End If
                        Next
                        If FoundItem = False Then Return SpellFailedReason.SPELL_FAILED_EQUIPPED_ITEM
                    End If
                End If

                For i As Byte = 0 To 2
                    If Not SpellEffects(i) Is Nothing Then
                        Select Case SpellEffects(i).ID
                            Case SpellEffects_Names.SPELL_EFFECT_DUMMY
                                If ID = 1648 Then 'Execute
                                    If Targets.unitTarget Is Nothing OrElse Targets.unitTarget.Life.Current > (Targets.unitTarget.Life.Maximum * 0.2) Then Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS
                                End If
                            Case SpellEffects_Names.SPELL_EFFECT_SCHOOL_DAMAGE
                                If SpellVisual = 7250 Then 'Hammer of Wrath
                                    If Targets.unitTarget Is Nothing Then Return SpellFailedReason.SPELL_FAILED_BAD_IMPLICIT_TARGETS
                                    If Targets.unitTarget.Life.Current > (Targets.unitTarget.Life.Maximum * 0.2) Then Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS
                                End If
                            Case SpellEffects_Names.SPELL_EFFECT_CHARGE
                                If Character.IsRooted Then Return SpellFailedReason.SPELL_FAILED_ROOTED
                            Case SpellEffects_Names.SPELL_EFFECT_SUMMON_OBJECT
                                If SpellEffects(i).MiscValue = 35591 Then
                                    Dim selectedX As Single = 0.0F
                                    Dim selectedY As Single = 0.0F
                                    Dim selectedZ As Single = 0.0F
                                    If SpellEffects(i).RadiusIndex > 0 Then
                                        selectedX = Character.positionX + Math.Cos(Character.orientation) * SpellEffects(i).GetRadius
                                        selectedY = Character.positionY + Math.Sin(Character.orientation) * SpellEffects(i).GetRadius
                                    Else
                                        selectedX = Character.positionX + Math.Cos(Character.orientation) * GetRange
                                        selectedY = Character.positionY + Math.Sin(Character.orientation) * GetRange
                                    End If
                                    If _WS_Maps.GetZCoord(selectedX, selectedY, Character.MapID) > _WS_Maps.GetWaterLevel(selectedX, selectedY, Character.MapID) Then Return SpellFailedReason.SPELL_FAILED_NOT_FISHABLE
                                End If
                            Case SpellEffects_Names.SPELL_EFFECT_OPEN_LOCK
                                'TODO: Fix this :P
                        End Select
                    End If
                Next

                'TODO: See if there was some thing that replaced this Spell Failed Reason, since it was not in 1.12
                'For i As Byte = 0 To 7
                '    If Reagents(i) > 0 AndAlso ReagentsCount(i) > 0 Then
                '        If Character.ItemCOUNT(Reagents(i)) < ReagentsCount(i) Then Return SpellFailedReason.SPELL_FAILED_REAGENTS
                '    End If
                'Next

                'TODO: Check for same category - more powerful spell
                'If (Not Targets.unitTarget Is Nothing) Then
                '    For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECTs_VISIBLE - 1
                '        If Not Targets.unitTarget.ActiveSpells(i) Is Nothing Then
                '            If Targets.unitTarget.ActiveSpells(i).SpellID <> 0 AndAlso _
                '                CType(SPELLs(Targets.unitTarget.ActiveSpells(i).SpellID), SpellInfo).Category = Category AndAlso _
                '                CType(SPELLs(Targets.unitTarget.ActiveSpells(i).SpellID), SpellInfo).spellLevel >= spellLevel Then
                '                Return SpellFailedReason.SPELL_FAILED_AURA_BOUNCED
                '            End If
                '        End If
                '    Next
                'End If

                'DONE: Check if the target is out of line of sight
                If Targets.unitTarget IsNot Nothing Then
                    If _WS_Maps.IsInLineOfSight(Character, Targets.unitTarget) = False Then
                        Return SpellFailedReason.SPELL_FAILED_LINE_OF_SIGHT
                    End If
                ElseIf (Targets.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
                    If _WS_Maps.IsInLineOfSight(Character, Targets.dstX, Targets.dstY, Targets.dstZ) = False Then
                        Return SpellFailedReason.SPELL_FAILED_LINE_OF_SIGHT
                    End If
                End If

                Return SpellFailedReason.SPELL_NO_ERROR
            End Function

            Public Sub StartChannel(ByRef Caster As WS_Base.BaseUnit, ByVal Duration As Integer, ByRef Targets As SpellTargets)
                If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                    Dim packet As New Packets.PacketClass(OPCODES.MSG_CHANNEL_START)
                    packet.AddInt32(ID)
                    packet.AddInt32(Duration)
                    CType(Caster, WS_PlayerData.CharacterObject).client.Send(packet)
                    packet.Dispose()
                ElseIf Not TypeOf Caster Is WS_Creatures.CreatureObject Then 'Only characters and creatures can channel spells
                    Exit Sub
                End If

                Dim updatePacket As New Packets.UpdatePacketClass()
                Dim updateBlock As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                updateBlock.SetUpdateFlag(EUnitFields.UNIT_CHANNEL_SPELL, ID)

                'DONE: Let others know what target we channel against
                If Targets.unitTarget IsNot Nothing Then
                    updateBlock.SetUpdateFlag(EUnitFields.UNIT_FIELD_CHANNEL_OBJECT, Targets.unitTarget.GUID)
                End If

                If TypeOf Caster Is WS_Creatures.CreatureObject Then
                    updateBlock.AddToPacket(updatePacket, ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_PlayerData.CharacterObject))
                ElseIf TypeOf Caster Is WS_Creatures.CreatureObject Then
                    updateBlock.AddToPacket(updatePacket, ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_Creatures.CreatureObject))
                End If

                Caster.SendToNearPlayers(updatePacket)
                updatePacket.Dispose()
            End Sub

            Public Sub WriteAmmoToPacket(ByRef Packet As Packets.PacketClass, ByRef Caster As WS_Base.BaseUnit)
                Dim ItemInfo As WS_Items.ItemInfo = Nothing

                If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                    With CType(Caster, WS_PlayerData.CharacterObject)

                        Dim RangedItem As ItemObject = .ItemGET(0, EquipmentSlots.EQUIPMENT_SLOT_RANGED)
                        If RangedItem IsNot Nothing Then
                            If RangedItem.ItemInfo.InventoryType = INVENTORY_TYPES.INVTYPE_THROWN Then
                                ItemInfo = RangedItem.ItemInfo
                            Else
                                If .AmmoID <> 0 AndAlso _WorldServer.ITEMDatabase.ContainsKey(.AmmoID) Then
                                    ItemInfo = _WorldServer.ITEMDatabase(.AmmoID)
                                End If
                            End If
                        End If

                    End With
                End If

                If ItemInfo Is Nothing Then
                    If _WorldServer.ITEMDatabase.ContainsKey(2512) = False Then
                        Dim tmpInfo As New WS_Items.ItemInfo(2512)
                        _WorldServer.ITEMDatabase.Add(2512, tmpInfo)
                    End If
                    ItemInfo = _WorldServer.ITEMDatabase(2512)

                End If

                Packet.AddInt32(ItemInfo.Model) 'Ammo Display ID
                Packet.AddInt32(ItemInfo.InventoryType) 'Ammo Inventory Type
            End Sub

            Public Sub SendInterrupted(ByVal result As Byte, ByRef Caster As WS_Base.BaseUnit)
                If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_SPELL_FAILURE)
                    packet.AddUInt64(Caster.GUID) 'PackGuid?
                    packet.AddInt32(ID)
                    packet.AddInt8(result)
                    CType(Caster, WS_PlayerData.CharacterObject).client.Send(packet)
                    packet.Dispose()
                End If

                Dim packet2 As New Packets.PacketClass(OPCODES.SMSG_SPELL_FAILED_OTHER)
                packet2.AddUInt64(Caster.GUID) 'PackGuid?
                packet2.AddInt32(ID)
                Caster.SendToNearPlayers(packet2)
                packet2.Dispose()
            End Sub

            Public Sub SendSpellGO(ByRef Caster As WS_Base.BaseObject, ByRef Targets As SpellTargets, ByRef InfectedTargets As Dictionary(Of ULong, SpellMissInfo), ByRef Item As ItemObject)
                Dim castFlags As Short = 256
                If IsRanged Then castFlags = castFlags Or SpellCastFlags.CAST_FLAG_RANGED
                If Item IsNot Nothing Then castFlags = castFlags Or SpellCastFlags.CAST_FLAG_ITEM_CASTER
                'TODO: If missed anyone SpellGoFlags.CAST_FLAG_EXTRA_MSG

                Dim hits As Integer = 0
                Dim misses As Integer = 0
                For Each Target As KeyValuePair(Of ULong, SpellMissInfo) In InfectedTargets
                    If Target.Value = SpellMissInfo.SPELL_MISS_NONE Then
                        hits += 1
                    Else
                        misses += 1
                    End If
                Next

                Dim packet As New Packets.PacketClass(OPCODES.SMSG_SPELL_GO)
                'SpellCaster (If the spell is casted by a item, then it's the item guid here, else caster guid)
                If Item IsNot Nothing Then
                    packet.AddPackGUID(Item.GUID)
                Else
                    packet.AddPackGUID(Caster.GUID)
                End If
                packet.AddPackGUID(Caster.GUID)                                 'SpellCaster
                packet.AddInt32(ID)                                             'SpellID
                packet.AddInt16(castFlags)                                      'Flags (&h20 - Ranged, &h100 - Item caster, &h400 - Targets resisted
                packet.AddInt8(hits)                                            'Targets Count
                For Each Target As KeyValuePair(Of ULong, SpellMissInfo) In InfectedTargets
                    If Target.Value = SpellMissInfo.SPELL_MISS_NONE Then
                        packet.AddUInt64(Target.Key)                            'GUID
                    End If
                Next
                packet.AddInt8(misses)                                          'Misses Count
                For Each Target As KeyValuePair(Of ULong, SpellMissInfo) In InfectedTargets
                    If Target.Value <> SpellMissInfo.SPELL_MISS_NONE Then
                        packet.AddUInt64(Target.Key)                            'GUID
                        packet.AddInt8(Target.Value)                            'SpellMissInfo
                    End If
                Next
                Targets.WriteTargets(packet)

                'DONE: Write ammo to packet
                If (castFlags And SpellCastFlags.CAST_FLAG_RANGED) Then
                    WriteAmmoToPacket(packet, Caster)
                End If

                Caster.SendToNearPlayers(packet)
                packet.Dispose()
            End Sub

            Public Sub SendSpellMiss(ByRef Caster As WS_Base.BaseObject, ByRef Target As WS_Base.BaseUnit, ByVal MissInfo As SpellMissInfo)
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_SPELLLOGMISS)
                packet.AddInt32(ID)
                packet.AddUInt64(Caster.GUID)
                packet.AddInt8(0) '0 or 1?
                packet.AddInt32(1) 'Target Count
                packet.AddUInt64(Target.GUID)
                packet.AddInt8(MissInfo)
                Caster.SendToNearPlayers(packet)
                packet.Dispose()
            End Sub

            Public Sub SendSpellLog(ByRef Caster As WS_Base.BaseObject, ByRef Targets As SpellTargets)
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_SPELLLOGEXECUTE)

                If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                    packet.AddPackGUID(Caster.GUID)
                Else
                    If Targets.unitTarget IsNot Nothing Then
                        packet.AddPackGUID(Targets.unitTarget.GUID)
                    Else
                        packet.AddPackGUID(Caster.GUID)
                    End If
                End If
                packet.AddInt32(ID)

                Dim numOfSpellEffects As Integer = 1
                packet.AddInt32(numOfSpellEffects) 'EffectCount

                Dim UnitTargetGUID As ULong = 0
                If Targets.unitTarget IsNot Nothing Then UnitTargetGUID = Targets.unitTarget.GUID
                Dim ItemTargetGUID As ULong = 0
                If Targets.itemTarget IsNot Nothing Then ItemTargetGUID = Targets.itemTarget.GUID

                For i As Integer = 0 To numOfSpellEffects - 1
                    packet.AddInt32(SpellEffects(i).ID) 'EffectID
                    packet.AddInt32(1) 'TargetCount

                    Select Case SpellEffects(i).ID
                        Case SpellEffects_Names.SPELL_EFFECT_MANA_DRAIN
                            packet.AddPackGUID(UnitTargetGUID)
                            packet.AddInt32(0)
                            packet.AddInt32(0)
                            packet.AddSingle(0.0F)
                        Case SpellEffects_Names.SPELL_EFFECT_ADD_EXTRA_ATTACKS
                            packet.AddPackGUID(UnitTargetGUID)
                            packet.AddInt32(0) 'Count?
                        Case SpellEffects_Names.SPELL_EFFECT_INTERRUPT_CAST
                            packet.AddPackGUID(UnitTargetGUID)
                            packet.AddInt32(0) 'SpellID?
                        Case SpellEffects_Names.SPELL_EFFECT_DURABILITY_DAMAGE
                            packet.AddPackGUID(UnitTargetGUID)
                            packet.AddInt32(0)
                            packet.AddInt32(0)
                        Case SpellEffects_Names.SPELL_EFFECT_OPEN_LOCK, SpellEffects_Names.SPELL_EFFECT_OPEN_LOCK_ITEM
                            packet.AddPackGUID(ItemTargetGUID)
                        Case SpellEffects_Names.SPELL_EFFECT_CREATE_ITEM
                            packet.AddInt32(SpellEffects(i).ItemType)
                        Case SpellEffects_Names.SPELL_EFFECT_SUMMON, SpellEffects_Names.SPELL_EFFECT_SUMMON_WILD, SpellEffects_Names.SPELL_EFFECT_SUMMON_GUARDIAN,
                            SpellEffects_Names.SPELL_EFFECT_SUMMON_OBJECT, SpellEffects_Names.SPELL_EFFECT_SUMMON_PET, SpellEffects_Names.SPELL_EFFECT_SUMMON_POSSESSED,
                            SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM, SpellEffects_Names.SPELL_EFFECT_SUMMON_OBJECT_WILD, SpellEffects_Names.SPELL_EFFECT_CREATE_HOUSE,
                            SpellEffects_Names.SPELL_EFFECT_DUEL, SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT1, SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT2,
                            SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT3, SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT4, SpellEffects_Names.SPELL_EFFECT_SUMMON_PHANTASM,
                            SpellEffects_Names.SPELL_EFFECT_SUMMON_CRITTER, SpellEffects_Names.SPELL_EFFECT_SUMMON_OBJECT_SLOT1, SpellEffects_Names.SPELL_EFFECT_SUMMON_OBJECT_SLOT2,
                            SpellEffects_Names.SPELL_EFFECT_SUMMON_OBJECT_SLOT3, SpellEffects_Names.SPELL_EFFECT_SUMMON_OBJECT_SLOT4, SpellEffects_Names.SPELL_EFFECT_SUMMON_DEMON,
                            SpellEffects_Names.SPELL_EFFECT_150

                            If Targets.unitTarget IsNot Nothing Then
                                packet.AddPackGUID(Targets.unitTarget.GUID)
                            ElseIf Targets.itemTarget IsNot Nothing Then
                                packet.AddPackGUID(Targets.itemTarget.GUID)
                            ElseIf Targets.goTarget IsNot Nothing Then
                                packet.AddPackGUID(Targets.goTarget.GUID)
                            Else
                                packet.AddInt8(0)
                            End If
                        Case SpellEffects_Names.SPELL_EFFECT_FEED_PET
                            packet.AddInt32(Targets.itemTarget.ItemEntry)
                        Case SpellEffects_Names.SPELL_EFFECT_DISMISS_PET
                            packet.AddPackGUID(UnitTargetGUID)
                        Case Else
                            Return
                    End Select
                Next

                Caster.SendToNearPlayers(packet)
                packet.Dispose()
            End Sub

            Public Sub SendSpellCooldown(ByRef objCharacter As WS_PlayerData.CharacterObject, Optional ByRef castItem As ItemObject = Nothing)
                If Not objCharacter.Spells.ContainsKey(ID) Then Return 'This is a trigger spell or something, exit
                Dim Recovery As Integer = SpellCooldown
                Dim CatRecovery As Integer = CategoryCooldown

                'DONE: Throw spell uses the equipped ranged item's attackspeed
                If ID = 2764 Then Recovery = objCharacter.AttackTime(WeaponAttackType.RANGED_ATTACK)

                'DONE: Shoot spell uses the equipped wand's attackspeed
                If ID = 5019 Then
                    If objCharacter.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) Then
                        Recovery = objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.Delay
                    End If
                End If

                'DONE: Get the recovery times from the item if the spell misses them
                If CatRecovery = 0 AndAlso Recovery = 0 AndAlso castItem IsNot Nothing Then
                    For i As Integer = 0 To 4
                        If castItem.ItemInfo.Spells(i).SpellID = ID Then
                            Recovery = castItem.ItemInfo.Spells(i).SpellCooldown
                            CatRecovery = castItem.ItemInfo.Spells(i).SpellCategoryCooldown
                            Exit For
                        End If
                    Next
                End If

                If CatRecovery = 0 AndAlso Recovery = 0 Then Exit Sub 'There is no cooldown

                objCharacter.Spells(ID).Cooldown = _Functions.GetTimestamp(Now) + (Recovery \ 1000)
                If castItem IsNot Nothing Then objCharacter.Spells(ID).CooldownItem = castItem.ItemEntry

                'DONE: Save the cooldown, but only if it's really worth saving
                If Recovery > 10000 Then
                    _WorldServer.CharacterDatabase.Update(String.Format("UPDATE characters_spells SET cooldown={2}, cooldownitem={3} WHERE guid = {0} AND spellid = {1};", objCharacter.GUID, ID, objCharacter.Spells(ID).Cooldown, objCharacter.Spells(ID).CooldownItem))
                End If

                'DONE: Send the cooldown
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_SPELL_COOLDOWN)
                packet.AddUInt64(objCharacter.GUID)

                If CatRecovery > 0 Then
                    For Each Spell As KeyValuePair(Of Integer, CharacterSpell) In objCharacter.Spells
                        If _WS_Spells.SPELLs(Spell.Key).Category = Category Then
                            packet.AddInt32(Spell.Key)
                            If Spell.Key <> ID OrElse Recovery = 0 Then
                                packet.AddInt32(CatRecovery)
                            Else
                                packet.AddInt32(Recovery)
                            End If
                        End If
                    Next
                ElseIf Recovery > 0 Then
                    packet.AddInt32(ID)
                    packet.AddInt32(Recovery)
                End If

                objCharacter.client.Send(packet)
                packet.Dispose()

                'DONE: Send item cooldown
                If castItem IsNot Nothing Then
                    packet = New Packets.PacketClass(OPCODES.SMSG_ITEM_COOLDOWN)
                    packet.AddUInt64(castItem.GUID)
                    packet.AddInt32(ID)
                    objCharacter.client.Send(packet)
                    packet.Dispose()
                End If
            End Sub
        End Class

        Public Class SpellEffect
            Public ID As SpellEffects_Names = SpellEffects_Names.SPELL_EFFECT_NOTHING
            Public Spell As SpellInfo

            Public diceSides As Integer = 0
            Public diceBase As Integer = 0
            Public dicePerLevel As Single = 0
            Public valueBase As Integer = 0
            Public valueDie As Integer = 0
            Public valuePerLevel As Integer = 0
            Public valuePerComboPoint As Integer = 0
            Public Mechanic As Integer = 0
            Public implicitTargetA As Integer = 0
            Public implicitTargetB As Integer = 0

            Public RadiusIndex As Integer = 0
            Public ApplyAuraIndex As Integer = 0

            Public Amplitude As Integer = 0
            Public MultipleValue As Integer = 0
            Public ChainTarget As Integer = 0
            Public ItemType As Integer = 0
            Public MiscValue As Integer = 0
            Public TriggerSpell As Integer = 0
            Public DamageMultiplier As Single = 1

            Public Sub New(ByRef Spell As SpellInfo)
                Me.Spell = Spell
            End Sub

            Public ReadOnly Property GetRadius() As Single
                Get
                    If _WS_Spells.SpellRadius.ContainsKey(RadiusIndex) Then Return _WS_Spells.SpellRadius(RadiusIndex)
                    Return 0
                End Get
            End Property

            Public ReadOnly Property GetValue(ByVal Level As Integer, Optional ByVal ComboPoints As Integer = 0) As Integer
                Get
                    Try
                        Return valueBase + (Level * valuePerLevel) + ComboPoints * valuePerComboPoint + _WorldServer.Rnd.Next(1, valueDie + (Level * dicePerLevel))
                    Catch
                        Return valueBase + (Level * valuePerLevel) + ComboPoints * valuePerComboPoint + 1
                    End Try
                End Get
            End Property

            Public ReadOnly Property IsNegative() As Boolean
                Get
                    If ID <> SpellEffects_Names.SPELL_EFFECT_APPLY_AURA Then Return False
                    Select Case ApplyAuraIndex
                        Case AuraEffects_Names.SPELL_AURA_GHOST, AuraEffects_Names.SPELL_AURA_MOD_CONFUSE, AuraEffects_Names.SPELL_AURA_MOD_DECREASE_SPEED, AuraEffects_Names.SPELL_AURA_MOD_FEAR
                            Return True
                        Case AuraEffects_Names.SPELL_AURA_MOD_PACIFY, AuraEffects_Names.SPELL_AURA_MOD_PACIFY_SILENCE, AuraEffects_Names.SPELL_AURA_MOD_POSSESS, AuraEffects_Names.SPELL_AURA_PERIODIC_DAMAGE
                            Return True
                        Case AuraEffects_Names.SPELL_AURA_MOD_POSSESS_PET, AuraEffects_Names.SPELL_AURA_MOD_ROOT, AuraEffects_Names.SPELL_AURA_MOD_SILENCE, AuraEffects_Names.SPELL_AURA_MOD_STUN
                            Return True
                        Case AuraEffects_Names.SPELL_AURA_PERIODIC_DAMAGE_PERCENT, AuraEffects_Names.SPELL_AURA_PERIODIC_LEECH, AuraEffects_Names.SPELL_AURA_PERIODIC_MANA_LEECH, AuraEffects_Names.SPELL_AURA_PROC_TRIGGER_DAMAGE
                            Return True
                        Case AuraEffects_Names.SPELL_AURA_TRANSFORM, AuraEffects_Names.SPELL_AURA_SPLIT_DAMAGE_FLAT, AuraEffects_Names.SPELL_AURA_SPLIT_DAMAGE_PCT, AuraEffects_Names.SPELL_AURA_POWER_BURN_MANA
                            Return True
                        Case AuraEffects_Names.SPELL_AURA_MOD_DAMAGE_DONE, AuraEffects_Names.SPELL_AURA_MOD_STAT, AuraEffects_Names.SPELL_AURA_MOD_PERCENT_STAT, AuraEffects_Names.SPELL_AURA_MOD_TOTAL_STAT_PERCENTAGE
                            If valueBase < 0 Then Return True Else Return False
                        Case Else
                            Return False
                    End Select
                End Get
            End Property

            Public ReadOnly Property IsAOE() As Boolean
                Get
                    For i As Integer = 0 To 1
                        Dim targets As SpellImplicitTargets
                        If i = 0 Then
                            targets = implicitTargetA
                        Else
                            targets = implicitTargetB
                        End If

                        Select Case targets
                            Case SpellImplicitTargets.TARGET_ALL_ENEMY_IN_AREA, SpellImplicitTargets.TARGET_ALL_ENEMY_IN_AREA_INSTANT, SpellImplicitTargets.TARGET_ALL_FRIENDLY_UNITS_AROUND_CASTER,
                                SpellImplicitTargets.TARGET_ALL_FRIENDLY_UNITS_IN_AREA, SpellImplicitTargets.TARGET_ALL_PARTY, SpellImplicitTargets.TARGET_ALL_PARTY_AROUND_CASTER_2,
                                SpellImplicitTargets.TARGET_AREA_EFFECT_ENEMY_CHANNEL, SpellImplicitTargets.TARGET_AREA_EFFECT_SELECTED, SpellImplicitTargets.TARGET_AREAEFFECT_CUSTOM,
                                SpellImplicitTargets.TARGET_AREAEFFECT_PARTY, SpellImplicitTargets.TARGET_AREAEFFECT_PARTY_AND_CLASS, SpellImplicitTargets.TARGET_AROUND_CASTER_ENEMY,
                                SpellImplicitTargets.TARGET_AROUND_CASTER_PARTY, SpellImplicitTargets.TARGET_INFRONT, SpellImplicitTargets.TARGET_TABLE_X_Y_Z_COORDINATES,
                                SpellImplicitTargets.TARGET_BEHIND_VICTIM

                                Return True
                        End Select
                    Next
                    Return False
                End Get
            End Property
        End Class

        Public Class SpellTargets
            Public unitTarget As WS_Base.BaseUnit = Nothing
            Public goTarget As WS_Base.BaseObject = Nothing
            Public corpseTarget As WS_Corpses.CorpseObject = Nothing
            Public itemTarget As ItemObject = Nothing
            Public srcX As Single = 0
            Public srcY As Single = 0
            Public srcZ As Single = 0
            Public dstX As Single = 0
            Public dstY As Single = 0
            Public dstZ As Single = 0
            Public stringTarget As String = ""

            Public targetMask As Integer = 0

            Public Sub ReadTargets(ByRef packet As Packets.PacketClass, ByRef Caster As WS_Base.BaseObject)
                targetMask = packet.GetInt16

                If targetMask = SpellCastTargetFlags.TARGET_FLAG_SELF Then
                    unitTarget = Caster
                    Exit Sub
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_UNIT) Then
                    Dim GUID As ULong = packet.GetPackGuid
                    If _CommonGlobalFunctions.GuidIsCreature(GUID) OrElse _CommonGlobalFunctions.GuidIsPet(GUID) Then
                        unitTarget = _WorldServer.WORLD_CREATUREs(GUID)
                    ElseIf _CommonGlobalFunctions.GuidIsPlayer(GUID) Then
                        unitTarget = _WorldServer.CHARACTERs(GUID)
                    End If
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_OBJECT) Then
                    Dim GUID As ULong = packet.GetPackGuid
                    If _CommonGlobalFunctions.GuidIsGameObject(GUID) Then
                        goTarget = _WorldServer.WORLD_GAMEOBJECTs(GUID)
                    ElseIf _CommonGlobalFunctions.GuidIsDnyamicObject(GUID) Then
                        goTarget = _WorldServer.WORLD_DYNAMICOBJECTs(GUID)
                    End If
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_ITEM) OrElse (targetMask And SpellCastTargetFlags.TARGET_FLAG_TRADE_ITEM) Then
                    Dim GUID As ULong = packet.GetPackGuid
                    If _CommonGlobalFunctions.GuidIsItem(GUID) Then
                        itemTarget = _WorldServer.WORLD_ITEMs(GUID)
                    End If
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_SOURCE_LOCATION) Then
                    srcX = packet.GetFloat
                    srcY = packet.GetFloat
                    srcZ = packet.GetFloat
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
                    dstX = packet.GetFloat
                    dstY = packet.GetFloat
                    dstZ = packet.GetFloat
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_STRING) Then stringTarget = packet.GetString

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_CORPSE) OrElse (targetMask And SpellCastTargetFlags.TARGET_FLAG_PVP_CORPSE) Then
                    Dim GUID As ULong = packet.GetPackGuid
                    If _CommonGlobalFunctions.GuidIsCorpse(GUID) Then
                        corpseTarget = _WorldServer.WORLD_CORPSEOBJECTs(GUID)
                    End If
                End If
            End Sub

            Public Sub WriteTargets(ByRef packet As Packets.PacketClass)
                packet.AddInt16(targetMask)

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_UNIT) Then packet.AddPackGUID(unitTarget.GUID)
                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_OBJECT) Then packet.AddPackGUID(goTarget.GUID)
                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_ITEM) OrElse (targetMask And SpellCastTargetFlags.TARGET_FLAG_TRADE_ITEM) Then packet.AddPackGUID(itemTarget.GUID)

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_SOURCE_LOCATION) Then
                    packet.AddSingle(srcX)
                    packet.AddSingle(srcY)
                    packet.AddSingle(srcZ)
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
                    packet.AddSingle(dstX)
                    packet.AddSingle(dstY)
                    packet.AddSingle(dstZ)
                End If

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_STRING) Then packet.AddString(stringTarget)

                If (targetMask And SpellCastTargetFlags.TARGET_FLAG_CORPSE) OrElse (targetMask And SpellCastTargetFlags.TARGET_FLAG_PVP_CORPSE) Then
                    packet.AddPackGUID(corpseTarget.GUID)
                End If
            End Sub

            Public Sub SetTarget_SELF(ByRef objCharacter As WS_Base.BaseUnit)
                unitTarget = objCharacter
                targetMask += SpellCastTargetFlags.TARGET_FLAG_SELF
            End Sub

            Public Sub SetTarget_UNIT(ByRef objCharacter As WS_Base.BaseUnit)
                unitTarget = objCharacter
                targetMask += SpellCastTargetFlags.TARGET_FLAG_UNIT
            End Sub

            Public Sub SetTarget_OBJECT(ByRef o As WS_Base.BaseObject)
                goTarget = o
                targetMask += SpellCastTargetFlags.TARGET_FLAG_OBJECT
            End Sub

            Public Sub SetTarget_ITEM(ByRef i As ItemObject)
                itemTarget = i
                targetMask += SpellCastTargetFlags.TARGET_FLAG_ITEM
            End Sub

            Public Sub SetTarget_SOURCELOCATION(ByVal x As Single, ByVal y As Single, ByVal z As Single)
                srcX = x
                srcY = y
                srcZ = z
                targetMask += SpellCastTargetFlags.TARGET_FLAG_SOURCE_LOCATION
            End Sub

            Public Sub SetTarget_DESTINATIONLOCATION(ByVal x As Single, ByVal y As Single, ByVal z As Single)
                dstX = x
                dstY = y
                dstZ = z
                targetMask += SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION
            End Sub

            Public Sub SetTarget_STRING(ByVal str As String)
                stringTarget = str
                targetMask += SpellCastTargetFlags.TARGET_FLAG_STRING
            End Sub
        End Class

        Public Class CastSpellParameters
            Implements IDisposable

            Public Targets As SpellTargets
            Public Caster As WS_Base.BaseObject
            Public SpellID As Integer
            Public Item As ItemObject = Nothing
            Public InstantCast As Boolean = False
            Public Delayed As Integer = 0

            Public Stopped As Boolean = False
            Public State As SpellCastState = SpellCastState.SPELL_STATE_IDLE

            Public Sub New(ByRef Targets As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByVal SpellID As Integer)
                Me.Targets = Targets
                Me.Caster = Caster
                Me.SpellID = SpellID
                Item = Nothing
                InstantCast = False
            End Sub

            Public Sub New(ByRef Targets As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByVal SpellID As Integer, ByVal Instant As Boolean)
                Me.Targets = Targets
                Me.Caster = Caster
                Me.SpellID = SpellID
                Item = Nothing
                InstantCast = Instant
            End Sub

            Public Sub New(ByRef Targets As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByVal SpellID As Integer, ByRef Item As ItemObject)
                Me.Targets = Targets
                Me.Caster = Caster
                Me.SpellID = SpellID
                Me.Item = Item
                InstantCast = False
            End Sub

            Public Sub New(ByRef Targets As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByVal SpellID As Integer, ByRef Item As ItemObject, ByVal Instant As Boolean)
                Me.Targets = Targets
                Me.Caster = Caster
                Me.SpellID = SpellID
                Me.Item = Item
                InstantCast = Instant
            End Sub

            Public ReadOnly Property SpellInfo() As SpellInfo
                Get
                    Return _WS_Spells.SPELLs(SpellID)
                End Get
            End Property

            Public ReadOnly Property Finished() As Boolean
                Get
                    Return (Stopped OrElse State = SpellCastState.SPELL_STATE_FINISHED OrElse State = SpellCastState.SPELL_STATE_IDLE)
                End Get
            End Property

            Public Sub Cast(ByVal status As Object)
                Try
                    Stopped = False
                    _WS_Spells.SPELLs(SpellID).Cast(Me)
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "Cast Exception {0} : Interrupted {1}", SpellID, Stopped)
                End Try
            End Sub

            Public Sub StopCast()
                Try
                    Stopped = True
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "StopCast Exception {0} : Interrupted {1}", SpellID, Stopped)
                End Try
            End Sub

            Public Sub Delay()
                If Caster Is Nothing OrElse Finished Then Exit Sub

                'Calculate resist chance
                Dim resistChance As Integer = CType(Caster, WS_Base.BaseUnit).GetAuraModifier(AuraEffects_Names.SPELL_AURA_RESIST_PUSHBACK)
                If resistChance > 0 AndAlso resistChance > _WorldServer.Rnd.Next(0, 100) Then Exit Sub 'Resisted pushback

                'TODO: Calculate delay time?
                Dim delaytime As Integer = 200

                Delayed += delaytime

                Dim packet As New Packets.PacketClass(OPCODES.SMSG_SPELL_DELAYED)
                packet.AddPackGUID(Caster.GUID)
                packet.AddInt32(delaytime)
                Caster.SendToNearPlayers(packet)
                packet.Dispose()
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

        Public Class CharacterSpell
            Public SpellID As Integer
            Public Active As Byte
            Public Cooldown As UInteger
            Public CooldownItem As Integer

            Public Sub New(ByVal SpellID As Integer, ByVal Active As Byte, ByVal Cooldown As UInteger, ByVal CooldownItem As Integer)
                Me.SpellID = SpellID
                Me.Active = Active
                Me.Cooldown = Cooldown
                Me.CooldownItem = CooldownItem
            End Sub
        End Class

        Public Sub SendCastResult(ByVal result As SpellFailedReason, ByRef client As WS_Network.ClientClass, ByVal id As Integer)
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_CAST_RESULT)
            packet.AddInt32(id)
            If result <> SpellFailedReason.SPELL_NO_ERROR Then
                packet.AddInt8(2)
                packet.AddInt8(result)
            Else
                packet.AddInt8(0)
            End If

            client.Send(packet)
            packet.Dispose()
        End Sub

        Public Sub SendNonMeleeDamageLog(ByRef Caster As WS_Base.BaseUnit, ByRef Target As WS_Base.BaseUnit, ByVal SpellID As Integer, ByVal SchoolType As Integer, ByVal Damage As Integer, ByVal Resist As Integer, ByVal Absorbed As Integer, ByVal CriticalHit As Boolean)
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_SPELLNONMELEEDAMAGELOG)
            packet.AddPackGUID(Target.GUID)
            packet.AddPackGUID(Caster.GUID)
            packet.AddInt32(SpellID)
            packet.AddInt32(Damage)
            packet.AddInt8(SchoolType)
            packet.AddInt32(Absorbed)       'AbsorbedDamage
            packet.AddInt32(Resist)         'Resist
            packet.AddInt8(0)               '1=Physical/0=Not Physical
            packet.AddInt8(0)               'Unk
            packet.AddInt32(0)              'Blocked
            If CriticalHit Then
                packet.AddInt8(&H2)
            Else
                packet.AddInt8(&H0)
            End If
            packet.AddInt32(0)               'Unk
            Caster.SendToNearPlayers(packet)
        End Sub

        Public Sub SendHealSpellLog(ByRef Caster As WS_Base.BaseUnit, ByRef Target As WS_Base.BaseUnit, ByVal SpellID As Integer, ByVal Damage As Integer, ByVal CriticalHit As Boolean)
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_HEALSPELL_ON_PLAYER_OBSOLETE)
            packet.AddPackGUID(Target.GUID)
            packet.AddPackGUID(Caster.GUID)
            packet.AddInt32(SpellID)
            packet.AddInt32(Damage)
            If CriticalHit Then
                packet.AddInt8(1)
            Else
                packet.AddInt8(0)
            End If
            Caster.SendToNearPlayers(packet)
        End Sub

        Public Sub SendEnergizeSpellLog(ByRef Caster As WS_Base.BaseUnit, ByRef Target As WS_Base.BaseUnit, ByVal SpellID As Integer, ByVal Damage As Integer, ByVal PowerType As Integer)
            'Dim packet As New PacketClass(OPCODES.SMSG_SPELLENERGIZELOG)
            'packet.AddPackGUID(Target.GUID)
            'packet.AddPackGUID(Caster.GUID)
            'packet.AddInt32(SpellID)
            'packet.AddInt32(PowerType)
            'packet.AddInt32(Damage)
            'Caster.SendToNearPlayers(packet)
            'packet.Dispose()
        End Sub

        Public Sub SendPeriodicAuraLog(ByRef Caster As WS_Base.BaseUnit, ByRef Target As WS_Base.BaseUnit, ByVal SpellID As Integer, ByVal School As Integer, ByVal Damage As Integer, ByVal AuraIndex As Integer)
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_PERIODICAURALOG)
            packet.AddPackGUID(Target.GUID)
            packet.AddPackGUID(Caster.GUID)
            packet.AddInt32(SpellID)
            packet.AddInt32(1)
            packet.AddInt8(AuraIndex)
            packet.AddInt32(Damage)
            packet.AddInt32(School)
            packet.AddInt32(0)
            Caster.SendToNearPlayers(packet)
            packet.Dispose()
        End Sub

        Public Sub SendPlaySpellVisual(ByRef Caster As WS_Base.BaseUnit, ByVal SpellId As Integer)
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_PLAY_SPELL_VISUAL)
            packet.AddUInt64(Caster.GUID)
            packet.AddInt32(SpellId)
            Caster.SendToNearPlayers(packet)
            packet.Dispose()
        End Sub

        Public Sub SendChannelUpdate(ByRef Caster As WS_PlayerData.CharacterObject, ByVal Time As Integer)
            'DONE: Update time for self
            Dim packet As New Packets.PacketClass(OPCODES.MSG_CHANNEL_UPDATE)
            packet.AddInt32(Time)
            Caster.client.Send(packet)
            packet.Dispose()

            If Time = 0 Then
                'DONE: Stop channeling for others
                Caster.SetUpdateFlag(EUnitFields.UNIT_FIELD_CHANNEL_OBJECT, 0L)
                Caster.SetUpdateFlag(EUnitFields.UNIT_CHANNEL_SPELL, 0)
                Caster.SendCharacterUpdate()
            End If
        End Sub

#End Region

#Region "WS.Spells.Database"

        Public SpellChains As New Dictionary(Of Integer, Integer)
        Public SPELLs As New Dictionary(Of Integer, SpellInfo)(29000)

        Public SpellCastTime As New Dictionary(Of Integer, Integer)
        Public SpellRadius As New Dictionary(Of Integer, Single)
        Public SpellRange As New Dictionary(Of Integer, Single)
        Public SpellDuration As New Dictionary(Of Integer, Integer)
        Public SpellFocusObject As New Dictionary(Of Integer, String)

        Public Sub InitializeSpellDB()
            For i As Integer = 0 To SPELL_EFFECT_COUNT
                SPELL_EFFECTs(i) = AddressOf SPELL_EFFECT_NOTHING
            Next

            SPELL_EFFECTs(0) = AddressOf SPELL_EFFECT_NOTHING                   'None
            SPELL_EFFECTs(1) = AddressOf SPELL_EFFECT_INSTAKILL                 'Instakill
            SPELL_EFFECTs(2) = AddressOf SPELL_EFFECT_SCHOOL_DAMAGE             'School Damage
            SPELL_EFFECTs(3) = AddressOf SPELL_EFFECT_DUMMY                     'Dummy
            'SPELL_EFFECTs(4) = AddressOf SPELL_EFFECT_PORTAL_TELEPORT           'Portal Teleport
            SPELL_EFFECTs(5) = AddressOf SPELL_EFFECT_TELEPORT_UNITS            'Teleport Units
            SPELL_EFFECTs(6) = AddressOf SPELL_EFFECT_APPLY_AURA                'Apply Aura
            SPELL_EFFECTs(7) = AddressOf SPELL_EFFECT_ENVIRONMENTAL_DAMAGE      'Environmental Damage
            SPELL_EFFECTs(8) = AddressOf SPELL_EFFECT_MANA_DRAIN                'Power Drain
            'SPELL_EFFECTs(9) = AddressOf SPELL_EFFECT_HEALTH_LEECH              'Health Leech
            SPELL_EFFECTs(10) = AddressOf SPELL_EFFECT_HEAL                     'Heal
            SPELL_EFFECTs(11) = AddressOf SPELL_EFFECT_BIND                     'Bind
            'SPELL_EFFECTs(12) = AddressOf SPELL_EFFECT_PORTAL                   'Portal
            'SPELL_EFFECTs(13) = AddressOf SPELL_EFFECT_RITUAL_BASE              'Ritual Base
            'SPELL_EFFECTs(14) = AddressOf SPELL_EFFECT_RITUAL_SPECIALIZE        'Ritual Specialize
            'SPELL_EFFECTs(15) = AddressOf SPELL_EFFECT_RITUAL_ACTIVATE_PORTAL   'Ritual Activate Portal
            SPELL_EFFECTs(16) = AddressOf SPELL_EFFECT_QUEST_COMPLETE           'Quest Complete
            SPELL_EFFECTs(17) = AddressOf SPELL_EFFECT_WEAPON_DAMAGE_NOSCHOOL   'Weapon Damage + (noschool)
            SPELL_EFFECTs(18) = AddressOf SPELL_EFFECT_RESURRECT                'Resurrect
            '!! SPELL_EFFECTs(19) = AddressOf SPELL_EFFECT_ADD_EXTRA_ATTACKS        'Extra Attacks
            SPELL_EFFECTs(20) = AddressOf SPELL_EFFECT_DODGE                    'Dodge
            SPELL_EFFECTs(21) = AddressOf SPELL_EFFECT_EVADE                    'Evade
            SPELL_EFFECTs(22) = AddressOf SPELL_EFFECT_PARRY                    'Parry
            SPELL_EFFECTs(23) = AddressOf SPELL_EFFECT_BLOCK                    'Block
            SPELL_EFFECTs(24) = AddressOf SPELL_EFFECT_CREATE_ITEM              'Create Item
            'SPELL_EFFECTs(25) = AddressOf SPELL_EFFECT_WEAPON                   'Weapon
            'SPELL_EFFECTs(26) = AddressOf SPELL_EFFECT_DEFENSE                  'Defense
            SPELL_EFFECTs(27) = AddressOf SPELL_EFFECT_PERSISTENT_AREA_AURA     'Persistent Area Aura
            SPELL_EFFECTs(28) = AddressOf SPELL_EFFECT_SUMMON                   'Summon
            SPELL_EFFECTs(29) = AddressOf SPELL_EFFECT_LEAP                     'Leap
            SPELL_EFFECTs(30) = AddressOf SPELL_EFFECT_ENERGIZE                 'Energize
            'SPELL_EFFECTs(31) = AddressOf SPELL_EFFECT_WEAPON_PERCENT_DAMAGE    'Weapon % Dmg
            'SPELL_EFFECTs(32) = AddressOf SPELL_EFFECT_TRIGGER_MISSILE          'Trigger Missile
            SPELL_EFFECTs(33) = AddressOf SPELL_EFFECT_OPEN_LOCK                'Open Lock
            'SPELL_EFFECTs(34) = AddressOf SPELL_EFFECT_SUMMON_MOUNT_OBSOLETE
            SPELL_EFFECTs(35) = AddressOf SPELL_EFFECT_APPLY_AREA_AURA          'Apply Area Aura
            SPELL_EFFECTs(36) = AddressOf SPELL_EFFECT_LEARN_SPELL              'Learn Spell
            'SPELL_EFFECTs(37) = AddressOf SPELL_EFFECT_SPELL_DEFENSE            'Spell Defense
            SPELL_EFFECTs(38) = AddressOf SPELL_EFFECT_DISPEL                   'Dispel
            'SPELL_EFFECTs(39) = AddressOf SPELL_EFFECT_LANGUAGE                 'Language
            SPELL_EFFECTs(40) = AddressOf SPELL_EFFECT_DUAL_WIELD               'Dual Wield
            SPELL_EFFECTs(41) = AddressOf SPELL_EFFECT_SUMMON_WILD          'Summon Wild
            SPELL_EFFECTs(42) = AddressOf SPELL_EFFECT_SUMMON_WILD             'Summon Guardian
            '! SPELL_EFFECTs(43) = AddressOf SPELL_EFFECT_TELEPORT_UNITS_FACE_CASTER
            SPELL_EFFECTs(44) = AddressOf SPELL_EFFECT_SKILL_STEP               'Skill Step
            SPELL_EFFECTs(45) = AddressOf SPELL_EFFECT_HONOR
            'SPELL_EFFECTs(46) = AddressOf SPELL_EFFECT_SPAWN                    'Spawn
            'SPELL_EFFECTs(47) = AddressOf SPELL_EFFECT_TRADE_SKILL              'Spell Cast UI
            SPELL_EFFECTs(48) = AddressOf SPELL_EFFECT_STEALTH                  'Stealth
            SPELL_EFFECTs(49) = AddressOf SPELL_EFFECT_DETECT                   'Detect
            SPELL_EFFECTs(50) = AddressOf SPELL_EFFECT_SUMMON_OBJECT            'Summon Object
            'SPELL_EFFECTs(51) = AddressOf SPELL_EFFECT_FORCE_CRITICAL_HIT       'Force Critical Hit
            'SPELL_EFFECTs(52) = AddressOf SPELL_EFFECT_GUARANTEE_HIT            'Guarantee Hit
            SPELL_EFFECTs(53) = AddressOf SPELL_EFFECT_ENCHANT_ITEM             'Enchant Item Permanent
            SPELL_EFFECTs(54) = AddressOf SPELL_EFFECT_ENCHANT_ITEM_TEMPORARY   'Enchant Item Temporary
            'SPELL_EFFECTs(55) = AddressOf SPELL_EFFECT_TAMECREATURE             'Tame Creature
            'SPELL_EFFECTs(56) = AddressOf SPELL_EFFECT_SUMMON_PET               'Summon Pet
            'SPELL_EFFECTs(57) = AddressOf SPELL_EFFECT_LEARN_PET_SPELL          'Learn Pet Spell
            SPELL_EFFECTs(58) = AddressOf SPELL_EFFECT_WEAPON_DAMAGE            'Weapon Damage +
            SPELL_EFFECTs(59) = AddressOf SPELL_EFFECT_OPEN_LOCK                'Open Lock (Item)
            SPELL_EFFECTs(60) = AddressOf SPELL_EFFECT_PROFICIENCY              'Proficiency
            'SPELL_EFFECTs(61) = AddressOf SPELL_EFFECT_SEND_EVENT               'Send Event
            'SPELL_EFFECTs(62) = AddressOf SPELL_EFFECT_POWER_BURN               'Power Burn
            'SPELL_EFFECTs(63) = AddressOf SPELL_EFFECT_THREAT                   'Threat
            SPELL_EFFECTs(64) = AddressOf SPELL_EFFECT_TRIGGER_SPELL            'Trigger Spell
            'SPELL_EFFECTs(65) = AddressOf SPELL_EFFECT_HEALTH_FUNNEL            'Health Funnel
            'SPELL_EFFECTs(66) = AddressOf SPELL_EFFECT_POWER_FUNNEL             'Power Funnel
            SPELL_EFFECTs(67) = AddressOf SPELL_EFFECT_HEAL_MAX_HEALTH          'Heal Max Health
            SPELL_EFFECTs(68) = AddressOf SPELL_EFFECT_INTERRUPT_CAST           'Interrupt Cast
            'SPELL_EFFECTs(69) = AddressOf SPELL_EFFECT_DISTRACT                 'Distract
            'SPELL_EFFECTs(70) = AddressOf SPELL_EFFECT_PULL                     'Pull
            SPELL_EFFECTs(71) = AddressOf SPELL_EFFECT_PICKPOCKET               'Pickpocket
            'SPELL_EFFECTs(72) = AddressOf SPELL_EFFECT_ADD_FARSIGHT             'Add Farsight
            'SPELL_EFFECTs(73) = AddressOf SPELL_EFFECT_SUMMON_POSSESSED         'Summon Possessed
            SPELL_EFFECTs(74) = AddressOf SPELL_EFFECT_SUMMON_TOTEM             'Summon Totem
            'SPELL_EFFECTs(75) = AddressOf SPELL_EFFECT_HEAL_MECHANICAL          'Heal Mechanical
            'SPELL_EFFECTs(76) = AddressOf SPELL_EFFECT_SUMMON_OBJECT_WILD       'Summon Object (Wild)
            SPELL_EFFECTs(77) = AddressOf SPELL_EFFECT_SCRIPT_EFFECT            'Script Effect
            'SPELL_EFFECTs(78) = AddressOf SPELL_EFFECT_ATTACK                   'Attack
            'SPELL_EFFECTs(79) = AddressOf SPELL_EFFECT_SANCTUARY                'Sanctuary
            'SPELL_EFFECTs(80) = AddressOf SPELL_EFFECT_ADD_COMBO_POINTS         'Add Combo Points
            'SPELL_EFFECTs(81) = AddressOf SPELL_EFFECT_CREATE_HOUSE             'Create House
            'SPELL_EFFECTs(82) = AddressOf SPELL_EFFECT_BIND_SIGHT               'Bind Sight
            SPELL_EFFECTs(83) = AddressOf SPELL_EFFECT_DUEL                     'Duel
            'SPELL_EFFECTs(84) = AddressOf SPELL_EFFECT_STUCK                    'Stuck
            'SPELL_EFFECTs(85) = AddressOf SPELL_EFFECT_SUMMON_PLAYER            'Summon Player
            'SPELL_EFFECTs(86) = AddressOf SPELL_EFFECT_ACTIVATE_OBJECT          'Activate Object
            SPELL_EFFECTs(87) = AddressOf SPELL_EFFECT_SUMMON_TOTEM             'Summon Totem (slot 1)
            SPELL_EFFECTs(88) = AddressOf SPELL_EFFECT_SUMMON_TOTEM             'Summon Totem (slot 2)
            SPELL_EFFECTs(89) = AddressOf SPELL_EFFECT_SUMMON_TOTEM             'Summon Totem (slot 3)
            SPELL_EFFECTs(90) = AddressOf SPELL_EFFECT_SUMMON_TOTEM             'Summon Totem (slot 4)
            'SPELL_EFFECTs(91) = AddressOf SPELL_EFFECT_THREAT_ALL               'Threat (All)
            SPELL_EFFECTs(92) = AddressOf SPELL_EFFECT_ENCHANT_HELD_ITEM        'Enchant Held Item
            'SPELL_EFFECTs(93) = AddressOf SPELL_EFFECT_SUMMON_PHANTASM          'Summon Phantasm
            'SPELL_EFFECTs(94) = AddressOf SPELL_EFFECT_SELF_RESURRECT           'Self Resurrect
            SPELL_EFFECTs(95) = AddressOf SPELL_EFFECT_SKINNING                 'Skinning
            SPELL_EFFECTs(96) = AddressOf SPELL_EFFECT_CHARGE                   'Charge
            'SPELL_EFFECTs(97) = AddressOf SPELL_EFFECT_SUMMON_CRITTER           'Summon Critter
            SPELL_EFFECTs(98) = AddressOf SPELL_EFFECT_KNOCK_BACK               'Knock Back
            SPELL_EFFECTs(99) = AddressOf SPELL_EFFECT_DISENCHANT               'Disenchant
            'SPELL_EFFECTs(100) = AddressOf SPELL_EFFECT_INEBRIATE               'Inebriate
            'SPELL_EFFECTs(101) = AddressOf SPELL_EFFECT_FEED_PET                'Feed Pet
            'SPELL_EFFECTs(102) = AddressOf SPELL_EFFECT_DISMISS_PET             'Dismiss Pet
            'SPELL_EFFECTs(103) = AddressOf SPELL_EFFECT_REPUTATION              'Reputation
            'SPELL_EFFECTs(104) = AddressOf SPELL_EFFECT_SUMMON_OBJECT_SLOT1     'Summon Object (slot 1)
            'SPELL_EFFECTs(105) = AddressOf SPELL_EFFECT_SUMMON_OBJECT_SLOT2     'Summon Object (slot 2)
            'SPELL_EFFECTs(106) = AddressOf SPELL_EFFECT_SUMMON_OBJECT_SLOT3     'Summon Object (slot 3)
            'SPELL_EFFECTs(107) = AddressOf SPELL_EFFECT_SUMMON_OBJECT_SLOT4     'Summon Object (slot 4)
            'SPELL_EFFECTs(108) = AddressOf SPELL_EFFECT_DISPEL_MECHANIC         'Dispel Mechanic
            'SPELL_EFFECTs(109) = AddressOf SPELL_EFFECT_SUMMON_DEAD_PET         'Summon Dead Pet
            'SPELL_EFFECTs(110) = AddressOf SPELL_EFFECT_DESTROY_ALL_TOTEMS      'Destroy All Totems
            'SPELL_EFFECTs(111) = AddressOf SPELL_EFFECT_DURABILITY_DAMAGE       'Durability Damage
            'SPELL_EFFECTs(112) = AddressOf SPELL_EFFECT_SUMMON_DEMON            'Summon Demon
            SPELL_EFFECTs(113) = AddressOf SPELL_EFFECT_RESURRECT_NEW           'Resurrect (Flat)
            'SPELL_EFFECTs(114) = AddressOf SPELL_EFFECT_ATTACK_ME               'Attack Me
            'SPELL_EFFECTs(115) = AddressOf SPELL_EFFECT_DURABILITY_DAMAGE_PCT
            'SPELL_EFFECTs(116) = AddressOf SPELL_EFFECT_SKIN_PLAYER_CORPSE
            'SPELL_EFFECTs(117) = AddressOf SPELL_EFFECT_SPIRIT_HEAL
            'SPELL_EFFECTs(118) = AddressOf SPELL_EFFECT_SKILL
            'SPELL_EFFECTs(119) = AddressOf SPELL_EFFECT_APPLY_AURA_NEW
            SPELL_EFFECTs(120) = AddressOf SPELL_EFFECT_TELEPORT_GRAVEYARD
            SPELL_EFFECTs(121) = AddressOf SPELL_EFFECT_ADICIONAL_DMG
            'SPELL_EFFECTs(122) = AddressOf SPELL_EFFECT_?
            'SPELL_EFFECTs(123) = AddressOf SPELL_EFFECT_TAXI                   'Taxi Flight
            'SPELL_EFFECTs(124) = AddressOf SPELL_EFFECT_PULL_TOWARD            'Pull target towards you
            'SPELL_EFFECTs(125) = AddressOf SPELL_EFFECT_INVISIBILITY_NEW       '
            'SPELL_EFFECTs(126) = AddressOf SPELL_EFFECT_SPELL_STEAL            'Steal benefical effect
            'SPELL_EFFECTs(127) = AddressOf SPELL_EFFECT_PROSPECT               'Search ore for gems
            'SPELL_EFFECTs(128) = AddressOf SPELL_EFFECT_APPLY_AURA_NEW2
            'SPELL_EFFECTs(129) = AddressOf SPELL_EFFECT_APPLY_AURA_NEW3
            'SPELL_EFFECTs(130) = AddressOf SPELL_EFFECT_REDIRECT_THREAT
            'SPELL_EFFECTs(131) = AddressOf SPELL_EFFECT_?
            'SPELL_EFFECTs(132) = AddressOf SPELL_EFFECT_?
            'SPELL_EFFECTs(133) = AddressOf SPELL_EFFECT_FORGET
            'SPELL_EFFECTs(134) = AddressOf SPELL_EFFECT_KILL_CREDIT
            'SPELL_EFFECTs(135) = AddressOf SPELL_EFFECT_SUMMON_PET_NEW
            'SPELL_EFFECTs(136) = AddressOf SPELL_EFFECT_HEAL_PCT
            SPELL_EFFECTs(137) = AddressOf SPELL_EFFECT_ENERGIZE_PCT

            For i As Integer = 0 To AURAs_COUNT
                AURAs(i) = AddressOf SPELL_AURA_NONE
            Next

            AURAs(0) = AddressOf SPELL_AURA_NONE                                            'None
            AURAs(1) = AddressOf SPELL_AURA_BIND_SIGHT                                      'Bind Sight
            AURAs(2) = AddressOf SPELL_AURA_MOD_POSSESS                                     'Mod Possess
            AURAs(3) = AddressOf SPELL_AURA_PERIODIC_DAMAGE                                 'Periodic Damage
            AURAs(4) = AddressOf SPELL_AURA_DUMMY                                           'Dummy
            'AURAs(	5	) = AddressOf 	SPELL_AURA_MOD_CONFUSE				                'Mod Confuse
            'AURAs(	6	) = AddressOf 	SPELL_AURA_MOD_CHARM				                'Mod Charm
            AURAs(7) = AddressOf SPELL_AURA_MOD_FEAR                                        'Mod Fear
            AURAs(8) = AddressOf SPELL_AURA_PERIODIC_HEAL                                   'Periodic Heal
            'AURAs(	9	) = AddressOf 	SPELL_AURA_MOD_ATTACKSPEED			                'Mod Attack Speed
            AURAs(10) = AddressOf SPELL_AURA_MOD_THREAT                                     'Mod Threat
            AURAs(11) = AddressOf SPELL_AURA_MOD_TAUNT                                      'Taunt
            AURAs(12) = AddressOf SPELL_AURA_MOD_STUN                                       'Stun
            AURAs(13) = AddressOf SPELL_AURA_MOD_DAMAGE_DONE                                'Mod Damage Done
            'AURAs(14) = AddressOf SPELL_AURA_MOD_DAMAGE_TAKEN                              'Mod Damage Taken
            'AURAs(	15	) = AddressOf 	SPELL_AURA_DAMAGE_SHIELD			                'Damage Shield
            AURAs(16) = AddressOf SPELL_AURA_MOD_STEALTH                                    'Mod Stealth
            AURAs(17) = AddressOf SPELL_AURA_MOD_DETECT                                     'Mod Detect
            AURAs(18) = AddressOf SPELL_AURA_MOD_INVISIBILITY                               'Mod Invisibility
            AURAs(19) = AddressOf SPELL_AURA_MOD_INVISIBILITY_DETECTION                     'Mod Invisibility Detection
            AURAs(20) = AddressOf SPELL_AURA_PERIODIC_HEAL_PERCENT                          'Mod Health Regeneration %
            AURAs(21) = AddressOf SPELL_AURA_PERIODIC_ENERGIZE_PERCENT                      'Mod Mana Regeneration %
            AURAs(22) = AddressOf SPELL_AURA_MOD_RESISTANCE                                 'Mod Resistance
            AURAs(23) = AddressOf SPELL_AURA_PERIODIC_TRIGGER_SPELL                         'Periodic Trigger
            AURAs(24) = AddressOf SPELL_AURA_PERIODIC_ENERGIZE                              'Periodic Energize
            AURAs(25) = AddressOf SPELL_AURA_MOD_PACIFY                                     'Pacify
            AURAs(26) = AddressOf SPELL_AURA_MOD_ROOT                                       'Root
            AURAs(27) = AddressOf SPELL_AURA_MOD_SILENCE                                    'Silence
            'AURAs(	28	) = AddressOf 	SPELL_AURA_REFLECT_SPELLS			                'Reflect Spells %
            AURAs(29) = AddressOf SPELL_AURA_MOD_STAT                                       'Mod Stat
            AURAs(30) = AddressOf SPELL_AURA_MOD_SKILL                                      'Mod Skill
            AURAs(31) = AddressOf SPELL_AURA_MOD_INCREASE_SPEED                             'Mod Speed
            AURAs(32) = AddressOf SPELL_AURA_MOD_INCREASE_MOUNTED_SPEED                     'Mod Speed Mounted
            AURAs(33) = AddressOf SPELL_AURA_MOD_DECREASE_SPEED                             'Mod Speed Slow
            AURAs(34) = AddressOf SPELL_AURA_MOD_INCREASE_HEALTH                            'Mod Increase Health
            AURAs(35) = AddressOf SPELL_AURA_MOD_INCREASE_ENERGY                            'Mod Increase Energy
            AURAs(36) = AddressOf SPELL_AURA_MOD_SHAPESHIFT                                 'Shapeshift
            'AURAs(	37	) = AddressOf 	SPELL_AURA_EFFECT_IMMUNITY			                'Immune Effect
            'AURAs(	38	) = AddressOf 	SPELL_AURA_STATE_IMMUNITY			                'Immune State
            AURAs(39) = AddressOf SPELL_AURA_SCHOOL_IMMUNITY                                'Immune School
            'AURAs(	40	) = AddressOf 	SPELL_AURA_DAMAGE_IMMUNITY			                'Immune Damage
            AURAs(41) = AddressOf SPELL_AURA_DISPEL_IMMUNITY                                'Immune Dispel Type
            AURAs(42) = AddressOf SPELL_AURA_PROC_TRIGGER_SPELL                             'Proc Trigger Spell
            'AURAs(	43	) = AddressOf 	SPELL_AURA_PROC_TRIGGER_DAMAGE		                'Proc Trigger Damage
            AURAs(44) = AddressOf SPELL_AURA_TRACK_CREATURES                                'Track Creatures
            AURAs(45) = AddressOf SPELL_AURA_TRACK_RESOURCES                                'Track Resources
            'AURAs(	46	) = AddressOf 	SPELL_AURA_MOD_PARRY_SKILL			                'Mod Parry Skill
            'AURAs(	47	) = AddressOf 	SPELL_AURA_MOD_PARRY_PERCENT		                'Mod Parry Percent
            'AURAs(	48	) = AddressOf 	SPELL_AURA_MOD_DODGE_SKILL			                'Mod Dodge Skill
            'AURAs(	49	) = AddressOf 	SPELL_AURA_MOD_DODGE_PERCENT		                'Mod Dodge Percent
            'AURAs(	50	) = AddressOf 	SPELL_AURA_MOD_BLOCK_SKILL			                'Mod Block Skill
            'AURAs(	51	) = AddressOf 	SPELL_AURA_MOD_BLOCK_PERCENT		                'Mod Block Percent
            'AURAs(	52	) = AddressOf 	SPELL_AURA_MOD_CRIT_PERCENT			                'Mod Crit Percent
            AURAs(53) = AddressOf SPELL_AURA_PERIODIC_LEECH                                 'Periodic Leech
            'AURAs(	54	) = AddressOf 	SPELL_AURA_MOD_HIT_CHANCE			                'Mod Hit Chance
            'AURAs(	55	) = AddressOf 	SPELL_AURA_MOD_SPELL_HIT_CHANCE		                'Mod Spell Hit Chance
            AURAs(56) = AddressOf SPELL_AURA_TRANSFORM                                      'Transform
            'AURAs(	57	) = AddressOf 	SPELL_AURA_MOD_SPELL_CRIT_CHANCE	                'Mod Spell Crit Chance
            AURAs(58) = AddressOf SPELL_AURA_MOD_INCREASE_SWIM_SPEED                        'Mod Speed Swim
            'AURAs(	59	) = AddressOf 	SPELL_AURA_MOD_DAMAGE_DONE_CREATURE	                'Mod Creature Dmg Done
            'AURAs(	60	) = AddressOf 	SPELL_AURA_MOD_PACIFY_SILENCE		                'Pacify & Silence
            AURAs(61) = AddressOf SPELL_AURA_MOD_SCALE                                      'Mod Scale
            'AURAs(	62	) = AddressOf 	SPELL_AURA_PERIODIC_HEALTH_FUNNEL	                'Periodic Health Funnel
            'AURAs(	63	) = AddressOf 	SPELL_AURA_PERIODIC_MANA_FUNNEL		                'Periodic Mana Funnel
            AURAs(64) = AddressOf SPELL_AURA_PERIODIC_MANA_LEECH                            'Periodic Mana Leech
            'AURAs(	65	) = AddressOf 	SPELL_AURA_MOD_CASTING_SPEED		                'Haste - Spells
            'AURAs(	66	) = AddressOf 	SPELL_AURA_FEIGN_DEATH				                'Feign Death
            AURAs(67) = AddressOf SPELL_AURA_MOD_DISARM                                     'Disarm
            'AURAs(	68	) = AddressOf 	SPELL_AURA_MOD_STALKED				                'Mod Stalked
            AURAs(69) = AddressOf SPELL_AURA_SCHOOL_ABSORB                                  'School Absorb
            'AURAs(	70	) = AddressOf 	SPELL_AURA_EXTRA_ATTACKS			                'Extra Attacks
            'AURAs(	71	) = AddressOf 	SPELL_AURA_MOD_SPELL_CRIT_CHANCE_SCHOOL				'Mod School Spell Crit Chance
            'AURAs(	72	) = AddressOf 	SPELL_AURA_MOD_POWER_COST			                'Mod Power Cost
            'AURAs(	73	) = AddressOf 	SPELL_AURA_MOD_POWER_COST_SCHOOL	                'Mod School Power Cost
            'AURAs(	74	) = AddressOf 	SPELL_AURA_REFLECT_SPELLS_SCHOOL	                'Reflect School Spells %
            AURAs(75) = AddressOf SPELL_AURA_MOD_LANGUAGE                                   'Mod Language
            AURAs(76) = AddressOf SPELL_AURA_FAR_SIGHT                                      'Far Sight
            AURAs(77) = AddressOf SPELL_AURA_MECHANIC_IMMUNITY                              'Immune Mechanic
            AURAs(78) = AddressOf SPELL_AURA_MOUNTED                                        'Mounted
            AURAs(79) = AddressOf SPELL_AURA_MOD_DAMAGE_DONE_PCT                            'Mod Dmg %
            AURAs(80) = AddressOf SPELL_AURA_MOD_STAT_PERCENT                               'Mod Stat %
            'AURAs(	81	) = AddressOf 	SPELL_AURA_SPLIT_DAMAGE				                'Split Damage
            AURAs(82) = AddressOf SPELL_AURA_WATER_BREATHING                                'Water Breathing
            AURAs(83) = AddressOf SPELL_AURA_MOD_BASE_RESISTANCE                            'Mod Base Resistance
            AURAs(84) = AddressOf SPELL_AURA_MOD_REGEN                                      'Mod Health Regen
            AURAs(85) = AddressOf SPELL_AURA_MOD_POWER_REGEN                                'Mod Power Regen
            'AURAs(	86	) = AddressOf 	SPELL_AURA_CHANNEL_DEATH_ITEM		                'Create Death Item
            'AURAs(	87	) = AddressOf 	SPELL_AURA_MOD_DAMAGE_TAKEN_PCT			            'Mod Dmg % Taken
            'AURAs(	88	) = AddressOf 	SPELL_AURA_MOD_REGEN				                'Mod Health Regen Percent
            AURAs(89) = AddressOf SPELL_AURA_PERIODIC_DAMAGE_PERCENT                        'Periodic Damage Percent
            'AURAs(	90	) = AddressOf 	SPELL_AURA_MOD_RESIST_CHANCE		                'Mod Resist Chance
            'AURAs(	91	) = AddressOf 	SPELL_AURA_MOD_DETECT_RANGE			                'Mod Detect Range
            'AURAs(	92	) = AddressOf 	SPELL_AURA_PREVENTS_FLEEING			                'Prevent Fleeing
            'AURAs(	93	) = AddressOf 	SPELL_AURA_MOD_UNATTACKABLE			                'Mod Uninteractible
            'AURAs(	94	) = AddressOf 	SPELL_AURA_INTERRUPT_REGEN			                'Interrupt Regen
            AURAs(95) = AddressOf SPELL_AURA_GHOST                                          'Ghost
            'AURAs(	96	) = AddressOf 	SPELL_AURA_SPELL_MAGNET				                'Spell Magnet
            'AURAs(	97	) = AddressOf 	SPELL_AURA_MANA_SHIELD				                'Mana Shield
            'AURAs(	98	) = AddressOf 	SPELL_AURA_MOD_SKILL_TALENT			                'Mod Skill Talent
            AURAs(99) = AddressOf SPELL_AURA_MOD_ATTACK_POWER                               'Mod Attack Power
            'AURAs(	100	) = AddressOf 	SPELL_AURA_AURAS_VISIBLE			                'Auras Visible
            AURAs(101) = AddressOf SPELL_AURA_MOD_RESISTANCE_PCT                            'Mod Resistance %
            'AURAs(	102	) = AddressOf 	SPELL_AURA_MOD_CREATURE_ATTACK_POWER			    'Mod Creature Attack Power
            AURAs(103) = AddressOf SPELL_AURA_MOD_TOTAL_THREAT                              'Mod Total Threat (Fade)
            AURAs(104) = AddressOf SPELL_AURA_WATER_WALK                                    'Water Walk
            AURAs(105) = AddressOf SPELL_AURA_FEATHER_FALL                                  'Feather Fall
            AURAs(106) = AddressOf SPELL_AURA_HOVER                                         'Hover
            AURAs(107) = AddressOf SPELL_AURA_ADD_FLAT_MODIFIER                             'Add Flat Modifier
            AURAs(108) = AddressOf SPELL_AURA_ADD_PCT_MODIFIER                              'Add % Modifier
            'AURAs(	109	) = AddressOf 	SPELL_AURA_ADD_TARGET_TRIGGER		                'Add Class Target Trigger
            AURAs(110) = AddressOf SPELL_AURA_MOD_POWER_REGEN_PERCENT                       'Mod Power Regen %
            'AURAs(	111	) = AddressOf 	SPELL_AURA_ADD_CASTER_HIT_TRIGGER	                'Add Class Caster Hit Trigger
            'AURAs(	112	) = AddressOf 	SPELL_AURA_OVERRIDE_CLASS_SCRIPTS	                'Override Class Scripts
            'AURAs(	113	) = AddressOf 	SPELL_AURA_MOD_RANGED_DAMAGE_TAKEN	                'Mod Ranged Dmg Taken
            'AURAs(	114	) = AddressOf 	SPELL_AURA_MOD_RANGED_DAMAGE_TAKEN_PCT			    'Mod Ranged % Dmg Taken
            'AURAs(115) = AddressOf SPELL_AURA_MOD_HEALING                                  'Mod Healing
            'AURAs(	116	) = AddressOf 	SPELL_AURA_IGNORE_REGEN_INTERRUPT	                'Regen During Combat
            'AURAs(	117	) = AddressOf 	SPELL_AURA_MOD_MECHANIC_RESISTANCE	                'Mod Mechanic Resistance
            'AURAs(118) = AddressOf SPELL_AURA_MOD_HEALING_PCT                              'Mod Healing %
            'AURAs(	119	) = AddressOf 	SPELL_AURA_SHARE_PET_TRACKING		                'Share Pet Tracking
            'AURAs(	120	) = AddressOf 	SPELL_AURA_UNTRACKABLE				                'Untrackable
            AURAs(121) = AddressOf SPELL_AURA_EMPATHY                                       'Empathy (Lore, whatever)
            'AURAs(	122	) = AddressOf 	SPELL_AURA_MOD_OFFHAND_DAMAGE_PCT	                'Mod Offhand Dmg %
            'AURAs(	123	) = AddressOf 	SPELL_AURA_MOD_POWER_COST_PCT		                'Mod Power Cost %
            AURAs(124) = AddressOf SPELL_AURA_MOD_RANGED_ATTACK_POWER                       'Mod Ranged Attack Power
            'AURAs(	125	) = AddressOf 	SPELL_AURA_MOD_MELEE_DAMAGE_TAKEN	                'Mod Melee Dmg Taken
            'AURAs(	126	) = AddressOf 	SPELL_AURA_MOD_MELEE_DAMAGE_TAKEN_PCT			    'Mod Melee % Dmg Taken
            'AURAs(	127	) = AddressOf 	SPELL_AURA_RANGED_ATTACK_POWER_ATTACKER_BONUS	    'Rngd Atk Pwr Attckr Bonus
            'AURAs(	128	) = AddressOf 	SPELL_AURA_MOD_POSSESS_PET			                'Mod Possess Pet
            AURAs(129) = AddressOf SPELL_AURA_MOD_INCREASE_SPEED_ALWAYS                     'Mod Speed Always
            AURAs(130) = AddressOf SPELL_AURA_MOD_INCREASE_MOUNTED_SPEED_ALWAYS             'Mod Mounted Speed Always
            'AURAs(	131	) = AddressOf 	SPELL_AURA_MOD_CREATURE_RANGED_ATTACK_POWER		    'Mod Creature Ranged Attack Power
            'AURAs(	132	) = AddressOf 	SPELL_AURA_MOD_INCREASE_ENERGY_PERCENT			    'Mod Increase Energy %
            'AURAs(	133	) = AddressOf 	SPELL_AURA_MOD_INCREASE_HEALTH_PERCENT			    'Mod Max Health %
            'AURAs(	134	) = AddressOf 	SPELL_AURA_MOD_MANA_REGEN_INTERRUPT				    'Mod Interrupted Mana Regen
            AURAs(135) = AddressOf SPELL_AURA_MOD_HEALING_DONE                              'Mod Healing Done
            AURAs(136) = AddressOf SPELL_AURA_MOD_HEALING_DONE_PCT                          'Mod Healing Done %
            AURAs(137) = AddressOf SPELL_AURA_MOD_TOTAL_STAT_PERCENTAGE                     'Mod Total Stat %
            AURAs(138) = AddressOf SPELL_AURA_MOD_HASTE                                     'Haste - Melee
            'AURAs(	139	) = AddressOf 	SPELL_AURA_FORCE_REACTION			                'Force Reaction
            AURAs(140) = AddressOf SPELL_AURA_MOD_RANGED_HASTE                              'Haste - Ranged
            AURAs(141) = AddressOf SPELL_AURA_MOD_RANGED_AMMO_HASTE                         'Haste - Ranged (Ammo Only)
            AURAs(142) = AddressOf SPELL_AURA_MOD_BASE_RESISTANCE_PCT                       'Mod Base Resistance %
            AURAs(143) = AddressOf SPELL_AURA_MOD_RESISTANCE_EXCLUSIVE                      'Mod Resistance Exclusive
            AURAs(144) = AddressOf SPELL_AURA_SAFE_FALL                                     'Safe Fall
            'AURAs(	145	) = AddressOf 	SPELL_AURA_CHARISMA				                    'Charisma
            'AURAs(	146	) = AddressOf 	SPELL_AURA_PERSUADED				                'Persuaded
            'AURAs(	147	) = AddressOf 	SPELL_AURA_ADD_CREATURE_IMMUNITY	                'Add Creature Immunity
            'AURAs(	148	) = AddressOf 	SPELL_AURA_RETAIN_COMBO_POINTS		                'Retain Combo Points
            'AURAs(	149	) = AddressOf 	SPELL_AURA_RESIST_PUSHBACK			                'Resist Pushback
            'AURAs(	150	) = AddressOf 	SPELL_AURA_MOD_SHIELD_BLOCK			                'Mod Shield Block %
            'AURAs(	151	) = AddressOf 	SPELL_AURA_TRACK_STEALTHED			                'Track Stealthed
            'AURAs(	152	) = AddressOf 	SPELL_AURA_MOD_DETECTED_RANGE		                'Mod Aggro Range
            'AURAs(	153	) = AddressOf 	SPELL_AURA_SPLIT_DAMAGE_FLAT		                'Split Damage Flat
            AURAs(154) = AddressOf SPELL_AURA_MOD_STEALTH_LEVEL                             'Stealth Level Modifier
            'AURAs(	155	) = AddressOf 	SPELL_AURA_MOD_WATER_BREATHING		                'Mod Water Breathing
            'AURAs(	156	) = AddressOf 	SPELL_AURA_MOD_REPUTATION_ADJUST	                'Mod Reputation Gain
            'AURAs(	157	) = AddressOf 	SPELL_AURA_PET_DAMAGE_MULTI			                'Mod Pet Damage
            'AURAs(	158	) = AddressOf   SPELL_AURA_MOD_SHIELD_BLOCKVALUE                    'Mod Shield Block
            'AURAs(	159	) = AddressOf   SPELL_AURA_NO_PVP_CREDIT                            'Honorless
            'AURAs(	160 ) = AddressOf 	SPELL_AURA_MOD_AOE_AVOIDANCE		                'Mod Side/Rear PBAE Damage Taken %
            'AURAs(	161 ) = AddressOf 	SPELL_AURA_MOD_HEALTH_REGEN_IN_COMBAT               'Mod Health Regen In Combat
            'AURAs(	162 ) = AddressOf 	SPELL_AURA_POWER_BURN_MANA                        	'Power Burn (Mana)
            'AURAs(	163 ) = AddressOf 	SPELL_AURA_MOD_CRIT_DAMAGE_BONUS_MELEE              'Mod Critical Damage
            'AURAs(	164 ) = AddressOf  	SPELL_AURA_164                        			    'TEST
            'AURAs(	165 ) = AddressOf  	SPELL_AURA_MELEE_ATTACK_POWER_ATTACKER_BONUS        '
            'AURAs(	166 ) = AddressOf 	SPELL_AURA_MOD_ATTACK_POWER_PCT                     'Mod Attack Power %
            'AURAs( 167 ) = AddressOf   SPELL_AURA_MOD_RANGED_ATTACK_POWER_PCT              'Mod Ranged Attack Power %
            'AURAs(	168 ) = AddressOf 	SPELL_AURA_MOD_DAMAGE_DONE_VERSUS                   'Increase Damage % (vs. %X)
            'AURAs(	169 ) = AddressOf 	SPELL_AURA_MOD_CRIT_PERCENT_VERSUS                  'Increase Critical % (vs. %X)
            'AURAs(	170 ) = AddressOf  	SPELL_AURA_DETECT_AMORE                       		'
            'AURAs(	171 ) = AddressOf  	SPELL_AURA_MOD_SPEED_NOT_STACK                      '
            'AURAs(	172 ) = AddressOf  	SPELL_AURA_MOD_MOUNTED_SPEED_NOT_STACK              '
            'AURAs(	173 ) = AddressOf  	SPELL_AURA_ALLOW_CHAMPION_SPELLS                    '
            'AURAs(	174 ) = AddressOf 	SPELL_AURA_MOD_SPELL_DAMAGE_OF_STAT_PERCENT	        'Increase Spell Damage by % Spirit (Spells)
            'AURAs(	175 ) = AddressOf 	SPELL_AURA_MOD_SPELL_HEALING_OF_STAT_PERCENT        'Increase Spell Healing by % Spirit
            'AURAs(	176 ) = AddressOf  	SPELL_AURA_SPIRIT_OF_REDEMPTION                     '
            'AURAs(	177 ) = AddressOf 	SPELL_AURA_AOE_CHARM                        		'Charm
            'AURAs(	178 ) = AddressOf  	SPELL_AURA_MOD_DEBUFF_RESISTANCE                    '
            'AURAs(	179 ) = AddressOf  	SPELL_AURA_MOD_ATTACKER_SPELL_CRIT_CHANCE           '
            'AURAs(	180	) = AddressOf 	SPELL_AURA_MOD_FLAT_SPELL_DAMAGE_VERSUS             'Increase Spell Damage (vs. %X)
            'AURAs(	171 ) = AddressOf  	SPELL_AURA_MOD_FLAT_SPELL_CRIT_DAMAGE_VERSUS        '
            'AURAs(	182	) = AddressOf 	SPELL_AURA_MOD_RESISTANCE_OF_STAT_PERCENT           'Increase Resist by % of Intellect (%X)
            'AURAs(	183	) = AddressOf 	SPELL_AURA_MOD_CRITICAL_THREAT                      'Decrease Critical Threat by % (Spells)
            'AURAs(	184	) = AddressOf   SPELL_AURA_MOD_ATTACKER_MELEE_HIT_CHANCE            'Mod Melee GetHit Chance
            'AURAs(	185	) = AddressOf   SPELL_AURA_MOD_ATTACKER_RANGED_HIT_CHANCE           'Mod Ranged GetHit Chance
            'AURAs(	186	) = AddressOf   SPELL_AURA_MOD_ATTACKER_SPELL_HIT_CHANCE            'Mod Spell GetHit Chance
            'AURAs(	187	) = AddressOf   SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_CHANCE           'Mod Melee Critical GetHit Chance
            'AURAs(	188	) = AddressOf   SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_CHANCE          'Mod Ranged Critical GetHit Chance
            'AURAs(	189	) = AddressOf   SPELL_AURA_MOD_RATING                               'Mod Skill Rating
            'AURAs(	190	) = AddressOf   SPELL_AURA_MOD_FACTION_REPUTATION_GAIN              'Mod Reputation Gain
            'AURAs(	191	) = AddressOf   SPELL_AURA_USE_NORMAL_MOVEMENT_SPEED                '
            'AURAs(	192	) = AddressOf   SPELL_AURA_HASTE_MELEE                              '
            'AURAs(	193	) = AddressOf   SPELL_AURA_MELEE_SLOW                               '
            'AURAs(	194	) = AddressOf   SPELL_AURA_MOD_DEPRICATED_1                         '
            'AURAs(	195	) = AddressOf   SPELL_AURA_MOD_DEPRICATED_2                         '
            'AURAs(	196	) = AddressOf   SPELL_AURA_MOD_COOLDOWN                             'Mod Global Cooldowns
            'AURAs(	197	) = AddressOf   SPELL_AURA_MOD_ATTACKER_SPELL_AND_WEAPON_CRIT_CHANCE'No Critical Damage Taken
            'AURAs(	198	) = AddressOf   SPELL_AURA_MOD_ALL_WEAPON_SKILLS                    'Mod Weapon Skills
            'AURAs(	199	) = AddressOf   SPELL_AURA_MOD_INCREASES_SPELL_PCT_TO_HIT           'Mod Hit Chance
            'AURAs(	200	) = AddressOf   SPELL_AURA_MOD_XP_PCT                               'Mod Gained XP
            'AURAs(	201	) = AddressOf   SPELL_AURA_FLY                                      'Fly
            'AURAs(	202	) = AddressOf   SPELL_AURA_IGNORE_COMBAT_RESULT                     '
            'AURAs(	203	) = AddressOf   SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_DAMAGE           'Mod Melee Critical Damage Taken
            'AURAs(	204	) = AddressOf   SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_DAMAGE          'Mod Ranged Critical Damage Taken
            'AURAs(	205	) = AddressOf   SPELL_AURA_205                                      '
            'AURAs(	206	) = AddressOf 	SPELL_AURA_MOD_SPEED_MOUNTED                        'Mod Fly Speed Always
            'AURAs(207) = AddressOf SPELL_AURA_MOD_INCREASE_MOUNTED_FLY_SPEED                'Mod Fly Speed Mounted
            'AURAs(208) = AddressOf SPELL_AURA_MOD_INCREASE_FLY_SPEED                        'Mod Fly Speed
            'AURAs(209) = AddressOf SPELL_AURA_MOD_INCREASE_MOUNTED_FLY_SPEED_ALWAYS         'Mod Fly Speed Mounted Always
            'AURAs(	210	) = AddressOf 	SPELL_AURA_210                                      '
            'AURAs(	211	) = AddressOf 	SPELL_AURA_MOD_FLIGHT_SPEED_NOT_STACK               '
            'AURAs(	212	) = AddressOf 	SPELL_AURA_MOD_RANGED_ATTACK_POWER_OF_STAT_PERCENT  'Mod Ranged Attack Power by % of Intellect
            'AURAs(	213	) = AddressOf 	SPELL_AURA_MOD_RAGE_FROM_DAMAGE_DEALT               'Mod Rage From Damage
            'AURAs(	214	) = AddressOf 	SPELL_AURA_214                                      '
            'AURAs(	215	) = AddressOf 	SPELL_AURA_ARENA_PREPARATION                        'TEST
            'AURAs(	216	) = AddressOf 	SPELL_AURA_HASTE_SPELLS                             'Mod Casting Speed
            'AURAs(	217	) = AddressOf 	SPELL_AURA_217                                      '
            'AURAs(	218	) = AddressOf 	SPELL_AURA_HASTE_RANGED                             '
            'AURAs(	219	) = AddressOf 	SPELL_AURA_MOD_MANA_REGEN_FROM_STAT                 'Mod Regenerate by % of Intellect
            'AURAs(	220	) = AddressOf 	SPELL_AURA_MOD_RATING_FROM_STAT                     '
            'AURAs(	221	) = AddressOf 	SPELL_AURA_221                                      '
            'AURAs(	222	) = AddressOf 	SPELL_AURA_222                                      '
            'AURAs(	223	) = AddressOf 	SPELL_AURA_223                                      '
            'AURAs(	224	) = AddressOf 	SPELL_AURA_224                                      '
            'AURAs(	225	) = AddressOf 	SPELL_AURA_PRAYER_OF_MENDING                        '
            AURAs(226) = AddressOf SPELL_AURA_PERIODIC_DUMMY                                'Periodic dummy
            'AURAs( 227 ) = AddressOf   SPELL_AURA_227                                      '
            AURAs(228) = AddressOf SPELL_AURA_DETECT_STEALTH                                'Detect stealth
            'AURAs( 229 ) = AddressOf   SPELL_AURA_MOD_AOE_DAMAGE_AVOIDANCE                 '
            'AURAs( 230 ) = AddressOf   SPELL_AURA_230                                      '
            'AURAs( 231 ) = AddressOf   SPELL_AURA_231                                      '
            'AURAs( 232 ) = AddressOf   SPELL_AURA_MECHANIC_DURATION_MOD                    '
            'AURAs( 233 ) = AddressOf   SPELL_AURA_233                                      '
            'AURAs( 234 ) = AddressOf   AURA_MECHANIC_DURATION_MOD_NOT_STACK                '
            'AURAs( 235 ) = AddressOf   SPELL_AURA_MOD_DISPEL_RESIST                        '
            'AURAs( 236 ) = AddressOf   SPELL_AURA_236                                      '
            'AURAs( 237 ) = AddressOf   SPELL_AURA_MOD_SPELL_DAMAGE_OF_ATTACK_POWER         '
            'AURAs( 238 ) = AddressOf   SPELL_AURA_MOD_SPELL_HEALING_OF_ATTACK_POWER        '
            'AURAs( 239 ) = AddressOf   SPELL_AURA_MOD_SCALE_2                              '
            'AURAs( 240 ) = AddressOf   SPELL_AURA_MOD_EXPERTISE                            '
            'AURAs( 241 ) = AddressOf   SPELL_AURA_241                                      '
            'AURAs( 242 ) = AddressOf   SPELL_AURA_MOD_SPELL_DAMAGE_FROM_HEALING            '
            'AURAs( 243 ) = AddressOf   SPELL_AURA_243                                      '
            'AURAs( 244 ) = AddressOf   SPELL_AURA_244                                      '
            'AURAs( 245 ) = AddressOf   SPELL_AURA_MOD_DURATION_OF_MAGIC_EFFECTS            '
            'AURAs( 246 ) = AddressOf   SPELL_AURA_246                                      '
            'AURAs( 247 ) = AddressOf   SPELL_AURA_247                                      '
            'AURAs( 248 ) = AddressOf   SPELL_AURA_MOD_COMBAT_RESULT_CHANCE                 '
            'AURAs( 249 ) = AddressOf   SPELL_AURA_249                                      '
            'AURAs( 250 ) = AddressOf   SPELL_AURA_MOD_INCREASE_HEALTH_2                    '
            'AURAs( 251 ) = AddressOf   SPELL_AURA_MOD_ENEMY_DODGE                          '
            'AURAs( 252 ) = AddressOf   SPELL_AURA_252                                      '
            'AURAs( 253 ) = AddressOf   SPELL_AURA_253                                      '
            'AURAs( 254 ) = AddressOf   SPELL_AURA_254                                      '
            'AURAs( 255 ) = AddressOf   SPELL_AURA_255                                      '
            'AURAs( 256 ) = AddressOf   SPELL_AURA_256                                      '
            'AURAs( 257 ) = AddressOf   SPELL_AURA_257                                      '
            'AURAs( 258 ) = AddressOf   SPELL_AURA_258                                      '
            'AURAs( 259 ) = AddressOf   SPELL_AURA_259                                      '
            'AURAs( 260 ) = AddressOf   SPELL_AURA_260                                      '
            'AURAs( 261 ) = AddressOf   SPELL_AURA_261                                      '
        End Sub

#End Region

#Region "WS.Spells.SpellEffects"

        Delegate Function SpellEffectHandler(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
        Public Const SPELL_EFFECT_COUNT As Integer = 153
        Public SPELL_EFFECTs(SPELL_EFFECT_COUNT) As SpellEffectHandler

        Public Function SPELL_EFFECT_NOTHING(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_BIND(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).BindPlayer(Caster.GUID)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_DUMMY(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_INSTAKILL(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            For Each Unit As WS_Base.BaseUnit In Infected
                Unit.Die(Caster)
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_SCHOOL_DAMAGE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            Dim Damage As Integer = 0
            Dim Current As Integer = 0
            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Caster Is WS_DynamicObjects.DynamicObjectObject Then
                    Damage = SpellInfo.GetValue(CType(Caster, WS_DynamicObjects.DynamicObjectObject).Caster.Level)
                Else
                    Damage = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                End If

                If Current > 0 Then Damage *= (SpellInfo.DamageMultiplier ^ Current)

                Dim realCaster As WS_Base.BaseUnit = Nothing
                If TypeOf Caster Is WS_Base.BaseUnit Then
                    realCaster = Caster
                ElseIf TypeOf Caster Is WS_DynamicObjects.DynamicObjectObject Then
                    realCaster = CType(Caster, WS_DynamicObjects.DynamicObjectObject).Caster
                End If

                If realCaster IsNot Nothing Then Unit.DealSpellDamage(realCaster, SpellInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_NONMELEE)
                Current += 1
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_ENVIRONMENTAL_DAMAGE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Dim Damage As Integer = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            For Each Unit As WS_Base.BaseUnit In Infected
                Unit.DealDamage(Damage, Caster)
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then CType(Unit, WS_PlayerData.CharacterObject).LogEnvironmentalDamage(SPELLs(SpellID).School, Damage)
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_TRIGGER_SPELL(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            'NOTE: Trigger spell shouldn't add a cast error?
            If SPELLs.ContainsKey(SpellInfo.TriggerSpell) = False Then Return SpellFailedReason.SPELL_NO_ERROR
            If Target.unitTarget Is Nothing Then Return SpellFailedReason.SPELL_NO_ERROR

            Select Case SpellInfo.TriggerSpell
                Case 18461
                    Target.unitTarget.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOD_ROOT)
                    Target.unitTarget.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOD_DECREASE_SPEED)
                    Target.unitTarget.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOD_STALKED)

                    'TODO: Cast highest rank of stealth
                Case 35729
                    For i As Byte = _Global_Constants.MAX_POSITIVE_AURA_EFFECTs To _Global_Constants.MAX_AURA_EFFECTs_VISIBLE - 1
                        If Not Target.unitTarget.ActiveSpells(i) Is Nothing Then
                            If (SPELLs(Target.unitTarget.ActiveSpells(i).SpellID).School And 1) = 0 Then 'No physical spells
                                If (SPELLs(Target.unitTarget.ActiveSpells(i).SpellID).Attributes And &H10000) Then
                                    Target.unitTarget.RemoveAura(i, Target.unitTarget.ActiveSpells(i).SpellCaster)
                                End If
                            End If
                        End If
                    Next
            End Select

            If SPELLs(SpellInfo.TriggerSpell).EquippedItemClass >= 0 And (TypeOf Caster Is WS_PlayerData.CharacterObject) Then
                'If (SPELLs(SpellInfo.TriggerSpell).AttributesEx3 And SpellAttributesEx3.SPELL_ATTR_EX3_MAIN_HAND) Then
                '    If CType(Caster, CharacterObject).Items.ContainsKey(EQUIPMENT_SLOT_MAINHAND) = False Then Return SpellFailedReason.SPELL_NO_ERROR
                '    If CType(Caster, CharacterObject).Items(EQUIPMENT_SLOT_MAINHAND).IsBroken Then Return SpellFailedReason.SPELL_NO_ERROR
                'End If
                'If (SPELLs(SpellInfo.TriggerSpell).AttributesEx3 And SpellAttributesEx3.SPELL_ATTR_EX3_REQ_OFFHAND) Then
                '    If CType(Caster, CharacterObject).Items.ContainsKey(EQUIPMENT_SLOT_OFFHAND) = False Then Return SpellFailedReason.SPELL_NO_ERROR
                '    If CType(Caster, CharacterObject).Items(EQUIPMENT_SLOT_OFFHAND).IsBroken Then Return SpellFailedReason.SPELL_NO_ERROR
                'End If
            End If

            Dim castParams As New CastSpellParameters(Target, Caster, SpellInfo.TriggerSpell)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_TELEPORT_UNITS(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    With CType(Unit, WS_PlayerData.CharacterObject)
                        Select Case SpellID
                            Case 8690 'Hearthstone
                                .Teleport(.bindpoint_positionX, .bindpoint_positionY, .bindpoint_positionZ, .orientation, .bindpoint_map_id)
                            Case Else
                                If _WS_DBCDatabase.TeleportCoords.ContainsKey(SpellID) Then
                                    .Teleport(_WS_DBCDatabase.TeleportCoords(SpellID).PosX, _WS_DBCDatabase.TeleportCoords(SpellID).PosY, _WS_DBCDatabase.TeleportCoords(SpellID).PosZ, .orientation, _WS_DBCDatabase.TeleportCoords(SpellID).MapID)
                                Else
                                    _WorldServer.Log.WriteLine(LogType.WARNING, "WARNING: Spell {0} did not have any teleport coordinates.", SpellID)
                                End If
                        End Select
                    End With
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_MANA_DRAIN(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            Dim Damage As Integer = 0
            Dim TargetPower As Integer = 0
            For Each Unit As WS_Base.BaseUnit In Infected
                Damage = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                If TypeOf Caster Is WS_PlayerData.CharacterObject Then Damage += SpellInfo.valuePerLevel * CType(Caster, WS_PlayerData.CharacterObject).Level

                'DONE: Take the power from the target and give to the caster
                'TODO: Rune power?
                TargetPower = 0
                Select Case SpellInfo.MiscValue
                    Case ManaTypes.TYPE_MANA
                        If Damage > Unit.Mana.Current Then Damage = Unit.Mana.Current
                        Unit.Mana.Current -= Damage
                        CType(Caster, WS_Base.BaseUnit).Mana.Current += Damage
                        TargetPower = Unit.Mana.Current
                    Case ManaTypes.TYPE_RAGE
                        If (TypeOf Unit Is WS_PlayerData.CharacterObject) AndAlso (TypeOf Caster Is WS_PlayerData.CharacterObject) Then
                            If Damage > CType(Unit, WS_PlayerData.CharacterObject).Rage.Current Then Damage = CType(Unit, WS_PlayerData.CharacterObject).Rage.Current
                            CType(Unit, WS_PlayerData.CharacterObject).Rage.Current -= Damage
                            CType(Caster, WS_PlayerData.CharacterObject).Rage.Current += Damage
                            TargetPower = CType(Unit, WS_PlayerData.CharacterObject).Rage.Current
                        End If
                    Case ManaTypes.TYPE_ENERGY
                        If (TypeOf Unit Is WS_PlayerData.CharacterObject) AndAlso (TypeOf Caster Is WS_PlayerData.CharacterObject) Then
                            If Damage > CType(Unit, WS_PlayerData.CharacterObject).Energy.Current Then Damage = CType(Unit, WS_PlayerData.CharacterObject).Energy.Current
                            CType(Unit, WS_PlayerData.CharacterObject).Energy.Current -= Damage
                            CType(Caster, WS_PlayerData.CharacterObject).Energy.Current += Damage
                            TargetPower = CType(Unit, WS_PlayerData.CharacterObject).Energy.Current
                        End If
                    Case Else
                        Unit.Mana.Current -= Damage
                        CType(Caster, WS_Base.BaseUnit).Mana.Current += Damage
                        TargetPower = Unit.Mana.Current
                End Select

                'DONE: Send victim mana update, for near
                If TypeOf Unit Is WS_Creatures.CreatureObject Then
                    Dim myTmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    Dim myPacket As New Packets.UpdatePacketClass
                    myTmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + SpellInfo.MiscValue, TargetPower)
                    myTmpUpdate.AddToPacket(myPacket, ObjectUpdateType.UPDATETYPE_VALUES, CType(Unit, WS_Creatures.CreatureObject))
                    Unit.SendToNearPlayers(myPacket)
                    myPacket.Dispose()
                    myTmpUpdate.Dispose()
                ElseIf TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + SpellInfo.MiscValue, TargetPower)
                    CType(Unit, WS_PlayerData.CharacterObject).SendCharacterUpdate()
                End If
            Next

            'TODO: SpellFailedReason.SPELL_FAILED_ALREADY_FULL_MANA
            'DONE: Send caster mana update, for near
            Dim CasterPower As Integer = 0
            Select Case SpellInfo.MiscValue
                Case ManaTypes.TYPE_MANA
                    CasterPower = CType(Caster, WS_Base.BaseUnit).Mana.Current
                Case ManaTypes.TYPE_RAGE
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then CasterPower = CType(Caster, WS_PlayerData.CharacterObject).Rage.Current
                Case ManaTypes.TYPE_ENERGY
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then CasterPower = CType(Caster, WS_PlayerData.CharacterObject).Energy.Current
                Case Else
                    CasterPower = CType(Caster, WS_Base.BaseUnit).Mana.Current
            End Select
            If TypeOf Caster Is WS_Creatures.CreatureObject Then
                Dim TmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                Dim Packet As New Packets.UpdatePacketClass
                TmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + SpellInfo.MiscValue, CasterPower)
                TmpUpdate.AddToPacket(Packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_Creatures.CreatureObject))
                Target.unitTarget.SendToNearPlayers(Packet)
                Packet.Dispose()
                TmpUpdate.Dispose()
            ElseIf TypeOf Caster Is WS_PlayerData.CharacterObject Then
                CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + SpellInfo.MiscValue, CasterPower)
                CType(Caster, WS_PlayerData.CharacterObject).SendCharacterUpdate()
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_HEAL(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            Dim Damage As Integer = 0
            Dim Current As Integer = 0
            For Each Unit As WS_Base.BaseUnit In Infected
                Damage = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                If Current > 0 Then Damage *= (SpellInfo.DamageMultiplier ^ Current)

                'NOTE: This function heals as well
                Unit.DealSpellDamage(Caster, SpellInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_HEAL)
                Current += 1
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_HEAL_MAX_HEALTH(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            Dim Damage As Integer = 0
            Dim Current As Integer = 0
            For Each Unit As WS_Base.BaseUnit In Infected
                Damage = CType(Caster, WS_Base.BaseUnit).Life.Maximum
                If Current > 0 AndAlso SpellInfo.DamageMultiplier < 1 Then Damage *= (SpellInfo.DamageMultiplier ^ Current)

                'NOTE: This function heals as well
                Unit.DealSpellDamage(Caster, SpellInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_HEAL)
                Current += 1
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_ENERGIZE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Dim Damage As Integer = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            For Each Unit As WS_Base.BaseUnit In Infected
                SendEnergizeSpellLog(Caster, Target.unitTarget, SpellID, Damage, SpellInfo.MiscValue)
                Unit.Energize(Damage, SpellInfo.MiscValue, Caster)
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_ENERGIZE_PCT(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Dim Damage As Integer = 0

            For Each Unit As WS_Base.BaseUnit In Infected
                Select Case SpellInfo.MiscValue
                    Case ManaTypes.TYPE_MANA
                        Damage = (SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) * Unit.Mana.Maximum
                End Select
                SendEnergizeSpellLog(Caster, Target.unitTarget, SpellID, Damage, SpellInfo.MiscValue)
                Unit.Energize(Damage, SpellInfo.MiscValue, Caster)
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_OPEN_LOCK(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Not TypeOf Caster Is WS_PlayerData.CharacterObject Then Return SpellFailedReason.SPELL_FAILED_ERROR

            Dim LootType As LootType = LootType.LOOTTYPE_CORPSE

            Dim targetGUID As ULong, lockID As Integer
            If Not Target.goTarget Is Nothing Then 'GO Target
                targetGUID = Target.goTarget.GUID
                lockID = CType(Target.goTarget, WS_GameObjects.GameObjectObject).LockID
            ElseIf Not Target.itemTarget Is Nothing Then 'Item Target
                targetGUID = Target.itemTarget.GUID
                lockID = Target.itemTarget.ItemInfo.LockID
            Else
                Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS
            End If

            'TODO: Check if it's a battlegroundflag

            If lockID = 0 Then
                'TODO: Send loot for items
                If _CommonGlobalFunctions.GuidIsGameObject(targetGUID) AndAlso _WorldServer.WORLD_GAMEOBJECTs.ContainsKey(targetGUID) Then
                    _WorldServer.WORLD_GAMEOBJECTs(targetGUID).LootObject(CType(Caster, WS_PlayerData.CharacterObject), LootType)
                End If

                Return SpellFailedReason.SPELL_NO_ERROR
            End If

            If _WS_Loot.Locks.ContainsKey(lockID) = False Then
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Lock {0} did not exist.", lockID)
                Return SpellFailedReason.SPELL_FAILED_ERROR
            End If

            For i As Byte = 0 To 4
                If Item IsNot Nothing AndAlso _WS_Loot.Locks(lockID).KeyType(i) = LockKeyType.LOCK_KEY_ITEM AndAlso _WS_Loot.Locks(lockID).Keys(i) = Item.ItemEntry Then
                    'TODO: Send loot for items
                    If _CommonGlobalFunctions.GuidIsGameObject(targetGUID) AndAlso _WorldServer.WORLD_GAMEOBJECTs.ContainsKey(targetGUID) Then
                        _WorldServer.WORLD_GAMEOBJECTs(targetGUID).LootObject(CType(Caster, WS_PlayerData.CharacterObject), LootType)
                    End If

                    Return SpellFailedReason.SPELL_NO_ERROR
                End If
            Next

            Dim SkillID As Integer = 0
            If (Not SPELLs(SpellID).SpellEffects(1) Is Nothing) AndAlso SPELLs(SpellID).SpellEffects(1).ID = SpellEffects_Names.SPELL_EFFECT_SKILL Then
                SkillID = SPELLs(SpellID).SpellEffects(1).MiscValue
            ElseIf (Not SPELLs(SpellID).SpellEffects(0) Is Nothing) AndAlso SPELLs(SpellID).SpellEffects(0).MiscValue = LockType.LOCKTYPE_PICKLOCK Then
                SkillID = SKILL_IDs.SKILL_LOCKPICKING
            End If

            Dim ReqSkillValue As Short = _WS_Loot.Locks(lockID).RequiredMiningSkill
            If _WS_Loot.Locks(lockID).RequiredLockingSkill > 0 Then
                If SkillID <> SKILL_IDs.SKILL_LOCKPICKING Then 'Cheat attempt?
                    Return SpellFailedReason.SPELL_FAILED_FIZZLE
                End If
                ReqSkillValue = _WS_Loot.Locks(lockID).RequiredLockingSkill
            ElseIf SkillID = SKILL_IDs.SKILL_LOCKPICKING Then 'Apply picklock skill to wrong target
                Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS
            End If

            If SkillID Then
                LootType = LootType.LOOTTYPE_SKINNING
                If CType(Caster, WS_PlayerData.CharacterObject).Skills.ContainsKey(SkillID) = False OrElse CType(Caster, WS_PlayerData.CharacterObject).Skills(SkillID).Current < ReqSkillValue Then
                    Return SpellFailedReason.SPELL_FAILED_LOW_CASTLEVEL
                End If

                'TODO: Update skill
            End If

            'TODO: Send loot for items
            If _CommonGlobalFunctions.GuidIsGameObject(targetGUID) AndAlso _WorldServer.WORLD_GAMEOBJECTs.ContainsKey(targetGUID) Then
                _WorldServer.WORLD_GAMEOBJECTs(targetGUID).LootObject(CType(Caster, WS_PlayerData.CharacterObject), LootType)
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_PICKPOCKET(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Not TypeOf Caster Is WS_PlayerData.CharacterObject Then Return SpellFailedReason.SPELL_FAILED_ERROR

            'DONE: Pickpocket the creature!
            If (TypeOf Target.unitTarget Is WS_Creatures.CreatureObject) AndAlso CType(Caster, WS_Base.BaseUnit).IsFriendlyTo(Target.unitTarget) = False Then
                With CType(Target.unitTarget, WS_Creatures.CreatureObject)
                    If .CreatureInfo.CreatureType = UNIT_TYPE.HUMANOID OrElse .CreatureInfo.CreatureType = UNIT_TYPE.UNDEAD Then
                        If .IsDead = False Then
                            Dim chance As Integer = 10 + CType(Caster, WS_Base.BaseUnit).Level - .Level

                            If chance > _WorldServer.Rnd.Next(0, 20) Then
                                'Successful pickpocket
                                If .CreatureInfo.PocketLootID > 0 Then
                                    Dim Loot As New WS_Loot.LootObject(.GUID, LootType.LOOTTYPE_PICKPOCKETING) With {
                                            .LootOwner = Caster.GUID
                                            }

                                    Dim Template As WS_Loot.LootTemplate = _WS_Loot.LootTemplates_Pickpocketing.GetLoot(.CreatureInfo.PocketLootID)
                                    If Template IsNot Nothing Then
                                        Template.Process(Loot, 0)
                                    End If

                                    Loot.SendLoot(CType(Caster, WS_PlayerData.CharacterObject).client)
                                Else
                                    _WS_Loot.SendEmptyLoot(.GUID, LootType.LOOTTYPE_PICKPOCKETING, CType(Caster, WS_PlayerData.CharacterObject).client)
                                End If
                            Else
                                'Failed pickpocket
                                CType(Caster, WS_Base.BaseUnit).RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TALK)
                                If .aiScript IsNot Nothing Then
                                    .aiScript.OnGenerateHate(CType(Caster, WS_Base.BaseUnit), 100)
                                End If
                            End If
                            Return SpellFailedReason.SPELL_NO_ERROR
                        Else
                            Return SpellFailedReason.SPELL_FAILED_TARGETS_DEAD
                        End If
                    End If
                End With
            End If

            Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS
        End Function

        Public Function SPELL_EFFECT_SKINNING(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Not TypeOf Caster Is WS_PlayerData.CharacterObject Then Return SpellFailedReason.SPELL_FAILED_ERROR

            'DONE: Skin the creature!
            If (TypeOf Target.unitTarget Is WS_Creatures.CreatureObject) Then
                With CType(Target.unitTarget, WS_Creatures.CreatureObject)
                    If .IsDead AndAlso _Functions.HaveFlags(.cUnitFlags, UnitFlags.UNIT_FLAG_SKINNABLE) Then
                        .cUnitFlags = .cUnitFlags And (Not UnitFlags.UNIT_FLAG_SKINNABLE)
                        'TODO: Is skinning skill requirement met?
                        'TODO: Update skinning skill!

                        If .CreatureInfo.SkinLootID > 0 Then
                            Dim Loot As New WS_Loot.LootObject(.GUID, LootType.LOOTTYPE_SKINNING) With {
                                    .LootOwner = Caster.GUID
                                    }

                            Dim Template As WS_Loot.LootTemplate = _WS_Loot.LootTemplates_Skinning.GetLoot(.CreatureInfo.SkinLootID)
                            If Template IsNot Nothing Then
                                Template.Process(Loot, 0)
                            End If

                            Loot.SendLoot(CType(Caster, WS_PlayerData.CharacterObject).client)
                        Else
                            _WS_Loot.SendEmptyLoot(.GUID, LootType.LOOTTYPE_SKINNING, CType(Caster, WS_PlayerData.CharacterObject).client)
                        End If

                        Dim TmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                        Dim Packet As New Packets.UpdatePacketClass
                        TmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, .cUnitFlags)
                        TmpUpdate.AddToPacket(Packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target.unitTarget, WS_Creatures.CreatureObject))
                        Target.unitTarget.SendToNearPlayers(Packet)
                        Packet.Dispose()
                        TmpUpdate.Dispose()
                    End If
                    Return SpellFailedReason.SPELL_NO_ERROR
                End With
            End If

            Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS
        End Function

        Public Function SPELL_EFFECT_DISENCHANT(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Not TypeOf Caster Is WS_PlayerData.CharacterObject Then Return SpellFailedReason.SPELL_FAILED_ERROR

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_PROFICIENCY(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).SendProficiencies()
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_LEARN_SPELL(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            If SpellInfo.TriggerSpell <> 0 Then
                For Each Unit As WS_Base.BaseUnit In Infected
                    If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                        CType(Unit, WS_PlayerData.CharacterObject).LearnSpell(SpellInfo.TriggerSpell)
                    End If
                Next
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_SKILL_STEP(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            If SpellInfo.MiscValue <> 0 Then
                For Each Unit As WS_Base.BaseUnit In Infected
                    If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                        CType(Unit, WS_PlayerData.CharacterObject).LearnSkill(SpellInfo.MiscValue, , (SpellInfo.valueBase + 1) * 75)
                        CType(Unit, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
                    End If
                Next
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_DISPEL(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            For Each Unit As WS_Base.BaseUnit In Infected
                'TODO: Remove friendly or enemy spells depending on the reaction?
                If (Unit.DispellImmunity And (1 << SpellInfo.MiscValue)) = 0 Then
                    Unit.RemoveAurasByDispellType(SpellInfo.MiscValue, SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level))
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_EVADE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                'TODO: Evade
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_DODGE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).combatDodge += SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_PARRY(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).combatParry += SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_BLOCK(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).combatBlock += SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_DUAL_WIELD(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).spellCanDualWeild = True
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_WEAPON_DAMAGE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            Dim damageInfo As WS_Combat.DamageInfo
            Dim Ranged As Boolean = False
            Dim Offhand As Boolean = False
            If SPELLs(SpellID).IsRanged Then
                Ranged = True
            End If

            For Each Unit As WS_Base.BaseUnit In Infected
                damageInfo = _WS_Combat.CalculateDamage(Caster, Unit, Offhand, Ranged, SPELLs(SpellID), SpellInfo)
                If (damageInfo.HitInfo And AttackHitState.HIT_RESIST) Then
                    SPELLs(SpellID).SendSpellMiss(Caster, Unit, SpellMissInfo.SPELL_MISS_RESIST)
                ElseIf (damageInfo.HitInfo And AttackHitState.HIT_MISS) Then
                    SPELLs(SpellID).SendSpellMiss(Caster, Unit, SpellMissInfo.SPELL_MISS_MISS)
                ElseIf (damageInfo.HitInfo And AttackHitState.HITINFO_ABSORB) Then
                    SPELLs(SpellID).SendSpellMiss(Caster, Unit, SpellMissInfo.SPELL_MISS_ABSORB)
                ElseIf (damageInfo.HitInfo And AttackHitState.HITINFO_BLOCK) Then
                    SPELLs(SpellID).SendSpellMiss(Caster, Unit, SpellMissInfo.SPELL_MISS_BLOCK)
                Else
                    SendNonMeleeDamageLog(Caster, Unit, SpellID, damageInfo.DamageType, damageInfo.GetDamage, damageInfo.Resist, damageInfo.Absorbed, (damageInfo.HitInfo And AttackHitState.HITINFO_CRITICALHIT))
                    Unit.DealDamage(damageInfo.GetDamage, Caster)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_WEAPON_DAMAGE_NOSCHOOL(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                    CType(Caster, WS_PlayerData.CharacterObject).attackState.DoMeleeDamageBySpell(Caster, Unit, SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level), SpellID)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_ADICIONAL_DMG(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            Dim damageInfo As WS_Combat.DamageInfo
            Dim Ranged As Boolean = False
            Dim Offhand As Boolean = False
            If SPELLs(SpellID).IsRanged Then
                Ranged = True
            End If

            For Each Unit As WS_Base.BaseUnit In Infected
                damageInfo = _WS_Combat.CalculateDamage(Caster, Unit, Offhand, Ranged, SPELLs(SpellID), SpellInfo)
                SendNonMeleeDamageLog(Caster, Unit, SpellID, damageInfo.DamageType, damageInfo.GetDamage, damageInfo.Resist, damageInfo.Absorbed, (damageInfo.HitInfo And AttackHitState.HITINFO_CRITICALHIT))
                Unit.DealDamage(damageInfo.GetDamage, Caster)
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_HONOR(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    CType(Unit, WS_PlayerData.CharacterObject).HonorPoints += SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                    If CType(Unit, WS_PlayerData.CharacterObject).HonorPoints > 75000 Then CType(Unit, WS_PlayerData.CharacterObject).HonorPoints = 75000
                    CType(Unit, WS_PlayerData.CharacterObject).HonorSave()
                    'CType(Unit, CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_HONOR_CURRENCY, CType(Unit, CharacterObject).HonorCurrency)
                    'CType(Unit, CharacterObject).SendCharacterUpdate(False)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Private Const SLOT_NOT_FOUND As Integer = -1
        Private Const SLOT_CREATE_NEW As Integer = -2
        Private Const SLOT_NO_SPACE As Integer = Integer.MaxValue
        Public Function ApplyAura(ByRef auraTarget As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer) As SpellFailedReason
            Try
                Dim spellCasted As Integer = SLOT_NOT_FOUND
                Do
                    'DONE: If active add to visible
                    'TODO: If positive effect add to upper part spells
                    Dim AuraStart As Integer = _Global_Constants.MAX_AURA_EFFECTs_VISIBLE - 1
                    Dim AuraEnd As Integer = 0

                    'DONE: Passives are not displayed
                    If SPELLs(SpellID).IsPassive Then
                        AuraStart = _Global_Constants.MAX_AURA_EFFECTs - 1
                        AuraEnd = _Global_Constants.MAX_AURA_EFFECTs_VISIBLE
                    End If

                    'DONE: Get spell duration
                    Dim Duration As Integer = SPELLs(SpellID).GetDuration

                    'HACK: Set duration for Resurrection Sickness spell
                    If SpellID = 15007 Then
                        Select Case auraTarget.Level
                            Case Is < 11
                                Duration = 0
                            Case Is > 19
                                Duration = 10 * 60 * 1000
                            Case Else
                                Duration = (auraTarget.Level - 10) * 60 * 1000
                        End Select
                    End If

                    'DONE: Find spell aura slot
                    For i As Integer = AuraStart To AuraEnd Step -1
                        If (Not auraTarget.ActiveSpells(i) Is Nothing) AndAlso auraTarget.ActiveSpells(i).SpellID = SpellID Then
                            spellCasted = i
                            If (auraTarget.ActiveSpells(i).Aura_Info(0) IsNot Nothing AndAlso auraTarget.ActiveSpells(i).Aura_Info(0) Is SpellInfo) OrElse (auraTarget.ActiveSpells(i).Aura_Info(1) IsNot Nothing AndAlso auraTarget.ActiveSpells(i).Aura_Info(1) Is SpellInfo) OrElse (auraTarget.ActiveSpells(i).Aura_Info(2) IsNot Nothing AndAlso auraTarget.ActiveSpells(i).Aura_Info(2) Is SpellInfo) Then
                                If auraTarget.ActiveSpells(i).Aura_Info(0) IsNot Nothing AndAlso auraTarget.ActiveSpells(i).Aura_Info(0) Is SpellInfo Then
                                    'DONE: Update the duration
                                    auraTarget.ActiveSpells(i).SpellDuration = Duration
                                    'DONE: Update the stack if possible
                                    If SPELLs(SpellID).maxStack > 0 AndAlso auraTarget.ActiveSpells(i).StackCount < SPELLs(SpellID).maxStack Then
                                        auraTarget.ActiveSpells(i).StackCount += 1
                                        AURAs(SpellInfo.ApplyAuraIndex).Invoke(auraTarget, Caster, SpellInfo, SpellID, 1, AuraAction.AURA_ADD)

                                        If TypeOf auraTarget Is WS_PlayerData.CharacterObject Then
                                            CType(auraTarget, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(auraTarget, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_AURAS
                                        ElseIf (TypeOf auraTarget Is WS_Pets.PetObject) AndAlso (TypeOf CType(auraTarget, WS_Pets.PetObject).Owner Is WS_PlayerData.CharacterObject) Then
                                            CType(CType(auraTarget, WS_Pets.PetObject).Owner, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(CType(auraTarget, WS_Pets.PetObject).Owner, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_AURAS
                                        End If
                                    End If
                                    auraTarget.UpdateAura(i)
                                End If
                                Return SpellFailedReason.SPELL_NO_ERROR
                            Else
                                If auraTarget.ActiveSpells(i).Aura(0) Is Nothing Then
                                    auraTarget.ActiveSpells(i).Aura(0) = AURAs(SpellInfo.ApplyAuraIndex)
                                    auraTarget.ActiveSpells(i).Aura_Info(0) = SpellInfo
                                    _WorldServer.Log.WriteLine(LogType.DEBUG, "APPLYING AURA {0}", CType(SpellInfo.ApplyAuraIndex, AuraEffects_Names))
                                    Exit For
                                ElseIf auraTarget.ActiveSpells(i).Aura(1) Is Nothing Then
                                    auraTarget.ActiveSpells(i).Aura(1) = AURAs(SpellInfo.ApplyAuraIndex)
                                    auraTarget.ActiveSpells(i).Aura_Info(1) = SpellInfo
                                    _WorldServer.Log.WriteLine(LogType.DEBUG, "APPLYING AURA {0}", CType(SpellInfo.ApplyAuraIndex, AuraEffects_Names))
                                    Exit For
                                ElseIf auraTarget.ActiveSpells(i).Aura(2) Is Nothing Then
                                    auraTarget.ActiveSpells(i).Aura(2) = AURAs(SpellInfo.ApplyAuraIndex)
                                    auraTarget.ActiveSpells(i).Aura_Info(2) = SpellInfo
                                    _WorldServer.Log.WriteLine(LogType.DEBUG, "APPLYING AURA {0}", CType(SpellInfo.ApplyAuraIndex, AuraEffects_Names))
                                    Exit For
                                Else
                                    spellCasted = SLOT_NO_SPACE
                                End If
                            End If
                        End If
                    Next

                    'DONE: Not found same active aura on that player, create new
                    If spellCasted = SLOT_NOT_FOUND Then auraTarget.AddAura(SpellID, Duration, Caster)
                    If spellCasted = SLOT_CREATE_NEW Then spellCasted = SLOT_NO_SPACE
                    If spellCasted < 0 Then spellCasted -= 1
                Loop While spellCasted < 0

                'DONE: No more space for auras
                If spellCasted = SLOT_NO_SPACE Then Return SpellFailedReason.SPELL_FAILED_TRY_AGAIN

                'DONE: Cast the aura
                AURAs(SpellInfo.ApplyAuraIndex).Invoke(auraTarget, Caster, SpellInfo, SpellID, 1, AuraAction.AURA_ADD)

            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.CRITICAL, "Error while applying aura for spell {0}:{1}", SpellID, Environment.NewLine & e.ToString)
            End Try

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_APPLY_AURA(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If ((Target.targetMask And SpellCastTargetFlags.TARGET_FLAG_UNIT) OrElse Target.targetMask = SpellCastTargetFlags.TARGET_FLAG_SELF) AndAlso Target.unitTarget Is Nothing Then Return SpellFailedReason.SPELL_FAILED_BAD_IMPLICIT_TARGETS

            Dim result As SpellFailedReason = SpellFailedReason.SPELL_NO_ERROR

            'DONE: Sit down on some spells
            If TypeOf Caster Is WS_PlayerData.CharacterObject AndAlso (SPELLs(SpellID).auraInterruptFlags And SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_NOT_SEATED) Then
                CType(Caster, WS_Base.BaseUnit).StandState = StandStates.STANDSTATE_SIT
                CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, CType(Caster, WS_Base.BaseUnit).cBytes1)
                CType(Caster, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                Dim packetACK As New Packets.PacketClass(OPCODES.SMSG_STANDSTATE_CHANGE_ACK)
                packetACK.AddInt8(CType(Caster, WS_Base.BaseUnit).StandState)
                CType(Caster, WS_PlayerData.CharacterObject).client.Send(packetACK)
                packetACK.Dispose()
            End If

            If (Target.targetMask And SpellCastTargetFlags.TARGET_FLAG_UNIT) OrElse Target.targetMask = SpellCastTargetFlags.TARGET_FLAG_SELF Then
                Dim count As Integer = SPELLs(SpellID).MaxTargets
                For Each u As WS_Base.BaseUnit In Infected
                    ApplyAura(u, Caster, SpellInfo, SpellID)
                    count -= 1
                    If count <= 0 AndAlso SPELLs(SpellID).MaxTargets > 0 Then Exit For
                Next

            ElseIf (Target.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
                For Each dynamic As WS_DynamicObjects.DynamicObjectObject In CType(Caster, WS_Base.BaseUnit).dynamicObjects.ToArray
                    If dynamic.SpellID = SpellID Then
                        dynamic.AddEffect(SpellInfo)
                        Exit For
                    End If
                Next
            End If

            Return result
        End Function

        Public Function SPELL_EFFECT_APPLY_AREA_AURA(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            For Each u As WS_Base.BaseUnit In Infected
                ApplyAura(u, Caster, SpellInfo, SpellID)
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_PERSISTENT_AREA_AURA(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If (Target.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) = 0 Then Return SpellFailedReason.SPELL_FAILED_BAD_IMPLICIT_TARGETS

            _WorldServer.Log.WriteLine(LogType.DEBUG, "Amplitude: {0}", SpellInfo.Amplitude)
            Dim tmpDO As New WS_DynamicObjects.DynamicObjectObject(Caster, SpellID, Target.dstX, Target.dstY, Target.dstZ, SPELLs(SpellID).GetDuration, SpellInfo.GetRadius)
            tmpDO.AddEffect(SpellInfo)
            tmpDO.Bytes = &H1EEEEEE
            CType(Caster, WS_Base.BaseUnit).dynamicObjects.Add(tmpDO)
            tmpDO.Spawn()

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_CREATE_ITEM(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Not TypeOf Target.unitTarget Is WS_PlayerData.CharacterObject Then Return SpellFailedReason.SPELL_FAILED_BAD_TARGETS
            Dim Amount As Integer = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level - SPELLs(SpellID).spellLevel)
            If Amount < 0 Then Return SpellFailedReason.SPELL_FAILED_ERROR
            If _WorldServer.ITEMDatabase.ContainsKey(SpellInfo.ItemType) = False Then
                Dim tmpInfo As New WS_Items.ItemInfo(SpellInfo.ItemType)
                _WorldServer.ITEMDatabase.Add(SpellInfo.ItemType, tmpInfo)
            End If
            If Amount > _WorldServer.ITEMDatabase(SpellInfo.ItemType).Stackable Then Amount = _WorldServer.ITEMDatabase(SpellInfo.ItemType).Stackable

            Dim Targets As List(Of WS_Base.BaseUnit) = GetFriendPlayersAroundMe(Caster, SpellInfo.GetRadius)
            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    Dim tmpItem As New ItemObject(SpellInfo.ItemType, Unit.GUID) With {
                            .StackCount = Amount
                            }

                    If Not CType(Unit, WS_PlayerData.CharacterObject).ItemADD(tmpItem) Then
                        tmpItem.Delete()
                    Else
                        CType(Target.unitTarget, WS_PlayerData.CharacterObject).LogLootItem(tmpItem, tmpItem.StackCount, False, True)
                    End If
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_RESURRECT(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseObject In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    'DONE: Character has already been requested a resurrect
                    If CType(Unit, WS_PlayerData.CharacterObject).resurrectGUID <> 0 Then
                        If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                            Dim RessurectFailed As New Packets.PacketClass(OPCODES.SMSG_RESURRECT_FAILED)
                            CType(Caster, WS_PlayerData.CharacterObject).client.Send(RessurectFailed)
                            RessurectFailed.Dispose()
                        End If
                        Return SpellFailedReason.SPELL_NO_ERROR
                    End If

                    'DONE: Save resurrection data
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectGUID = Caster.GUID
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectMapID = Caster.MapID
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectPositionX = Caster.positionX
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectPositionY = Caster.positionY
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectPositionZ = Caster.positionZ
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectHealth = CType(Unit, WS_PlayerData.CharacterObject).Life.Maximum * SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) \ 100
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectMana = CType(Unit, WS_PlayerData.CharacterObject).Mana.Maximum * SpellInfo.MiscValue \ 100

                    'DONE: Send a resurrection request
                    Dim RessurectRequest As New Packets.PacketClass(OPCODES.SMSG_RESURRECT_REQUEST)
                    RessurectRequest.AddUInt64(Caster.GUID)
                    RessurectRequest.AddUInt32(1)
                    RessurectRequest.AddUInt16(0)
                    RessurectRequest.AddUInt32(1)
                    CType(Unit, WS_PlayerData.CharacterObject).client.Send(RessurectRequest)
                    RessurectRequest.Dispose()
                ElseIf TypeOf Unit Is WS_Creatures.CreatureObject Then
                    'DONE: Ressurect pets
                    Target.unitTarget.Life.Current = CType(Unit, WS_Creatures.CreatureObject).Life.Maximum * SpellInfo.valueBase \ 100
                    Target.unitTarget.cUnitFlags = Target.unitTarget.cUnitFlags And (Not UnitFlags.UNIT_FLAG_DEAD)
                    Dim packetForNear As New Packets.UpdatePacketClass
                    Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CType(Unit, WS_Creatures.CreatureObject).Life.Current)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Target.unitTarget.cUnitFlags)
                    UpdateData.AddToPacket(packetForNear, ObjectUpdateType.UPDATETYPE_VALUES, CType(Unit, WS_Creatures.CreatureObject))
                    CType(Unit, WS_Creatures.CreatureObject).SendToNearPlayers(packetForNear)
                    packetForNear.Dispose()
                    UpdateData.Dispose()

                    CType(Target.unitTarget, WS_Creatures.CreatureObject).MoveToInstant(Caster.positionX, Caster.positionY, Caster.positionZ, Caster.orientation)
                ElseIf TypeOf Unit Is WS_Corpses.CorpseObject Then
                    If _WorldServer.CHARACTERs.ContainsKey(CType(Unit, WS_Corpses.CorpseObject).Owner) Then
                        'DONE: Save resurrection data
                        With _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner)
                            .resurrectGUID = Caster.GUID
                            .resurrectMapID = Caster.MapID
                            .resurrectPositionX = Caster.positionX
                            .resurrectPositionY = Caster.positionY
                            .resurrectPositionZ = Caster.positionZ
                            .resurrectHealth = .Life.Maximum * SpellInfo.valueBase \ 100
                            .resurrectMana = .Mana.Maximum * SpellInfo.MiscValue \ 100

                            'DONE: Send request to corpse owner
                            Dim RessurectRequest As New Packets.PacketClass(OPCODES.SMSG_RESURRECT_REQUEST)
                            RessurectRequest.AddUInt64(Caster.GUID)
                            RessurectRequest.AddUInt32(1)
                            RessurectRequest.AddUInt16(0)
                            RessurectRequest.AddUInt32(1)
                            .client.Send(RessurectRequest)
                            RessurectRequest.Dispose()
                        End With
                    End If
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_RESURRECT_NEW(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseObject In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    'DONE: Character has already been requested a resurrect
                    If CType(Unit, WS_PlayerData.CharacterObject).resurrectGUID <> 0 Then
                        If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                            Dim RessurectFailed As New Packets.PacketClass(OPCODES.SMSG_RESURRECT_FAILED)
                            CType(Caster, WS_PlayerData.CharacterObject).client.Send(RessurectFailed)
                            RessurectFailed.Dispose()
                        End If
                        Return SpellFailedReason.SPELL_NO_ERROR
                    End If

                    'DONE: Save resurrection data
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectGUID = Caster.GUID
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectMapID = Caster.MapID
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectPositionX = Caster.positionX
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectPositionY = Caster.positionY
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectPositionZ = Caster.positionZ
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectHealth = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                    CType(Unit, WS_PlayerData.CharacterObject).resurrectMana = SpellInfo.MiscValue

                    'DONE: Send a resurrection request
                    Dim RessurectRequest As New Packets.PacketClass(OPCODES.SMSG_RESURRECT_REQUEST)
                    RessurectRequest.AddUInt64(Caster.GUID)
                    RessurectRequest.AddUInt32(1)
                    RessurectRequest.AddUInt16(0)
                    RessurectRequest.AddUInt32(1)
                    CType(Unit, WS_PlayerData.CharacterObject).client.Send(RessurectRequest)
                    RessurectRequest.Dispose()
                ElseIf TypeOf Unit Is WS_Creatures.CreatureObject Then
                    'DONE: Ressurect pets
                    CType(Unit, WS_Creatures.CreatureObject).Life.Current = CType(Unit, WS_Creatures.CreatureObject).Life.Maximum * SpellInfo.valueBase \ 100
                    Dim packetForNear As New Packets.UpdatePacketClass
                    Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CType(Unit, WS_Creatures.CreatureObject).Life.Current)
                    UpdateData.AddToPacket(packetForNear, ObjectUpdateType.UPDATETYPE_VALUES, CType(Unit, WS_Creatures.CreatureObject))
                    CType(Unit, WS_Creatures.CreatureObject).SendToNearPlayers(packetForNear)
                    packetForNear.Dispose()
                    UpdateData.Dispose()

                    CType(Target.unitTarget, WS_Creatures.CreatureObject).MoveToInstant(Caster.positionX, Caster.positionY, Caster.positionZ, Caster.orientation)
                ElseIf TypeOf Unit Is WS_Corpses.CorpseObject Then
                    If _WorldServer.CHARACTERs.ContainsKey(CType(Unit, WS_Corpses.CorpseObject).Owner) Then
                        'DONE: Save resurrection data
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).resurrectGUID = Caster.GUID
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).resurrectMapID = Caster.MapID
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).resurrectPositionX = Caster.positionX
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).resurrectPositionY = Caster.positionY
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).resurrectPositionZ = Caster.positionZ
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).resurrectHealth = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).resurrectMana = SpellInfo.MiscValue

                        'DONE: Send request to corpse owner
                        Dim RessurectRequest As New Packets.PacketClass(OPCODES.SMSG_RESURRECT_REQUEST)
                        RessurectRequest.AddUInt64(Caster.GUID)
                        RessurectRequest.AddUInt32(1)
                        RessurectRequest.AddUInt16(0)
                        RessurectRequest.AddUInt32(1)
                        _WorldServer.CHARACTERs(CType(Unit, WS_Corpses.CorpseObject).Owner).client.Send(RessurectRequest)
                        RessurectRequest.Dispose()
                    End If
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_TELEPORT_GRAVEYARD(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    _WorldServer.AllGraveYards.GoToNearestGraveyard(CType(Unit, WS_PlayerData.CharacterObject), False, True)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_INTERRUPT_CAST(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    If CType(Unit, WS_PlayerData.CharacterObject).FinishAllSpells(False) Then
                        CType(Unit, WS_PlayerData.CharacterObject).ProhibitSpellSchool(SPELLs(SpellID).School, SPELLs(SpellID).GetDuration)
                    End If
                ElseIf TypeOf Unit Is WS_Creatures.CreatureObject Then
                    CType(Unit, WS_Creatures.CreatureObject).StopCasting()
                    'TODO: Prohibit spell school as with creatures
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_STEALTH(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                _Functions.SetFlag(Unit.cBytes1, 25, True)
                Unit.Invisibility = InvisibilityLevel.INIVISIBILITY
                Unit.Invisibility_Value = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then _WS_CharMovement.UpdateCell(CType(Unit, WS_PlayerData.CharacterObject))
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_DETECT(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                Unit.CanSeeInvisibility = InvisibilityLevel.INIVISIBILITY
                Unit.CanSeeInvisibility_Stealth = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then _WS_CharMovement.UpdateCell(CType(Unit, WS_PlayerData.CharacterObject))
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_LEAP(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Dim selectedX As Single = Caster.positionX + Math.Cos(Caster.orientation) * SpellInfo.GetRadius
            Dim selectedY As Single = Caster.positionY + Math.Sin(Caster.orientation) * SpellInfo.GetRadius
            Dim selectedZ As Single = _WS_Maps.GetZCoord(selectedX, selectedY, Caster.positionZ, Caster.MapID)
            If Math.Abs(Caster.positionZ - selectedZ) > SpellInfo.GetRadius Then
                'DONE: Special case if caster is above the ground
                selectedX = Caster.positionX
                selectedY = Caster.positionY
                selectedZ = Caster.positionZ - SpellInfo.GetRadius
            End If

            'DONE: Check if we hit something
            Dim hitX As Single, hitY As Single, hitZ As Single
            If _WS_Maps.GetObjectHitPos(Caster, selectedX, selectedY, selectedZ + 2.0F, hitX, hitY, hitZ, -1.0F) Then
                selectedX = hitX
                selectedY = hitY
                selectedZ = hitZ + 0.2F
            End If

            If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                CType(Caster, WS_PlayerData.CharacterObject).Teleport(selectedX, selectedY, selectedZ, Caster.orientation, Caster.MapID)
            Else
                CType(Caster, WS_Creatures.CreatureObject).MoveToInstant(selectedX, selectedY, selectedZ, Caster.orientation)
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_SUMMON(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            'Select Case SpellInfo.MiscValueB
            '    Case SummonType.SUMMON_TYPE_GUARDIAN, SummonType.SUMMON_TYPE_POSESSED, SummonType.SUMMON_TYPE_POSESSED2
            '        _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Summon Guardian")
            '    Case SummonType.SUMMON_TYPE_WILD
            '        Dim Duration As Integer = SPELLs(SpellID).GetDuration
            '        Dim Type As TempSummonType = TempSummonType.TEMPSUMMON_TIMED_OR_DEAD_DESPAWN
            '        If Duration = 0 Then Type = TempSummonType.TEMPSUMMON_DEAD_DESPAWN

            '        Dim SelectedX As Single, SelectedY As Single, SelectedZ As Single
            '        If (Target.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
            '            SelectedX = Target.dstX
            '            SelectedY = Target.dstY
            '            SelectedZ = Target.dstZ
            '        Else
            '            SelectedX = Caster.positionX
            '            SelectedY = Caster.positionY
            '            SelectedZ = Caster.positionZ
            '        End If

            '        Dim tmpCreature As New CreatureObject(SpellInfo.MiscValue, SelectedX, SelectedY, SelectedZ, Caster.orientation, Caster.MapID, Duration)
            '        'TODO: Level by engineering skill level
            '        tmpCreature.Level = CType(Caster, BaseUnit).Level
            '        tmpCreature.CreatedBy = Caster.GUID
            '        tmpCreature.CreatedBySpell = SpellID
            '        tmpCreature.AddToWorld()
            '    Case SummonType.SUMMON_TYPE_DEMON
            '        _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Summon Demon")
            '    Case SummonType.SUMMON_TYPE_SUMMON
            '        _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Summon")
            '    Case SummonType.SUMMON_TYPE_CRITTER, SummonType.SUMMON_TYPE_CRITTER2
            '        If CType(Caster, CharacterObject).NonCombatPet IsNot Nothing AndAlso CType(Caster, CharacterObject).NonCombatPet.ID = SpellInfo.MiscValue Then
            '            CType(Caster, CharacterObject).NonCombatPet.Destroy()
            '            CType(Caster, CharacterObject).NonCombatPet = Nothing
            '            Return SpellFailedReason.SPELL_NO_ERROR
            '        End If
            '        If CType(Caster, CharacterObject).NonCombatPet IsNot Nothing Then
            '            CType(Caster, CharacterObject).NonCombatPet.Destroy()
            '        End If

            '        Dim SelectedX As Single, SelectedY As Single, SelectedZ As Single
            '        If (Target.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
            '            SelectedX = Target.dstX
            '            SelectedY = Target.dstY
            '            SelectedZ = Target.dstZ
            '        Else
            '            SelectedX = Caster.positionX
            '            SelectedY = Caster.positionY
            '            SelectedZ = Caster.positionZ
            '        End If
            '        CType(Caster, CharacterObject).NonCombatPet = New CreatureObject(SpellInfo.MiscValue, SelectedX, SelectedY, SelectedZ, Caster.orientation, Caster.MapID, SPELLs(SpellID).GetDuration)
            '        CType(Caster, CharacterObject).NonCombatPet.SummonedBy = Caster.GUID
            '        CType(Caster, CharacterObject).NonCombatPet.CreatedBy = Caster.GUID
            '        CType(Caster, CharacterObject).NonCombatPet.CreatedBySpell = SpellID
            '        CType(Caster, CharacterObject).NonCombatPet.Faction = CType(Caster, CharacterObject).Faction
            '        CType(Caster, CharacterObject).NonCombatPet.Level = 1
            '        CType(Caster, CharacterObject).NonCombatPet.Life.Base = 1
            '        CType(Caster, CharacterObject).NonCombatPet.Life.Current = 1
            '        CType(Caster, CharacterObject).NonCombatPet.AddToWorld()

            '    Case SummonType.SUMMON_TYPE_TOTEM, SummonType.SUMMON_TYPE_TOTEM_SLOT1, SummonType.SUMMON_TYPE_TOTEM_SLOT2, SummonType.SUMMON_TYPE_TOTEM_SLOT3, SummonType.SUMMON_TYPE_TOTEM_SLOT4
            '        Dim Slot As Byte = 0
            '        Select Case SpellInfo.MiscValueB
            '            Case SummonType.SUMMON_TYPE_TOTEM_SLOT1
            '                Slot = 0
            '            Case SummonType.SUMMON_TYPE_TOTEM_SLOT2
            '                Slot = 1
            '            Case SummonType.SUMMON_TYPE_TOTEM_SLOT3
            '                Slot = 2
            '            Case SummonType.SUMMON_TYPE_TOTEM_SLOT4
            '                Slot = 3
            '            Case SummonType.SUMMON_TYPE_TOTEM
            '                Slot = 254
            '            Case SummonType.SUMMON_TYPE_GUARDIAN
            '                Slot = 255
            '        End Select

            '        _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Totem Slot [{0}].", Slot)

            '        'Normal shaman totem
            '        If Slot < 4 Then
            '            Dim GUID As ULong = CType(Caster, CharacterObject).TotemSlot(Slot)
            '            If GUID <> 0 Then
            '                If _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) Then
            '                    _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Destroyed old totem.")
            '                    _WorldServer.WORLD_CREATUREs(GUID).Destroy()
            '                End If
            '            End If
            '        End If

            '        Dim angle As Single = 0
            '        If Slot < 4 Then angle = Math.PI / 4 - (Slot * 2 * Math.PI / 4)

            '        Dim selectedX As Single = Caster.positionX + Math.Cos(Caster.orientation) * 2
            '        Dim selectedY As Single = Caster.positionY + Math.Sin(Caster.orientation) * 2
            '        Dim selectedZ As Single = GetZCoord(selectedX, selectedY, Caster.positionZ, Caster.MapID)
            '        If Math.Abs(Caster.positionZ - selectedZ) > 5 Then selectedZ = Caster.positionZ

            '        Dim NewTotem As New TotemObject(SpellInfo.MiscValue, selectedX, selectedY, selectedZ, angle, Caster.MapID, SPELLs(SpellID).GetDuration)
            '        NewTotem.Life.Base = SpellInfo.GetValue(CType(Caster, BaseUnit).Level)
            '        NewTotem.Life.Current = NewTotem.Life.Maximum
            '        NewTotem.Caster = Caster
            '        NewTotem.Level = CType(Caster, BaseUnit).Level
            '        NewTotem.SummonedBy = Caster.GUID
            '        NewTotem.CreatedBy = Caster.GUID
            '        NewTotem.CreatedBySpell = SpellID
            '        If TypeOf Caster Is CharacterObject Then
            '            NewTotem.Faction = CType(Caster, CharacterObject).Faction
            '        ElseIf TypeOf Caster Is CreatureObject Then
            '            NewTotem.Faction = CType(Caster, CreatureObject).Faction
            '        End If
            '        Select Case SpellID
            '            Case 25547
            '                NewTotem.InitSpell(25539)
            '            Case 25359
            '                NewTotem.InitSpell(25360)
            '            Case 2484
            '                NewTotem.InitSpell(6474)
            '            Case 8170
            '                NewTotem.InitSpell(8172)
            '            Case 8166
            '                NewTotem.InitSpell(8179)
            '            Case 8177
            '                NewTotem.InitSpell(8167)
            '            Case 5675
            '                NewTotem.InitSpell(5677)
            '            Case 10495
            '                NewTotem.InitSpell(10491)
            '            Case 10496
            '                NewTotem.InitSpell(10493)
            '            Case 10497
            '                NewTotem.InitSpell(10494)
            '            Case 25570
            '                NewTotem.InitSpell(25569)
            '            Case 25552
            '                NewTotem.InitSpell(25551)
            '            Case 25587
            '                NewTotem.InitSpell(25582)
            '            Case 16190
            '                NewTotem.InitSpell(16191)
            '            Case 25528
            '                NewTotem.InitSpell(25527)
            '            Case 8143
            '                NewTotem.InitSpell(8145)
            '        End Select
            '        NewTotem.AddToWorld()
            '        _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Totem spawned [{0:X}].", NewTotem.GUID)

            '        If Slot < 4 AndAlso TypeOf Caster Is CharacterObject Then
            '            CType(Caster, CharacterObject).TotemSlot(Slot) = NewTotem.GUID

            '            'Dim TotemCreated As New PacketClass(OPCODES.SMSG_TOTEM_CREATED)
            '            'TotemCreated.AddInt8(Slot)
            '            'TotemCreated.AddUInt64(NewTotem.GUID)
            '            'TotemCreated.AddInt32(SPELLs(SpellID).GetDuration)
            '            'TotemCreated.AddInt32(SpellID)
            '            'CType(Caster, CharacterObject).Client.Send(TotemCreated)
            '            'TotemCreated.Dispose()
            '        End If
            '    Case Else
            '        _WorldServer.Log.WriteLine(LogType.DEBUG, "Unknown SummonType: {0}", SpellInfo.MiscValueB)
            '        Exit Function
            'End Select

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_SUMMON_WILD(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Dim Duration As Integer = SPELLs(SpellID).GetDuration
            Dim Type As TempSummonType = TempSummonType.TEMPSUMMON_TIMED_OR_DEAD_DESPAWN
            If Duration = 0 Then Type = TempSummonType.TEMPSUMMON_DEAD_DESPAWN

            Dim SelectedX As Single, SelectedY As Single, SelectedZ As Single
            If (Target.targetMask And SpellCastTargetFlags.TARGET_FLAG_DEST_LOCATION) Then
                SelectedX = Target.dstX
                SelectedY = Target.dstY
                SelectedZ = Target.dstZ
            Else
                SelectedX = Caster.positionX
                SelectedY = Caster.positionY
                SelectedZ = Caster.positionZ
            End If

            'TODO: Level by engineering skill level
            Dim tmpCreature As New WS_Creatures.CreatureObject(SpellInfo.MiscValue, SelectedX, SelectedY, SelectedZ, Caster.orientation, Caster.MapID, Duration) With {
                    .Level = CType(Caster, WS_Base.BaseUnit).Level,
                    .CreatedBy = Caster.GUID,
                    .CreatedBySpell = SpellID
                    }
            tmpCreature.AddToWorld()

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_SUMMON_TOTEM(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            Dim Slot As Byte = 0
            Select Case SpellInfo.ID
                Case SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT1
                    Slot = 0
                Case SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT2
                    Slot = 1
                Case SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT3
                    Slot = 2
                Case SpellEffects_Names.SPELL_EFFECT_SUMMON_TOTEM_SLOT4
                    Slot = 3
                Case Else
                    Exit Function
            End Select

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Totem Slot [{0}].", Slot)

            'Normal shaman totem
            If Slot < 4 Then
                Dim GUID As ULong = CType(Caster, WS_PlayerData.CharacterObject).TotemSlot(Slot)
                If GUID <> 0 Then
                    If _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) Then
                        _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Destroyed old totem.")
                        _WorldServer.WORLD_CREATUREs(GUID).Destroy()
                    End If
                End If
            End If

            Dim angle As Single = 0
            If Slot < 4 Then angle = Math.PI / 4 - (Slot * 2 * Math.PI / 4)

            Dim selectedX As Single = Caster.positionX + Math.Cos(Caster.orientation) * 2
            Dim selectedY As Single = Caster.positionY + Math.Sin(Caster.orientation) * 2
            Dim selectedZ As Single = _WS_Maps.GetZCoord(selectedX, selectedY, Caster.positionZ, Caster.MapID)

            Dim NewTotem As New WS_Totems.TotemObject(SpellInfo.MiscValue, selectedX, selectedY, selectedZ, angle, Caster.MapID, SPELLs(SpellID).GetDuration)
            NewTotem.Life.Base = SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
            NewTotem.Life.Current = NewTotem.Life.Maximum
            NewTotem.Caster = Caster
            NewTotem.Level = CType(Caster, WS_Base.BaseUnit).Level
            NewTotem.SummonedBy = Caster.GUID
            NewTotem.CreatedBy = Caster.GUID
            NewTotem.CreatedBySpell = SpellID
            If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                NewTotem.Faction = CType(Caster, WS_PlayerData.CharacterObject).Faction
            ElseIf TypeOf Caster Is WS_Creatures.CreatureObject Then
                NewTotem.Faction = CType(Caster, WS_Creatures.CreatureObject).Faction
            End If

            If _WorldServer.CREATURESDatabase.ContainsKey(SpellInfo.MiscValue) = False Then
                Dim tmpInfo As New CreatureInfo(SpellInfo.MiscValue)
                _WorldServer.CREATURESDatabase.Add(SpellInfo.MiscValue, tmpInfo)
            End If
            NewTotem.InitSpell(_WorldServer.CREATURESDatabase(SpellInfo.MiscValue).Spells(0))
            NewTotem.AddToWorld()
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Totem spawned [{0:X}].", NewTotem.GUID)

            If Slot < 4 AndAlso TypeOf Caster Is WS_PlayerData.CharacterObject Then
                CType(Caster, WS_PlayerData.CharacterObject).TotemSlot(Slot) = NewTotem.GUID

                'Dim TotemCreated As New PacketClass(OPCODES.SMSG_TOTEM_CREATED)
                'TotemCreated.AddInt8(Slot)
                'TotemCreated.AddUInt64(NewTotem.GUID)
                'TotemCreated.AddInt32(SPELLs(SpellID).GetDuration)
                'TotemCreated.AddInt32(SpellID)
                'CType(Caster, CharacterObject).Client.Send(TotemCreated)
                'TotemCreated.Dispose()
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_SUMMON_OBJECT(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Not TypeOf Caster Is WS_Base.BaseUnit Then Return SpellFailedReason.SPELL_FAILED_CASTER_DEAD

            Dim selectedX As Single = 0.0F
            Dim selectedY As Single = 0.0F
            Dim selectedZ As Single = 0.0F

            If SpellInfo.RadiusIndex > 0 Then
                selectedX = Caster.positionX + Math.Cos(Caster.orientation) * SpellInfo.GetRadius
                selectedY = Caster.positionY + Math.Sin(Caster.orientation) * SpellInfo.GetRadius
            Else
                selectedX = Caster.positionX + Math.Cos(Caster.orientation) * SPELLs(SpellID).GetRange
                selectedY = Caster.positionY + Math.Sin(Caster.orientation) * SPELLs(SpellID).GetRange
            End If

            Dim GameobjectInfo As WS_GameObjects.GameObjectInfo
            If _WorldServer.GAMEOBJECTSDatabase.ContainsKey(SpellInfo.MiscValue) = False Then
                GameobjectInfo = New WS_GameObjects.GameObjectInfo(SpellInfo.MiscValue)
            Else
                GameobjectInfo = _WorldServer.GAMEOBJECTSDatabase(SpellInfo.MiscValue)
            End If

            If GameobjectInfo.Type = GameObjectType.GAMEOBJECT_TYPE_FISHINGNODE Then
                selectedZ = _WS_Maps.GetWaterLevel(selectedX, selectedY, Caster.MapID)
            Else
                selectedZ = _WS_Maps.GetZCoord(selectedX, selectedY, Caster.positionZ, Caster.MapID)
            End If

            Dim tmpGO As New WS_GameObjects.GameObjectObject(SpellInfo.MiscValue, Caster.MapID, selectedX, selectedY, selectedZ, Caster.orientation, Caster.GUID) With {
                    .CreatedBySpell = SpellID,
                    .Level = CType(Caster, WS_Base.BaseUnit).Level,
                    .instance = Caster.instance
                    }
            CType(Caster, WS_Base.BaseUnit).gameObjects.Add(tmpGO)

            If GameobjectInfo.Type = GameObjectType.GAMEOBJECT_TYPE_FISHINGNODE Then
                tmpGO.SetupFishingNode()
            End If

            tmpGO.AddToWorld()

            Dim packet As New Packets.PacketClass(OPCODES.SMSG_GAMEOBJECT_SPAWN_ANIM)
            packet.AddUInt64(tmpGO.GUID)
            tmpGO.SendToNearPlayers(packet)
            packet.Dispose()

            If GameobjectInfo.Type = GameObjectType.GAMEOBJECT_TYPE_FISHINGNODE Then
                CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_CHANNEL_SPELL, SpellID)
                CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_CHANNEL_OBJECT, tmpGO.GUID)
                CType(Caster, WS_PlayerData.CharacterObject).SendCharacterUpdate()
            End If

            'DONE: Despawn the object after the duration
            If SPELLs(SpellID).GetDuration > 0 Then
                tmpGO.Despawn(SPELLs(SpellID).GetDuration)
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_ENCHANT_ITEM(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Target.itemTarget Is Nothing Then Return SpellFailedReason.SPELL_FAILED_ITEM_NOT_FOUND

            'TODO: If there already is an enchantment here, ask for permission?

            Target.itemTarget.AddEnchantment(SpellInfo.MiscValue, EnchantSlots.ENCHANTMENT_PERM)
            If _WorldServer.CHARACTERs.ContainsKey(Target.itemTarget.OwnerGUID) Then
                _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).SendItemUpdate(Target.itemTarget)

                Dim EnchantLog As New Packets.PacketClass(OPCODES.SMSG_ENCHANTMENTLOG)
                EnchantLog.AddUInt64(Target.itemTarget.OwnerGUID)
                EnchantLog.AddUInt64(Caster.GUID)
                EnchantLog.AddInt32(Target.itemTarget.ItemEntry)
                EnchantLog.AddInt32(SpellInfo.MiscValue)
                EnchantLog.AddInt8(0)
                _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).client.Send(EnchantLog)
                'DONE: Send to trader also
                If _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).tradeInfo IsNot Nothing Then
                    If _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).tradeInfo.Trader Is _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID) Then
                        _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).tradeInfo.SendTradeUpdateToTarget()
                    Else
                        _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).tradeInfo.SendTradeUpdateToTrader()
                    End If
                End If
                EnchantLog.Dispose()
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_ENCHANT_ITEM_TEMPORARY(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If Target.itemTarget Is Nothing Then Return SpellFailedReason.SPELL_FAILED_ITEM_NOT_FOUND

            'TODO: If there already is an enchantment here, ask for permission?

            Dim Duration As Integer = SPELLs(SpellID).GetDuration
            If Duration = 0 Then
                If SPELLs(SpellID).SpellVisual = 563 Then ' Fishing
                    Duration = 600 '10 mins
                ElseIf SPELLs(SpellID).SpellFamilyName = SpellFamilyNames.SPELLFAMILY_ROGUE Then
                    Duration = 3600 '1 hour
                ElseIf SPELLs(SpellID).SpellFamilyName = SpellFamilyNames.SPELLFAMILY_SHAMAN Then
                    Duration = 1800 '30 mins
                ElseIf SPELLs(SpellID).SpellVisual = 215 Then
                    Duration = 1800 '30 mins
                ElseIf SPELLs(SpellID).SpellVisual = 0 Then ' Shaman Rockbiter Weapon
                    Duration = 1800 '30 mins
                Else
                    Duration = 3600 '1 hour
                End If

                Duration *= 1000
            End If

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Enchant duration [{0}]", Duration)

            Target.itemTarget.AddEnchantment(SpellInfo.MiscValue, EnchantSlots.ENCHANTMENT_TEMP, Duration)
            If _WorldServer.CHARACTERs.ContainsKey(Target.itemTarget.OwnerGUID) Then
                _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).SendItemUpdate(Target.itemTarget)

                Dim EnchantLog As New Packets.PacketClass(OPCODES.SMSG_ENCHANTMENTLOG)
                EnchantLog.AddUInt64(Target.itemTarget.OwnerGUID)
                EnchantLog.AddUInt64(Caster.GUID)
                EnchantLog.AddInt32(Target.itemTarget.ItemEntry)
                EnchantLog.AddInt32(SpellInfo.MiscValue)
                EnchantLog.AddInt8(0)
                _WorldServer.CHARACTERs(Target.itemTarget.OwnerGUID).client.Send(EnchantLog)
                EnchantLog.Dispose()
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_ENCHANT_HELD_ITEM(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            'TODO: If there already is an enchantment here, ask for permission?

            Dim Duration As Integer = SPELLs(SpellID).GetDuration
            If Duration = 0 Then Duration = (SpellInfo.valueBase + 1) * 1000
            If Duration = 0 Then Duration = 10000
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Enchant duration [{0}]({1})", Duration, SpellInfo.valueBase)

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject AndAlso CType(Unit, WS_PlayerData.CharacterObject).Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) Then
                    If CType(Unit, WS_PlayerData.CharacterObject).Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).Enchantments.ContainsKey(EnchantSlots.ENCHANTMENT_TEMP) AndAlso CType(Unit, WS_PlayerData.CharacterObject).Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).Enchantments(EnchantSlots.ENCHANTMENT_TEMP).ID = SpellInfo.MiscValue Then
                        CType(Unit, WS_PlayerData.CharacterObject).Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).AddEnchantment(SpellInfo.MiscValue, EnchantSlots.ENCHANTMENT_TEMP, Duration)
                        CType(Unit, WS_PlayerData.CharacterObject).SendItemUpdate(CType(Unit, WS_PlayerData.CharacterObject).Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND))
                    End If
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_CHARGE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If TypeOf Caster Is WS_Creatures.CreatureObject Then
                CType(Caster, WS_Creatures.CreatureObject).SetToRealPosition()
            End If

            Dim NearX As Single = Target.unitTarget.positionX
            If Target.unitTarget.positionX > Caster.positionX Then NearX -= 1.0F Else NearX += 1.0F
            Dim NearY As Single = Target.unitTarget.positionY
            If Target.unitTarget.positionY > Caster.positionY Then NearY -= 1.0F Else NearY += 1.0F
            Dim NearZ As Single = _WS_Maps.GetZCoord(NearX, NearY, Caster.positionZ, Caster.MapID)
            If NearZ > (Target.unitTarget.positionZ + 2) Or NearZ < (Target.unitTarget.positionZ - 2) Then NearZ = Target.unitTarget.positionZ

            Dim moveDist As Single = _WS_Combat.GetDistance(Caster, NearX, NearY, NearZ)
            Dim TimeToMove As Integer = moveDist / SPELLs(SpellID).Speed * 1000.0F

            Dim SMSG_MONSTER_MOVE As New Packets.PacketClass(OPCODES.SMSG_MONSTER_MOVE)
            SMSG_MONSTER_MOVE.AddPackGUID(Caster.GUID)
            SMSG_MONSTER_MOVE.AddSingle(Caster.positionX)
            SMSG_MONSTER_MOVE.AddSingle(Caster.positionY)
            SMSG_MONSTER_MOVE.AddSingle(Caster.positionZ)
            SMSG_MONSTER_MOVE.AddInt32(_NativeMethods.timeGetTime(""))         'Sequence/MSTime?
            SMSG_MONSTER_MOVE.AddInt8(0)
            SMSG_MONSTER_MOVE.AddInt32(&H100)
            SMSG_MONSTER_MOVE.AddInt32(TimeToMove)  'Time
            SMSG_MONSTER_MOVE.AddInt32(1)           'Points Count
            SMSG_MONSTER_MOVE.AddSingle(NearX)          'First Point X
            SMSG_MONSTER_MOVE.AddSingle(NearY)          'First Point Y
            SMSG_MONSTER_MOVE.AddSingle(NearZ)          'First Point Z

            Caster.SendToNearPlayers(SMSG_MONSTER_MOVE)
            SMSG_MONSTER_MOVE.Dispose()

            If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                _WS_Combat.SendAttackStart(Caster.GUID, Target.unitTarget.GUID, CType(Caster, WS_PlayerData.CharacterObject).client)
                CType(Caster, WS_PlayerData.CharacterObject).attackState.AttackStart(Target.unitTarget)
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_KNOCK_BACK(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            For Each Unit As WS_Base.BaseUnit In Infected
                Dim Direction As Single = _WS_Combat.GetOrientation(Caster.positionX, Unit.positionX, Caster.positionY, Unit.positionY)

                Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_KNOCK_BACK)
                packet.AddPackGUID(Unit.GUID)
                packet.AddInt32(0)
                packet.AddSingle(Math.Cos(Direction)) 'X-direction
                packet.AddSingle(Math.Sin(Direction)) 'Y-direction
                packet.AddSingle(SpellInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 10.0F) 'horizontal speed
                packet.AddSingle(SpellInfo.MiscValue / -10.0F) 'Z-speed
                Unit.SendToNearPlayers(packet)
                packet.Dispose()

                If TypeOf Unit Is WS_Creatures.CreatureObject Then
                    'TODO: Calculate were the creature would fall, and pause the AI until it lands
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_SCRIPT_EFFECT(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason
            If SPELLs(SpellID).SpellFamilyName = SpellFamilyNames.SPELLFAMILY_PALADIN Then
                If SPELLs(SpellID).SpellIconID = 70 OrElse SPELLs(SpellID).SpellIconID = 242 Then
                    Return SPELL_EFFECT_HEAL(Target, Caster, SpellInfo, SpellID, Infected, Item)
                ElseIf (SPELLs(SpellID).SpellFamilyFlags And (1 << 23)) Then
                    If Target.unitTarget Is Nothing OrElse Target.unitTarget.IsDead Then Return SpellFailedReason.SPELL_FAILED_TARGETS_DEAD

                    Dim SpellID2 As Integer = 0
                    For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECTs_VISIBLE - 1
                        If CType(Caster, WS_Base.BaseUnit).ActiveSpells(i) IsNot Nothing AndAlso CType(Caster, WS_Base.BaseUnit).ActiveSpells(i).GetSpellInfo.SpellVisual = 5622 AndAlso CType(Caster, WS_Base.BaseUnit).ActiveSpells(i).GetSpellInfo.SpellFamilyName = SpellFamilyNames.SPELLFAMILY_PALADIN Then
                            If CType(Caster, WS_Base.BaseUnit).ActiveSpells(i).Aura_Info(2) IsNot Nothing Then
                                SpellID2 = CType(Caster, WS_Base.BaseUnit).ActiveSpells(i).Aura_Info(2).valueBase + 1
                                Exit For
                            End If
                        End If
                    Next

                    If SpellID2 = 0 OrElse SPELLs.ContainsKey(SpellID2) = False Then Return SpellFailedReason.SPELL_FAILED_UNKNOWN

                    Dim castParams As New CastSpellParameters(Target, Caster, SpellID2)
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
                End If
            End If

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_DUEL(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            Select Case SpellInfo.implicitTargetA
                Case SpellImplicitTargets.TARGET_DUEL_VS_PLAYER
                    If Not TypeOf Target.unitTarget Is WS_PlayerData.CharacterObject Then Return SpellFailedReason.SPELL_FAILED_TARGET_NOT_PLAYER
                    If Not TypeOf Caster Is WS_PlayerData.CharacterObject Then Exit Function

                    'TODO: Some more checks
                    If CType(Caster, WS_PlayerData.CharacterObject).DuelArbiter <> 0 Then Return SpellFailedReason.SPELL_FAILED_SPELL_IN_PROGRESS
                    If CType(Target.unitTarget, WS_PlayerData.CharacterObject).IsInDuel Then Return SpellFailedReason.SPELL_FAILED_TARGET_DUELING
                    If CType(Target.unitTarget, WS_PlayerData.CharacterObject).inCombatWith.Count > 0 Then Return SpellFailedReason.SPELL_FAILED_TARGET_IN_COMBAT
                    If Caster.Invisibility <> InvisibilityLevel.VISIBLE Then Return SpellFailedReason.SPELL_FAILED_CANT_DUEL_WHILE_INVISIBLE
                    'CAST_FAIL_CANT_START_DUEL_STEALTHED
                    'CAST_FAIL_NO_DUELING_HERE

                    'DONE: Get middle coordinate
                    Dim flagX As Single = Caster.positionX + (Target.unitTarget.positionX - Caster.positionX) / 2
                    Dim flagY As Single = Caster.positionY + (Target.unitTarget.positionY - Caster.positionY) / 2
                    Dim flagZ As Single = _WS_Maps.GetZCoord(flagX, flagY, Caster.positionZ + 3.0F, Caster.MapID)

                    'DONE: Spawn duel flag (GO Entry in SpellInfo.MiscValue) in middle of the 2 players
                    Dim tmpGO As WS_GameObjects.GameObjectObject = New WS_GameObjects.GameObjectObject(SpellInfo.MiscValue, Caster.MapID, flagX, flagY, flagZ, 0, Caster.GUID)
                    tmpGO.AddToWorld()

                    'DONE: Set duel arbiter and parner
                    'CType(Caster, CharacterObject).DuelArbiter = tmpGO.GUID        Commented to fix 2 packets for duel accept
                    CType(Target.unitTarget, WS_PlayerData.CharacterObject).DuelArbiter = tmpGO.GUID
                    CType(Caster, WS_PlayerData.CharacterObject).DuelPartner = CType(Target.unitTarget, WS_PlayerData.CharacterObject)
                    CType(Target.unitTarget, WS_PlayerData.CharacterObject).DuelPartner = CType(Caster, WS_PlayerData.CharacterObject)

                    'DONE: Send duel request packet
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_DUEL_REQUESTED)
                    packet.AddUInt64(tmpGO.GUID)
                    packet.AddUInt64(Caster.GUID)
                    CType(Target.unitTarget, WS_PlayerData.CharacterObject).client.SendMultiplyPackets(packet)
                    CType(Caster, WS_PlayerData.CharacterObject).client.SendMultiplyPackets(packet)
                    packet.Dispose()
                Case Else
                    Return SpellFailedReason.SPELL_FAILED_BAD_IMPLICIT_TARGETS
            End Select

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function SPELL_EFFECT_QUEST_COMPLETE(ByRef Target As SpellTargets, ByRef Caster As WS_Base.BaseObject, ByRef SpellInfo As SpellEffect, ByVal SpellID As Integer, ByRef Infected As List(Of WS_Base.BaseObject), ByRef Item As ItemObject) As SpellFailedReason

            For Each Unit As WS_Base.BaseUnit In Infected
                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    _WorldServer.ALLQUESTS.CompleteQuest(CType(Unit, WS_PlayerData.CharacterObject), SpellInfo.MiscValue, Caster.GUID)
                End If
            Next

            Return SpellFailedReason.SPELL_NO_ERROR
        End Function

        Public Function GetEnemyAtPoint(ByRef objCharacter As WS_Base.BaseUnit, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Distance As Single) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit)

            If TypeOf objCharacter Is WS_PlayerData.CharacterObject Then
                For Each pGUID As ULong In CType(objCharacter, WS_PlayerData.CharacterObject).playersNear.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso (CType(objCharacter, WS_PlayerData.CharacterObject).IsHorde <> _WorldServer.CHARACTERs(pGUID).IsHorde OrElse (CType(objCharacter, WS_PlayerData.CharacterObject).DuelPartner IsNot Nothing AndAlso CType(objCharacter, WS_PlayerData.CharacterObject).DuelPartner Is _WorldServer.CHARACTERs(pGUID))) AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), PosX, PosY, PosZ) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next

                For Each cGUID As ULong In CType(objCharacter, WS_PlayerData.CharacterObject).creaturesNear.ToArray
                    If _WorldServer.WORLD_CREATUREs.ContainsKey(cGUID) AndAlso (Not TypeOf _WorldServer.WORLD_CREATUREs(cGUID) Is WS_Totems.TotemObject) AndAlso _WorldServer.WORLD_CREATUREs(cGUID).IsDead = False AndAlso CType(objCharacter, WS_PlayerData.CharacterObject).GetReaction(_WorldServer.WORLD_CREATUREs(cGUID).Faction) <= TReaction.NEUTRAL Then
                        If _WS_Combat.GetDistance(_WorldServer.WORLD_CREATUREs(cGUID), PosX, PosY, PosZ) < Distance Then result.Add(_WorldServer.WORLD_CREATUREs(cGUID))
                    End If
                Next

            ElseIf TypeOf objCharacter Is WS_Creatures.CreatureObject Then
                For Each pGUID As ULong In objCharacter.SeenBy.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False AndAlso _WorldServer.CHARACTERs(pGUID).GetReaction(CType(objCharacter, WS_Creatures.CreatureObject).Faction) <= TReaction.NEUTRAL Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), PosX, PosY, PosZ) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next
            End If

            Return result
        End Function

        Public Function GetEnemyAroundMe(ByRef objCharacter As WS_Base.BaseUnit, ByVal Distance As Single, Optional ByRef r As WS_Base.BaseUnit = Nothing) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit)

            If r Is Nothing Then r = objCharacter
            If TypeOf r Is WS_PlayerData.CharacterObject Then
                For Each pGUID As ULong In CType(r, WS_PlayerData.CharacterObject).playersNear.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso (CType(r, WS_PlayerData.CharacterObject).IsHorde <> _WorldServer.CHARACTERs(pGUID).IsHorde OrElse (CType(r, WS_PlayerData.CharacterObject).DuelPartner IsNot Nothing AndAlso CType(r, WS_PlayerData.CharacterObject).DuelPartner Is _WorldServer.CHARACTERs(pGUID))) AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), objCharacter) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next

                For Each cGUID As ULong In CType(r, WS_PlayerData.CharacterObject).creaturesNear.ToArray
                    If _WorldServer.WORLD_CREATUREs.ContainsKey(cGUID) AndAlso (Not TypeOf _WorldServer.WORLD_CREATUREs(cGUID) Is WS_Totems.TotemObject) AndAlso _WorldServer.WORLD_CREATUREs(cGUID).IsDead = False AndAlso CType(r, WS_PlayerData.CharacterObject).GetReaction(_WorldServer.WORLD_CREATUREs(cGUID).Faction) <= TReaction.NEUTRAL Then
                        If _WS_Combat.GetDistance(_WorldServer.WORLD_CREATUREs(cGUID), objCharacter) < Distance Then result.Add(_WorldServer.WORLD_CREATUREs(cGUID))
                    End If
                Next

            ElseIf TypeOf r Is WS_Creatures.CreatureObject Then
                For Each pGUID As ULong In r.SeenBy.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False AndAlso _WorldServer.CHARACTERs(pGUID).GetReaction(CType(r, WS_Creatures.CreatureObject).Faction) <= TReaction.NEUTRAL Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), objCharacter) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next
            End If

            Return result
        End Function

        Public Function GetFriendAroundMe(ByRef objCharacter As WS_Base.BaseUnit, ByVal Distance As Single) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit)

            If TypeOf objCharacter Is WS_PlayerData.CharacterObject Then
                For Each pGUID As ULong In CType(objCharacter, WS_PlayerData.CharacterObject).playersNear.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso CType(objCharacter, WS_PlayerData.CharacterObject).IsHorde = _WorldServer.CHARACTERs(pGUID).IsHorde AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), objCharacter) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next

                For Each cGUID As ULong In CType(objCharacter, WS_PlayerData.CharacterObject).creaturesNear.ToArray
                    If _WorldServer.WORLD_CREATUREs.ContainsKey(cGUID) AndAlso (Not TypeOf _WorldServer.WORLD_CREATUREs(cGUID) Is WS_Totems.TotemObject) AndAlso _WorldServer.WORLD_CREATUREs(cGUID).IsDead = False AndAlso CType(objCharacter, WS_PlayerData.CharacterObject).GetReaction(_WorldServer.WORLD_CREATUREs(cGUID).Faction) > TReaction.NEUTRAL Then
                        If _WS_Combat.GetDistance(_WorldServer.WORLD_CREATUREs(cGUID), objCharacter) < Distance Then result.Add(_WorldServer.WORLD_CREATUREs(cGUID))
                    End If
                Next

            ElseIf TypeOf objCharacter Is WS_Creatures.CreatureObject Then
                For Each pGUID As ULong In objCharacter.SeenBy.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False AndAlso _WorldServer.CHARACTERs(pGUID).GetReaction(CType(objCharacter, WS_Creatures.CreatureObject).Faction) > TReaction.NEUTRAL Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), objCharacter) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next
            End If

            Return result
        End Function

        Public Function GetFriendPlayersAroundMe(ByRef objCharacter As WS_Base.BaseUnit, ByVal Distance As Single) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit)

            If TypeOf objCharacter Is WS_PlayerData.CharacterObject Then
                For Each pGUID As ULong In CType(objCharacter, WS_PlayerData.CharacterObject).playersNear.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso CType(objCharacter, WS_PlayerData.CharacterObject).IsHorde = _WorldServer.CHARACTERs(pGUID).IsHorde AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), objCharacter) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next

            ElseIf TypeOf objCharacter Is WS_Creatures.CreatureObject Then
                For Each pGUID As ULong In objCharacter.SeenBy.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(pGUID) AndAlso _WorldServer.CHARACTERs(pGUID).IsDead = False AndAlso _WorldServer.CHARACTERs(pGUID).GetReaction(CType(objCharacter, WS_Creatures.CreatureObject).Faction) > TReaction.NEUTRAL Then
                        If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(pGUID), objCharacter) < Distance Then result.Add(_WorldServer.CHARACTERs(pGUID))
                    End If
                Next
            End If

            Return result
        End Function

        Public Function GetPartyMembersAroundMe(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal distance As Single) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit) From {
                    objCharacter
                    }
            If Not objCharacter.IsInGroup Then Return result

            For Each GUID As ULong In objCharacter.Group.LocalMembers.ToArray
                If objCharacter.playersNear.Contains(GUID) AndAlso _WorldServer.CHARACTERs.ContainsKey(GUID) Then
                    If _WS_Combat.GetDistance(objCharacter, _WorldServer.CHARACTERs(GUID)) < distance Then result.Add(_WorldServer.CHARACTERs(GUID))
                End If
            Next

            Return result
        End Function

        Public Function GetPartyMembersAtPoint(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal Distance As Single, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit)

            If _WS_Combat.GetDistance(objCharacter, PosX, PosY, PosZ) < Distance Then result.Add(objCharacter)
            If Not objCharacter.IsInGroup Then Return result

            For Each GUID As ULong In objCharacter.Group.LocalMembers.ToArray
                If objCharacter.playersNear.Contains(GUID) AndAlso _WorldServer.CHARACTERs.ContainsKey(GUID) Then
                    If _WS_Combat.GetDistance(_WorldServer.CHARACTERs(GUID), PosX, PosY, PosZ) < Distance Then result.Add(_WorldServer.CHARACTERs(GUID))
                End If
            Next

            Return result
        End Function

        Public Function GetEnemyInFrontOfMe(ByRef objCharacter As WS_Base.BaseUnit, ByVal Distance As Single) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit)

            Dim tmp As List(Of WS_Base.BaseUnit) = GetEnemyAroundMe(objCharacter, Distance)
            For Each unit As WS_Base.BaseUnit In tmp
                If _WS_Combat.IsInFrontOf(objCharacter, unit) Then result.Add(unit)
            Next

            Return result
        End Function

        Public Function GetEnemyInBehindMe(ByRef objCharacter As WS_Base.BaseUnit, ByVal Distance As Single) As List(Of WS_Base.BaseUnit)
            Dim result As New List(Of WS_Base.BaseUnit)

            Dim tmp As List(Of WS_Base.BaseUnit) = GetEnemyAroundMe(objCharacter, Distance)
            For Each unit As WS_Base.BaseUnit In tmp
                If _WS_Combat.IsInBackOf(objCharacter, unit) Then result.Add(unit)
            Next

            Return result
        End Function

#End Region
#Region "WS.Spells.SpellAuraEffects"

        Delegate Sub ApplyAuraHandler(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
        Public Const AURAs_COUNT As Integer = 261
        Public AURAs(AURAs_COUNT) As ApplyAuraHandler

        Public Sub SPELL_AURA_NONE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

        End Sub

        Public Sub SPELL_AURA_DUMMY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[DEBUG] Aura Dummy for spell {0}.", SpellID)
            Select Case Action
                Case AuraAction.AURA_REMOVEBYDURATION
                    Select Case SpellID
                        Case 33763
                            'HACK: Lifebloom
                            'TODO: Can lifebloom crit (the end damage)?
                            Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                            SendHealSpellLog(Caster, Target, SpellID, Damage, False)
                            Target.Heal(Damage)
                    End Select
            End Select
        End Sub

        Public Sub SPELL_AURA_BIND_SIGHT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        CType(Caster, WS_PlayerData.CharacterObject).DuelArbiter = Target.GUID
                        CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, Target.GUID)
                        CType(Caster, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        CType(Caster, WS_PlayerData.CharacterObject).DuelArbiter = 0
                        CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, CType(0, Long))
                        CType(Caster, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_FAR_SIGHT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, EffectInfo.MiscValue)
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, 0)
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_SCHOOL_IMMUNITY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.SchoolImmunity = Target.SchoolImmunity Or (1 << EffectInfo.MiscValue)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.SchoolImmunity = Target.SchoolImmunity And (Not (1 << EffectInfo.MiscValue))
            End Select

        End Sub

        Public Sub SPELL_AURA_MECHANIC_IMMUNITY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.MechanicImmunity = Target.MechanicImmunity Or (1 << EffectInfo.MiscValue)
                    Target.RemoveAurasByMechanic(EffectInfo.MiscValue)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.MechanicImmunity = Target.MechanicImmunity And (Not (1 << EffectInfo.MiscValue))
            End Select

        End Sub

        Public Sub SPELL_AURA_DISPEL_IMMUNITY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.DispellImmunity = Target.DispellImmunity Or (1 << EffectInfo.MiscValue)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.DispellImmunity = Target.DispellImmunity And (Not (1 << EffectInfo.MiscValue))
            End Select

        End Sub

        Public Sub SPELL_AURA_TRACK_CREATURES(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_TRACK_CREATURES, 1 << (EffectInfo.MiscValue - 1))
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_TRACK_CREATURES, 0)
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_TRACK_RESOURCES(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_TRACK_RESOURCES, 1 << (EffectInfo.MiscValue - 1))
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_TRACK_RESOURCES, 0)
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_SCALE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub
                Case AuraAction.AURA_ADD
                    Target.Size *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Size /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
            End Select

            'DONE: Send update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, Target.Size)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EObjectFields.OBJECT_END)
                tmpUpdate.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, Target.Size)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                Target.SendToNearPlayers(packet)
                tmpUpdate.Dispose()
                packet.Dispose()
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_SKILL(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject AndAlso CType(Target, WS_PlayerData.CharacterObject).Skills.ContainsKey(EffectInfo.MiscValue) Then
                        With CType(Target, WS_PlayerData.CharacterObject)
                            .Skills(EffectInfo.MiscValue).Bonus += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                            Call .SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + .SkillsPositions(EffectInfo.MiscValue) * 3 + 2, .Skills(EffectInfo.MiscValue).Bonus)                      'skill1.Bonus
                            Call .SendCharacterUpdate(True)
                        End With
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject AndAlso CType(Target, WS_PlayerData.CharacterObject).Skills.ContainsKey(EffectInfo.MiscValue) Then
                        With CType(Target, WS_PlayerData.CharacterObject)
                            .Skills(EffectInfo.MiscValue).Bonus -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                            Call .SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + .SkillsPositions(EffectInfo.MiscValue) * 3 + 2, .Skills(EffectInfo.MiscValue).Bonus)                      'skill1.Bonus
                            Call .SendCharacterUpdate(True)
                        End With
                    End If
            End Select
        End Sub

        Public Sub SPELL_AURA_PERIODIC_DUMMY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub
                    Select Case SpellID
                        Case 430, 431, 432, 1133, 1135, 1137, 10250, 22734, 27089, 34291, 43706, 46755
                            'HACK: Drink
                            Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                            CType(Target, WS_PlayerData.CharacterObject).ManaRegenBonus += Damage
                            CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()
                    End Select
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If Not TypeOf Caster Is WS_PlayerData.CharacterObject Then Exit Sub
                    Select Case SpellID
                        Case 430, 431, 432, 1133, 1135, 1137, 10250, 22734, 27089, 34291, 43706, 46755
                            'HACK: Drink
                            Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                            CType(Target, WS_PlayerData.CharacterObject).ManaRegenBonus -= Damage
                            CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()
                    End Select
                Case AuraAction.AURA_UPDATE
                    Select Case SpellID
                        Case 43265, 49936, 49937, 49938
                            'HACK: Death and Decay
                            Dim Damage As Integer
                            If TypeOf Caster Is WS_DynamicObjects.DynamicObjectObject Then
                                Damage = EffectInfo.GetValue(CType(Caster, WS_DynamicObjects.DynamicObjectObject).Caster.Level) * StackCount
                                Target.DealDamage(Damage, CType(Caster, WS_DynamicObjects.DynamicObjectObject).Caster)
                            Else
                                Damage = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                                Target.DealDamage(Damage, CType(Caster, WS_Base.BaseUnit))
                            End If
                    End Select
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_DAMAGE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    If TypeOf Caster Is WS_DynamicObjects.DynamicObjectObject Then
                        Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_DynamicObjects.DynamicObjectObject).Caster.Level) * StackCount
                        Target.DealSpellDamage(CType(Caster, WS_DynamicObjects.DynamicObjectObject).Caster, EffectInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_DOT)
                    Else
                        Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                        Target.DealSpellDamage(Caster, EffectInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_DOT)
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_HEAL(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount

                    'NOTE: This function heals as well
                    Target.DealSpellDamage(Caster, EffectInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_HEALDOT)
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_ENERGIZE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    Dim Power As ManaTypes = EffectInfo.MiscValue
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                    SendPeriodicAuraLog(Caster, Target, SpellID, Power, Damage, EffectInfo.ApplyAuraIndex)
                    Target.Energize(Damage, Power, Caster)
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_LEECH(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                    SendPeriodicAuraLog(Caster, Target, SpellID, SPELLs(SpellID).School, Damage, EffectInfo.ApplyAuraIndex)
                    SendPeriodicAuraLog(Target, Caster, SpellID, SPELLs(SpellID).School, Damage, EffectInfo.ApplyAuraIndex)
                    Target.DealDamage(Damage, Caster)
                    CType(Caster, WS_Base.BaseUnit).Heal(Damage, Target)
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_MANA_LEECH(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    Dim Power As ManaTypes = EffectInfo.MiscValue
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                    SendPeriodicAuraLog(Target, Caster, SpellID, Power, Damage, EffectInfo.ApplyAuraIndex)
                    Target.Energize(-Damage, Power, Caster)
                    CType(Caster, WS_Base.BaseUnit).Energize(Damage, Power, Target)
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_TRIGGER_SPELL(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    'TODO: Arcane missiles are casted on yourself
                    Dim Targets As New SpellTargets
                    Targets.SetTarget_UNIT(Target)
                    Dim castParams As New CastSpellParameters(Targets, Caster, EffectInfo.TriggerSpell)
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
                    If TypeOf Caster Is WS_Base.BaseUnit Then
                        SendPeriodicAuraLog(Caster, Target, SpellID, SPELLs(SpellID).School, 0, EffectInfo.ApplyAuraIndex)
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_DAMAGE_PERCENT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    Dim Damage As Integer = (Target.Life.Maximum * EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) * StackCount
                    Target.DealSpellDamage(Caster, EffectInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_DOT)
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_HEAL_PERCENT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Dim Damage As Integer = (Target.Life.Maximum * EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) * StackCount

                    'NOTE: This function heals as well
                    Target.DealSpellDamage(Caster, EffectInfo, SpellID, Damage, SPELLs(SpellID).School, SpellType.SPELL_TYPE_HEALDOT)

                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
            End Select

        End Sub

        Public Sub SPELL_AURA_PERIODIC_ENERGIZE_PERCENT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Dim Power As ManaTypes = EffectInfo.MiscValue
                    Dim Damage As Integer = (Target.Mana.Maximum * EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) * StackCount
                    SendPeriodicAuraLog(Caster, Target, SpellID, Power, Damage, EffectInfo.ApplyAuraIndex)
                    Target.Energize(Damage, Power, Caster)

                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_REGEN(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub
                Case AuraAction.AURA_ADD
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                    CType(Target, WS_PlayerData.CharacterObject).LifeRegenBonus += Damage

                    'TODO: Increase threat (gain * 0.5)

                    If (SPELLs(SpellID).auraInterruptFlags And SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_NOT_SEATED) Then
                        'Eat emote
                        Target.DoEmote(Emotes.ONESHOT_EAT)
                    ElseIf SpellID = 20577 Then
                        'HACK: Cannibalize emote
                        Target.DoEmote(Emotes.STATE_CANNIBALIZE)
                    End If
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                    CType(Target, WS_PlayerData.CharacterObject).LifeRegenBonus -= Damage
            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_POWER_REGEN(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub
                Case AuraAction.AURA_ADD
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                    If EffectInfo.MiscValue = ManaTypes.TYPE_MANA Then
                        CType(Target, WS_PlayerData.CharacterObject).ManaRegenBonus += Damage
                        CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()
                    ElseIf EffectInfo.MiscValue = ManaTypes.TYPE_RAGE Then
                        CType(Target, WS_PlayerData.CharacterObject).RageRegenBonus += ((Damage / 17) * 10)
                    End If

                    If (SPELLs(SpellID).auraInterruptFlags And SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_NOT_SEATED) Then
                        'Eat emote
                        Target.DoEmote(Emotes.ONESHOT_EAT)
                    ElseIf SpellID = 20577 Then
                        'HACK: Cannibalize emote
                        Target.DoEmote(Emotes.STATE_CANNIBALIZE)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount
                    If EffectInfo.MiscValue = ManaTypes.TYPE_MANA Then
                        CType(Target, WS_PlayerData.CharacterObject).ManaRegenBonus -= Damage
                        CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()
                    ElseIf EffectInfo.MiscValue = ManaTypes.TYPE_RAGE Then
                        CType(Target, WS_PlayerData.CharacterObject).RageRegenBonus -= (Damage / 17) * 10
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_POWER_REGEN_PERCENT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub
                Case AuraAction.AURA_ADD
                    If EffectInfo.MiscValue = ManaTypes.TYPE_MANA Then
                        Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount / 100
                        CType(Target, WS_PlayerData.CharacterObject).ManaRegenerationModifier += Damage
                        CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim Damage As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount / 100
                    CType(Target, WS_PlayerData.CharacterObject).ManaRegenerationModifier -= Damage
                    CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()
            End Select

        End Sub

        Public Sub SPELL_AURA_TRANSFORM(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    If Not _WorldServer.CREATURESDatabase.ContainsKey(EffectInfo.MiscValue) Then
                        Dim creature As New CreatureInfo(EffectInfo.MiscValue)
                        _WorldServer.CREATURESDatabase.Add(EffectInfo.MiscValue, creature)
                    End If
                    Target.Model = _WorldServer.CREATURESDatabase(EffectInfo.MiscValue).GetFirstModel

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Target.Model = _Functions.GetRaceModel(CType(Target, WS_PlayerData.CharacterObject).Race, CType(Target, WS_PlayerData.CharacterObject).Gender)
                    Else
                        Target.Model = CType(Target, WS_Creatures.CreatureObject).CreatureInfo.GetRandomModel()
                    End If

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select

            'DONE: Model update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, Target.Model)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, Target.Model)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                Target.SendToNearPlayers(packet)
                tmpUpdate.Dispose()
                packet.Dispose()
            End If

        End Sub

        Public Sub SPELL_AURA_GHOST(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.Invisibility = InvisibilityLevel.DEAD
                    Target.CanSeeInvisibility = InvisibilityLevel.DEAD
                    _WS_CharMovement.UpdateCell(Target)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Invisibility = InvisibilityLevel.VISIBLE
                    Target.CanSeeInvisibility = InvisibilityLevel.INIVISIBILITY
                    _WS_CharMovement.UpdateCell(Target)

            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_INVISIBILITY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    CType(Target, WS_PlayerData.CharacterObject).cPlayerFieldBytes2 = CType(Target, WS_PlayerData.CharacterObject).cPlayerFieldBytes2 Or &H4000
                    Target.Invisibility = InvisibilityLevel.INIVISIBILITY
                    Target.Invisibility_Value += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    CType(Target, WS_PlayerData.CharacterObject).cPlayerFieldBytes2 = CType(Target, WS_PlayerData.CharacterObject).cPlayerFieldBytes2 And (Not &H4000)
                    Target.Invisibility = InvisibilityLevel.VISIBLE
                    Target.Invisibility_Value -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            End Select

            'DONE: Send update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BYTES2, CType(Target, WS_PlayerData.CharacterObject).cPlayerFieldBytes2)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                _WS_CharMovement.UpdateCell(CType(Target, WS_PlayerData.CharacterObject))
            Else
                'TODO: Still not done for creatures
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_STEALTH(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.Invisibility = InvisibilityLevel.STEALTH
                    Target.Invisibility_Value += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Invisibility = InvisibilityLevel.VISIBLE
                    Target.Invisibility_Value -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            End Select

            'DONE: Update the cell
            _WS_CharMovement.UpdateCell(CType(Target, WS_PlayerData.CharacterObject))
        End Sub

        Public Sub SPELL_AURA_MOD_STEALTH_LEVEL(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.Invisibility_Bonus += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Invisibility_Bonus -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_INVISIBILITY_DETECTION(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.CanSeeInvisibility_Invisibility += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.CanSeeInvisibility_Invisibility -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                _WS_CharMovement.UpdateCell(CType(Target, WS_PlayerData.CharacterObject))
            End If
        End Sub

        Public Sub SPELL_AURA_MOD_DETECT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.CanSeeInvisibility_Stealth += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.CanSeeInvisibility_Stealth -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                _WS_CharMovement.UpdateCell(CType(Target, WS_PlayerData.CharacterObject))
            End If
        End Sub

        Public Sub SPELL_AURA_DETECT_STEALTH(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.CanSeeStealth = True

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.CanSeeStealth = False

            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                _WS_CharMovement.UpdateCell(CType(Target, WS_PlayerData.CharacterObject))
            End If
        End Sub

        Public Sub SPELL_AURA_MOD_DISARM(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub
            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub
                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).Disarmed = True
                        CType(Target, WS_PlayerData.CharacterObject).cUnitFlags = UnitFlags.UNIT_FLAG_DISARMED
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, CType(Target, WS_PlayerData.CharacterObject).cUnitFlags)
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).Disarmed = False
                        CType(Target, WS_PlayerData.CharacterObject).cUnitFlags = CType(Target, WS_PlayerData.CharacterObject).cUnitFlags And (Not UnitFlags.UNIT_FLAG_DISARMED)
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, CType(Target, WS_PlayerData.CharacterObject).cUnitFlags)
                        CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
                    End If
            End Select
        End Sub

        Public Sub SPELL_AURA_SCHOOL_ABSORB(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Caster Is WS_Base.BaseUnit Then Exit Sub
            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub
                Case AuraAction.AURA_ADD
                    If Target.AbsorbSpellLeft.ContainsKey(SpellID) Then Exit Sub

                    Target.AbsorbSpellLeft.Add(SpellID, CUInt(EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)) + (CUInt(EffectInfo.MiscValue) << 23UI))
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If Not Target.AbsorbSpellLeft.ContainsKey(SpellID) Then Exit Sub

                    Target.AbsorbSpellLeft.Remove(SpellID)
            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_SHAPESHIFT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Target.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOD_SHAPESHIFT, SpellID)  'Remove other shapeshift forms
                    Target.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOUNTED)                  'Remove mounted spells

                    'Druid
                    If TypeOf Target Is WS_PlayerData.CharacterObject AndAlso CType(Target, WS_PlayerData.CharacterObject).Classe = Classes.CLASS_DRUID Then
                        If EffectInfo.MiscValue = ShapeshiftForm.FORM_AQUA OrElse EffectInfo.MiscValue = ShapeshiftForm.FORM_CAT OrElse EffectInfo.MiscValue = ShapeshiftForm.FORM_BEAR OrElse EffectInfo.MiscValue = ShapeshiftForm.FORM_DIREBEAR OrElse EffectInfo.MiscValue = ShapeshiftForm.FORM_TRAVEL OrElse EffectInfo.MiscValue = ShapeshiftForm.FORM_FLIGHT OrElse EffectInfo.MiscValue = ShapeshiftForm.FORM_SWIFT OrElse EffectInfo.MiscValue = ShapeshiftForm.FORM_MOONKIN Then
                            Target.RemoveAurasOfType(26) 'Remove Root
                            Target.RemoveAurasOfType(33) 'Remove Slow
                        End If
                    End If

                    Target.ShapeshiftForm = EffectInfo.MiscValue
                    Target.ManaType = _CommonGlobalFunctions.GetShapeshiftManaType(EffectInfo.MiscValue, Target.ManaType)
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Target.Model = _CommonGlobalFunctions.GetShapeshiftModel(EffectInfo.MiscValue, CType(Target, WS_PlayerData.CharacterObject).Race, Target.Model)
                    Else
                        Target.Model = _CommonGlobalFunctions.GetShapeshiftModel(EffectInfo.MiscValue, 0, Target.Model)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.ShapeshiftForm = ShapeshiftForm.FORM_NORMAL

                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Target.ManaType = _WS_Player_Initializator.GetClassManaType(CType(Target, WS_PlayerData.CharacterObject).Classe)
                        Target.Model = _Functions.GetRaceModel(CType(Target, WS_PlayerData.CharacterObject).Race, CType(Target, WS_PlayerData.CharacterObject).Gender)
                    Else
                        Target.ManaType = CType(Target, WS_Creatures.CreatureObject).CreatureInfo.ManaType
                        Target.Model = CType(Target, WS_Creatures.CreatureObject).CreatureInfo.GetRandomModel
                    End If

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select

            'DONE: Send update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                With CType(Target, WS_PlayerData.CharacterObject)
                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_2, .cBytes2)
                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, .Model)
                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_0, .cBytes0)
                    If .ManaType = ManaTypes.TYPE_MANA Then
                        .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, .Mana.Current)
                        .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, .Mana.Maximum)
                    ElseIf .ManaType = ManaTypes.TYPE_RAGE Then
                        .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER2, .Rage.Current)
                        .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER2, .Rage.Maximum)
                    ElseIf .ManaType = ManaTypes.TYPE_ENERGY Then
                        .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER4, .Energy.Current)
                        .SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER4, .Energy.Maximum)
                    End If
                    _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.BASE_ATTACK)
                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_MINDAMAGE, .Damage.Minimum)
                    .SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXDAMAGE, .Damage.Maximum)
                    .SendCharacterUpdate(True)
                    .GroupUpdateFlag = .GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_POWER_TYPE
                    .GroupUpdateFlag = .GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_POWER
                    .GroupUpdateFlag = .GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER
                    _WS_PlayerHelper.InitializeTalentSpells(CType(Target, WS_PlayerData.CharacterObject))
                End With
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_2, Target.cBytes2)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, Target.Model)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                Target.SendToNearPlayers(packet)
                tmpUpdate.Dispose()
                packet.Dispose()
            End If

            'TODO: The running emote is fucked up
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                If Action = AuraAction.AURA_ADD Then
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_TRAVEL Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(5419)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_CAT Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(3025)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_BEAR Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(1178)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_DIREBEAR Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(9635)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_AQUA Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(5421)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_MOONKIN Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(24905)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_FLIGHT Then
                        CType(Target, WS_PlayerData.CharacterObject).ApplySpell(33948)
                        CType(Target, WS_PlayerData.CharacterObject).ApplySpell(34764)
                    End If
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_SWIFT Then
                        CType(Target, WS_PlayerData.CharacterObject).ApplySpell(40121)
                        CType(Target, WS_PlayerData.CharacterObject).ApplySpell(40122)
                    End If
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_BATTLESTANCE Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(21156)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_BERSERKERSTANCE Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(7381)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_DEFENSIVESTANCE Then CType(Target, WS_PlayerData.CharacterObject).ApplySpell(7376)
                Else
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_TRAVEL Then Target.RemoveAuraBySpell(5419)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_CAT Then Target.RemoveAuraBySpell(3025)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_BEAR Then Target.RemoveAuraBySpell(1178)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_DIREBEAR Then Target.RemoveAuraBySpell(9635)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_AQUA Then Target.RemoveAuraBySpell(5421)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_MOONKIN Then Target.RemoveAuraBySpell(24905)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_FLIGHT Then
                        Target.RemoveAuraBySpell(33948)
                        Target.RemoveAuraBySpell(34764)
                    End If
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_SWIFT Then
                        Target.RemoveAuraBySpell(40121)
                        Target.RemoveAuraBySpell(40122)
                    End If
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_BATTLESTANCE Then Target.RemoveAuraBySpell(21156)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_BERSERKERSTANCE Then Target.RemoveAuraBySpell(7381)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_DEFENSIVESTANCE Then Target.RemoveAuraBySpell(7376)
                    If EffectInfo.MiscValue = ShapeshiftForm.FORM_GHOSTWOLF Then Target.RemoveAuraBySpell(7376)
                End If
            End If
        End Sub

        Public Sub SPELL_AURA_PROC_TRIGGER_SPELL(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            Select Case Action
                Case AuraAction.AURA_ADD
                    Exit Sub
                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Exit Sub
                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_SPEED(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            Select Case Action
                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                        newSpeed *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).SetToRealPosition()
                        CType(Target, WS_Creatures.CreatureObject).SpeedMod *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                        If Caster Is Nothing Then
                            ' do nothing?
                        Else
                            newSpeed /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        End If
                        CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).SetToRealPosition()
                        CType(Target, WS_Creatures.CreatureObject).SpeedMod /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    End If

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_DECREASE_SPEED(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            'NOTE: Some values of EffectInfo.GetValue are in old format, new format uses (-) values

            Select Case Action
                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                        If EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) < 0 Then
                            newSpeed /= Math.Abs(EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        Else
                            newSpeed /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        End If

                        CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)

                        'DONE: Remove some auras when slowed
                        CType(Target, WS_PlayerData.CharacterObject).RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_SLOWED)
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).SetToRealPosition()
                        If EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) < 0 Then
                            CType(Target, WS_Creatures.CreatureObject).SpeedMod /= Math.Abs(EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        Else
                            CType(Target, WS_Creatures.CreatureObject).SpeedMod /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        End If

                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                        If EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) < 0 Then
                            newSpeed *= Math.Abs(EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        Else
                            newSpeed *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        End If
                        CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).SetToRealPosition()
                        If EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) < 0 Then
                            CType(Target, WS_Creatures.CreatureObject).SpeedMod *= Math.Abs(EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        Else
                            CType(Target, WS_Creatures.CreatureObject).SpeedMod *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        End If
                    End If

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_SPEED_ALWAYS(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_ADD
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                    newSpeed *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                    newSpeed /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_MOUNTED_SPEED(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_ADD
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                    newSpeed *= ((EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1)
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                    newSpeed /= ((EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1)
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_MOUNTED_SPEED_ALWAYS(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_ADD
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                    newSpeed *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).RunSpeed
                    newSpeed /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.RUN, newSpeed)

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_SWIM_SPEED(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_ADD
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).SwimSpeed
                    newSpeed *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    CType(Target, WS_PlayerData.CharacterObject).SwimSpeed = newSpeed
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.SWIM, newSpeed)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim newSpeed As Single = CType(Target, WS_PlayerData.CharacterObject).SwimSpeed
                    If Caster Is Nothing Then
                        'do nothing?
                    Else
                        newSpeed /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    End If
                    CType(Target, WS_PlayerData.CharacterObject).SwimSpeed = newSpeed
                    CType(Target, WS_PlayerData.CharacterObject).ChangeSpeedForced(ChangeSpeedType.SWIM, newSpeed)

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select
        End Sub

        Public Sub SPELL_AURA_MOUNTED(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Target.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOD_SHAPESHIFT)       'Remove shapeshift forms
                    Target.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOUNTED, SpellID)     'Remove other mounted spells
                    Target.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_MOUNTING)

                    If Not _WorldServer.CREATURESDatabase.ContainsKey(EffectInfo.MiscValue) Then
                        Dim creature As New CreatureInfo(EffectInfo.MiscValue)
                        _WorldServer.CREATURESDatabase.Add(EffectInfo.MiscValue, creature)
                    End If
                    If _WorldServer.CREATURESDatabase.ContainsKey(EffectInfo.MiscValue) Then
                        Target.Mount = _WorldServer.CREATURESDatabase(EffectInfo.MiscValue).GetFirstModel
                    Else
                        Target.Mount = 0
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Mount = 0
                    Target.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_NOT_MOUNTED)

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select

            'DONE: Model update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, Target.Mount)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, Target.Mount)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                Target.SendToNearPlayers(packet)
                tmpUpdate.Dispose()
                packet.Dispose()
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_HASTE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            With CType(Target, WS_PlayerData.CharacterObject)
                Select Case Action
                    Case AuraAction.AURA_ADD
                        .AttackTimeMods(0) /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        .AttackTimeMods(1) /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                        .AttackTimeMods(0) *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                        .AttackTimeMods(1) *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    Case AuraAction.AURA_UPDATE
                        Exit Sub
                End Select

                .SetUpdateFlag(EUnitFields.UNIT_FIELD_BASEATTACKTIME, .AttackTime(0))
                .SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGEDATTACKTIME, .AttackTime(1))
                .SendCharacterUpdate(False)
            End With
        End Sub

        Public Sub SPELL_AURA_MOD_RANGED_HASTE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            With CType(Target, WS_PlayerData.CharacterObject)
                Select Case Action
                    Case AuraAction.AURA_ADD
                        .AttackTimeMods(2) /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                        .AttackTimeMods(2) *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    Case AuraAction.AURA_UPDATE
                        Exit Sub
                End Select

                .SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGEDATTACKTIME, .AttackTime(2))
                .SendCharacterUpdate(False)
            End With
        End Sub

        Public Sub SPELL_AURA_MOD_RANGED_AMMO_HASTE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            With CType(Target, WS_PlayerData.CharacterObject)
                Select Case Action
                    Case AuraAction.AURA_ADD
                        .AmmoMod *= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                        .AmmoMod /= (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100) + 1
                    Case AuraAction.AURA_UPDATE
                        Exit Sub
                End Select

                _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.RANGED_ATTACK)
                .SendCharacterUpdate(False)
            End With
        End Sub

        Public Sub SPELL_AURA_MOD_ROOT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    'Target.cUnitFlags = Target.cUnitFlags Or UnitFlags.UNIT_FLAG_ROOTED
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetMoveRoot()
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_TARGET, CType(0, Long))
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        If CType(Target, WS_Creatures.CreatureObject).aiScript IsNot Nothing Then CType(Target, WS_Creatures.CreatureObject).aiScript.OnGenerateHate(CType(Caster, WS_Base.BaseUnit), 1)

                        CType(Target, WS_Creatures.CreatureObject).StopMoving()
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    'Target.cUnitFlags = Target.cUnitFlags And (Not UnitFlags.UNIT_FLAG_ROOTED)
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetMoveUnroot()
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).StopMoving()
                    End If

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select

            'DONE: Send update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Target.cUnitFlags)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Target.cUnitFlags)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                Target.SendToNearPlayers(packet)
                tmpUpdate.Dispose()
                packet.Dispose()
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_STUN(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Target.cUnitFlags = Target.cUnitFlags Or UnitFlags.UNIT_FLAG_STUNTED
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetMoveRoot()
                        CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_TARGET, 0UL)
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).StopMoving()

                        If CType(Target, WS_Creatures.CreatureObject).aiScript IsNot Nothing Then CType(Target, WS_Creatures.CreatureObject).aiScript.OnGenerateHate(CType(Caster, WS_Base.BaseUnit), 1)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.cUnitFlags = Target.cUnitFlags And (Not UnitFlags.UNIT_FLAG_STUNTED)
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        CType(Target, WS_PlayerData.CharacterObject).SetMoveUnroot()
                    End If

                Case AuraAction.AURA_UPDATE
                    Exit Sub
            End Select

            'DONE: Send update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Target.cUnitFlags)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Target.cUnitFlags)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                Target.SendToNearPlayers(packet)
                tmpUpdate.Dispose()
                packet.Dispose()
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_FEAR(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Dim response As New Packets.PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
            response.AddPackGUID(Target.GUID)

            Select Case Action
                Case AuraAction.AURA_ADD
                    Target.cUnitFlags = Target.cUnitFlags Or UnitFlags.UNIT_FLAG_FLEEING
                    response.AddInt8(0)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.cUnitFlags = Target.cUnitFlags And (Not UnitFlags.UNIT_FLAG_FLEEING)
                    response.AddInt8(1)

                Case AuraAction.AURA_UPDATE
                    'TODO: Random movement
                    Exit Sub
            End Select

            'DONE: Send update
            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Target.cUnitFlags)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(True)

                CType(Target, WS_PlayerData.CharacterObject).client.Send(response)
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Target.cUnitFlags)
                tmpUpdate.AddToPacket((packet), ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                Target.SendToNearPlayers(packet)
                tmpUpdate.Dispose()
                packet.Dispose()
            End If

            response.Dispose()

        End Sub

        Public Sub SPELL_AURA_SAFE_FALL(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_FEATHER_FALL)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_NORMAL_FALL)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

            End Select

        End Sub

        Public Sub SPELL_AURA_FEATHER_FALL(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_FEATHER_FALL)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_NORMAL_FALL)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

            End Select

        End Sub

        Public Sub SPELL_AURA_WATER_WALK(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_WATER_WALK)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_LAND_WALK)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

            End Select

        End Sub

        Public Sub SPELL_AURA_HOVER(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_SET_HOVER)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_MOVE_UNSET_HOVER)
                    packet.AddPackGUID(Target.GUID)
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()

            End Select

        End Sub

        Public Sub SPELL_AURA_WATER_BREATHING(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    CType(Target, WS_PlayerData.CharacterObject).underWaterBreathing = True

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    CType(Target, WS_PlayerData.CharacterObject).underWaterBreathing = False

            End Select

        End Sub

        Public Sub SPELL_AURA_ADD_FLAT_MODIFIER(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub
            If EffectInfo.MiscValue > 32 Then Exit Sub

            Dim op As SpellModOp = EffectInfo.MiscValue
            Dim value As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
            Dim mask As Integer = EffectInfo.ItemType

            If Action = AuraAction.AURA_ADD Then
                'TODO: Add spell modifier!

                Dim send_val As UShort
                Dim send_mark As UShort
                Dim tmpval As Short = EffectInfo.valueBase
                Dim shiftdata As UInteger = &H1

                If tmpval <> 0 Then
                    If tmpval > 0 Then
                        send_val = tmpval + 1
                        send_mark = &H0
                    Else
                        send_val = ((&HFFFFUS + (tmpval + 2)) And &HFFFFUS)
                        send_mark = &HFFFFUS
                    End If
                End If

                For eff As Integer = 0 To 31
                    If (mask And shiftdata) Then
                        Dim packet As New Packets.PacketClass(OPCODES.SMSG_SET_FLAT_SPELL_MODIFIER)
                        packet.AddInt8(eff)
                        packet.AddInt8(op)
                        packet.AddUInt16(send_val)
                        packet.AddUInt16(send_mark)
                        CType(Caster, WS_PlayerData.CharacterObject).client.Send(packet)
                        packet.Dispose()
                    End If
                    shiftdata = shiftdata << 1
                Next
            ElseIf Action = AuraAction.AURA_REMOVE OrElse Action = AuraAction.AURA_REMOVEBYDURATION Then
                'TODO: Remove spell modifier!
            End If
        End Sub

        Public Sub SPELL_AURA_ADD_PCT_MODIFIER(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub
            If EffectInfo.MiscValue > 32 Then Exit Sub

            Dim op As SpellModOp = EffectInfo.MiscValue
            Dim value As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
            Dim mask As Integer = EffectInfo.ItemType

            If Action = AuraAction.AURA_ADD Then
                'TODO: Add spell modifier!

                Dim send_val As UShort
                Dim send_mark As UShort
                Dim tmpval As Short = EffectInfo.valueBase
                Dim shiftdata As UInteger = &H1

                If tmpval <> 0 Then
                    If tmpval > 0 Then
                        send_val = tmpval + 1
                        send_mark = &H0
                    Else
                        send_val = ((&HFFFFUS + (tmpval + 2)) And &HFFFFUS)
                        send_mark = &HFFFFUS
                    End If
                End If

                For eff As Integer = 0 To 31
                    If (mask And shiftdata) Then
                        Dim packet As New Packets.PacketClass(OPCODES.SMSG_SET_PCT_SPELL_MODIFIER)
                        packet.AddInt8(eff)
                        packet.AddInt8(op)
                        packet.AddUInt16(send_val)
                        packet.AddUInt16(send_mark)
                        CType(Caster, WS_PlayerData.CharacterObject).client.Send(packet)
                        packet.Dispose()
                    End If
                    shiftdata = shiftdata << 1
                Next
            ElseIf Action = AuraAction.AURA_REMOVE OrElse Action = AuraAction.AURA_REMOVEBYDURATION Then
                'TODO: Remove spell modifier!
            End If
        End Sub

        'TODO: Update values based on stats
        Public Sub SPELL_AURA_MOD_STAT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Action = AuraAction.AURA_UPDATE Then Exit Sub
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Dim value As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
            Dim value_sign As Integer = value
            If Action = AuraAction.AURA_REMOVE Then value = -value

            Select Case EffectInfo.MiscValue
                Case -1
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base /= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base *= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base /= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base *= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base /= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base *= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base /= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base *= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base /= CType(Target, WS_PlayerData.CharacterObject).Intellect.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base *= CType(Target, WS_PlayerData.CharacterObject).Intellect.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Strength.PositiveBonus += value
                        CType(Target, WS_PlayerData.CharacterObject).Agility.PositiveBonus += value
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.PositiveBonus += value
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.PositiveBonus += value
                        CType(Target, WS_PlayerData.CharacterObject).Intellect.PositiveBonus += value
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Strength.NegativeBonus -= value
                        CType(Target, WS_PlayerData.CharacterObject).Agility.NegativeBonus -= value
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.NegativeBonus -= value
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.NegativeBonus -= value
                        CType(Target, WS_PlayerData.CharacterObject).Intellect.NegativeBonus -= value
                    End If
                Case 0
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base /= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base *= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Strength.PositiveBonus += value
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Strength.NegativeBonus -= value
                    End If
                Case 1
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base /= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base *= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Agility.PositiveBonus += value
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Agility.NegativeBonus -= value
                    End If
                Case 2
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base /= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base *= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.PositiveBonus += value
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.NegativeBonus -= value
                    End If
                Case 3
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base /= CType(Target, WS_PlayerData.CharacterObject).Intellect.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base *= CType(Target, WS_PlayerData.CharacterObject).Intellect.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Intellect.PositiveBonus += value
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Intellect.NegativeBonus -= value
                    End If
                Case 4
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base /= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base += value
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base *= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.PositiveBonus += value
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.NegativeBonus -= value
                    End If
            End Select

            CType(Target, WS_PlayerData.CharacterObject).Life.Bonus = ((CType(Target, WS_PlayerData.CharacterObject).Stamina.Base - 18) * 10)
            CType(Target, WS_PlayerData.CharacterObject).Mana.Bonus = ((CType(Target, WS_PlayerData.CharacterObject).Intellect.Base - 18) * 15)
            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER
            CType(Target, WS_PlayerData.CharacterObject).Resistances(DamageTypes.DMG_PHYSICAL).Base += value * 2
            CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()

            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.BASE_ATTACK)
            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.OFF_ATTACK)
            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.RANGED_ATTACK)

            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_STRENGTH, CType(Target, WS_PlayerData.CharacterObject).Strength.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AGILITY, CType(Target, WS_PlayerData.CharacterObject).Agility.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_STAMINA, CType(Target, WS_PlayerData.CharacterObject).Stamina.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_SPIRIT, CType(Target, WS_PlayerData.CharacterObject).Spirit.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_INTELLECT, CType(Target, WS_PlayerData.CharacterObject).Intellect.Base)
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT0, CType(CType(Target, CharacterObject).Strength.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT1, CType(CType(Target, CharacterObject).Agility.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT2, CType(CType(Target, CharacterObject).Stamina.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT3, CType(CType(Target, CharacterObject).Intellect.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT4, CType(CType(Target, CharacterObject).Spirit.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT0, CType(CType(Target, CharacterObject).Strength.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT1, CType(CType(Target, CharacterObject).Agility.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT2, CType(CType(Target, CharacterObject).Stamina.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT3, CType(CType(Target, CharacterObject).Intellect.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT4, CType(CType(Target, CharacterObject).Spirit.NegativeBonus, Integer))
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CType(Target, WS_PlayerData.CharacterObject).Life.Current)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, CType(Target, WS_PlayerData.CharacterObject).Life.Maximum)
            If _WS_Player_Initializator.GetClassManaType(CType(Target, WS_PlayerData.CharacterObject).Classe) = ManaTypes.TYPE_MANA Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, CType(Target, WS_PlayerData.CharacterObject).Mana.Current)
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1, CType(Target, WS_PlayerData.CharacterObject).Mana.Maximum)
            End If
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + CByte(DamageTypes.DMG_PHYSICAL), CType(Target, WS_PlayerData.CharacterObject).Resistances(DamageTypes.DMG_PHYSICAL).Base)
            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)

            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP
            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER
        End Sub

        Public Sub SPELL_AURA_MOD_STAT_PERCENT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Action = AuraAction.AURA_UPDATE Then Exit Sub
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            'TODO: This is only supposed to add % of the base stat, not the entire one.

            Dim value As Single = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100
            Dim value_sign As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
            If Action = AuraAction.AURA_REMOVE Then value = -value

            Dim OldStr As Short = CType(Target, WS_PlayerData.CharacterObject).Strength.Base
            Dim OldAgi As Short = CType(Target, WS_PlayerData.CharacterObject).Agility.Base
            Dim OldSta As Short = CType(Target, WS_PlayerData.CharacterObject).Stamina.Base
            Dim OldSpi As Short = CType(Target, WS_PlayerData.CharacterObject).Spirit.Base
            Dim OldInt As Short = CType(Target, WS_PlayerData.CharacterObject).Intellect.Base

            Select Case EffectInfo.MiscValue
                Case -1
                    CType(Target, WS_PlayerData.CharacterObject).Strength.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Strength.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Strength.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Strength.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Strength.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Agility.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Agility.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Agility.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Stamina.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Stamina.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Spirit.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Spirit.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Intellect.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Intellect.BaseModifier
                Case 0
                    CType(Target, WS_PlayerData.CharacterObject).Strength.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Strength.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Strength.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Strength.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Strength.BaseModifier
                Case 1
                    CType(Target, WS_PlayerData.CharacterObject).Agility.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Agility.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Agility.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Agility.BaseModifier
                Case 2
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Stamina.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Stamina.BaseModifier
                Case 3
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Intellect.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Intellect.BaseModifier
                Case 4
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.RealBase /= CType(Target, WS_PlayerData.CharacterObject).Spirit.BaseModifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.BaseModifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.RealBase *= CType(Target, WS_PlayerData.CharacterObject).Spirit.BaseModifier
            End Select

            CType(Target, WS_PlayerData.CharacterObject).Life.Bonus += (CType(Target, WS_PlayerData.CharacterObject).Stamina.Base - OldSta) * 10
            CType(Target, WS_PlayerData.CharacterObject).Mana.Bonus += (CType(Target, WS_PlayerData.CharacterObject).Intellect.Base - OldInt) * 15
            CType(Target, WS_PlayerData.CharacterObject).Resistances(DamageTypes.DMG_PHYSICAL).Base += (CType(Target, WS_PlayerData.CharacterObject).Agility.Base - OldAgi) * 2
            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER
            CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()

            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.BASE_ATTACK)
            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.OFF_ATTACK)
            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.RANGED_ATTACK)

            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_STRENGTH, CType(Target, WS_PlayerData.CharacterObject).Strength.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AGILITY, CType(Target, WS_PlayerData.CharacterObject).Agility.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_STAMINA, CType(Target, WS_PlayerData.CharacterObject).Stamina.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_SPIRIT, CType(Target, WS_PlayerData.CharacterObject).Spirit.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_INTELLECT, CType(Target, WS_PlayerData.CharacterObject).Intellect.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CType(Target, WS_PlayerData.CharacterObject).Life.Current)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, CType(Target, WS_PlayerData.CharacterObject).Life.Maximum)
            If _WS_Player_Initializator.GetClassManaType(CType(Target, WS_PlayerData.CharacterObject).Classe) = ManaTypes.TYPE_MANA Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, CType(Target, WS_PlayerData.CharacterObject).Mana.Current)
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1, CType(Target, WS_PlayerData.CharacterObject).Mana.Maximum)
            End If
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + CByte(DamageTypes.DMG_PHYSICAL), CType(Target, WS_PlayerData.CharacterObject).Resistances(DamageTypes.DMG_PHYSICAL).Base)
            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)

            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP
            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER
        End Sub

        Public Sub SPELL_AURA_MOD_TOTAL_STAT_PERCENTAGE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Action = AuraAction.AURA_UPDATE Then Exit Sub
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Dim value As Single = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100
            Dim value_sign As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
            If Action = AuraAction.AURA_REMOVE Then value = -value

            Dim OldStr As Short = CType(Target, WS_PlayerData.CharacterObject).Strength.Base
            Dim OldAgi As Short = CType(Target, WS_PlayerData.CharacterObject).Agility.Base
            Dim OldSta As Short = CType(Target, WS_PlayerData.CharacterObject).Stamina.Base
            Dim OldSpi As Short = CType(Target, WS_PlayerData.CharacterObject).Spirit.Base
            Dim OldInt As Short = CType(Target, WS_PlayerData.CharacterObject).Intellect.Base

            Select Case EffectInfo.MiscValue
                Case -1
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base /= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base *= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base /= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base *= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base /= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base *= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base /= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base *= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base /= CType(Target, WS_PlayerData.CharacterObject).Intellect.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Intellect.Base *= CType(Target, WS_PlayerData.CharacterObject).Intellect.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Strength.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Strength.Base - OldStr)
                        CType(Target, WS_PlayerData.CharacterObject).Agility.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Agility.Base - OldAgi)
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Stamina.Base - OldSta)
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Spirit.Base - OldSpi)
                        CType(Target, WS_PlayerData.CharacterObject).Intellect.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Intellect.Base - OldInt)
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Strength.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Strength.Base - OldStr)
                        CType(Target, WS_PlayerData.CharacterObject).Agility.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Agility.Base - OldAgi)
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Stamina.Base - OldSta)
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Spirit.Base - OldSpi)
                        CType(Target, WS_PlayerData.CharacterObject).Intellect.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Intellect.Base - OldInt)
                    End If
                Case 0
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base /= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Strength.Base *= CType(Target, WS_PlayerData.CharacterObject).Strength.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Strength.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Strength.Base - OldStr)
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Strength.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Strength.Base - OldStr)
                    End If
                Case 1
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base /= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Agility.Base *= CType(Target, WS_PlayerData.CharacterObject).Agility.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Agility.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Agility.Base - OldAgi)
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Agility.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Agility.Base - OldAgi)
                    End If
                Case 2
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base /= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Stamina.Base *= CType(Target, WS_PlayerData.CharacterObject).Stamina.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Stamina.Base - OldSta)
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Stamina.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Stamina.Base - OldSta)
                    End If
                Case 3

                Case 4
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base /= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier += value
                    CType(Target, WS_PlayerData.CharacterObject).Spirit.Base *= CType(Target, WS_PlayerData.CharacterObject).Spirit.Modifier
                    If value_sign > 0 Then
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.PositiveBonus += (CType(Target, WS_PlayerData.CharacterObject).Spirit.Base - OldSpi)
                    Else
                        CType(Target, WS_PlayerData.CharacterObject).Spirit.NegativeBonus -= (CType(Target, WS_PlayerData.CharacterObject).Spirit.Base - OldSpi)
                    End If
            End Select

            CType(Target, WS_PlayerData.CharacterObject).Life.Bonus = ((CType(Target, WS_PlayerData.CharacterObject).Stamina.Base - 18) * 10)
            CType(Target, WS_PlayerData.CharacterObject).Mana.Bonus = ((CType(Target, WS_PlayerData.CharacterObject).Intellect.Base - 18) * 15)
            CType(Target, WS_PlayerData.CharacterObject).Resistances(DamageTypes.DMG_PHYSICAL).Base += (CType(Target, WS_PlayerData.CharacterObject).Agility.Base - OldAgi) * 2
            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER
            CType(Target, WS_PlayerData.CharacterObject).UpdateManaRegen()

            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.BASE_ATTACK)
            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.OFF_ATTACK)
            _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.RANGED_ATTACK)

            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_STRENGTH, CType(Target, WS_PlayerData.CharacterObject).Strength.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AGILITY, CType(Target, WS_PlayerData.CharacterObject).Agility.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_STAMINA, CType(Target, WS_PlayerData.CharacterObject).Stamina.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_SPIRIT, CType(Target, WS_PlayerData.CharacterObject).Spirit.Base)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_INTELLECT, CType(Target, WS_PlayerData.CharacterObject).Intellect.Base)
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT0, CType(CType(Target, CharacterObject).Strength.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT1, CType(CType(Target, CharacterObject).Agility.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT2, CType(CType(Target, CharacterObject).Stamina.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT3, CType(CType(Target, CharacterObject).Intellect.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POSSTAT4, CType(CType(Target, CharacterObject).Spirit.PositiveBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT0, CType(CType(Target, CharacterObject).Strength.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT1, CType(CType(Target, CharacterObject).Agility.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT2, CType(CType(Target, CharacterObject).Stamina.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT3, CType(CType(Target, CharacterObject).Intellect.NegativeBonus, Integer))
            'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_NEGSTAT4, CType(CType(Target, CharacterObject).Spirit.NegativeBonus, Integer))
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CType(Target, WS_PlayerData.CharacterObject).Life.Current)
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, CType(Target, WS_PlayerData.CharacterObject).Life.Maximum)
            If _WS_Player_Initializator.GetClassManaType(CType(Target, WS_PlayerData.CharacterObject).Classe) = ManaTypes.TYPE_MANA Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, CType(Target, WS_PlayerData.CharacterObject).Mana.Current)
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1, CType(Target, WS_PlayerData.CharacterObject).Mana.Maximum)
            End If
            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + CByte(DamageTypes.DMG_PHYSICAL), CType(Target, WS_PlayerData.CharacterObject).Resistances(DamageTypes.DMG_PHYSICAL).Base)
            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)

            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP
            CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER
        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_HEALTH(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.Life.Bonus += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Life.Bonus -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)

            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Target.Life.Maximum)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate()
                CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Target.Life.Maximum)
                UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))

                CType(Target, WS_Creatures.CreatureObject).SendToNearPlayers(packet)
                packet.Dispose()
                UpdateData.Dispose()
            End If
        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_HEALTH_PERCENT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.Life.Modifier += (EffectInfo.GetValue(Target.Level) / 100)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Life.Modifier -= (EffectInfo.GetValue(Target.Level) / 100)

            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Target.Life.Maximum)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate()
                CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag = CType(Target, WS_PlayerData.CharacterObject).GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Target.Life.Maximum)
                UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))

                CType(Target, WS_Creatures.CreatureObject).SendToNearPlayers(packet)
                packet.Dispose()
                UpdateData.Dispose()
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_INCREASE_ENERGY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If EffectInfo.MiscValue = Target.ManaType Then
                        If Not TypeOf Target Is WS_PlayerData.CharacterObject Then
                            Target.Mana.Bonus += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                        Else
                            Select Case Target.ManaType
                                Case ManaTypes.TYPE_ENERGY
                                    CType(Target, WS_PlayerData.CharacterObject).Energy.Bonus += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                                Case ManaTypes.TYPE_MANA
                                    CType(Target, WS_PlayerData.CharacterObject).Mana.Bonus += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                                Case ManaTypes.TYPE_RAGE
                                    CType(Target, WS_PlayerData.CharacterObject).Rage.Bonus += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                            End Select
                        End If
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If EffectInfo.MiscValue = Target.ManaType Then
                        If Not TypeOf Target Is WS_PlayerData.CharacterObject Then
                            Target.Mana.Bonus -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                        Else
                            Select Case Target.ManaType
                                Case ManaTypes.TYPE_ENERGY
                                    CType(Target, WS_PlayerData.CharacterObject).Energy.Bonus -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                                Case ManaTypes.TYPE_MANA
                                    CType(Target, WS_PlayerData.CharacterObject).Mana.Bonus -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                                Case ManaTypes.TYPE_RAGE
                                    CType(Target, WS_PlayerData.CharacterObject).Rage.Bonus -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                            End Select
                        End If
                    End If
            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1 + ManaTypes.TYPE_ENERGY, CType(Target, WS_PlayerData.CharacterObject).Energy.Maximum)
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1 + ManaTypes.TYPE_MANA, CType(Target, WS_PlayerData.CharacterObject).Mana.Maximum)
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1 + ManaTypes.TYPE_RAGE, CType(Target, WS_PlayerData.CharacterObject).Rage.Maximum)
            Else
                Dim packet As New Packets.UpdatePacketClass
                Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1 + Target.ManaType, Target.Mana.Maximum)
                UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))

                CType(Target, WS_Creatures.CreatureObject).SendToNearPlayers(packet)
                packet.Dispose()
                UpdateData.Dispose()
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_BASE_RESISTANCE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    For i As DamageTypes = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base += EffectInfo.GetValue(Target.Level)
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    For i As DamageTypes = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base -= EffectInfo.GetValue(Target.Level)
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_BASE_RESISTANCE_PCT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier += (EffectInfo.GetValue(Target.Level) / 100)
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier -= (EffectInfo.GetValue(Target.Level) / 100)
                            CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

            End Select
        End Sub

        Public Sub SPELL_AURA_MOD_RESISTANCE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                Target.Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).Base += EffectInfo.GetValue(Target.Level)
                                Target.Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).PositiveBonus += EffectInfo.GetValue(Target.Level)
                            Else
                                Target.Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).Base += EffectInfo.GetValue(Target.Level)
                                Target.Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).NegativeBonus -= EffectInfo.GetValue(Target.Level)
                            End If
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                Target.Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).Base -= EffectInfo.GetValue(Target.Level)
                                Target.Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).PositiveBonus -= EffectInfo.GetValue(Target.Level)
                            Else
                                Target.Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).Base -= EffectInfo.GetValue(Target.Level)
                                Target.Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                Target.Resistances(i).NegativeBonus += EffectInfo.GetValue(Target.Level)
                            End If
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

            End Select
            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
        End Sub

        Public Sub SPELL_AURA_MOD_RESISTANCE_PCT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            Dim OldBase As Short = CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier += EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).PositiveBonus += CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base - OldBase
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + i, CType(Target, CharacterObject).Resistances(i).PositiveBonus)
                            Else
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier -= EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + i, CType(Target, CharacterObject).Resistances(i).NegativeBonus)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).PositiveBonus += CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base - OldBase
                            End If
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            Dim OldBase As Short = CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier -= EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).PositiveBonus -= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base - OldBase
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + i, CType(Target, CharacterObject).Resistances(i).PositiveBonus)
                            Else
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier += EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).NegativeBonus -= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base - OldBase
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + i, CType(Target, CharacterObject).Resistances(i).NegativeBonus)
                            End If
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                        End If
                    Next

            End Select
            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
        End Sub

        Public Sub SPELL_AURA_MOD_RESISTANCE_EXCLUSIVE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base += EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).PositiveBonus += EffectInfo.GetValue(Target.Level)
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + i, CType(Target, CharacterObject).Resistances(i).PositiveBonus)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                            Else
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base -= EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).NegativeBonus -= EffectInfo.GetValue(Target.Level)
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + i, CType(Target, CharacterObject).Resistances(i).NegativeBonus)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                            End If
                        End If
                    Next

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base -= EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).PositiveBonus -= EffectInfo.GetValue(Target.Level)
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + i, CType(Target, CharacterObject).Resistances(i).PositiveBonus)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                            Else
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base /= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base += EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base *= CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Modifier
                                CType(Target, WS_PlayerData.CharacterObject).Resistances(i).PositiveBonus += EffectInfo.GetValue(Target.Level)
                                'CType(Target, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + i, CType(Target, CharacterObject).Resistances(i).NegativeBonus)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + i, CType(Target, WS_PlayerData.CharacterObject).Resistances(i).Base)
                            End If
                        End If
                    Next

            End Select
            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
        End Sub

        Public Sub SPELL_AURA_MOD_ATTACK_POWER(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.AttackPowerMods += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.AttackPowerMods -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount

            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER, CType(Target, WS_PlayerData.CharacterObject).AttackPower)
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER_MODS, CType(Target, WS_PlayerData.CharacterObject).AttackPowerMods)
                _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.BASE_ATTACK)
                _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.OFF_ATTACK)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_RANGED_ATTACK_POWER(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.AttackPowerModsRanged += EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.AttackPowerModsRanged -= EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) * StackCount

            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER, CType(Target, WS_PlayerData.CharacterObject).AttackPowerRanged)
                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER_MODS, CType(Target, WS_PlayerData.CharacterObject).AttackPowerModsRanged)
                _WS_Combat.CalculateMinMaxDamage(CType(Target, WS_PlayerData.CharacterObject), WeaponAttackType.RANGED_ATTACK)
                CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
            End If

        End Sub

        Public Sub SPELL_AURA_MOD_HEALING_DONE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Dim Value As Integer = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    CType(Target, WS_PlayerData.CharacterObject).healing.PositiveBonus += Value

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    CType(Target, WS_PlayerData.CharacterObject).healing.PositiveBonus -= Value
            End Select

            'CType(Target, CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_HEALING_DONE_POS, CType(Target, CharacterObject).healing.PositiveBonus)
            'CType(Target, CharacterObject).SendCharacterUpdate(False)
        End Sub

        Public Sub SPELL_AURA_MOD_HEALING_DONE_PCT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Dim Value As Single = (EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) / 100)
            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    CType(Target, WS_PlayerData.CharacterObject).healing.Modifier += Value

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    CType(Target, WS_PlayerData.CharacterObject).healing.Modifier -= Value
            End Select

            'CType(Target, CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_HEALING_DONE_POS, CType(Target, CharacterObject).healing.Value)
            'CType(Target, CharacterObject).SendCharacterUpdate(False)
        End Sub

        Public Sub SPELL_AURA_MOD_DAMAGE_DONE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    For i As DamageTypes = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).PositiveBonus += EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_POS + i, CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).PositiveBonus)
                            Else
                                CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).NegativeBonus -= EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG + i, CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).NegativeBonus)
                            End If
                        End If
                    Next

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    For i As DamageTypes = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            If EffectInfo.GetValue(Target.Level) > 0 Then
                                CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).PositiveBonus -= EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_POS + i, CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).PositiveBonus)
                            Else
                                CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).NegativeBonus += EffectInfo.GetValue(Target.Level)
                                CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG + i, CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).NegativeBonus)
                            End If
                        End If
                    Next
            End Select

            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
        End Sub

        Public Sub SPELL_AURA_MOD_DAMAGE_DONE_PCT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    For i As DamageTypes = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).Modifier += (EffectInfo.GetValue(Target.Level) / 100)
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT + i, CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).Modifier)
                        End If
                    Next

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    For i As DamageTypes = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                        If _Functions.HaveFlag(EffectInfo.MiscValue, i) Then
                            CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).Modifier -= (EffectInfo.GetValue(Target.Level) / 100)
                            CType(Target, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT + i, CType(Target, WS_PlayerData.CharacterObject).spellDamage(i).Modifier)
                        End If
                    Next
            End Select

            CType(Target, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)
        End Sub

        Public Sub SPELL_AURA_EMPATHY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_Creatures.CreatureObject AndAlso CType(Target, WS_Creatures.CreatureObject).CreatureInfo.CreatureType = UNIT_TYPE.BEAST Then
                        Dim packet As New Packets.UpdatePacketClass
                        Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, Target.cDynamicFlags Or DynamicFlags.UNIT_DYNFLAG_SPECIALINFO)
                        tmpUpdate.AddToPacket((packet), ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                        CType(Caster, WS_PlayerData.CharacterObject).client.Send((packet))
                        tmpUpdate.Dispose()
                        packet.Dispose()
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_Creatures.CreatureObject AndAlso CType(Target, WS_Creatures.CreatureObject).CreatureInfo.CreatureType = UNIT_TYPE.BEAST Then
                        Dim packet As New Packets.UpdatePacketClass
                        Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, Target.cDynamicFlags)
                        tmpUpdate.AddToPacket((packet), ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                        CType(Caster, WS_PlayerData.CharacterObject).client.Send((packet))
                        tmpUpdate.Dispose()
                        packet.Dispose()
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_SILENCE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.Spell_Silenced = True

                    If TypeOf Target Is WS_Creatures.CreatureObject AndAlso CType(Target, WS_Creatures.CreatureObject).aiScript IsNot Nothing Then CType(Target, WS_Creatures.CreatureObject).aiScript.OnGenerateHate(CType(Caster, WS_Base.BaseUnit), 1)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Spell_Silenced = False

            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_PACIFY(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    Target.Spell_Pacifyed = True

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Target.Spell_Pacifyed = False

            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_LANGUAGE(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_PlayerData.CharacterObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    CType(Target, WS_PlayerData.CharacterObject).Spell_Language = EffectInfo.MiscValue

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    CType(Target, WS_PlayerData.CharacterObject).Spell_Language = -1

            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_POSSESS(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If (Not TypeOf Target Is WS_Creatures.CreatureObject) AndAlso (Not TypeOf Target Is WS_PlayerData.CharacterObject) Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If Target.Level > EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level) Then Exit Sub

                    Dim packet As New Packets.UpdatePacketClass
                    Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_CHARMEDBY, Caster.GUID)
                    tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_FACTIONTEMPLATE, CType(Caster, WS_PlayerData.CharacterObject).Faction)
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_PlayerData.CharacterObject))
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                    End If
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()
                    tmpUpdate.Dispose()

                    packet = New Packets.UpdatePacketClass
                    tmpUpdate = New Packets.UpdateClass(EUnitFields.UNIT_END)
                    tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_CHARM, Target.GUID)
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_PlayerData.CharacterObject))
                    ElseIf TypeOf Caster Is WS_Creatures.CreatureObject Then
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_Creatures.CreatureObject))
                    End If
                    Caster.SendToNearPlayers(packet)
                    packet.Dispose()
                    tmpUpdate.Dispose()

                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        _WS_Pets.SendPetInitialize(Caster, Target)

                        Dim packet2 As New Packets.PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                        packet2.AddPackGUID(Target.GUID)
                        packet2.AddInt8(1)
                        CType(Caster, WS_PlayerData.CharacterObject).client.Send(packet2)
                        packet2.Dispose()

                        CType(Caster, WS_PlayerData.CharacterObject).cUnitFlags = CType(Caster, WS_PlayerData.CharacterObject).cUnitFlags Or UnitFlags.UNIT_FLAG_UNK21
                        CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, Target.GUID)
                        CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, CType(Caster, WS_PlayerData.CharacterObject).cUnitFlags)
                        CType(Caster, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)

                        CType(Caster, WS_PlayerData.CharacterObject).MindControl = Target
                    End If

                    If TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).aiScript.Reset()
                    ElseIf TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Dim packet1 As New Packets.PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                        packet1.AddPackGUID(Target.GUID)
                        packet1.AddInt8(0)
                        CType(Target, WS_PlayerData.CharacterObject).client.Send(packet1)
                        packet1.Dispose()
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    Dim packet As New Packets.UpdatePacketClass
                    Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_CHARMEDBY, 0)
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_FACTIONTEMPLATE, CType(Target, WS_PlayerData.CharacterObject).Faction)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_PlayerData.CharacterObject))
                    ElseIf TypeOf Target Is WS_Creatures.CreatureObject Then
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_FACTIONTEMPLATE, CType(Target, WS_Creatures.CreatureObject).Faction)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(Target, WS_Creatures.CreatureObject))
                    End If
                    Target.SendToNearPlayers(packet)
                    packet.Dispose()
                    tmpUpdate.Dispose()

                    packet = New Packets.UpdatePacketClass
                    tmpUpdate = New Packets.UpdateClass(EUnitFields.UNIT_END)
                    tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_CHARM, 0)
                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        tmpUpdate.AddToPacket((packet), ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_PlayerData.CharacterObject))
                    ElseIf TypeOf Caster Is WS_Creatures.CreatureObject Then
                        tmpUpdate.AddToPacket((packet), ObjectUpdateType.UPDATETYPE_VALUES, CType(Caster, WS_Creatures.CreatureObject))
                    End If
                    Caster.SendToNearPlayers(packet)
                    packet.Dispose()
                    tmpUpdate.Dispose()

                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                        Dim packet1 As New Packets.PacketClass(OPCODES.SMSG_PET_SPELLS)
                        packet1.AddUInt64(0)
                        CType(Caster, WS_PlayerData.CharacterObject).client.Send(packet1)
                        packet1.Dispose()

                        Dim packet2 As New Packets.PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                        packet2.AddPackGUID(Target.GUID)
                        packet2.AddInt8(0)
                        CType(Caster, WS_PlayerData.CharacterObject).client.Send(packet2)
                        packet2.Dispose()

                        CType(Caster, WS_PlayerData.CharacterObject).cUnitFlags = CType(Caster, WS_PlayerData.CharacterObject).cUnitFlags And (Not UnitFlags.UNIT_FLAG_UNK21)
                        CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, 0)
                        CType(Caster, WS_PlayerData.CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, CType(Caster, WS_PlayerData.CharacterObject).cUnitFlags)
                        CType(Caster, WS_PlayerData.CharacterObject).SendCharacterUpdate(False)

                        CType(Caster, WS_PlayerData.CharacterObject).MindControl = Nothing
                    End If

                    If TypeOf Target Is WS_Creatures.CreatureObject Then
                        CType(Target, WS_Creatures.CreatureObject).aiScript.State = AIState.AI_ATTACKING
                    ElseIf TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Dim packet1 As New Packets.PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                        packet1.AddPackGUID(Target.GUID)
                        packet1.AddInt8(1)
                        CType(Target, WS_PlayerData.CharacterObject).client.Send(packet1)
                        packet1.Dispose()
                    End If
            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_THREAT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)

            'NOTE: EffectInfo.MiscValue => DamageType (not used for now, till new combat sytem)
            'TODO: This does not work the correct way

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    'Target.Spell_ThreatModifier *= EffectInfo.GetValue(CType(Caster, BaseUnit).Level)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    'Target.Spell_ThreatModifier /= EffectInfo.GetValue(CType(Caster, BaseUnit).Level)

            End Select

        End Sub

        Public Sub SPELL_AURA_MOD_TOTAL_THREAT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            Dim Value As Integer

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Value = EffectInfo.GetValue(Target.Level)
                    Else
                        Value = EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                    End If

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    If TypeOf Target Is WS_PlayerData.CharacterObject Then
                        Value = -EffectInfo.GetValue(Target.Level)
                    Else
                        Value = -EffectInfo.GetValue(CType(Caster, WS_Base.BaseUnit).Level)
                    End If
            End Select

            If TypeOf Target Is WS_PlayerData.CharacterObject Then
                For Each CreatureGUID As ULong In CType(Target, WS_PlayerData.CharacterObject).creaturesNear
                    If Not _WorldServer.WORLD_CREATUREs(CreatureGUID).aiScript Is Nothing AndAlso
                       _WorldServer.WORLD_CREATUREs(CreatureGUID).aiScript.aiHateTable.ContainsKey(Target) Then
                        _WorldServer.WORLD_CREATUREs(CreatureGUID).aiScript.OnGenerateHate(Target, Value)
                    End If
                Next
            Else
                If Not CType(Target, WS_Creatures.CreatureObject).aiScript Is Nothing AndAlso
                   CType(Target, WS_Creatures.CreatureObject).aiScript.aiHateTable.ContainsKey(Caster) Then
                    CType(Target, WS_Creatures.CreatureObject).aiScript.OnGenerateHate(Caster, Value)
                End If
            End If
        End Sub

        Public Sub SPELL_AURA_MOD_TAUNT(ByRef Target As WS_Base.BaseUnit, ByRef Caster As WS_Base.BaseObject, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal StackCount As Integer, ByVal Action As AuraAction)
            If Not TypeOf Target Is WS_Creatures.CreatureObject Then Exit Sub

            Select Case Action
                Case AuraAction.AURA_UPDATE
                    Exit Sub

                Case AuraAction.AURA_ADD
                    CType(Target, WS_Creatures.CreatureObject).aiScript.OnGenerateHate(Caster, 9999999)

                Case AuraAction.AURA_REMOVE, AuraAction.AURA_REMOVEBYDURATION
                    CType(Target, WS_Creatures.CreatureObject).aiScript.OnGenerateHate(Caster, -9999999)

            End Select

        End Sub

#End Region

#Region "WS.Spells.Handlers.Duel"

        Public Const DUEL_COUNTDOWN As Integer = 3000              'in miliseconds
        Const DUEL_OUTOFBOUNDS_DISTANCE As Single = 40.0F

        Public Const DUEL_COUNTER_START As Byte = 10
        Public Const DUEL_COUNTER_DISABLED As Byte = 11

        Public Sub CheckDuelDistance(ByRef objCharacter As WS_PlayerData.CharacterObject)
            If _WorldServer.WORLD_GAMEOBJECTs.ContainsKey(objCharacter.DuelArbiter) = False Then Exit Sub
            If _WS_Combat.GetDistance(objCharacter, _WorldServer.WORLD_GAMEOBJECTs(objCharacter.DuelArbiter)) > DUEL_OUTOFBOUNDS_DISTANCE Then
                If objCharacter.DuelOutOfBounds = DUEL_COUNTER_DISABLED Then
                    'DONE: Notify for out of bounds of the duel flag and start counting
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_DUEL_OUTOFBOUNDS)
                    objCharacter.client.Send(packet)
                    packet.Dispose()

                    objCharacter.DuelOutOfBounds = DUEL_COUNTER_START
                End If
            Else
                If objCharacter.DuelOutOfBounds <> DUEL_COUNTER_DISABLED Then
                    objCharacter.DuelOutOfBounds = DUEL_COUNTER_DISABLED

                    'DONE: Notify for in bounds of the duel flag
                    Dim packet As New Packets.PacketClass(OPCODES.SMSG_DUEL_INBOUNDS)
                    objCharacter.client.Send(packet)
                    packet.Dispose()
                End If
            End If
        End Sub

        Public Sub DuelComplete(ByRef Winner As WS_PlayerData.CharacterObject, ByRef Loser As WS_PlayerData.CharacterObject)
            If Winner Is Nothing Then Exit Sub
            If Loser Is Nothing Then Exit Sub

            'DONE: First stop the fight
            Dim response As New Packets.PacketClass(OPCODES.SMSG_DUEL_COMPLETE)
            response.AddInt8(1)
            Winner.client.SendMultiplyPackets(response)
            Loser.client.SendMultiplyPackets(response)
            response.Dispose()

            'DONE: Stop timed attacks for both
            Winner.FinishSpell(CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL)
            Winner.FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL)
            Winner.AutoShotSpell = 0
            Winner.attackState.AttackStop()
            Loser.FinishSpell(CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL)
            Loser.FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL)
            Loser.AutoShotSpell = 0
            Loser.attackState.AttackStop()

            'DONE: Clear duel things
            If _WorldServer.WORLD_GAMEOBJECTs.ContainsKey(Winner.DuelArbiter) Then _WorldServer.WORLD_GAMEOBJECTs(Winner.DuelArbiter).Destroy(_WorldServer.WORLD_GAMEOBJECTs(Winner.DuelArbiter))

            Winner.DuelOutOfBounds = DUEL_COUNTER_DISABLED
            Winner.DuelArbiter = 0
            Winner.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_ARBITER, 0)
            Winner.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_TEAM, 0)
            Winner.RemoveFromCombat(Loser)

            Loser.DuelOutOfBounds = DUEL_COUNTER_DISABLED
            Loser.DuelArbiter = 0
            Loser.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_ARBITER, 0)
            Loser.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_TEAM, 0)
            Loser.RemoveFromCombat(Winner)

            'DONE: Remove all spells by your duel partner
            For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECTs_VISIBLE - 1
                If Winner.ActiveSpells(i) IsNot Nothing Then Winner.RemoveAura(i, Winner.ActiveSpells(i).SpellCaster)
                If Loser.ActiveSpells(i) IsNot Nothing Then Loser.RemoveAura(i, Loser.ActiveSpells(i).SpellCaster)
            Next

            'DONE: Update life
            If Loser.Life.Current = 0 Then
                Loser.Life.Current = 1
                Loser.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, 1)
                Loser.SetUpdateFlag(EUnitFields.UNIT_NPC_EMOTESTATE, EmoteStates.ANIM_EMOTE_BEG)
            End If
            Loser.SendCharacterUpdate(True)
            Winner.SendCharacterUpdate(True)

            'DONE: Notify client
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_DUEL_WINNER)
            packet.AddInt8(0)
            packet.AddString(Winner.Name)
            packet.AddInt8(1)
            packet.AddString(Loser.Name)
            Winner.SendToNearPlayers(packet)
            packet.Dispose()

            'DONE: Beg emote for loser
            Dim SMSG_EMOTE As New Packets.PacketClass(OPCODES.SMSG_EMOTE)
            SMSG_EMOTE.AddInt32(Emotes.ONESHOT_BEG)
            SMSG_EMOTE.AddUInt64(Loser.GUID)
            Loser.SendToNearPlayers(SMSG_EMOTE)
            SMSG_EMOTE.Dispose()

            'DONE: Final clearing (if we clear it before we can't get names)
            Dim tmpCharacter As WS_PlayerData.CharacterObject
            tmpCharacter = Winner
            Loser.DuelPartner = Nothing
            tmpCharacter.DuelPartner = Nothing
            tmpCharacter = Nothing
        End Sub

        Public Sub On_CMSG_DUEL_ACCEPTED(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_DUEL_ACCEPTED [{2:X}]", client.IP, client.Port, GUID)

            'NOTE: Only invited player must have GUID set up
            If client.Character.DuelArbiter <> GUID Then Exit Sub

            Dim c1 As WS_PlayerData.CharacterObject = client.Character.DuelPartner
            Dim c2 As WS_PlayerData.CharacterObject = client.Character
            c1.DuelArbiter = GUID
            c2.DuelArbiter = GUID

            'DONE: Do updates
            c1.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_ARBITER, c1.DuelArbiter)
            'c1.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_TEAM, 1)
            c2.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_ARBITER, c2.DuelArbiter)
            'c2.SetUpdateFlag(EPlayerFields.PLAYER_DUEL_TEAM, 2)
            c2.SendCharacterUpdate(True)
            c1.SendCharacterUpdate(True)

            'DONE: Start the duel
            Dim response As New Packets.PacketClass(OPCODES.SMSG_DUEL_COUNTDOWN)
            response.AddInt32(DUEL_COUNTDOWN)
            c1.client.SendMultiplyPackets(response)
            c2.client.SendMultiplyPackets(response)
            response.Dispose()

            Dim StartDuel As New Thread(AddressOf c2.StartDuel) With {
                    .Name = "Duel timer"
                    }

            StartDuel.Start()
        End Sub

        Public Sub On_CMSG_DUEL_CANCELLED(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_DUEL_CANCELLED [{2:X}]", client.IP, client.Port, GUID)

            'DONE: Clear for client
            _WorldServer.WORLD_GAMEOBJECTs(client.Character.DuelArbiter).Despawn()
            client.Character.DuelArbiter = 0
            client.Character.DuelPartner.DuelArbiter = 0

            Dim response As New Packets.PacketClass(OPCODES.SMSG_DUEL_COMPLETE)
            response.AddInt8(0)
            client.Character.client.SendMultiplyPackets(response)
            client.Character.DuelPartner.client.SendMultiplyPackets(response)
            response.Dispose()

            'DONE: Final clearing
            client.Character.DuelPartner.DuelPartner = Nothing
            client.Character.DuelPartner = Nothing
        End Sub

        Public Sub On_CMSG_RESURRECT_RESPONSE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 14 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64
            Dim Status As Byte = packet.GetInt8

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_RESURRECT_RESPONSE [GUID={2:X} Status={3}]", client.IP, client.Port, GUID, Status)

            If Status = 0 Then
                'DONE: Decline the request
                client.Character.resurrectGUID = 0
                client.Character.resurrectMapID = 0
                client.Character.resurrectPositionX = 0
                client.Character.resurrectPositionY = 0
                client.Character.resurrectPositionZ = 0
                client.Character.resurrectHealth = 0
                client.Character.resurrectMana = 0
                Exit Sub
            End If
            If GUID <> client.Character.resurrectGUID Then Exit Sub

            'DONE: Resurrect
            _WS_Handlers_Misc.CharacterResurrect(client.Character)
            client.Character.Life.Current = client.Character.resurrectHealth
            If client.Character.ManaType = ManaTypes.TYPE_MANA Then client.Character.Mana.Current = client.Character.resurrectMana
            client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, client.Character.Life.Current)
            If client.Character.ManaType = ManaTypes.TYPE_MANA Then client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, client.Character.Mana.Current)
            client.Character.SendCharacterUpdate()
            client.Character.Teleport(client.Character.resurrectPositionX, client.Character.resurrectPositionY, client.Character.resurrectPositionZ, client.Character.orientation, client.Character.resurrectMapID)
        End Sub

#End Region

#Region "WS.Spells.Handlers"
        Public Sub On_CMSG_CAST_SPELL(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            'If (packet.Data.Length - 1) < 11 Then Exit Sub
            packet.GetInt16()
            Dim spellID As Integer = packet.GetInt32
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CSMG_CAST_SPELL [spellID={2}]", client.IP, client.Port, spellID)
            If Not client.Character.HaveSpell(spellID) Then
                _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] CHEAT: Character {2} casting unlearned spell {3}!", client.IP, client.Port, client.Character.Name, spellID)
                Exit Sub
            End If
            If SPELLs.ContainsKey(spellID) = False Then
                _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] Character tried to cast a spell that didn't exist: {2}!", client.IP, client.Port, spellID)
                Exit Sub
            End If
            Dim spellCooldown As UInteger = client.Character.Spells(spellID).Cooldown
            If spellCooldown >= 0UI Then
                Dim timeNow As UInteger = _Functions.GetTimestamp(Now)
                If timeNow >= spellCooldown Then
                    client.Character.Spells(spellID).Cooldown = 0UI
                    client.Character.Spells(spellID).CooldownItem = 0
                Else
                    Exit Sub 'The cooldown has not ran off yet
                End If
            End If

            'TODO: In duel disable

            Dim SpellType As CurrentSpellTypes = CurrentSpellTypes.CURRENT_GENERIC_SPELL
            If SPELLs(spellID).IsAutoRepeat Then
                SpellType = CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL
                Dim tmpSpellID As Integer = 0
                If client.Character.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) Then
                    'Select Case client.Character.Items(EQUIPMENT_SLOT_RANGED).ItemInfo.SubClass
                    '    Case ITEM_SUBCLASS.ITEM_SUBCLASS_BOW, ITEM_SUBCLASS.ITEM_SUBCLASS_GUN, ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW
                    '        tmpSpellID = 3018
                    '    Case ITEM_SUBCLASS.ITEM_SUBCLASS_THROWN
                    '        tmpSpellID = 2764
                    '    Case ITEM_SUBCLASS.ITEM_SUBCLASS_WAND
                    '        tmpSpellID = 5019
                    '    Case Else
                    '        tmpSpellID = spellID
                    'End Select

                    If client.Character.AutoShotSpell = 0 Then
                        Try
                            client.Character.AutoShotSpell = spellID
                            client.Character.attackState.Ranged = True
                            client.Character.attackState.AttackStart(client.Character.GetTarget)
                        Catch e As Exception
                            _WorldServer.Log.WriteLine(LogType.FAILED, "Error casting auto-shoot {0}.{1}", spellID, Environment.NewLine & e.ToString)
                        End Try
                    End If
                End If
                Exit Sub
            ElseIf SPELLs(spellID).channelInterruptFlags <> 0 Then
                SpellType = CurrentSpellTypes.CURRENT_CHANNELED_SPELL
            ElseIf SPELLs(spellID).IsMelee Then
                SpellType = CurrentSpellTypes.CURRENT_MELEE_SPELL
            End If

            Dim Targets As New SpellTargets
            Dim castResult As SpellFailedReason = SpellFailedReason.SPELL_FAILED_ERROR
            Try
                Targets.ReadTargets(packet, (client.Character))
                castResult = SPELLs(spellID).CanCast(client.Character, Targets, True)
                If client.Character.spellCasted(SpellType) IsNot Nothing AndAlso client.Character.spellCasted(SpellType).Finished = False Then castResult = SpellFailedReason.SPELL_FAILED_SPELL_IN_PROGRESS
                If castResult = SpellFailedReason.SPELL_NO_ERROR Then
                    Dim tmpSpell As New CastSpellParameters(Targets, client.Character, spellID)
                    client.Character.spellCasted(SpellType) = tmpSpell
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf tmpSpell.Cast))
                Else
                    SendCastResult(castResult, client, spellID)
                End If

            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.FAILED, "Error casting spell {0}.{1}", spellID, Environment.NewLine & e.ToString)
                SendCastResult(castResult, client, spellID)
            End Try
        End Sub

        Public Sub On_CMSG_CANCEL_CAST(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim SpellID As Integer = packet.GetInt32
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CANCEL_CAST", client.IP, client.Port)

            'TODO: Other players can't see when you are interrupting your spells

            If client.Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL) IsNot Nothing AndAlso client.Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL).SpellID = SpellID Then
                client.Character.FinishSpell(CurrentSpellTypes.CURRENT_GENERIC_SPELL)
            ElseIf client.Character.spellCasted(CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL) IsNot Nothing AndAlso client.Character.spellCasted(CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL).SpellID = SpellID Then
                client.Character.FinishSpell(CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL)
            ElseIf client.Character.spellCasted(CurrentSpellTypes.CURRENT_CHANNELED_SPELL) IsNot Nothing AndAlso client.Character.spellCasted(CurrentSpellTypes.CURRENT_CHANNELED_SPELL).SpellID = SpellID Then
                client.Character.FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL)
            ElseIf client.Character.spellCasted(CurrentSpellTypes.CURRENT_MELEE_SPELL) IsNot Nothing AndAlso client.Character.spellCasted(CurrentSpellTypes.CURRENT_MELEE_SPELL).SpellID = SpellID Then
                client.Character.FinishSpell(CurrentSpellTypes.CURRENT_MELEE_SPELL)
            End If
        End Sub

        Public Sub On_CMSG_CANCEL_AUTO_REPEAT_SPELL(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CANCEL_AUTO_REPEAT_SPELL", client.IP, client.Port)

            client.Character.FinishSpell(CurrentSpellTypes.CURRENT_AUTOREPEAT_SPELL)
        End Sub

        Public Sub On_CMSG_CANCEL_CHANNELLING(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CANCEL_CHANNELLING", client.IP, client.Port)

            client.Character.FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL)
        End Sub

        Public Sub On_CMSG_CANCEL_AURA(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim spellID As Integer = packet.GetInt32
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CANCEL_AURA [spellID={2}]", client.IP, client.Port, spellID)

            client.Character.RemoveAuraBySpell(spellID)
        End Sub

        Public Sub On_CMSG_LEARN_TALENT(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            Try
                If (packet.Data.Length - 1) < 13 Then Exit Sub
                packet.GetInt16()
                Dim TalentID As Integer = packet.GetInt32()
                Dim RequestedRank As Integer = packet.GetInt32()
                Dim CurrentTalentPoints As Byte = client.Character.TalentPoints
                Dim SpellID As Integer
                Dim ReSpellID As Integer
                Dim j As Integer
                Dim HasEnoughRank As Boolean
                Dim DependsOn As Integer
                Dim SpentPoints As Integer

                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LEARN_TALENT [{2}:{3}]", client.IP, client.Port, TalentID, RequestedRank)

                If CurrentTalentPoints = 0 Then Exit Sub
                If RequestedRank > 4 Then Exit Sub

                'DONE: Now the character can't cheat, he must have the earlier rank to get the new one
                If RequestedRank > 0 Then
                    If Not client.Character.HaveSpell(_WS_DBCDatabase.Talents(TalentID).RankID(RequestedRank - 1)) Then
                        Exit Sub
                    End If
                End If

                'DONE: Now the character can't cheat, he must have the other talents that is needed to get this one
                For j = 0 To 2
                    If _WS_DBCDatabase.Talents(TalentID).RequiredTalent(j) > 0 Then
                        HasEnoughRank = False
                        DependsOn = _WS_DBCDatabase.Talents(TalentID).RequiredTalent(j)
                        For i As Integer = _WS_DBCDatabase.Talents(TalentID).RequiredPoints(j) To 4
                            If _WS_DBCDatabase.Talents(DependsOn).RankID(i) <> 0 Then
                                If client.Character.HaveSpell(_WS_DBCDatabase.Talents(DependsOn).RankID(i)) Then
                                    HasEnoughRank = True
                                End If
                            End If
                        Next i

                        If HasEnoughRank = False Then Exit Sub
                    End If
                Next j

                'DONE: Count spent talent points
                SpentPoints = 0
                If _WS_DBCDatabase.Talents(TalentID).Row > 0 Then
                    For Each TalentInfo As KeyValuePair(Of Integer, WS_DBCDatabase.TalentInfo) In _WS_DBCDatabase.Talents
                        If _WS_DBCDatabase.Talents(TalentID).TalentTab = TalentInfo.Value.TalentTab Then
                            For i As Integer = 0 To 4
                                If TalentInfo.Value.RankID(i) <> 0 Then
                                    If client.Character.HaveSpell(TalentInfo.Value.RankID(i)) Then
                                        SpentPoints += i + 1
                                    End If
                                End If
                            Next i
                        End If
                    Next
                End If

#If DEBUG Then
                _WorldServer.Log.WriteLine(LogType.INFORMATION, "_WS_DBCDatabase.Talents spent: {0}", SpentPoints)
#End If

                If SpentPoints < (_WS_DBCDatabase.Talents(TalentID).Row * 5) Then Exit Sub

                SpellID = _WS_DBCDatabase.Talents(TalentID).RankID(RequestedRank)

                If SpellID = 0 Then Exit Sub

                If client.Character.HaveSpell(SpellID) Then Exit Sub

                client.Character.LearnSpell(SpellID)

                'DONE: Cast passive talents on the character
                If SPELLs.ContainsKey(SpellID) AndAlso (SPELLs(SpellID).IsPassive) Then client.Character.ApplySpell(SpellID)

                'DONE: Unlearning the earlier rank of the talent
                If RequestedRank > 0 Then
                    ReSpellID = _WS_DBCDatabase.Talents(TalentID).RankID(RequestedRank - 1)
                    client.Character.UnLearnSpell(ReSpellID)
                    client.Character.RemoveAuraBySpell(ReSpellID)
                End If

                'DONE: Remove 1 talentpoint from the character
                client.Character.TalentPoints -= 1
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_CHARACTER_POINTS1, client.Character.TalentPoints)
                client.Character.SendCharacterUpdate(True)

                client.Character.SaveCharacter()
            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.FAILED, "Error learning talen: {0}{1}", Environment.NewLine, e.ToString)
            End Try
        End Sub

#End Region

#Region "WS.Spells.Loot"
        Public Sub SendLoot(ByVal Player As WS_PlayerData.CharacterObject, ByVal GUID As ULong, ByVal LootingType As LootType)
            If _CommonGlobalFunctions.GuidIsGameObject(GUID) Then
                Select Case _WorldServer.WORLD_GAMEOBJECTs(GUID).ObjectInfo.Type
                    Case GameObjectType.GAMEOBJECT_TYPE_DOOR, GameObjectType.GAMEOBJECT_TYPE_BUTTON
                        Exit Sub
                    Case GameObjectType.GAMEOBJECT_TYPE_QUESTGIVER
                        Exit Sub
                    Case GameObjectType.GAMEOBJECT_TYPE_SPELL_FOCUS
                        Exit Sub
                    Case GameObjectType.GAMEOBJECT_TYPE_GOOBER
                        Exit Sub
                    Case GameObjectType.GAMEOBJECT_TYPE_CHEST
                        'TODO: Script events
                        'Note: Don't exit sub here! We need the loot if it's a chest :P
                End Select
            End If

            'DONE: Sending loot
            _WorldServer.WORLD_GAMEOBJECTs(GUID).LootObject(Player, LootingType)
        End Sub
#End Region

    End Class
End Namespace