' 
' Copyright (C) 2011 SpuriousZero <http://www.spuriousemu.com/>
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

Imports System.Threading
Imports System.Runtime.CompilerServices
Imports SpuriousZero.Common

Public Module WS_Creatures_AI

#Region "WS.Creatures.AI.Framework"
    Public Class TBaseAI
        Implements IDisposable
        Public Enum AIState
            AI_DO_NOTHING
            AI_DEAD
            AI_MOVING_TO_SPAWN
            AI_ATTACKING
            AI_MOVE_FOR_ATTACK
            AI_MOVING
            AI_WANDERING
            AI_RESPAWN
        End Enum

        Public State As AIState = AIState.AI_DO_NOTHING
        Public aiTarget As BaseUnit = Nothing
        Public aiHateTable As New Dictionary(Of BaseUnit, Integer)
        Public aiHateTableRemove As New List(Of BaseUnit)

        Public Overridable Function InCombat() As Boolean
            Return (aiHateTable.Count > 0)
        End Function
        Public Sub ResetThreatTable()
            Dim tmpUnits As New List(Of BaseUnit)
            For Each Victim As KeyValuePair(Of BaseUnit, Integer) In aiHateTable
                tmpUnits.Add(Victim.Key)
            Next
            aiHateTable.Clear()
            For Each Victim As BaseUnit In tmpUnits
                aiHateTable.Add(Victim, 0)
            Next
        End Sub
        Public Overridable Function IsMoving() As Boolean
            Select Case Me.State
                Case AIState.AI_ATTACKING, AIState.AI_MOVE_FOR_ATTACK, AIState.AI_MOVING, AIState.AI_WANDERING, AIState.AI_MOVING_TO_SPAWN
                    Return True
                Case Else
                    Return False
            End Select
        End Function
        Public Overridable Function IsRunning() As Boolean
            Return Me.State = AIState.AI_MOVE_FOR_ATTACK
        End Function
        Public Overridable Sub Reset()
            State = AIState.AI_DO_NOTHING
        End Sub
        Public Overridable Sub Pause(ByVal Time As Integer)
        End Sub
        Public Overridable Sub OnEnterCombat()
        End Sub
        Public Overridable Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
        End Sub
        Public Overridable Sub OnGenerateHate(ByRef Attacker As BaseUnit, ByVal HateValue As Integer)
        End Sub
        Public Overridable Sub OnKill(ByRef Victim As BaseUnit)
        End Sub
        Public Overridable Sub OnHealthChange(ByVal Percent As Integer)
        End Sub
        Public Overridable Sub OnDeath()
        End Sub

        Public Overridable Sub DoThink()
        End Sub
        Public Overridable Sub DoMove()
        End Sub

        Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
        End Sub
        Public Sub New()
        End Sub
    End Class


#End Region

#Region "WS.Creatures.AI.StandardAIs"

    'TODO: Base Escort AI for escort units

