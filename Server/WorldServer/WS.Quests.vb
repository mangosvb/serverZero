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

Imports mangosVB.Common.BaseWriter


Public Module WS_Quests
    Const QUEST_OBJECTIVES_COUNT As Integer = 3
    Const QUEST_REWARD_CHOICES_COUNT As Integer = 5
    Const QUEST_REWARDS_COUNT As Integer = 3
    Const QUEST_DEPLINK_COUNT As Integer = 9

    Public Const QUEST_SLOTS As Integer = 24

    '-QUEST TYPE-
    '81 = Burn ?

    Public Enum QuestgiverStatus
        DIALOG_STATUS_NONE = 0                  ' There aren't any quests available. - No Mark
        DIALOG_STATUS_UNAVAILABLE = 1           ' Quest available and your leve isn't enough. - Gray Quotation ! Mark
        DIALOG_STATUS_CHAT = 2                  ' Quest available it shows a talk baloon. - No Mark
        DIALOG_STATUS_INCOMPLETE = 3            ' Quest isn't finished yet. - Gray Question ? Mark
        DIALOG_STATUS_REWARD_REP = 4            ' Rewards rep? :P
        DIALOG_STATUS_AVAILABLE = 5             ' Quest available, and your level is enough. - Yellow Quotation ! Mark
        DIALOG_STATUS_REWARD = 6                ' Quest has been finished. - Yellow dot on the minimap
    End Enum
    Public Enum QuestObjectiveFlag 'These flags are custom and are only used for MangosVB
        QUEST_OBJECTIVE_KILL = 1 'You have to kill creatures
        QUEST_OBJECTIVE_EXPLORE = 2 'You have to explore an area
        QUEST_OBJECTIVE_ESCORT = 4 'You have to escort someone
        QUEST_OBJECTIVE_EVENT = 8 'Something is required to happen (escort may be included in this one)
        QUEST_OBJECTIVE_CAST = 16 'You will have to cast a spell on a creature or a gameobject (spells on gameobjects are f.ex opening)
        QUEST_OBJECTIVE_ITEM = 32 'You have to recieve some items to deliver
        QUEST_OBJECTIVE_EMOTE = 64 'You do an emote to a creature
    End Enum
    Public Enum QuestSpecialFlag As Integer
        QUEST_SPECIALFLAGS_NONE = 0
        QUEST_SPECIALFLAGS_DELIVER = 1
        QUEST_SPECIALFLAGS_EXPLORE = 2
        QUEST_SPECIALFLAGS_SPEAKTO = 4
        QUEST_SPECIALFLAGS_KILLORCAST = 8
        QUEST_SPECIALFLAGS_TIMED = 16
        '32 is unknown
        QUEST_SPECIALFLAGS_REPUTATION = 64
    End Enum

    Public Enum QuestInvalidError
        'SMSG_QUESTGIVER_QUEST_INVALID
        '   uint32 invalidReason

        INVALIDREASON_DONT_HAVE_REQ = 0                     'You don't meet the requirements for that quest
        INVALIDREASON_DONT_HAVE_LEVEL = 1                   'You are not high enough level for that quest.
        INVALIDREASON_DONT_HAVE_RACE = 6                    'That quest is not available to your race
        INVALIDREASON_COMPLETED_QUEST = 7                   'You have already completed this quest
        INVALIDREASON_HAVE_TIMED_QUEST = 12                 'You can only be on one timed quest at a time
        INVALIDREASON_HAVE_QUEST = 13                       'You are already on that quest
        INVALIDREASON_DONT_HAVE_EXP_ACCOUNT = 16            '??????
        INVALIDREASON_DONT_HAVE_REQ_ITEMS = 21  'Changed for 2.1.3  'You don't have the required items with you. Check storage.
        INVALIDREASON_DONT_HAVE_REQ_MONEY = 23              'You don't have enough money for that quest
        INVALIDREASON_REACHED_DAILY_LIMIT = 26              'You have completed xx daily quests today
        INVALIDREASON_UNKNOW27 = 27                         'You can not complete quests once you have reached tired time ???
    End Enum
    Public Enum QuestFailedReason
        'SMSG_QUESTGIVER_QUEST_FAILED
        '		uint32 questID
        '		uint32 failedReason

        FAILED_INVENTORY_FULL = 4       '0x04: '%s failed: Inventory is full.'
        FAILED_DUPE_ITEM = &H11         '0x11: '%s failed: Duplicate item found.'
        FAILED_INVENTORY_FULL2 = &H31   '0x31: '%s failed: Inventory is full.'
        FAILED_NOREASON = 0       '0x00: '%s failed.'
    End Enum
    Public Enum QuestPartyPushError As Byte
        QUEST_PARTY_MSG_SHARRING_QUEST = 0
        QUEST_PARTY_MSG_CANT_TAKE_QUEST = 1
        QUEST_PARTY_MSG_ACCEPT_QUEST = 2
        QUEST_PARTY_MSG_REFUSE_QUEST = 3
        QUEST_PARTY_MSG_TO_FAR = 4
        QUEST_PARTY_MSG_BUSY = 5
        QUEST_PARTY_MSG_LOG_FULL = 6
        QUEST_PARTY_MSG_HAVE_QUEST = 7
        QUEST_PARTY_MSG_FINISH_QUEST = 8
    End Enum



