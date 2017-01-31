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


Public Module WS_Combat

#Region "WS.Combat.Constants"

    Public Enum ProcFlags
        PROC_FLAG_NONE = &H0                            ' None
        PROC_FLAG_HIT_MELEE = &H1                       ' On melee hit
        PROC_FLAG_STRUCK_MELEE = &H2                    ' On being struck melee
        PROC_FLAG_KILL_XP_GIVER = &H4                   ' On kill target giving XP or honor
        PROC_FLAG_SPECIAL_DROP = &H8                    '
        PROC_FLAG_DODGE = &H10                          ' On dodge melee attack
        PROC_FLAG_PARRY = &H20                          ' On parry melee attack
        PROC_FLAG_BLOCK = &H40                          ' On block attack
        PROC_FLAG_TOUCH = &H80                          ' On being touched (for bombs, probably?)
        PROC_FLAG_TARGET_LOW_HEALTH = &H100             ' On deal damage to enemy with 20% or less health
        PROC_FLAG_LOW_HEALTH = &H200                    ' On health dropped below 20%
        PROC_FLAG_STRUCK_RANGED = &H400                 ' On being struck ranged
        PROC_FLAG_HIT_SPECIAL = &H800                   ' (!)Removed, may be reassigned in future
        PROC_FLAG_CRIT_MELEE = &H1000                   ' On crit melee
        PROC_FLAG_STRUCK_CRIT_MELEE = &H2000            ' On being critically struck in melee
        PROC_FLAG_CAST_SPELL = &H4000                   ' On cast spell
        PROC_FLAG_TAKE_DAMAGE = &H8000                  ' On take damage
        PROC_FLAG_CRIT_SPELL = &H10000                  ' On crit spell
        PROC_FLAG_HIT_SPELL = &H20000                   ' On hit spell
        PROC_FLAG_STRUCK_CRIT_SPELL = &H40000           ' On being critically struck by a spell
        PROC_FLAG_HIT_RANGED = &H80000                  ' On getting ranged hit
        PROC_FLAG_STRUCK_SPELL = &H100000               ' On being struck by a spell
        PROC_FLAG_TRAP = &H200000                       ' On trap activation (?)
        PROC_FLAG_CRIT_RANGED = &H400000                ' On getting ranged crit
        PROC_FLAG_STRUCK_CRIT_RANGED = &H800000         ' On being critically struck by a ranged attack
        PROC_FLAG_RESIST_SPELL = &H1000000              ' On resist enemy spell
        PROC_FLAG_TARGET_RESISTS = &H2000000            ' On enemy resisted spell
        PROC_FLAG_TARGET_DODGE_OR_PARRY = &H4000000     ' On enemy dodges/parries
        PROC_FLAG_HEAL = &H8000000                      ' On heal
        PROC_FLAG_CRIT_HEAL = &H10000000                ' On critical healing effect
        PROC_FLAG_HEALED = &H20000000                   ' On healing
        PROC_FLAG_TARGET_BLOCK = &H40000000             ' On enemy blocks
        PROC_FLAG_MISS = &H80000000                     ' On miss melee attack
    End Enum

    Public Enum WeaponAttackType As Byte
        BASE_ATTACK = 0
        OFF_ATTACK = 1
        RANGED_ATTACK = 2
    End Enum

#End Region

