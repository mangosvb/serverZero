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

Imports Mangos.Common.Enums.Global
Imports Mangos.World.Player
Imports Mangos.World.Server

Public Enum ViolationType
    AC_VIOLATION_NONE = 0
    AC_VIOLATION_SPEEDHACK_TIME = 1
    AC_VIOLATION_SPEEDHACK_MEM = 3
    AC_VIOLATION_MOVEMENT_Z = 2
End Enum

Public Class SpeedHackViolation
    ' Class used to store information about a player, and their detected anticheat violations.
    Public Character As String ' Character name
    Public Violations As Int32 ' Violation amount
    Public LastClientTime As Int32 ' Last time reported by client during movement
    Public LastServerTime As Int32 ' Last time reported by server during movement
    Public TotalOffset As Int32 ' Difference between the offsets of the server and client times. Under normal conditions, should be <= 60-100
    Public LastMessage As String ' Message used in the console to show information about a triggered violation
    Public LastViolation As ViolationType = ViolationType.AC_VIOLATION_NONE ' Default the last violation type to none
    ' Constructor
    Public Sub New(ByVal Name As String, ByVal cTime As Int32, ByVal sTime As Int32)
        Character = Name
        Violations = 0
        LastClientTime = cTime
        LastServerTime = sTime
        TotalOffset = 0
        LastMessage = ""
    End Sub
    ' Calculate distance between new position and old position. Any value above the actual run speed value (literally) is too high, as typical movement ticks will result in value close to RunSpeed * 0.54
    Public Function PlayerMoveDistance(ByVal posX As Single, ByVal positionX As Single, ByVal posY As Single, ByVal positionY As Single, ByVal posZ As Single, ByVal positionZ As Single) As Single
        Return Math.Sqrt((Math.Abs(posX - positionX) ^ 2) + (Math.Abs(posY - positionY) ^ 2) + (Math.Abs(posZ - positionZ) ^ 2))
    End Function
    ' Calculate the total offset difference between client and server. If the client time offset is higher than the server by too much, it could indicate the client is ticking faster than it should be.
    Public Function GetTotalOffset(ByVal cTime As Int32, ByVal sTime As Int32) As Int32
        Return Math.Abs((cTime - LastClientTime) - (sTime - LastServerTime))
    End Function
    ' Run checks on the position, speed, and time of the client.
    Public Sub TriggerViolation(ByVal posX As Single, ByVal positionX As Single, ByVal posY As Single, ByVal positionY As Single, ByVal posZ As Single, ByVal positionZ As Single, ByVal sTime As Int32, ByVal cTime As Int32, ByVal RunSpeed As Single)
        ' Only check for violations if the player's position has actually changed. Ignores packets only updating orientation
        If posX <> positionX AndAlso posY <> positionY AndAlso posZ <> positionZ Then
            TotalOffset = GetTotalOffset(cTime, sTime)
            Dim Distance As Single = PlayerMoveDistance(posX, positionX, posY, positionY, posZ, positionZ)
            ' Check for time hack. >= 235 is probably a time hack, while ABOVE 35000 is likely a client hitch.
            If TotalOffset >= 235 AndAlso TotalOffset < 35000 Then
                LastMessage = String.Format("Time Hack | Offset: {0}", TotalOffset)
                LastViolation = ViolationType.AC_VIOLATION_SPEEDHACK_TIME
                ' Check for memory hack
            ElseIf Distance >= (RunSpeed) Then
                Dim Estimate As Single = Distance * 1.54
                LastMessage = String.Format("Memory Hack | Distance: {0} Estimated Speed: {1}", Distance, Estimate)
                LastViolation = ViolationType.AC_VIOLATION_SPEEDHACK_MEM
                ' Check for Z height hack (fly/jump)
            ElseIf Math.Abs(posZ - positionZ) >= 10 Then
                LastMessage = String.Format("Jump/Fly Hack | Z: {0}", posZ)
                LastViolation = ViolationType.AC_VIOLATION_MOVEMENT_Z
            Else
                LastMessage = ""
                LastViolation = ViolationType.AC_VIOLATION_NONE
            End If
        End If
        LastClientTime = cTime
        LastServerTime = sTime
    End Sub
End Class

Public Module WS_Anticheat
    ' List used to track user violations
    Dim SpeedHacks As List(Of SpeedHackViolation) = New List(Of SpeedHackViolation)

    Public Sub MovementEvent(ByRef client As WS_Network.ClientClass, ByVal RunSpeed As Single, ByVal posX As Single, ByVal positionX As Single, ByVal posY As Single, ByVal positionY As Single, ByVal posZ As Single, ByVal positionZ As Single, ByVal sTime As Int32, ByVal cTime As Int32)
        Dim sData As SpeedHackViolation
        Dim pChar As WS_PlayerData.CharacterObject = client.Character
        If Not SpeedHacks.Exists(Function(obj) obj.Character.Equals(pChar.Name)) Then
            sData = New SpeedHackViolation(client.Character.Name, cTime, sTime)
            SpeedHacks.Add(sData)
        Else
            sData = SpeedHacks.Find(Function(obj) obj.Character.Equals(pChar.Name))
        End If
        sData.TriggerViolation(posX, positionX, posY, positionY, posZ, positionZ, sTime, cTime, RunSpeed)
        If sData.LastViolation <> ViolationType.AC_VIOLATION_NONE Then
            sData.Violations += sData.LastViolation
            _WorldServer.Log.WriteLine(LogType.INFORMATION, "[AntiCheat] Player {0} triggered a speedhack violation. ({1}) {2}", client.Character.Name, sData.Violations, sData.LastMessage)
            If sData.Violations >= 10 Then
                _WorldServer.Log.WriteLine(LogType.USER, "[AntiCheat] Player {0} exceeded violation value. Taking action.", client.Character.Name)
                'Take Action
                'client.Character.CastOnSelf(31366) ' Apply Root Anybody Forever to the cheater
                'client.Character.SendChatMessage(client.Character, "You have been punished for cheating.", ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_GLOBAL, "Global", True)
                client.Character.Logout(Nothing)
                SpeedHacks.Remove(sData)
            End If
        Else
            If sData.Violations > 0 Then
                Select Case (sData.LastViolation)
                    Case ViolationType.AC_VIOLATION_MOVEMENT_Z, ViolationType.AC_VIOLATION_SPEEDHACK_TIME, ViolationType.AC_VIOLATION_NONE
                        sData.Violations -= 1
                    Case ViolationType.AC_VIOLATION_SPEEDHACK_MEM
                        sData.Violations -= 0
                End Select
            End If
            If sData.Violations < 0 Then
                sData.Violations = 0
            End If
        End If
    End Sub
End Module