#Region "Quests.DataTypes"

    'WARNING: These are used only for Quests packets
    Public QUESTs As New Dictionary(Of Integer, QuestInfo)
    Public Class QuestInfo
        Implements IDisposable

        Public ID As Integer
        Public PreQuests As List(Of Integer)
        Public NextQuest As Integer = 0
        Public NextQuestInChain As Integer = 0
        Public Method As Byte = 0
        Public Type As Integer
        Public ZoneOrSort As Integer
        Public QuestFlags As Integer = 0
        Public SpecialFlags As Integer = 0
        Public Level_Start As Byte = 0
        Public Level_Normal As Short = 0

        Public Title As String = ""
        Public TextObjectives As String = ""
        Public TextDescription As String = ""
        Public TextEnd As String = ""
        Public TextIncomplete As String = ""
        Public TextComplete As String = ""

        Public RequiredRace As Integer
        Public RequiredClass As Integer
        Public RequiredTradeSkill As Integer
        Public RequiredTradeSkillValue As Integer
        Public RequiredMinReputation As Integer
        Public RequiredMinReputation_Faction As Integer
        Public RequiredMaxReputation As Integer
        Public RequiredMaxReputation_Faction As Integer

        Public RewardHonor As Integer = 0
        Public RewardGold As Integer = 0
        Public RewMoneyMaxLevel As Integer = 0
        Public RewardSpell As Integer = 0
        Public RewardSpellCast As Integer = 0
        Public RewardItems(QUEST_REWARD_CHOICES_COUNT) As Integer
        Public RewardItems_Count(QUEST_REWARD_CHOICES_COUNT) As Integer
        Public RewardStaticItems(QUEST_REWARDS_COUNT) As Integer
        Public RewardStaticItems_Count(QUEST_REWARDS_COUNT) As Integer
        Public RewardRepFaction(4) As Integer
        Public RewardRepValue(4) As Integer
        Public RewMailTemplateId As Integer
        Public RewMailDelaySecs As Integer

        Public ObjectiveRepFaction As Integer
        Public ObjectiveRepStanding As Integer

        'Explore <place_name>
        Public ObjectivesTrigger(3) As Integer
        'Cast <x> spell on <mob_name> / <object_name>
        Public ObjectivesCastSpell(3) As Integer
        'Kill <x> of <mob_name>
        Public ObjectivesKill(3) As Integer
        Public ObjectivesKill_Count(3) As Integer
        'Gather <x> of <item_name>
        Public ObjectivesItem(3) As Integer
        Public ObjectivesItem_Count(3) As Integer
        'Deliver <item_name>
        Public ObjectivesDeliver As Integer
        Public ObjectivesDeliver_Count As Integer

        Public ObjectivesText() As String = {"", "", "", ""}

        Public TimeLimit As Integer = 0
        Public SourceSpell As Integer = 0

        Public PointMapID As Integer = 0
        Public PointX As Single = 0
        Public PointY As Single = 0
        Public PointOpt As Integer = 0

        Public DetailsEmote(3) As Integer
        Public IncompleteEmote As Integer = 0
        Public CompleteEmote As Integer = 0
        Public OfferRewardEmote(3) As Integer

        Public StartScript As Integer = 0
        Public CompleteScript As Integer = 0

        Public Sub New(ByVal QuestID As Integer)
            ID = QuestID
            PreQuests = New List(Of Integer)
            Dim MySQLQuery As New DataTable

            QUESTs.Add(ID, Me)

            WorldDatabase.Query(String.Format("SELECT * FROM quests WHERE entry = {0};", QuestID), MySQLQuery)
            If MySQLQuery.Rows.Count = 0 Then Throw New ApplicationException("Quest " & QuestID & " not found in database.")

            If MySQLQuery.Rows(0).Item("PrevQuestId") > 0 Then PreQuests.Add(MySQLQuery.Rows(0).Item("PrevQuestId"))
            NextQuest = MySQLQuery.Rows(0).Item("NextQuestId")
            NextQuestInChain = MySQLQuery.Rows(0).Item("NextQuestInChain")
            Level_Start = MySQLQuery.Rows(0).Item("MinLevel")
            Level_Normal = MySQLQuery.Rows(0).Item("QuestLevel")
            Method = MySQLQuery.Rows(0).Item("Method")
            Type = MySQLQuery.Rows(0).Item("Type")
            ZoneOrSort = MySQLQuery.Rows(0).Item("ZoneOrSort")
            QuestFlags = MySQLQuery.Rows(0).Item("QuestFlags")
            SpecialFlags = MySQLQuery.Rows(0).Item("SpecialFlags")

            Dim SkillOrClass As Integer = MySQLQuery.Rows(0).Item("SkillOrClass")
            If SkillOrClass < 0 Then
                RequiredClass = -SkillOrClass
            Else
                RequiredTradeSkill = SkillOrClass
            End If

            RequiredRace = MySQLQuery.Rows(0).Item("RequiredRaces")
            RequiredTradeSkillValue = MySQLQuery.Rows(0).Item("RequiredSkillValue")
            RequiredMinReputation_Faction = MySQLQuery.Rows(0).Item("RequiredMinRepFaction")
            RequiredMinReputation = MySQLQuery.Rows(0).Item("RequiredMinRepValue")
            RequiredMinReputation_Faction = MySQLQuery.Rows(0).Item("RequiredMaxRepFaction")
            RequiredMinReputation = MySQLQuery.Rows(0).Item("RequiredMaxRepValue")

            ObjectiveRepFaction = MySQLQuery.Rows(0).Item("RepObjectiveFaction")
            ObjectiveRepStanding = MySQLQuery.Rows(0).Item("RepObjectiveValue")

            If Not TypeOf MySQLQuery.Rows(0).Item("Title") Is DBNull Then Title = MySQLQuery.Rows(0).Item("Title")
            If Not TypeOf MySQLQuery.Rows(0).Item("Objectives") Is DBNull Then TextObjectives = MySQLQuery.Rows(0).Item("Objectives")
            If Not TypeOf MySQLQuery.Rows(0).Item("Details") Is DBNull Then TextDescription = MySQLQuery.Rows(0).Item("Details")
            If Not TypeOf MySQLQuery.Rows(0).Item("EndText") Is DBNull Then TextEnd = MySQLQuery.Rows(0).Item("EndText")
            If Not TypeOf MySQLQuery.Rows(0).Item("RequestItemsText") Is DBNull Then TextIncomplete = MySQLQuery.Rows(0).Item("RequestItemsText")
            If Not TypeOf MySQLQuery.Rows(0).Item("OfferRewardText") Is DBNull Then TextComplete = MySQLQuery.Rows(0).Item("OfferRewardText")

            RewardGold = MySQLQuery.Rows(0).Item("RewOrReqMoney")
            RewMoneyMaxLevel = MySQLQuery.Rows(0).Item("RewMoneyMaxLevel")
            RewardSpell = MySQLQuery.Rows(0).Item("RewSpell")
            RewardSpellCast = MySQLQuery.Rows(0).Item("RewSpellCast")
            RewMailTemplateId = MySQLQuery.Rows(0).Item("RewMailTemplateId")
            RewMailDelaySecs = MySQLQuery.Rows(0).Item("RewMailDelaySecs")

            RewardItems(0) = MySQLQuery.Rows(0).Item("RewChoiceItemId1")
            RewardItems(1) = MySQLQuery.Rows(0).Item("RewChoiceItemId2")
            RewardItems(2) = MySQLQuery.Rows(0).Item("RewChoiceItemId3")
            RewardItems(3) = MySQLQuery.Rows(0).Item("RewChoiceItemId4")
            RewardItems(4) = MySQLQuery.Rows(0).Item("RewChoiceItemId5")
            RewardItems(5) = MySQLQuery.Rows(0).Item("RewChoiceItemId6")
            RewardItems_Count(0) = MySQLQuery.Rows(0).Item("RewChoiceItemCount1")
            RewardItems_Count(1) = MySQLQuery.Rows(0).Item("RewChoiceItemCount2")
            RewardItems_Count(2) = MySQLQuery.Rows(0).Item("RewChoiceItemCount3")
            RewardItems_Count(3) = MySQLQuery.Rows(0).Item("RewChoiceItemCount4")
            RewardItems_Count(4) = MySQLQuery.Rows(0).Item("RewChoiceItemCount5")
            RewardItems_Count(5) = MySQLQuery.Rows(0).Item("RewChoiceItemCount6")

            RewardStaticItems(0) = MySQLQuery.Rows(0).Item("RewItemId1")
            RewardStaticItems(1) = MySQLQuery.Rows(0).Item("RewItemId2")
            RewardStaticItems(2) = MySQLQuery.Rows(0).Item("RewItemId3")
            RewardStaticItems(3) = MySQLQuery.Rows(0).Item("RewItemId4")
            RewardStaticItems_Count(0) = MySQLQuery.Rows(0).Item("RewItemCount1")
            RewardStaticItems_Count(1) = MySQLQuery.Rows(0).Item("RewItemCount2")
            RewardStaticItems_Count(2) = MySQLQuery.Rows(0).Item("RewItemCount3")
            RewardStaticItems_Count(3) = MySQLQuery.Rows(0).Item("RewItemCount4")

            RewardRepFaction(0) = MySQLQuery.Rows(0).Item("RewRepFaction1")
            RewardRepFaction(1) = MySQLQuery.Rows(0).Item("RewRepFaction2")
            RewardRepFaction(2) = MySQLQuery.Rows(0).Item("RewRepFaction3")
            RewardRepFaction(3) = MySQLQuery.Rows(0).Item("RewRepFaction4")
            RewardRepFaction(4) = MySQLQuery.Rows(0).Item("RewRepFaction5")
            RewardRepValue(0) = MySQLQuery.Rows(0).Item("RewRepValue1")
            RewardRepValue(1) = MySQLQuery.Rows(0).Item("RewRepValue2")
            RewardRepValue(2) = MySQLQuery.Rows(0).Item("RewRepValue3")
            RewardRepValue(3) = MySQLQuery.Rows(0).Item("RewRepValue4")
            RewardRepValue(4) = MySQLQuery.Rows(0).Item("RewRepValue5")

            'ObjectivesTrigger(0) = MySQLQuery.Rows(0).Item("Objective_Trigger1")
            'ObjectivesTrigger(1) = MySQLQuery.Rows(0).Item("Objective_Trigger2")
            'ObjectivesTrigger(2) = MySQLQuery.Rows(0).Item("Objective_Trigger3")
            'ObjectivesTrigger(3) = MySQLQuery.Rows(0).Item("Objective_Trigger4")

            ObjectivesCastSpell(0) = MySQLQuery.Rows(0).Item("ReqSpellCast1")
            ObjectivesCastSpell(1) = MySQLQuery.Rows(0).Item("ReqSpellCast2")
            ObjectivesCastSpell(2) = MySQLQuery.Rows(0).Item("ReqSpellCast3")
            ObjectivesCastSpell(3) = MySQLQuery.Rows(0).Item("ReqSpellCast4")

            ObjectivesKill(0) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOId1")
            ObjectivesKill(1) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOId2")
            ObjectivesKill(2) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOId3")
            ObjectivesKill(3) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOId4")
            ObjectivesKill_Count(0) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOCount1")
            ObjectivesKill_Count(1) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOCount2")
            ObjectivesKill_Count(2) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOCount3")
            ObjectivesKill_Count(3) = MySQLQuery.Rows(0).Item("ReqCreatureOrGOCount4")

            ObjectivesItem(0) = MySQLQuery.Rows(0).Item("ReqItemId1")
            ObjectivesItem(1) = MySQLQuery.Rows(0).Item("ReqItemId2")
            ObjectivesItem(2) = MySQLQuery.Rows(0).Item("ReqItemId3")
            ObjectivesItem(3) = MySQLQuery.Rows(0).Item("ReqItemId4")
            ObjectivesItem_Count(0) = MySQLQuery.Rows(0).Item("ReqItemCount1")
            ObjectivesItem_Count(1) = MySQLQuery.Rows(0).Item("ReqItemCount2")
            ObjectivesItem_Count(2) = MySQLQuery.Rows(0).Item("ReqItemCount3")
            ObjectivesItem_Count(3) = MySQLQuery.Rows(0).Item("ReqItemCount4")

            ObjectivesDeliver = MySQLQuery.Rows(0).Item("SrcItemId")
            ObjectivesDeliver_Count = MySQLQuery.Rows(0).Item("SrcItemCount")

            If Not TypeOf MySQLQuery.Rows(0).Item("ObjectiveText1") Is DBNull Then ObjectivesText(0) = MySQLQuery.Rows(0).Item("ObjectiveText1")
            If Not TypeOf MySQLQuery.Rows(0).Item("ObjectiveText2") Is DBNull Then ObjectivesText(1) = MySQLQuery.Rows(0).Item("ObjectiveText2")
            If Not TypeOf MySQLQuery.Rows(0).Item("ObjectiveText3") Is DBNull Then ObjectivesText(2) = MySQLQuery.Rows(0).Item("ObjectiveText3")
            If Not TypeOf MySQLQuery.Rows(0).Item("ObjectiveText4") Is DBNull Then ObjectivesText(3) = MySQLQuery.Rows(0).Item("ObjectiveText4")

            TimeLimit = MySQLQuery.Rows(0).Item("LimitTime")
            SourceSpell = MySQLQuery.Rows(0).Item("SrcSpell")

            PointMapID = MySQLQuery.Rows(0).Item("PointMapId")
            PointX = MySQLQuery.Rows(0).Item("PointX")
            PointY = MySQLQuery.Rows(0).Item("PointY")
            PointOpt = MySQLQuery.Rows(0).Item("PointOpt")

            DetailsEmote(0) = MySQLQuery.Rows(0).Item("DetailsEmote1")
            DetailsEmote(1) = MySQLQuery.Rows(0).Item("DetailsEmote2")
            DetailsEmote(2) = MySQLQuery.Rows(0).Item("DetailsEmote3")
            DetailsEmote(3) = MySQLQuery.Rows(0).Item("DetailsEmote4")
            IncompleteEmote = MySQLQuery.Rows(0).Item("IncompleteEmote")
            CompleteEmote = MySQLQuery.Rows(0).Item("CompleteEmote")
            OfferRewardEmote(0) = MySQLQuery.Rows(0).Item("OfferRewardEmote1")
            OfferRewardEmote(1) = MySQLQuery.Rows(0).Item("OfferRewardEmote2")
            OfferRewardEmote(2) = MySQLQuery.Rows(0).Item("OfferRewardEmote3")
            OfferRewardEmote(3) = MySQLQuery.Rows(0).Item("OfferRewardEmote4")

            StartScript = MySQLQuery.Rows(0).Item("StartScript")
            CompleteScript = MySQLQuery.Rows(0).Item("CompleteScript")

            InitQuest()
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
            GC.Collect()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Private Sub InitQuest()
            If NextQuestInChain > 0 Then
                If QUESTs.ContainsKey(NextQuestInChain) = False Then Dim tmpQuest As New QuestInfo(NextQuestInChain)
                If QUESTs(NextQuestInChain).PreQuests.Contains(ID) = False Then
                    Log.WriteLine(LogType.DEBUG, "Added [{0}] to quest [{1}] prequests.", ID, NextQuestInChain)
                    QUESTs(NextQuestInChain).PreQuests.Add(ID)
                End If
            End If
            If NextQuest <> 0 Then
                Dim unsignedNextQuest As Integer = Math.Abs(NextQuest)
                Dim signedQuestID As Integer = If((NextQuest < 0), -ID, ID)
                If QUESTs.ContainsKey(unsignedNextQuest) = False Then Dim tmpQuest As New QuestInfo(unsignedNextQuest)
                If QUESTs(unsignedNextQuest).PreQuests.Contains(signedQuestID) = False Then
                    QUESTs(unsignedNextQuest).PreQuests.Add(signedQuestID)
                End If
            End If
        End Sub

        Public Function CanSeeQuest(ByRef c As CharacterObject) As Boolean
            If (CInt(c.Level) + 6) < Level_Start Then Return False
            If RequiredClass > 0 AndAlso RequiredClass <> c.Classe Then Return False
            If ZoneOrSort < 0 Then
                Dim reqSort As Byte = ClassByQuestSort(-ZoneOrSort)
                If reqSort > 0 AndAlso reqSort <> c.Classe Then Return False
            End If
            If RequiredRace <> 0 AndAlso (RequiredRace And c.RaceMask) = 0 Then Return False
            If RequiredTradeSkill > 0 Then
                If c.Skills.ContainsKey(RequiredTradeSkill) = False Then Return False
                If c.Skills(RequiredTradeSkill).Current < RequiredTradeSkillValue Then Return False
            End If
            If RequiredMinReputation_Faction > 0 AndAlso c.GetReputationValue(RequiredMinReputation_Faction) < RequiredMinReputation Then Return False
            If RequiredMaxReputation_Faction > 0 AndAlso c.GetReputationValue(RequiredMaxReputation_Faction) >= RequiredMaxReputation Then Return False
            Dim mysqlQuery As New DataTable
            If PreQuests.Count > 0 Then
                'Check if we have done the prequest
                For Each QuestID As Integer In PreQuests
                    If QuestID > 0 Then 'If we haven't done this prequest we can't do this quest
                        If c.QuestsCompleted.Contains(QuestID) = False Then Return False
                    ElseIf QuestID < 0 Then 'If we have done this prequest we can't do this quest
                        If c.QuestsCompleted.Contains(QuestID) Then Return False
                    End If
                Next
            End If
            If c.QuestsCompleted.Contains(ID) Then Return False 'We have already completed this quest
            Return True
        End Function

        Public Function SatisfyQuestLevel(ByRef c As CharacterObject) As Boolean
            If c.Level < Level_Start Then Return False
            Return True
        End Function

    End Class



    'WARNING: These are used only for CharManagment
    Public Class BaseQuest
        Public ID As Integer = 0
        Public Title As String = ""
        Public SpecialFlags As Integer = 0
        Public ObjectiveFlags As Integer = 0

        Public Slot As Byte = 0

        Public ObjectivesType() As Byte = {0, 0, 0, 0}
        Public ObjectivesDeliver As Integer
        Public ObjectivesExplore(3) As Integer
        Public ObjectivesSpell(3) As Integer
        Public ObjectivesItem(3) As Integer
        Public ObjectivesItemCount() As Byte = {0, 0, 0, 0}
        Public ObjectivesObject(3) As Integer
        Public ObjectivesCount() As Byte = {0, 0, 0, 0}
        Public Explored As Boolean = True
        Public Progress() As Byte = {0, 0, 0, 0}
        Public ProgressItem() As Byte = {0, 0, 0, 0}
        Public Complete As Boolean = False
        Public Failed As Boolean = False

        Public TimeEnd As Integer = 0

        Public Sub New()
            'Nothing? :/
        End Sub

        Public Sub New(ByVal Quest As QuestInfo)
            Dim i As Byte, j As Byte

            'Load Spell Casts
            For i = 0 To 3
                If Quest.ObjectivesCastSpell(i) > 0 Then
                    ObjectiveFlags = ObjectiveFlags Or QuestObjectiveFlag.QUEST_OBJECTIVE_CAST
                    ObjectivesType(i) = QuestObjectiveFlag.QUEST_OBJECTIVE_CAST
                    ObjectivesSpell(i) = Quest.ObjectivesCastSpell(i)
                    ObjectivesObject(j) = Quest.ObjectivesKill(i)
                    ObjectivesCount(j) = Quest.ObjectivesKill_Count(i)
                End If
            Next

            'Load Kills
            For i = 0 To 3
                If Quest.ObjectivesKill(i) > 0 Then
                    For j = 0 To 3
                        If ObjectivesType(j) = 0 Then
                            ObjectiveFlags = ObjectiveFlags Or QuestObjectiveFlag.QUEST_OBJECTIVE_KILL
                            ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL
                            ObjectivesObject(j) = Quest.ObjectivesKill(i)
                            ObjectivesCount(j) = Quest.ObjectivesKill_Count(i)
                            Exit For
                        End If
                    Next
                End If
            Next

            'Load Items
            For i = 0 To 3
                If Quest.ObjectivesItem(i) > 0 Then
                    ObjectiveFlags = ObjectiveFlags Or QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM
                    ObjectivesType(i) = QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM
                    ObjectivesItem(i) = Quest.ObjectivesItem(i)
                    ObjectivesItemCount(i) = Quest.ObjectivesItem_Count(i)
                End If
            Next

            'Load Exploration loctions
            If (Quest.SpecialFlags And QuestSpecialFlag.QUEST_SPECIALFLAGS_EXPLORE) Then
                ObjectiveFlags = ObjectiveFlags Or QuestObjectiveFlag.QUEST_OBJECTIVE_EXPLORE
                For i = 0 To 3
                    ObjectivesType(i) = QuestObjectiveFlag.QUEST_OBJECTIVE_EXPLORE
                    ObjectivesExplore(i) = Quest.ObjectivesTrigger(i)
                Next
            End If
            ''TODO: Fix this below
            'If (Quest.Flags And QuestFlag.QUEST_FLAGS_EVENT) Then
            '    ObjectiveFlags = ObjectiveFlags Or QuestObjectiveFlag.QUEST_OBJECTIVE_EVENT
            '    For i = 0 To 3
            '        If ObjectivesType(i) = 0 Then
            '            ObjectivesType(i) = QuestObjectiveFlag.QUEST_OBJECTIVE_EVENT
            '            ObjectivesCount(i) = 1
            '        End If
            '    Next
            'End If

            'No objective flags are set, complete it directly
            If ObjectiveFlags = 0 Then
                For i = 0 To 3
                    'Make sure these are zero
                    ObjectivesObject(i) = 0
                    ObjectivesCount(i) = 0
                    ObjectivesExplore(i) = 0
                    ObjectivesSpell(i) = 0
                    ObjectivesType(i) = 0
                Next
                IsCompleted()
            End If

            Title = Quest.Title
            ID = Quest.ID
            SpecialFlags = Quest.SpecialFlags
            ObjectivesDeliver = Quest.ObjectivesDeliver
            'TODO: Fix a timer or something so that the quest really expires when it does
            If Quest.TimeLimit > 0 Then TimeEnd = GetTimestamp(Now) + Quest.TimeLimit 'The time the quest expires
        End Sub
        Public Sub UpdateItemCount(ByRef c As CharacterObject)
            'DONE: Update item count at login
            For i As Byte = 0 To 3
                If ObjectivesItem(i) <> 0 Then
                    ProgressItem(i) = c.ItemCOUNT(ObjectivesItem(i))
                    Log.WriteLine(LogType.DEBUG, "ITEM COUNT UPDATED TO: {0}", ProgressItem(i))
                End If
            Next

            'DONE: If the quest doesn't require any explore than set this as completed
            If (ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_EXPLORE) = 0 Then Explored = True

            'DONE: Check if the quest is completed
            IsCompleted()
        End Sub
        Public Sub Initialize(ByRef c As CharacterObject)
            Dim i As Byte
            If ObjectivesDeliver > 0 Then
                Dim tmpItem As New ItemObject(ObjectivesDeliver, c.GUID)
                If Not c.ItemADD(tmpItem) Then
                    'DONE: Some error, unable to add item, quest is uncompletable
                    tmpItem.Delete()

                    Dim response As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_FAILED)
                    response.AddInt32(ID)
                    response.AddInt32(QuestFailedReason.FAILED_INVENTORY_FULL)
                    c.Client.Send(response)
                    response.Dispose()
                    Exit Sub
                Else
                    c.LogLootItem(tmpItem, 1, True, False)
                End If
            End If

            For i = 0 To 3
                If ObjectivesItem(i) <> 0 Then ProgressItem(i) = c.ItemCOUNT(ObjectivesItem(i))
            Next

            If (ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_EXPLORE) Then Explored = False

            IsCompleted()
        End Sub
        Public Overridable Function IsCompleted() As Boolean
            Complete = (ObjectivesCount(0) <= Progress(0) AndAlso ObjectivesCount(1) <= Progress(1) AndAlso ObjectivesCount(2) <= Progress(2) AndAlso ObjectivesCount(3) <= Progress(3) AndAlso ObjectivesItemCount(0) <= ProgressItem(0) AndAlso ObjectivesItemCount(1) <= ProgressItem(1) AndAlso ObjectivesItemCount(2) <= ProgressItem(2) AndAlso ObjectivesItemCount(3) <= ProgressItem(3) AndAlso Explored AndAlso Failed = False)
            Return Complete
        End Function
        Public Overridable Function GetState(Optional ByVal ForSave As Boolean = False) As Integer
            Dim tmpState As Integer
            If Complete Then tmpState = 1
            If Failed Then tmpState = 2
            Return tmpState
        End Function
        Public Overridable Function GetProgress(Optional ByVal ForSave As Boolean = False) As Integer
            Dim tmpProgress As Integer = 0
            If ForSave Then
                tmpProgress += CType(Progress(0), Integer)
                tmpProgress += CType(Progress(1), Integer) << 6
                tmpProgress += CType(Progress(2), Integer) << 12
                tmpProgress += CType(Progress(3), Integer) << 18
                If Explored Then tmpProgress += CType(1, Integer) << 24
                If Complete Then tmpProgress += CType(1, Integer) << 25
                If Failed Then tmpProgress += CType(1, Integer) << 26
            Else
                tmpProgress += CType(Progress(0), Integer)
                tmpProgress += CType(Progress(1), Integer) << 6
                tmpProgress += CType(Progress(2), Integer) << 12
                tmpProgress += CType(Progress(3), Integer) << 18

                If Complete Then tmpProgress += CType(1, Integer) << 24
                If Failed Then tmpProgress += CType(1, Integer) << 25
            End If
            Return tmpProgress
        End Function
        Public Overridable Sub LoadState(ByVal state As Integer)
            Progress(0) = state And &H3F
            Progress(1) = (state >> 6) And &H3F
            Progress(2) = (state >> 12) And &H3F
            Progress(3) = (state >> 18) And &H3F
            Explored = (((state >> 24) And &H1) = 1)
            Complete = (((state >> 25) And &H1) = 1)
            Failed = (((state >> 26) And &H1) = 1)
        End Sub
        Public Sub AddKill(ByVal c As CharacterObject, ByVal index As Byte, ByVal oGUID As ULong)
            Progress(index) += 1
            IsCompleted()
            c.TalkUpdateQuest(Slot)

            SendQuestMessageAddKill(c.Client, ID, oGUID, ObjectivesObject(index), Progress(index), ObjectivesCount(index))
        End Sub
        Public Sub AddCast(ByVal c As CharacterObject, ByVal index As Byte, ByVal oGUID As ULong)
            Progress(index) += 1
            IsCompleted()
            c.TalkUpdateQuest(Slot)

            SendQuestMessageAddKill(c.Client, ID, oGUID, ObjectivesObject(index), Progress(index), ObjectivesCount(index))
        End Sub
        Public Sub AddExplore(ByVal c As CharacterObject)
            Explored = True
            IsCompleted()
            c.TalkUpdateQuest(Slot)

            SendQuestMessageComplete(c.Client, ID)
        End Sub
        Public Sub AddEmote(ByVal c As CharacterObject, ByVal index As Byte)
            Progress(index) += 1
            IsCompleted()
            c.TalkUpdateQuest(Slot)

            SendQuestMessageComplete(c.Client, ID)
        End Sub
        Public Sub AddItem(ByVal c As CharacterObject, ByVal index As Byte, ByVal Count As Byte)
            If ProgressItem(index) + Count > ObjectivesItemCount(index) Then Count = ObjectivesItemCount(index) - ProgressItem(index)
            ProgressItem(index) += Count
            IsCompleted()
            c.TalkUpdateQuest(Slot)

            'TODO: When item quest event is fired as it should, remove -1 here.
            Dim ItemCount As Integer = Count - 1
            SendQuestMessageAddItem(c.Client, ObjectivesItem(index), ItemCount)
        End Sub
        Public Sub RemoveItem(ByVal c As CharacterObject, ByVal index As Byte, ByVal Count As Byte)
            If CInt(ProgressItem(index)) - CInt(Count) < 0 Then Count = ProgressItem(index)
            ProgressItem(index) -= Count
            IsCompleted()
            c.TalkUpdateQuest(Slot)
        End Sub
    End Class
    Public Class BaseQuestScripted
        Inherits BaseQuest
        Public Overridable Sub OnQuestStart(ByRef c As CharacterObject)
        End Sub
        Public Overridable Sub OnQuestComplete(ByRef c As CharacterObject)
        End Sub
        Public Overridable Sub OnQuestCancel(ByRef c As CharacterObject)
        End Sub

        Public Overridable Sub OnQuestItem(ByRef c As CharacterObject, ByVal ItemID As Integer, ByVal ItemCount As Integer)
        End Sub
        Public Overridable Sub OnQuestKill(ByRef c As CharacterObject, ByRef Creature As CreatureObject)
        End Sub
        Public Overridable Sub OnQuestCastSpell(ByRef c As CharacterObject, ByRef Creature As CreatureObject, ByVal SpellID As Integer)
        End Sub
        Public Overridable Sub OnQuestCastSpell(ByRef c As CharacterObject, ByRef GameObject As GameObjectObject, ByVal SpellID As Integer)
        End Sub
        Public Overridable Sub OnQuestExplore(ByRef c As CharacterObject, ByVal AreaID As Integer)
        End Sub
        Public Overridable Sub OnQuestEmote(ByRef c As CharacterObject, ByRef Creature As CreatureObject, ByVal EmoteID As Integer)
        End Sub
    End Class

