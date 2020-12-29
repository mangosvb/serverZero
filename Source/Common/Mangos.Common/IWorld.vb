'
' Copyright (C) 2013-2021 getMaNGOS <https://getmangos.eu>
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

Imports System.ComponentModel

Public Interface IWorld

    <Description("Initialize client object.")>
    Sub ClientConnect(ByVal id As UInteger, ByVal client As ClientInfo)
    <Description("Destroy client object.")>
    Sub ClientDisconnect(ByVal id As UInteger)
    <Description("Assing particular client to this world server (Use client ID).")>
    Sub ClientLogin(ByVal id As UInteger, ByVal guid As ULong)
    <Description("Remove particular client from this world server (Use client ID).")>
    Sub ClientLogout(ByVal id As UInteger)
    <Description("Transfer packet from Realm to World using client's ID.")>
    Sub ClientPacket(ByVal id As UInteger, ByVal data() As Byte)

    <Description("Create CharacterObject.")>
    Function ClientCreateCharacter(ByVal account As String, ByVal name As String, ByVal race As Byte, ByVal classe As Byte, ByVal gender As Byte, ByVal skin As Byte,
                                   ByVal face As Byte, ByVal hairStyle As Byte, ByVal hairColor As Byte, ByVal facialHair As Byte, ByVal outfitId As Byte) As Integer

    <Description("Respond to world server if still alive.")>
    Function Ping(ByVal timestamp As Integer, ByVal latency As Integer) As Integer

    <Description("Tell the cluster about your CPU & Memory Usage")>
    Function GetServerInfo() As ServerInfo

    <Description("Make world create specific map.")>
    Sub InstanceCreate(ByVal Map As UInteger)
    <Description("Make world destroy specific map.")>
    Sub InstanceDestroy(ByVal Map As UInteger)
    <Description("Check world configuration.")>
    Function InstanceCanCreate(ByVal Type As Integer) As Boolean

    <Description("Set client's group.")>
    Sub ClientSetGroup(ByVal ID As UInteger, ByVal GroupID As Long)
    <Description("Update group information.")>
    Sub GroupUpdate(ByVal GroupID As Long, ByVal GroupType As Byte, ByVal GroupLeader As ULong, ByVal Members() As ULong)
    <Description("Update group information about looting.")>
    Sub GroupUpdateLoot(ByVal GroupID As Long, ByVal Difficulty As Byte, ByVal Method As Byte, ByVal Threshold As Byte, ByVal Master As ULong)

    <Description("Request party member stats.")>
    Function GroupMemberStats(ByVal GUID As ULong, ByVal Flag As Integer) As Byte()

    <Description("Update guild information.")>
    Sub GuildUpdate(ByVal GUID As ULong, ByVal GuildID As UInteger, ByVal GuildRank As Byte)

    Sub BattlefieldCreate(ByVal BattlefieldID As Integer, ByVal BattlefieldMapType As Byte, ByVal Map As UInteger)
    Sub BattlefieldDelete(ByVal BattlefieldID As Integer)
    Sub BattlefieldJoin(ByVal BattlefieldID As Integer, ByVal GUID As ULong)
    Sub BattlefieldLeave(ByVal BattlefieldID As Integer, ByVal GUID As ULong)

End Interface