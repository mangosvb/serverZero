'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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

Imports Mangos.Common

Public Module WS_GuardGossip

    Public Class TGuardTalk
        Inherits TBaseTalk

#Region "Gossip functions"
        Public Overrides Sub OnGossipHello(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim Gossip As Gossips = GetGossip(WORLD_CREATUREs(cGUID).ID)

            Select Case Gossip
                Case Gossips.Darnassus
                    OnGossipHello_Darnassus(objCharacter, cGUID)
                Case Gossips.DunMorogh
                    OnGossipHello_DunMorogh(objCharacter, cGUID)
                Case Gossips.Durotar
                    OnGossipHello_Durotar(objCharacter, cGUID)
                Case Gossips.ElwynnForest
                    OnGossipHello_ElwynnForest(objCharacter, cGUID)
                Case Gossips.Ironforge
                    OnGossipHello_Ironforge(objCharacter, cGUID)
                Case Gossips.Mulgore
                    OnGossipHello_Mulgore(objCharacter, cGUID)
                Case Gossips.Orgrimmar
                    OnGossipHello_Orgrimmar(objCharacter, cGUID)
                Case Gossips.Stormwind
                    OnGossipHello_Stormwind(objCharacter, cGUID)
                Case Gossips.Teldrassil
                    OnGossipHello_Teldrassil(objCharacter, cGUID)
                Case Gossips.Thunderbluff
                    OnGossipHello_Thunderbluff(objCharacter, cGUID)
                Case Gossips.Tirisfall
                    OnGossipHello_Tirisfall(objCharacter, cGUID)
                Case Gossips.Undercity
                    OnGossipHello_Undercity(objCharacter, cGUID)
                Case Else
                    Log.WriteLine(LogType.CRITICAL, "Unknown gossip [{0}].", Gossip)
            End Select
        End Sub

        Public Overrides Sub OnGossipSelect(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal selected As Integer)
            Dim Gossip As Gossips = GetGossip(WORLD_CREATUREs(cGUID).ID)

            Select Case Gossip
                Case Gossips.Darnassus
                    OnGossipSelect_Darnassus(objCharacter, cGUID, selected)
                Case Gossips.DunMorogh
                    OnGossipSelect_DunMorogh(objCharacter, cGUID, selected)
                Case Gossips.Durotar
                    OnGossipSelect_Durotar(objCharacter, cGUID, selected)
                Case Gossips.ElwynnForest
                    OnGossipSelect_ElwynnForest(objCharacter, cGUID, selected)
                Case Gossips.Ironforge
                    OnGossipSelect_Ironforge(objCharacter, cGUID, selected)
                Case Gossips.Mulgore
                    OnGossipSelect_Mulgore(objCharacter, cGUID, selected)
                Case Gossips.Orgrimmar
                    OnGossipSelect_Orgrimmar(objCharacter, cGUID, selected)
                Case Gossips.Stormwind
                    OnGossipSelect_Stormwind(objCharacter, cGUID, selected)
                Case Gossips.Teldrassil
                    OnGossipSelect_Teldrassil(objCharacter, cGUID, selected)
                Case Gossips.Thunderbluff
                    OnGossipSelect_Thunderbluff(objCharacter, cGUID, selected)
                Case Gossips.Tirisfall
                    OnGossipSelect_Tirisfall(objCharacter, cGUID, selected)
                Case Gossips.Undercity
                    OnGossipSelect_Undercity(objCharacter, cGUID, selected)
                Case Else
                    Log.WriteLine(LogType.CRITICAL, "Unknown gossip [{0}].", Gossip)
            End Select
        End Sub

        Public Function GetGossip(ByVal Entry As Integer) As Gossips
            Select Case Entry
                Case Guards.Bluffwatcher
                    Return Gossips.Thunderbluff
                Case Guards.Darnassus_Sentinel
                    Return Gossips.Darnassus
                Case Guards.Orgrimmar_Grunt
                    Return Gossips.Orgrimmar
                Case Guards.Stormwind_City_Guard, Guards.Stormwind_City_Patroller
                    Return Gossips.Stormwind
                Case Guards.Stormwind_Guard
                    Return Gossips.ElwynnForest
                Case Guards.Ironforge_Guard
                    Return Gossips.Ironforge
                Case Guards.Ironforge_Mountaineer
                    Return Gossips.DunMorogh
                Case Guards.Razor_Hill_Grunt
                    Return Gossips.Durotar
                Case Guards.Teldrassil_Sentinel
                    Return Gossips.Teldrassil
                Case Guards.Undercity_Guardian
                    Return Gossips.Undercity
                Case Guards.Deathguard_Bartholomew, Guards.Deathguard_Burgess, Guards.Deathguard_Cyrus, Guards.Deathguard_Dillinger, Guards.Deathguard_Lawrence, Guards.Deathguard_Lundmark, Guards.Deathguard_Morris, Guards.Deathguard_Mort, Guards.Deathguard_Terrence
                    Return Gossips.Tirisfall
                Case Else
                    Log.WriteLine(LogType.DEBUG, "Creature Entry [{0}] was not found in guard table.", Entry)
                    Return 0
            End Select
        End Function
#End Region

#Region "Stormwind"
        Private Sub OnGossipHello_Stormwind(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_AUCTIONHOUSE, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STORMWIND_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_DEEPRUNTRAM, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GRYPHON, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_MAILBOX, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WEAPONMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_OFFICERS, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATTLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 13
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 933, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Stormwind(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Auctionhouse
                    objCharacter.SendPointOfInterest(-8811.46F, 667.46F, 6, 6, 0, "Stormwind Auction House")
                    objCharacter.SendGossip(cGUID, 3834)
                Case 2 'Bank
                    objCharacter.SendPointOfInterest(-8916.87F, 622.87F, 6, 6, 0, "Stormwind Bank")
                    objCharacter.SendGossip(cGUID, 764)
                Case 3 'Deeprun Tram
                    objCharacter.SendPointOfInterest(-8378.88F, 554.23F, 6, 6, 0, "The Deeprun Tram")
                    objCharacter.SendGossip(cGUID, 3813)
                Case 4 'Inn
                    objCharacter.SendPointOfInterest(-8869.0F, 675.4F, 6, 6, 0, "The Gilded Rose")
                    objCharacter.SendGossip(cGUID, 3860)
                Case 5 'Gryphon Master
                    objCharacter.SendPointOfInterest(-8837.0F, 493.5F, 6, 6, 0, "Stormwind Gryphon Master")
                    objCharacter.SendGossip(cGUID, 879)
                Case 6 'Guild Master
                    objCharacter.SendPointOfInterest(-8894.0F, 611.2F, 6, 6, 0, "Stormwind Vistor`s Center")
                    objCharacter.SendGossip(cGUID, 882)
                Case 7 'Mailbox
                    objCharacter.SendPointOfInterest(-8876.48F, 649.18F, 6, 6, 0, "Stormwind Mailbox")
                    objCharacter.SendGossip(cGUID, 3861)
                Case 8 'Stable Master
                    objCharacter.SendPointOfInterest(-8433.0F, 554.7F, 6, 6, 0, "Jenova Stoneshield")
                    objCharacter.SendGossip(cGUID, 5984)
                Case 9 'Weapon Trainer
                    objCharacter.SendPointOfInterest(-8797.0F, 612.8F, 6, 6, 0, "Woo Ping")
                    objCharacter.SendGossip(cGUID, 4516)
                Case 10 'Officers Lounge
                    objCharacter.SendPointOfInterest(-8759.92F, 399.69F, 6, 6, 0, "Champions` Hall")
                    objCharacter.SendGossip(cGUID, 7047)
                Case 11 'Battlemasters
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALTERACVALLEY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ARATHIBASIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARSONGULCH, MenuIcon.MENUICON_GOSSIP)
                    objCharacter.TalkMenuTypes.Add(14)
                    objCharacter.TalkMenuTypes.Add(15)
                    objCharacter.TalkMenuTypes.Add(16)
                    objCharacter.SendGossip(cGUID, 7499, npcMenu)
                Case 12 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_DRUID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PALADIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 17 To 24
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 898, npcMenu)
                Case 13 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 25 To 36
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 918, npcMenu)
                Case 14 'AV
                    objCharacter.SendPointOfInterest(-8443.88F, 335.99F, 6, 6, 0, "Thelman Slatefist")
                    objCharacter.SendGossip(cGUID, 7500)
                Case 15 'AB
                    objCharacter.SendPointOfInterest(-8443.88F, 335.99F, 6, 6, 0, "Lady Hoteshem")
                    objCharacter.SendGossip(cGUID, 7650)
                Case 16 'WSG
                    objCharacter.SendPointOfInterest(-8443.88F, 335.99F, 6, 6, 0, "Elfarran")
                    objCharacter.SendGossip(cGUID, 7501)
                Case 17 'Mage
                    objCharacter.SendPointOfInterest(-9012.0F, 867.6F, 6, 6, 0, "Wizard`s Sanctum")
                    objCharacter.SendGossip(cGUID, 899)
                Case 18 'Rogue
                    objCharacter.SendPointOfInterest(-8753.0F, 367.8F, 6, 6, 0, "Stormwind - Rogue House")
                    objCharacter.SendGossip(cGUID, 900)
                Case 19 'Warrior
                    objCharacter.SendPointOfInterest(-8690.11F, 324.85F, 6, 6, 0, "Command Center")
                    objCharacter.SendGossip(cGUID, 901)
                Case 20 'Druid
                    objCharacter.SendPointOfInterest(-8751.0F, 1124.5F, 6, 6, 0, "The Park")
                    objCharacter.SendGossip(cGUID, 902)
                Case 21 'Priest
                    objCharacter.SendPointOfInterest(-8512.0F, 862.4F, 6, 6, 0, "Catedral Of Light")
                    objCharacter.SendGossip(cGUID, 903)
                Case 22 'Paladin
                    objCharacter.SendPointOfInterest(-8577.0F, 881.7F, 6, 6, 0, "Catedral Of Light")
                    objCharacter.SendGossip(cGUID, 904)
                Case 23 'Hunter
                    objCharacter.SendPointOfInterest(-8413.0F, 541.5F, 6, 6, 0, "Hunter Lodge")
                    objCharacter.SendGossip(cGUID, 905)
                Case 24 'Hunter
                    objCharacter.SendPointOfInterest(-8948.91F, 998.35F, 6, 6, 0, "The Slaughtered Lamb")
                    objCharacter.SendGossip(cGUID, 906)
                Case 25 'Alchemy
                    objCharacter.SendPointOfInterest(-8988.0F, 759.6F, 6, 6, 0, "Alchemy Needs")
                    objCharacter.SendGossip(cGUID, 919)
                Case 26 'Blacksmithing
                    objCharacter.SendPointOfInterest(-8424.0F, 616.9F, 6, 6, 0, "Therum Deepforge")
                    objCharacter.SendGossip(cGUID, 920)
                Case 27 'Cooking
                    objCharacter.SendPointOfInterest(-8611.0F, 364.6F, 6, 6, 0, "Pig and Whistle Tavern")
                    objCharacter.SendGossip(cGUID, 921)
                Case 28 'Enchanting
                    objCharacter.SendPointOfInterest(-8858.0F, 803.7F, 6, 6, 0, "Lucan Cordell")
                    objCharacter.SendGossip(cGUID, 941)
                Case 29 'Engineering
                    objCharacter.SendPointOfInterest(-8347.0F, 644.1F, 6, 6, 0, "Lilliam Sparkspindle")
                    objCharacter.SendGossip(cGUID, 922)
                Case 30 'First Aid
                    objCharacter.SendPointOfInterest(-8513.0F, 801.8F, 6, 6, 0, "Shaina Fuller")
                    objCharacter.SendGossip(cGUID, 923)
                Case 31 'Fishing
                    objCharacter.SendPointOfInterest(-8803.0F, 767.5F, 6, 6, 0, "Arnold Leland")
                    objCharacter.SendGossip(cGUID, 940)
                Case 32 'Herbalism
                    objCharacter.SendPointOfInterest(-8967.0F, 779.5F, 6, 6, 0, "Alchemy Needs")
                    objCharacter.SendGossip(cGUID, 924)
                Case 33 'Leatherworking
                    objCharacter.SendPointOfInterest(-8726.0F, 477.4F, 6, 6, 0, "The Protective Hide")
                    objCharacter.SendGossip(cGUID, 925)
                Case 34 'Mining
                    objCharacter.SendPointOfInterest(-8434.0F, 692.8F, 6, 6, 0, "Gelman Stonehand")
                    objCharacter.SendGossip(cGUID, 927)
                Case 35 'Skinning
                    objCharacter.SendPointOfInterest(-8716.0F, 469.4F, 6, 6, 0, "The Protective Hide")
                    objCharacter.SendGossip(cGUID, 928)
                Case 36 'Tailoring
                    objCharacter.SendPointOfInterest(-8938.0F, 800.7F, 6, 6, 0, "Duncan`s Textiles")
                    objCharacter.SendGossip(cGUID, 929)
            End Select
        End Sub