#End Region
#Region "Quests.HelpingSubs"


    Public Function GetQuestMenu(ByRef c As CharacterObject, ByVal GUID As ULong) As QuestMenu
        Dim QuestMenu As New QuestMenu
        Dim CreatureEntry As Integer = WORLD_CREATUREs(GUID).ID

        'DONE: Quests for completing
        Dim i As Integer
        Dim alreadyHave As New List(Of Integer)
        If CreatureQuestFinishers.ContainsKey(CreatureEntry) Then
            For i = 0 To QUEST_SLOTS
                If c.TalkQuests(i) IsNot Nothing Then
                    alreadyHave.Add(c.TalkQuests(i).ID)
                    If CreatureQuestFinishers(CreatureEntry).Contains(c.TalkQuests(i).ID) Then
                        QuestMenu.AddMenu(c.TalkQuests(i).Title, c.TalkQuests(i).ID, 0, QuestgiverStatus.DIALOG_STATUS_INCOMPLETE)
                    End If
                End If
            Next
        End If

        'DONE: Quests for taking
        If CreatureQuestStarters.ContainsKey(CreatureEntry) Then
            For Each QuestID As Integer In CreatureQuestStarters(CreatureEntry)
                If alreadyHave.Contains(QuestID) Then Continue For
                If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)
                If QUESTs(QuestID).CanSeeQuest(c) Then
                    If QUESTs(QuestID).SatisfyQuestLevel(c) Then
                        QuestMenu.AddMenu(QUESTs(QuestID).Title, QuestID, QUESTs(QuestID).Level_Normal, QuestgiverStatus.DIALOG_STATUS_AVAILABLE)
                    End If
                End If
            Next
        End If

        Return QuestMenu
    End Function
    Public Function GetQuestMenuGO(ByRef c As CharacterObject, ByVal GUID As ULong) As QuestMenu
        Dim QuestMenu As New QuestMenu
        Dim GOEntry As Integer = WORLD_GAMEOBJECTs(GUID).ID

        'DONE: Quests for completing
        Dim i As Integer
        Dim alreadyHave As New List(Of Integer)
        If GameobjectQuestFinishers.ContainsKey(GOEntry) Then
            For i = 0 To QUEST_SLOTS
                If c.TalkQuests(i) IsNot Nothing Then
                    alreadyHave.Add(c.TalkQuests(i).ID)
                    If GameobjectQuestFinishers(GOEntry).Contains(c.TalkQuests(i).ID) Then
                        QuestMenu.AddMenu(c.TalkQuests(i).Title, c.TalkQuests(i).ID, 0, QuestgiverStatus.DIALOG_STATUS_INCOMPLETE)
                    End If
                End If
            Next
        End If

        'DONE: Quests for taking
        If GameobjectQuestStarters.ContainsKey(GOEntry) Then
            For Each QuestID As Integer In GameobjectQuestStarters(GOEntry)
                If alreadyHave.Contains(QuestID) Then Continue For
                If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)
                If QUESTs(QuestID).CanSeeQuest(c) Then
                    If QUESTs(QuestID).SatisfyQuestLevel(c) Then
                        QuestMenu.AddMenu(QUESTs(QuestID).Title, QuestID, QUESTs(QuestID).Level_Normal, QuestgiverStatus.DIALOG_STATUS_AVAILABLE)
                    End If
                End If
            Next
        End If

        Return QuestMenu
    End Function
    Public Sub SendQuestMenu(ByRef c As CharacterObject, ByVal GUID As ULong, Optional ByVal Title As String = "Available quests", Optional ByVal QuestMenu As QuestMenu = Nothing)
        If QuestMenu Is Nothing Then
            QuestMenu = GetQuestMenu(c, GUID)
        End If

        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_LIST)
        packet.AddUInt64(GUID)
        packet.AddString(Title)
        packet.AddInt32(1)              'Delay
        packet.AddInt32(1)              'Emote
        packet.AddInt8(QuestMenu.IDs.Count) 'Count
        Dim i As Integer = 0
        For i = 0 To QuestMenu.IDs.Count - 1
            packet.AddInt32(QuestMenu.IDs(i))
            packet.AddInt32(QuestMenu.Icons(i))
            packet.AddInt32(QuestMenu.Levels(i))
            packet.AddString(QuestMenu.Names(i))
        Next
        c.Client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub SendQuestDetails(ByRef client As ClientClass, ByRef Quest As QuestInfo, ByVal GUID As ULong, ByVal AcceptActive As Boolean)
        Dim i As Integer
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_DETAILS)
        packet.AddUInt64(GUID)

        'QuestDetails
        packet.AddInt32(Quest.ID)
        packet.AddString(Quest.Title)
        packet.AddString(Quest.TextDescription)
        packet.AddString(Quest.TextObjectives)
        packet.AddInt32(If(AcceptActive, 1, 0))

        'QuestRewards (Choosable)
        Dim questRewardsCount As Integer = 0
        For i = 0 To QUEST_REWARD_CHOICES_COUNT
            If Quest.RewardItems(i) <> 0 Then questRewardsCount += 1
        Next
        packet.AddInt32(questRewardsCount)
        For i = 0 To QUEST_REWARD_CHOICES_COUNT
            If Quest.RewardItems(i) <> 0 Then
                'Add item if not loaded into server
                If Not ITEMDatabase.ContainsKey(Quest.RewardItems(i)) Then Dim tmpItem As New ItemInfo(Quest.RewardItems(i))
                packet.AddInt32(Quest.RewardItems(i))
                packet.AddInt32(Quest.RewardItems_Count(i))
                packet.AddInt32(ITEMDatabase(Quest.RewardItems(i)).Model)
            Else
                packet.AddInt32(0)
                packet.AddInt32(0)
                packet.AddInt32(0)
            End If
        Next
        'QuestRewards (Static)
        questRewardsCount = 0
        For i = 0 To QUEST_REWARDS_COUNT
            If Quest.RewardStaticItems(i) <> 0 Then questRewardsCount += 1
        Next
        packet.AddInt32(questRewardsCount)
        For i = 0 To QUEST_REWARDS_COUNT
            If Quest.RewardStaticItems(i) <> 0 Then
                'Add item if not loaded into server
                If Not ITEMDatabase.ContainsKey(Quest.RewardStaticItems(i)) Then Dim tmpItem As New ItemInfo(Quest.RewardStaticItems(i))
                packet.AddInt32(Quest.RewardStaticItems(i))
                packet.AddInt32(Quest.RewardStaticItems_Count(i))
                packet.AddInt32(ITEMDatabase(Quest.RewardStaticItems(i)).Model)
            Else
                packet.AddInt32(0)
                packet.AddInt32(0)
                packet.AddInt32(0)
            End If
        Next
        packet.AddInt32(Quest.RewardGold)

        questRewardsCount = 0
        For i = 0 To QUEST_OBJECTIVES_COUNT
            If Quest.ObjectivesItem(i) <> 0 Then questRewardsCount += 1
        Next
        packet.AddInt32(questRewardsCount)
        For i = 0 To QUEST_OBJECTIVES_COUNT
            'Add item if not loaded into server
            If Quest.ObjectivesItem(i) <> 0 AndAlso ITEMDatabase.ContainsKey(Quest.ObjectivesItem(i)) = False Then Dim tmpItem As New ItemInfo(Quest.ObjectivesItem(i))
            packet.AddInt32(Quest.ObjectivesItem(i))
            packet.AddInt32(Quest.ObjectivesItem_Count(i))
        Next

        questRewardsCount = 0
        For i = 0 To QUEST_OBJECTIVES_COUNT
            If Quest.ObjectivesKill(i) <> 0 Then questRewardsCount += 1
        Next
        packet.AddInt32(questRewardsCount)
        For i = 0 To QUEST_OBJECTIVES_COUNT
            packet.AddUInt32(Quest.ObjectivesKill(i))
            packet.AddInt32(Quest.ObjectivesKill_Count(i))
        Next

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_QUESTGIVER_QUEST_DETAILS [GUID={2:X} Quest={3}]", client.IP, client.Port, GUID, Quest.ID)

        'Finishing
        client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub SendQuest(ByRef client As ClientClass, ByRef Quest As QuestInfo)
        Dim packet As New PacketClass(OPCODES.SMSG_QUEST_QUERY_RESPONSE)
        packet.AddInt32(Quest.ID)

        'Basic Details
        packet.AddInt32(Quest.Level_Start)
        packet.AddInt32(Quest.Level_Normal)
        packet.AddInt32(Quest.ZoneOrSort)
        packet.AddInt32(Quest.Type)
        packet.AddInt32(Quest.ObjectiveRepFaction)
        packet.AddInt32(Quest.ObjectiveRepStanding)
        packet.AddInt32(0)
        packet.AddInt32(0)
        packet.AddInt32(Quest.NextQuestInChain)
        packet.AddInt32(Quest.RewardGold) 'Negative is required money
        packet.AddInt32(Quest.RewMoneyMaxLevel)

        If Quest.RewardSpell > 0 Then
            If SPELLs.ContainsKey(Quest.RewardSpell) Then
                If SPELLs(Quest.RewardSpell).SpellEffects(0) IsNot Nothing AndAlso SPELLs(Quest.RewardSpell).SpellEffects(0).ID = SpellEffects_Names.SPELL_EFFECT_LEARN_SPELL Then
                    packet.AddInt32(SPELLs(Quest.RewardSpell).SpellEffects(0).TriggerSpell)
                Else
                    packet.AddInt32(Quest.RewardSpell)
                End If
            Else
                packet.AddInt32(0)
            End If
        Else
            packet.AddInt32(0)
        End If

        packet.AddInt32(Quest.ObjectivesDeliver) ' Item given at the start of a quest (srcItem)
        packet.AddInt32((Quest.QuestFlags And &HFFFF))

        Dim i As Integer
        For i = 0 To QUEST_REWARDS_COUNT
            packet.AddInt32(Quest.RewardStaticItems(i))
            packet.AddInt32(Quest.RewardStaticItems_Count(i))
        Next
        For i = 0 To QUEST_REWARD_CHOICES_COUNT
            packet.AddInt32(Quest.RewardItems(i))
            packet.AddInt32(Quest.RewardItems_Count(i))
        Next

        packet.AddInt32(Quest.PointMapID)       'Point MapID
        packet.AddSingle(Quest.PointX)          'Point X
        packet.AddSingle(Quest.PointY)          'Point Y
        packet.AddInt32(Quest.PointOpt)         'Point Opt

        'Texts
        packet.AddString(Quest.Title)
        packet.AddString(Quest.TextObjectives)
        packet.AddString(Quest.TextDescription)
        packet.AddString(Quest.TextEnd)

        'Objectives
        For i = 0 To QUEST_OBJECTIVES_COUNT
            packet.AddInt32(Quest.ObjectivesKill(i))
            packet.AddInt32(Quest.ObjectivesKill_Count(i))
            packet.AddInt32(Quest.ObjectivesItem(i))
            packet.AddInt32(Quest.ObjectivesItem_Count(i))

            'HACK: Fix for not showing "Unknown Item" (sometimes client doesn't get items on time)
            If Quest.ObjectivesItem(i) <> 0 Then SendItemInfo(client, Quest.ObjectivesItem(i))
        Next

        For i = 0 To QUEST_OBJECTIVES_COUNT
            packet.AddString(Quest.ObjectivesText(i))
        Next

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_QUEST_QUERY_RESPONSE [Quest={2}]", client.IP, client.Port, Quest.ID)

        'Finishing
        client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub SendQuestMessageAddItem(ByRef client As ClientClass, ByVal itemID As Integer, ByVal itemCount As Integer)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_ADD_ITEM)
        packet.AddInt32(itemID)
        packet.AddInt32(itemCount)
        client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub SendQuestMessageAddKill(ByRef client As ClientClass, ByVal questID As Integer, ByVal killGUID As ULong, ByVal killID As Integer, ByVal killCurrentCount As Integer, ByVal killCount As Integer)
        'Message: %s slain: %d/%d
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_ADD_KILL)
        packet.AddInt32(questID)
        If killID < 0 Then killID = ((-killID) Or &H80000000) 'Gameobject
        packet.AddInt32(killID)
        packet.AddInt32(killCurrentCount)
        packet.AddInt32(killCount)
        packet.AddUInt64(killGUID)
        client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub SendQuestMessageFailed(ByRef client As ClientClass, ByVal QuestID As Integer)
        'Message: ?
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_FAILED)
        packet.AddInt32(QuestID)
        ' TODO: Need to add failed reason to packet here
        client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub SendQuestMessageFailedTimer(ByRef client As ClientClass, ByVal QuestID As Integer)
        'Message: ?
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_FAILEDTIMER)
        packet.AddInt32(QuestID)
        client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub SendQuestMessageComplete(ByRef client As ClientClass, ByVal QuestID As Integer)
        'Message: Objective Complete.
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_COMPLETE)
        packet.AddInt32(QuestID)
        client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub SendQuestComplete(ByRef client As ClientClass, ByRef Quest As QuestInfo, ByVal XP As Integer, ByVal Gold As Integer)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_COMPLETE)

        packet.AddInt32(Quest.ID)
        packet.AddInt32(3)
        packet.AddInt32(XP)
        packet.AddInt32(Gold)
        packet.AddInt32(Quest.RewardHonor) ' bonus honor...used in BG quests

        Dim i As Integer, rewardsCount As Integer = 0
        For i = 0 To QUEST_REWARDS_COUNT
            If Quest.RewardStaticItems(i) > 0 Then rewardsCount += 1
        Next
        packet.AddInt32(rewardsCount)
        For i = 0 To QUEST_REWARDS_COUNT
            If Quest.RewardStaticItems(i) > 0 Then
                packet.AddInt32(Quest.RewardStaticItems(i))
                packet.AddInt32(Quest.RewardStaticItems_Count(i))
            End If
        Next
        client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub SendQuestReward(ByRef client As ClientClass, ByRef Quest As QuestInfo, ByVal GUID As ULong, ByRef q As BaseQuest)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_OFFER_REWARD)

        packet.AddUInt64(GUID)
        packet.AddInt32(q.ID)
        packet.AddString(q.Title)
        packet.AddString(Quest.TextComplete)

        packet.AddInt32(CType(q.Complete, Integer))     'EnbleNext

        Dim emoteCount As Integer = 0
        Dim i As Integer
        For i = 0 To 3
            If Quest.OfferRewardEmote(i) <= 0 Then Continue For
            emoteCount += 1
        Next

        packet.AddInt32(emoteCount)
        For i = 0 To emoteCount - 1
            packet.AddInt32(0) 'EmoteDelay
            packet.AddInt32(Quest.OfferRewardEmote(i))
        Next

        Dim questRewardsCount As Integer = 0
        For i = 0 To QUEST_REWARD_CHOICES_COUNT
            If Quest.RewardItems(i) <> 0 Then questRewardsCount += 1
        Next
        packet.AddInt32(questRewardsCount)
        For i = 0 To QUEST_REWARD_CHOICES_COUNT
            If Quest.RewardItems(i) <> 0 Then
                packet.AddInt32(Quest.RewardItems(i))
                packet.AddInt32(Quest.RewardItems_Count(i))

                'Add item if not loaded into server
                If Not ITEMDatabase.ContainsKey(Quest.RewardItems(i)) Then Dim tmpItem As New ItemInfo(Quest.RewardItems(i))
                packet.AddInt32(ITEMDatabase(Quest.RewardItems(i)).Model)
            End If
        Next

        questRewardsCount = 0
        For i = 0 To QUEST_REWARDS_COUNT
            If Quest.RewardStaticItems(i) <> 0 Then questRewardsCount += 1
        Next
        packet.AddInt32(questRewardsCount)
        For i = 0 To QUEST_REWARDS_COUNT
            If Quest.RewardStaticItems(i) <> 0 Then
                packet.AddInt32(Quest.RewardStaticItems(i))
                packet.AddInt32(Quest.RewardStaticItems_Count(i))

                'Add item if not loaded into server
                If Not ITEMDatabase.ContainsKey(Quest.RewardStaticItems(i)) Then Dim tmpItem As New ItemInfo(Quest.RewardStaticItems(i))
                packet.AddInt32(ITEMDatabase(Quest.RewardStaticItems(i)).Model)
            End If
        Next

        packet.AddInt32(Quest.RewardGold)
        packet.AddInt32(0)

        If Quest.RewardSpell > 0 Then
            If SPELLs.ContainsKey(Quest.RewardSpell) Then
                If SPELLs(Quest.RewardSpell).SpellEffects(0) IsNot Nothing AndAlso SPELLs(Quest.RewardSpell).SpellEffects(0).ID = SpellEffects_Names.SPELL_EFFECT_LEARN_SPELL Then
                    packet.AddInt32(SPELLs(Quest.RewardSpell).SpellEffects(0).TriggerSpell)
                Else
                    packet.AddInt32(Quest.RewardSpell)
                End If
            Else
                packet.AddInt32(0)
            End If
        Else
            packet.AddInt32(0)
        End If

        client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub SendQuestRequireItems(ByRef client As ClientClass, ByRef Quest As QuestInfo, ByVal GUID As ULong, ByRef q As BaseQuest)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_REQUEST_ITEMS)

        packet.AddUInt64(GUID)
        packet.AddInt32(q.ID)
        packet.AddString(q.Title)
        packet.AddString(Quest.TextIncomplete)
        packet.AddInt32(0) 'Unknown

        If q.Complete Then
            packet.AddInt32(Quest.CompleteEmote)
        Else
            packet.AddInt32(Quest.IncompleteEmote)
        End If

        packet.AddInt32(0)                      'Close Window on Cancel (1 = true / 0 = false)
        If Quest.RewardGold < 0 Then
            packet.AddInt32(-Quest.RewardGold)   'Required Money
        Else
            packet.AddInt32(0)
        End If

        'DONE: Count the required items
        Dim i As Integer = 0
        Dim requiredItemsCount As Byte = 0
        For i = 0 To QUEST_OBJECTIVES_COUNT
            If Quest.ObjectivesItem(i) <> 0 Then requiredItemsCount += 1
        Next
        packet.AddInt32(requiredItemsCount)

        'DONE: List items
        If requiredItemsCount > 0 Then
            For i = 0 To QUEST_OBJECTIVES_COUNT
                If Quest.ObjectivesItem(i) <> 0 Then
                    If ITEMDatabase.ContainsKey(Quest.ObjectivesItem(i)) = False Then Dim tmpItem As ItemInfo = New ItemInfo(Quest.ObjectivesItem(i))
                    packet.AddInt32(Quest.ObjectivesItem(i))
                    packet.AddInt32(Quest.ObjectivesItem_Count(i))
                    If ITEMDatabase.ContainsKey(Quest.ObjectivesItem(i)) Then
                        packet.AddInt32(ITEMDatabase(Quest.ObjectivesItem(i)).Model)
                    Else
                        packet.AddInt32(0)
                    End If
                End If
            Next
        End If

        packet.AddInt32(2)
        If q.Complete Then
            packet.AddInt32(3)
        Else
            packet.AddInt32(0)
        End If

        packet.AddInt32(&H4)
        packet.AddInt32(&H8)
        packet.AddInt32(&H10)

        client.Send(packet)
        packet.Dispose()
    End Sub


    Public Sub LoadQuests(ByRef c As CharacterObject)
        Dim cQuests As New DataTable
        CharacterDatabase.Query(String.Format("SELECT * FROM characters_quests q WHERE q.char_guid = {0};", c.GUID), cQuests)

        Dim i As Integer = 0
        For Each cRow As DataRow In cQuests.Rows
            Dim questID As Integer = CInt(cRow.Item("quest_id"))
            Dim questStatus As Integer = cRow.Item("quest_status")
            If questStatus >= 0 Then
                Dim tmpQuest As QuestInfo
                If QUESTs.ContainsKey(questID) Then
                    tmpQuest = QUESTs(questID)
                Else
                    tmpQuest = New QuestInfo(questID)
                End If

                'DONE: Initialize quest info
                CreateQuest(c.TalkQuests(i), tmpQuest)

                c.TalkQuests(i).LoadState(cRow.Item("quest_status"))
                c.TalkQuests(i).Slot = i
                c.TalkQuests(i).UpdateItemCount(c)

                i += 1
            ElseIf questStatus = -1 Then 'Completed
                c.QuestsCompleted.Add(questID)
            End If
        Next

    End Sub
    Public Sub CreateQuest(ByRef q As BaseQuest, ByRef tmpQuest As QuestInfo)
        'Initialize Quest
        q = New BaseQuest(tmpQuest)
    End Sub