#Region "WS.Combat.Calculations"

    Public Sub DoEmote(ByVal AnimationID As Integer, ByRef Unit As BaseObject)
        'EMOTE_ONESHOT_WOUNDCRITICAL
        'EMOTE_ONESHOT_PARRYSHIELD
        'EMOTE_ONESHOT_PARRYUNARMED

        Dim packet As New PacketClass(OPCODES.SMSG_EMOTE)
        packet.AddInt32(AnimationID)
        packet.AddUInt64(Unit.GUID)

        Unit.SendToNearPlayers(packet)
        packet.Dispose()
    End Sub

    Public Function GetWeaponDmg(ByRef objCharacter As CharacterObject, ByVal AttackType As WeaponAttackType, ByVal MaxDmg As Boolean) As Single
        Dim WepSlot As Byte
        Select Case AttackType
            Case WeaponAttackType.BASE_ATTACK
                WepSlot = EquipmentSlots.EQUIPMENT_SLOT_MAINHAND
            Case WeaponAttackType.OFF_ATTACK
                WepSlot = EquipmentSlots.EQUIPMENT_SLOT_OFFHAND
            Case WeaponAttackType.RANGED_ATTACK
                WepSlot = EquipmentSlots.EQUIPMENT_SLOT_RANGED
            Case Else
                Return 0
        End Select

        If objCharacter.Items.ContainsKey(WepSlot) = False OrElse objCharacter.Items(WepSlot).ItemInfo.ObjectClass <> ITEM_CLASS.ITEM_CLASS_WEAPON OrElse objCharacter.Items(WepSlot).IsBroken Then Return 0.0F

        Dim Dmg As Single = 0
        For i As Byte = 0 To 4
            If MaxDmg Then
                Dmg += objCharacter.Items(WepSlot).ItemInfo.Damage(i).Maximum
            Else
                Dmg += objCharacter.Items(WepSlot).ItemInfo.Damage(i).Minimum
            End If
        Next
        Return Dmg
    End Function

    Public Function GetAPMultiplier(ByRef objCharacter As BaseUnit, ByVal AttackType As WeaponAttackType, ByVal Normalized As Boolean) As Single
        If Normalized = False OrElse (Not TypeOf objCharacter Is CharacterObject) Then
            Select Case AttackType
                Case WeaponAttackType.BASE_ATTACK
                    Return (CType(objCharacter, CreatureObject).CreatureInfo.BaseAttackTime / 1000.0F)
                Case WeaponAttackType.RANGED_ATTACK
                    Return (CType(objCharacter, CreatureObject).CreatureInfo.BaseRangedAttackTime / 1000.0F)
                Case Else
                    Return 0.0F
            End Select
        End If

        Dim Weapon As ItemObject = Nothing
        Select Case AttackType
            Case WeaponAttackType.BASE_ATTACK
                If CType(objCharacter, CharacterObject).Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) = False Then Return 2.4F 'Fist attack
                Weapon = CType(objCharacter, CharacterObject).Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND)
            Case WeaponAttackType.OFF_ATTACK
                If CType(objCharacter, CharacterObject).Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) = False Then Return 2.4F 'Fist attack
                Weapon = CType(objCharacter, CharacterObject).Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND)
            Case WeaponAttackType.RANGED_ATTACK
                If CType(objCharacter, CharacterObject).Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) = False Then Return 0.0F
                Weapon = CType(objCharacter, CharacterObject).Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED)
            Case Else
                Return 0.0F
        End Select

        If Weapon Is Nothing OrElse Weapon.ItemInfo.ObjectClass <> ITEM_CLASS.ITEM_CLASS_WEAPON Then
            If AttackType = WeaponAttackType.RANGED_ATTACK Then Return 0.0F
            Return 2.4F
        End If

        Select Case Weapon.ItemInfo.InventoryType
            Case INVENTORY_TYPES.INVTYPE_TWOHAND_WEAPON
                Return 3.3F
            Case INVENTORY_TYPES.INVTYPE_RANGED, INVENTORY_TYPES.INVTYPE_RANGEDRIGHT, INVENTORY_TYPES.INVTYPE_THROWN
                Return 2.8F
            Case Else
                If Weapon.ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_DAGGER Then Return 1.7
                Return 2.4F
        End Select
    End Function

    Public Sub CalculateMinMaxDamage(ByRef objCharacter As CharacterObject, ByVal AttackType As WeaponAttackType)
        Dim AttSpeed As Single = GetAPMultiplier(objCharacter, AttackType, True)
        Dim BaseValue As Single = 0
        Dim BasePercent As Single = 1
        Select Case AttackType
            Case WeaponAttackType.BASE_ATTACK, WeaponAttackType.OFF_ATTACK
                BaseValue = objCharacter.AttackPower + objCharacter.AttackPowerMods
            Case WeaponAttackType.RANGED_ATTACK
                BaseValue = objCharacter.AttackPowerRanged + objCharacter.AttackPowerModsRanged
            Case Else
                Exit Sub
        End Select
        BaseValue = BaseValue / 14.0F * AttSpeed

        Dim WepMin As Single = GetWeaponDmg(objCharacter, AttackType, False)
        Dim WepMax As Single = GetWeaponDmg(objCharacter, AttackType, True)

        If AttackType = WeaponAttackType.RANGED_ATTACK Then 'Add ammo dps
            If objCharacter.AmmoID > 0 Then
                Dim AmmoDmg As Single = (objCharacter.AmmoDPS / (1 / objCharacter.AmmoMod)) * AttSpeed
                WepMin += AmmoDmg
                WepMax += AmmoDmg
            End If
        ElseIf objCharacter.ShapeshiftForm = ShapeshiftForm.FORM_BEAR OrElse objCharacter.ShapeshiftForm = ShapeshiftForm.FORM_DIREBEAR OrElse objCharacter.ShapeshiftForm = ShapeshiftForm.FORM_CAT Then
            WepMin += objCharacter.Level * 0.85 * AttSpeed
            WepMax += objCharacter.Level * 0.85 * AttSpeed
        End If

        Dim MinDamage As Single = (BaseValue + WepMin) * BasePercent
        Dim MaxDamage As Single = (BaseValue + WepMax) * BasePercent

        Select Case AttackType
            Case WeaponAttackType.BASE_ATTACK
                objCharacter.Damage.Minimum = MinDamage
                objCharacter.Damage.Maximum = MaxDamage
                objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MINDAMAGE, objCharacter.Damage.Minimum)
                objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXDAMAGE, objCharacter.Damage.Maximum)
            Case WeaponAttackType.OFF_ATTACK
                objCharacter.OffHandDamage.Minimum = MinDamage
                objCharacter.OffHandDamage.Maximum = MaxDamage
                objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MINOFFHANDDAMAGE, objCharacter.OffHandDamage.Minimum)
                objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXOFFHANDDAMAGE, objCharacter.OffHandDamage.Maximum)
            Case WeaponAttackType.RANGED_ATTACK
                objCharacter.RangedDamage.Minimum = MinDamage
                objCharacter.RangedDamage.Maximum = MaxDamage
                objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MINRANGEDDAMAGE, objCharacter.RangedDamage.Minimum)
                objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXRANGEDDAMAGE, objCharacter.RangedDamage.Maximum)
        End Select
    End Sub

    Public Function CalculateDamage(ByRef Attacker As BaseUnit, ByRef Victim As BaseUnit, ByVal DualWield As Boolean, ByVal Ranged As Boolean, Optional ByVal Ability As SpellInfo = Nothing, Optional ByVal Effect As SpellEffect = Nothing) As DamageInfo
        Dim result As DamageInfo

        'DONE: Initialize result
        result.victimState = AttackVictimState.VICTIMSTATE_NORMAL
        result.Blocked = 0
        result.Absorbed = 0
        result.Turn = 0
        result.HitInfo = 0
        If DualWield Then result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_LEFTSWING

        If Ability IsNot Nothing Then
            result.DamageType = Ability.School
        Else
            'TODO: Get creature damage type
            If TypeOf Attacker Is CharacterObject Then
                With CType(Attacker, CharacterObject)
                    If Ranged Then
                        If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) Then
                            result.DamageType = .Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.Damage(0).Type
                        End If
                    ElseIf DualWield Then
                        If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) Then
                            result.DamageType = .Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).ItemInfo.Damage(0).Type
                        End If
                    Else
                        If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) Then
                            result.DamageType = .Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).ItemInfo.Damage(0).Type
                        End If
                    End If
                End With
            Else
                result.DamageType = DamageTypes.DMG_PHYSICAL
            End If
        End If

        If TypeOf Victim Is CreatureObject AndAlso CType(Victim, CreatureObject).aiScript IsNot Nothing Then
            If CType(Victim, CreatureObject).aiScript.State = TBaseAI.AIState.AI_MOVING_TO_SPAWN Then
                result.HitInfo = result.HitInfo Or AttackHitState.HIT_MISS
                Return result
            End If
        End If

        'DONE: Miss chance calculation
        'http://www.wowwiki.com/Formulas:Weapon_Skill
        Dim skillDiference As Integer = GetSkillWeapon(Attacker, DualWield)

        'http://www.wowwiki.com/Defense
        skillDiference -= GetSkillDefence(Victim)
        If TypeOf Victim Is CharacterObject Then CType(Victim, CharacterObject).UpdateSkill(SKILL_IDs.SKILL_DEFENSE)

        'DONE: Final calculations
        Dim chanceToMiss As Single = GetBasePercentMiss(Attacker, skillDiference)
        Dim chanceToCrit As Single = GetBasePercentCrit(Attacker, skillDiference)
        Dim chanceToBlock As Single = GetBasePercentBlock(Victim, skillDiference)
        Dim chanceToParry As Single = GetBasePercentParry(Victim, skillDiference)
        Dim chanceToDodge As Single = GetBasePercentDodge(Victim, skillDiference)

        'DONE: Glancing blow (only VS Creatures)
        Dim chanceToGlancingBlow As Short = 0
        If (TypeOf Attacker Is CharacterObject) AndAlso (TypeOf Victim Is CreatureObject) AndAlso (Attacker.Level > Victim.Level + 2) AndAlso skillDiference <= -15 Then chanceToGlancingBlow = (Victim.Level - CInt(Attacker.Level)) * 10

        'DONE: Crushing blow, fix real damage (only for Creatures)
        Dim chanceToCrushingBlow As Short = 0
        If (TypeOf Attacker Is CreatureObject) AndAlso (TypeOf Victim Is CharacterObject) AndAlso Ability Is Nothing AndAlso (Attacker.Level > Victim.Level + 2) Then chanceToCrushingBlow = (skillDiference * 2.0F - 15)

        'DONE: Some limitations
        If chanceToMiss > 60.0F Then chanceToMiss = 60.0F
        If chanceToGlancingBlow > 40.0F Then chanceToGlancingBlow = 40.0F
        If chanceToMiss < 0.0F Then chanceToMiss = 0.0F
        If chanceToCrit < 0.0F Then chanceToCrit = 0.0F
        If chanceToBlock < 0.0F Then chanceToBlock = 0.0F
        If chanceToParry < 0.0F Then chanceToParry = 0.0F
        If chanceToDodge < 0.0F Then chanceToDodge = 0.0F
        If chanceToGlancingBlow < 0.0F Then chanceToGlancingBlow = 0.0F
        If chanceToCrushingBlow < 0.0F Then chanceToCrushingBlow = 0.0F

        'DONE: Always crit against a sitting target
        If TypeOf Victim Is CharacterObject AndAlso CType(Victim, CharacterObject).StandState <> 0 Then
            chanceToCrit = 100.0F
            chanceToCrushingBlow = 0.0F
        End If

        'DONE: No glancing with ranged weapon
        If Ranged Then
            chanceToGlancingBlow = 0.0F
        End If

        'DONE: Calculating the damage
        GetDamage(Attacker, DualWield, result)
        If Effect IsNot Nothing Then
            result.Damage += Effect.GetValue(Attacker.Level)
        End If

        'DONE: Damage reduction
        Dim DamageReduction As Single = Victim.GetDamageReduction(Attacker, result.DamageType, result.Damage)
        result.Damage -= result.Damage * DamageReduction

        'TODO: More aurastates!
        'DONE: Rolling the dice
        Dim roll As Single = Rnd.Next(0, 10000) / 100
        Select Case roll
            Case Is < chanceToMiss
                'DONE: Miss attack
                result.Damage = 0
                result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_MISS
            Case Is < chanceToMiss + chanceToDodge
                'DONE: Dodge attack
                result.Damage = 0
                result.victimState = AttackVictimState.VICTIMSTATE_DODGE
                DoEmote(Emotes.ONESHOT_PARRYUNARMED, Victim)
                'TODO: Remove after 5 secs?
                Victim.AuraState = Victim.AuraState Or SpellAuraStates.AURASTATE_FLAG_DODGE_BLOCK
                If TypeOf Victim Is CharacterObject Then
                    CType(Victim, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AURASTATE, Victim.AuraState)
                    CType(Victim, CharacterObject).SendCharacterUpdate()
                End If
            Case Is < chanceToMiss + chanceToDodge + chanceToParry
                'DONE: Parry attack
                result.Damage = 0
                result.victimState = AttackVictimState.VICTIMSTATE_PARRY
                DoEmote(Emotes.ONESHOT_PARRYUNARMED, Victim)
                'TODO: Remove after 5 secs?
                Victim.AuraState = Victim.AuraState Or SpellAuraStates.AURASTATE_FLAG_PARRY
                If TypeOf Victim Is CharacterObject Then
                    CType(Victim, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AURASTATE, Victim.AuraState)
                    CType(Victim, CharacterObject).SendCharacterUpdate()
                End If
            Case Is < chanceToMiss + chanceToDodge + chanceToParry + chanceToGlancingBlow
                'DONE: Glancing Blow
                result.Damage -= Fix(skillDiference * 0.03F * result.Damage)
                result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_HITANIMATION
                result.HitInfo = result.HitInfo Or AttackHitState.HIT_GLANCING_BLOW
            Case Is < chanceToMiss + chanceToDodge + chanceToParry + chanceToGlancingBlow + chanceToBlock
                'DONE: Block (http://www.wowwiki.com/Formulas:Block)
                If TypeOf Victim Is CharacterObject Then
                    result.Blocked = CType(Victim, CharacterObject).combatBlockValue + (CType(Victim, CharacterObject).Strength.Base / 20)     '... hits you for 60. (40 blocked)
                    If CType(Victim, CharacterObject).combatBlockValue <> 0 Then
                        DoEmote(Emotes.ONESHOT_PARRYSHIELD, Victim)
                    Else
                        DoEmote(Emotes.ONESHOT_PARRYUNARMED, Victim)
                    End If
                    result.victimState = AttackVictimState.VICTIMSTATE_BLOCKS
                End If
                result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_HITANIMATION
                If result.GetDamage <= 0 Then result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_BLOCK
            Case Is < chanceToMiss + chanceToDodge + chanceToParry + chanceToGlancingBlow + chanceToBlock + chanceToCrit
                'DONE: Critical hit attack
                result.Damage *= 2
                result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_HITANIMATION
                result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_CRITICALHIT
                DoEmote(Emotes.ONESHOT_WOUNDCRITICAL, Victim)
            Case Is < chanceToMiss + chanceToDodge + chanceToParry + chanceToGlancingBlow + chanceToBlock + chanceToCrit + chanceToCrushingBlow
                'DONE: Crushing Blow
                result.Damage = (result.Damage * 3) >> 1
                result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_HITANIMATION
                result.HitInfo = result.HitInfo Or AttackHitState.HIT_CRUSHING_BLOW
            Case Else
                'DONE: Normal hit
                result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_HITANIMATION
        End Select

        'DONE: Resist
        If result.GetDamage > 0 AndAlso result.DamageType > DamageTypes.DMG_PHYSICAL Then
            result.Resist = Victim.GetResist(Attacker, result.DamageType, result.Damage)
            If result.GetDamage <= 0 Then result.HitInfo = result.HitInfo Or AttackHitState.HIT_RESIST
        End If

        'DONE: Absorb
        If result.GetDamage > 0 Then
            result.Absorbed = Victim.GetAbsorb(result.DamageType, result.Damage)
            If result.GetDamage <= 0 Then result.HitInfo = result.HitInfo Or AttackHitState.HITINFO_ABSORB
        End If

        'TODO: Procs