#End Region

#Region "Orgrimmar"
        Private Sub OnGossipHello_Orgrimmar(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WINDRIDER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_MAILBOX, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_AUCTIONHOUSE, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_ZEPPLINMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WEAPONMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_OFFICERS, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATTLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 13
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 2593, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Orgrimmar(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendPointOfInterest(1631.51F, -4375.33F, 6, 6, 0, "Bank of Orgrimmar")
                    objCharacter.SendGossip(cGUID, 2554)
                Case 2 'Wind Rider
                    objCharacter.SendPointOfInterest(1676.6F, -4332.72F, 6, 6, 0, "The Sky Tower")
                    objCharacter.SendGossip(cGUID, 2555)
                Case 3 'Guild Master
                    objCharacter.SendPointOfInterest(1576.93F, -4294.75F, 6, 6, 0, "Horde Embassy")
                    objCharacter.SendGossip(cGUID, 2556)
                Case 4 'Inn
                    objCharacter.SendPointOfInterest(1644.51F, -4447.27F, 6, 6, 0, "Orgrimmar Inn")
                    objCharacter.SendGossip(cGUID, 2557)
                Case 5 'Mailbox
                    objCharacter.SendPointOfInterest(1622.53F, -4388.79F, 6, 6, 0, "Orgrimmar Mailbox")
                    objCharacter.SendGossip(cGUID, 2558)
                Case 6 'Auction House
                    objCharacter.SendPointOfInterest(1679.21F, -4450.1F, 6, 6, 0, "Orgrimmar Auction House")
                    objCharacter.SendGossip(cGUID, 3075)
                Case 7 'Zeppelin
                    objCharacter.SendPointOfInterest(1337.36F, -4632.7F, 6, 6, 0, "Orgrimmar Zeppelin Tower")
                    objCharacter.SendGossip(cGUID, 3173)
                Case 8 'Weapon Trainer
                    objCharacter.SendPointOfInterest(2092.56F, -4823.95F, 6, 6, 0, "Sayoc & Hanashi")
                    objCharacter.SendGossip(cGUID, 4519)
                Case 9 'Stable Master
                    objCharacter.SendPointOfInterest(2133.12F, -4663.93F, 6, 6, 0, "Xon'cha")
                    objCharacter.SendGossip(cGUID, 5974)
                Case 10 'Officers Lounge
                    objCharacter.SendPointOfInterest(1633.56F, -4249.37F, 6, 6, 0, "Hall of Legends")
                    objCharacter.SendGossip(cGUID, 7046)
                Case 11 'Battlemasters
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALTERACVALLEY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ARATHIBASIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARSONGULCH, MenuIcon.MENUICON_GOSSIP)
                    objCharacter.TalkMenuTypes.Add(14)
                    objCharacter.TalkMenuTypes.Add(15)
                    objCharacter.TalkMenuTypes.Add(16)
                    objCharacter.SendGossip(cGUID, 7521, npcMenu)
                Case 12 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SHAMAN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 17 To 23
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 2599, npcMenu)
                Case 13 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 24 To 35
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 2594, npcMenu)
                Case 14 'AV
                    objCharacter.SendPointOfInterest(1983.92F, -4794.2F, 6, 6, 0, "Hall of the Brave")
                    objCharacter.SendGossip(cGUID, 7484)
                Case 15 'AB
                    objCharacter.SendPointOfInterest(1983.92F, -4794.2F, 6, 6, 0, "Hall of the Brave")
                    objCharacter.SendGossip(cGUID, 7644)
                Case 16 'WSG
                    objCharacter.SendPointOfInterest(1983.92F, -4794.2F, 6, 6, 0, "Hall of the Brave")
                    objCharacter.SendGossip(cGUID, 7520)
                Case 17 'Hunter
                    objCharacter.SendPointOfInterest(2114.84F, -4625.31F, 6, 6, 0, "Orgrimmar Hunter's Hall")
                    objCharacter.SendGossip(cGUID, 2559)
                Case 18 'Mage
                    objCharacter.SendPointOfInterest(1451.26F, -4223.33F, 6, 6, 0, "Darkbriar Lodge")
                    objCharacter.SendGossip(cGUID, 2560)
                Case 19 'Priest
                    objCharacter.SendPointOfInterest(1442.21F, -4183.24F, 6, 6, 0, "Spirit Lodge")
                    objCharacter.SendGossip(cGUID, 2561)
                Case 20 'Shaman
                    objCharacter.SendPointOfInterest(1925.34F, -4181.89F, 6, 6, 0, "Thrall's Fortress")
                    objCharacter.SendGossip(cGUID, 2562)
                Case 21 'Rogue
                    objCharacter.SendPointOfInterest(1773.39F, -4278.97F, 6, 6, 0, "Shadowswift Brotherhood")
                    objCharacter.SendGossip(cGUID, 2563)
                Case 22 'Warlock
                    objCharacter.SendPointOfInterest(1849.57F, -4359.68F, 6, 6, 0, "Darkfire Enclave")
                    objCharacter.SendGossip(cGUID, 2564)
                Case 23 'Warrior
                    objCharacter.SendPointOfInterest(1983.92F, -4794.2F, 6, 6, 0, "Hall of the Brave")
                    objCharacter.SendGossip(cGUID, 2565)
                Case 24 'Alchemy
                    objCharacter.SendPointOfInterest(1955.17F, -4475.79F, 6, 6, 0, "Yelmak's Alchemy and Potions")
                    objCharacter.SendGossip(cGUID, 2497)
                Case 25 'Blacksmithing
                    objCharacter.SendPointOfInterest(2054.34F, -4831.85F, 6, 6, 0, "The Burning Anvil")
                    objCharacter.SendGossip(cGUID, 2499)
                Case 26 'Cooking
                    objCharacter.SendPointOfInterest(1780.96F, -4481.31F, 6, 6, 0, "Borstan's Firepit")
                    objCharacter.SendGossip(cGUID, 2500)
                Case 27 'Enchanting
                    objCharacter.SendPointOfInterest(1917.5F, -4434.95F, 6, 6, 0, "Godan's Runeworks")
                    objCharacter.SendGossip(cGUID, 2501)
                Case 28 'Engineering
                    objCharacter.SendPointOfInterest(2038.45F, -4744.75F, 6, 6, 0, "Nogg's Machine Shop")
                    objCharacter.SendGossip(cGUID, 2653)
                Case 29 'First Aid
                    objCharacter.SendPointOfInterest(1485.21F, -4160.91F, 6, 6, 0, "Survival of the Fittest")
                    objCharacter.SendGossip(cGUID, 2502)
                Case 30 'Fishing
                    objCharacter.SendPointOfInterest(1994.15F, -4655.7F, 6, 6, 0, "Lumak's Fishing")
                    objCharacter.SendGossip(cGUID, 2503)
                Case 31 'Herbalism
                    objCharacter.SendPointOfInterest(1898.61F, -4454.93F, 6, 6, 0, "Jandi's Arboretum")
                    objCharacter.SendGossip(cGUID, 2504)
                Case 32 'Leatherworking
                    objCharacter.SendPointOfInterest(1852.82F, -4562.31F, 6, 6, 0, "Kodohide Leatherworkers")
                    objCharacter.SendGossip(cGUID, 2513)
                Case 33 'Mining
                    objCharacter.SendPointOfInterest(2029.79F, -4704.0F, 6, 6, 0, "Red Canyon Mining")
                    objCharacter.SendGossip(cGUID, 2515)
                Case 34 'Skinning
                    objCharacter.SendPointOfInterest(1852.82F, -4562.31F, 6, 6, 0, "Kodohide Leatherworkers")
                    objCharacter.SendGossip(cGUID, 2516)
                Case 35 'Tailoring
                    objCharacter.SendPointOfInterest(1802.66F, -4560.66F, 6, 6, 0, "Magar's Cloth Goods")
                    objCharacter.SendGossip(cGUID, 2518)
            End Select
        End Sub
