Imports mangosVB.Common.BaseWriter
Imports mangosVB.WorldServer.WS_QuestSystem

Public Class WS_Quests

#Region "Quests.HelpingSubs"
    Private ReadOnly _quests As Collection = New Collection

    ''' <summary>
    ''' Loads All the quests into the collection.
    ''' </summary>
    Public Sub LoadAllQuests()
        Dim cQuests As New DataTable
        Dim tmpQuest As WS_QuestInfo

        Log.WriteLine(LogType.WARNING, "Loading Quests...")
        WorldDatabase.Query(String.Format("SELECT entry FROM quests;"), cQuests)

        Dim i As Integer = 0

        For Each cRow As DataRow In cQuests.Rows
            Dim questID As Integer = CInt(cRow.Item("entry"))
            tmpQuest = New WS_QuestInfo(questID)

            _quests.Add(tmpQuest, questID)
        Next
        Log.WriteLine(LogType.WARNING, "Loading Quests...Complete")

    End Sub

    ''' <summary>
    ''' Returns the first Quest Id found when passed the quest name
    ''' </summary>
    ''' <param name="searchValue"></param>
    ''' <returns>QuestId (Int)</returns>
    ''' <remarks></remarks>
    Public Function ReturnQuestIdByName(ByVal searchValue As String) As Integer
        For Each thisService As WS_QuestInfo In _quests
            If thisService.Title = searchValue Then Return thisService.ID
        Next
        Return 0
    End Function

    Public Function DoesPreQuestExist(ByVal questID As Integer, ByVal preQuestID As Integer) As Boolean
        Dim ret As Boolean = False
        For Each thisService As WS_QuestInfo In _quests
            If thisService.ID = questID Then
                If thisService.PreQuests.Contains(preQuestID) = True Then
                    ret = True
                    Exit For
                End If
            End If
        Next
        Return ret
    End Function

    ''' <summary>
    ''' Returns whether the Quest Id is a valid quest
    ''' </summary>
    ''' <param name="questID"></param>
    ''' <returns>Bool</returns>
    ''' <remarks></remarks>    
    Public Function IsValidQuest(ByVal questID As Integer) As Boolean
        Dim ret As Boolean = False
        For Each thisQuest As WS_QuestInfo In _quests
            If thisQuest.ID = questID Then Return True
        Next
        Return ret
    End Function

    ''' <summary>
    ''' Returns the Quest Name found when passed the quest Id
    ''' </summary>
    ''' <param name="questId"></param>
    ''' <returns>QuestName (String)</returns>
    ''' <remarks></remarks>
    Public Function ReturnQuestNameById(ByVal questId As Integer) As String
        Dim ret As String = ""
        For Each thisQuest As WS_QuestInfo In _quests
            If thisQuest.ID = questId Then ret = thisQuest.Title
        Next
        Return ret
    End Function

    ''' <summary>
    ''' Returns the QuestInfo Structure  when passed the quest Id
    ''' </summary>
    ''' <param name="questId"></param>
    ''' <returns><c>WS_QuestInfo</c></returns>
    ''' <remarks></remarks>
    Public Function ReturnQuestInfoById(ByVal questId As Integer) As WS_QuestInfo
        Dim ret As WS_QuestInfo = Nothing
        Try
            For Each thisQuest As WS_QuestInfo In _quests
                If thisQuest.ID = questId Then
                    ret = thisQuest
                    Exit For
                End If
            Next
        Finally
        End Try
        Return ret
    End Function


    ''' Rewritten Code above this line


    ''' <summary>
    ''' Gets the quest menu.
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <returns></returns>
    Public Function GetQuestMenu(ByRef objChar As CharacterObject, ByVal GUID As ULong) As QuestMenu
        Dim QuestMenu As New QuestMenu
        Dim CreatureEntry As Integer = WORLD_CREATUREs(GUID).ID

        'DONE: Quests for completing
        Dim i As Integer
        Dim alreadyHave As New List(Of Integer)
        If CreatureQuestFinishers.ContainsKey(CreatureEntry) Then
            For i = 0 To QUEST_SLOTS
                If objChar.TalkQuests(i) IsNot Nothing Then
                    alreadyHave.Add(objChar.TalkQuests(i).ID)
                    If CreatureQuestFinishers(CreatureEntry).Contains(objChar.TalkQuests(i).ID) Then
                        QuestMenu.AddMenu(objChar.TalkQuests(i).Title, objChar.TalkQuests(i).ID, 0, WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE)
                    End If
                End If
            Next
        End If

        'DONE: Quests for taking
        If CreatureQuestStarters.ContainsKey(CreatureEntry) Then
            For Each QuestID As Integer In CreatureQuestStarters(CreatureEntry)
                If alreadyHave.Contains(QuestID) Then Continue For
                If Not ALLQUESTS.IsValidQuest(QuestID) Then
                    Try 'Sometimes Initialising Questinfo triggers an exception
                        Dim tmpQuest As New WS_QuestInfo(QuestID)
                        If tmpQuest.CanSeeQuest(objChar) Then
                            If tmpQuest.SatisfyQuestLevel(objChar) Then
                                QuestMenu.AddMenu(tmpQuest.Title, QuestID, tmpQuest.Level_Normal, WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                Else
                    If ALLQUESTS.ReturnQuestInfoById(QuestID).CanSeeQuest(objChar) Then
                        If ALLQUESTS.ReturnQuestInfoById(QuestID).SatisfyQuestLevel(objChar) Then
                            QuestMenu.AddMenu(ALLQUESTS.ReturnQuestInfoById(QuestID).Title, QuestID, ALLQUESTS.ReturnQuestInfoById(QuestID).Level_Normal, WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE)
                        End If
                    End If
                End If
            Next
        End If

        Return QuestMenu
    End Function

    ''' <summary>
    ''' Gets the quest menu go.
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <returns></returns>
    Public Function GetQuestMenuGO(ByRef objChar As CharacterObject, ByVal GUID As ULong) As QuestMenu
        Dim QuestMenu As New QuestMenu
        Dim GOEntry As Integer = WORLD_GAMEOBJECTs(GUID).ID

        'DONE: Quests for completing
        Dim i As Integer
        Dim alreadyHave As New List(Of Integer)
        If GameobjectQuestFinishers.ContainsKey(GOEntry) Then
            For i = 0 To QUEST_SLOTS
                If objChar.TalkQuests(i) IsNot Nothing Then
                    alreadyHave.Add(objChar.TalkQuests(i).ID)
                    If GameobjectQuestFinishers(GOEntry).Contains(objChar.TalkQuests(i).ID) Then
                        QuestMenu.AddMenu(objChar.TalkQuests(i).Title, objChar.TalkQuests(i).ID, 0, WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE)
                    End If
                End If
            Next
        End If

        'DONE: Quests for taking
        If GameobjectQuestStarters.ContainsKey(GOEntry) Then
            For Each QuestID As Integer In GameobjectQuestStarters(GOEntry)
                If alreadyHave.Contains(QuestID) Then Continue For
                If Not ALLQUESTS.IsValidQuest(QuestID) Then Dim tmpQuest As New WS_QuestInfo(QuestID)
                If ALLQUESTS.ReturnQuestInfoById(QuestID).CanSeeQuest(objChar) Then
                    If ALLQUESTS.ReturnQuestInfoById(QuestID).SatisfyQuestLevel(objChar) Then
                        QuestMenu.AddMenu(ALLQUESTS.ReturnQuestInfoById(QuestID).Title, QuestID, ALLQUESTS.ReturnQuestInfoById(QuestID).Level_Normal, WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE)
                    End If
                End If
            Next
        End If

        Return QuestMenu
    End Function

    ''' <summary>
    ''' Sends the quest menu.
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="Title">The title.</param>
    ''' <param name="QuestMenu">The quest menu.</param>
    Public Sub SendQuestMenu(ByRef objChar As CharacterObject, ByVal GUID As ULong, Optional ByVal Title As String = "Available quests", Optional ByVal QuestMenu As QuestMenu = Nothing)
        If QuestMenu Is Nothing Then
            QuestMenu = GetQuestMenu(objChar, GUID)
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
        objChar.Client.Send(packet)
        packet.Dispose()
    End Sub

    ''' <summary>
    ''' Sends the quest details.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="AcceptActive">if set to <c>true</c> [accept active].</param>
    Public Sub SendQuestDetails(ByRef client As ClientClass, ByRef Quest As WS_QuestInfo, ByVal GUID As ULong, ByVal AcceptActive As Boolean)
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

    ''' <summary>
    ''' Sends the quest.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    Public Sub SendQuest(ByRef client As ClientClass, ByRef Quest As WS_QuestInfo)
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

    ''' <summary>
    ''' Sends the quest message add item.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="itemID">The item identifier.</param>
    ''' <param name="itemCount">The item count.</param>
    Public Sub SendQuestMessageAddItem(ByRef client As ClientClass, ByVal itemID As Integer, ByVal itemCount As Integer)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_ADD_ITEM)
        packet.AddInt32(itemID)
        packet.AddInt32(itemCount)
        client.Send(packet)
        packet.Dispose()
    End Sub

    ''' <summary>
    ''' Sends the quest message add kill.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="questID">The quest identifier.</param>
    ''' <param name="killGUID">The kill unique identifier.</param>
    ''' <param name="killID">The kill identifier.</param>
    ''' <param name="killCurrentCount">The kill current count.</param>
    ''' <param name="killCount">The kill count.</param>
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

    ''' <summary>
    ''' Sends the quest message failed.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="QuestID">The quest identifier.</param>
    Public Sub SendQuestMessageFailed(ByRef client As ClientClass, ByVal QuestID As Integer)
        'Message: ?
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_FAILED)
        packet.AddInt32(QuestID)
        ' TODO: Need to add failed reason to packet here
        client.Send(packet)
        packet.Dispose()
    End Sub

    ''' <summary>
    ''' Sends the quest message failed timer.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="QuestID">The quest identifier.</param>
    Public Sub SendQuestMessageFailedTimer(ByRef client As ClientClass, ByVal QuestID As Integer)
        'Message: ?
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_FAILEDTIMER)
        packet.AddInt32(QuestID)
        client.Send(packet)
        packet.Dispose()
    End Sub

    ''' <summary>
    ''' Sends the quest message complete.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="QuestID">The quest identifier.</param>
    Public Sub SendQuestMessageComplete(ByRef client As ClientClass, ByVal QuestID As Integer)
        'Message: Objective Complete.
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_COMPLETE)
        packet.AddInt32(QuestID)
        client.Send(packet)
        packet.Dispose()
    End Sub

    ''' <summary>
    ''' Sends the quest complete.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="XP">The xp.</param>
    ''' <param name="Gold">The gold.</param>
    Public Sub SendQuestComplete(ByRef client As ClientClass, ByRef Quest As WS_QuestInfo, ByVal XP As Integer, ByVal Gold As Integer)
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

    ''' <summary>
    ''' Sends the quest reward.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="objBaseQuest">The Base Quest.</param>
    Public Sub SendQuestReward(ByRef client As ClientClass, ByRef Quest As WS_QuestInfo, ByVal GUID As ULong, ByRef objBaseQuest As WS_QuestsBase)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_OFFER_REWARD)

        packet.AddUInt64(GUID)
        packet.AddInt32(objBaseQuest.ID)
        packet.AddString(objBaseQuest.Title)
        packet.AddString(Quest.TextComplete)

        packet.AddInt32(CType(objBaseQuest.Complete, Integer))     'EnbleNext

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

    ''' <summary>
    ''' Sends the quest required items.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="objBaseQuest">The Base Quests.</param>
    Public Sub SendQuestRequireItems(ByRef client As ClientClass, ByRef Quest As WS_QuestInfo, ByVal GUID As ULong, ByRef objBaseQuest As WS_QuestsBase)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_REQUEST_ITEMS)

        packet.AddUInt64(GUID)
        packet.AddInt32(objBaseQuest.ID)
        packet.AddString(objBaseQuest.Title)
        packet.AddString(Quest.TextIncomplete)
        packet.AddInt32(0) 'Unknown

        If objBaseQuest.Complete Then
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
        If objBaseQuest.Complete Then
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

    ''' <summary>
    ''' Loads the quests.
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    Public Sub LoadQuests(ByRef objChar As CharacterObject)
        Dim cQuests As New DataTable
        CharacterDatabase.Query(String.Format("SELECT quest_id, quest_status FROM characters_quests q WHERE q.char_guid = {0};", objChar.GUID), cQuests)

        Dim i As Integer = 0
        For Each cRow As DataRow In cQuests.Rows
            Dim questID As Integer = CInt(cRow.Item("quest_id"))
            Dim questStatus As Integer = cRow.Item("quest_status")
            If questStatus >= 0 Then    'Outstanding Quest

                If IsValidQuest(questID) = True Then
                    Dim tmpQuest As WS_QuestInfo
                    tmpQuest = ReturnQuestInfoById(questID)


                    'DONE: Initialize quest info
                    CreateQuest(objChar.TalkQuests(i), tmpQuest)

                    objChar.TalkQuests(i).LoadState(questStatus)
                    objChar.TalkQuests(i).Slot = i
                    objChar.TalkQuests(i).UpdateItemCount(objChar)

                    i += 1
                End If
            ElseIf questStatus = -1 Then 'Completed
                objChar.QuestsCompleted.Add(questID)
            End If
        Next

    End Sub

    ''' <summary>
    ''' Creates the quest.
    ''' </summary>
    ''' <param name="objBaseQuest">The Base Quest.</param>
    ''' <param name="tmpQuest">The temporary quest.</param>
    Public Sub CreateQuest(ByRef objBaseQuest As WS_QuestsBase, ByRef tmpQuest As WS_QuestInfo)
        'Initialize Quest
        objBaseQuest = New WS_QuestsBase(tmpQuest)
    End Sub