#End Region
#Region "Quests.Events"


    'DONE: Kill quest events
    Public Sub OnQuestKill(ByRef c As CharacterObject, ByRef Creature As CreatureObject)
        'HANDLERS: Added to DealDamage sub

        'DONE: Do not count killed from guards
        If c Is Nothing Then Exit Sub
        Dim i As Integer, j As Byte

        'DONE: Count kills
        For i = 0 To QUEST_SLOTS
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_KILL) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) = 0 Then
                If TypeOf c.TalkQuests(i) Is BaseQuestScripted Then
                    CType(c.TalkQuests(i), BaseQuestScripted).OnQuestKill(c, Creature)
                Else
                    With c.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesObject(j) = Creature.ID Then
                                If .Progress(j) < .ObjectivesCount(j) Then
                                    .AddKill(c, j, Creature.GUID)
                                    Exit Sub
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i


        Exit Sub  'For now next is disabled

        'DONE: Check all in c's party for that quest
        For Each GUID As ULong In c.Group.LocalMembers
            If GUID = c.GUID Then Continue For

            With CHARACTERs(GUID)
                For i = 0 To QUEST_SLOTS
                    If (Not .TalkQuests(i) Is Nothing) AndAlso (.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_KILL) AndAlso (.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) = 0 Then
                        With .TalkQuests(i)
                            For j = 0 To 3
                                If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesObject(j) = Creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddKill(c, j, Creature.GUID)
                                        Exit Sub
                                    End If
                                End If
                            Next
                        End With
                    End If
                Next i
            End With
        Next

    End Sub

    Public Sub OnQuestCastSpell(ByRef c As CharacterObject, ByRef Creature As CreatureObject, ByVal SpellID As Integer)
        Dim i As Integer, j As Byte

        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct creature
        For i = 0 To QUEST_SLOTS
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) Then
                If TypeOf c.TalkQuests(i) Is BaseQuestScripted Then
                    CType(c.TalkQuests(i), BaseQuestScripted).OnQuestCastSpell(c, Creature, SpellID)
                Else
                    With c.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesSpell(j) = SpellID Then
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = Creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddCast(c, j, Creature.GUID)
                                        Exit Sub
                                    End If
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i
    End Sub

    Public Sub OnQuestCastSpell(ByRef c As CharacterObject, ByRef GameObject As GameObjectObject, ByVal SpellID As Integer)
        Dim i As Integer, j As Byte

        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct gameobject
        For i = 0 To QUEST_SLOTS
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) Then
                If TypeOf c.TalkQuests(i) Is BaseQuestScripted Then
                    CType(c.TalkQuests(i), BaseQuestScripted).OnQuestCastSpell(c, GameObject, SpellID)
                Else
                    With c.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesSpell(j) = SpellID Then
                                'NOTE: GameObjects are negative here!
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = -(GameObject.ID) Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddCast(c, j, GameObject.GUID)
                                        Exit Sub
                                    End If
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i
    End Sub

    Public Sub OnQuestDoEmote(ByRef c As CharacterObject, ByRef Creature As CreatureObject, ByVal EmoteID As Integer)
        Dim i As Integer, j As Byte

        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct gameobject
        For i = 0 To QUEST_SLOTS
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_EMOTE) Then
                If TypeOf c.TalkQuests(i) Is BaseQuestScripted Then
                    CType(c.TalkQuests(i), BaseQuestScripted).OnQuestEmote(c, Creature, EmoteID)
                Else
                    With c.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_EMOTE AndAlso .ObjectivesSpell(j) = EmoteID Then
                                'NOTE: GameObjects are negative here!
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = Creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddEmote(c, j)
                                        Exit Sub
                                    End If
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i
    End Sub

    Public Function IsItemNeededForQuest(ByRef c As CharacterObject, ByRef ItemEntry As Integer) As Boolean
        Dim j As Integer, k As Byte, IsRaid As Boolean

        'DONE: Check if anyone in the group has the quest that requires this item
        'DONE: If the quest isn't a raid quest then you can't loot quest items
        IsRaid = c.IsInRaid
        If c.IsInGroup Then
            For Each GUID As ULong In c.Group.LocalMembers
                With CHARACTERs(GUID)

                    For j = 0 To QUEST_SLOTS
                        If (Not .TalkQuests(j) Is Nothing) AndAlso (.TalkQuests(j).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) AndAlso IsRaid = False Then
                            With .TalkQuests(j)
                                For k = 0 To 3
                                    If .ObjectivesItem(k) = ItemEntry Then
                                        If .ProgressItem(k) < .ObjectivesItemCount(k) Then Return True
                                    End If
                                Next
                            End With
                        End If
                    Next
                End With
            Next

        Else
            For j = 0 To QUEST_SLOTS
                If (Not c.TalkQuests(j) Is Nothing) AndAlso (c.TalkQuests(j).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                    With c.TalkQuests(j)
                        For k = 0 To 3
                            If .ObjectivesItem(k) = ItemEntry Then
                                If .ProgressItem(k) < .ObjectivesItemCount(k) Then Return True
                            End If
                        Next
                    End With
                End If
            Next
        End If

        Return False
    End Function

    Public Function IsGameObjectUsedForQuest(ByRef Gameobject As GameObjectObject, ByRef c As CharacterObject) As Byte
        If Not Gameobject.IsUsedForQuests Then Return 0

        Dim i As Integer, j As Byte
        For Each QuestItemID As Integer In Gameobject.IncludesQuestItems
            'DONE: Check quests needing that item
            For i = 0 To QUEST_SLOTS
                If c.TalkQuests(i) IsNot Nothing AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                    For j = 0 To 3
                        If c.TalkQuests(i).ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM AndAlso c.TalkQuests(i).ObjectivesItem(j) = QuestItemID Then
                            If c.ItemCOUNT(QuestItemID) < c.TalkQuests(i).ObjectivesItemCount(j) Then Return 2
                        End If
                    Next
                End If
            Next i
        Next

        Return 1
    End Function

    'DONE: Quest's loot generation
    Public Sub OnQuestAddQuestLoot(ByRef c As CharacterObject, ByRef Creature As CreatureObject, ByRef Loot As LootObject)
        'HANDLERS: Added in loot generation sub

        'TODO: Check for quest loots for adding to looted creature
    End Sub
    Public Sub OnQuestAddQuestLoot(ByRef c As CharacterObject, ByRef GameObject As GameObjectObject, ByRef Loot As LootObject)
        'HANDLERS: None
        'TODO: Check for quest loots for adding to looted gameObject
    End Sub
    Public Sub OnQuestAddQuestLoot(ByRef c As CharacterObject, ByRef Character As CharacterObject, ByRef Loot As LootObject)
        'HANDLERS: None
        'TODO: Check for quest loots for adding to looted player (used only in battleground?)
    End Sub

    'DONE: Item quest events
    Public Sub OnQuestItemAdd(ByRef c As CharacterObject, ByVal ItemID As Integer, ByVal Count As Byte)
        'HANDLERS: Added to looting sub

        If Count = 0 Then Count = 1
        Dim i As Integer, j As Byte


        'DONE: Check quests needing that item
        For i = 0 To QUEST_SLOTS
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                If TypeOf c.TalkQuests(i) Is BaseQuestScripted Then
                    CType(c.TalkQuests(i), BaseQuestScripted).OnQuestItem(c, ItemID, Count)
                Else
                    With c.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesItem(j) = ItemID Then
                                If .ProgressItem(j) < .ObjectivesItemCount(j) Then
                                    .AddItem(c, j, Count)
                                    Exit Sub
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i
    End Sub
    Public Sub OnQuestItemRemove(ByRef c As CharacterObject, ByVal ItemID As Integer, ByVal Count As Byte)
        'HANDLERS: Added to delete sub

        If Count = 0 Then Count = 1
        Dim i As Integer, j As Byte


        'DONE: Check quests needing that item
        For i = 0 To QUEST_SLOTS
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                If TypeOf c.TalkQuests(i) Is BaseQuestScripted Then
                    CType(c.TalkQuests(i), BaseQuestScripted).OnQuestItem(c, ItemID, -Count)
                Else
                    With c.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesItem(j) = ItemID Then
                                If .ProgressItem(j) > 0 Then
                                    .RemoveItem(c, j, Count)
                                    Exit Sub
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i
    End Sub

    'DONE: Exploration quest events
    Public Sub OnQuestExplore(ByRef c As CharacterObject, ByVal AreaID As Integer)
        Dim i As Integer, j As Byte
        For i = 0 To QUEST_SLOTS
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_EXPLORE) Then
                If TypeOf c.TalkQuests(i) Is BaseQuestScripted Then
                    CType(c.TalkQuests(i), BaseQuestScripted).OnQuestExplore(c, AreaID)
                Else
                    With c.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesExplore(j) = AreaID Then
                                If .Explored = False Then
                                    .AddExplore(c)
                                    Exit Sub
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i
    End Sub