#Region "WS.Creatures.AI.DefaultAI"
    Public Class DefaultAI
        Inherits TBaseAI

        Protected aiCreature As CreatureObject = Nothing
        Protected aiTimer As Integer = 0
        Protected nextAttack As Integer = 0
        Protected ignoreLoot As Boolean = False

        Protected AllowedAttack As Boolean = True
        Protected AllowedMove As Boolean = True
        Protected IsWaypoint As Boolean = False

        Protected ResetX As Single = 0.0F
        Protected ResetY As Single = 0.0F
        Protected ResetZ As Single = 0.0F
        Protected ResetO As Single = 0.0F
        Protected ResetRun As Boolean = True
        Protected ResetFinished As Boolean = False

        Protected LastHitX As Single = 0.0F
        Protected LastHitY As Single = 0.0F
        Protected LastHitZ As Single = 0.0F

        Protected Const AI_INTERVAL_MOVE As Integer = 3000
        Protected Const AI_INTERVAL_SLEEP As Integer = 6000
        Protected Const AI_INTERVAL_DEAD As Integer = 60000
        Protected Const PIx2 As Single = 2 * Math.PI

        Public Sub New(ByRef Creature As CreatureObject)
            State = AIState.AI_WANDERING

            aiCreature = Creature
            aiTarget = Nothing
        End Sub
        Public Overrides Function IsMoving() As Boolean
            If (timeGetTime - aiCreature.LastMove) < aiTimer Then
                Select Case Me.State
                    Case AIState.AI_MOVE_FOR_ATTACK
                        Return True
                    Case AIState.AI_MOVING
                        Return True
                    Case AIState.AI_WANDERING
                        Return True
                    Case Else
                        Return False
                End Select
            Else
                Return False
            End If
        End Function
        Public Overrides Sub Pause(ByVal Time As Integer)
            aiTimer = Time
        End Sub
        Public Overrides Sub OnEnterCombat()
            If aiCreature.isDead Then Exit Sub 'Prevents the creature from doing this below if it's dead already
            'DONE: Decide it's real position if it hasn't stopped
            aiCreature.SetToRealPosition()

            ResetX = aiCreature.positionX
            ResetY = aiCreature.positionY
            ResetZ = aiCreature.positionZ
            ResetO = aiCreature.orientation

            Me.State = AIState.AI_ATTACKING
            DoThink()

            If MonsterSayCombat.ContainsKey(aiCreature.ID) Then
                Dim Chance As Integer = (MonsterSayCombat(aiCreature.ID).Chance)
                If Rnd.Next(1, 101) <= Chance Then
                    Dim TargetGUID As ULong = 0UL
                    If Not aiTarget Is Nothing Then TargetGUID = aiTarget.GUID
                    aiCreature.SendChatMessage(SelectMonsterSay(aiCreature.ID), ChatMsg.CHAT_MSG_MONSTER_SAY, LANGUAGES.LANG_UNIVERSAL, TargetGUID)
                End If
            End If
        End Sub
        Public Overrides Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
            'DONE: Remove combat flag from everyone
            For Each Victim As KeyValuePair(Of BaseUnit, Integer) In aiHateTable
                If TypeOf Victim.Key Is CharacterObject Then
                    CType(Victim.Key, CharacterObject).RemoveFromCombat(aiCreature)
                End If
            Next

            If aiCreature.DestroyAtNoCombat Then
                aiCreature.Destroy()
                Exit Sub
            End If

            aiTarget = Nothing
            aiHateTable.Clear()
            aiHateTableRemove.Clear()
            aiCreature.SendTargetUpdate(0UL)

            If Reset Then
                'DONE: Reset values and move to spawn
                'TODO: Evade
                'TODO: Remove all buffs & debuffs
                Me.State = TBaseAI.AIState.AI_MOVING_TO_SPAWN
                aiCreature.Life.Current = aiCreature.Life.Maximum
                aiCreature.Heal(0) 'So players get the health update

                If ResetX = 0.0F AndAlso ResetY = 0.0F AndAlso ResetZ = 0.0F Then
                    ResetX = aiCreature.SpawnX
                    ResetY = aiCreature.SpawnY
                    ResetZ = aiCreature.SpawnZ
                    ResetO = aiCreature.SpawnO
                End If
                DoMoveReset()
            End If
        End Sub
        Public Overrides Sub OnGenerateHate(ByRef Attacker As BaseUnit, ByVal HateValue As Integer)
            If Attacker Is aiCreature Then Exit Sub
            If Me.State <> TBaseAI.AIState.AI_DEAD AndAlso Me.State <> TBaseAI.AIState.AI_RESPAWN AndAlso Me.State <> AIState.AI_MOVING_TO_SPAWN Then
                aiCreature.SetToRealPosition()
                LastHitX = aiCreature.positionX
                LastHitY = aiCreature.positionY
                LastHitZ = aiCreature.positionZ

                If TypeOf Attacker Is CharacterObject Then
                    CType(Attacker, CharacterObject).AddToCombat(aiCreature)
                End If

                If Me.InCombat = False Then
                    aiHateTable.Add(Attacker, HateValue * Attacker.Spell_ThreatModifier)
                    OnEnterCombat()
                    Exit Sub
                End If

                If aiHateTable.ContainsKey(Attacker) = False Then
                    aiHateTable.Add(Attacker, HateValue * Attacker.Spell_ThreatModifier)
                Else
                    aiHateTable(Attacker) += HateValue * Attacker.Spell_ThreatModifier
                End If
            End If
        End Sub
        Public Overrides Sub Reset()
            aiTimer = 0
            OnLeaveCombat(True)
        End Sub
        Protected Sub GoBackToSpawn()
            Me.State = AIState.AI_MOVING_TO_SPAWN
            ResetX = aiCreature.SpawnX
            ResetY = aiCreature.SpawnY
            ResetZ = aiCreature.SpawnZ
            ResetO = aiCreature.SpawnO

            ResetRun = False
            Reset()
        End Sub
        Protected Sub SelectTarget()
            Try
                Dim max As Integer = -1
                Dim tmpTarget As BaseUnit = Nothing

                'DONE: Select max hate
                For Each Victim As KeyValuePair(Of BaseUnit, Integer) In aiHateTable
                    If Victim.Key.isDead Then
                        aiHateTableRemove.Add(Victim.Key)
                        If TypeOf Victim.Key Is CharacterObject Then
                            CType(Victim.Key, CharacterObject).RemoveFromCombat(aiCreature)
                        End If
                    ElseIf Victim.Value > max Then
                        max = Victim.Value
                        tmpTarget = Victim.Key
                    End If
                Next

                ' Remove From aiHateTable
                For Each VictimRemove As BaseUnit In aiHateTableRemove
                    aiHateTable.Remove(VictimRemove)
                Next

                'DONE: Set the target
                If (tmpTarget IsNot Nothing) AndAlso (Not aiTarget Is tmpTarget) Then
                    aiTarget = tmpTarget
                    aiCreature.TurnTo(aiTarget.positionX, aiTarget.positionY)
                    aiCreature.SendTargetUpdate(tmpTarget.GUID)

                    Me.State = AIState.AI_ATTACKING
                End If
            Catch ex As Exception
                Log.WriteLine(BaseWriter.LogType.CRITICAL, "Error selecting target.{0}{1}", vbNewLine, ex.ToString)
                Reset()
            End Try

            If aiTarget Is Nothing Then Reset()
        End Sub
        Protected Function CheckTarget() As Boolean
            If aiTarget Is Nothing Then
                Reset()
                Return True
            End If

            Return False
        End Function

        Public Overrides Sub DoThink()
            If aiCreature Is Nothing Then Exit Sub 'Fixes a crash
            If aiTimer > TAIManager.UPDATE_TIMER Then
                aiTimer -= TAIManager.UPDATE_TIMER
                Exit Sub
            Else
                aiTimer = 0
            End If

            'DONE: Creature has finished resetting
            If ResetFinished Then
                aiCreature.positionX = aiCreature.MoveX
                aiCreature.positionY = aiCreature.MoveY
                aiCreature.positionZ = aiCreature.MoveZ
                aiCreature.PositionUpdated = True

                ResetFinished = False
                ResetRun = True
                State = AIState.AI_WANDERING
                aiCreature.orientation = ResetO
            End If

            'DONE: Fixes a bug where creatures attack you when they are dead
            If Me.State <> AIState.AI_DEAD AndAlso Me.State <> AIState.AI_RESPAWN AndAlso aiCreature.Life.Current = 0 Then
                Me.State = AIState.AI_DEAD
            End If

            'DONE: If stunned
            If aiCreature.isStunned Then
                aiTimer = 1000
                Exit Sub
            End If

            'TODO: Check if there are any players to aggro!
            If aiTarget Is Nothing Then
                'Here!
            End If

            Select Case Me.State
                Case AIState.AI_DEAD
                    If Me.aiHateTable.Count > 0 Then
                        OnLeaveCombat(False)

                        aiTimer = CorpseDecay(aiCreature.CreatureInfo.Elite) * 1000
                        ignoreLoot = False
                    Else
                        If ignoreLoot = False AndAlso LootTable.ContainsKey(aiCreature.GUID) Then
                            'DONE: There's still loot, double up the decay time
                            aiTimer = CorpseDecay(aiCreature.CreatureInfo.Elite) * 1000
                            ignoreLoot = True 'And make sure the corpse decay after this
                        Else
                            Me.State = AIState.AI_RESPAWN

                            Dim RespawnTime As Integer = aiCreature.SpawnTime
                            If RespawnTime > 0 Then
                                aiTimer = RespawnTime * 1000
                                aiCreature.Despawn()
                            Else
                                aiCreature.Destroy()
                            End If
                        End If
                    End If
                Case AIState.AI_RESPAWN
                    Me.State = AIState.AI_WANDERING
                    aiCreature.Respawn()
                    aiTimer = 10000 'Wait 10 seconds before starting to react
                Case AIState.AI_MOVE_FOR_ATTACK
                    DoMove()
                Case AIState.AI_WANDERING
                    If Not AllowedMove Then State = AIState.AI_DO_NOTHING : Exit Sub
                    If IsWaypoint OrElse Rnd.NextDouble > 0.2F Then
                        DoMove()
                    End If
                Case AIState.AI_MOVING_TO_SPAWN
                    DoMoveReset()
                Case AIState.AI_ATTACKING
                    If Not AllowedAttack Then State = AIState.AI_DO_NOTHING : Exit Sub
                    DoAttack()
                Case AIState.AI_MOVING
                    If Not AllowedMove Then State = AIState.AI_DO_NOTHING : Exit Sub
                    DoMove()
                Case AIState.AI_DO_NOTHING
                Case Else
                    aiCreature.SendChatMessage("Unknown AI mode!", ChatMsg.CHAT_MSG_MONSTER_SAY, LANGUAGES.LANG_UNIVERSAL)
                    Me.State = TBaseAI.AIState.AI_DO_NOTHING
            End Select
        End Sub

        Protected Sub DoAttack()
            If Not AllowedAttack Then
                State = AIState.AI_DO_NOTHING
                aiTimer = AI_INTERVAL_MOVE
                Exit Sub
            End If
            If aiCreature.Spell_Pacifyed Then
                aiTimer = 1000
                Exit Sub
            End If

            'DONE: Change the target to the one with most threat
            SelectTarget()

            If Me.State <> AIState.AI_ATTACKING Then
                'DONE: Seems like we lost our target
                aiTimer = AI_INTERVAL_SLEEP
            Else
                'DONE: Do real melee attacks
                Try
                    If aiTarget IsNot Nothing AndAlso aiTarget.isDead Then
                        aiHateTable.Remove(aiTarget)
                        aiTarget = Nothing
                        SelectTarget()
                    End If
                    If CheckTarget() Then
                        Exit Sub
                    End If

                    Dim distance As Single = GetDistance(aiCreature, aiTarget)

                    'DONE: Far objects handling
                    If distance > (BaseUnit.CombatReach_Base + aiCreature.CombatReach + aiTarget.BoundingRadius) Then
                        'DONE: Move closer
                        Me.State = AIState.AI_MOVE_FOR_ATTACK
                        Me.DoMove()
                        Exit Sub
                    End If

                    nextAttack -= TAIManager.UPDATE_TIMER
                    If nextAttack > 0 Then Exit Sub
                    nextAttack = 0

                    'DONE: Look to aiTarget
                    If Not IsInFrontOf(CType(aiCreature, CreatureObject), CType(aiTarget, BaseUnit)) Then
                        aiCreature.TurnTo(CType(aiTarget, BaseUnit))
                    End If

                    'DONE: Deal the damage
                    Dim damageInfo As DamageInfo = CalculateDamage(CType(aiCreature, CreatureObject), aiTarget, False, False)
                    SendAttackerStateUpdate(CType(aiCreature, CreatureObject), CType(aiTarget, BaseUnit), damageInfo)
                    aiTarget.DealDamage(damageInfo.GetDamage, aiCreature)

                    'TODO: Do in another way, since 1001-2000 = 2 secs, and for creatures with like 1.05 sec attack time attacks ALOT slower
                    nextAttack = CREATURESDatabase(aiCreature.ID).BaseAttackTime
                    aiTimer = 1000
                Catch e As Exception
                    Reset()
                End Try
            End If

        End Sub
        Public Overrides Sub DoMove()
            'DONE: Back to spawn if too far away
            If aiTarget Is Nothing Then
                Dim distanceToSpawn As Single = GetDistance(aiCreature.positionX, aiCreature.SpawnX, aiCreature.positionY, aiCreature.SpawnY, aiCreature.positionZ, aiCreature.SpawnZ)
                If IsWaypoint = False AndAlso aiCreature.SpawnID > 0 AndAlso distanceToSpawn > aiCreature.MaxDistance Then
                    GoBackToSpawn()
                    Exit Sub
                End If
            Else
                Dim distanceToLastHit As Single = GetDistance(aiCreature.positionX, LastHitX, aiCreature.positionY, LastHitY, aiCreature.positionZ, LastHitZ)
                If distanceToLastHit > aiCreature.MaxDistance Then
                    OnLeaveCombat(True)
                    Exit Sub
                End If
            End If

            'DONE: If rooted don't move
            If aiCreature.isRooted Then
                aiTimer = 1000
                Exit Sub
            End If

            If aiTarget Is Nothing Then
                'DONE: Do simple random movement
                Dim MoveTries As Integer = 0
                Dim selectedX As Single = 0.0F
                Dim selectedY As Single = 0.0F
                Dim selectedZ As Single = 0.0F
                While True
                    If MoveTries > 5 Then 'The creature is at a very weird location right now
                        GoBackToSpawn()
                        Exit Sub
                    End If

                    Dim distance As Single = AI_INTERVAL_MOVE / 1000 * aiCreature.CreatureInfo.WalkSpeed
                    Dim angle As Single = Rnd.NextDouble * PIx2

                    aiCreature.SetToRealPosition()
                    aiCreature.orientation = angle
                    selectedX = aiCreature.positionX + Math.Cos(angle) * distance
                    selectedY = aiCreature.positionY + Math.Sin(angle) * distance
                    selectedZ = GetZCoord(selectedX, selectedY, aiCreature.positionZ, aiCreature.MapID)
                    MoveTries += 1
                    If Math.Abs(aiCreature.positionZ - selectedZ) > 5.0F Then Continue While 'Prevent most cases of wall climbing
                    If IsInLineOfSight(aiCreature, selectedX, selectedY, selectedZ + 1.0F) = False Then Continue While 'Prevent moving through walls
                    Exit While 'Movement success
                End While

                If aiCreature.CanMoveTo(selectedX, selectedY, selectedZ) Then
                    Me.State = TBaseAI.AIState.AI_WANDERING
                    aiTimer = aiCreature.MoveTo(selectedX, selectedY, selectedZ, , False)
                Else
                    aiTimer = AI_INTERVAL_MOVE
                End If

            Else
                'DONE: Change the target to the one with most threat
                SelectTarget()
                If CheckTarget() Then
                    Exit Sub
                End If

                'DONE: Decide it's real position
                aiCreature.SetToRealPosition()
                If TypeOf aiTarget Is CreatureObject Then CType(aiTarget, CreatureObject).SetToRealPosition()

                'DONE: Do targeted movement to attack target
                Dim distance As Single = 1000 * aiCreature.CreatureInfo.RunSpeed
                Dim distanceToTarget As Single = GetDistance(CType(aiCreature, CreatureObject), CType(aiTarget, BaseUnit))

                If distanceToTarget < distance Then
                    'DONE: Move to target
                    Me.State = TBaseAI.AIState.AI_ATTACKING

                    Dim destDist As Single = BaseUnit.CombatReach_Base + aiCreature.CombatReach + aiTarget.BoundingRadius
                    If distanceToTarget <= destDist Then
                        DoAttack()
                    End If
                    destDist *= 0.5F

                    Dim NearX As Single = aiTarget.positionX
                    If aiTarget.positionX > aiCreature.positionX Then NearX -= destDist Else NearX += destDist
                    Dim NearY As Single = aiTarget.positionY
                    If aiTarget.positionY > aiCreature.positionY Then NearY -= destDist Else NearY += destDist
                    Dim NearZ As Single = GetZCoord(NearX, NearY, aiCreature.positionZ, aiCreature.MapID)
                    If NearZ > (aiTarget.positionZ + 2) Or NearZ < (aiTarget.positionZ - 2) Then NearZ = aiTarget.positionZ
                    If aiCreature.CanMoveTo(NearX, NearY, NearZ) Then
                        aiCreature.orientation = GetOrientation(aiCreature.positionX, NearX, aiCreature.positionY, NearY)
                        aiTimer = aiCreature.MoveTo(NearX, NearY, NearZ, , True)
                    Else
                        'DONE: Select next target
                        aiHateTable.Remove(aiTarget)
                        If TypeOf aiTarget Is CharacterObject Then
                            CType(aiTarget, CharacterObject).RemoveFromCombat(aiCreature)
                        End If
                        SelectTarget()
                        CheckTarget()
                    End If

                Else
                    'DONE: Move to target by vector
                    Me.State = TBaseAI.AIState.AI_MOVE_FOR_ATTACK

                    Dim angle As Single = GetOrientation(aiCreature.positionX, aiTarget.positionX, aiCreature.positionY, aiTarget.positionY)
                    aiCreature.orientation = angle
                    Dim selectedX As Single = aiCreature.positionX + Math.Cos(angle) * distance
                    Dim selectedY As Single = aiCreature.positionY + Math.Sin(angle) * distance
                    Dim selectedZ As Single = GetZCoord(selectedX, selectedY, aiCreature.positionZ, aiCreature.MapID)

                    If aiCreature.CanMoveTo(selectedX, selectedY, selectedZ) Then
                        aiTimer = aiCreature.MoveTo(selectedX, selectedY, selectedZ, , True)
                    Else
                        'DONE: Select next target
                        aiHateTable.Remove(aiTarget)
                        If TypeOf aiTarget Is CharacterObject Then
                            CType(aiTarget, CharacterObject).RemoveFromCombat(aiCreature)
                        End If
                        SelectTarget()
                        CheckTarget()
                    End If

                End If
            End If

        End Sub

        Protected Sub DoMoveReset()
            Dim distance As Single = 0.0F
            If ResetRun Then
                distance = AI_INTERVAL_MOVE / 1000 * aiCreature.CreatureInfo.RunSpeed
            Else
                distance = AI_INTERVAL_MOVE / 1000 * aiCreature.CreatureInfo.WalkSpeed
            End If
            aiCreature.SetToRealPosition(True)
            Dim angle As Single = GetOrientation(aiCreature.positionX, ResetX, aiCreature.positionY, ResetY)
            aiCreature.orientation = angle

            Dim tmpDist As Single = GetDistance(aiCreature, ResetX, ResetY, ResetZ)
            If tmpDist < distance Then
                aiTimer = aiCreature.MoveTo(ResetX, ResetY, ResetZ, ResetO, ResetRun)
                ResetFinished = True
            Else
                Dim selectedX As Single = aiCreature.positionX + Math.Cos(angle) * distance
                Dim selectedY As Single = aiCreature.positionY + Math.Sin(angle) * distance
                Dim selectedZ As Single = GetZCoord(selectedX, selectedY, aiCreature.positionZ, aiCreature.MapID)

                aiTimer = aiCreature.MoveTo(selectedX, selectedY, selectedZ, , ResetRun) - 50 'Remove 50ms so that it doesn't pause
            End If
        End Sub

    End Class
