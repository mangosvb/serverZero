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
Imports System.Data
Imports MangosVB.Common
Imports MangosVB.Common.Globals
Imports MangosVB.Shared
Imports MangosVB.Shared.Races
Imports MangosVB.Shared.Classes

Public Module Functions

#Region "System"

    Public Function ToInteger(ByVal Value As Boolean) As Integer
        If Value Then
            Return 1
        Else
            Return 0
        End If
    End Function

    Public Function ToHex(ByVal bBytes() As Byte, Optional ByVal start As Integer = 0) As String
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

    Public Function ByteToCharArray(ByVal bBytes() As Byte) As Char()
        If bBytes.Length = 0 Then Return New Char() {}
        Dim bChar(bBytes.Length - 1) As Char
        For i As Integer = 0 To bBytes.Length - 1
            bChar(i) = Chr(bBytes(i))
        Next
        Return bChar
    End Function

    Public Function ByteToIntArray(ByVal bBytes() As Byte) As Integer()
        If bBytes.Length = 0 Then Return New Integer() {}
        Dim bInt(((bBytes.Length - 1) \ 4)) As Integer
        For i As Integer = 0 To bBytes.Length - 1 Step 4
            bInt(i \ 4) = BitConverter.ToInt32(bBytes, i)
        Next
        Return bInt
    End Function

    Public Function IntToByteArray(ByVal bInt() As Integer) As Byte()
        If bInt.Length = 0 Then Return New Byte() {}
        Dim bBytes((bInt.Length * 4) - 1) As Byte
        For i As Integer = 0 To bInt.Length - 1
            Dim tmpBytes() As Byte = BitConverter.GetBytes(bInt(i))
            Array.Copy(tmpBytes, 0, bBytes, (i * 4), 4)
        Next
        Return bBytes
    End Function

    Public Function Concat(ByVal a As Byte(), ByVal b As Byte()) As Byte()
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

    Public Function HaveFlag(ByVal value As UInteger, ByVal flagPos As Byte) As Boolean
        value = value >> CUInt(flagPos)
        value = value Mod 2

        If value = 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function HaveFlags(ByVal value As Integer, ByVal flags As Integer) As Boolean
        Return ((value And flags) = flags)
    End Function

    Public Sub SetFlag(ByRef value As UInteger, ByVal flagPos As Byte, ByVal flagValue As Boolean)
        If flagValue Then
            value = (value Or (&H1UI << CUInt(flagPos)))
        Else
            value = (value And ((&H0UI << CUInt(flagPos)) And &HFFFFFFFFUI))
        End If
    End Sub

    Public Function GetNextDay(ByVal iDay As DayOfWeek, Optional ByVal Hour As Integer = 0) As Date
        Dim iDiff As Integer = iDay - Today.DayOfWeek
        If iDiff <= 0 Then iDiff += 7
        Dim nextFriday As Date = Today.AddDays(iDiff)
        nextFriday = nextFriday.AddHours(Hour)
        Return nextFriday
    End Function

    Public Function GetNextDate(ByVal Days As Integer, Optional ByVal Hours As Integer = 0) As Date
        Dim nextDate As Date = Today.AddDays(Days)
        nextDate = nextDate.AddHours(Hours)
        Return nextDate
    End Function

    Public Function GetTimestamp(ByVal fromDateTime As Date) As UInteger
        Dim startDate As Date = #1/1/1970#
        Dim timeSpan As TimeSpan

        timeSpan = fromDateTime.Subtract(startDate)
        Return Math.Abs(timeSpan.TotalSeconds())
    End Function

    Public Function GetDateFromTimestamp(ByVal unixTimestamp As UInteger) As Date
        Dim timeSpan As TimeSpan
        Dim startDate As Date = #1/1/1970#

        If unixTimestamp = 0 Then Return startDate

        timeSpan = New TimeSpan(0, 0, unixTimestamp)
        Return startDate.Add(timeSpan)
    End Function

    Public Function GetTimeLeftString(ByVal seconds As UInteger) As String
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

    Public Function EscapeString(ByVal s As String) As String
        Return s.Replace("""", "").Replace("'", "")
    End Function

    Public Function CapitalizeName(ByRef Name As String) As String
        If Name.Length > 1 Then 'Why would a name be one letter, or even 0? :P
            Return UppercaseFirstLetter(Left(Name, 1)) & LowercaseFirstLetter(Right(Name, Name.Length - 1))
        Else
            Return UppercaseFirstLetter(Name)
        End If
    End Function

    Private Regex_AZ As Regex = New Regex("^[a-zA-Z]+$")
    Public Function ValidateName(ByVal strName As String) As Boolean
        If strName.Length < 2 OrElse strName.Length > 16 Then Return False
        Return Regex_AZ.IsMatch(strName)
    End Function

    Private Regex_Guild As Regex = New Regex("^[a-z A-Z]+$")
    Public Function ValidateGuildName(ByVal strName As String) As Boolean
        If strName.Length < 2 OrElse strName.Length > 16 Then Return False
        Return Regex_Guild.IsMatch(strName)
    End Function

    Public Function FixName(ByVal strName As String) As String
        Return strName.Replace("""", "'").Replace("<", "").Replace(">", "").Replace("*", "").Replace("/", "").Replace("\", "").Replace(":", "").Replace("|", "").Replace("?", "")
    End Function

    Public Sub RAND_bytes(ByRef bBytes() As Byte, ByVal length As Integer)
        If length = 0 Then Exit Sub
        bBytes = New Byte(length - 1) {}

        Dim rnd As New Random()
        For i As Integer = 0 To length - 1
            If i = bBytes.Length Then Exit For
            bBytes(i) = rnd.Next(0, 256)
        Next
    End Sub

    Public Function MathLerp(ByVal value1 As Single, ByVal value2 As Single, ByVal amount As Single) As Single
        Return value1 + (value2 - value1) * amount
    End Function

#End Region

#Region "Database"

    Public Sub Ban_Account(ByVal Name As String, ByVal Reason As String)
        Dim account As New DataTable
        Dim bannedAccount As New DataTable
        AccountDatabase.Query(String.Format("SELECT id, username FROM account WHERE username = {0};", Name), account)
        If (account.Rows.Count > 0) Then
            Dim accID As Integer = account.Rows(0).Item("id")
            AccountDatabase.Query(String.Format("SELECT id, active FROM account_banned WHERE id = {0};", accID), bannedAccount)

            If (bannedAccount.Rows.Count > 0) Then
                AccountDatabase.Update("UPDATE account_banned SET active = 1 WHERE id = '" & accID & "';")
            Else
                Dim tempBanDate As String = FormatDateTime(Date.Now.ToFileTimeUtc.ToString(), DateFormat.LongDate) & " " & FormatDateTime(Date.Now.ToFileTimeUtc.ToString(), DateFormat.LongTime)
                AccountDatabase.Update(String.Format("INSERT INTO `account_banned` VALUES ('{0}', UNIX_TIMESTAMP('{1}'), UNIX_TIMESTAMP('{2}'), '{3}', '{4}', active = 1);", accID, tempBanDate, "0000-00-00 00:00:00", Name, Reason))
            End If
            Log.WriteLine(LogType.INFORMATION, "Account [{0}] banned by server. Reason: [{1}].", Name, Reason)
        Else
            Log.WriteLine(LogType.INFORMATION, "Account [{0}] NOT Found in Database.", Name)
        End If
    End Sub

#End Region

#Region "Game"

    Public Function GetClassName(ByRef Classe As Integer) As String
        Select Case Classe
            Case CLASS_DRUID
                GetClassName = "Druid"
            Case CLASS_HUNTER
                GetClassName = "Hunter"
            Case CLASS_MAGE
                GetClassName = "Mage"
            Case CLASS_PALADIN
                GetClassName = "Paladin"
            Case CLASS_PRIEST
                GetClassName = "Priest"
            Case CLASS_ROGUE
                GetClassName = "Rogue"
            Case CLASS_SHAMAN
                GetClassName = "Shaman"
            Case CLASS_WARLOCK
                GetClassName = "Warlock"
            Case CLASS_WARRIOR
                GetClassName = "Warrior"
            Case Else
                GetClassName = "Unknown Class"
        End Select
    End Function

    Public Function GetRaceName(ByRef Race As Integer) As String
        Select Case Race
            Case RACE_DWARF
                GetRaceName = "Dwarf"
            Case RACE_GNOME
                GetRaceName = "Gnome"
            Case RACE_HUMAN
                GetRaceName = "Human"
            Case RACE_NIGHT_ELF
                GetRaceName = "Night Elf"
            Case RACE_ORC
                GetRaceName = "Orc"
            Case RACE_TAUREN
                GetRaceName = "Tauren"
            Case RACE_TROLL
                GetRaceName = "Troll"
            Case RACE_UNDEAD
                GetRaceName = "Undead"
            Case Else
                GetRaceName = "Unknown Race"
        End Select
    End Function

    Public Function GetRaceModel(ByVal Race As Races, ByVal Gender As Integer) As Integer
        Select Case Race
            Case RACE_HUMAN
                Return 49 + Gender
            Case RACE_ORC
                Return 51 + Gender
            Case RACE_DWARF
                Return 53 + Gender
            Case RACE_NIGHT_ELF
                Return 55 + Gender
            Case RACE_UNDEAD
                Return 57 + Gender
            Case RACE_TAUREN
                Return 59 + Gender
            Case RACE_GNOME
                Return 1563 + Gender
            Case RACE_TROLL
                Return 1478 + Gender
            Case Else
                Return 16358                    'PinkPig? Lol
        End Select
    End Function

    Public Function GetCharacterSide(ByVal Race As Byte) As Boolean
        Select Case Race
            Case RACE_DWARF, RACE_GNOME, RACE_HUMAN, RACE_NIGHT_ELF
                Return False
            Case Else
                Return True
        End Select
    End Function

    Public Function IsContinentMap(ByVal Map As Integer) As Boolean
        Select Case Map
            Case 0, 1
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Function SetColor(ByVal Message As String, ByVal Red As Byte, ByVal Green As Byte, ByVal Blue As Byte) As String
        SetColor = "|cFF"
        If Red < 16 Then
            SetColor = SetColor & "0" & Hex(Red)
        Else
            SetColor = SetColor & Hex(Red)
        End If
        If Green < 16 Then
            SetColor = SetColor & "0" & Hex(Green)
        Else
            SetColor = SetColor & Hex(Green)
        End If
        If Blue < 16 Then
            SetColor = SetColor & "0" & Hex(Blue)
        Else
            SetColor = SetColor & Hex(Blue)
        End If
        SetColor = SetColor & Message & "|r"

        'SetColor = String.Format("|cff{0:x}{1:x}{2:x}{3}|r", Red, Green, Blue, Message)
    End Function

    Public Function RollChance(ByVal Chance As Single) As Boolean
        Dim nChance As Integer = Chance * 100
        If Rnd.Next(1, 10001) <= nChance Then Return True
        Return False
    End Function

#End Region

#Region "Packets"

    Public Sub SendMessageMOTD(ByRef client As ClientClass, ByVal Message As String)
        Dim packet As PacketClass = BuildChatMessage(0, Message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL)
        client.Send(packet)
    End Sub

    Public Sub SendMessageNotification(ByRef client As ClientClass, ByVal Message As String)
        Dim packet As New PacketClass(OPCODES.SMSG_NOTIFICATION)
        Try
            packet.AddString(Message)
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    Public Sub SendMessageSystem(ByVal objCharacter As ClientClass, ByVal Message As String)
        Dim packet As PacketClass = BuildChatMessage(0, Message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL, 0, "")
        Try
            objCharacter.Send(packet)
        Finally
            packet.Dispose()
        End Try
    End Sub

    Public Sub Broadcast(ByVal Message As String)
        CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
        For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
            If Character.Value.client IsNot Nothing Then SendMessageSystem(Character.Value.client, "System Message: " & SetColor(Message, 255, 0, 0))
        Next
        CHARACTERs_Lock.ReleaseReaderLock()
    End Sub

    Public Sub SendAccountMD5(ByRef client As ClientClass, ByRef Character As CharacterObject)
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

        Dim SMSG_ACCOUNT_DATA_TIMES As New PacketClass(OPCODES.SMSG_ACCOUNT_DATA_MD5)
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
                    SMSG_ACCOUNT_DATA_TIMES.AddInt64(0)
                    SMSG_ACCOUNT_DATA_TIMES.AddInt64(0)
                End If
            Next
            'md5hash.Clear()
            'md5hash = Nothing

            client.Send(SMSG_ACCOUNT_DATA_TIMES)
        Finally
            SMSG_ACCOUNT_DATA_TIMES.Dispose()
        End Try
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_ACCOUNT_DATA_MD5", client.IP, client.Port)
    End Sub

    Public Sub SendTriggerCinematic(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim packet As New PacketClass(OPCODES.SMSG_TRIGGER_CINEMATIC)
        Try
            If CharRaces.ContainsKey(Character.Race) Then
                packet.AddInt32(CharRaces(Character.Race).CinematicID)
            Else
                Log.WriteLine(LogType.WARNING, "[{0}:{1}] SMSG_TRIGGER_CINEMATIC [Error: RACE={2} CLASS={3}]", client.IP, client.Port, Character.Race, Character.Classe)
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

    Public Sub SendGameTime(ByRef client As ClientClass, ByRef Character As CharacterObject)
        Dim SMSG_LOGIN_SETTIMESPEED As New PacketClass(OPCODES.SMSG_LOGIN_SETTIMESPEED)
        Try
            Dim time As Date = Date.Now
            Dim Year As Integer = time.Year - 2000
            Dim Month As Integer = time.Month - 1
            Dim Day As Integer = time.Day - 1
            Dim DayOfWeek As Integer = time.DayOfWeek
            Dim Hour As Integer = time.Hour
            Dim Minute As Integer = time.Minute

            'SMSG_LOGIN_SETTIMESPEED.AddInt32(CType((((((Minute + (Hour << 6)) + (DayOfWeek << 11)) + (Day << 14)) + (Year << 18)) + (Month << 20)), Integer))
            SMSG_LOGIN_SETTIMESPEED.AddInt32(((((Minute + (Hour << 6)) + (DayOfWeek << 11)) + (Day << 14)) + (Month << 20)) + (Year << 24))
            SMSG_LOGIN_SETTIMESPEED.AddSingle(0.01666667F)

            client.Send(SMSG_LOGIN_SETTIMESPEED)
        Finally
            SMSG_LOGIN_SETTIMESPEED.Dispose()
        End Try
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_LOGIN_SETTIMESPEED", client.IP, client.Port)
    End Sub

    Public Sub SendProficiency(ByRef client As ClientClass, ByVal ProficiencyType As Byte, ByVal ProficiencyFlags As Integer)
        Dim packet As New PacketClass(OPCODES.SMSG_SET_PROFICIENCY)
        Try
            packet.AddInt8(ProficiencyType)
            packet.AddInt32(ProficiencyFlags)

            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_SET_PROFICIENCY", client.IP, client.Port)
    End Sub

    Public Sub SendCorpseReclaimDelay(ByRef client As ClientClass, ByRef Character As CharacterObject, Optional ByVal Seconds As Integer = 30)
        Dim packet As New PacketClass(OPCODES.SMSG_CORPSE_RECLAIM_DELAY)
        Try
            packet.AddInt32(Seconds * 1000)
            client.Send(packet)
        Finally
            packet.Dispose()
        End Try
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_CORPSE_RECLAIM_DELAY [{2}s]", client.IP, client.Port, Seconds)
    End Sub

    Public Function BuildChatMessage(ByVal SenderGUID As ULong, ByVal Message As String, ByVal msgType As ChatMsg, ByVal msgLanguage As LANGUAGES, Optional ByVal Flag As Byte = 0, Optional ByVal msgChannel As String = "Global") As PacketClass
        Dim packet As New PacketClass(OPCODES.SMSG_MESSAGECHAT)
        Try
            packet.AddInt8(msgType)
            packet.AddInt32(msgLanguage)

            Select Case msgType
                Case ChatMsg.CHAT_MSG_CHANNEL
                    packet.AddString(msgChannel)
                    packet.AddUInt32(0)
                    packet.AddUInt64(SenderGUID)
                Case ChatMsg.CHAT_MSG_YELL, ChatMsg.CHAT_MSG_SAY, ChatMsg.CHAT_MSG_PARTY
                    packet.AddUInt64(SenderGUID)
                    packet.AddUInt64(SenderGUID)
                Case ChatMsg.CHAT_MSG_SYSTEM, ChatMsg.CHAT_MSG_EMOTE, ChatMsg.CHAT_MSG_IGNORED, ChatMsg.CHAT_MSG_SKILL, ChatMsg.CHAT_MSG_GUILD, ChatMsg.CHAT_MSG_OFFICER, ChatMsg.CHAT_MSG_RAID, ChatMsg.CHAT_MSG_WHISPER_INFORM, ChatMsg.CHAT_MSG_GUILD, ChatMsg.CHAT_MSG_WHISPER, ChatMsg.CHAT_MSG_AFK, ChatMsg.CHAT_MSG_DND, ChatMsg.CHAT_MSG_RAID_LEADER, ChatMsg.CHAT_MSG_RAID_WARNING
                    packet.AddUInt64(SenderGUID)
                Case ChatMsg.CHAT_MSG_MONSTER_SAY, ChatMsg.CHAT_MSG_MONSTER_EMOTE, ChatMsg.CHAT_MSG_MONSTER_YELL
                    Log.WriteLine(LogType.WARNING, "Use Creature.SendChatMessage() for this message type - {0}!", msgType)
                Case Else
                    Log.WriteLine(LogType.WARNING, "Unknown chat message type - {0}!", msgType)
            End Select

            packet.AddUInt32(Text.Encoding.UTF8.GetByteCount(Message) + 1)
            packet.AddString(Message)

            packet.AddInt8(Flag)
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

    Function BuildPartyMemberStatsOffline(ByVal GUID As ULong) As PacketClass
        Dim packet As New PacketClass(OPCODES.SMSG_PARTY_MEMBER_STATS_FULL)
        packet.AddPackGUID(GUID)
        packet.AddUInt32(PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS)
        packet.AddInt8(PartyMemberStatsStatus.STATUS_OFFLINE)
        Return packet
    End Function

#End Region

End Module