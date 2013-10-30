'
' Copyright (objCharacter) 2013 getMaNGOS <http://www.getMangos.co.uk>
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
Imports System.IO
Imports System.Runtime.InteropServices
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common

Public Module WS_DBCDatabase


#Region "Maps"


    Public Maps As New Dictionary(Of Integer, MapInfo)
    Public Sub InitializeMaps()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\Map.dbc")

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                Dim m As New MapInfo
                m.ID = tmpDBC.Item(i, 0, DBC.DBCValueType.DBC_INTEGER)
                m.Type = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_INTEGER)
                m.Name = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_STRING)
                m.ParentMap = tmpDBC.Item(i, 3, DBC.DBCValueType.DBC_INTEGER)
                m.ResetTime = tmpDBC.Item(i, 38, DBC.DBCValueType.DBC_INTEGER)

                Maps.Add(m.ID, m)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Maps initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : Maps missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
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


#End Region
#Region "WorldMapArea"


    Public WorldSafeLocs As New Dictionary(Of Integer, TWorldSafeLoc)
    Public Sub InitializeWorldSafeLocs()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\WorldSafeLocs.dbc")

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                Dim wsl As New TWorldSafeLoc

                wsl.ID = tmpDBC.Item(i, 0, DBC.DBCValueType.DBC_INTEGER)
                wsl.map = tmpDBC.Item(i, 1)
                wsl.x = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_FLOAT)
                wsl.y = tmpDBC.Item(i, 3, DBC.DBCValueType.DBC_FLOAT)
                wsl.z = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_FLOAT)

                WorldSafeLocs.Add(wsl.ID, wsl)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} WorldSafeLocs initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : WorldSafeLocs missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Class TWorldSafeLoc
        Public ID As Integer

        Public map As UInteger
        Public x As Single
        Public y As Single
        Public z As Single
    End Class


#End Region
#Region "Battlegrounds"


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

        Log.WriteLine(LogType.INFORMATION, "World: {0} Battlegrounds Loaded.", MySQLQuery.Rows.Count)
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


#End Region
#Region "Chat Channels"

    <Flags()> _
    Public Enum ChatChannelsFlags
        FLAG_NONE = &H0
        FLAG_INITIAL = &H1              ' General, Trade, LocalDefense, LFG
        FLAG_ZONE_DEP = &H2             ' General, Trade, LocalDefense, GuildRecruitment
        FLAG_GLOBAL = &H4               ' WorldDefense
        FLAG_TRADE = &H8                ' Trade
        FLAG_CITY_ONLY = &H10           ' Trade, GuildRecruitment
        FLAG_CITY_ONLY2 = &H20          ' Trade, GuildRecruitment
        FLAG_DEFENSE = &H10000          ' LocalDefense, WorldDefense
        FLAG_GUILD_REQ = &H20000        ' GuildRecruitment
        FLAG_LFG = &H40000              ' LookingForGroup
    End Enum
    Public ChatChannelsInfo As New Dictionary(Of Integer, ChatChannelInfo)
    Public Sub InitializeChatChannels()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ChatChannels.dbc")

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                Dim objCharacter As New ChatChannelInfo
                objCharacter.Index = tmpDBC.Item(i, 0, DBC.DBCValueType.DBC_INTEGER)
                objCharacter.Flags = tmpDBC.Item(i, 1, DBC.DBCValueType.DBC_INTEGER)
                objCharacter.Name = tmpDBC.Item(i, 3, DBC.DBCValueType.DBC_STRING)

                ChatChannelsInfo.Add(objCharacter.Index, objCharacter)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} ChatChannels initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : ChatChannels missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Class ChatChannelInfo
        Public Index As Integer
        Public Flags As Integer
        Public Name As String
    End Class


#End Region
#Region "Character"
    Public Sub InitializeCharRaces()
        Try
            Dim i As Integer

            'Loading from DBC
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ChrRaces.dbc")

            Dim raceID As Integer
            Dim factionID As Integer
            Dim modelM As Integer
            Dim modelF As Integer
            Dim teamID As Integer '1 = Horde / 7 = Alliance
            Dim cinematicID As Integer

            For i = 0 To tmpDBC.Rows - 1
                raceID = tmpDBC.Item(i, 0)
                factionID = tmpDBC.Item(i, 2)
                modelM = tmpDBC.Item(i, 4)
                modelF = tmpDBC.Item(i, 5)
                teamID = tmpDBC.Item(i, 8)
                cinematicID = tmpDBC.Item(i, 16)

                CharRaces(CByte(raceID)) = New TCharRace(CShort(factionID), modelM, modelF, CByte(teamID), cinematicID)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} CharRaces initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : CharRaces missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeCharClasses()
        Try
            Dim i As Integer

            'Loading from DBC
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ChrClasses.dbc")

            Dim classID As Integer
            Dim cinematicID As Integer

            For i = 0 To tmpDBC.Rows - 1
                classID = tmpDBC.Item(i, 0)
                cinematicID = tmpDBC.Item(i, 5) ' or 14 or 15?

                CharClasses(CByte(classID)) = New TCharClass(cinematicID)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} CharRaces initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : CharRaces missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public CharRaces As New Dictionary(Of Integer, TCharRace)
    Public Class TCharRace
        Public FactionID As Short
        Public ModelMale As Integer
        Public ModelFemale As Integer
        Public TeamID As Byte
        Public CinematicID As Integer

        Public Sub New(ByVal Faction As Short, ByVal ModelM As Integer, ByVal ModelF As Integer, ByVal Team As Byte, ByVal Cinematic As Integer)
            FactionID = Faction
            ModelMale = ModelM
            ModelFemale = ModelF
            TeamID = Team
            CinematicID = Cinematic
        End Sub
    End Class

    Public CharClasses As New Dictionary(Of Integer, TCharClass)
    Public Class TCharClass
        Public CinematicID As Integer

        Public Sub New(ByVal Cinematic As Integer)
            CinematicID = Cinematic
        End Sub
    End Class
#End Region


End Module
