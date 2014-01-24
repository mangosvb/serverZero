'
' Copyright (C) 2013 - 2014 getMaNGOS <http://www.getMangos.co.uk>
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

Public Class WS_QuestInfo
    Implements IDisposable

    Public ID As Integer
    Public PreQuests As List(Of Integer)
    Public NextQuest As Integer ' = 0
    Public NextQuestInChain As Integer ' = 0
    Public Method As Byte ' = 0
    Public Type As Integer
    Public ZoneOrSort As Integer
    Public QuestFlags As Integer ' = 0
    Public SpecialFlags As Integer '= 0
    Public Level_Start As Byte ' = 0
    Public Level_Normal As Short '= 0

    Public Title As String '= ""
    Public TextObjectives As String '= ""
    Public TextDescription As String ' = ""
    Public TextEnd As String '= ""
    Public TextIncomplete As String '= ""
    Public TextComplete As String ' = ""

    Public RequiredRace As Integer
    Public RequiredClass As Integer
    Public RequiredTradeSkill As Integer
    Public RequiredTradeSkillValue As Integer
    Public RequiredMinReputation As Integer
    Public RequiredMinReputation_Faction As Integer
    Public RequiredMaxReputation As Integer
    Public RequiredMaxReputation_Faction As Integer

    Public RewardHonor As Integer '= 0
    Public RewardGold As Integer ' = 0
    Public RewMoneyMaxLevel As Integer '= 0
    Public RewardSpell As Integer ' = 0
    Public RewardSpellCast As Integer '= 0
    Public RewardItems(QuestInfo.QUEST_REWARD_CHOICES_COUNT) As Integer
    Public RewardItems_Count(QuestInfo.QUEST_REWARD_CHOICES_COUNT) As Integer
    Public RewardStaticItems(QuestInfo.QUEST_REWARDS_COUNT) As Integer
    Public RewardStaticItems_Count(QuestInfo.QUEST_REWARDS_COUNT) As Integer
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

    Public ObjectivesText() As String ' = {"", "", "", ""}

    Public TimeLimit As Integer ' = 0
    Public SourceSpell As Integer ' = 0

    Public PointMapID As Integer ' = 0
    Public PointX As Single '= 0
    Public PointY As Single '= 0
    Public PointOpt As Integer '= 0

    Public DetailsEmote(3) As Integer
    Public IncompleteEmote As Integer ' = 0
    Public CompleteEmote As Integer '= 0
    Public OfferRewardEmote(3) As Integer

    Public StartScript As Integer '= 0
    Public CompleteScript As Integer ' = 0

    Public Sub New(ByVal QuestID As Integer)
        'Initialise Varibles correctly here
        NextQuest = 0
        NextQuestInChain = 0
        Method = 0

        QuestFlags = 0
        SpecialFlags = 0
        Level_Start = 0
        Level_Normal = 0

        Title = ""
        TextObjectives = ""
        TextDescription = ""
        TextEnd = ""
        TextIncomplete = ""
        TextComplete = ""

        RewardHonor = 0
        RewardGold = 0
        RewMoneyMaxLevel = 0
        RewardSpell = 0
        RewardSpellCast = 0

        ObjectivesText = {"", "", "", ""}

        TimeLimit = 0
        SourceSpell = 0

        PointMapID = 0
        PointX = 0
        PointY = 0
        PointOpt = 0

        IncompleteEmote = 0
        CompleteEmote = 0

        StartScript = 0
        CompleteScript = 0

        ID = QuestID
        PreQuests = New List(Of Integer)
        Dim MySQLQuery As New DataTable

        '        Quests.Add(ID, Me)

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
    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        _disposedValue = True
        GC.Collect()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    ''' <summary>
    ''' Initializes the quest.
    ''' </summary>
    Private Sub InitQuest()
        If NextQuestInChain > 0 Then
            If ALLQUESTS.IsValidQuest(NextQuestInChain) = False Then
                Dim tmpQuest As New WS_QuestInfo(NextQuestInChain)
                If tmpQuest.PreQuests.Contains(ID) = False Then
                    Log.WriteLine(LogType.DEBUG, "Added prequest [{0}] to quest [{1}]", ID, NextQuestInChain) ', ALLQUESTS.ReturnQuestNameById(ID), ALLQUESTS.ReturnQuestNameById(NextQuestInChain))
                    tmpQuest.PreQuests.Add(ID)
                End If
            Else
                If ALLQUESTS.DoesPreQuestExist(NextQuestInChain, ID) = False Then
                    Log.WriteLine(LogType.DEBUG, "Added prequest [{0}] to quest [{1}]", NextQuestInChain, ID) ', ALLQUESTS.ReturnQuestNameById(NextQuestInChain), ALLQUESTS.ReturnQuestNameById(ID))
                    ALLQUESTS.ReturnQuestInfoById(NextQuestInChain).PreQuests.Add(ID)
                End If
            End If
        End If
        If NextQuest <> 0 Then
            Dim unsignedNextQuest As Integer = Math.Abs(NextQuest)
            Dim signedQuestID As Integer = If((NextQuest < 0), -ID, ID)
            If ALLQUESTS.IsValidQuest(unsignedNextQuest) = False Then
                Dim tmpQuest As New WS_QuestInfo(unsignedNextQuest)
                If tmpQuest.PreQuests.Contains(signedQuestID) = False Then
                    Log.WriteLine(LogType.DEBUG, "Added prequest [{0}] to quest [{1}]", signedQuestID, unsignedNextQuest) ', ALLQUESTS.ReturnQuestNameById(signedQuestID), ALLQUESTS.ReturnQuestNameById(unsignedNextQuest))
                    tmpQuest.PreQuests.Add(signedQuestID)
                End If
            Else
                If ALLQUESTS.DoesPreQuestExist(unsignedNextQuest, signedQuestID) = False Then
                    Log.WriteLine(LogType.DEBUG, "Added prequest [{0}] to quest [{1}]", signedQuestID, unsignedNextQuest) ', ALLQUESTS.ReturnQuestNameById(signedQuestID), ALLQUESTS.ReturnQuestNameById(unsignedNextQuest))
                    ALLQUESTS.ReturnQuestInfoById(unsignedNextQuest).PreQuests.Add(signedQuestID)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Determines whether this instance Character can see the quest.
    ''' </summary>
    ''' <param name="objCharacter">The objCharacter.</param>
    ''' <returns></returns>
    Public Function CanSeeQuest(ByRef objCharacter As CharacterObject) As Boolean
        Try
            If (CInt(objCharacter.Level) + 6) < Level_Start Then Return False
            If RequiredClass > 0 AndAlso RequiredClass <> objCharacter.Classe Then Return False
            If ZoneOrSort < 0 Then
                Dim tmpQuest As New WS_Quests
                Dim reqSort As Byte = tmpQuest.ClassByQuestSort(-ZoneOrSort)
                If reqSort > 0 AndAlso reqSort <> objCharacter.Classe Then Return False
            End If
            If RequiredRace <> 0 AndAlso (RequiredRace And objCharacter.RaceMask) = 0 Then Return False
            If RequiredTradeSkill > 0 Then
                If objCharacter.Skills.ContainsKey(RequiredTradeSkill) = False Then Return False
                If objCharacter.Skills(RequiredTradeSkill).Current < RequiredTradeSkillValue Then Return False
            End If
            If RequiredMinReputation_Faction > 0 AndAlso objCharacter.GetReputationValue(RequiredMinReputation_Faction) < RequiredMinReputation Then Return False
            If RequiredMaxReputation_Faction > 0 AndAlso objCharacter.GetReputationValue(RequiredMaxReputation_Faction) >= RequiredMaxReputation Then Return False
            Dim mysqlQuery As New DataTable
            If PreQuests.Count > 0 Then
                'Check if we have done the prequest
                For Each QuestID As Integer In PreQuests
                    If QuestID > 0 Then 'If we haven't done this prequest we can't do this quest
                        If objCharacter.QuestsCompleted.Contains(QuestID) = False Then Return False
                    ElseIf QuestID < 0 Then 'If we have done this prequest we can't do this quest
                        If objCharacter.QuestsCompleted.Contains(QuestID) Then Return False
                    End If
                Next
            End If
            If objCharacter.QuestsCompleted.Contains(ID) Then Return False 'We have already completed this quest
            Return True
        Catch ex As Exception
            Stop
        End Try
    End Function

    ''' <summary>
    ''' Satisfies the quest level.
    ''' </summary>
    ''' <param name="objCharacter">The Character.</param>
    ''' <returns>Boolean</returns>
    Public Function SatisfyQuestLevel(ByRef objCharacter As CharacterObject) As Boolean
        If objCharacter.Level < Level_Start Then Return False
        Return True
    End Function

End Class