#End Region

#Region "Thunder Bluff"
        Private Sub OnGossipHello_Thunderbluff(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WINDRIDER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_MAILBOX, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_AUCTIONHOUSE, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WEAPONMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATTLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 11
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 3543, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Thunderbluff(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendPointOfInterest(-1257.8F, 24.14F, 6, 6, 0, "Thunder Bluff Bank")
                    objCharacter.SendGossip(cGUID, 1292)
                Case 2 'Wind Rider
                    objCharacter.SendPointOfInterest(-1196.43F, 28.26F, 6, 6, 0, "Wind Rider Roost")
                    objCharacter.SendGossip(cGUID, 1293)
                Case 3 'Guild Master
                    objCharacter.SendPointOfInterest(-1296.5F, 127.57F, 6, 6, 0, "Thunder Bluff Civic Information")
                    objCharacter.SendGossip(cGUID, 1291)
                Case 4 'Inn
                    objCharacter.SendPointOfInterest(-1296.0F, 39.7F, 6, 6, 0, "Thunder Bluff Inn")
                    objCharacter.SendGossip(cGUID, 3153)
                Case 5 'Mailbox
                    objCharacter.SendPointOfInterest(-1263.59F, 44.36F, 6, 6, 0, "Thunder Bluff Mailbox")
                    objCharacter.SendGossip(cGUID, 3154)
                Case 6 'Auction House
                    objCharacter.SendPointOfInterest(1381.77F, -4371.16F, 6, 6, 0, GOSSIP_TEXT_AUCTIONHOUSE)
                    objCharacter.SendGossip(cGUID, 3155)
                Case 7 'Weapon Trainer
                    objCharacter.SendPointOfInterest(-1282.31F, 89.56F, 6, 6, 0, "Ansekhwa")
                    objCharacter.SendGossip(cGUID, 4520)
                Case 8 'Stable Master
                    objCharacter.SendPointOfInterest(-1270.19F, 48.84F, 6, 6, 0, "Bulrug")
                    objCharacter.SendGossip(cGUID, 5977)
                Case 9 'Battlemasters
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALTERACVALLEY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ARATHIBASIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARSONGULCH, MenuIcon.MENUICON_GOSSIP)
                    objCharacter.TalkMenuTypes.Add(12)
                    objCharacter.TalkMenuTypes.Add(13)
                    objCharacter.TalkMenuTypes.Add(14)
                    objCharacter.SendGossip(cGUID, 7527, npcMenu)
                Case 10 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_DRUID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SHAMAN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 15 To 20
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 3542, npcMenu)
                Case 11 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 21 To 31
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 3541, npcMenu)
                Case 12 'AV
                    objCharacter.SendPointOfInterest(-1387.82F, -97.55F, 6, 6, 0, "Taim Ragetotem")
                    objCharacter.SendGossip(cGUID, 7522)
                Case 13 'AB
                    objCharacter.SendPointOfInterest(-997.0F, 214.12F, 6, 6, 0, "Martin Lindsey")
                    objCharacter.SendGossip(cGUID, 7648)
                Case 14 'WSG
                    objCharacter.SendPointOfInterest(-1384.94F, -75.91F, 6, 6, 0, "Kergul Bloodaxe")
                    objCharacter.SendGossip(cGUID, 7523)
                Case 15 'Druid
                    objCharacter.SendPointOfInterest(-1054.47F, -285.0F, 6, 6, 0, "Hall of Elders")
                    objCharacter.SendGossip(cGUID, 1294)
                Case 16 'Hunter
                    objCharacter.SendPointOfInterest(-1416.32F, -114.28F, 6, 6, 0, "Hunter's Hall")
                    objCharacter.SendGossip(cGUID, 1295)
                Case 17 'Mage
                    objCharacter.SendPointOfInterest(-1061.2F, 195.5F, 6, 6, 0, "Pools of Vision")
                    objCharacter.SendGossip(cGUID, 1296)
                Case 18 'Priest
                    objCharacter.SendPointOfInterest(-1061.2F, 195.5F, 6, 6, 0, "Pools of Vision")
                    objCharacter.SendGossip(cGUID, 1297)
                Case 19 'Shaman
                    objCharacter.SendPointOfInterest(-989.54F, 278.25F, 6, 6, 0, "Hall of Spirits")
                    objCharacter.SendGossip(cGUID, 1298)
                Case 20 'Warrior
                    objCharacter.SendPointOfInterest(-1416.32F, -114.28F, 6, 6, 0, "Hunter's Hall")
                    objCharacter.SendGossip(cGUID, 1299)
                Case 21 'Alchemy
                    objCharacter.SendPointOfInterest(-1085.56F, 27.29F, 6, 6, 0, "Bena's Alchemy")
                    objCharacter.SendGossip(cGUID, 1332)
                Case 22 'Blacksmithing
                    objCharacter.SendPointOfInterest(-1239.75F, 104.88F, 6, 6, 0, "Karn's Smithy")
                    objCharacter.SendGossip(cGUID, 1333)
                Case 23 'Cooking
                    objCharacter.SendPointOfInterest(-1214.5F, -21.23F, 6, 6, 0, "Aska's Kitchen")
                    objCharacter.SendGossip(cGUID, 1334)
                Case 24 'Enchanting
                    objCharacter.SendPointOfInterest(-1112.65F, 48.26F, 6, 6, 0, "Dawnstrider Enchanters")
                    objCharacter.SendGossip(cGUID, 1335)
                Case 25 'First Aid
                    objCharacter.SendPointOfInterest(-996.58F, 200.5F, 6, 6, 0, "Spiritual Healing")
                    objCharacter.SendGossip(cGUID, 1336)
                Case 26 'Fishing
                    objCharacter.SendPointOfInterest(-1169.35F, -68.87F, 6, 6, 0, "Mountaintop Bait & Tackle")
                    objCharacter.SendGossip(cGUID, 1337)
                Case 27 'Herbalism
                    objCharacter.SendPointOfInterest(-1137.7F, -1.51F, 6, 6, 0, "Holistic Herbalism")
                    objCharacter.SendGossip(cGUID, 1338)
                Case 28 'Leatherworking
                    objCharacter.SendPointOfInterest(-1156.22F, 66.86F, 6, 6, 0, "Thunder Bluff Armorers")
                    objCharacter.SendGossip(cGUID, 1339)
                Case 29 'Mining
                    objCharacter.SendPointOfInterest(-1249.17F, 155.0F, 6, 6, 0, "Stonehoof Geology")
                    objCharacter.SendGossip(cGUID, 1340)
                Case 30 'Skinning
                    objCharacter.SendPointOfInterest(-1148.56F, 51.18F, 6, 6, 0, "Mooranta")
                    objCharacter.SendGossip(cGUID, 1343)
                Case 31 'Tailoring
                    objCharacter.SendPointOfInterest(-1156.22F, 66.86F, 6, 6, 0, "Thunder Bluff Armorers")
                    objCharacter.SendGossip(cGUID, 1341)
            End Select
        End Sub
#End Region