#End Region

#Region "WS.Creatures.AI.StandStillAI"
    Public Class StandStillAI
        Inherits DefaultAI

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
        End Sub

    End Class

#End Region

#Region "WS.Creatures.AI.CritterAI"
    Public Class CritterAI
        Inherits TBaseAI

        Protected aiCreature As CreatureObject = Nothing
        Protected aiTimer As Integer = 0
        Protected CombatTimer As Integer = 0
        Protected WasAlive As Boolean = True

        Protected Const AI_INTERVAL_MOVE As Integer = 3000
        Protected Const PIx2 As Single = 2 * Math.PI

        Public Sub New(ByRef Creature As CreatureObject)
            State = AIState.AI_WANDERING

            aiCreature = Creature
            aiTarget = Nothing
        End Sub
        Public Overrides Function IsMoving() As Boolean
            If (timeGetTime - aiCreature.LastMove) < aiTimer Then
                Select Case Me.State
                    Case AIState.AI_MOVE_FOR_ATTACK
                        Return True
                    Case AIState.AI_MOVING
                        Return True
                    Case AIState.AI_WANDERING
                        Return True
                    Case Else
                        Return False
                End Select
            Else
                Return False
            End If
        End Function
        Public Overrides Sub Pause(ByVal Time As Integer)
            aiTimer = Time
        End Sub
        Public Overrides Sub OnEnterCombat()
        End Sub
        Public Overrides Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
        End Sub
        Public Overrides Sub OnGenerateHate(ByRef Attacker As BaseUnit, ByVal HateValue As Integer)
            If CombatTimer > 0 Then Exit Sub

            CombatTimer = 6000
            State = AIState.AI_ATTACKING
        End Sub
        Public Overrides Sub Reset()
            aiTimer = 0
        End Sub

        Public Overrides Sub DoThink()
            If aiCreature Is Nothing Then Exit Sub 'Fixes a crash
            If aiTimer > TAIManager.UPDATE_TIMER Then
                aiTimer -= TAIManager.UPDATE_TIMER
                Exit Sub
            Else
                aiTimer = 0
            End If

            'DONE: Fixes a bug where creatures attack you when they are dead
            If Me.State <> AIState.AI_DEAD AndAlso Me.State <> AIState.AI_RESPAWN AndAlso aiCreature.Life.Current = 0 Then
                Me.State = AIState.AI_DEAD
            End If

            'DONE: If stunned
            If aiCreature.isStunned Then
                aiTimer = 1000
                Exit Sub
            End If

            Select Case Me.State
                Case AIState.AI_DEAD
                    If WasAlive Then
                        OnLeaveCombat(False)

                        aiTimer = 30000 '30 seconds until the corpse disappear
                        WasAlive = False
                    Else
                        Me.State = AIState.AI_RESPAWN
                        WasAlive = True

                        Dim RespawnTime As Integer = aiCreature.SpawnTime
                        If RespawnTime > 0 Then
                            aiTimer = RespawnTime * 1000
                            aiCreature.Despawn()
                        Else
                            aiCreature.Destroy()
                        End If
                    End If
                Case AIState.AI_RESPAWN
                    Me.State = AIState.AI_WANDERING
                    aiCreature.Respawn()
                    aiTimer = 10000 'Wait 10 seconds before starting to react
                Case AIState.AI_MOVE_FOR_ATTACK
                    State = AIState.AI_ATTACKING
                    DoMove()
                Case AIState.AI_WANDERING
                    If Rnd.NextDouble > 0.2F Then
                        DoMove()
                    End If
                Case AIState.AI_MOVING_TO_SPAWN
                    State = AIState.AI_WANDERING
                    DoMove()
                Case AIState.AI_ATTACKING
                    DoMove()
                Case AIState.AI_MOVING
                    State = AIState.AI_WANDERING
                    DoMove()
                Case AIState.AI_DO_NOTHING
                Case Else
                    aiCreature.SendChatMessage("Unknown AI mode!", ChatMsg.CHAT_MSG_MONSTER_SAY, LANGUAGES.LANG_UNIVERSAL)
                    Me.State = TBaseAI.AIState.AI_DO_NOTHING
            End Select
        End Sub

        Public Overrides Sub DoMove()
            'DONE: If rooted don't move
            If aiCreature.isRooted Then
                aiTimer = 1000
                Exit Sub
            End If

            'DONE: Do simple random movement
            Dim MoveTries As Byte = 0
