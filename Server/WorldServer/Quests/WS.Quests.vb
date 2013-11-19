Imports mangosVB.Common.BaseWriter

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
            If thisService.Title = searchValue Then
                Return thisService.ID
            End If
        Next
        Return 0
    End Function

    ''' <summary>
    ''' Does the pre quest exist.
    ''' </summary>
    ''' <param name="questID">The quest ID.</param>
    ''' <param name="preQuestID">The pre quest ID.</param>
    ''' <returns></returns>
    Public Function DoesPreQuestExist(ByVal questID As Integer, ByVal preQuestID As Integer) As Boolean
        Dim ret As Boolean = False
        For Each thisService As WS_QuestInfo In _quests
            If thisService.ID = questID Then
                If thisService.PreQuests.Contains(preQuestID) = True Then
                    Return True
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
                    Return thisQuest
                End If
            Next
        Catch ex As Exception
            Log.WriteLine(LogType.WARNING, "ReturnQuestInfoById returned error on QuestId {0}", questId)
        End Try
        Return ret
    End Function

    ''' Rewritten Code above this line

    ''' <summary>
    ''' Gets the quest menu.
    ''' </summary>
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <returns></returns>
    Public Function GetQuestMenu(ByRef objCharacter As CharacterObject, ByVal guid As ULong) As QuestMenu
        Dim questMenu As New QuestMenu
        Dim creatureEntry As Integer = WORLD_CREATUREs(guid).ID

        'DONE: Quests for completing
        Dim alreadyHave As New List(Of Integer)
        If CreatureQuestFinishers.ContainsKey(creatureEntry) Then
            Try
                For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                    If objCharacter.TalkQuests(i) IsNot Nothing Then
                        alreadyHave.Add(objCharacter.TalkQuests(i).ID)
                        If CreatureQuestFinishers(creatureEntry).Contains(objCharacter.TalkQuests(i).ID) Then
                            questMenu.AddMenu(objCharacter.TalkQuests(i).Title, objCharacter.TalkQuests(i).ID, 0, QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE)
                        End If
                    End If
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "GetQuestMenu Failed: ", ex.ToString())
            End Try
        End If


        'DONE: Quests for taking
        If CreatureQuestStarters.ContainsKey(creatureEntry) Then
            Try
                For Each questID As Integer In CreatureQuestStarters(creatureEntry)
                    If alreadyHave.Contains(questID) Then Continue For
                    If Not ALLQUESTS.IsValidQuest(questID) Then
                        Try 'Sometimes Initialising Questinfo triggers an exception
                            Dim tmpQuest As New WS_QuestInfo(questID)
                            If tmpQuest.CanSeeQuest(objCharacter) Then
                                If tmpQuest.SatisfyQuestLevel(objCharacter) Then
                                    questMenu.AddMenu(tmpQuest.Title, questID, tmpQuest.Level_Normal, QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE)
                                End If
                            End If
                        Catch ex As Exception
                            Log.WriteLine(LogType.WARNING, "GetQuestMenu returned error for QuestId {0}", questID)
                        End Try
                    Else
                        If ALLQUESTS.ReturnQuestInfoById(questID).CanSeeQuest(objCharacter) Then
                            If ALLQUESTS.ReturnQuestInfoById(questID).SatisfyQuestLevel(objCharacter) Then
                                questMenu.AddMenu(ALLQUESTS.ReturnQuestInfoById(questID).Title, questID, ALLQUESTS.ReturnQuestInfoById(questID).Level_Normal, QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE)
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "GetQuestMenu Failed: ", ex.ToString())
            End Try

        End If

        Return questMenu
    End Function

    ''' <summary>
    ''' Gets the quest menu go.
    ''' </summary>
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <returns></returns>
    Public Function GetQuestMenuGO(ByRef objCharacter As CharacterObject, ByVal guid As ULong) As QuestMenu
        Dim questMenu As New QuestMenu
        Dim gOEntry As Integer = WORLD_GAMEOBJECTs(guid).ID

        'DONE: Quests for completing
        Dim alreadyHave As New List(Of Integer)
        If GameobjectQuestFinishers.ContainsKey(gOEntry) Then
            Try
                For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                    If objCharacter.TalkQuests(i) IsNot Nothing Then
                        alreadyHave.Add(objCharacter.TalkQuests(i).ID)
                        If GameobjectQuestFinishers(gOEntry).Contains(objCharacter.TalkQuests(i).ID) Then
                            questMenu.AddMenu(objCharacter.TalkQuests(i).Title, objCharacter.TalkQuests(i).ID, 0, QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE)
                        End If
                    End If
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "GetQuestMenuGO Failed: ", ex.ToString())
            End Try
        End If

        'DONE: Quests for taking
        If GameobjectQuestStarters.ContainsKey(gOEntry) Then
            Try
                For Each questID As Integer In GameobjectQuestStarters(gOEntry)
                    If alreadyHave.Contains(questID) Then Continue For
                    If Not ALLQUESTS.IsValidQuest(questID) Then
                        Dim tmpQuest As New WS_QuestInfo(questID)
                        If tmpQuest.CanSeeQuest(objCharacter) Then
                            If tmpQuest.SatisfyQuestLevel(objCharacter) Then
                                questMenu.AddMenu(tmpQuest.Title, questID, tmpQuest.Level_Normal, QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE)
                            End If
                        End If
                    Else
                        If ALLQUESTS.ReturnQuestInfoById(questID).CanSeeQuest(objCharacter) Then
                            If ALLQUESTS.ReturnQuestInfoById(questID).SatisfyQuestLevel(objCharacter) Then
                                questMenu.AddMenu(ALLQUESTS.ReturnQuestInfoById(questID).Title, questID, ALLQUESTS.ReturnQuestInfoById(questID).Level_Normal, QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE)
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "GetQuestMenuGO Failed: ", ex.ToString())
            End Try
        End If

        Return questMenu
    End Function

    ''' <summary>
    ''' Sends the quest menu.
    ''' </summary>
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="Title">The title.</param>
    ''' <param name="QuestMenu">The quest menu.</param>
    Public Sub SendQuestMenu(ByRef objCharacter As CharacterObject, ByVal guid As ULong, Optional ByVal title As String = "Available quests", Optional ByVal questMenu As QuestMenu = Nothing)
        If questMenu Is Nothing Then
            questMenu = GetQuestMenu(objCharacter, guid)
        End If

        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_LIST)
        Try
            packet.AddUInt64(guid)
            packet.AddString(title)
            packet.AddInt32(1)              'Delay
            packet.AddInt32(1)              'Emote
            packet.AddInt8(questMenu.IDs.Count) 'Count
            Try
                For i As Integer = 0 To questMenu.IDs.Count - 1
                    packet.AddInt32(questMenu.IDs(i))
                    packet.AddInt32(questMenu.Icons(i))
                    packet.AddInt32(questMenu.Levels(i))
                    packet.AddString(questMenu.Names(i))
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "GetQuestMenu Failed: ", ex.ToString())
            End Try
            objCharacter.client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest details.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="AcceptActive">if set to <c>true</c> [accept active].</param>
    Public Sub SendQuestDetails(ByRef client As ClientClass, ByRef quest As WS_QuestInfo, ByVal guid As ULong, ByVal acceptActive As Boolean)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_DETAILS)
        Try
            packet.AddUInt64(guid)

            'QuestDetails
            packet.AddInt32(quest.ID)
            packet.AddString(quest.Title)
            packet.AddString(quest.TextDescription)
            packet.AddString(quest.TextObjectives)
            packet.AddInt32(If(acceptActive, 1, 0))

            'QuestRewards (Choosable)
            Dim questRewardsCount As Integer = 0
            Try
                For i As Integer = 0 To QuestInfo.QUEST_REWARD_CHOICES_COUNT
                    If quest.RewardItems(i) <> 0 Then questRewardsCount += 1
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "SendQuestDetails Failed: ", ex.ToString())
            End Try

            packet.AddInt32(questRewardsCount)
            Try
                For i As Integer = 0 To QuestInfo.QUEST_REWARD_CHOICES_COUNT
                    If quest.RewardItems(i) <> 0 Then
                        'Add item if not loaded into server
                        If Not ITEMDatabase.ContainsKey(quest.RewardItems(i)) Then
                            Dim tmpItem As New ItemInfo(quest.RewardItems(i))
                            packet.AddInt32(tmpItem.Id)
                        Else
                            packet.AddInt32(quest.RewardItems(i))
                        End If
                        packet.AddInt32(quest.RewardItems_Count(i))
                        packet.AddInt32(ITEMDatabase(quest.RewardItems(i)).Model)
                    Else
                        packet.AddInt32(0)
                        packet.AddInt32(0)
                        packet.AddInt32(0)
                    End If
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "SendQuestDetails Failed: ", ex.ToString())
            End Try
            'QuestRewards (Static)
            questRewardsCount = 0
            For i As Integer = 0 To QuestInfo.QUEST_REWARDS_COUNT
                If quest.RewardStaticItems(i) <> 0 Then questRewardsCount += 1
            Next
            packet.AddInt32(questRewardsCount)
            Try
                For i As Integer = 0 To QuestInfo.QUEST_REWARDS_COUNT
                    If quest.RewardStaticItems(i) <> 0 Then
                        'Add item if not loaded into server
                        If Not ITEMDatabase.ContainsKey(quest.RewardStaticItems(i)) Then
                            Dim tmpItem As New ItemInfo(quest.RewardStaticItems(i))
                            packet.AddInt32(tmpItem.Id)
                        Else
                            packet.AddInt32(quest.RewardStaticItems(i))
                        End If
                        packet.AddInt32(quest.RewardStaticItems_Count(i))
                        packet.AddInt32(ITEMDatabase(quest.RewardStaticItems(i)).Model)
                    Else
                        packet.AddInt32(0)
                        packet.AddInt32(0)
                        packet.AddInt32(0)
                    End If
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "SendQuestDetails Failed: ", ex.ToString())
            End Try

            packet.AddInt32(quest.RewardGold)

            questRewardsCount = 0
            For i As Integer = 0 To quest.ObjectivesItem.GetUpperBound(0) 'QuestInfo.QUEST_OBJECTIVES_COUNT
                If quest.ObjectivesItem(i) <> 0 Then questRewardsCount += 1
            Next
            packet.AddInt32(questRewardsCount)
            For i As Integer = 0 To quest.ObjectivesItem.GetUpperBound(0) 'QuestInfo.QUEST_OBJECTIVES_COUNT
                'Add item if not loaded into server
                If quest.ObjectivesItem(i) <> 0 AndAlso ITEMDatabase.ContainsKey(quest.ObjectivesItem(i)) = False Then
                    Dim tmpItem As New ItemInfo(quest.ObjectivesItem(i))
                    packet.AddInt32(tmpItem.Id)
                Else
                    packet.AddInt32(quest.ObjectivesItem(i))
                End If
                packet.AddInt32(quest.ObjectivesItem_Count(i))
            Next

            questRewardsCount = 0
            For i As Integer = 0 To quest.ObjectivesItem.GetUpperBound(0) 'QuestInfo.QUEST_OBJECTIVES_COUNT
                If quest.ObjectivesKill(i) <> 0 Then questRewardsCount += 1
            Next
            packet.AddInt32(questRewardsCount)
            For i As Integer = 0 To quest.ObjectivesItem.GetUpperBound(0) 'QuestInfo.QUEST_OBJECTIVES_COUNT
                packet.AddUInt32(quest.ObjectivesKill(i))
                packet.AddInt32(quest.ObjectivesKill_Count(i))
            Next

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_QUESTGIVER_QUEST_DETAILS [GUID={2:X} Quest={3}]", client.IP, client.Port, guid, quest.ID)

            'Finishing
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    Public Sub SendQuest(ByRef client As ClientClass, ByRef quest As WS_QuestInfo)
        Dim packet As New PacketClass(OPCODES.SMSG_QUEST_QUERY_RESPONSE)
        Try
            packet.AddInt32(quest.ID)

            'Basic Details
            packet.AddInt32(quest.Level_Start)
            packet.AddInt32(quest.Level_Normal)
            packet.AddInt32(quest.ZoneOrSort)
            packet.AddInt32(quest.Type)
            packet.AddInt32(quest.ObjectiveRepFaction)
            packet.AddInt32(quest.ObjectiveRepStanding)
            packet.AddInt32(0)
            packet.AddInt32(0)
            packet.AddInt32(quest.NextQuestInChain)
            packet.AddInt32(quest.RewardGold) 'Negative is required money
            packet.AddInt32(quest.RewMoneyMaxLevel)

            If quest.RewardSpell > 0 Then
                If SPELLs.ContainsKey(quest.RewardSpell) Then
                    If SPELLs(quest.RewardSpell).SpellEffects(0) IsNot Nothing AndAlso SPELLs(quest.RewardSpell).SpellEffects(0).ID = SpellEffects_Names.SPELL_EFFECT_LEARN_SPELL Then
                        packet.AddInt32(SPELLs(quest.RewardSpell).SpellEffects(0).TriggerSpell)
                    Else
                        packet.AddInt32(quest.RewardSpell)
                    End If
                Else
                    packet.AddInt32(0)
                End If
            Else
                packet.AddInt32(0)
            End If

            packet.AddInt32(quest.ObjectivesDeliver) ' Item given at the start of a quest (srcItem)
            packet.AddInt32((quest.QuestFlags And &HFFFF))

            Try
                For i As Integer = 0 To QuestInfo.QUEST_REWARDS_COUNT
                    packet.AddInt32(quest.RewardStaticItems(i))
                    packet.AddInt32(quest.RewardStaticItems_Count(i))
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "SendQuest Failed: ", ex.ToString())
            End Try

            Try
                For i As Integer = 0 To QuestInfo.QUEST_REWARD_CHOICES_COUNT
                    packet.AddInt32(quest.RewardItems(i))
                    packet.AddInt32(quest.RewardItems_Count(i))
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "SendQuest Failed: ", ex.ToString())
            End Try

            packet.AddInt32(quest.PointMapID)       'Point MapID
            packet.AddSingle(quest.PointX)          'Point X
            packet.AddSingle(quest.PointY)          'Point Y
            packet.AddInt32(quest.PointOpt)         'Point Opt

            'Texts
            packet.AddString(quest.Title)
            packet.AddString(quest.TextObjectives)
            packet.AddString(quest.TextDescription)
            packet.AddString(quest.TextEnd)

            'Objectives
            For i As Integer = 0 To QuestInfo.QUEST_OBJECTIVES_COUNT
                packet.AddInt32(quest.ObjectivesKill(i))
                packet.AddInt32(quest.ObjectivesKill_Count(i))
                packet.AddInt32(quest.ObjectivesItem(i))
                packet.AddInt32(quest.ObjectivesItem_Count(i))

                'HACK: Fix for not showing "Unknown Item" (sometimes client doesn't get items on time)
                If quest.ObjectivesItem(i) <> 0 Then SendItemInfo(client, quest.ObjectivesItem(i))
            Next

            For i As Integer = 0 To QuestInfo.QUEST_OBJECTIVES_COUNT
                packet.AddString(quest.ObjectivesText(i))
            Next

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_QUEST_QUERY_RESPONSE [Quest={2}]", client.IP, client.Port, quest.ID)

            'Finishing
            client.Send(packet)
        Catch ex As Exception
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SendQuest Failed [Quest={2}] {3}", client.IP, client.Port, quest.ID, ex.ToString())

        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest message add item.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="itemID">The item identifier.</param>
    ''' <param name="itemCount">The item count.</param>
    Public Sub SendQuestMessageAddItem(ByRef client As ClientClass, ByVal itemID As Integer, ByVal itemCount As Integer)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_ADD_ITEM)
        Try
            packet.AddInt32(itemID)
            packet.AddInt32(itemCount)
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
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
    Public Sub SendQuestMessageAddKill(ByRef client As ClientClass, ByVal questID As Integer, ByVal killGuid As ULong, ByVal killID As Integer, ByVal killCurrentCount As Integer, ByVal killCount As Integer)
        'Message: %s slain: %d/%d
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_ADD_KILL)
        Try
            packet.AddInt32(questID)
            If killID < 0 Then killID = ((-killID) Or &H80000000) 'Gameobject
            packet.AddInt32(killID)
            packet.AddInt32(killCurrentCount)
            packet.AddInt32(killCount)
            packet.AddUInt64(killGuid)
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest message failed.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="QuestID">The quest identifier.</param>
    Public Sub SendQuestMessageFailed(ByRef client As ClientClass, ByVal questID As Integer)
        'Message: ?
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_FAILED)
        Try
            packet.AddInt32(questID)
            ' TODO: Need to add failed reason to packet here
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest message failed timer.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="QuestID">The quest identifier.</param>
    Public Sub SendQuestMessageFailedTimer(ByRef client As ClientClass, ByVal questID As Integer)
        'Message: ?
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_FAILEDTIMER)
        Try
            packet.AddInt32(questID)
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest message complete.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="QuestID">The quest identifier.</param>
    Public Sub SendQuestMessageComplete(ByRef client As ClientClass, ByVal questID As Integer)
        'Message: Objective Complete.
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTUPDATE_COMPLETE)
        Try
            packet.AddInt32(questID)
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest complete.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="XP">The xp.</param>
    ''' <param name="Gold">The gold.</param>
    Public Sub SendQuestComplete(ByRef client As ClientClass, ByRef quest As WS_QuestInfo, ByVal xP As Integer, ByVal gold As Integer)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_COMPLETE)
        Try
            packet.AddInt32(quest.ID)
            packet.AddInt32(3)
            packet.AddInt32(xP)
            packet.AddInt32(gold)
            packet.AddInt32(quest.RewardHonor) ' bonus honor...used in BG quests

            Dim rewardsCount As Integer = 0
            For i As Integer = 0 To QuestInfo.QUEST_REWARDS_COUNT
                If quest.RewardStaticItems(i) > 0 Then rewardsCount += 1
            Next
            packet.AddInt32(rewardsCount)
            For i As Integer = 0 To QuestInfo.QUEST_REWARDS_COUNT
                If quest.RewardStaticItems(i) > 0 Then
                    packet.AddInt32(quest.RewardStaticItems(i))
                    packet.AddInt32(quest.RewardStaticItems_Count(i))
                End If
            Next
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest reward.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="objBaseQuest">The Base Quest.</param>
    Public Sub SendQuestReward(ByRef client As ClientClass, ByRef quest As WS_QuestInfo, ByVal guid As ULong, ByRef objBaseQuest As WS_QuestsBase)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_OFFER_REWARD)
        Try
            packet.AddUInt64(guid)
            packet.AddInt32(objBaseQuest.ID)
            packet.AddString(objBaseQuest.Title)
            packet.AddString(quest.TextComplete)

            packet.AddInt32(CType(objBaseQuest.Complete, Integer))     'EnbleNext

            Dim emoteCount As Integer = 0
            For i As Integer = 0 To 3
                If quest.OfferRewardEmote(i) <= 0 Then Continue For
                emoteCount += 1
            Next

            packet.AddInt32(emoteCount)
            For i As Integer = 0 To emoteCount - 1
                packet.AddInt32(0) 'EmoteDelay
                packet.AddInt32(quest.OfferRewardEmote(i))
            Next

            Dim questRewardsCount As Integer = 0
            For i As Integer = 0 To QuestInfo.QUEST_REWARD_CHOICES_COUNT
                If quest.RewardItems(i) <> 0 Then questRewardsCount += 1
            Next
            packet.AddInt32(questRewardsCount)
            For i As Integer = 0 To QuestInfo.QUEST_REWARD_CHOICES_COUNT
                If quest.RewardItems(i) <> 0 Then
                    packet.AddInt32(quest.RewardItems(i))
                    packet.AddInt32(quest.RewardItems_Count(i))

                    'Add item if not loaded into server
                    If Not ITEMDatabase.ContainsKey(quest.RewardItems(i)) Then
                        Dim tmpItem As New ItemInfo(quest.RewardItems(i))
                        packet.AddInt32(tmpItem.Model)
                    Else
                        packet.AddInt32(ITEMDatabase(quest.RewardItems(i)).Model)
                    End If
                End If
            Next

            questRewardsCount = 0
            For i As Integer = 0 To QuestInfo.QUEST_REWARDS_COUNT
                If quest.RewardStaticItems(i) <> 0 Then questRewardsCount += 1
            Next
            packet.AddInt32(questRewardsCount)
            For i As Integer = 0 To QuestInfo.QUEST_REWARDS_COUNT
                If quest.RewardStaticItems(i) <> 0 Then
                    packet.AddInt32(quest.RewardStaticItems(i))
                    packet.AddInt32(quest.RewardStaticItems_Count(i))

                    'Add item if not loaded into server
                    If Not ITEMDatabase.ContainsKey(quest.RewardStaticItems(i)) Then
                        'TODO: Another one of these useless bits of code, needs to be implemented correctly
                        Dim tmpItem As New ItemInfo(quest.RewardStaticItems(i))
                    End If
                    packet.AddInt32(ITEMDatabase(quest.RewardStaticItems(i)).Model)
                End If
            Next

            packet.AddInt32(quest.RewardGold)
            packet.AddInt32(0)

            If quest.RewardSpell > 0 Then
                If SPELLs.ContainsKey(quest.RewardSpell) Then
                    If SPELLs(quest.RewardSpell).SpellEffects(0) IsNot Nothing AndAlso SPELLs(quest.RewardSpell).SpellEffects(0).ID = SpellEffects_Names.SPELL_EFFECT_LEARN_SPELL Then
                        packet.AddInt32(SPELLs(quest.RewardSpell).SpellEffects(0).TriggerSpell)
                    Else
                        packet.AddInt32(quest.RewardSpell)
                    End If
                Else
                    packet.AddInt32(0)
                End If
            Else
                packet.AddInt32(0)
            End If

            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the quest required items.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="Quest">The quest.</param>
    ''' <param name="GUID">The unique identifier.</param>
    ''' <param name="objBaseQuest">The Base Quests.</param>
    Public Sub SendQuestRequireItems(ByRef client As ClientClass, ByRef quest As WS_QuestInfo, ByVal guid As ULong, ByRef objBaseQuest As WS_QuestsBase)
        Dim packet As New PacketClass(OPCODES.SMSG_QUESTGIVER_REQUEST_ITEMS)
        Try
            packet.AddUInt64(guid)
            packet.AddInt32(objBaseQuest.ID)
            packet.AddString(objBaseQuest.Title)
            packet.AddString(quest.TextIncomplete)
            packet.AddInt32(0) 'Unknown

            If objBaseQuest.Complete Then
                packet.AddInt32(quest.CompleteEmote)
            Else
                packet.AddInt32(quest.IncompleteEmote)
            End If

            packet.AddInt32(0)                      'Close Window on Cancel (1 = true / 0 = false)
            If quest.RewardGold < 0 Then
                packet.AddInt32(-quest.RewardGold)   'Required Money
            Else
                packet.AddInt32(0)
            End If

            'DONE: Count the required items
            Dim requiredItemsCount As Byte = 0
            For i As Integer = 0 To quest.ObjectivesItem.GetUpperBound(0) 'QuestInfo.QUEST_OBJECTIVES_COUNT
                If quest.ObjectivesItem(i) <> 0 Then requiredItemsCount += 1
            Next
            packet.AddInt32(requiredItemsCount)

            'DONE: List items
            If requiredItemsCount > 0 Then
                For i As Integer = 0 To quest.ObjectivesItem.GetUpperBound(0) 'QuestInfo.QUEST_OBJECTIVES_COUNT
                    If quest.ObjectivesItem(i) <> 0 Then
                        If ITEMDatabase.ContainsKey(quest.ObjectivesItem(i)) = False Then
                            Dim tmpItem As ItemInfo = New ItemInfo(quest.ObjectivesItem(i))
                            packet.AddInt32(tmpItem.Id)
                        Else
                            packet.AddInt32(quest.ObjectivesItem(i))
                        End If
                        packet.AddInt32(quest.ObjectivesItem_Count(i))
                        If ITEMDatabase.ContainsKey(quest.ObjectivesItem(i)) Then
                            packet.AddInt32(ITEMDatabase(quest.ObjectivesItem(i)).Model)
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
        Finally
            packet.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Loads the quests.
    ''' </summary>
    ''' <param name="objCharacter">The Character.</param>
    Public Sub LoadQuests(ByRef objCharacter As CharacterObject)
        Dim cQuests As New DataTable
        Dim i As Integer = 0
        CharacterDatabase.Query(String.Format("SELECT quest_id, quest_status FROM characters_quests q WHERE q.char_guid = {0};", objCharacter.GUID), cQuests)

        For Each cRow As DataRow In cQuests.Rows
            Dim questID As Integer = CInt(cRow.Item("quest_id"))
            Dim questStatus As Integer = cRow.Item("quest_status")
            If questStatus >= 0 Then    'Outstanding Quest

                If IsValidQuest(questID) = True Then
                    Dim tmpQuest As WS_QuestInfo
                    tmpQuest = ReturnQuestInfoById(questID)

                    'DONE: Initialize quest info
                    CreateQuest(objCharacter.TalkQuests(i), tmpQuest)

                    objCharacter.TalkQuests(i).LoadState(questStatus)
                    objCharacter.TalkQuests(i).Slot = i
                    objCharacter.TalkQuests(i).UpdateItemCount(objCharacter)

                    i += 1
                End If
            ElseIf questStatus = -1 Then 'Completed
                objCharacter.QuestsCompleted.Add(questID)
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
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="Creature">The creature.</param>
    Public Sub OnQuestKill(ByRef objCharacter As CharacterObject, ByRef creature As CreatureObject)
        'HANDLERS: Added to DealDamage sub

        'DONE: Do not count killed from guards
        If objCharacter Is Nothing Then Exit Sub

        'DONE: Count kills
        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If (Not objCharacter.TalkQuests(i) Is Nothing) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_KILL) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) = 0 Then
                If TypeOf objCharacter.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objCharacter.TalkQuests(i), WS_QuestsBaseScripted).OnQuestKill(objCharacter, creature)
                Else
                    With objCharacter.TalkQuests(i)
                        For j As Byte = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesObject(j) = creature.ID Then
                                If .Progress(j) < .ObjectivesCount(j) Then
                                    .AddKill(objCharacter, j, creature.GUID)
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
        For Each guid As ULong In objCharacter.Group.LocalMembers
            If guid = objCharacter.GUID Then Continue For

            With CHARACTERs(guid)
                For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                    If (Not .TalkQuests(i) Is Nothing) AndAlso (.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_KILL) AndAlso (.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) = 0 Then
                        With .TalkQuests(i)
                            For j As Byte = 0 To 3
                                If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesObject(j) = creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddKill(objCharacter, j, creature.GUID)
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
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="Creature">The creature.</param>
    ''' <param name="SpellID">The spell identifier.</param>
    Public Sub OnQuestCastSpell(ByRef objCharacter As CharacterObject, ByRef creature As CreatureObject, ByVal spellID As Integer)
        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct creature
        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If (Not objCharacter.TalkQuests(i) Is Nothing) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) Then
                If TypeOf objCharacter.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objCharacter.TalkQuests(i), WS_QuestsBaseScripted).OnQuestCastSpell(objCharacter, creature, spellID)
                Else
                    With objCharacter.TalkQuests(i)
                        For j As Byte = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesSpell(j) = spellID Then
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddCast(objCharacter, j, creature.GUID)
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
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="GameObject">The game object.</param>
    ''' <param name="SpellID">The spell identifier.</param>
    Public Sub OnQuestCastSpell(ByRef objCharacter As CharacterObject, ByRef gameObject As GameObjectObject, ByVal spellID As Integer)
        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct gameobject
        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If (Not objCharacter.TalkQuests(i) Is Nothing) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_CAST) Then
                If TypeOf objCharacter.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objCharacter.TalkQuests(i), WS_QuestsBaseScripted).OnQuestCastSpell(objCharacter, gameObject, spellID)
                Else
                    With objCharacter.TalkQuests(i)
                        For j As Byte = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_KILL AndAlso .ObjectivesSpell(j) = spellID Then
                                'NOTE: GameObjects are negative here!
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = -(gameObject.ID) Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddCast(objCharacter, j, gameObject.GUID)
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
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="Creature">The creature.</param>
    ''' <param name="EmoteID">The emote identifier.</param>
    Public Sub OnQuestDoEmote(ByRef objCharacter As CharacterObject, ByRef creature As CreatureObject, ByVal emoteID As Integer)
        Dim j As Byte

        'DONE: Count spell casts
        'DONE: Check if we're casting it on the correct gameobject
        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If (Not objCharacter.TalkQuests(i) Is Nothing) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_EMOTE) Then
                If TypeOf objCharacter.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objCharacter.TalkQuests(i), WS_QuestsBaseScripted).OnQuestEmote(objCharacter, creature, emoteID)
                Else
                    With objCharacter.TalkQuests(i)
                        For j = 0 To 3
                            If .ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_EMOTE AndAlso .ObjectivesSpell(j) = emoteID Then
                                'NOTE: GameObjects are negative here!
                                If .ObjectivesObject(j) = 0 OrElse .ObjectivesObject(j) = creature.ID Then
                                    If .Progress(j) < .ObjectivesCount(j) Then
                                        .AddEmote(objCharacter, j)
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
    ''' <param name="objCharacter">The Character.</param>
    ''' <param name="ItemEntry">The item entry.</param>
    ''' <returns></returns>
    Public Function IsItemNeededForQuest(ByRef objCharacter As CharacterObject, ByRef itemEntry As Integer) As Boolean

        'DONE: Check if anyone in the group has the quest that requires this item
        'DONE: If the quest isn't a raid quest then you can't loot quest items
        Dim isRaid As Boolean
        isRaid = objCharacter.IsInRaid
        If objCharacter.IsInGroup Then
            For Each guid As ULong In objCharacter.Group.LocalMembers
                With CHARACTERs(guid)
                    For j As Integer = 0 To QuestInfo.QUEST_SLOTS
                        If (Not .TalkQuests(j) Is Nothing) AndAlso (.TalkQuests(j).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) AndAlso isRaid = False Then
                            With .TalkQuests(j)
                                For k As Byte = 0 To 3
                                    If .ObjectivesItem(k) = itemEntry Then
                                        If .ProgressItem(k) < .ObjectivesItemCount(k) Then Return True
                                    End If
                                Next
                            End With
                        End If
                    Next
                End With
            Next

        Else
            For j As Integer = 0 To QuestInfo.QUEST_SLOTS
                If (Not objCharacter.TalkQuests(j) Is Nothing) AndAlso (objCharacter.TalkQuests(j).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                    With objCharacter.TalkQuests(j)
                        For k As Byte = 0 To 3
                            If .ObjectivesItem(k) = itemEntry Then
                                If .ProgressItem(k) < .ObjectivesItemCount(k) Then Return True
                            End If
                        Next
                    End With
                End If
            Next
        End If

        Return False
    End Function

    ''' <summary>
    ''' Determines whether the specified gameobject is used for quest.
    ''' </summary>
    ''' <param name="gameobject">The gameobject.</param>
    ''' <param name="objCharacter">The obj character.</param>
    ''' <returns></returns>
    Public Function IsGameObjectUsedForQuest(ByRef gameobject As GameObjectObject, ByRef objCharacter As CharacterObject) As Byte
        If Not gameobject.IsUsedForQuests Then Return 0

        For Each questItemID As Integer In gameobject.IncludesQuestItems
            'DONE: Check quests needing that item
            For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                If objCharacter.TalkQuests(i) IsNot Nothing AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                    For j As Byte = 0 To 3
                        If objCharacter.TalkQuests(i).ObjectivesType(j) = QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM AndAlso objCharacter.TalkQuests(i).ObjectivesItem(j) = questItemID Then
                            If objCharacter.ItemCOUNT(questItemID) < objCharacter.TalkQuests(i).ObjectivesItemCount(j) Then Return 2
                        End If
                    Next
                End If
            Next i
        Next

        Return 1
    End Function

    'DONE: Quest's loot generation
    Public Sub OnQuestAddQuestLoot(ByRef objCharacter As CharacterObject, ByRef creature As CreatureObject, ByRef loot As LootObject)
        'HANDLERS: Added in loot generation sub

        'TODO: Check for quest loots for adding to looted creature
    End Sub

    Public Sub OnQuestAddQuestLoot(ByRef objCharacter As CharacterObject, ByRef gameObject As GameObjectObject, ByRef loot As LootObject)
        'HANDLERS: None
        'TODO: Check for quest loots for adding to looted gameObject
    End Sub

    Public Sub OnQuestAddQuestLoot(ByRef objCharacter As CharacterObject, ByRef character As CharacterObject, ByRef loot As LootObject)
        'HANDLERS: None
        'TODO: Check for quest loots for adding to looted player (used only in battleground?)
    End Sub

    'DONE: Item quest events
    Public Sub OnQuestItemAdd(ByRef objCharacter As CharacterObject, ByVal itemID As Integer, ByVal count As Byte)
        'HANDLERS: Added to looting sub

        If count = 0 Then count = 1

        'DONE: Check quests needing that item
        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If (Not objCharacter.TalkQuests(i) Is Nothing) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                If TypeOf objCharacter.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objCharacter.TalkQuests(i), WS_QuestsBaseScripted).OnQuestItem(objCharacter, itemID, count)
                Else
                    With objCharacter.TalkQuests(i)
                        For j As Integer = 0 To 3
                            If .ObjectivesItem(j) = itemID Then
                                If .ProgressItem(j) < .ObjectivesItemCount(j) Then
                                    .AddItem(objCharacter, j, count)
                                    Exit Sub
                                End If
                            End If
                        Next
                    End With
                End If
            End If
        Next i
    End Sub

    Public Sub OnQuestItemRemove(ByRef objCharacter As CharacterObject, ByVal itemID As Integer, ByVal count As Byte)
        'HANDLERS: Added to delete sub
        If count = 0 Then count = 1

        'DONE: Check quests needing that item
        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If (Not objCharacter.TalkQuests(i) Is Nothing) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                If TypeOf objCharacter.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objCharacter.TalkQuests(i), WS_QuestsBaseScripted).OnQuestItem(objCharacter, itemID, -count)
                Else
                    With objCharacter.TalkQuests(i)
                        For j As Byte = 0 To 3
                            If .ObjectivesItem(j) = itemID Then
                                If .ProgressItem(j) > 0 Then
                                    .RemoveItem(objCharacter, j, count)
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
    Public Sub OnQuestExplore(ByRef objCharacter As CharacterObject, ByVal areaID As Integer)
        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If (Not objCharacter.TalkQuests(i) Is Nothing) AndAlso (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_EXPLORE) Then
                If TypeOf objCharacter.TalkQuests(i) Is WS_QuestsBaseScripted Then
                    CType(objCharacter.TalkQuests(i), WS_QuestsBaseScripted).OnQuestExplore(objCharacter, areaID)
                Else
                    With objCharacter.TalkQuests(i)
                        For j As Byte = 0 To 3
                            If .ObjectivesExplore(j) = areaID Then
                                If .Explored = False Then
                                    .AddExplore(objCharacter)
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

    ''' <summary>
    ''' Classes the by quest sort.
    ''' </summary>
    ''' <param name="QuestSort">This is the Value from Col 0 of QuestSort.dbc.</param>
    ''' <returns></returns>
    Public Function ClassByQuestSort(ByVal questSort As Integer) As Byte
        'TODO: There are many other quest types missing from this list, but present in the DBC
        Select Case questSort
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

    Public Function GetQuestgiverStatus(ByRef objCharacter As CharacterObject, ByVal cGuid As ULong) As QuestgiverStatusFlag
        'DONE: Invoke scripted quest status
        Dim status As QuestgiverStatusFlag = QuestgiverStatusFlag.DIALOG_STATUS_NONE
        'DONE: Do search for completed quests or in progress

        Dim alreadyHave As New List(Of Integer)

        If GuidIsCreature(cGuid) = True Then    'Is the GUID a creature (or npc)
            If WORLD_CREATUREs.ContainsKey(cGuid) = False Then
                status = QuestgiverStatusFlag.DIALOG_STATUS_NONE
                Return status
            End If

            Log.WriteLine(LogType.CRITICAL, "QuestStatus ID: {0} NPC Name: {1}", WORLD_CREATUREs(cGuid).ID, WORLD_CREATUREs(cGuid).Name)
            '            Log.WriteLine(LogType.CRITICAL, "QuestStatus ID: {0} NPC Name: {1} Has Quest: {2}", WORLD_CREATUREs(cGuid).ID, WORLD_CREATUREs(cGuid).Name, IsNothing(WORLD_CREATUREs(cGuid).CreatureInfo.TalkScript))
            ' Log.WriteLine(LogType.CRITICAL, "Status = {0} {1} {2}", WORLD_CREATUREs(cGUID).)
            '     If IsNothing(WORLD_CREATUREs(cGUID).CreatureInfo.TalkScript) = False Then    'NPC is a questgiven
            Dim creatureQuestId As Integer
            creatureQuestId = WORLD_CREATUREs(cGuid).ID
            If IsValidQuest(creatureQuestId) = True Then
                Log.WriteLine(LogType.CRITICAL, "QuestStatus ID: {0} Valid Quest: {1}", WORLD_CREATUREs(cGuid).ID, IsValidQuest(creatureQuestId))
                If CreatureQuestStarters.ContainsKey(creatureQuestId) = True Then
                    For Each questID As Integer In CreatureQuestStarters(creatureQuestId)
                        Try
                            If ALLQUESTS.ReturnQuestInfoById(questID).CanSeeQuest(objCharacter) = True Then
                                'If objCharacter.IsQuestInProgress(creatureQuestId) = False Then
                                '    Dim Prequest As mangosVB.WorldServer.WS_QuestInfo = ALLQUESTS.ReturnQuestInfoById(creatureQuestId)
                                '    Prequest.PreQuests.Contains()
                                'ALLQUESTS.DoesPreQuestExist(creatureQuestId,

                                status = QuestgiverStatusFlag.DIALOG_STATUS_AVAILABLE
                                Return status
                                'End If
                            End If
                        Catch ex As Exception
                            Log.WriteLine(LogType.CRITICAL, "GetQuestGiverStatus Error")
                        End Try
                    Next

                    For Each questID As Integer In CreatureQuestFinishers(creatureQuestId)
                        Try
                            If ALLQUESTS.ReturnQuestInfoById(questID).CanSeeQuest(objCharacter) = True Then
                                If objCharacter.IsQuestInProgress(questID) = True Then
                                    status = QuestgiverStatusFlag.DIALOG_STATUS_REWARD
                                    Return status
                                End If
                            End If
                        Catch ex As Exception
                            Log.WriteLine(LogType.CRITICAL, "GetQuestGiverStatus Error")
                        End Try
                    Next
                End If
            End If
            'If WORLD_CREATUREs(cGUID).CreatureInfo.Id
            'IF cannot see quest, run line below
            status = WORLD_CREATUREs(cGuid).CreatureInfo.TalkScript.OnQuestStatus(objCharacter, cGuid)
            Return status
            'End If

        ElseIf GuidIsGameObject(cGuid) = True Then  'Or is it a worldobject
            If WORLD_GAMEOBJECTs.ContainsKey(cGuid) = False Then
                status = QuestgiverStatusFlag.DIALOG_STATUS_NONE
                Return status

            End If
        Else        'everything else doesn't get a marker
            status = QuestgiverStatusFlag.DIALOG_STATUS_NONE
            Return status

        End If

        For i As Integer = 0 To QuestInfo.QUEST_SLOTS
            If objCharacter.TalkQuests(i) IsNot Nothing Then
                alreadyHave.Add(objCharacter.TalkQuests(i).ID)
                If GuidIsCreature(cGuid) Then
                    If CreatureQuestFinishers.ContainsKey(WORLD_CREATUREs(cGuid).ID) AndAlso CreatureQuestFinishers(WORLD_CREATUREs(cGuid).ID).Contains(objCharacter.TalkQuests(i).ID) Then
                        If objCharacter.TalkQuests(i).Complete Then
                            status = QuestgiverStatusFlag.DIALOG_STATUS_REWARD
                            Exit For
                        End If
                        status = QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE
                    End If
                Else
                    If GameobjectQuestFinishers.ContainsKey(WORLD_GAMEOBJECTs(cGuid).ID) AndAlso GameobjectQuestFinishers(WORLD_GAMEOBJECTs(cGuid).ID).Contains(objCharacter.TalkQuests(i).ID) Then
                        If objCharacter.TalkQuests(i).Complete Then
                            status = QuestgiverStatusFlag.DIALOG_STATUS_REWARD
                            Exit For
                        End If
                        status = QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE
                    End If
                End If
            End If
        Next

        Return status
    End Function

    Public Sub On_CMSG_QUESTGIVER_STATUS_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
        Try
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim guid As ULong = packet.GetUInt64()

            Dim status As QuestgiverStatusFlag = GetQuestgiverStatus(client.Character, guid)

            Dim response As New PacketClass(OPCODES.SMSG_QUESTGIVER_STATUS)
            Try
                response.AddUInt64(guid)
                response.AddInt32(status)
                client.Send(response)
            Finally
                response.Dispose()
            End Try
        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "On_CMSG_QUESTGIVER_STATUS_QUERY - Error in questgiver status query.{0}", vbNewLine & e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_QUESTGIVER_HELLO(ByRef packet As PacketClass, ByRef client As ClientClass)
        Try
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim guid As ULong = packet.GetUInt64

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_HELLO [GUID={2:X}]", client.IP, client.Port, guid)
            If WORLD_CREATUREs(guid).Evade Then Exit Sub

            WORLD_CREATUREs(guid).StopMoving()
            client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TALK)

            'TODO: There is something here not working all the time :/
            If CREATURESDatabase(WORLD_CREATUREs(guid).ID).TalkScript Is Nothing Then
                SendQuestMenu(client.Character, guid, "I have some tasks for you, $N.")
            Else
                CREATURESDatabase(WORLD_CREATUREs(guid).ID).TalkScript.OnGossipHello(client.Character, guid)
            End If
        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "On_CMSG_QUESTGIVER_HELLO - Error when sending quest menu.{0}", vbNewLine & e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_QUESTGIVER_QUERY_QUEST(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong = packet.GetUInt64
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_QUERY_QUEST [GUID={2:X} QuestID={3}]", client.IP, client.Port, guid, questID)

        If Not ALLQUESTS.IsValidQuest(questID) Then
            Dim tmpQuest As New WS_QuestInfo(questID)
            Try
                client.Character.TalkCurrentQuest = tmpQuest
                SendQuestDetails(client, client.Character.TalkCurrentQuest, guid, True)
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "On_CMSG_QUESTGIVER_QUERY_QUEST - Error while querying a quest.{0}{1}", vbNewLine, ex.ToString)
            End Try
        Else
            Try
                client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                SendQuestDetails(client, client.Character.TalkCurrentQuest, guid, True)
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "On_CMSG_QUESTGIVER_QUERY_QUEST - Error while querying a quest.{0}{1}", vbNewLine, ex.ToString)
            End Try
        End If
    End Sub

    Public Sub On_CMSG_QUESTGIVER_ACCEPT_QUEST(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong = packet.GetUInt64
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_ACCEPT_QUEST [GUID={2:X} QuestID={3}]", client.IP, client.Port, guid, questID)

        If Not ALLQUESTS.IsValidQuest(questID) Then
            Dim tmpQuest As New WS_QuestInfo(questID)
            'Load quest data
            If client.Character.TalkCurrentQuest.ID <> questID Then client.Character.TalkCurrentQuest = tmpQuest
        Else
            'Load quest data
            If client.Character.TalkCurrentQuest.ID <> questID Then client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
        End If

        If client.Character.TalkCanAccept(client.Character.TalkCurrentQuest) Then
            If client.Character.TalkAddQuest(client.Character.TalkCurrentQuest) Then
                If GuidIsPlayer(guid) Then
                    Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                    Try
                        response.AddUInt64(client.Character.GUID)
                        response.AddInt8(QuestPartyPushError.QUEST_PARTY_MSG_ACCEPT_QUEST)
                        response.AddInt32(0)
                        CHARACTERs(guid).Client.Send(response)
                    Finally
                        response.Dispose()
                    End Try
                Else
                    Dim status As QuestgiverStatusFlag = GetQuestgiverStatus(client.Character, guid)
                    Dim response As New PacketClass(OPCODES.SMSG_QUESTGIVER_STATUS)
                    Try
                        response.AddUInt64(guid)
                        response.AddInt32(status)
                        client.Send(response)
                    Finally
                        response.Dispose()
                    End Try
                End If
            Else
                Dim response As New PacketClass(OPCODES.SMSG_QUESTLOG_FULL)
                Try
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
            End If
        End If
    End Sub

    Public Sub On_CMSG_QUESTLOG_REMOVE_QUEST(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim slot As Byte = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTLOG_REMOVE_QUEST [Slot={2}]", client.IP, client.Port, slot)

        client.Character.TalkDeleteQuest(slot)
    End Sub

    Public Sub On_CMSG_QUEST_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUEST_QUERY [QuestID={2}]", client.IP, client.Port, questID)

        If Not ALLQUESTS.IsValidQuest(questID) Then
            Dim tmpQuest As New WS_QuestInfo(questID)
            If client.Character.TalkCurrentQuest Is Nothing Then
                SendQuest(client, tmpQuest)
                Exit Sub
            End If

            If client.Character.TalkCurrentQuest.ID = questID Then
                SendQuest(client, client.Character.TalkCurrentQuest)
            Else
                SendQuest(client, tmpQuest)
            End If
        Else
            If client.Character.TalkCurrentQuest Is Nothing Then
                SendQuest(client, ALLQUESTS.ReturnQuestInfoById(questID))
                Exit Sub
            End If

            If client.Character.TalkCurrentQuest.ID = questID Then
                SendQuest(client, client.Character.TalkCurrentQuest)
            Else
                SendQuest(client, ALLQUESTS.ReturnQuestInfoById(questID))
            End If
        End If
    End Sub

    Public Sub CompleteQuest(ByRef objCharacter As CharacterObject, ByVal questID As Integer, ByVal questGiverGuid As ULong)
        If Not ALLQUESTS.IsValidQuest(questID) Then
            Dim tmpQuest As New WS_QuestInfo(questID)
            For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                If Not objCharacter.TalkQuests(i) Is Nothing Then
                    If objCharacter.TalkQuests(i).ID = questID Then

                        'Load quest data
                        If objCharacter.TalkCurrentQuest Is Nothing Then objCharacter.TalkCurrentQuest = tmpQuest
                        If objCharacter.TalkCurrentQuest.ID <> questID Then objCharacter.TalkCurrentQuest = tmpQuest

                        If objCharacter.TalkQuests(i).Complete Then
                            'DONE: Show completion dialog
                            If (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                                'Request items
                                SendQuestRequireItems(objCharacter.client, objCharacter.TalkCurrentQuest, questGiverGuid, objCharacter.TalkQuests(i))
                            Else
                                SendQuestReward(objCharacter.client, objCharacter.TalkCurrentQuest, questGiverGuid, objCharacter.TalkQuests(i))
                            End If
                        Else
                            'DONE: Just show incomplete text with disabled complete button
                            SendQuestRequireItems(objCharacter.client, objCharacter.TalkCurrentQuest, questGiverGuid, objCharacter.TalkQuests(i))
                        End If

                        Exit For
                    End If
                End If
            Next
        Else
            For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                If Not objCharacter.TalkQuests(i) Is Nothing Then
                    If objCharacter.TalkQuests(i).ID = questID Then

                        'Load quest data
                        If objCharacter.TalkCurrentQuest Is Nothing Then objCharacter.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                        If objCharacter.TalkCurrentQuest.ID <> questID Then objCharacter.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)

                        If objCharacter.TalkQuests(i).Complete Then
                            'DONE: Show completion dialog
                            If (objCharacter.TalkQuests(i).ObjectiveFlags And QuestObjectiveFlag.QUEST_OBJECTIVE_ITEM) Then
                                'Request items
                                SendQuestRequireItems(objCharacter.client, objCharacter.TalkCurrentQuest, questGiverGuid, objCharacter.TalkQuests(i))
                            Else
                                SendQuestReward(objCharacter.client, objCharacter.TalkCurrentQuest, questGiverGuid, objCharacter.TalkQuests(i))
                            End If
                        Else
                            'DONE: Just show incomplete text with disabled complete button
                            SendQuestRequireItems(objCharacter.client, objCharacter.TalkCurrentQuest, questGiverGuid, objCharacter.TalkQuests(i))
                        End If

                        Exit For
                    End If
                End If
            Next
        End If

    End Sub

    Public Sub On_CMSG_QUESTGIVER_COMPLETE_QUEST(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong = packet.GetUInt64
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_COMPLETE_QUEST [GUID={2:X} Quest={3}]", client.IP, client.Port, guid, questID)

        CompleteQuest(client.Character, questID, guid)
    End Sub

    Public Sub On_CMSG_QUESTGIVER_REQUEST_REWARD(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong = packet.GetUInt64
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_REQUEST_REWARD [GUID={2:X} Quest={3}]", client.IP, client.Port, guid, questID)

        If Not ALLQUESTS.IsValidQuest(questID) Then
            Dim tmpQuest As New WS_QuestInfo(questID)
            For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                If client.Character.TalkQuests(i) IsNot Nothing AndAlso client.Character.TalkQuests(i).ID = questID AndAlso client.Character.TalkQuests(i).Complete Then

                    'Load quest data
                    If client.Character.TalkCurrentQuest.ID <> questID Then client.Character.TalkCurrentQuest = tmpQuest
                    SendQuestReward(client, client.Character.TalkCurrentQuest, guid, client.Character.TalkQuests(i))

                    Exit For
                End If
            Next
        Else
            For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                If client.Character.TalkQuests(i) IsNot Nothing AndAlso client.Character.TalkQuests(i).ID = questID AndAlso client.Character.TalkQuests(i).Complete Then

                    'Load quest data
                    If client.Character.TalkCurrentQuest.ID <> questID Then client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                    SendQuestReward(client, client.Character.TalkCurrentQuest, guid, client.Character.TalkQuests(i))

                    Exit For
                End If
            Next
        End If

    End Sub

    Public Sub On_CMSG_QUESTGIVER_CHOOSE_REWARD(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong = packet.GetUInt64
        Dim questID As Integer = packet.GetInt32
        Dim rewardIndex As Integer = packet.GetInt32

        If Not ALLQUESTS.IsValidQuest(questID) Then
            Try
                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_CHOOSE_REWARD [GUID={2:X} Quest={3} Reward={4}]", client.IP, client.Port, guid, questID, rewardIndex)
                If WORLD_CREATUREs.ContainsKey(guid) = False Then Exit Sub

                'Load quest data
                If client.Character.TalkCurrentQuest Is Nothing Then client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                If client.Character.TalkCurrentQuest.ID <> questID Then client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)

                'DONE: Removing required gold
                If client.Character.TalkCurrentQuest.RewardGold < 0 Then
                    If (-client.Character.TalkCurrentQuest.RewardGold) <= client.Character.Copper Then
                        'NOTE: Update flag set below
                        'NOTE: Negative reward gold is required gold, that's why this should be plus
                        client.Character.Copper += client.Character.TalkCurrentQuest.RewardGold
                    Else
                        Dim errorPacket As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                        errorPacket.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_MONEY)
                        client.Send(errorPacket)
                        errorPacket.Dispose()
                        Exit Sub
                    End If
                End If

                'DONE: Removing required items
                For i As Integer = 0 To QuestInfo.QUEST_OBJECTIVES_COUNT
                    If client.Character.TalkCurrentQuest.ObjectivesItem(i) <> 0 Then
                        If Not client.Character.ItemCONSUME(client.Character.TalkCurrentQuest.ObjectivesItem(i), client.Character.TalkCurrentQuest.ObjectivesItem_Count(i)) Then
                            'DONE: Restore gold
                            If client.Character.TalkCurrentQuest.RewardGold < 0 Then
                                'NOTE: Negative reward gold is required gold, that's why this should be minus
                                client.Character.Copper -= client.Character.TalkCurrentQuest.RewardGold
                            End If
                            'TODO: Restore items (not needed?)
                            Dim errorPacket As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                            errorPacket.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_ITEMS)
                            client.Send(errorPacket)
                            errorPacket.Dispose()
                            Exit Sub
                        End If
                    Else
                        Exit For
                    End If
                Next

                'DONE: Adding reward choice
                If client.Character.TalkCurrentQuest.RewardItems(rewardIndex) <> 0 Then
                    Dim tmpItem As New ItemObject(client.Character.TalkCurrentQuest.RewardItems(rewardIndex), client.Character.GUID)
                    tmpItem.StackCount = client.Character.TalkCurrentQuest.RewardItems_Count(rewardIndex)
                    If Not client.Character.ItemADD(tmpItem) Then
                        tmpItem.Delete()
                        'DONE: Inventory full sent form SetItemSlot
                        Exit Sub
                    Else
                        client.Character.LogLootItem(tmpItem, 1, True, False)
                    End If
                End If

                'DONE: Adding gold
                If client.Character.TalkCurrentQuest.RewardGold > 0 Then
                    client.Character.Copper += client.Character.TalkCurrentQuest.RewardGold
                End If
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

                'DONE: Add honor
                If client.Character.TalkCurrentQuest.RewardHonor <> 0 Then
                    client.Character.HonorPoints += client.Character.TalkCurrentQuest.RewardHonor
                    'Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_HONOR_CURRENCY, client.Character.HonorCurrency)
                End If

                'DONE: Cast spell
                If client.Character.TalkCurrentQuest.RewardSpell > 0 Then
                    Dim spellTargets As New SpellTargets
                    spellTargets.SetTarget_UNIT(client.Character)

                    Dim castParams As New CastSpellParameters(spellTargets, WORLD_CREATUREs(guid), client.Character.TalkCurrentQuest.RewardSpell, True)
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
                End If

                'DONE: Remove quest
                For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                    If Not client.Character.TalkQuests(i) Is Nothing Then
                        If client.Character.TalkQuests(i).ID = client.Character.TalkCurrentQuest.ID Then
                            client.Character.TalkCompleteQuest(i)
                            Exit For
                        End If
                    End If
                Next

                'DONE: XP Calculations
                Dim xp As Integer = 0
                Dim gold As Integer = client.Character.TalkCurrentQuest.RewardGold
                If client.Character.TalkCurrentQuest.RewMoneyMaxLevel > 0 Then
                    Dim reqMoneyMaxLevel As Integer = client.Character.TalkCurrentQuest.RewMoneyMaxLevel
                    Dim pLevel As Integer = client.Character.Level
                    Dim qLevel As Integer = client.Character.TalkCurrentQuest.Level_Normal
                    Dim fullxp As Single = 0.0F

                    If pLevel <= DEFAULT_MAX_LEVEL Then
                        If qLevel >= 65 Then
                            fullxp = reqMoneyMaxLevel / 6.0F
                        ElseIf qLevel = 64 Then
                            fullxp = reqMoneyMaxLevel / 4.8F
                        ElseIf qLevel = 63 Then
                            fullxp = reqMoneyMaxLevel / 3.6F
                        ElseIf qLevel = 62 Then
                            fullxp = reqMoneyMaxLevel / 2.4F
                        ElseIf qLevel = 61 Then
                            fullxp = reqMoneyMaxLevel / 1.2F
                        ElseIf qLevel > 0 AndAlso qLevel <= 60 Then
                            fullxp = reqMoneyMaxLevel / 0.6F
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
                        client.Character.AddXP(xp, 0, 0, True)
                    Else
                        gold += reqMoneyMaxLevel
                    End If
                End If

                If gold < 0 AndAlso (-gold) >= client.Character.Copper Then
                    client.Character.Copper = 0
                Else
                    client.Character.Copper += gold
                End If
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                client.Character.SendCharacterUpdate()

                SendQuestComplete(client, client.Character.TalkCurrentQuest, xp, gold)

                'DONE: Follow-up quests (no requirements checked?)
                If client.Character.TalkCurrentQuest.NextQuest <> 0 Then
                    If Not ALLQUESTS.IsValidQuest(client.Character.TalkCurrentQuest.NextQuest) Then
                        Dim tmpQuest2 As New WS_QuestInfo(client.Character.TalkCurrentQuest.NextQuest)
                        client.Character.TalkCurrentQuest = tmpQuest2
                    Else
                        client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(client.Character.TalkCurrentQuest.NextQuest)
                    End If
                    SendQuestDetails(client, client.Character.TalkCurrentQuest, guid, True)
                End If

            Catch e As Exception
                Log.WriteLine(LogType.CRITICAL, "On_CMSG_QUESTGIVER_CHOOSE_REWARD - Error while choosing reward.{0}", vbNewLine & e.ToString)
            End Try

        Else
            Try
                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUESTGIVER_CHOOSE_REWARD [GUID={2:X} Quest={3} Reward={4}]", client.IP, client.Port, guid, questID, rewardIndex)
                If WORLD_CREATUREs.ContainsKey(guid) = False Then Exit Sub

                'Load quest data
                If client.Character.TalkCurrentQuest Is Nothing Then client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                If client.Character.TalkCurrentQuest.ID <> questID Then client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)

                'DONE: Removing required gold
                If client.Character.TalkCurrentQuest.RewardGold < 0 Then
                    If (-client.Character.TalkCurrentQuest.RewardGold) <= client.Character.Copper Then
                        'NOTE: Update flag set below
                        'NOTE: Negative reward gold is required gold, that's why this should be plus
                        client.Character.Copper += client.Character.TalkCurrentQuest.RewardGold
                    Else
                        Dim errorPacket As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                        errorPacket.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_MONEY)
                        client.Send(errorPacket)
                        errorPacket.Dispose()
                        Exit Sub
                    End If
                End If

                'DONE: Removing required items
                For i As Integer = 0 To QuestInfo.QUEST_OBJECTIVES_COUNT
                    If client.Character.TalkCurrentQuest.ObjectivesItem(i) <> 0 Then
                        If Not client.Character.ItemCONSUME(client.Character.TalkCurrentQuest.ObjectivesItem(i), client.Character.TalkCurrentQuest.ObjectivesItem_Count(i)) Then
                            'DONE: Restore gold
                            If client.Character.TalkCurrentQuest.RewardGold < 0 Then
                                'NOTE: Negative reward gold is required gold, that's why this should be minus
                                client.Character.Copper -= client.Character.TalkCurrentQuest.RewardGold
                            End If
                            'TODO: Restore items (not needed?)
                            Dim errorPacket As New PacketClass(OPCODES.SMSG_QUESTGIVER_QUEST_INVALID)
                            errorPacket.AddInt32(QuestInvalidError.INVALIDREASON_DONT_HAVE_REQ_ITEMS)
                            client.Send(errorPacket)
                            errorPacket.Dispose()
                            Exit Sub
                        End If
                    Else
                        Exit For
                    End If
                Next

                'DONE: Adding reward choice
                If client.Character.TalkCurrentQuest.RewardItems(rewardIndex) <> 0 Then
                    Dim tmpItem As New ItemObject(client.Character.TalkCurrentQuest.RewardItems(rewardIndex), client.Character.GUID)
                    tmpItem.StackCount = client.Character.TalkCurrentQuest.RewardItems_Count(rewardIndex)
                    If Not client.Character.ItemADD(tmpItem) Then
                        tmpItem.Delete()
                        'DONE: Inventory full sent form SetItemSlot
                        Exit Sub
                    Else
                        client.Character.LogLootItem(tmpItem, 1, True, False)
                    End If
                End If

                'DONE: Adding gold
                If client.Character.TalkCurrentQuest.RewardGold > 0 Then
                    client.Character.Copper += client.Character.TalkCurrentQuest.RewardGold
                End If
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

                'DONE: Add honor
                If client.Character.TalkCurrentQuest.RewardHonor <> 0 Then
                    client.Character.HonorPoints += client.Character.TalkCurrentQuest.RewardHonor
                    'Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_HONOR_CURRENCY, client.Character.HonorCurrency)
                End If

                'DONE: Cast spell
                If client.Character.TalkCurrentQuest.RewardSpell > 0 Then
                    Dim spellTargets As New SpellTargets
                    spellTargets.SetTarget_UNIT(client.Character)

                    Dim castParams As New CastSpellParameters(spellTargets, WORLD_CREATUREs(guid), client.Character.TalkCurrentQuest.RewardSpell, True)
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
                End If

                'DONE: Remove quest
                For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                    If Not client.Character.TalkQuests(i) Is Nothing Then
                        If client.Character.TalkQuests(i).ID = client.Character.TalkCurrentQuest.ID Then
                            client.Character.TalkCompleteQuest(i)
                            Exit For
                        End If
                    End If
                Next

                'DONE: XP Calculations
                Dim xp As Integer = 0
                Dim gold As Integer = client.Character.TalkCurrentQuest.RewardGold
                If client.Character.TalkCurrentQuest.RewMoneyMaxLevel > 0 Then
                    Dim reqMoneyMaxLevel As Integer = client.Character.TalkCurrentQuest.RewMoneyMaxLevel
                    Dim pLevel As Integer = client.Character.Level
                    Dim qLevel As Integer = client.Character.TalkCurrentQuest.Level_Normal
                    Dim fullxp As Single = 0.0F

                    If pLevel <= DEFAULT_MAX_LEVEL Then
                        If qLevel >= 65 Then
                            fullxp = reqMoneyMaxLevel / 6.0F
                        ElseIf qLevel = 64 Then
                            fullxp = reqMoneyMaxLevel / 4.8F
                        ElseIf qLevel = 63 Then
                            fullxp = reqMoneyMaxLevel / 3.6F
                        ElseIf qLevel = 62 Then
                            fullxp = reqMoneyMaxLevel / 2.4F
                        ElseIf qLevel = 61 Then
                            fullxp = reqMoneyMaxLevel / 1.2F
                        ElseIf qLevel > 0 AndAlso qLevel <= 60 Then
                            fullxp = reqMoneyMaxLevel / 0.6F
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
                        client.Character.AddXP(xp, 0, 0, True)
                    Else
                        gold += reqMoneyMaxLevel
                    End If
                End If

                If gold < 0 AndAlso (-gold) >= client.Character.Copper Then
                    client.Character.Copper = 0
                Else
                    client.Character.Copper += gold
                End If
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                client.Character.SendCharacterUpdate()

                SendQuestComplete(client, client.Character.TalkCurrentQuest, xp, gold)

                'DONE: Follow-up quests (no requirements checked?)
                If client.Character.TalkCurrentQuest.NextQuest <> 0 Then
                    If Not ALLQUESTS.IsValidQuest(client.Character.TalkCurrentQuest.NextQuest) Then
                        Dim tmpQuest3 As New WS_QuestInfo(client.Character.TalkCurrentQuest.NextQuest)
                        client.Character.TalkCurrentQuest = tmpQuest3

                    Else
                        client.Character.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(client.Character.TalkCurrentQuest.NextQuest)
                    End If
                    SendQuestDetails(client, client.Character.TalkCurrentQuest, guid, True)
                End If

            Catch e As Exception
                Log.WriteLine(LogType.CRITICAL, "On_CMSG_QUESTGIVER_CHOOSE_REWARD - Error while choosing reward.{0}", vbNewLine & e.ToString)
            End Try

        End If

    End Sub

    Public Sub On_CMSG_PUSHQUESTTOPARTY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim questID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PUSHQUESTTOPARTY [{2}]", client.IP, client.Port, questID)

        If client.Character.IsInGroup Then
            If Not ALLQUESTS.IsValidQuest(questID) Then
                Dim tmpQuest As New WS_QuestInfo(questID)
                For Each guid As ULong In client.Character.Group.LocalMembers
                    If guid = client.Character.GUID Then Continue For

                    With CHARACTERs(guid)

                        Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                        response.AddUInt64(guid)
                        response.AddInt32(QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST)
                        response.AddInt8(0)
                        client.Send(response)
                        response.Dispose()

                        Dim message As QuestPartyPushError = QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST

                        'DONE: Check distance and ...
                        If (Math.Sqrt((.positionX - client.Character.positionX) ^ 2 + (.positionY - client.Character.positionY) ^ 2) > QuestInfo.QUEST_SHARING_DISTANCE) Then
                            message = QuestPartyPushError.QUEST_PARTY_MSG_TO_FAR
                        ElseIf .IsQuestInProgress(questID) Then
                            message = QuestPartyPushError.QUEST_PARTY_MSG_HAVE_QUEST
                        ElseIf .IsQuestCompleted(questID) Then
                            message = QuestPartyPushError.QUEST_PARTY_MSG_FINISH_QUEST
                        Else
                            If (.TalkCurrentQuest Is Nothing) OrElse (.TalkCurrentQuest.ID <> questID) Then .TalkCurrentQuest = tmpQuest
                            If .TalkCanAccept(.TalkCurrentQuest) Then
                                SendQuestDetails(.client, .TalkCurrentQuest, client.Character.GUID, True)
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
                            client.Send(errorPacket)
                            errorPacket.Dispose()
                        End If

                    End With
                Next
            Else
                For Each guid As ULong In client.Character.Group.LocalMembers
                    If guid = client.Character.GUID Then Continue For

                    With CHARACTERs(guid)

                        Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
                        response.AddUInt64(guid)
                        response.AddInt32(QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST)
                        response.AddInt8(0)
                        client.Send(response)
                        response.Dispose()

                        Dim message As QuestPartyPushError = QuestPartyPushError.QUEST_PARTY_MSG_SHARRING_QUEST

                        'DONE: Check distance and ...
                        If (Math.Sqrt((.positionX - client.Character.positionX) ^ 2 + (.positionY - client.Character.positionY) ^ 2) > QuestInfo.QUEST_SHARING_DISTANCE) Then
                            message = QuestPartyPushError.QUEST_PARTY_MSG_TO_FAR
                        ElseIf .IsQuestInProgress(questID) Then
                            message = QuestPartyPushError.QUEST_PARTY_MSG_HAVE_QUEST
                        ElseIf .IsQuestCompleted(questID) Then
                            message = QuestPartyPushError.QUEST_PARTY_MSG_FINISH_QUEST
                        Else
                            If (.TalkCurrentQuest Is Nothing) OrElse (.TalkCurrentQuest.ID <> questID) Then .TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                            If .TalkCanAccept(.TalkCurrentQuest) Then
                                SendQuestDetails(.client, .TalkCurrentQuest, client.Character.GUID, True)
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
                            client.Send(errorPacket)
                            errorPacket.Dispose()
                        End If

                    End With
                Next
            End If
        End If
    End Sub

    Public Sub On_MSG_QUEST_PUSH_RESULT(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 14 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong = packet.GetUInt64
        Dim message As QuestPartyPushError = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_QUEST_PUSH_RESULT [{2:X} {3}]", client.IP, client.Port, guid, message)

        Dim response As New PacketClass(OPCODES.MSG_QUEST_PUSH_RESULT)
        response.AddUInt64(guid)
        response.AddInt8(QuestPartyPushError.QUEST_PARTY_MSG_ACCEPT_QUEST)
        response.AddInt32(0)
        client.Send(response)
        response.Dispose()
    End Sub

#End Region

    Public Sub New()
        ' _quests(1) = New Dictionary(Of Integer, WS_QuestInfo)

    End Sub
End Class