#Region "Darnassus"
        Private Sub OnGossipHello_Darnassus(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_AUCTIONHOUSE, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_HIPPOGRYPH, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_MAILBOX, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WEAPONMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATTLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 11
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 3543, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Darnassus(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Auction House
                    objCharacter.SendPointOfInterest(9861.23F, 2334.55F, 6, 6, 0, "Darnassus Auction House")
                    objCharacter.SendGossip(cGUID, 3833)
                Case 2 'Bank
                    objCharacter.SendPointOfInterest(9938.45F, 2512.35F, 6, 6, 0, "Darnassus Bank")
                    objCharacter.SendGossip(cGUID, 3017)
                Case 3 'Hippogryph
                    objCharacter.SendPointOfInterest(9945.65F, 2618.94F, 6, 6, 0, "Rut'theran Village")
                    objCharacter.SendGossip(cGUID, 3018)
                Case 4 'Guild Master
                    objCharacter.SendPointOfInterest(10076.4F, 2199.59F, 6, 6, 0, "Darnassus Guild Master")
                    objCharacter.SendGossip(cGUID, 3019)
                Case 5 'Inn
                    objCharacter.SendPointOfInterest(10133.29F, 2222.52F, 6, 6, 0, "Darnassus Inn")
                    objCharacter.SendGossip(cGUID, 3020)
                Case 6 'Mailbox
                    objCharacter.SendPointOfInterest(9942.17F, 2495.48F, 6, 6, 0, "Darnassus Mailbox")
                    objCharacter.SendGossip(cGUID, 3021)
                Case 7 'Stable Master
                    objCharacter.SendPointOfInterest(10167.2F, 2522.66F, 6, 6, 0, "Alassin")
                    objCharacter.SendGossip(cGUID, 5980)
                Case 8 'Weapon Trainer
                    objCharacter.SendPointOfInterest(9907.11F, 2329.7F, 6, 6, 0, "Ilyenia Moonfire")
                    objCharacter.SendGossip(cGUID, 4517)
                Case 9 'Battlemasters
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALTERACVALLEY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ARATHIBASIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARSONGULCH, MenuIcon.MENUICON_GOSSIP)
                    objCharacter.TalkMenuTypes.Add(12)
                    objCharacter.TalkMenuTypes.Add(13)
                    objCharacter.TalkMenuTypes.Add(14)
                    objCharacter.SendGossip(cGUID, 7519, npcMenu)
                Case 10 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_DRUID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 15 To 19
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4264, npcMenu)
                Case 11 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 20 To 28
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4273, npcMenu)
                Case 12 'AV
                    objCharacter.SendPointOfInterest(9923.61F, 2327.43F, 6, 6, 0, "Brogun Stoneshield")
                    objCharacter.SendGossip(cGUID, 7518)
                Case 13 'AB
                    objCharacter.SendPointOfInterest(9977.37F, 2324.39F, 6, 6, 0, "Keras Wolfheart")
                    objCharacter.SendGossip(cGUID, 7651)
                Case 14 'WSG
                    objCharacter.SendPointOfInterest(9979.84F, 2315.79F, 6, 6, 0, "Aethalas")
                    objCharacter.SendGossip(cGUID, 7482)
                Case 15 'Druid
                    objCharacter.SendPointOfInterest(10186.0F, 2570.46F, 6, 6, 0, "Darnassus Druid Trainer")
                    objCharacter.SendGossip(cGUID, 3024)
                Case 16 'Hunter
                    objCharacter.SendPointOfInterest(10177.29F, 2511.1F, 6, 6, 0, "Darnassus Hunter Trainer")
                    objCharacter.SendGossip(cGUID, 3023)
                Case 17 'Priest
                    objCharacter.SendPointOfInterest(9659.12F, 2524.88F, 6, 6, 0, "Temple of the Moon")
                    objCharacter.SendGossip(cGUID, 3025)
                Case 18 'Rogue
                    objCharacter.SendPointOfInterest(10122.0F, 2599.12F, 6, 6, 0, "Darnassus Rogue Trainer")
                    objCharacter.SendGossip(cGUID, 3026)
                Case 19 'Warrior
                    objCharacter.SendPointOfInterest(9951.91F, 2280.38F, 6, 6, 0, "Warrior's Terrace")
                    objCharacter.SendGossip(cGUID, 3033)
                Case 20 'Alchemy
                    objCharacter.SendPointOfInterest(10075.9F, 2356.76F, 6, 6, 0, "Darnassus Alchemy Trainer")
                    objCharacter.SendGossip(cGUID, 3035)
                Case 21 'Cooking
                    objCharacter.SendPointOfInterest(10088.59F, 2419.21F, 6, 6, 0, "Darnassus Cooking Trainer")
                    objCharacter.SendGossip(cGUID, 3036)
                Case 22 'Enchanting
                    objCharacter.SendPointOfInterest(10146.09F, 2313.42F, 6, 6, 0, "Darnassus Enchanting Trainer")
                    objCharacter.SendGossip(cGUID, 3337)
                Case 23 'First Aid
                    objCharacter.SendPointOfInterest(10150.09F, 2390.43F, 6, 6, 0, "Darnassus First Aid Trainer")
                    objCharacter.SendGossip(cGUID, 3037)
                Case 24 'Fishing
                    objCharacter.SendPointOfInterest(9836.2F, 2432.17F, 6, 6, 0, "Darnassus Fishing Trainer")
                    objCharacter.SendGossip(cGUID, 3038)
                Case 25 'Herbalism
                    objCharacter.SendPointOfInterest(9757.17F, 2430.16F, 6, 6, 0, "Darnassus Herbalism Trainer")
                    objCharacter.SendGossip(cGUID, 3039)
                Case 26 'Leatherworking
                    objCharacter.SendPointOfInterest(10086.59F, 2255.77F, 6, 6, 0, "Darnassus Leatherworking Trainer")
                    objCharacter.SendGossip(cGUID, 3040)
                Case 27 'Skinning
                    objCharacter.SendPointOfInterest(10081.4F, 2257.18F, 6, 6, 0, "Darnassus Skinning Trainer")
                    objCharacter.SendGossip(cGUID, 3042)
                Case 28 'Tailoring
                    objCharacter.SendPointOfInterest(10079.7F, 2268.19F, 6, 6, 0, "Darnassus Tailor")
                    objCharacter.SendGossip(cGUID, 3044)
            End Select
        End Sub
#End Region