#End Region
#Region "Quests.OpcodeHandlers"

    Public Function ClassByQuestSort(ByVal QuestSort As Integer) As Byte
        Select Case QuestSort
            Case 61
                Return Classes.CLASS_WARLOCK
            Case 81
                Return Classes.CLASS_WARRIOR
            Case 82
                Return Classes.CLASS_SHAMAN
            Case 141
                Return Classes.CLASS_PALADIN
            Case 161
                Return Classes.CLASS_MAGE
            Case 162
                Return Classes.CLASS_ROGUE
            Case 261
                Return Classes.CLASS_HUNTER
            Case 262
                Return Classes.CLASS_PRIEST
            Case 263
                Return Classes.CLASS_DRUID
            Case Else
                Return 0
        End Select
    End Function

    Public Function GetQuestgiverStatus(ByVal c As CharacterObject, ByVal cGUID As ULong) As QuestgiverStatus
        'DONE: Invoke scripted quest status
        Dim Status As QuestgiverStatus = QuestgiverStatus.DIALOG_STATUS_NONE
        'DONE: Do search for completed quests or in progress
        Dim i As Integer
        Dim alreadyHave As New List(Of Integer)
        Try
            If GuidIsCreature(cGUID) Then
                If WORLD_CREATUREs.ContainsKey(cGUID) = False Then Return QuestgiverStatus.DIALOG_STATUS_NONE
                If WORLD_CREATUREs(cGUID).CreatureInfo.TalkScript IsNot Nothing Then Status = WORLD_CREATUREs(cGUID).CreatureInfo.TalkScript.OnQuestStatus(c, cGUID)
            ElseIf GuidIsGameObject(cGUID) Then
                If WORLD_GAMEOBJECTs.ContainsKey(cGUID) = False Then Return QuestgiverStatus.DIALOG_STATUS_NONE
            Else
                Return QuestgiverStatus.DIALOG_STATUS_NONE
            End If

            For i = 0 To QUEST_SLOTS
                If c.TalkQuests(i) IsNot Nothing Then
                    alreadyHave.Add(c.TalkQuests(i).ID)
                    If GuidIsCreature(cGUID) Then
                        If CreatureQuestFinishers.ContainsKey(WORLD_CREATUREs(cGUID).ID) AndAlso CreatureQuestFinishers(WORLD_CREATUREs(cGUID).ID).Contains(c.TalkQuests(i).ID) Then
                            If c.TalkQuests(i).Complete Then
                                Return QuestgiverStatus.DIALOG_STATUS_REWARD
                            End If
                            Status = QuestgiverStatus.DIALOG_STATUS_INCOMPLETE
                        End If
                    Else
                        If GameobjectQuestFinishers.ContainsKey(WORLD_GAMEOBJECTs(cGUID).ID) AndAlso GameobjectQuestFinishers(WORLD_GAMEOBJECTs(cGUID).ID).Contains(c.TalkQuests(i).ID) Then
                            If c.TalkQuests(i).Complete Then
                                Return QuestgiverStatus.DIALOG_STATUS_REWARD
                            End If
                            Status = QuestgiverStatus.DIALOG_STATUS_INCOMPLETE
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            If Status = QuestgiverStatus.DIALOG_STATUS_NONE OrElse Status = QuestgiverStatus.DIALOG_STATUS_INCOMPLETE Then
                Dim questList As List(Of Integer) = Nothing
                If GuidIsCreature(cGUID) Then
                    Dim CreatureEntry As Integer = WORLD_CREATUREs(cGUID).ID
                    If CreatureQuestStarters.ContainsKey(CreatureEntry) Then
                        questList = CreatureQuestStarters(CreatureEntry)
                    End If
                Else
                    Dim GOEntry As Integer = WORLD_GAMEOBJECTs(cGUID).ID
                    If GameobjectQuestStarters.ContainsKey(GOEntry) Then
                        questList = GameobjectQuestStarters(GOEntry)
                    End If
                End If

                If questList IsNot Nothing Then
                    For Each QuestID As Integer In questList
                        If alreadyHave.Contains(QuestID) Then Continue For
                        'If QUESTs.ContainsKey(QuestID) = False Then Dim tmpQuest As New QuestInfo(QuestID)

                        If QUESTs(QuestID).CanSeeQuest(c) Then
                            If QUESTs(QuestID).SatisfyQuestLevel(c) Then
                                If QUESTs(QuestID).Level_Normal = -1 OrElse c.Level < (QUESTs(QuestID).Level_Normal + 6) Then
                                    Return QuestgiverStatus.DIALOG_STATUS_AVAILABLE
                                Else
                                    If Status = QuestgiverStatus.DIALOG_STATUS_NONE Then
                                        Status = QuestgiverStatus.DIALOG_STATUS_CHAT
                                    End If
                                End If
                            Else
                                Status = QuestgiverStatus.DIALOG_STATUS_UNAVAILABLE
                            End If
                        End If
                    Next
                End If
            End If
        End Try
        Return Status
    End Function
    Public Sub On_CMSG_QUESTGIVER_STATUS_QUERY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Try
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64()

            Dim status As QuestgiverStatus = GetQuestgiverStatus(Client.Character, GUID)

            Dim response As New PacketClass(OPCODES.SMSG_QUESTGIVER_STATUS)
            response.AddUInt64(GUID)
            response.AddInt32(status)
            Client.Send(response)
            response.Dispose()
        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "Error in questgiver status query.{0}", vbNewLine & e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_QUESTGIVER_HELLO(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Try
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_HELLO [GUID={2:X}]", Client.IP, Client.Port, GUID)
            If WORLD_CREATUREs(GUID).Evade Then Exit Sub

            WORLD_CREATUREs(GUID).StopMoving()
            Client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TALK)

            'TODO: There is something hare not working all the time :/
            If CREATURESDatabase(WORLD_CREATUREs(GUID).ID).TalkScript Is Nothing Then
                SendQuestMenu(Client.Character, GUID, "I have some tasks for you, $N.")
            Else
                CREATURESDatabase(WORLD_CREATUREs(GUID).ID).TalkScript.OnGossipHello(Client.Character, GUID)
            End If
        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "Error when sending quest menu.{0}", vbNewLine & e.ToString)
        End Try
    End Sub
    Public Sub On_CMSG_QUESTGIVER_QUERY_QUEST(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim QuestID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_QUERY_QUEST [GUID={2:X} QuestID={3}]", Client.IP, Client.Port, GUID, QuestID)

        If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)

        Try
            Client.Character.TalkCurrentQuest = QUESTs(QuestID)
            SendQuestDetails(Client, Client.Character.TalkCurrentQuest, GUID, True)
        Catch ex As Exception
            Log.WriteLine(LogType.CRITICAL, "Error while querying a quest.{0}{1}", vbNewLine, ex.ToString)
        End Try
    End Sub
    Public Sub On_CMSG_QUESTGIVER_ACCEPT_QUEST(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim QuestID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_ACCEPT_QUEST [GUID={2:X} QuestID={3}]", Client.IP, Client.Port, GUID, QuestID)

        If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)

        'Load quest data
        If Client.Character.TalkCurrentQuest.ID <> QuestID Then Client.Character.TalkCurrentQuest = QUESTs(QuestID)

        If Client.Character.TalkCanAccept(Client.Character.TalkCurrentQuest) Then
            If Client.Character.TalkAddQuest(Client.Character.TalkCurrentQuest) Then
                If GuidIsPlayer(GUID) Then
                    Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                    response.AddUInt64(Client.Character.GUID)
                    response.AddInt8(QuestPartyPushError.QUEST_PARTY_MSG_ACCEPT_QUEST)
                    response.AddInt32(0)
                    CHARACTERs(GUID).Client.Send(response)
                    response.Dispose()
                Else
                    Dim status As QuestgiverStatus = GetQuestgiverStatus(Client.Character, GUID)
                    Dim response As New PacketClass(OPCODES.SMSG_QUESTGIVER_STATUS)
                    response.AddUInt64(GUID)
                    response.AddInt32(status)
                    Client.Send(response)
                    response.Dispose()
                End If
            Else
                Dim response As New PacketClass(OPCODES.SMSG_QUESTLOG_FULL)
                Client.Send(response)
                response.Dispose()
            End If
        End If
    End Sub
    Public Sub On_CMSG_QUESTLOG_REMOVE_QUEST(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim Slot As Byte = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTLOG_REMOVE_QUEST [Slot={2}]", Client.IP, Client.Port, Slot)

        Client.Character.TalkDeleteQuest(Slot)
    End Sub

    Public Sub On_CMSG_QUEST_QUERY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim QuestID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUEST_QUERY [QuestID={2}]", Client.IP, Client.Port, QuestID)

        If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)

        If Client.Character.TalkCurrentQuest Is Nothing Then
            SendQuest(Client, QUESTs(QuestID))
            Exit Sub
        End If

        If Client.Character.TalkCurrentQuest.ID = QuestID Then
            SendQuest(Client, Client.Character.TalkCurrentQuest)
        Else
            SendQuest(Client, QUESTs(QuestID))
        End If
    End Sub

    Public Sub CompleteQuest(ByVal c As CharacterObject, ByVal QuestID As Integer, ByVal QuestGiverGUID As ULong)
        If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)
        Dim i As Integer
        For i = 0 To QUEST_SLOTS
            If Not c.TalkQuests(i) Is Nothing Then
                If c.TalkQuests(i).ID = QuestID Then

                    'Load quest data
                    If c.TalkCurrentQuest Is Nothing Then c.TalkCurrentQuest = QUESTs(QuestID)
                    If c.TalkCurrentQuest.ID <> QuestID Then c.TalkCurrentQuest = QUESTs(QuestID)


                    If c.TalkQuests(i).Complete Then
                        'DONE: Show completion dialog
                        If (c.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                            'Request items
                            SendQuestRequireItems(c.Client, c.TalkCurrentQuest, QuestGiverGUID, c.TalkQuests(i))
                        Else
                            SendQuestReward(c.Client, c.TalkCurrentQuest, QuestGiverGUID, c.TalkQuests(i))
                        End If
                    Else
                        'DONE: Just show incomplete text with disabled complete button
                        SendQuestRequireItems(c.Client, c.TalkCurrentQuest, QuestGiverGUID, c.TalkQuests(i))
                    End If


                    Exit For
                End If
            End If
        Next
    End Sub
    Public Sub On_CMSG_QUESTGIVER_COMPLETE_QUEST(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim QuestID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_COMPLETE_QUEST [GUID={2:X} Quest={3}]", Client.IP, Client.Port, GUID, QuestID)

        CompleteQuest(Client.Character, QuestID, GUID)
    End Sub
    Public Sub On_CMSG_QUESTGIVER_REQUEST_REWARD(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim QuestID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_REQUEST_REWARD [GUID={2:X} Quest={3}]", Client.IP, Client.Port, GUID, QuestID)

        If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)

        For i As Integer = 0 To QUEST_SLOTS
            If Client.Character.TalkQuests(i) IsNot Nothing AndAlso Client.Character.TalkQuests(i).ID = QuestID AndAlso Client.Character.TalkQuests(i).Complete Then

                'Load quest data
                If Client.Character.TalkCurrentQuest.ID <> QuestID Then Client.Character.TalkCurrentQuest = QUESTs(QuestID)
                SendQuestReward(Client, Client.Character.TalkCurrentQuest, GUID, Client.Character.TalkQuests(i))

                Exit For
            End If
        Next

    End Sub
    Public Sub On_CMSG_QUESTGIVER_CHOOSE_REWARD(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim QuestID As Integer = packet.GetInt32
        Dim RewardIndex As Integer = packet.GetInt32
        Dim i As Integer

        If Not QUESTs.ContainsKey(QuestID) Then Dim tmpQuest As New QuestInfo(QuestID)

        Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_CHOOSE_REWARD [GUID={2:X} Quest={3} Reward={4}]", Client.IP, Client.Port, GUID, QuestID, RewardIndex)
            If WORLD_CREATUREs.ContainsKey(GUID) = False Then Exit Sub

            'Load quest data
            If Client.Character.TalkCurrentQuest Is Nothing Then Client.Character.TalkCurrentQuest = QUESTs(QuestID)
            If Client.Character.TalkCurrentQuest.ID <> QuestID Then Client.Character.TalkCurrentQuest = QUESTs(QuestID)

            'DONE: Removing required gold
            If Client.Character.TalkCurrentQuest.RewardGold < 0 Then
                If (-Client.Character.TalkCurrentQuest.RewardGold) <= Client.Character.Copper Then
                    'NOTE: Update flag set below
                    'NOTE: Negative reward gold is required gold, that's why this should be plus
                    Client.Character.Copper += Client.Character.TalkCurrentQuest.RewardGold
                Else
                    Dim errorPacket As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                    errorPacket.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_MONEY)
                    Client.Send(errorPacket)
                    errorPacket.Dispose()
                    Exit Sub
                End If
            End If

            'DONE: Removing required items
            For i = 0 To QUEST_OBJECTIVES_COUNT
                If Client.Character.TalkCurrentQuest.ObjectivesItem(i) <> 0 Then
                    If Not Client.Character.ItemCONSUME(Client.Character.TalkCurrentQuest.ObjectivesItem(i), Client.Character.TalkCurrentQuest.ObjectivesItem_Count(i)) Then
                        'DONE: Restore gold
                        If Client.Character.TalkCurrentQuest.RewardGold < 0 Then
                            'NOTE: Negative reward gold is required gold, that's why this should be minus
                            Client.Character.Copper -= Client.Character.TalkCurrentQuest.RewardGold
                        End If
                        'TODO: Restore items (not needed?)
                        Dim errorPacket As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                        errorPacket.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_ITEMS)
                        Client.Send(errorPacket)
                        errorPacket.Dispose()
                        Exit Sub
                    End If
                Else
                    Exit For
                End If
            Next


            'DONE: Adding reward choice
            If Client.Character.TalkCurrentQuest.RewardItems(RewardIndex) <> 0 Then
                Dim tmpItem As New ItemObject(Client.Character.TalkCurrentQuest.RewardItems(RewardIndex), Client.Character.GUID)
                tmpItem.StackCount = Client.Character.TalkCurrentQuest.RewardItems_Count(RewardIndex)
                If Not Client.Character.ItemADD(tmpItem) Then
                    tmpItem.Delete()
                    'DONE: Inventory full sent form SetItemSlot
                    Exit Sub
                Else
                    Client.Character.LogLootItem(tmpItem, 1, True, False)
                End If
            End If

            'DONE: Adding gold
            If Client.Character.TalkCurrentQuest.RewardGold > 0 Then
                Client.Character.Copper += Client.Character.TalkCurrentQuest.RewardGold
            End If
            Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Client.Character.Copper)

            'DONE: Add honor
            If Client.Character.TalkCurrentQuest.RewardHonor <> 0 Then
                Client.Character.HonorPoints += Client.Character.TalkCurrentQuest.RewardHonor
                'Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_HONOR_CURRENCY, Client.Character.HonorCurrency)
            End If

            'DONE: Cast spell
            If Client.Character.TalkCurrentQuest.RewardSpell > 0 Then
                Dim spellTargets As New SpellTargets
                spellTargets.SetTarget_UNIT(Client.Character)

                Dim castParams As New CastSpellParameters(spellTargets, WORLD_CREATUREs(GUID), Client.Character.TalkCurrentQuest.RewardSpell, True)
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
            End If

            'DONE: Remove quest
            For i = 0 To QUEST_SLOTS
                If Not Client.Character.TalkQuests(i) Is Nothing Then
                    If Client.Character.TalkQuests(i).ID = Client.Character.TalkCurrentQuest.ID Then
                        Client.Character.TalkCompleteQuest(i)
                        Exit For
                    End If
                End If
            Next

            'DONE: XP Calculations
            Dim xp As Integer = 0
            Dim gold As Integer = Client.Character.TalkCurrentQuest.RewardGold
            If Client.Character.TalkCurrentQuest.RewMoneyMaxLevel > 0 Then
                Dim ReqMoneyMaxLevel As Integer = Client.Character.TalkCurrentQuest.RewMoneyMaxLevel
                Dim pLevel As Integer = Client.Character.Level
                Dim qLevel As Integer = Client.Character.TalkCurrentQuest.Level_Normal
                Dim fullxp As Single = 0.0F

                If pLevel <= MAX_LEVEL Then
                    If qLevel >= 65 Then
                        fullxp = ReqMoneyMaxLevel / 6.0F
                    ElseIf qLevel = 64 Then
                        fullxp = ReqMoneyMaxLevel / 4.8F
                    ElseIf qLevel = 63 Then
                        fullxp = ReqMoneyMaxLevel / 3.6F
                    ElseIf qLevel = 62 Then
                        fullxp = ReqMoneyMaxLevel / 2.4F
                    ElseIf qLevel = 61 Then
                        fullxp = ReqMoneyMaxLevel / 1.2F
                    ElseIf qLevel > 0 AndAlso qLevel <= 60 Then
                        fullxp = ReqMoneyMaxLevel / 0.6F
                    End If

                    If pLevel <= (qLevel + 5) Then
                        xp = CInt(Fix(fullxp))
                    ElseIf pLevel = (qLevel + 6) Then
                        xp = CInt(Fix(fullxp * 0.8F))
                    ElseIf pLevel = (qLevel + 7) Then
                        xp = CInt(Fix(fullxp * 0.6F))
                    ElseIf pLevel = (qLevel + 8) Then
                        xp = CInt(Fix(fullxp * 0.4F))
                    ElseIf pLevel = (qLevel + 9) Then
                        xp = CInt(Fix(fullxp * 0.2F))
                    Else
                        xp = CInt(Fix(fullxp * 0.1F))
                    End If

                    'DONE: Adding XP
                    Client.Character.AddXP(xp, 0, 0, True)
                Else
                    gold += ReqMoneyMaxLevel
                End If
            End If

            If gold < 0 AndAlso (-gold) >= Client.Character.Copper Then
                Client.Character.Copper = 0
            Else
                Client.Character.Copper += gold
            End If
            Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Client.Character.Copper)
            Client.Character.SendCharacterUpdate()

            SendQuestComplete(Client, Client.Character.TalkCurrentQuest, xp, gold)

            'DONE: Follow-up quests (no requirements checked?)
            If Client.Character.TalkCurrentQuest.NextQuest <> 0 Then
                If Not QUESTs.ContainsKey(Client.Character.TalkCurrentQuest.NextQuest) Then Dim tmpQuest As New QuestInfo(Client.Character.TalkCurrentQuest.NextQuest)
                Client.Character.TalkCurrentQuest = QUESTs(Client.Character.TalkCurrentQuest.NextQuest)
                SendQuestDetails(Client, Client.Character.TalkCurrentQuest, GUID, True)
            End If

        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "Error while choosing reward.{0}", vbNewLine & e.ToString)
        End Try
    End Sub



    Const QUEST_SHARING_DISTANCE As Integer = 10
    Public Sub On_CMSG_PUSHQUESTTOPARTY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PUSHQUESTTOPARTY [{2}]", Client.IP, Client.Port, questID)

        If Client.Character.IsInGroup Then
            If Not QUESTs.ContainsKey(questID) Then Dim tmpQuest As New QuestInfo(questID)

            For Each GUID As ULong In Client.Character.Group.LocalMembers
                If GUID = Client.Character.GUID Then Continue For

                With CHARACTERs(GUID)

                    Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                    response.AddUInt64(GUID)
                    response.AddInt32(QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST)
                    response.AddInt8(0)
                    Client.Send(response)
                    response.Dispose()

                    Dim message As QuestPartyPushError = QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST

                    'DONE: Check distance and ...
                    If (Math.Sqrt((.positionX - Client.Character.positionX) ^ 2 + (.positionY - Client.Character.positionY) ^ 2) > QUEST_SHARING_DISTANCE) Then
                        message = QuestPartyPushError.QUEST_PARTY_MSG_TO_FAR
                    ElseIf .IsQuestInProgress(questID) Then
                        message = QuestPartyPushError.QUEST_PARTY_MSG_HAVE_QUEST
                    ElseIf .IsQuestCompleted(questID) Then
                        message = QuestPartyPushError.QUEST_PARTY_MSG_FINISH_QUEST
                    Else
                        If (.TalkCurrentQuest Is Nothing) OrElse (.TalkCurrentQuest.ID <> questID) Then .TalkCurrentQuest = QUESTs(questID)
                        If .TalkCanAccept(.TalkCurrentQuest) Then
                            SendQuestDetails(.Client, .TalkCurrentQuest, Client.Character.GUID, True)
                        Else
                            message = QuestPartyPushError.QUEST_PARTY_MSG_CANT_TAKE_QUEST
                        End If
                    End If


                    'DONE: Send error if present
                    If message <> QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST Then
                        Dim errorPacket As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                        errorPacket.AddUInt64(.GUID)
                        errorPacket.AddInt32(message)
                        errorPacket.AddInt8(0)
                        Client.Send(errorPacket)
                        errorPacket.Dispose()
                    End If

                End With
            Next

        End If
    End Sub
    Public Sub On_MSG_QUEST_PUSH_RESULT(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 14 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim Message As QuestPartyPushError = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_QUEST_PUSH_RESULT [{2:X} {3}]", Client.IP, Client.Port, GUID, Message)

        'Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
        'response.AddUInt64(GUID)
        'response.AddInt8(QuestPartyPushError.QUEST_PARTY_MSG_ACCEPT_QUEST)
        'response.AddInt32(0)
        'Client.Send(response)
        'response.Dispose()
    End Sub


#End Region



End Module