TryMoveAgain:
            If MoveTries > 5 Then 'The creature is at a very weird location right now
                aiCreature.MoveToInstant(aiCreature.SpawnX, aiCreature.SpawnY, aiCreature.SpawnZ, aiCreature.orientation)
                Exit Sub
            End If

            'DONE: If the creature was attacked it will start fleeing randomly
            Dim DoRun As Boolean = False
            If State = AIState.AI_ATTACKING Then
                CombatTimer -= TAIManager.UPDATE_TIMER

                If CombatTimer <= 0 Then
                    CombatTimer = 0
                    State = AIState.AI_WANDERING
                Else
                    DoRun = True
                End If
            End If

            Dim distance As Single = 0.0F
            If DoRun Then
                distance = AI_INTERVAL_MOVE / 1000 * aiCreature.CreatureInfo.RunSpeed * aiCreature.SpeedMod
            Else
                distance = AI_INTERVAL_MOVE / 1000 * aiCreature.CreatureInfo.WalkSpeed
            End If

            Dim angle As Single = Rnd.NextDouble * PIx2

            aiCreature.SetToRealPosition()
            aiCreature.orientation = angle
            Dim selectedX As Single = aiCreature.positionX + Math.Cos(angle) * distance
            Dim selectedY As Single = aiCreature.positionY + Math.Sin(angle) * distance
            Dim selectedZ As Single = GetZCoord(selectedX, selectedY, aiCreature.positionZ, aiCreature.MapID)
            MoveTries += 1
            If Math.Abs(aiCreature.positionZ - selectedZ) > 5.0F Then GoTo TryMoveAgain 'Prevent most cases of wall climbing
            If IsInLineOfSight(aiCreature, selectedX, selectedY, selectedZ + 2.0F) = False Then GoTo TryMoveAgain 'Prevent moving through walls

            If aiCreature.CanMoveTo(selectedX, selectedY, selectedZ) Then
                aiTimer = aiCreature.MoveTo(selectedX, selectedY, selectedZ, , DoRun)
            Else
                aiTimer = AI_INTERVAL_MOVE
            End If
        End Sub

    End Class