#Region "Ironforge"
        Private Sub OnGossipHello_Ironforge(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_AUCTIONHOUSE, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_IRONFORGE_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_DEEPRUNTRAM, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GRYPHON, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_MAILBOX, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WEAPONMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATTLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 12
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 933, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Ironforge(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Auctionhouse
                    objCharacter.SendPointOfInterest(-4957.39F, -911.6F, 6, 6, 0, "Ironforge Auction House")
                    objCharacter.SendGossip(cGUID, 3014)
                Case 2 'Bank
                    objCharacter.SendPointOfInterest(-4891.91F, -991.47F, 6, 6, 0, "The Vault")
                    objCharacter.SendGossip(cGUID, 2761)
                Case 3 'Deeprun Tram
                    objCharacter.SendPointOfInterest(-4835.27F, -1294.69F, 6, 6, 0, "Deeprun Tram")
                    objCharacter.SendGossip(cGUID, 3814)
                Case 4 'Gryphon Master
                    objCharacter.SendPointOfInterest(-4821.52, -1152.3, 6, 6, 0, "Ironforge Gryphon Master")
                    objCharacter.SendGossip(cGUID, 2762)
                Case 5 'Guild Master
                    objCharacter.SendPointOfInterest(-5021.0F, -996.45F, 6, 6, 0, "Ironforge Visitor's Center")
                    objCharacter.SendGossip(cGUID, 2764)
                Case 6 'Inn
                    objCharacter.SendPointOfInterest(-4850.47F, -872.57F, 6, 6, 0, "Stonefire Tavern")
                    objCharacter.SendGossip(cGUID, 2768)
                Case 7 'Mailbox
                    objCharacter.SendPointOfInterest(-4845.7F, -880.55F, 6, 6, 0, "Ironforge Mailbox")
                    objCharacter.SendGossip(cGUID, 2769)
                Case 8 'Stable Master
                    objCharacter.SendPointOfInterest(-5010.2F, -1262.0F, 6, 6, 0, "Ulbrek Firehand")
                    objCharacter.SendGossip(cGUID, 5986)
                Case 9 'Weapon Trainer
                    objCharacter.SendPointOfInterest(-5040.0F, -1201.88F, 6, 6, 0, "Bixi and Buliwyf")
                    objCharacter.SendGossip(cGUID, 4518)
                Case 10 'Battlemasters
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALTERACVALLEY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ARATHIBASIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARSONGULCH, MenuIcon.MENUICON_GOSSIP)
                    objCharacter.TalkMenuTypes.Add(13)
                    objCharacter.TalkMenuTypes.Add(14)
                    objCharacter.TalkMenuTypes.Add(15)
                    objCharacter.SendGossip(cGUID, 7529, npcMenu)
                Case 11 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PALADIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 16 To 22
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 2766, npcMenu)
                Case 12 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 23 To 34
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 2793, npcMenu)
                Case 13 'AV
                    objCharacter.SendPointOfInterest(-5047.87F, -1263.77F, 6, 6, 0, "Glordrum Steelbeard")
                    objCharacter.SendGossip(cGUID, 7483)
                Case 14 'AB
                    objCharacter.SendPointOfInterest(-5038.37F, -1266.39F, 6, 6, 0, "Donal Osgood")
                    objCharacter.SendGossip(cGUID, 7649)
                Case 15 'WSG
                    objCharacter.SendPointOfInterest(-5037.24F, -1274.82F, 6, 6, 0, "Lylandris")
                    objCharacter.SendGossip(cGUID, 7528)
                Case 16 'Hunter
                    objCharacter.SendPointOfInterest(-5023.0F, -1253.68F, 6, 6, 0, "Hall of Arms")
                    objCharacter.SendGossip(cGUID, 2770)
                Case 17 'Mage
                    objCharacter.SendPointOfInterest(-4627.0F, -926.45F, 6, 6, 0, "Hall of Mysteries")
                    objCharacter.SendGossip(cGUID, 2771)
                Case 18 'Paladin
                    objCharacter.SendPointOfInterest(-4627.02F, -926.45F, 6, 6, 0, "Hall of Mysteries")
                    objCharacter.SendGossip(cGUID, 2773)
                Case 19 'Priest
                    objCharacter.SendPointOfInterest(-4627.0F, -926.45F, 6, 6, 0, "Hall of Mysteries")
                    objCharacter.SendGossip(cGUID, 2772)
                Case 20 'Rogue
                    objCharacter.SendPointOfInterest(-4647.83F, -1124.0F, 6, 6, 0, "Ironforge Rogue Trainer")
                    objCharacter.SendGossip(cGUID, 2774)
                Case 21 'Warlock
                    objCharacter.SendPointOfInterest(-4605.0F, -1110.45F, 6, 6, 0, "Ironforge Warlock Trainer")
                    objCharacter.SendGossip(cGUID, 2775)
                Case 22 'Warrior
                    objCharacter.SendPointOfInterest(-5023.08F, -1253.68F, 6, 6, 0, "Hall of Arms")
                    objCharacter.SendGossip(cGUID, 2776)
                Case 23 'Alchemy
                    objCharacter.SendPointOfInterest(-4858.5F, -1241.83F, 6, 6, 0, "Berryfizz's Potions and Mixed Drinks")
                    objCharacter.SendGossip(cGUID, 2794)
                Case 24 'Blacksmithing
                    objCharacter.SendPointOfInterest(-4796.97F, -1110.17F, 6, 6, 0, "The Great Forge")
                    objCharacter.SendGossip(cGUID, 2795)
                Case 25 'Cooking
                    objCharacter.SendPointOfInterest(-4767.83F, -1184.59F, 6, 6, 0, "The Bronze Kettle")
                    objCharacter.SendGossip(cGUID, 2796)
                Case 26 'Enchanting
                    objCharacter.SendPointOfInterest(-4803.72F, -1196.53F, 6, 6, 0, "Thistlefuzz Arcanery")
                    objCharacter.SendGossip(cGUID, 2797)
                Case 27 'Engineering
                    objCharacter.SendPointOfInterest(-4799.56F, -1250.23F, 6, 6, 0, "Springspindle's Gadgets")
                    objCharacter.SendGossip(cGUID, 2798)
                Case 28 'First Aid
                    objCharacter.SendPointOfInterest(-4881.6F, -1153.13F, 6, 6, 0, "Ironforge Physician")
                    objCharacter.SendGossip(cGUID, 2799)
                Case 29 'Fishing
                    objCharacter.SendPointOfInterest(-4597.91F, -1091.93F, 6, 6, 0, "Traveling Fisherman")
                    objCharacter.SendGossip(cGUID, 2800)
                Case 30 'Herbalism
                    objCharacter.SendPointOfInterest(-4876.9F, -1151.92F, 6, 6, 0, "Ironforge Physician")
                    objCharacter.SendGossip(cGUID, 2801)
                Case 31 'Leatherworking
                    objCharacter.SendPointOfInterest(-4745.0F, -1027.57F, 6, 6, 0, "Finespindle's Leather Goods")
                    objCharacter.SendGossip(cGUID, 2802)
                Case 32 'Mining
                    objCharacter.SendPointOfInterest(-4705.06F, -1116.43F, 6, 6, 0, "Deepmountain Mining Guild")
                    objCharacter.SendGossip(cGUID, 2804)
                Case 33 'Skinning
                    objCharacter.SendPointOfInterest(-4745.0F, -1027.57F, 6, 6, 0, "Finespindle's Leather Goods")
                    objCharacter.SendGossip(cGUID, 2805)
                Case 34 'Tailoring
                    objCharacter.SendPointOfInterest(-4719.6F, -1056.96F, 6, 6, 0, "Stonebrow's Clothier")
                    objCharacter.SendGossip(cGUID, 2807)
            End Select
        End Sub
#End Region

#Region "Undercity"
        Private Sub OnGossipHello_Undercity(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATHANDLER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_MAILBOX, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_AUCTIONHOUSE, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_ZEPPLINMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WEAPONMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATTLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 12
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 3543, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Undercity(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendPointOfInterest(1595.64F, 232.45F, 6, 6, 0, "Undercity Bank")
                    objCharacter.SendGossip(cGUID, 3514)
                Case 2 'Bat Handler
                    objCharacter.SendPointOfInterest(1565.9F, 271.43F, 6, 6, 0, "Undercity Bat Handler")
                    objCharacter.SendGossip(cGUID, 3515)
                Case 3 'Guild Master
                    objCharacter.SendPointOfInterest(1594.17F, 205.57F, 6, 6, 0, "Undercity Guild Master")
                    objCharacter.SendGossip(cGUID, 3516)
                Case 4 'Inn
                    objCharacter.SendPointOfInterest(1639.43F, 220.99F, 6, 6, 0, "Undercity Inn")
                    objCharacter.SendGossip(cGUID, 3517)
                Case 5 'Mailbox
                    objCharacter.SendPointOfInterest(1632.68F, 219.4F, 6, 6, 0, "Undercity Mailbox")
                    objCharacter.SendGossip(cGUID, 3518)
                Case 6 'Auction House
                    objCharacter.SendPointOfInterest(1647.9F, 258.49F, 6, 6, 0, "Undercity Auction House")
                    objCharacter.SendGossip(cGUID, 3519)
                Case 7 'Zeppelin
                    objCharacter.SendPointOfInterest(2059.0F, 274.86F, 6, 6, 0, "Undercity Zeppelin")
                    objCharacter.SendGossip(cGUID, 3520)
                Case 8 'Weapon Trainer
                    objCharacter.SendPointOfInterest(1670.31F, 324.66F, 6, 6, 0, "Archibald")
                    objCharacter.SendGossip(cGUID, 4521)
                Case 9 'Stable Master
                    objCharacter.SendPointOfInterest(1634.18F, 226.76F, 6, 6, 0, "Anya Maulray")
                    objCharacter.SendGossip(cGUID, 5979)
                Case 10 'Battlemasters
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALTERACVALLEY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ARATHIBASIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARSONGULCH, MenuIcon.MENUICON_GOSSIP)
                    objCharacter.TalkMenuTypes.Add(13)
                    objCharacter.TalkMenuTypes.Add(14)
                    objCharacter.TalkMenuTypes.Add(15)
                    objCharacter.SendGossip(cGUID, 7527, npcMenu)
                Case 11 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 16 To 20
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 3542, npcMenu)
                Case 12 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 21 To 32
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 3541, npcMenu)
                Case 13 'AV
                    objCharacter.SendPointOfInterest(1329.0F, 333.92F, 6, 6, 0, "Grizzle Halfmane")
                    objCharacter.SendGossip(cGUID, 7525)
                Case 14 'AB
                    objCharacter.SendPointOfInterest(1283.3F, 287.16F, 6, 6, 0, "Sir Malory Wheeler")
                    objCharacter.SendGossip(cGUID, 7646)
                Case 15 'WSG
                    objCharacter.SendPointOfInterest(1265.0F, 351.18F, 6, 6, 0, "Kurden Bloodclaw")
                    objCharacter.SendGossip(cGUID, 7526)
                Case 16 'Mage
                    objCharacter.SendPointOfInterest(1781.0F, 53.0F, 6, 6, 0, "Undercity Mage Trainers")
                    objCharacter.SendGossip(cGUID, 3513)
                Case 17 'Priest
                    objCharacter.SendPointOfInterest(1758.33F, 401.5F, 6, 6, 0, "Undercity Priest Trainers")
                    objCharacter.SendGossip(cGUID, 3521)
                Case 18 'Rogue
                    objCharacter.SendPointOfInterest(1418.56F, 65.0F, 6, 6, 0, "Undercity Rogue Trainers")
                    objCharacter.SendGossip(cGUID, 3524)
                Case 19 'Warlock
                    objCharacter.SendPointOfInterest(1780.92F, 53.16F, 6, 6, 0, "Undercity Warlock Trainers")
                    objCharacter.SendGossip(cGUID, 3526)
                Case 20 'Warrior
                    objCharacter.SendPointOfInterest(1775.59F, 418.19F, 6, 6, 0, "Undercity Warrior Trainers")
                    objCharacter.SendGossip(cGUID, 3527)
                Case 21 'Alchemy
                    objCharacter.SendPointOfInterest(1419.82F, 417.19F, 6, 6, 0, "The Apothecarium")
                    objCharacter.SendGossip(cGUID, 3528)
                Case 22 'Blacksmithing
                    objCharacter.SendPointOfInterest(1696.0F, 285.0F, 6, 6, 0, "Undercity Blacksmithing Trainer")
                    objCharacter.SendGossip(cGUID, 3529)
                Case 23 'Cooking
                    objCharacter.SendPointOfInterest(1596.34F, 274.68F, 6, 6, 0, "Undercity Cooking Trainer")
                    objCharacter.SendGossip(cGUID, 3530)
                Case 24 'Enchanting
                    objCharacter.SendPointOfInterest(1488.54F, 280.19F, 6, 6, 0, "Undercity Enchanting Trainer")
                    objCharacter.SendGossip(cGUID, 3531)
                Case 25 'Engineering
                    objCharacter.SendPointOfInterest(1408.58F, 143.43F, 6, 6, 0, "Undercity Engineering Trainer")
                    objCharacter.SendGossip(cGUID, 3532)
                Case 26 'First Aid
                    objCharacter.SendPointOfInterest(1519.65F, 167.19F, 6, 6, 0, "Undercity First Aid Trainer")
                    objCharacter.SendGossip(cGUID, 3533)
                Case 27 'Fishing
                    objCharacter.SendPointOfInterest(1679.9F, 89.0F, 6, 6, 0, "Undercity Fishing Trainer")
                    objCharacter.SendGossip(cGUID, 3534)
                Case 28 'Herbalism
                    objCharacter.SendPointOfInterest(1558.0F, 349.36F, 6, 6, 0, "Undercity Herbalism Trainer")
                    objCharacter.SendGossip(cGUID, 3535)
                Case 29 'Leatherworking
                    objCharacter.SendPointOfInterest(1498.76F, 196.43F, 6, 6, 0, "Undercity Leatherworking Trainer")
                    objCharacter.SendGossip(cGUID, 3536)
                Case 30 'Mining
                    objCharacter.SendPointOfInterest(1642.88F, 335.58F, 6, 6, 0, "Undercity Mining Trainer")
                    objCharacter.SendGossip(cGUID, 3537)
                Case 31 'Skinning
                    objCharacter.SendPointOfInterest(1498.6F, 196.46F, 6, 6, 0, "Undercity Skinning Trainer")
                    objCharacter.SendGossip(cGUID, 3538)
                Case 32 'Tailoring
                    objCharacter.SendPointOfInterest(1689.55F, 193.0F, 6, 6, 0, "Undercity Tailoring Trainer")
                    objCharacter.SendGossip(cGUID, 3539)
            End Select
        End Sub