#If DEBUG Then
        'Log.WriteLine(LogType.INFORMATION, "skillDiference = {0}", skillDiference)
        'Log.WriteLine(LogType.INFORMATION, "chanceToMiss = {0}", chanceToMiss)
        'Log.WriteLine(LogType.INFORMATION, "chanceToCrit = {0}", chanceToCrit)
        'Log.WriteLine(LogType.INFORMATION, "chanceToParry = {0}", chanceToParry)
        'Log.WriteLine(LogType.INFORMATION, "chanceToDodge = {0}", chanceToDodge)
        'Log.WriteLine(LogType.INFORMATION, "chanceToBlock = {0}", chanceToBlock)
        'Log.WriteLine(LogType.INFORMATION, "result.Damage = {0}", result.Damage)
        'Log.WriteLine(LogType.INFORMATION, "result.Blocked = {0}", result.Blocked)
        'Log.WriteLine(LogType.INFORMATION, "result.HitInfo = {0}", result.HitInfo)
        'Log.WriteLine(LogType.INFORMATION, "result.victimState = {0}", result.victimState)
#End If

        Return result
    End Function

    'Combat system calculations
    Public Function GetBasePercentDodge(ByRef objCharacter As BaseUnit, ByVal skillDiference As Integer) As Single
        'http://www.wowwiki.com/Formulas:Dodge

        If TypeOf objCharacter Is CharacterObject Then
            'DONE: Stunned target cannot dodge
            If (objCharacter.cUnitFlags And UnitFlags.UNIT_FLAG_STUNTED) Then Return 0

            If CType(objCharacter, CharacterObject).combatDodge > 0 Then
                Dim combatDodgeAgilityBonus As Integer = 0
                Select Case CType(objCharacter, CharacterObject).Classe
                    Case Classes.CLASS_HUNTER
                        combatDodgeAgilityBonus = Fix(CType(objCharacter, CharacterObject).Agility.Base / 26.5F)
                    Case Classes.CLASS_ROGUE
                        combatDodgeAgilityBonus = Fix(CType(objCharacter, CharacterObject).Agility.Base / 14.5F)
                    Case Classes.CLASS_MAGE, Classes.CLASS_PALADIN, Classes.CLASS_WARLOCK
                        combatDodgeAgilityBonus = Fix(CType(objCharacter, CharacterObject).Agility.Base / 19.5F)
                    Case Else
                        combatDodgeAgilityBonus = Fix(CType(objCharacter, CharacterObject).Agility.Base / 20)
                End Select

                Return CType(objCharacter, CharacterObject).combatDodge + combatDodgeAgilityBonus - skillDiference * 0.04F
            End If
        End If

        Return 0
    End Function

    Public Function GetBasePercentParry(ByRef objCharacter As BaseUnit, ByVal skillDiference As Integer) As Single
        'http://www.wowwiki.com/Formulas:Parry

        If TypeOf objCharacter Is CharacterObject Then
            'NOTE: Must have learned "Parry" spell, ID=3127
            If CType(objCharacter, CharacterObject).combatParry > 0 Then
                Return CType(objCharacter, CharacterObject).combatParry - skillDiference * 0.04F
            End If
        End If

        Return 0
    End Function

    Public Function GetBasePercentBlock(ByRef objCharacter As BaseUnit, ByVal skillDiference As Integer) As Single
        'http://www.wowwiki.com/Formulas:Block

        If TypeOf objCharacter Is CharacterObject Then
            'NOTE: Must have learned "Block" spell, ID=107
            If CType(objCharacter, CharacterObject).combatBlock > 0 Then
                Return CType(objCharacter, CharacterObject).combatBlock - skillDiference * 0.04F
            End If
        End If

        Return 0
    End Function

    Public Function GetBasePercentMiss(ByRef objCharacter As BaseUnit, ByVal skillDiference As Integer) As Single
        'http://www.wowwiki.com/Miss

        If TypeOf objCharacter Is CharacterObject Then
            With CType(objCharacter, CharacterObject)
                If .attackSheathState = SHEATHE_SLOT.SHEATHE_WEAPON Then

                    'NOTE: Character is with selected hand weapons
                    If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) Then
                        'NOTE: Character is with equiped offhand item, checking if it is weapon
                        If .Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_WEAPON Then
                            'DualWield Miss chance
                            If skillDiference > 10 Then
                                Return 19 + 5 - skillDiference * 0.1F
                            Else
                                Return 19 + 5 - skillDiference * 0.2F
                            End If
                        End If
                    End If

                    If skillDiference > 10 Then
                        Return 5 - skillDiference * 0.1F
                    Else
                        Return 5 - skillDiference * 0.2F
                    End If

                End If
            End With
        End If

        'Base Miss chance
        Return 5 - skillDiference * 0.04F
    End Function

    Public Function GetBasePercentCrit(ByRef objCharacter As BaseUnit, ByVal skillDiference As Integer) As Single
        '5% base critical chance

        If TypeOf objCharacter Is CharacterObject Then
            Dim baseCrit As Single = 0
            Select Case CType(objCharacter, CharacterObject).Classe
                Case Classes.CLASS_ROGUE
                    baseCrit = 0.0F + CType(objCharacter, CharacterObject).Agility.Base / 29
                Case Classes.CLASS_DRUID
                    baseCrit = 0.92F + CType(objCharacter, CharacterObject).Agility.Base / 20
                Case Classes.CLASS_HUNTER
                    baseCrit = 0.0F + CType(objCharacter, CharacterObject).Agility.Base / 33
                Case Classes.CLASS_MAGE
                    baseCrit = 3.2F + CType(objCharacter, CharacterObject).Agility.Base / 19.44
                Case Classes.CLASS_PALADIN
                    baseCrit = 0.7F + CType(objCharacter, CharacterObject).Agility.Base / 19.77
                Case Classes.CLASS_PRIEST
                    baseCrit = 3.0F + CType(objCharacter, CharacterObject).Agility.Base / 20
                Case Classes.CLASS_SHAMAN
                    baseCrit = 1.7F + CType(objCharacter, CharacterObject).Agility.Base / 19.7
                Case Classes.CLASS_WARLOCK
                    baseCrit = 2.0F + CType(objCharacter, CharacterObject).Agility.Base / 20
                Case Classes.CLASS_WARRIOR
                    baseCrit = 0.0F + CType(objCharacter, CharacterObject).Agility.Base / 20
            End Select

            Return baseCrit + CType(objCharacter, CharacterObject).combatCrit + skillDiference * 0.2F
        Else
            Return 5 + skillDiference * 0.2F
        End If
    End Function

    'Helper calculations
    Public Function GetDistance(ByVal Object1 As BaseObject, ByVal Object2 As BaseObject) As Single
        Return GetDistance(Object1.positionX, Object2.positionX, Object1.positionY, Object2.positionY, Object1.positionZ, Object2.positionZ)
    End Function

    Public Function GetDistance(ByVal Object1 As BaseObject, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single) As Single
        Return GetDistance(Object1.positionX, x2, Object1.positionY, y2, Object1.positionZ, z2)
    End Function

    Public Function GetDistance(ByVal x1 As Single, ByVal x2 As Single, ByVal y1 As Single, ByVal y2 As Single, ByVal z1 As Single, ByVal z2 As Single) As Single
        Return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2))
    End Function

    Public Function GetDistance(ByVal x1 As Single, ByVal x2 As Single, ByVal y1 As Single, ByVal y2 As Single) As Single
        Return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2))
    End Function

    Public Function GetOrientation(ByVal x1 As Single, ByVal x2 As Single, ByVal y1 As Single, ByVal y2 As Single) As Single
        Dim angle As Single = Math.Atan2(y2 - y1, x2 - x1)

        If angle < 0 Then
            angle = angle + 2 * Math.PI
        End If
        Return angle
    End Function

    Public Function IsInFrontOf(ByRef Object1 As BaseObject, ByRef Object2 As BaseObject) As Boolean
        Return IsInFrontOf(Object1, Object2.positionX, Object2.positionY)
    End Function

    Public Function IsInFrontOf(ByRef Object1 As BaseObject, ByVal x2 As Single, ByVal y2 As Single) As Boolean
        Dim angle2 As Single = GetOrientation(Object1.positionX, x2, Object1.positionY, y2)
        Dim lowAngle As Single = Object1.orientation - 1.04719758F
        Dim hiAngle As Single = Object1.orientation + 1.04719758F

        If lowAngle < 0 Then
            Return ((angle2 >= 2 * Math.PI + lowAngle And angle2 <= 2 * Math.PI) Or (angle2 >= 0 And angle2 <= hiAngle))
        End If
        Return (angle2 >= lowAngle) And (angle2 <= hiAngle)
    End Function

    Public Function IsInBackOf(ByRef Object1 As BaseObject, ByRef Object2 As BaseObject) As Boolean
        Return IsInBackOf(Object1, Object2.positionX, Object2.positionY)
    End Function

    Public Function IsInBackOf(ByRef Object1 As BaseObject, ByVal x2 As Single, ByVal y2 As Single) As Boolean
        Dim angle2 As Single = GetOrientation(x2, Object1.positionX, y2, Object1.positionY)
        Dim lowAngle As Single = Object1.orientation - 1.04719758F
        Dim hiAngle As Single = Object1.orientation + 1.04719758F

        If lowAngle < 0 Then
            Return ((angle2 >= 2 * Math.PI + lowAngle And angle2 <= 2 * Math.PI) Or (angle2 >= 0 And angle2 <= hiAngle))
        End If
        Return (angle2 >= lowAngle) And (angle2 <= hiAngle)
    End Function

    'Helper functions
    Public Function GetSkillWeapon(ByRef objCharacter As BaseUnit, ByVal DualWield As Boolean) As Integer
        If TypeOf objCharacter Is CharacterObject Then
            Dim tmpSkill As Integer
            With CType(objCharacter, CharacterObject)

                Select Case .attackSheathState
                    Case SHEATHE_SLOT.SHEATHE_NONE
                        tmpSkill = SKILL_IDs.SKILL_UNARMED
                    Case SHEATHE_SLOT.SHEATHE_WEAPON
                        If DualWield AndAlso .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) Then
                            tmpSkill = ITEMDatabase(.Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).ItemEntry).GetReqSkill
                        ElseIf .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) Then
                            tmpSkill = ITEMDatabase(.Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).ItemEntry).GetReqSkill
                        End If
                    Case SHEATHE_SLOT.SHEATHE_RANGED
                        If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) Then
                            tmpSkill = ITEMDatabase(.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemEntry).GetReqSkill
                        End If
                End Select

                If tmpSkill = 0 Then
                    Return objCharacter.Level * 5
                Else
                    .UpdateSkill(tmpSkill, 0.01F)
                    Return .Skills(tmpSkill).CurrentWithBonus
                End If

            End With
        End If

        Return objCharacter.Level * 5
    End Function

    Public Function GetSkillDefence(ByRef objCharacter As BaseUnit) As Integer
        If TypeOf objCharacter Is CharacterObject Then
            CType(objCharacter, CharacterObject).UpdateSkill(SKILL_IDs.SKILL_DEFENSE, 0.01)
            Return CType(objCharacter, CharacterObject).Skills(SKILL_IDs.SKILL_DEFENSE).CurrentWithBonus
        End If
        Return objCharacter.Level * 5
    End Function

    Public Function GetAttackTime(ByRef objCharacter As CharacterObject, ByRef combatDualWield As Boolean) As Integer
        Select Case objCharacter.attackSheathState
            Case SHEATHE_SLOT.SHEATHE_NONE
                Return objCharacter.AttackTime(0)
            Case SHEATHE_SLOT.SHEATHE_WEAPON
                If combatDualWield Then
                    If objCharacter.AttackTime(1) = 0 Then Return objCharacter.AttackTime(0)
                    Return objCharacter.AttackTime(1)
                Else
                    If objCharacter.AttackTime(0) = 0 Then Return objCharacter.AttackTime(1)
                    Return objCharacter.AttackTime(0)
                End If
            Case SHEATHE_SLOT.SHEATHE_RANGED
                Return objCharacter.AttackTime(2)
        End Select
    End Function

    Public Sub GetDamage(ByRef objCharacter As BaseUnit, ByVal DualWield As Boolean, ByRef result As DamageInfo)
        If TypeOf objCharacter Is CharacterObject Then
            With CType(objCharacter, CharacterObject)
                Select Case .attackSheathState
                    Case SHEATHE_SLOT.SHEATHE_NONE
                        result.HitInfo = AttackHitState.HITINFO_NORMALSWING
                        result.DamageType = DamageMasks.DMG_PHYSICAL
                        result.Damage = Rnd.Next(.BaseUnarmedDamage, .BaseUnarmedDamage + 1)
                    Case SHEATHE_SLOT.SHEATHE_WEAPON
                        If DualWield Then
                            result.HitInfo = AttackHitState.HITINFO_HITANIMATION + AttackHitState.HITINFO_LEFTSWING
                            result.DamageType = DamageMasks.DMG_PHYSICAL
                            result.Damage = Rnd.Next(.OffHandDamage.Minimum / 2, .OffHandDamage.Maximum / 2 + 1) + .BaseUnarmedDamage
                        Else
                            result.HitInfo = AttackHitState.HITINFO_HITANIMATION
                            result.DamageType = DamageMasks.DMG_PHYSICAL
                            result.Damage = Rnd.Next(.Damage.Minimum, .Damage.Maximum + 1) + .BaseUnarmedDamage
                        End If
                    Case SHEATHE_SLOT.SHEATHE_RANGED
                        result.HitInfo = AttackHitState.HITINFO_HITANIMATION + AttackHitState.HITINFO_RANGED
                        result.DamageType = DamageMasks.DMG_PHYSICAL
                        result.Damage = Rnd.Next(.RangedDamage.Minimum, .RangedDamage.Maximum + 1) + .BaseRangedDamage
                End Select
            End With

        Else
            With CType(objCharacter, CreatureObject)
                result.DamageType = DamageTypes.DMG_PHYSICAL
                result.Damage = Rnd.Next(CREATURESDatabase(.ID).Damage.Minimum, CREATURESDatabase(.ID).Damage.Maximum + 1) ' + (CType(CREATURESDatabase(.ID), CreatureInfo).AtackPower / 14 * (CType(CREATURESDatabase(.ID), CreatureInfo).BaseAttackTime / 1000))
            End With
        End If
    End Sub

