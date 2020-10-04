'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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

Imports System.Text.RegularExpressions

Imports mangosVB.Common
Imports mangosVB.Common.Globals

Imports mangosVB.Shared

Imports WorldCluster.Handlers
Imports WorldCluster.DataStores
Imports WorldCluster.Server
Imports System.Data

Namespace Globals
    Public Module Functions
        Public Function ToInteger(value As Boolean) As Integer
            If value Then
                Return 1
            Else
                Return 0
            End If
        End Function

        Public Function ToHex(bBytes() As Byte, Optional ByVal start As Integer = 0) As String
            If bBytes.Length = 0 Then Return "''"
            Dim tmpStr As String = "0x"
            For i As Integer = start To bBytes.Length - 1
                If bBytes(i) < 16 Then
                    tmpStr &= "0" & Hex(bBytes(i))
                Else
                    tmpStr &= Hex(bBytes(i))
                End If
            Next
            Return tmpStr
        End Function

        Public Function ByteToCharArray(bBytes() As Byte) As Char()
            If bBytes.Length = 0 Then Return New Char() {}
            Dim bChar(bBytes.Length - 1) As Char
            For i As Integer = 0 To bBytes.Length - 1
                bChar(i) = Chr(bBytes(i))
            Next
            Return bChar
        End Function

        Public Function ByteToIntArray(bBytes() As Byte) As Integer()
            If bBytes.Length = 0 Then Return New Integer() {}
            Dim bInt((bBytes.Length - 1) \ 4) As Integer
            For i As Integer = 0 To bBytes.Length - 1 Step 4
                bInt(i \ 4) = BitConverter.ToInt32(bBytes, i)
            Next
            Return bInt
        End Function

        Public Function IntToByteArray(bInt() As Integer) As Byte()
            If bInt.Length = 0 Then Return New Byte() {}
            Dim bBytes((bInt.Length * 4) - 1) As Byte
            For i As Integer = 0 To bInt.Length - 1
                Dim tmpBytes() As Byte = BitConverter.GetBytes(bInt(i))
                Array.Copy(tmpBytes, 0, bBytes, (i * 4), 4)
            Next
            Return bBytes
        End Function

        Public Function Concat(a As Byte(), b As Byte()) As Byte()
            Dim buffer1 As Byte() = New Byte((a.Length + b.Length) - 1) {}
            Dim num1 As Integer
            For num1 = 0 To a.Length - 1
                buffer1(num1) = a(num1)
            Next num1
            Dim num2 As Integer
            For num2 = 0 To b.Length - 1
                buffer1((num2 + a.Length)) = b(num2)
            Next num2
            Return buffer1
        End Function

        Public Function HaveFlag(value As UInteger, flagPos As Byte) As Boolean
            value = value >> CUInt(flagPos)
            value = value Mod 2

            If value = 1 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function HaveFlags(value As Integer, flags As Integer) As Boolean
            Return ((value And flags) = flags)
        End Function

        Public Sub SetFlag(ByRef value As UInteger, flagPos As Byte, flagValue As Boolean)
            If flagValue Then
                value = (value Or (&H1UI << CUInt(flagPos)))
            Else
                value = (value And ((&H0UI << CUInt(flagPos)) And &HFFFFFFFFUI))
            End If
        End Sub

        Public Function GetNextDay(iDay As DayOfWeek, Optional ByVal hour As Integer = 0) As Date
            Dim iDiff As Integer = iDay - Today.DayOfWeek
            If iDiff <= 0 Then iDiff += 7
            Dim nextFriday As Date = Today.AddDays(iDiff)
            nextFriday = nextFriday.AddHours(hour)
            Return nextFriday
        End Function

        Public Function GetNextDate(days As Integer, Optional ByVal hours As Integer = 0) As Date
            Dim nextDate As Date = Today.AddDays(days)
            nextDate = nextDate.AddHours(hours)
            Return nextDate
        End Function

        Public Function GetTimestamp(fromDateTime As Date) As UInteger
            Dim startDate As Date = #1/1/1970#
            Dim timeSpan As TimeSpan

            timeSpan = fromDateTime.Subtract(startDate)
            Return Math.Abs(timeSpan.TotalSeconds())
        End Function

        Public Function GetDateFromTimestamp(unixTimestamp As UInteger) As Date
            Dim timeSpan As TimeSpan
            Dim startDate As Date = #1/1/1970#

            If unixTimestamp = 0 Then Return startDate

            timeSpan = New TimeSpan(0, 0, unixTimestamp)
            Return startDate.Add(timeSpan)
        End Function

        Public Function GetTimeLeftString(seconds As UInteger) As String
            If seconds < 60 Then
                Return seconds & "s"
            ElseIf seconds < 3600 Then
                Return (seconds \ 60) & "m " & (seconds Mod 60) & "s"
            ElseIf seconds < 86400 Then
                Return (seconds \ 3600) & "h " & ((seconds \ 60) Mod 60) & "m " & (seconds Mod 60) & "s"
            Else
                Return (seconds \ 86400) & "d " & ((seconds \ 3600) Mod 24) & "h " & ((seconds \ 60) Mod 60) & "m " & (seconds Mod 60) & "s"
            End If
        End Function

        Public Function EscapeString(s As String) As String
            Return s.Replace("""", "").Replace("'", "")
        End Function

        Public Function CapitalizeName(ByRef name As String) As String
            Return If(name.Length > 1, UCase(Left(name, 1)) & LCase(Right(name, name.Length - 1)), UCase(name))
        End Function

        Private Regex_AZ As Regex = New Regex("^[a-zA-Z]+$")
        Public Function ValidateName(strName As String) As Boolean
            If strName.Length < 2 OrElse strName.Length > 16 Then Return False
            Return Regex_AZ.IsMatch(strName)
        End Function

        Private Regex_Guild As Regex = New Regex("^[a-z A-Z]+$")
        Public Function ValidateGuildName(strName As String) As Boolean
            If strName.Length < 2 OrElse strName.Length > 16 Then Return False
            Return Regex_Guild.IsMatch(strName)
        End Function

        Public Function FixName(strName As String) As String
            Return strName.Replace("""", "'").Replace("<", "").Replace(">", "").Replace("*", "").Replace("/", "").Replace("\", "").Replace(":", "").Replace("|", "").Replace("?", "")
        End Function

        Public Sub RAND_bytes(ByRef bBytes() As Byte, length As Integer)
            If length = 0 Then Exit Sub
            bBytes = (New Byte(length - 1) {})

            For i As Integer = 0 To length - 1
                If i = bBytes.Length Then Exit For
                bBytes(i) = New Random().Next(0, 256)
            Next
        End Sub

        Public Function MathLerp(value1 As Single, value2 As Single, amount As Single) As Single
            Return value1 + (value2 - value1) * amount
        End Function

        Public Sub Ban_Account(name As String, reason As String)
            Dim account As New DataTable
            Dim bannedAccount As New DataTable
            AccountDatabase.Query(String.Format("SELECT id, username FROM account WHERE username = {0};", name), account)
            If (account.Rows.Count > 0) Then
                Dim accId As Integer = account.Rows(0).Item("id")
                AccountDatabase.Query(String.Format("SELECT id, active FROM account_banned WHERE id = {0};", accId), bannedAccount)

                If (bannedAccount.Rows.Count > 0) Then
                    AccountDatabase.Update("UPDATE account_banned SET active = 1 WHERE id = '" & accId & "';")
                Else
                    Dim tempBanDate As String = FormatDateTime(Date.Now.ToFileTimeUtc.ToString(), DateFormat.LongDate) & " " & FormatDateTime(Date.Now.ToFileTimeUtc.ToString(), DateFormat.LongTime)
                    AccountDatabase.Update(String.Format("INSERT INTO `account_banned` VALUES ('{0}', UNIX_TIMESTAMP('{1}'), UNIX_TIMESTAMP('{2}'), '{3}', '{4}', active = 1);", accId, tempBanDate, "0000-00-00 00:00:00", name, reason))
                End If
                Log.WriteLine(LogType.INFORMATION, "Account [{0}] banned by server. Reason: [{1}].", name, reason)
            Else
                Log.WriteLine(LogType.INFORMATION, "Account [{0}] NOT Found in Database.", name)
            End If
        End Sub

        Public Function GetClassName(ByRef classe As Integer) As String
            Select Case classe
                Case Classes.CLASS_DRUID
                    GetClassName = "Druid"

                Case Classes.CLASS_HUNTER
                    GetClassName = "Hunter"

                Case Classes.CLASS_MAGE
                    GetClassName = "Mage"

                Case Classes.CLASS_PALADIN
                    GetClassName = "Paladin"

                Case Classes.CLASS_PRIEST
                    GetClassName = "Priest"

                Case Classes.CLASS_ROGUE
                    GetClassName = "Rogue"

                Case Classes.CLASS_SHAMAN
                    GetClassName = "Shaman"

                Case Classes.CLASS_WARLOCK
                    GetClassName = "Warlock"

                Case Classes.CLASS_WARRIOR
                    GetClassName = "Warrior"
                Case Else
                    GetClassName = "Unknown Class"
            End Select
        End Function

        Public Function GetRaceName(ByRef race As Integer) As String
            Select Case race
                Case Races.RACE_DWARF
                    GetRaceName = "Dwarf"

                Case Races.RACE_GNOME
                    GetRaceName = "Gnome"

                Case Races.RACE_HUMAN
                    GetRaceName = "Human"

                Case Races.RACE_NIGHT_ELF
                    GetRaceName = "Night Elf"

                Case Races.RACE_ORC
                    GetRaceName = "Orc"

                Case Races.RACE_TAUREN
                    GetRaceName = "Tauren"

                Case Races.RACE_TROLL
                    GetRaceName = "Troll"

                Case Races.RACE_UNDEAD
                    GetRaceName = "Undead"
                Case Else
                    GetRaceName = "Unknown Race"
            End Select
        End Function

        Public Function GetRaceModel(race As Races, gender As Integer) As Integer
            Select Case race
                Case Races.RACE_HUMAN
                    Return 49 + gender

                Case Races.RACE_ORC
                    Return 51 + gender

                Case Races.RACE_DWARF
                    Return 53 + gender

                Case Races.RACE_NIGHT_ELF
                    Return 55 + gender

                Case Races.RACE_UNDEAD
                    Return 57 + gender

                Case Races.RACE_TAUREN
                    Return 59 + gender

                Case Races.RACE_GNOME
                    Return 1563 + gender

                Case Races.RACE_TROLL
                    Return 1478 + gender
                Case Else
                    Return 16358                    'PinkPig? Lol
            End Select
        End Function

        Public Function GetCharacterSide(race As Byte) As Boolean
            Select Case race
                Case Races.RACE_DWARF, Races.RACE_GNOME, Races.RACE_HUMAN, Races.RACE_NIGHT_ELF
                    Return False
                Case Else
                    Return True
            End Select
        End Function

        Public Function IsContinentMap(map As Integer) As Boolean
            Select Case map
                Case 0, 1
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Public Function SetColor(message As String, red As Byte, green As Byte, blue As Byte) As String
            SetColor = "|cFF"
            SetColor = If(red < 16, SetColor & "0" & Hex(red), SetColor & Hex(red))
            SetColor = If(green < 16, SetColor & "0" & Hex(green), SetColor & Hex(green))
            SetColor = If(blue < 16, SetColor & "0" & Hex(blue), SetColor & Hex(blue))
            SetColor = SetColor & message & "|r"

            'SetColor = String.Format("|cff{0:x}{1:x}{2:x}{3}|r", Red, Green, Blue, Message)
        End Function

        Public Function RollChance(chance As Single) As Boolean
            Dim nChance As Integer = chance * 100
            If Rnd.Next(1, 10001) <= nChance Then Return True
            Return False
        End Function

        Public Sub SendMessageMOTD(ByRef client As ClientClass, message As String)
            Dim packet As PacketClass = BuildChatMessage(0, message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL)
            client.Send(packet)
        End Sub

        Public Sub SendMessageNotification(ByRef client As ClientClass, message As String)
            Dim packet As New PacketClass(OPCODES.SMSG_NOTIFICATION)
            Try
                packet.AddString(message)
                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub SendMessageSystem(objCharacter As ClientClass, message As String)
            Dim packet As PacketClass = BuildChatMessage(0, message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL, 0, "")
            Try
                objCharacter.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub Broadcast(message As String)
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If character.Value.Client IsNot Nothing Then SendMessageSystem(character.Value.Client, "System Message: " & SetColor(message, 255, 0, 0))
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
        End Sub

        Public Sub SendAccountMD5(ByRef client As ClientClass, ByRef character As CharacterObject)
            Dim FoundData As Boolean = False

            'TODO: How Does Mangos Zero Handle the Account Data For the Characters?
            'Dim AccData As New DataTable
            'AccountDatabase.Query(String.Format("SELECT id FROM account WHERE username = ""{0}"";", client.Account), AccData)
            'If AccData.Rows.Count > 0 Then
            '    Dim AccID As Integer = CType(AccData.Rows(0).Item("account_id"), Integer)

            '    AccData.Clear()
            '    AccountDatabase.Query(String.Format("SELECT * FROM account_data WHERE account_id = {0}", AccID), AccData)
            '    If AccData.Rows.Count > 0 Then
            '        FoundData = True
            '    Else
            '        AccountDatabase.Update(String.Format("INSERT INTO account_data VALUES({0}, '', '', '', '', '', '', '', '')", AccID))
            '    End If
            'End If

            Dim smsgAccountDataTimes As New PacketClass(OPCODES.SMSG_ACCOUNT_DATA_MD5)
            Try
                'Dim md5hash As MD5 = MD5.Create()
                For i As Integer = 0 To 7
                    If FoundData Then
                        'Dim tmpBytes() As Byte = AccData.Rows(0).Item("account_data" & i)
                        'If tmpBytes.Length = 0 Then
                        'SMSG_ACCOUNT_DATA_TIMES.AddInt64(0)
                        'SMSG_ACCOUNT_DATA_TIMES.AddInt64(0)
                        'Else
                        'SMSG_ACCOUNT_DATA_TIMES.AddByteArray(md5hash.ComputeHash(tmpBytes))
                        'End If
                    Else
                        smsgAccountDataTimes.AddInt64(0)
                        smsgAccountDataTimes.AddInt64(0)
                    End If
                Next
                'md5hash.Clear()
                'md5hash = Nothing

                client.Send(smsgAccountDataTimes)
            Finally
                smsgAccountDataTimes.Dispose()
            End Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_ACCOUNT_DATA_MD5", client.IP, client.Port)
        End Sub

        Public Sub SendTriggerCinematic(ByRef client As ClientClass, ByRef character As CharacterObject)
            Dim packet As New PacketClass(OPCODES.SMSG_TRIGGER_CINEMATIC)
            Try
                If CharRaces.ContainsKey(character.Race) Then
                    packet.AddInt32(CharRaces(character.Race).CinematicID)
                Else
                    Log.WriteLine(LogType.WARNING, "[{0}:{1}] SMSG_TRIGGER_CINEMATIC [Error: RACE={2} CLASS={3}]", client.IP, client.Port, character.Race, character.Classe)
                    Exit Sub
                End If

                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_TRIGGER_CINEMATIC", client.IP, client.Port)
        End Sub

        Public Sub SendTimeSyncReq(ByRef client As ClientClass)
            'Dim packet As New PacketClass(OPCODES.SMSG_TIME_SYNC_REQ)
            'packet.AddInt32(0)
            'Client.Send(packet)
        End Sub

        Public Sub SendGameTime(ByRef client As ClientClass, ByRef character As CharacterObject)
            Dim smsgLoginSettimespeed As New PacketClass(OPCODES.SMSG_LOGIN_SETTIMESPEED)
            Try
                Dim time As Date = Date.Now
                Dim year As Integer = time.Year - 2000
                Dim month As Integer = time.Month - 1
                Dim day As Integer = time.Day - 1
                Dim dayOfWeek As Integer = time.DayOfWeek
                Dim hour As Integer = time.Hour
                Dim minute As Integer = time.Minute

                'SMSG_LOGIN_SETTIMESPEED.AddInt32(CType((((((Minute + (Hour << 6)) + (DayOfWeek << 11)) + (Day << 14)) + (Year << 18)) + (Month << 20)), Integer))
                smsgLoginSettimespeed.AddInt32(((((minute + (hour << 6)) + (dayOfWeek << 11)) + (day << 14)) + (month << 20)) + (year << 24))
                smsgLoginSettimespeed.AddSingle(0.01666667F)

                client.Send(smsgLoginSettimespeed)
            Finally
                smsgLoginSettimespeed.Dispose()
            End Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_LOGIN_SETTIMESPEED", client.IP, client.Port)
        End Sub

        Public Sub SendProficiency(ByRef client As ClientClass, proficiencyType As Byte, proficiencyFlags As Integer)
            Dim packet As New PacketClass(OPCODES.SMSG_SET_PROFICIENCY)
            Try
                packet.AddInt8(proficiencyType)
                packet.AddInt32(proficiencyFlags)

                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_SET_PROFICIENCY", client.IP, client.Port)
        End Sub

        Public Sub SendCorpseReclaimDelay(ByRef client As ClientClass, ByRef character As CharacterObject, Optional ByVal seconds As Integer = 30)
            Dim packet As New PacketClass(OPCODES.SMSG_CORPSE_RECLAIM_DELAY)
            Try
                packet.AddInt32(seconds * 1000)
                client.Send(packet)
            Finally
                packet.Dispose()
            End Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_CORPSE_RECLAIM_DELAY [{2}s]", client.IP, client.Port, seconds)
        End Sub

        Public Function BuildChatMessage(senderGuid As ULong, message As String, msgType As ChatMsg, msgLanguage As LANGUAGES, Optional ByVal flag As Byte = 0, Optional ByVal msgChannel As String = "Global") As PacketClass
            Dim packet As New PacketClass(OPCODES.SMSG_MESSAGECHAT)
            Try
                packet.AddInt8(msgType)
                packet.AddInt32(msgLanguage)

                Select Case msgType
                    Case ChatMsg.CHAT_MSG_CHANNEL
                        packet.AddString(msgChannel)
                        packet.AddUInt32(0)
                        packet.AddUInt64(senderGuid)
                    Case ChatMsg.CHAT_MSG_YELL, ChatMsg.CHAT_MSG_SAY, ChatMsg.CHAT_MSG_PARTY
                        packet.AddUInt64(senderGuid)
                        packet.AddUInt64(senderGuid)
                    Case ChatMsg.CHAT_MSG_SYSTEM, ChatMsg.CHAT_MSG_EMOTE, ChatMsg.CHAT_MSG_IGNORED, ChatMsg.CHAT_MSG_SKILL, ChatMsg.CHAT_MSG_GUILD, ChatMsg.CHAT_MSG_OFFICER, ChatMsg.CHAT_MSG_RAID, ChatMsg.CHAT_MSG_WHISPER_INFORM, ChatMsg.CHAT_MSG_GUILD, ChatMsg.CHAT_MSG_WHISPER, ChatMsg.CHAT_MSG_AFK, ChatMsg.CHAT_MSG_DND, ChatMsg.CHAT_MSG_RAID_LEADER, ChatMsg.CHAT_MSG_RAID_WARNING
                        packet.AddUInt64(senderGuid)
                    Case ChatMsg.CHAT_MSG_MONSTER_SAY, ChatMsg.CHAT_MSG_MONSTER_EMOTE, ChatMsg.CHAT_MSG_MONSTER_YELL
                        Log.WriteLine(LogType.WARNING, "Use Creature.SendChatMessage() for this message type - {0}!", msgType)
                    Case Else
                        Log.WriteLine(LogType.WARNING, "Unknown chat message type - {0}!", msgType)
                End Select

                packet.AddUInt32(Text.Encoding.UTF8.GetByteCount(message) + 1)
                packet.AddString(message)

                packet.AddInt8(flag)
            Catch ex As Exception
                Log.WriteLine(LogType.FAILED, "failed chat message type - {0}!", msgType)
            End Try
            Return packet
        End Function

        Public Enum PartyMemberStatsStatus As Byte
            STATUS_OFFLINE = &H0
            STATUS_ONLINE = &H1
            STATUS_PVP = &H2
            STATUS_CORPSE = &H8
            STATUS_DEAD = &H10
        End Enum

        Public Enum PartyMemberStatsBits As Byte
            FIELD_STATUS = 0
            FIELD_LIFE_CURRENT = 1
            FIElD_LIFE_MAX = 2
            FIELD_MANA_TYPE = 3
            FIELD_MANA_CURRENT = 4
            FIELD_MANA_MAX = 5
            FIELD_LEVEL = 6
            FIELD_ZONEID = 7
            FIELD_POSXPOSY = 8
        End Enum

        Public Enum PartyMemberStatsFlag As UInteger
            GROUP_UPDATE_FLAG_NONE = &H0 'nothing
            GROUP_UPDATE_FLAG_STATUS = &H1 'uint16, flags
            GROUP_UPDATE_FLAG_CUR_HP = &H2 'uint16
            GROUP_UPDATE_FLAG_MAX_HP = &H4 'uint16
            GROUP_UPDATE_FLAG_POWER_TYPE = &H8 'uint8
            GROUP_UPDATE_FLAG_CUR_POWER = &H10 'uint16
            GROUP_UPDATE_FLAG_MAX_POWER = &H20 'uint16
            GROUP_UPDATE_FLAG_LEVEL = &H40 'uint16
            GROUP_UPDATE_FLAG_ZONE = &H80 'uint16
            GROUP_UPDATE_FLAG_POSITION = &H100 'uint16, uint16
            GROUP_UPDATE_FLAG_AURAS = &H200 'uint64 mask, for each bit set uint16 spellid + uint8 unk
            GROUP_UPDATE_FLAG_PET_GUID = &H400 'uint64 pet guid
            GROUP_UPDATE_FLAG_PET_NAME = &H800 'pet name, NULL terminated string
            GROUP_UPDATE_FLAG_PET_MODEL_ID = &H1000 'uint16, model id
            GROUP_UPDATE_FLAG_PET_CUR_HP = &H2000 'uint16 pet cur health
            GROUP_UPDATE_FLAG_PET_MAX_HP = &H4000 'uint16 pet max health
            GROUP_UPDATE_FLAG_PET_POWER_TYPE = &H8000 'uint8 pet power type
            GROUP_UPDATE_FLAG_PET_CUR_POWER = &H10000 'uint16 pet cur power
            GROUP_UPDATE_FLAG_PET_MAX_POWER = &H20000 'uint16 pet max power
            GROUP_UPDATE_FLAG_PET_AURAS = &H40000 'uint64 mask, for each bit set uint16 spellid + uint8 unk, pet auras...

            GROUP_UPDATE_PET = GROUP_UPDATE_FLAG_PET_GUID Or GROUP_UPDATE_FLAG_PET_NAME Or
                               GROUP_UPDATE_FLAG_PET_MODEL_ID Or GROUP_UPDATE_FLAG_PET_CUR_HP Or GROUP_UPDATE_FLAG_PET_MAX_HP Or
                               GROUP_UPDATE_FLAG_PET_POWER_TYPE Or GROUP_UPDATE_FLAG_PET_CUR_POWER Or
                               GROUP_UPDATE_FLAG_PET_MAX_POWER Or GROUP_UPDATE_FLAG_PET_AURAS

            GROUP_UPDATE_FULL = GROUP_UPDATE_FLAG_STATUS Or GROUP_UPDATE_FLAG_CUR_HP Or GROUP_UPDATE_FLAG_MAX_HP Or
                                GROUP_UPDATE_FLAG_CUR_POWER Or GROUP_UPDATE_FLAG_LEVEL Or GROUP_UPDATE_FLAG_ZONE Or
                                GROUP_UPDATE_FLAG_MAX_POWER Or GROUP_UPDATE_FLAG_POSITION Or GROUP_UPDATE_FLAG_AURAS
            GROUP_UPDATE_FULL_PET = GROUP_UPDATE_FULL Or GROUP_UPDATE_PET
            GROUP_UPDATE_FULL_REQUEST_REPLY = &H7FFC0BFF
        End Enum

        Function BuildPartyMemberStatsOffline(guid As ULong) As PacketClass
            Dim packet As New PacketClass(OPCODES.SMSG_PARTY_MEMBER_STATS_FULL)
            packet.AddPackGUID(guid)
            packet.AddUInt32(PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS)
            packet.AddInt8(PartyMemberStatsStatus.STATUS_OFFLINE)
            Return packet
        End Function
    End Module
End Namespace