#End Region

#Region "Mulgore"
        Private Sub OnGossipHello_Mulgore(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WINDRIDER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 6
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 3543, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Mulgore(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendGossip(cGUID, 4051)
                Case 2 'Wind Rider
                    objCharacter.SendGossip(cGUID, 4052)
                Case 3 'Inn
                    objCharacter.SendPointOfInterest(-2361.38F, -349.19F, 6, 6, 0, "Bloodhoof Village Inn")
                    objCharacter.SendGossip(cGUID, 4053)
                Case 4 'Stable Master
                    objCharacter.SendPointOfInterest(-2338.86F, -357.56F, 6, 6, 0, "Seikwa")
                    objCharacter.SendGossip(cGUID, 5976)
                Case 5 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_DRUID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SHAMAN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 7 To 10
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4069, npcMenu)
                Case 6 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 11 To 21
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4070, npcMenu)
                Case 7 'Druid
                    objCharacter.SendPointOfInterest(-2312.15F, -443.69F, 6, 6, 0, "Gennia Runetotem")
                    objCharacter.SendGossip(cGUID, 4054)
                Case 8 'Hunter
                    objCharacter.SendPointOfInterest(-2178.14F, -406.14F, 6, 6, 0, "Yaw Sharpmane")
                    objCharacter.SendGossip(cGUID, 4055)
                Case 9 'Shaman
                    objCharacter.SendPointOfInterest(-2301.5F, -439.87F, 6, 6, 0, "Narm Skychaser")
                    objCharacter.SendGossip(cGUID, 4056)
                Case 10 'Warrior
                    objCharacter.SendPointOfInterest(-2345.43F, -494.11F, 6, 6, 0, "Krang Stonehoof")
                    objCharacter.SendGossip(cGUID, 4057)
                Case 11 'Alchemy
                    objCharacter.SendGossip(cGUID, 4058)
                Case 12 'Blacksmithing
                    objCharacter.SendGossip(cGUID, 4059)
                Case 13 'Cooking
                    objCharacter.SendPointOfInterest(-2263.34F, -287.91F, 6, 6, 0, "Pyall Silentstride")
                    objCharacter.SendGossip(cGUID, 4060)
                Case 14 'Enchanting
                    objCharacter.SendGossip(cGUID, 4061)
                Case 15 'First Aid
                    objCharacter.SendPointOfInterest(-2353.52F, -355.82F, 6, 6, 0, "Vira Younghoof")
                    objCharacter.SendGossip(cGUID, 4062)
                Case 16 'Fishing
                    objCharacter.SendPointOfInterest(-2349.21F, -241.37F, 6, 6, 0, "Uthan Stillwater")
                    objCharacter.SendGossip(cGUID, 4063)
                Case 17 'Herbalism
                    objCharacter.SendGossip(cGUID, 4064)
                Case 18 'Leatherworking
                    objCharacter.SendPointOfInterest(-2257.12F, -288.63F, 6, 6, 0, "Chaw Stronghide")
                    objCharacter.SendGossip(cGUID, 4065)
                Case 19 'Mining
                    objCharacter.SendGossip(cGUID, 4066)
                Case 20 'Skinning
                    objCharacter.SendPointOfInterest(-2252.94F, -291.32F, 6, 6, 0, "Yonn Deepcut")
                    objCharacter.SendGossip(cGUID, 4067)
                Case 21 'Tailoring
                    objCharacter.SendGossip(cGUID, 4068)
            End Select
        End Sub
#End Region

#Region "Durotar"
        Private Sub OnGossipHello_Durotar(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_WINDRIDER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 6
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 4037, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Durotar(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendGossip(cGUID, 4032)
                Case 2 'Wind Rider
                    objCharacter.SendGossip(cGUID, 4033)
                Case 3 'Inn
                    objCharacter.SendPointOfInterest(338.7F, -4688.87F, 6, 6, 0, "Razor Hill Inn")
                    objCharacter.SendGossip(cGUID, 4034)
                Case 4 'Stable Master
                    objCharacter.SendPointOfInterest(330.31F, -4710.66F, 6, 6, 0, "Shoja'my")
                    objCharacter.SendGossip(cGUID, 5973)
                Case 5 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SHAMAN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 7 To 13
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4035, npcMenu)
                Case 6 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 14 To 25
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4036, npcMenu)
                Case 7 'Hunter
                    objCharacter.SendPointOfInterest(276.0F, -4706.72F, 6, 6, 0, "Thotar")
                    objCharacter.SendGossip(cGUID, 4013)
                Case 8 'Mage
                    objCharacter.SendPointOfInterest(-839.33F, -4935.6F, 6, 6, 0, "Un'Thuwa")
                    objCharacter.SendGossip(cGUID, 4014)
                Case 9 'Priest
                    objCharacter.SendPointOfInterest(296.22F, -4828.1F, 6, 6, 0, "Tai'jin")
                    objCharacter.SendGossip(cGUID, 4015)
                Case 10 'Rogue
                    objCharacter.SendPointOfInterest(265.76F, -4709.0F, 6, 6, 0, "Kaplak")
                    objCharacter.SendGossip(cGUID, 4016)
                Case 11 'Shaman
                    objCharacter.SendPointOfInterest(307.79F, -4836.97F, 6, 6, 0, "Swart")
                    objCharacter.SendGossip(cGUID, 4017)
                Case 12 'Warlock
                    objCharacter.SendPointOfInterest(355.88F, -4836.45F, 6, 6, 0, "Dhugru Gorelust")
                    objCharacter.SendGossip(cGUID, 4018)
                Case 13 'Warrior
                    objCharacter.SendPointOfInterest(312.3F, -4824.66F, 6, 6, 0, "Tarshaw Jaggedscar")
                    objCharacter.SendGossip(cGUID, 4019)
                Case 14 'Alchemy
                    objCharacter.SendPointOfInterest(-800.25F, -4894.33F, 6, 6, 0, "Miao'zan")
                    objCharacter.SendGossip(cGUID, 4020)
                Case 15 'Blacksmithing
                    objCharacter.SendPointOfInterest(373.24F, -4716.45F, 6, 6, 0, "Dwukk")
                    objCharacter.SendGossip(cGUID, 4021)
                Case 16 'Cooking
                    objCharacter.SendGossip(cGUID, 4022)
                Case 17 'Enchanting
                    objCharacter.SendGossip(cGUID, 4023)
                Case 18 'Engineering
                    objCharacter.SendPointOfInterest(368.95F, -4723.95F, 6, 6, 0, "Mukdrak")
                    objCharacter.SendGossip(cGUID, 4024)
                Case 19 'First Aid
                    objCharacter.SendPointOfInterest(327.17F, -4825.62F, 6, 6, 0, "Rawrk")
                    objCharacter.SendGossip(cGUID, 4025)
                Case 20 'Fishing
                    objCharacter.SendPointOfInterest(-1065.48F, -4777.43F, 6, 6, 0, "Lau'Tiki")
                    objCharacter.SendGossip(cGUID, 4026)
                Case 21 'Herbalism
                    objCharacter.SendPointOfInterest(-836.25F, -4896.89F, 6, 6, 0, "Mishiki")
                    objCharacter.SendGossip(cGUID, 4027)
                Case 22 'Leatherworking
                    objCharacter.SendGossip(cGUID, 4028)
                Case 23 'Mining
                    objCharacter.SendPointOfInterest(366.94F, -4705.0F, 6, 6, 0, "Krunn")
                    objCharacter.SendGossip(cGUID, 4029)
                Case 24 'Skinning
                    objCharacter.SendGossip(cGUID, 4030)
                Case 25 'Tailoring
                    objCharacter.SendGossip(cGUID, 4031)
            End Select
        End Sub