#End Region

#Region "Quests.Events"


    'DONE: Kill quest events
    ''' <summary>
    ''' Called when [quest kill].
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="Creature">The creature.</param>
    Public Sub OnQuestKill(ByRef objChar As CharacterObject, ByRef Creature As CreatureObject)
        'HANDLERS: Added to DealDamage sub

        'DONE: Do not count killed from guards
        If objChar Is Nothing Then Exit Sub
        Dim i As Integer, j As Byte

        'DONE: Count kills
        For i = 0 To QUEST_SLOTS
            If (Not objChar.TalkQuests(i) Is Nothing) AndAlso (objChar.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_KILL) AndAlso (objChar.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) = 0 Then
                If TypeOf objChar.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objChar.TalkQuests(i), WS_QuestsBaseScripted).OnQuestKill(objChar, Creature)
                Else
                    With objChar.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesObject(j) = Creature.ID Then
                                If .Progress(j) < .ObjectivesCount(j) Then
                                    .AddKill(objChar, j, Creature.GUID)
                                    Exit Sub
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i


        Exit Sub  'For now next is disabled

        'DONE: Check all in objChar's party for that quest
        For Each GUID As ULong In objChar.Group.LocalMembers
            If GUID = objChar.GUID Then Continue For

            With CHARACTERs(GUID)
                For i = 0 To QUEST_SLOTS
                    If (Not .TalkQuests(i) Is Nothing) AndAlso (.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_KILL) AndAlso (.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) = 0 Then
                        With .TalkQuests(i)
                            For j = 0 To 3
                                If .ObjectivesType(j) = WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesObject(j) = Creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddKill(objChar, j, Creature.GUID)
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

    ''' <summary>
    ''' Called when [quest cast spell].
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="Creature">The creature.</param>
    ''' <param name="SpellID">The spell identifier.</param>
    Public Sub OnQuestCastSpell(ByRef objChar As CharacterObject, ByRef Creature As CreatureObject, ByVal SpellID As Integer)
        Dim i As Integer, j As Byte

        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct creature
        For i = 0 To QUEST_SLOTS
            If (Not objChar.TalkQuests(i) Is Nothing) AndAlso (objChar.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) Then
                If TypeOf objChar.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objChar.TalkQuests(i), WS_QuestsBaseScripted).OnQuestCastSpell(objChar, Creature, SpellID)
                Else
                    With objChar.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesSpell(j) = SpellID Then
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = Creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddCast(objChar, j, Creature.GUID)
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

    ''' <summary>
    ''' Called when [quest cast spell].
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="GameObject">The game object.</param>
    ''' <param name="SpellID">The spell identifier.</param>
    Public Sub OnQuestCastSpell(ByRef objChar As CharacterObject, ByRef GameObject As GameObjectObject, ByVal SpellID As Integer)
        Dim i As Integer, j As Byte

        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct gameobject
        For i = 0 To QUEST_SLOTS
            If (Not objChar.TalkQuests(i) Is Nothing) AndAlso (objChar.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) Then
                If TypeOf objChar.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objChar.TalkQuests(i), WS_QuestsBaseScripted).OnQuestCastSpell(objChar, GameObject, SpellID)
                Else
                    With objChar.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesSpell(j) = SpellID Then
                                'NOTE: GameObjects are negative here!
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = -(GameObject.ID) Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddCast(objChar, j, GameObject.GUID)
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

    ''' <summary>
    ''' Called when [quest do emote].
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="Creature">The creature.</param>
    ''' <param name="EmoteID">The emote identifier.</param>
    Public Sub OnQuestDoEmote(ByRef objChar As CharacterObject, ByRef Creature As CreatureObject, ByVal EmoteID As Integer)
        Dim i As Integer, j As Byte

        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct gameobject
        For i = 0 To QUEST_SLOTS
            If (Not objChar.TalkQuests(i) Is Nothing) AndAlso (objChar.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_EMOTE) Then
                If TypeOf objChar.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objChar.TalkQuests(i), WS_QuestsBaseScripted).OnQuestEmote(objChar, Creature, EmoteID)
                Else
                    With objChar.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_EMOTE AndAlso .ObjectivesSpell(j) = EmoteID Then
                                'NOTE: GameObjects are negative here!
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = Creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddEmote(objChar, j)
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

    ''' <summary>
    ''' Determines whether the item is needed for quest for the specified character.
    ''' </summary>
    ''' <param name="objChar">The Character.</param>
    ''' <param name="ItemEntry">The item entry.</param>
    ''' <returns></returns>
    Public Function IsItemNeededForQuest(ByRef objChar As CharacterObject, ByRef ItemEntry As Integer) As Boolean
        Dim j As Integer, k As Byte, IsRaid As Boolean

        'DONE: Check if anyone in the group has the quest that requires this item
        'DONE: If the quest isn't a raid quest then you can't loot quest items
        IsRaid = objChar.IsInRaid
        If objChar.IsInGroup Then
            For Each GUID As ULong In objChar.Group.LocalMembers
                With CHARACTERs(GUID)

                    For j = 0 To QUEST_SLOTS
                        If (Not .TalkQuests(j) Is Nothing) AndAlso (.TalkQuests(j).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) AndAlso IsRaid = False Then
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
                If (Not objChar.TalkQuests(j) Is Nothing) AndAlso (objChar.TalkQuests(j).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                    With objChar.TalkQuests(j)
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
                If c.TalkQuests(i) IsNot Nothing AndAlso (c.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                    For j = 0 To 3
                        If c.TalkQuests(i).ObjectivesType(j) = WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM AndAlso c.TalkQuests(i).ObjectivesItem(j) = QuestItemID Then
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
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                If TypeOf c.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(c.TalkQuests(i), WS_QuestsBaseScripted).OnQuestItem(c, ItemID, Count)
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
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                If TypeOf c.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(c.TalkQuests(i), WS_QuestsBaseScripted).OnQuestItem(c, ItemID, -Count)
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
            If (Not c.TalkQuests(i) Is Nothing) AndAlso (c.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_EXPLORE) Then
                If TypeOf c.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(c.TalkQuests(i), WS_QuestsBaseScripted).OnQuestExplore(c, AreaID)
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

    Public Function GetQuestgiverStatus(ByVal c As CharacterObject, ByVal cGUID As ULong) As WS_QuestSystem.QuestgiverStatusFlag
        'DONE: Invoke scripted quest status
        Dim Status As WS_QuestSystem.QuestgiverStatusFlag = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_NONE
        'DONE: Do search for completed quests or in progress

        Dim alreadyHave As New List(Of Integer)

        If GuidIsCreature(cGUID) = True Then    'Is the GUID a creature (or npc)
            If WORLD_CREATUREs.ContainsKey(cGUID) = False Then
                Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_NONE
                Return Status
            End If

            Log.WriteLine(LogType.CRITICAL, "Status = {0} {1} {2}", WORLD_CREATUREs(cGUID).ID, WORLD_CREATUREs(cGUID).Name, IsNothing(WORLD_CREATUREs(cGUID).CreatureInfo.TalkScript))
            ' Log.WriteLine(LogType.CRITICAL, "Status = {0} {1} {2}", WORLD_CREATUREs(cGUID).)
            '     If IsNothing(WORLD_CREATUREs(cGUID).CreatureInfo.TalkScript) = False Then    'NPC is a questgiven
            Dim CreatureQuestId As Integer
            CreatureQuestId = WORLD_CREATUREs(cGUID).ID
            If IsValidQuest(CreatureQuestId) = True Then
                If CreatureQuestStarters.ContainsKey(CreatureQuestId) = True Then
                    For Each QuestID As Integer In CreatureQuestStarters(CreatureQuestId)
                        Try
                            If ALLQUESTS.ReturnQuestInfoById(QuestID).CanSeeQuest(c) = True Then
                                Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE
                                Return Status
                            End If
                        Catch ex As Exception

                        End Try
                    Next
                End If
            End If
            'If WORLD_CREATUREs(cGUID).CreatureInfo.Id
            'IF cannot see quest, run line below
            Status = WORLD_CREATUREs(cGUID).CreatureInfo.TalkScript.OnQuestStatus(c, cGUID)
            Return Status
            'End If

        ElseIf GuidIsGameObject(cGUID) = True Then  'Or is it a worldobject
            If WORLD_GAMEOBJECTs.ContainsKey(cGUID) = False Then
                Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_NONE
                Return Status

            End If
        Else        'everything else doesn't get a marker
            Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_NONE
            Return Status

        End If

        For i As Integer = 0 To QUEST_SLOTS
            If c.TalkQuests(i) IsNot Nothing Then
                alreadyHave.Add(c.TalkQuests(i).ID)
                If GuidIsCreature(cGUID) Then
                    If CreatureQuestFinishers.ContainsKey(WORLD_CREATUREs(cGUID).ID) AndAlso CreatureQuestFinishers(WORLD_CREATUREs(cGUID).ID).Contains(c.TalkQuests(i).ID) Then
                        If c.TalkQuests(i).Complete Then
                            Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_REWARD
                            Exit For
                        End If
                        Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE
                    End If
                Else
                    If GameobjectQuestFinishers.ContainsKey(WORLD_GAMEOBJECTs(cGUID).ID) AndAlso GameobjectQuestFinishers(WORLD_GAMEOBJECTs(cGUID).ID).Contains(c.TalkQuests(i).ID) Then
                        If c.TalkQuests(i).Complete Then
                            Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_REWARD
                            Exit For
                        End If
                        Status = WS_QuestSystem.QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE
                    End If
                End If
            End If
        Next

        Return Status
    End Function
    Public Sub On_CMSG_QUESTGIVER_STATUS_QUERY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Try
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64()

            Dim status As QuestgiverStatusFlag = GetQuestgiverStatus(Client.Character, GUID)

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

        If Not ALLQUESTS.IsValidQuest(QuestID) Then Dim tmpQuest As New WS_QuestInfo(QuestID)

        Try
            Client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(QuestID)
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

        If Not ALLQUESTS.IsValidQuest(QuestID) Then Dim tmpQuest As New WS_QuestInfo(QuestID)

        'Load quest data
        If Client.Character.TalkCurrentQuest.ID <> QuestID Then Client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(QuestID)

        If Client.Character.TalkCanAccept(Client.Character.TalkCurrentQuest) Then
            If Client.Character.TalkAddQuest(Client.Character.TalkCurrentQuest) Then
                If GuidIsPlayer(GUID) Then
                    Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                    response.AddUInt64(Client.Character.GUID)
                    response.AddInt8(WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_ACCEPT_QUEST)
                    response.AddInt32(0)
                    CHARACTERs(GUID).Client.Send(response)
                    response.Dispose()
                Else
                    Dim status As QuestgiverStatusFlag = GetQuestgiverStatus(Client.Character, GUID)
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

        If Not ALLQUESTS.IsValidQuest(QuestID) Then Dim tmpQuest As New WS_QuestInfo(QuestID)

        If Client.Character.TalkCurrentQuest Is Nothing Then
            SendQuest(Client, ALLQUESTS.ReturnQuestInfoById(QuestID))
            Exit Sub
        End If

        If Client.Character.TalkCurrentQuest.ID = QuestID Then
            SendQuest(Client, Client.Character.TalkCurrentQuest)
        Else
            SendQuest(Client, ALLQUESTS.ReturnQuestInfoById(QuestID))
        End If
    End Sub

    Public Sub CompleteQuest(ByVal c As CharacterObject, ByVal QuestID As Integer, ByVal QuestGiverGUID As ULong)
        If Not ALLQUESTS.IsValidQuest(QuestID) Then Dim tmpQuest As New WS_QuestInfo(QuestID)
        Dim i As Integer
        For i = 0 To QUEST_SLOTS
            If Not c.TalkQuests(i) Is Nothing Then
                If c.TalkQuests(i).ID = QuestID Then

                    'Load quest data
                    If c.TalkCurrentQuest Is Nothing Then c.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(QuestID)
                    If c.TalkCurrentQuest.ID <> QuestID Then c.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(QuestID)


                    If c.TalkQuests(i).Complete Then
                        'DONE: Show completion dialog
                        If (c.TalkQuests(i).ObjectiveFlags And WS_QuestSystem.QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
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

        If Not ALLQUESTS.IsValidQuest(QuestID) Then Dim tmpQuest As New WS_QuestInfo(QuestID)

        For i As Integer = 0 To QUEST_SLOTS
            If Client.Character.TalkQuests(i) IsNot Nothing AndAlso Client.Character.TalkQuests(i).ID = QuestID AndAlso Client.Character.TalkQuests(i).Complete Then

                'Load quest data
                If Client.Character.TalkCurrentQuest.ID <> QuestID Then Client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(QuestID)
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

        If Not ALLQUESTS.IsValidQuest(QuestID) Then Dim tmpQuest As New WS_QuestInfo(QuestID)

        Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_CHOOSE_REWARD [GUID={2:X} Quest={3} Reward={4}]", Client.IP, Client.Port, GUID, QuestID, RewardIndex)
            If WORLD_CREATUREs.ContainsKey(GUID) = False Then Exit Sub

            'Load quest data
            If Client.Character.TalkCurrentQuest Is Nothing Then Client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(QuestID)
            If Client.Character.TalkCurrentQuest.ID <> QuestID Then Client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(QuestID)

            'DONE: Removing required gold
            If Client.Character.TalkCurrentQuest.RewardGold < 0 Then
                If (-Client.Character.TalkCurrentQuest.RewardGold) <= Client.Character.Copper Then
                    'NOTE: Update flag set below
                    'NOTE: Negative reward gold is required gold, that's why this should be plus
                    Client.Character.Copper += Client.Character.TalkCurrentQuest.RewardGold
                Else
                    Dim errorPacket As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                    errorPacket.AddInt32(WS_QuestSystem.QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_MONEY)
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
                        errorPacket.AddInt32(WS_QuestSystem.QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_ITEMS)
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
                'If Not ALLQUESTS.IsValidQuest(Client.Character.TalkCurrentQuest.NextQuest) Then Dim tmpQuest As New WS_QuestInfo(Client.Character.TalkCurrentQuest.NextQuest)
                Client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(Client.Character.TalkCurrentQuest.NextQuest)
                SendQuestDetails(Client, Client.Character.TalkCurrentQuest, GUID, True)
            End If

        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "Error while choosing reward.{0}", vbNewLine & e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_PUSHQUESTTOPARTY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PUSHQUESTTOPARTY [{2}]", Client.IP, Client.Port, questID)

        If Client.Character.IsInGroup Then
            If Not ALLQUESTS.IsValidQuest(questID) Then Dim tmpQuest As New WS_QuestInfo(questID)

            For Each GUID As ULong In Client.Character.Group.LocalMembers
                If GUID = Client.Character.GUID Then Continue For

                With CHARACTERs(GUID)

                    Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                    response.AddUInt64(GUID)
                    response.AddInt32(WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST)
                    response.AddInt8(0)
                    Client.Send(response)
                    response.Dispose()

                    Dim message As WS_QuestSystem.QuestPartyPushError = WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST

                    'DONE: Check distance and ...
                    If (Math.Sqrt((.positionX - Client.Character.positionX) ^ 2 + (.positionY - Client.Character.positionY) ^ 2) > QUEST_SHARING_DISTANCE) Then
                        message = WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_TO_FAR
                    ElseIf .IsQuestInProgress(questID) Then
                        message = WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_HAVE_QUEST
                    ElseIf .IsQuestCompleted(questID) Then
                        message = WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_FINISH_QUEST
                    Else
                        If (.TalkCurrentQuest Is Nothing) OrElse (.TalkCurrentQuest.ID <> questID) Then .TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                        If .TalkCanAccept(.TalkCurrentQuest) Then
                            SendQuestDetails(.Client, .TalkCurrentQuest, Client.Character.GUID, True)
                        Else
                            message = WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_CANT_TAKE_QUEST
                        End If
                    End If


                    'DONE: Send error if present
                    If message <> WS_QuestSystem.QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST Then
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
        Dim Message As WS_QuestSystem.QuestPartyPushError = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_QUEST_PUSH_RESULT [{2:X} {3}]", Client.IP, Client.Port, GUID, Message)

        'Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
        'response.AddUInt64(GUID)
        'response.AddInt8(QuestPartyPushError.QUEST_PARTY_MSG_ACCEPT_QUEST)
        'response.AddInt32(0)
        'Client.Send(response)
        'response.Dispose()
    End Sub


#End Region

    Public Sub New()
        ' _quests(1) = New Dictionary(Of Integer, WS_QuestInfo)

    End Sub
End Class
