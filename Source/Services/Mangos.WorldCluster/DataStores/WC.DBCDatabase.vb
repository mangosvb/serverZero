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
Imports System.Data
Imports System.IO
Imports Mangos.Common
Imports Mangos.Shared

Namespace DataStores
    Public Module WS_DBCDatabase

        Private ReadOnly MapDBC As String = "dbc" & Path.DirectorySeparatorChar & "Map.dbc"
        Public Maps As New Dictionary(Of Integer, MapInfo)
        Public Sub InitializeMaps()
            Try

                Dim data As DBC.BufferedDbc = New DBC.BufferedDbc(MapDBC)
                For i As Integer = 0 To New DBC.BufferedDbc(MapDBC).Rows - 1
                    Dim m As New MapInfo With {
                        .ID = data.Item(i, 0, DBCValueType.DBC_INTEGER),
                        .Type = data.Item(i, 2, DBCValueType.DBC_INTEGER),
                        .Name = data.Item(i, 4, DBCValueType.DBC_STRING),
                        .ParentMap = data.Item(i, 3, DBCValueType.DBC_INTEGER),
                        .ResetTime = data.Item(i, 38, DBCValueType.DBC_INTEGER)
                    }

                    Maps.Add(m.ID, m)
                Next i

                Log.WriteLine(LogType.INFORMATION, "DBC: {0} Maps Initialized.", New DBC.BufferedDbc(MapDBC).Rows - 1)
                Call New DBC.BufferedDbc(MapDBC).Dispose()
            Catch e As DirectoryNotFoundException
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("DBC File : Maps.dbc missing.")
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub

        Public Class MapInfo
            Public ID As Integer
            Public Type As MapTypes = MapTypes.MAP_COMMON
            Public Name As String = ""
            Public ParentMap As Integer = -1
            Public ResetTime As Integer = 0

            Public ReadOnly Property IsDungeon() As Boolean
                Get
                    Return Type = MapTypes.MAP_INSTANCE OrElse Type = MapTypes.MAP_RAID
                End Get
            End Property

            Public ReadOnly Property IsRaid() As Boolean
                Get
                    Return Type = MapTypes.MAP_RAID
                End Get
            End Property

            Public ReadOnly Property IsBattleGround() As Boolean
                Get
                    Return Type = MapTypes.MAP_BATTLEGROUND
                End Get
            End Property

            Public ReadOnly Property HasResetTime() As Boolean
                Get
                    Return (ResetTime <> 0)
                End Get
            End Property

        End Class

        Private ReadOnly WorldSafeLocsDBC As String = "dbc" & Path.DirectorySeparatorChar & "WorldSafeLocs.dbc"
        Public WorldSafeLocs As New Dictionary(Of Integer, TWorldSafeLoc)
        Public Sub InitializeWorldSafeLocs()
            Try

                Dim data As DBC.BufferedDbc = New DBC.BufferedDbc(WorldSafeLocsDBC)
                For i As Integer = 0 To New DBC.BufferedDbc(WorldSafeLocsDBC).Rows - 1
                    Dim WorldSafeLoc As New TWorldSafeLoc With {
                        .ID = data.Item(i, 0, DBCValueType.DBC_INTEGER),
                        .map = data.Item(i, 1),
                        .x = data.Item(i, 2, DBCValueType.DBC_FLOAT),
                        .y = data.Item(i, 3, DBCValueType.DBC_FLOAT),
                        .z = data.Item(i, 4, DBCValueType.DBC_FLOAT)
                    }

                    WorldSafeLocs.Add(WorldSafeLoc.ID, WorldSafeLoc)
                Next i

                Log.WriteLine(LogType.INFORMATION, "DBC: {0} WorldSafeLocs Initialized.", New DBC.BufferedDbc(WorldSafeLocsDBC).Rows - 1)
                Call New DBC.BufferedDbc(WorldSafeLocsDBC).Dispose()
            Catch e As DirectoryNotFoundException
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("DBC File : WorldSafeLocs.dbc missing.")
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub

        Public Class TWorldSafeLoc
            Public ID As Integer

            Public map As UInteger
            Public x As Single
            Public y As Single
            Public z As Single
        End Class

        Public Battlegrounds As New Dictionary(Of Byte, TBattleground)
        Public Sub InitializeBattlegrounds()
            Dim Entry As Byte

            Dim MySQLQuery As New DataTable
            WorldDatabase.Query(String.Format("SELECT * FROM battleground_template"), MySQLQuery)

            For Each row As DataRow In MySQLQuery.Rows
                Entry = row.Item("id")
                Battlegrounds.Add(Entry, New TBattleground)

                'TODO: the MAPId needs to be located from somewhere other than the template file
                'BUG: THIS IS AN UGLY HACK UNTIL THE ABOVE IS FIXED
                '            Battlegrounds(Entry).Map = row.Item("Map")
                Battlegrounds(Entry).MinPlayersPerTeam = row.Item("MinPlayersPerTeam")
                Battlegrounds(Entry).MaxPlayersPerTeam = row.Item("MaxPlayersPerTeam")
                Battlegrounds(Entry).MinLevel = row.Item("MinLvl")
                Battlegrounds(Entry).MaxLevel = row.Item("MaxLvl")
                Battlegrounds(Entry).AllianceStartLoc = row.Item("AllianceStartLoc")
                Battlegrounds(Entry).AllianceStartO = row.Item("AllianceStartO")
                Battlegrounds(Entry).HordeStartLoc = row.Item("HordeStartLoc")
                Battlegrounds(Entry).HordeStartO = row.Item("HordeStartO")
            Next

            Log.WriteLine(LogType.INFORMATION, "World: {0} Battlegrounds Initialized.", MySQLQuery.Rows.Count)
        End Sub

        Public Class TBattleground
            'Public Map As UInteger
            Public MinPlayersPerTeam As Byte
            Public MaxPlayersPerTeam As Byte
            Public MinLevel As Byte
            Public MaxLevel As Byte
            Public AllianceStartLoc As Integer
            Public AllianceStartO As Single
            Public HordeStartLoc As Integer
            Public HordeStartO As Single
        End Class

        Private ReadOnly ChatChannelsDBC As String = "dbc" & Path.DirectorySeparatorChar & "ChatChannels.dbc"
        Public ChatChannelsInfo As New Dictionary(Of Integer, ChatChannelInfo)
        Public Sub InitializeChatChannels()
            Try

                Dim data As DBC.BufferedDbc = New DBC.BufferedDbc(ChatChannelsDBC)
                For i As Integer = 0 To New DBC.BufferedDbc(ChatChannelsDBC).Rows - 1
                    Dim ChatChannels As New ChatChannelInfo With {
                        .Index = data.Item(i, 0, DBCValueType.DBC_INTEGER),
                        .Flags = data.Item(i, 1, DBCValueType.DBC_INTEGER),
                        .Name = data.Item(i, 3, DBCValueType.DBC_STRING)
                    }

                    ChatChannelsInfo.Add(ChatChannels.Index, ChatChannels)
                Next i

                Log.WriteLine(LogType.INFORMATION, "DBC: {0} ChatChannels Initialized.", New DBC.BufferedDbc(ChatChannelsDBC).Rows - 1)
                Call New DBC.BufferedDbc(ChatChannelsDBC).Dispose()
            Catch e As DirectoryNotFoundException
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("DBC File : ChatChannels.dbc missing.")
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub

        Public Class ChatChannelInfo
            Public Index As Integer
            Public Flags As Integer
            Public Name As String
        End Class

        Private ReadOnly ChrRacesDBC As String = "dbc" & Path.DirectorySeparatorChar & "ChrRaces.dbc"
        Public Sub InitializeCharRaces()
            Try
                'Loading from DBC
                Dim raceID As Integer
                Dim factionID As Integer
                Dim modelM As Integer
                Dim modelF As Integer
                Dim teamID As Integer '1 = Horde / 7 = Alliance
                Dim cinematicID As Integer

                Dim data As DBC.BufferedDbc = New DBC.BufferedDbc(ChrRacesDBC)
                For i As Integer = 0 To New DBC.BufferedDbc(ChrRacesDBC).Rows - 1
                    raceID = data.Item(i, 0)
                    factionID = data.Item(i, 2)
                    modelM = data.Item(i, 4)
                    modelF = data.Item(i, 5)
                    teamID = data.Item(i, 8)
                    cinematicID = data.Item(i, 16)

                    CharRaces(CByte(raceID)) = New TCharRace(factionID, modelM, modelF, teamID, cinematicID)
                Next i

                Log.WriteLine(LogType.INFORMATION, "DBC: {0} ChrRace Loaded.", New DBC.BufferedDbc(ChrRacesDBC).Rows - 1)
                Call New DBC.BufferedDbc(ChrRacesDBC).Dispose()
            Catch e As DirectoryNotFoundException
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("DBC File : ChrRaces.dbc missing.")
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub

        Private ReadOnly ChrClassesDBC As String = "dbc" & Path.DirectorySeparatorChar & "ChrClasses.dbc"
        Public Sub InitializeCharClasses()
            Try
                'Loading from DBC
                Dim classID As Integer
                Dim cinematicID As Integer

                For i As Integer = 0 To New DBC.BufferedDbc(ChrClassesDBC).Rows - 1
                    Dim data As DBC.BufferedDbc = New DBC.BufferedDbc(ChrClassesDBC)
                    classID = data.Item(i, 0)
                    cinematicID = data.Item(i, 5) ' or 14 or 15?

                    CharClasses(CByte(classID)) = New TCharClass(cinematicID)
                Next i

                Log.WriteLine(LogType.INFORMATION, "DBC: {0} ChrClasses Loaded.", New DBC.BufferedDbc(ChrClassesDBC).Rows - 1)
                Call New DBC.BufferedDbc(ChrClassesDBC).Dispose()
            Catch e As DirectoryNotFoundException
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("DBC File : ChrClasses.dbc missing.")
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub

        Public CharRaces As New Dictionary(Of Integer, TCharRace)
        Public Class TCharRace
            Public FactionID As Short
            Public ModelMale As Integer
            Public ModelFemale As Integer
            Public TeamID As Byte
            Public CinematicID As Integer

            Public Sub New(faction As Short, modelM As Integer, modelF As Integer, team As Byte, cinematic As Integer)
                FactionID = faction
                ModelMale = modelM
                ModelFemale = modelF
                TeamID = team
                CinematicID = cinematic
            End Sub
        End Class

        Public CharClasses As New Dictionary(Of Integer, TCharClass)
        Public Class TCharClass
            Public CinematicID As Integer

            Public Sub New(cinematic As Integer)
                CinematicID = cinematic
            End Sub
        End Class

    End Module
End Namespace