#End Region

#Region "WS.Combat.Framework"

    Public Enum SwingTypes As Byte
        NOSWING = 0
        SINGLEHANDEDSWING = 1
        TWOHANDEDSWING = 2
    End Enum

    Public Enum AttackVictimState As Integer
        VICTIMSTATE_UNKNOWN1 = 0
        VICTIMSTATE_NORMAL = 1
        VICTIMSTATE_DODGE = 2
        VICTIMSTATE_PARRY = 3
        VICTIMSTATE_UNKNOWN2 = 4
        VICTIMSTATE_BLOCKS = 5
        VICTIMSTATE_EVADES = 6
        VICTIMSTATE_IS_IMMUNE = 7
        VICTIMSTATE_DEFLECTS = 8
    End Enum

    Public Enum AttackHitState As Integer

        HIT_UNARMED = HITINFO_NORMALSWING
        HIT_NORMAL = HITINFO_HITANIMATION
        HIT_NORMAL_OFFHAND = HITINFO_HITANIMATION + HITINFO_LEFTSWING
        HIT_MISS = HITINFO_MISS
        HIT_MISS_OFFHAND = HITINFO_MISS + HITINFO_LEFTSWING
        HIT_CRIT = HITINFO_CRITICALHIT
        HIT_CRIT_OFFHAND = HITINFO_CRITICALHIT + HITINFO_LEFTSWING
        HIT_RESIST = HITINFO_RESIST
        HIT_CRUSHING_BLOW = HITINFO_CRUSHING
        HIT_GLANCING_BLOW = HITINFO_GLANCING

        HITINFO_NORMALSWING = &H0
        HITINFO_UNK = &H1
        HITINFO_HITANIMATION = &H2
        HITINFO_LEFTSWING = &H4
        HITINFO_RANGED = &H8
        HITINFO_MISS = &H10
        HITINFO_ABSORB = &H20
        HITINFO_RESIST = &H40
        HITINFO_UNK2 = &H100
        HITINFO_CRITICALHIT = &H200
        HITINFO_BLOCK = &H800
        HITINFO_UNK3 = &H2000
        HITINFO_CRUSHING = &H8000
        HITINFO_GLANCING = &H10000
        HITINFO_NOACTION = &H10000
        HITINFO_SWINGNOHITSOUND = &H80000
    End Enum

    Structure DamageInfo
        Public Damage As Integer
        Public DamageType As DamageTypes
        Public Blocked As Integer
        Public Absorbed As Integer
        Public Resist As Integer
        Public victimState As AttackVictimState
        Public HitInfo As Integer
        Public Turn As Byte
        Public ReadOnly Property GetDamage() As Integer
            Get
                Return Damage - Absorbed - Blocked - Resist
            End Get
        End Property
    End Structure

    Public Class TAttackTimer
        Implements IDisposable

        'Internal
        Private LastAttack As Integer = 0
        Private NextAttackTimer As Timer = Nothing
        Public Victim As BaseUnit
        Public Character As CharacterObject
        Public combatReach As Single
        Public minRanged As Single
        Public combatDualWield As Boolean = False
        Public Ranged As Boolean = False

        Private TimeLeftMainHand As Integer = -1
        Private TimeLeftOffHand As Integer = -1

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                NextAttackTimer.Dispose()
                NextAttackTimer = Nothing

                combatNextAttack.Dispose()

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

        Public Sub New(ByRef Victim_ As BaseObject, ByRef Character_ As CharacterObject)
            NextAttackTimer = New Timer(AddressOf DoAttack, Nothing, 1000, Timeout.Infinite)
            Victim = Victim_
            Character = Character_
        End Sub

        Public Sub New(ByRef Character_ As CharacterObject)
            NextAttackTimer = New Timer(AddressOf DoAttack, Nothing, Timeout.Infinite, Timeout.Infinite)
            Character = Character_
            Victim = Nothing
        End Sub

        'Packets
        Public Sub AttackStop()
            If Character.AutoShotSpell > 0 Then Exit Sub
            NextAttackTimer.Change(Timeout.Infinite, Timeout.Infinite)
            Victim = Nothing
            Ranged = False
        End Sub

        Public Sub AttackStart(Optional ByVal Victim_ As BaseUnit = Nothing)
            If Victim Is Nothing Then
                Victim = Victim_
                combatReach = BaseUnit.CombatReach_Base + Victim.BoundingRadius + Character.CombatReach
                minRanged = Victim.BoundingRadius + 8.0F
            ElseIf Victim.GUID = Victim_.GUID Then
                'DONE: If it's the same target we do nothing
                Exit Sub
            Else
                SendAttackStop(Character.GUID, Victim.GUID, Character.client)
                Victim = Victim_
                combatReach = BaseUnit.CombatReach_Base + Victim.BoundingRadius + Character.CombatReach
                minRanged = Victim.BoundingRadius + 8.0F
            End If

            Dim AttackSpeed As Integer = GetAttackTime(Character, False)
            If (timeGetTime("") - LastAttack) >= AttackSpeed Then
                DoAttack(Nothing)
            Else
                NextAttackTimer.Change((timeGetTime("") - LastAttack), Timeout.Infinite)
            End If
        End Sub

        Public Sub DoAttack(ByVal Status As Object)
            'DONE: Stop attacking when there's no victim
            If Victim Is Nothing Then
                Character.AutoShotSpell = 0
                AttackStop()
                Exit Sub
            End If

            LastAttack = timeGetTime("")
            Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_START_ATTACK)
            Try
                If Ranged Then
                    DoRangedAttack(False)
                Else
                    DoMeleeAttack(False)
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "Error doing attack.{0}{1}", vbNewLine, ex.ToString)
            End Try
        End Sub

        Public Sub DoMeleeAttack(ByVal Status As Object)
            If Victim Is Nothing Then
                Dim SMSG_ATTACKSWING_CANT_ATTACK As New PacketClass(OPCODES.SMSG_ATTACKSWING_CANT_ATTACK)
                Character.client.Send(SMSG_ATTACKSWING_CANT_ATTACK)
                SMSG_ATTACKSWING_CANT_ATTACK.Dispose()

                Character.AutoShotSpell = 0
                AttackStop()
                Exit Sub
            End If

            Try
                'DONE: If casting spell exit
                If Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL) IsNot Nothing AndAlso Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL).Finished = False Then
                    Log.WriteLine(LogType.DEBUG, "AttackStop: Casting Spell")
                    'AttackStop()
                    NextAttackTimer.Change(GetAttackTime(Character, False), Timeout.Infinite)
                    Exit Sub
                End If

                If Victim.isDead Then
                    Dim SMSG_ATTACKSWING_DEADTARGET As New PacketClass(OPCODES.SMSG_ATTACKSWING_DEADTARGET)
                    Character.client.Send(SMSG_ATTACKSWING_DEADTARGET)
                    SMSG_ATTACKSWING_DEADTARGET.Dispose()

                    Character.AutoShotSpell = 0
                    AttackStop()
                    Exit Sub
                End If

                If Character.isDead Then
                    Dim SMSG_ATTACKSWING_DEADTARGET As New PacketClass(OPCODES.SMSG_ATTACKSWING_DEADTARGET)
                    Character.client.Send(SMSG_ATTACKSWING_DEADTARGET)
                    SMSG_ATTACKSWING_DEADTARGET.Dispose()

                    Character.AutoShotSpell = 0
                    AttackStop()
                    Exit Sub
                End If

                If Character.StandState > 0 Then
                    Dim SMSG_ATTACKSWING_NOTSTANDING As New PacketClass(OPCODES.SMSG_ATTACKSWING_NOTSTANDING)
                    Character.client.Send(SMSG_ATTACKSWING_NOTSTANDING)
                    SMSG_ATTACKSWING_NOTSTANDING.Dispose()

                    Character.AutoShotSpell = 0
                    AttackStop()
                    Exit Sub
                End If

                'DONE: Decide it's real position
                If TypeOf Victim Is CreatureObject Then CType(Victim, CreatureObject).SetToRealPosition()
                Dim tmpPosX As Single = Victim.positionX
                Dim tmpPosY As Single = Victim.positionY
                Dim tmpPosZ As Single = Victim.positionZ

                Dim tmpDist As Single = GetDistance(Character, tmpPosX, tmpPosY, tmpPosZ)
                If tmpDist > (8 + Victim.CombatReach) Then
                    'DONE: Use ranged if you're too far away for melee
                    If Character.CanShootRanged Then
                        Ranged = True
                        DoRangedAttack(Nothing)
                        Exit Sub
                    Else
                        NextAttackTimer.Change(2000, Timeout.Infinite)
                        Dim SMSG_ATTACKSWING_NOTINRANGE As New PacketClass(OPCODES.SMSG_ATTACKSWING_NOTINRANGE)
                        Character.client.Send(SMSG_ATTACKSWING_NOTINRANGE)
                        SMSG_ATTACKSWING_NOTINRANGE.Dispose()
                        Exit Sub
                    End If
                ElseIf tmpDist > combatReach Then
                    NextAttackTimer.Change(2000, Timeout.Infinite)
                    Dim SMSG_ATTACKSWING_NOTINRANGE As New PacketClass(OPCODES.SMSG_ATTACKSWING_NOTINRANGE)
                    Character.client.Send(SMSG_ATTACKSWING_NOTINRANGE)
                    SMSG_ATTACKSWING_NOTINRANGE.Dispose()
                    Exit Sub
                End If

                If Not IsInFrontOf(Character, tmpPosX, tmpPosY) Then
                    NextAttackTimer.Change(2000, Timeout.Infinite)
                    Dim SMSG_ATTACKSWING_BADFACING As New PacketClass(OPCODES.SMSG_ATTACKSWING_BADFACING)
                    Character.client.Send(SMSG_ATTACKSWING_BADFACING)
                    SMSG_ATTACKSWING_BADFACING.Dispose()
                    Exit Sub
                End If

                Dim HaveMainHand As Boolean = (Character.AttackTime(0) > 0 AndAlso Character.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND))
                Dim HaveOffHand As Boolean = (Character.AttackTime(1) > 0 AndAlso Character.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND))

                'DONE: Spells that add to attack
                If Not combatNextAttackSpell Then
                    DoMeleeDamage()
                Else
                    combatNextAttack.Set()
                    combatNextAttack.Set()
                    combatNextAttackSpell = False
                End If

                'DONE: Calculate next attack
                Dim NextAttack As Integer = GetAttackTime(Character, combatDualWield)
                If HaveMainHand AndAlso HaveOffHand Then 'If we are dualwielding
                    '' Character.CommandResponse("You are dualwielding!")

                    If combatDualWield Then
                        If TimeLeftMainHand = -1 Then TimeLeftMainHand = Character.AttackTime(1) / 2
                        TimeLeftOffHand = Character.AttackTime(1)
                    Else
                        If TimeLeftMainHand = -1 Then TimeLeftOffHand = Character.AttackTime(0) / 2
                        TimeLeftMainHand = Character.AttackTime(0)
                    End If

                    If TimeLeftMainHand < TimeLeftOffHand Then
                        NextAttack = TimeLeftMainHand
                        combatDualWield = False
                    Else
                        NextAttack = TimeLeftOffHand
                        combatDualWield = True
                    End If
                    TimeLeftMainHand -= NextAttack
                    TimeLeftOffHand -= NextAttack
                    Character.CommandResponse("NO: " & TimeLeftOffHand)
                    Character.CommandResponse("NM: " & TimeLeftMainHand)
                Else
                    '' Character.CommandResponse("You're not dualwielding!")
                    TimeLeftMainHand = -1
                    combatDualWield = HaveOffHand
                End If

                'DONE: Enqueue next attack
                NextAttackTimer.Change(NextAttack, Timeout.Infinite)

            Catch e As Exception
                If (Not Character Is Nothing) AndAlso (Not Character.client Is Nothing) Then
                    Dim SMSG_ATTACKSWING_CANT_ATTACK As New PacketClass(OPCODES.SMSG_ATTACKSWING_CANT_ATTACK)
                    Character.client.Send(SMSG_ATTACKSWING_CANT_ATTACK)
                    SMSG_ATTACKSWING_CANT_ATTACK.Dispose()
                End If
                AttackStop()
                Log.WriteLine(LogType.DEBUG, "Error while doing melee attack.{0}", vbNewLine & e.ToString)
            End Try
        End Sub

        Public Sub DoRangedAttack(ByVal Status As Object)
            'DONE: Decide it's real position
            If TypeOf Victim Is CreatureObject Then CType(Victim, CreatureObject).SetToRealPosition()
            Dim tmpPosX As Single = Victim.positionX
            Dim tmpPosY As Single = Victim.positionY
            Dim tmpPosZ As Single = Victim.positionZ

            If Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL) IsNot Nothing AndAlso Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL).Finished = False Then
                Log.WriteLine(LogType.DEBUG, "AttackPause: Casting Spell")
                NextAttackTimer.Change(GetAttackTime(Character, False), Timeout.Infinite)
                Exit Sub
            End If

            If Victim.Life.Current = 0 Then
                Dim SMSG_ATTACKSWING_DEADTARGET As New PacketClass(OPCODES.SMSG_ATTACKSWING_DEADTARGET)
                Character.client.Send(SMSG_ATTACKSWING_DEADTARGET)
                SMSG_ATTACKSWING_DEADTARGET.Dispose()

                Character.AutoShotSpell = 0
                AttackStop()
                Exit Sub
            End If

            If Character.DEAD Then
                Dim SMSG_ATTACKSWING_DEADTARGET As New PacketClass(OPCODES.SMSG_ATTACKSWING_DEADTARGET)
                Character.client.Send(SMSG_ATTACKSWING_DEADTARGET)
                SMSG_ATTACKSWING_DEADTARGET.Dispose()

                Character.AutoShotSpell = 0
                AttackStop()
                Exit Sub
            End If

            If Character.StandState > 0 Then
                Dim SMSG_ATTACKSWING_NOTSTANDING As New PacketClass(OPCODES.SMSG_ATTACKSWING_NOTSTANDING)
                Character.client.Send(SMSG_ATTACKSWING_NOTSTANDING)
                SMSG_ATTACKSWING_NOTSTANDING.Dispose()

                Character.AutoShotSpell = 0
                AttackStop()
                Exit Sub
            End If

            'DONE: Change to melee if we're too close for ranged
            Dim tmpDist As Single = GetDistance(Character, tmpPosX, tmpPosY, tmpPosZ)
            If tmpDist < combatReach Then
                Ranged = False

                DoMeleeAttack(Nothing)
                Exit Sub
            ElseIf tmpDist < minRanged Then
                NextAttackTimer.Change(2000, Timeout.Infinite)
                Dim SMSG_ATTACKSWING_NOTINRANGE As New PacketClass(OPCODES.SMSG_ATTACKSWING_NOTINRANGE)
                Character.client.Send(SMSG_ATTACKSWING_NOTINRANGE)
                SMSG_ATTACKSWING_NOTINRANGE.Dispose()
                Exit Sub
            End If

            If Not IsInFrontOf(Character, tmpPosX, tmpPosY) Then
                NextAttackTimer.Change(2000, Timeout.Infinite)
                Dim SMSG_ATTACKSWING_BADFACING As New PacketClass(OPCODES.SMSG_ATTACKSWING_BADFACING)
                Character.client.Send(SMSG_ATTACKSWING_BADFACING)
                SMSG_ATTACKSWING_BADFACING.Dispose()
                Exit Sub
            End If

            DoRangedDamage()

            'DONE: Enqueue next attack
            NextAttackTimer.Change(GetAttackTime(Character, False), Timeout.Infinite)
        End Sub

        Public Sub DoMeleeDamage()

            Dim damageInfo As DamageInfo = CalculateDamage(Character, Victim, combatDualWield, False)
            SendAttackerStateUpdate(Character, Victim, damageInfo, Character.client)

            'TODO: If the victim has a spelltrigger on melee attacks
            Dim Target As New SpellTargets
            Target.SetTarget_UNIT(Character)
            For i As Byte = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If Victim.ActiveSpells(i) IsNot Nothing AndAlso (Victim.ActiveSpells(i).GetSpellInfo.procFlags And ProcFlags.PROC_FLAG_HIT_MELEE) Then
                    For j As Byte = 0 To 2
                        If Victim.ActiveSpells(i).Aura_Info(j) IsNot Nothing AndAlso Victim.ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraEffects_Names.SPELL_AURA_PROC_TRIGGER_SPELL Then
                            If RollChance(Victim.ActiveSpells(i).GetSpellInfo.procChance) Then
                                Dim castParams As New CastSpellParameters(Target, Victim, Victim.ActiveSpells(i).Aura_Info(j).TriggerSpell, True)
                                castParams.Cast(Nothing)
                            End If
                        End If
                    Next
                End If
            Next

            'DONE: Rage generation
            'http://www.wowwiki.com/Formulas:Rage_generation
            If Character.Classe = Classes.CLASS_WARRIOR OrElse (Character.Classe = Classes.CLASS_DRUID AndAlso (Character.ShapeshiftForm = ShapeshiftForm.FORM_BEAR OrElse Character.ShapeshiftForm = ShapeshiftForm.FORM_DIREBEAR)) Then
                Character.Rage.Increment(Fix((7.5 * damageInfo.Damage / Character.GetRageConversion + Character.GetHitFactor((damageInfo.HitInfo And AttackHitState.HITINFO_LEFTSWING) = 0, (damageInfo.HitInfo And AttackHitState.HITINFO_CRITICALHIT)) * GetAttackTime(Character, combatDualWield)) / 2))
                Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + ManaTypes.TYPE_RAGE, Character.Rage.Current)
                Character.SendCharacterUpdate(True)
            End If

            Victim.DealDamage(damageInfo.GetDamage, Character)
            If Victim Is Nothing OrElse Victim.isDead Then AttackStop()
        End Sub

        Public Sub DoRangedDamage()
            Dim Targets As New SpellTargets
            Targets.SetTarget_UNIT(Victim)

            Dim SpellID As Integer
            If Character.AutoShotSpell > 0 Then
                SpellID = Character.AutoShotSpell
            Else
                SpellID = 75
            End If

            Dim tmpSpell As New CastSpellParameters(Targets, Character, SpellID, True)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf tmpSpell.Cast))
        End Sub

        'Spells
        Public Sub DoMeleeDamageBySpell(ByRef Character As CharacterObject, ByRef Victim2 As BaseObject, ByVal BonusDamage As Integer, ByVal SpellID As Integer)

            Dim damageInfo As DamageInfo = CalculateDamage(Character, Victim2, False, False, SPELLs(SpellID))
            Dim IsCrit As Boolean = False

            If damageInfo.Damage > 0 Then damageInfo.Damage += BonusDamage
            If damageInfo.HitInfo = AttackHitState.HIT_CRIT Then
                damageInfo.Damage += BonusDamage
                IsCrit = True
            End If

            SendNonMeleeDamageLog(Character, Victim2, SpellID, damageInfo.DamageType, damageInfo.Damage, 0, damageInfo.Absorbed, IsCrit)

            If TypeOf Victim2 Is CreatureObject Then
                CType(Victim2, CreatureObject).DealDamage(damageInfo.GetDamage, Character)
                If Victim2 Is Victim AndAlso CType(Victim, CreatureObject).isDead Then
                    AttackStop()
                End If
            ElseIf TypeOf Victim2 Is CharacterObject Then
                CType(Victim2, CharacterObject).DealDamage(damageInfo.GetDamage, Character)

                If CType(Victim2, CharacterObject).Classe = Classes.CLASS_WARRIOR Then
                    CType(Victim2, CharacterObject).Rage.Increment(Fix((damageInfo.Damage / (CType(Victim2, CharacterObject).Level * 4)) * 25 + 10))
                    CType(Victim2, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + ManaTypes.TYPE_RAGE, CType(Victim2, CharacterObject).Rage.Current)
                    Character.SendCharacterUpdate(True)
                End If
            End If

            'DONE: Rage generation
            'http://www.wowwiki.com/Formulas:Rage_generation
            If Character.Classe = Classes.CLASS_WARRIOR OrElse (Character.Classe = Classes.CLASS_DRUID AndAlso (Character.ShapeshiftForm = ShapeshiftForm.FORM_BEAR OrElse Character.ShapeshiftForm = ShapeshiftForm.FORM_DIREBEAR)) Then
                Character.Rage.Increment(Fix((damageInfo.Damage / (Character.Level * 4)) * 75 + 10))
                Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + ManaTypes.TYPE_RAGE, Character.Rage.Current)
                Character.SendCharacterUpdate(True)
            End If
        End Sub

        Public combatNextAttack As New AutoResetEvent(False)
        Public combatNextAttackSpell As Boolean = False

    End Class

    Public Sub SetPlayerInCombat(ByRef objCharacter As CharacterObject)
        objCharacter.cUnitFlags = objCharacter.cUnitFlags Or UnitFlags.UNIT_FLAG_IN_COMBAT
        objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, objCharacter.cUnitFlags)
        objCharacter.SendCharacterUpdate(False)

        objCharacter.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_ENTER_COMBAT)
    End Sub

    Public Sub SetPlayerOutOfCombat(ByRef objCharacter As CharacterObject)
        objCharacter.cUnitFlags = objCharacter.cUnitFlags And (Not UnitFlags.UNIT_FLAG_IN_COMBAT)
        objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, objCharacter.cUnitFlags)
        objCharacter.SendCharacterUpdate(False)
    End Sub