#End Region

#Region "WS.Creatures.AI.GuardAI"
    Public Class GuardAI
        Inherits DefaultAI

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
        End Sub

        Public Sub OnEmote(ByVal emote As Integer)
            Select Case emote
                Case 58 'Kiss
                    aiCreature.DoEmote(Emotes.ONESHOT_BOW)
                Case 101 'Wave
                    aiCreature.DoEmote(Emotes.ONESHOT_WAVE)
                Case 78 'Salute
                    aiCreature.DoEmote(Emotes.ONESHOT_SALUTE)
                Case 84 'Shy
                    aiCreature.DoEmote(Emotes.ONESHOT_FLEX)
                Case 77, 22 'Rude, Chicken
                    aiCreature.DoEmote(Emotes.ONESHOT_POINT)
            End Select
        End Sub

    End Class

#End Region

#Region "WS.Creatures.AI.WaypointAI"
    Public Class WaypointAI
        Inherits DefaultAI

        Public CurrentWaypoint As Integer = -1

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            IsWaypoint = True
        End Sub

        Public Overrides Sub Pause(ByVal Time As Integer)
            CurrentWaypoint -= 1
            aiTimer = Time
        End Sub

        Public Overrides Sub DoMove()
            Dim distanceToSpawn As Single = GetDistance(aiCreature.positionX, aiCreature.SpawnX, aiCreature.positionY, aiCreature.SpawnY, aiCreature.positionZ, aiCreature.SpawnZ)

            If aiTarget Is Nothing Then
                If CreatureMovement.ContainsKey(aiCreature.WaypointID) = False Then
                    Log.WriteLine(BaseWriter.LogType.CRITICAL, "Creature [{0:X}] is missing waypoints.", aiCreature.GUID - GUID_UNIT)
                    aiCreature.ResetAI()
                    Exit Sub
                End If

                Try
                    CurrentWaypoint += 1
                    If CreatureMovement(aiCreature.WaypointID).ContainsKey(CurrentWaypoint) = False Then CurrentWaypoint = 1
                    Dim MovementPoint As CreatureMovePoint = CreatureMovement(aiCreature.WaypointID)(CurrentWaypoint)
                    aiTimer = aiCreature.MoveTo(MovementPoint.x, MovementPoint.y, MovementPoint.z, , False) + MovementPoint.waittime
                Catch ex As Exception
                    Log.WriteLine(BaseWriter.LogType.CRITICAL, "Creature [{0:X}] waypoints are damaged.", aiCreature.GUID - GUID_UNIT)
                    aiCreature.ResetAI()
                    Exit Sub
                End Try
            Else
                MyBase.DoMove()
            End If
        End Sub

    End Class