#End Region

#Region "Elwynn Forest"
        Private Sub OnGossipHello_ElwynnForest(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GRYPHON, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 7
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 933, npcMenu)
        End Sub

        Private Sub OnGossipSelect_ElwynnForest(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendGossip(cGUID, 4260)
                Case 2 'Gryphon Master
                    objCharacter.SendGossip(cGUID, 4261)
                Case 3 'Guild Master
                    objCharacter.SendGossip(cGUID, 4262)
                Case 4 'Inn
                    objCharacter.SendPointOfInterest(-9459.34F, 42.08F, 6, 6, 0, "Lion's Pride Inn")
                    objCharacter.SendGossip(cGUID, 4263)
                Case 5 'Stable Master
                    objCharacter.SendPointOfInterest(-9466.62F, 45.87F, 6, 6, 0, "Erma")
                    objCharacter.SendGossip(cGUID, 5983)
                Case 6 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_DRUID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PALADIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 8 To 15
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4264, npcMenu)
                Case 7 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 16 To 27
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4036, npcMenu)
                Case 8 'Druid
                    objCharacter.SendGossip(cGUID, 4265)
                Case 9 'Hunter
                    objCharacter.SendGossip(cGUID, 4266)
                Case 10 'Mage
                    objCharacter.SendPointOfInterest(-9471.12F, 33.44F, 6, 6, 0, "Zaldimar Wefhellt")
                    objCharacter.SendGossip(cGUID, 4015)
                Case 11 'Paladin
                    objCharacter.SendPointOfInterest(-9469.0F, 108.05F, 6, 6, 0, "Brother Wilhelm")
                    objCharacter.SendGossip(cGUID, 4269)
                Case 12 'Priest
                    objCharacter.SendPointOfInterest(-9461.07F, 32.6F, 6, 6, 0, "Priestess Josetta")
                    objCharacter.SendGossip(cGUID, 4267)
                Case 13 'Rogue
                    objCharacter.SendPointOfInterest(-9465.13F, 13.29F, 6, 6, 0, "Keryn Sylvius")
                    objCharacter.SendGossip(cGUID, 4270)
                Case 14 'Warlock
                    objCharacter.SendPointOfInterest(-9473.21F, -4.08F, 6, 6, 0, "Maximillian Crowe")
                    objCharacter.SendGossip(cGUID, 4272)
                Case 15 'Warrior
                    objCharacter.SendPointOfInterest(-9461.82F, 109.5F, 6, 6, 0, "Lyria Du Lac")
                    objCharacter.SendGossip(cGUID, 4271)
                Case 16 'Alchemy
                    objCharacter.SendPointOfInterest(-9057.04F, 153.63F, 6, 6, 0, "Alchemist Mallory")
                    objCharacter.SendGossip(cGUID, 4274)
                Case 17 'Blacksmithing
                    objCharacter.SendPointOfInterest(-9456.58F, 87.9F, 6, 6, 0, "Smith Argus")
                    objCharacter.SendGossip(cGUID, 4275)
                Case 18 'Cooking
                    objCharacter.SendPointOfInterest(-9467.54F, -3.16F, 6, 6, 0, "Tomas")
                    objCharacter.SendGossip(cGUID, 4276)
                Case 19 'Enchanting
                    objCharacter.SendGossip(cGUID, 4277)
                Case 20 'Engineering
                    objCharacter.SendGossip(cGUID, 4278)
                Case 21 'First Aid
                    objCharacter.SendPointOfInterest(-9456.82F, 30.49F, 6, 6, 0, "Michelle Belle")
                    objCharacter.SendGossip(cGUID, 4279)
                Case 22 'Fishing
                    objCharacter.SendPointOfInterest(-9386.54F, -118.73F, 6, 6, 0, "Lee Brown")
                    objCharacter.SendGossip(cGUID, 4280)
                Case 23 'Herbalism
                    objCharacter.SendPointOfInterest(-9060.7F, 149.23F, 6, 6, 0, "Herbalist Pomeroy")
                    objCharacter.SendGossip(cGUID, 4281)
                Case 24 'Leatherworking
                    objCharacter.SendPointOfInterest(-9376.12F, -75.23F, 6, 6, 0, "Adele Fielder")
                    objCharacter.SendGossip(cGUID, 4282)
                Case 25 'Mining
                    objCharacter.SendGossip(cGUID, 4283)
                Case 26 'Skinning
                    objCharacter.SendPointOfInterest(-9536.91F, -1212.76F, 6, 6, 0, "Helene Peltskinner")
                    objCharacter.SendGossip(cGUID, 4284)
                Case 27 'Tailoring
                    objCharacter.SendPointOfInterest(-9376.12F, -75.23F, 6, 6, 0, "Eldrin")
                    objCharacter.SendGossip(cGUID, 4285)
            End Select
        End Sub
#End Region

#Region "Dun Morogh"
        Private Sub OnGossipHello_DunMorogh(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_HIPPOGRYPH, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 7
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 4287, npcMenu)
        End Sub

        Private Sub OnGossipSelect_DunMorogh(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendGossip(cGUID, 4288)
                Case 2 'Gryphon Master
                    objCharacter.SendGossip(cGUID, 4289)
                Case 3 'Guild Master
                    objCharacter.SendGossip(cGUID, 4290)
                Case 4 'Inn
                    objCharacter.SendPointOfInterest(-5582.66F, -525.89F, 6, 6, 0, "Thunderbrew Distillery")
                    objCharacter.SendGossip(cGUID, 4291)
                Case 5 'Stable Master
                    objCharacter.SendPointOfInterest(-5604.0F, -509.58F, 6, 6, 0, "Shelby Stoneflint")
                    objCharacter.SendGossip(cGUID, 5985)
                Case 6 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PALADIN, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 8 To 14
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4292, npcMenu)
                Case 7 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 15 To 26
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4300, npcMenu)
                Case 8 'Hunter
                    objCharacter.SendPointOfInterest(-5618.29F, -454.25F, 6, 6, 0, "Grif Wildheart")
                    objCharacter.SendGossip(cGUID, 4293)
                Case 9 'Mage
                    objCharacter.SendPointOfInterest(-5585.6F, -539.99F, 6, 6, 0, "Magis Sparkmantle")
                    objCharacter.SendGossip(cGUID, 4266)
                Case 10 'Paladin
                    objCharacter.SendPointOfInterest(-5585.6F, -539.99F, 6, 6, 0, "Azar Stronghammer")
                    objCharacter.SendGossip(cGUID, 4295)
                Case 11 'Priest
                    objCharacter.SendPointOfInterest(-5591.74F, -525.61F, 6, 6, 0, "Maxan Anvol")
                    objCharacter.SendGossip(cGUID, 4296)
                Case 12 'Rogue
                    objCharacter.SendPointOfInterest(-5602.75F, -542.4F, 6, 6, 0, "Hogral Bakkan")
                    objCharacter.SendGossip(cGUID, 4267)
                Case 13 'Warlock
                    objCharacter.SendPointOfInterest(-5641.97F, -523.76F, 6, 6, 0, "Gimrizz Shadowcog")
                    objCharacter.SendGossip(cGUID, 4298)
                Case 14 'Warrior
                    objCharacter.SendPointOfInterest(-5604.79F, -529.38F, 6, 6, 0, "Granis Swiftaxe")
                    objCharacter.SendGossip(cGUID, 4299)
                Case 15 'Alchemy
                    objCharacter.SendGossip(cGUID, 4301)
                Case 16 'Blacksmithing
                    objCharacter.SendPointOfInterest(-5584.72F, -428.41F, 6, 6, 0, "Tognus Flintfire")
                    objCharacter.SendGossip(cGUID, 4302)
                Case 17 'Cooking
                    objCharacter.SendPointOfInterest(-5596.85F, -541.43F, 6, 6, 0, "Gremlock Pilsnor")
                    objCharacter.SendGossip(cGUID, 4303)
                Case 18 'Enchanting
                    objCharacter.SendGossip(cGUID, 4304)
                Case 19 'Engineering
                    objCharacter.SendPointOfInterest(-5531.0F, -666.53F, 6, 6, 0, "Bronk Guzzlegear")
                    objCharacter.SendGossip(cGUID, 4305)
                Case 20 'First Aid
                    objCharacter.SendPointOfInterest(-5603.67F, -523.57F, 6, 6, 0, "Thamner Pol")
                    objCharacter.SendGossip(cGUID, 4306)
                Case 21 'Fishing
                    objCharacter.SendPointOfInterest(-5199.9F, 58.58F, 6, 6, 0, "Paxton Ganter")
                    objCharacter.SendGossip(cGUID, 4307)
                Case 22 'Herbalism
                    objCharacter.SendGossip(cGUID, 4308)
                Case 23 'Leatherworking
                    objCharacter.SendGossip(cGUID, 4310)
                Case 24 'Mining
                    objCharacter.SendPointOfInterest(-5531, -666.53, 6, 6, 0, "Yarr Hamerstone")
                    objCharacter.SendGossip(cGUID, 4311)
                Case 25 'Skinning
                    objCharacter.SendGossip(cGUID, 4312)
                Case 26 'Tailoring
                    objCharacter.SendGossip(cGUID, 4313)
            End Select
        End Sub