#End Region

#Region "WS.Combat.Handlers"

    Public Sub On_CMSG_SET_SELECTION(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        client.Character.TargetGUID = packet.GetUInt64
        client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_TARGET, client.Character.TargetGUID)
        client.Character.SendCharacterUpdate()
    End Sub

    Public Sub On_CMSG_ATTACKSWING(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ATTACKSWING [GUID={2:X}]", client.IP, client.Port, GUID)

        If client.Character.Spell_Pacifyed Then
            Dim SMSG_ATTACKSWING_CANT_ATTACK As New PacketClass(OPCODES.SMSG_ATTACKSWING_CANT_ATTACK)
            client.Send(SMSG_ATTACKSWING_CANT_ATTACK)
            SMSG_ATTACKSWING_CANT_ATTACK.Dispose()
            SendAttackStop(client.Character.GUID, GUID, client)
            Exit Sub
        End If

        If GuidIsCreature(GUID) Then
            client.Character.attackState.AttackStart(WORLD_CREATUREs(GUID))
        ElseIf GuidIsPlayer(GUID) Then
            client.Character.attackState.AttackStart(CHARACTERs(GUID))
        Else
            Dim SMSG_ATTACKSWING_CANT_ATTACK As New PacketClass(OPCODES.SMSG_ATTACKSWING_CANT_ATTACK)
            client.Send(SMSG_ATTACKSWING_CANT_ATTACK)
            SMSG_ATTACKSWING_CANT_ATTACK.Dispose()
            SendAttackStop(client.Character.GUID, GUID, client)
        End If
    End Sub

    Public Sub On_CMSG_ATTACKSTOP(ByRef packet As PacketClass, ByRef client As ClientClass)
        Try
            packet.GetInt16()
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ATTACKSTOP", client.IP, client.Port)

            SendAttackStop(client.Character.GUID, client.Character.TargetGUID, client)
            client.Character.attackState.AttackStop()
        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Error stopping attack: {0}", e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_SET_AMMO(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim AmmoID As Integer = packet.GetInt32
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_AMMO [{2}]", client.IP, client.Port, AmmoID)

        If client.Character.isDead Then
            SendInventoryChangeFailure(client.Character, InventoryChangeFailure.EQUIP_ERR_YOU_ARE_DEAD, 0, 0)
            Exit Sub
        End If

        If AmmoID Then 'Set Ammo
            client.Character.AmmoID = AmmoID
            If ITEMDatabase.ContainsKey(AmmoID) = False Then
                'TODO: Another one of these useless bits of code, needs to be implemented correctly
                Dim tmpItem As ItemInfo = New ItemInfo(AmmoID)
            End If
            Dim CanUse As InventoryChangeFailure = CanUseAmmo(client.Character, AmmoID)
            If CanUse <> InventoryChangeFailure.EQUIP_ERR_OK Then
                SendInventoryChangeFailure(client.Character, CanUse, 0, 0)
                Exit Sub
            End If
            Dim currentDPS As Single = 0
            If ITEMDatabase.ContainsKey(AmmoID) = True AndAlso ITEMDatabase(AmmoID).ObjectClass = ITEM_CLASS.ITEM_CLASS_PROJECTILE OrElse CheckAmmoCompatibility(client.Character, AmmoID) Then
                currentDPS = ITEMDatabase(AmmoID).Damage(0).Minimum
            End If
            If client.Character.AmmoDPS <> currentDPS Then
                client.Character.AmmoDPS = currentDPS
                CalculateMinMaxDamage(client.Character, WeaponAttackType.RANGED_ATTACK)
            End If

            client.Character.AmmoID = AmmoID
            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_AMMO_ID, client.Character.AmmoID)
            client.Character.SendCharacterUpdate(False)

        Else 'Remove Ammo
            If client.Character.AmmoID Then
                client.Character.AmmoDPS = 0
                CalculateMinMaxDamage(client.Character, WeaponAttackType.RANGED_ATTACK)

                client.Character.AmmoID = 0
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_AMMO_ID, 0)
                client.Character.SendCharacterUpdate(False)
            End If
        End If
    End Sub

    Public Sub On_CMSG_SETSHEATHED(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim sheathed As SHEATHE_SLOT = packet.GetInt32
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SETSHEATHED [{2}]", client.IP, client.Port, sheathed)

        SetSheath(client.Character, sheathed)
    End Sub

    Public Sub SetSheath(ByRef objCharacter As CharacterObject, ByVal State As SHEATHE_SLOT)
        objCharacter.attackSheathState = State
        objCharacter.combatCanDualWield = False

        objCharacter.cBytes2 = ((objCharacter.cBytes2 And (Not &HFF)) Or State)
        objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_2, objCharacter.cBytes2)

        Select Case State
            Case SHEATHE_SLOT.SHEATHE_NONE
                SetVirtualItemInfo(objCharacter, 0, Nothing)
                SetVirtualItemInfo(objCharacter, 1, Nothing)
                SetVirtualItemInfo(objCharacter, 2, Nothing)

            Case SHEATHE_SLOT.SHEATHE_WEAPON
                If objCharacter.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) AndAlso (Not objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND).IsBroken) Then
                    SetVirtualItemInfo(objCharacter, 0, objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND))
                Else
                    SetVirtualItemInfo(objCharacter, 0, Nothing)
                    objCharacter.attackSheathState = SHEATHE_SLOT.SHEATHE_NONE
                End If
                If objCharacter.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) AndAlso (Not objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).IsBroken) Then
                    SetVirtualItemInfo(objCharacter, 1, objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND))
                    'DONE: Must be applyed SPELL_EFFECT_DUAL_WIELD and weapon in offhand
                    Log.WriteLine(LogType.DEBUG, "spellCanDualWeild = {0}", objCharacter.spellCanDualWeild)
                    Log.WriteLine(LogType.DEBUG, "objectClass = {0}", objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).ItemInfo.ObjectClass)
                    If objCharacter.spellCanDualWeild AndAlso objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND).ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_WEAPON Then objCharacter.combatCanDualWield = True
                Else
                    SetVirtualItemInfo(objCharacter, 1, Nothing)
                End If
                SetVirtualItemInfo(objCharacter, 2, Nothing)

            Case SHEATHE_SLOT.SHEATHE_RANGED
                SetVirtualItemInfo(objCharacter, 0, Nothing)
                SetVirtualItemInfo(objCharacter, 1, Nothing)
                If objCharacter.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) AndAlso (Not objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).IsBroken) Then
                    SetVirtualItemInfo(objCharacter, 2, objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED))
                Else
                    SetVirtualItemInfo(objCharacter, 2, Nothing)
                    objCharacter.attackSheathState = SHEATHE_SLOT.SHEATHE_NONE
                End If

            Case Else
                Log.WriteLine(LogType.WARNING, "Unhandled sheathe state [{0}]", State)
                SetVirtualItemInfo(objCharacter, 0, Nothing)
                SetVirtualItemInfo(objCharacter, 1, Nothing)
                SetVirtualItemInfo(objCharacter, 2, Nothing)
        End Select

        objCharacter.SendCharacterUpdate(True)
    End Sub

    Public Sub SetVirtualItemInfo(ByVal objChar As CharacterObject, ByVal Slot As Byte, ByRef Item As ItemObject)
        If Slot > 2 Then Exit Sub
        If Item Is Nothing Then
            'c.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + Slot * 2, 0)
            'c.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + Slot * 2 + 1, 0)
            'c.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY + Slot, 0)
        Else
            'c.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + Slot * 2, CType(Item.GUID << 32UI >> 32UI, UInteger))
            'c.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + Slot * 2 + 1, Item.ItemInfo.Sheath)
            'c.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY + Slot, Item.ItemInfo.Model)
        End If
    End Sub

    Public Sub SendAttackStop(ByVal attackerGUID As ULong, ByVal victimGUID As ULong, ByRef client As ClientClass)
        'AttackerGUID stopped attacking victimGUID
        Dim SMSG_ATTACKSTOP As New PacketClass(OPCODES.SMSG_ATTACKSTOP)
        SMSG_ATTACKSTOP.AddPackGUID(attackerGUID)
        SMSG_ATTACKSTOP.AddPackGUID(victimGUID)
        SMSG_ATTACKSTOP.AddInt32(0)
        SMSG_ATTACKSTOP.AddInt8(0)
        client.Character.SendToNearPlayers(SMSG_ATTACKSTOP)
        SMSG_ATTACKSTOP.Dispose()
    End Sub

    Public Sub SendAttackStart(ByVal attackerGUID As ULong, ByVal victimGUID As ULong, Optional ByRef client As ClientClass = Nothing)
        Dim SMSG_ATTACKSTART As New PacketClass(OPCODES.SMSG_ATTACKSTART)
        SMSG_ATTACKSTART.AddUInt64(attackerGUID)
        SMSG_ATTACKSTART.AddUInt64(victimGUID)

        client.Character.SendToNearPlayers(SMSG_ATTACKSTART)

        SMSG_ATTACKSTART.Dispose()
    End Sub

    Public Sub SendAttackerStateUpdate(ByRef Attacker As BaseObject, ByRef Victim As BaseObject, ByVal damageInfo As DamageInfo, Optional ByRef client As ClientClass = Nothing)
        Dim packet As New PacketClass(OPCODES.SMSG_ATTACKERSTATEUPDATE)
        packet.AddInt32(damageInfo.HitInfo)
        packet.AddPackGUID(Attacker.GUID)
        packet.AddPackGUID(Victim.GUID)
        packet.AddInt32(damageInfo.GetDamage)                               'RealDamage

        'TODO: How do we know what type of swing it is?
        packet.AddInt8(SwingTypes.SINGLEHANDEDSWING)                        'Swing type
        packet.AddUInt32(damageInfo.DamageType) 'Damage type

        packet.AddSingle(damageInfo.GetDamage)                                 'Damage float
        packet.AddInt32(damageInfo.GetDamage)                                  'Damage amount
        packet.AddInt32(damageInfo.Absorbed)                            'Damage absorbed
        packet.AddInt32(damageInfo.Resist)                              'Damage resisted
        packet.AddInt32(damageInfo.victimState)                              'Victim state
        If damageInfo.Absorbed = 0 Then
            packet.AddInt32(&H3E8)
        Else
            packet.AddInt32(-1)
        End If
        packet.AddInt32(0)
        packet.AddInt32(damageInfo.Blocked)                                 'Damage amount blocked

        If client IsNot Nothing Then
            client.Character.SendToNearPlayers(packet)
        Else
            Attacker.SendToNearPlayers(packet)
        End If

        packet.Dispose()
    End Sub

#End Region

End Module