#End Region

#Region "WS.Creatures.AI.GuardWaypointAI"
    Public Class GuardWaypointAI
        Inherits GuardAI

        Public CurrentWaypoint As Integer = -1

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = True
            IsWaypoint = True
        End Sub

        Public Overrides Sub Pause(ByVal Time As Integer)
            CurrentWaypoint -= 1
            aiTimer = Time
        End Sub

        Public Overrides Sub DoMove()
            Dim distanceToSpawn As Single = GetDistance(aiCreature.positionX, aiCreature.SpawnX, aiCreature.positionY, aiCreature.SpawnY, aiCreature.positionZ, aiCreature.SpawnZ)

            If aiTarget Is Nothing Then
                If CreatureMovement.ContainsKey(aiCreature.WaypointID) = False Then
                    Log.WriteLine(BaseWriter.LogType.CRITICAL, "Creature [{0:X}] is missing waypoints.", aiCreature.GUID - GUID_UNIT)
                    aiCreature.ResetAI()
                    Exit Sub
                End If

                Try
                    CurrentWaypoint += 1
                    If CreatureMovement(aiCreature.WaypointID).ContainsKey(CurrentWaypoint) = False Then CurrentWaypoint = 1
                    Dim MovementPoint As CreatureMovePoint = CreatureMovement(aiCreature.WaypointID)(CurrentWaypoint)
                    aiTimer = aiCreature.MoveTo(MovementPoint.x, MovementPoint.y, MovementPoint.z, , False) + MovementPoint.waittime
                Catch
                    Log.WriteLine(BaseWriter.LogType.CRITICAL, "Creature [{0:X}] waypoints are damaged.", aiCreature.GUID - GUID_UNIT)
                    aiCreature.ResetAI()
                    Exit Sub
                End Try
            Else
                MyBase.DoMove()
            End If
        End Sub

    End Class