#End Region

#Region "Tirisfall Glades"
        Private Sub OnGossipHello_Tirisfall(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_BATHANDLER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 6
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 4097, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Tirisfall(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendGossip(cGUID, 4074)
                Case 2 'Bat Handler
                    objCharacter.SendGossip(cGUID, 4075)
                Case 3 'Inn
                    objCharacter.SendPointOfInterest(2246.68F, 241.89F, 6, 6, 0, "Gallows` End Tavern")
                    objCharacter.SendGossip(cGUID, 4076)
                Case 4 'Stable Master
                    objCharacter.SendPointOfInterest(2267.66F, 319.32F, 6, 6, 0, "Morganus")
                    objCharacter.SendGossip(cGUID, 5978)
                Case 5 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_MAGE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARLOCK, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 7 To 11
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4292, npcMenu)
                Case 6 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_BLACKSMITHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENGINEERING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_MINING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 12 To 23
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4096, npcMenu)
                Case 7 'Mage
                    objCharacter.SendPointOfInterest(2259.18F, 240.93F, 6, 6, 0, "Cain Firesong")
                    objCharacter.SendGossip(cGUID, 4077)
                Case 8 'Priest
                    objCharacter.SendPointOfInterest(2259.18F, 240.93F, 6, 6, 0, "Dark Cleric Beryl")
                    objCharacter.SendGossip(cGUID, 4078)
                Case 9 'Rogue
                    objCharacter.SendPointOfInterest(2259.18F, 240.93F, 6, 6, 0, "Marion Call")
                    objCharacter.SendGossip(cGUID, 4079)
                Case 10 'Warlock
                    objCharacter.SendPointOfInterest(2259.18F, 240.93F, 6, 6, 0, "Rupert Boch")
                    objCharacter.SendGossip(cGUID, 4080)
                Case 11 'Warrior
                    objCharacter.SendPointOfInterest(2256.48F, 240.32F, 6, 6, 0, "Austil de Mon")
                    objCharacter.SendGossip(cGUID, 4081)
                Case 12 'Alchemy
                    objCharacter.SendPointOfInterest(2263.25, 344.23, 6, 6, 0, "Carolai Anise")
                    objCharacter.SendGossip(cGUID, 4082)
                Case 13 'Blacksmithing
                    objCharacter.SendGossip(cGUID, 4083)
                Case 14 'Cooking
                    objCharacter.SendGossip(cGUID, 4084)
                Case 15 'Enchanting
                    objCharacter.SendPointOfInterest(2250.35F, 249.12F, 6, 6, 0, "Vance Undergloom")
                    objCharacter.SendGossip(cGUID, 4085)
                Case 16 'Engineering
                    objCharacter.SendGossip(cGUID, 4086)
                Case 17 'First Aid
                    objCharacter.SendPointOfInterest(2246.68F, 241.89F, 6, 6, 0, "Nurse Neela")
                    objCharacter.SendGossip(cGUID, 4087)
                Case 18 'Fishing
                    objCharacter.SendPointOfInterest(2292.37F, -10.72F, 6, 6, 0, "Clyde Kellen")
                    objCharacter.SendGossip(cGUID, 4088)
                Case 19 'Herbalism
                    objCharacter.SendPointOfInterest(2268.21F, 331.69F, 6, 6, 0, "Faruza")
                    objCharacter.SendGossip(cGUID, 4089)
                Case 20 'Leatherworking
                    objCharacter.SendPointOfInterest(2027.0F, 78.72F, 6, 6, 0, "Shelene Rhobart")
                    objCharacter.SendGossip(cGUID, 4090)
                Case 21 'Mining
                    objCharacter.SendGossip(cGUID, 4091)
                Case 22 'Skinning
                    objCharacter.SendPointOfInterest(2027.0F, 78.72F, 6, 6, 0, "Rand Rhobart")
                    objCharacter.SendGossip(cGUID, 4092)
                Case 23 'Tailoring
                    objCharacter.SendPointOfInterest(2160.45F, 659.93F, 6, 6, 0, "Bowen Brisboise")
                    objCharacter.SendGossip(cGUID, 4093)
            End Select
        End Sub
#End Region

#Region "Teldrassil"
        Private Sub OnGossipHello_Teldrassil(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
            Dim npcMenu As New GossipMenu
            objCharacter.TalkMenuTypes.Clear()
            npcMenu.AddMenu(GOSSIP_TEXT_BANK, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_FERRY, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_GUILDMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_INN, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_STABLEMASTER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_CLASSTRAINER, MenuIcon.MENUICON_GOSSIP)
            npcMenu.AddMenu(GOSSIP_TEXT_PROFTRAINER, MenuIcon.MENUICON_GOSSIP)

            For i As Integer = 1 To 7
                objCharacter.TalkMenuTypes.Add(i)
            Next

            objCharacter.SendGossip(cGUID, 4316, npcMenu)
        End Sub

        Private Sub OnGossipSelect_Teldrassil(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal Selected As Integer)
            'TODO: These hardcoded values need to be replaced by values from either the DB or DBC's
            Select Case objCharacter.TalkMenuTypes(Selected)
                Case 1 'Bank
                    objCharacter.SendGossip(cGUID, 4317)
                Case 2 'Ferry
                    objCharacter.SendGossip(cGUID, 4318)
                Case 3 'Guild Master
                    objCharacter.SendGossip(cGUID, 4319)
                Case 4 'Inn
                    objCharacter.SendPointOfInterest(9821.49F, 960.13F, 6, 6, 0, "Dolanaar Inn")
                    objCharacter.SendGossip(cGUID, 4320)
                Case 5 'Stable Master
                    objCharacter.SendPointOfInterest(9808.37F, 931.1F, 6, 6, 0, "Seriadne")
                    objCharacter.SendGossip(cGUID, 5982)
                Case 6 'Class trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_DRUID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HUNTER, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_PRIEST, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ROGUE, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_WARRIOR, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 8 To 12
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4264, npcMenu)
                Case 7 'Profession trainers
                    Dim npcMenu As New GossipMenu
                    objCharacter.TalkMenuTypes.Clear()
                    npcMenu.AddMenu(GOSSIP_TEXT_ALCHEMY, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_COOKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_ENCHANTING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FIRSTAID, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_FISHING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_HERBALISM, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_LEATHERWORKING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_SKINNING, MenuIcon.MENUICON_GOSSIP)
                    npcMenu.AddMenu(GOSSIP_TEXT_TAILORING, MenuIcon.MENUICON_GOSSIP)
                    For i As Integer = 13 To 21
                        objCharacter.TalkMenuTypes.Add(i)
                    Next
                    objCharacter.SendGossip(cGUID, 4273, npcMenu)
                Case 8 'Druid
                    objCharacter.SendPointOfInterest(9741.58F, 963.7F, 6, 6, 0, "Kal")
                    objCharacter.SendGossip(cGUID, 4323)
                Case 9 'Hunter
                    objCharacter.SendPointOfInterest(9815.12F, 926.28F, 6, 6, 0, "Dazalar")
                    objCharacter.SendGossip(cGUID, 4324)
                Case 10 'Priest
                    objCharacter.SendPointOfInterest(9906.16F, 986.63F, 6, 6, 0, "Laurna Morninglight")
                    objCharacter.SendGossip(cGUID, 4325)
                Case 11 'Rogue
                    objCharacter.SendPointOfInterest(9789.0F, 942.86F, 6, 6, 0, "Jannok Breezesong")
                    objCharacter.SendGossip(cGUID, 4326)
                Case 12 'Warrior
                    objCharacter.SendPointOfInterest(9821.96F, 950.61F, 6, 6, 0, "Kyra Windblade")
                    objCharacter.SendGossip(cGUID, 4327)
                Case 13 'Alchemy
                    objCharacter.SendPointOfInterest(9767.59F, 878.81F, 6, 6, 0, "Cyndra Kindwhisper")
                    objCharacter.SendGossip(cGUID, 4329)
                Case 14 'Cooking
                    objCharacter.SendPointOfInterest(9751.19F, 906.13F, 6, 6, 0, "Zarrin")
                    objCharacter.SendGossip(cGUID, 4330)
                Case 15 'Enchanting
                    objCharacter.SendPointOfInterest(10677.59F, 1946.56F, 6, 6, 0, "Alanna Raveneye")
                    objCharacter.SendGossip(cGUID, 4331)
                Case 16 'First Aid
                    objCharacter.SendPointOfInterest(9903.12F, 999.0F, 6, 6, 0, "Byancie")
                    objCharacter.SendGossip(cGUID, 4332)
                Case 17 'Fishing
                    objCharacter.SendGossip(cGUID, 4333)
                Case 18 'Herbalism
                    objCharacter.SendPointOfInterest(9773.78F, 875.88F, 6, 6, 0, "Malorne Bladeleaf")
                    objCharacter.SendGossip(cGUID, 4334)
                Case 19 'Leatherworking
                    objCharacter.SendPointOfInterest(10152.59F, 1681.46F, 6, 6, 0, "Nadyia Maneweaver")
                    objCharacter.SendGossip(cGUID, 4335)
                Case 20 'Skinning
                    objCharacter.SendPointOfInterest(10135.59F, 1673.18F, 6, 6, 0, "Radnaal Maneweaver")
                    objCharacter.SendGossip(cGUID, 4336)
                Case 21 'Tailoring
                    objCharacter.SendGossip(cGUID, 4337)
            End Select
        End Sub
#End Region

    End Class

End Module