#End Region

#End Region

#Region "WS.Creatures.AI.BossAIs"
    Public Class BossAI
        Inherits DefaultAI

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
        End Sub

        Public Overrides Sub OnEnterCombat()
            MyBase.OnEnterCombat()

            'DONE: Set every player in the same raid into combat
            For Each Unit As KeyValuePair(Of BaseUnit, Integer) In aiHateTable
                If TypeOf Unit.Key Is CharacterObject Then
                    With CType(Unit.Key, CharacterObject)
                        If .IsInGroup Then
                            Dim localMembers() As ULong = .Group.LocalMembers.ToArray()
                            For Each member As ULong In localMembers
                                If CHARACTERs.ContainsKey(member) AndAlso CHARACTERs(member).MapID = .MapID AndAlso CHARACTERs(member).instance = .instance Then
                                    aiHateTable.Add(CHARACTERs(member), 0)
                                End If
                            Next
                            Exit For
                        End If
                    End With
                End If
            Next
        End Sub

        Public Overrides Sub DoThink()
            MyBase.DoThink()

            'NOTE: Bosses uses a new thread because of their heavy updates sometimes
            Dim tmpThread As New Thread(AddressOf OnThink)
            tmpThread.Name = "Boss Thinking"
            tmpThread.Start()
        End Sub

        'NOTE: Bosses uses OnThink instead of DoThink
        Public Overridable Sub OnThink()

        End Sub

    End Class

#End Region

End Module
