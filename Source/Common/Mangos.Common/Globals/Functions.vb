'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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

'Note: Temp place holder
Imports System.Data
Imports Mangos.Common.Enums

Namespace Globals
    Public Module Functions
        Public Function GuidIsCreature(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_UNIT Then Return True
            Return False
        End Function

        Public Function GuidIsPet(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_PET Then Return True
            Return False
        End Function

        Public Function GuidIsItem(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_ITEM Then Return True
            Return False
        End Function

        Public Function GuidIsGameObject(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_GAMEOBJECT Then Return True
            Return False
        End Function

        Public Function GuidIsDnyamicObject(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_DYNAMICOBJECT Then Return True
            Return False
        End Function

        Public Function GuidIsTransport(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_TRANSPORT Then Return True
            Return False
        End Function

        Public Function GuidIsMoTransport(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_MO_TRANSPORT Then Return True
            Return False
        End Function

        Public Function GuidIsCorpse(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_CORPSE Then Return True
            Return False
        End Function

        Public Function GuidIsPlayer(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_PLAYER Then Return True
            Return False
        End Function

        Public Function GuidHigh2(ByVal guid As ULong) As ULong
            Return (guid And GUID_MASK_HIGH)
        End Function

        Public Function GuidHigh(ByVal guid As ULong) As UInteger
            Return (guid And GUID_MASK_HIGH) >> 32UL
        End Function

        Public Function GuidLow(ByVal guid As ULong) As UInteger
            Return (guid And GUID_MASK_LOW)
        End Function

        Public Function GetShapeshiftModel(ByVal form As GlobalEnum.ShapeshiftForm, ByVal race As Races, ByVal model As Integer) As Integer
            Select Case form
                Case ShapeshiftForm.FORM_CAT
                    If race = Races.RACE_NIGHT_ELF Then Return 892
                    If race = Races.RACE_TAUREN Then Return 8571
                Case ShapeshiftForm.FORM_BEAR, ShapeshiftForm.FORM_DIREBEAR
                    If race = Races.RACE_NIGHT_ELF Then Return 2281
                    If race = Races.RACE_TAUREN Then Return 2289
                Case ShapeshiftForm.FORM_MOONKIN
                    If race = Races.RACE_NIGHT_ELF Then Return 15374
                    If race = Races.RACE_TAUREN Then Return 15375
                Case ShapeshiftForm.FORM_TRAVEL
                    Return 632
                Case ShapeshiftForm.FORM_AQUA
                    Return 2428
                Case ShapeshiftForm.FORM_FLIGHT
                    If race = Races.RACE_NIGHT_ELF Then Return 20857
                    If race = Races.RACE_TAUREN Then Return 20872
                Case ShapeshiftForm.FORM_SWIFT
                    If race = Races.RACE_NIGHT_ELF Then Return 21243
                    If race = Races.RACE_TAUREN Then Return 21244
                Case ShapeshiftForm.FORM_GHOUL
                    If race = Races.RACE_NIGHT_ELF Then Return 10045 Else Return model
                Case ShapeshiftForm.FORM_CREATUREBEAR
                    Return 902
                Case ShapeshiftForm.FORM_GHOSTWOLF
                    Return 4613
                Case ShapeshiftForm.FORM_SPIRITOFREDEMPTION
                    Return 12824
                Case Else
                    Return model
                    'Case ShapeshiftForm.FORM_CREATURECAT
                    'Case ShapeshiftForm.FORM_AMBIENT
                    'Case ShapeshiftForm.FORM_SHADOW
            End Select
        End Function

        Public Function GetShapeshiftManaType(ByVal form As ShapeshiftForm, ByVal manaType As ManaTypes) As ManaTypes
            Select Case form
                Case ShapeshiftForm.FORM_CAT, ShapeshiftForm.FORM_STEALTH
                    Return ManaTypes.TYPE_ENERGY
                Case ShapeshiftForm.FORM_AQUA, ShapeshiftForm.FORM_TRAVEL, ShapeshiftForm.FORM_MOONKIN,
                 ShapeshiftForm.FORM_MOONKIN, ShapeshiftForm.FORM_MOONKIN, ShapeshiftForm.FORM_SPIRITOFREDEMPTION, ShapeshiftForm.FORM_FLIGHT, ShapeshiftForm.FORM_SWIFT
                    Return ManaTypes.TYPE_MANA
                Case ShapeshiftForm.FORM_BEAR, ShapeshiftForm.FORM_DIREBEAR
                    Return ManaTypes.TYPE_RAGE
                Case Else
                    Return manaType
            End Select
        End Function

        Public Function CheckRequiredDbVersion(ByRef thisDatabase As SQL, thisServerDb As ServerDb) As Boolean
            Dim mySqlQuery As New DataTable
            'thisDatabase.Query(String.Format("SELECT column_name FROM information_schema.columns WHERE table_name='" & thisTableName & "'  AND TABLE_SCHEMA='" & thisDatabase.SQLDBName & "'"), mySqlQuery)
            thisDatabase.Query("SELECT `version`,`structure`,`content` FROM db_version", mySqlQuery)
            'Check database version against code version

            Dim coreDbVersion As Integer = 0
            Dim coreDbStructure As Integer = 0
            Dim coreDbContent As Integer = 0
            Select Case thisServerDb
                Case ServerDb.Realm
                    coreDbVersion = RevisionDbRealmVersion
                    coreDbStructure = RevisionDbRealmStructure
                    coreDbContent = RevisionDbRealmContent
                Case ServerDb.Character
                    coreDbVersion = RevisionDbCharactersVersion
                    coreDbStructure = RevisionDbCharactersStructure
                    coreDbContent = RevisionDbCharactersContent
                Case ServerDb.World
                    coreDbVersion = RevisionDbMangosVersion
                    coreDbStructure = RevisionDbMangosStructure
                    coreDbContent = RevisionDbMangosContent
            End Select

            If mySqlQuery.Rows.Count > 0 Then
                'For Each row As DataRow In mySqlQuery.Rows
                '    dtVersion = row.Item("column_name").ToString
                'Next
                Dim dbVersion As Integer = Convert.ToInt32(mySqlQuery.Rows(0).Item("version").ToString())
                Dim dbStructure As Integer = Convert.ToInt32(mySqlQuery.Rows(0).Item("structure").ToString())
                Dim dbContent As Integer = Convert.ToInt32(mySqlQuery.Rows(0).Item("content").ToString())

                'NOTES: Version or Structure mismatch is a hard error, Content mismatch as a warning

                If dbVersion = coreDbVersion And dbStructure = coreDbStructure And dbContent = coreDbContent Then 'Full Match
                    Console.WriteLine("[{0}] Db Version Matched", Format(TimeOfDay, "hh:mm:ss"))
                    Return True
                ElseIf dbVersion = coreDbVersion And dbStructure = coreDbStructure And dbContent <> coreDbContent Then 'Content MisMatch, only a warning
                    Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] -- WARNING: CONTENT VERSION MISMATCH                        --", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}]  Your Database " & thisDatabase.SQLDBName & " requires updating.", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}]  You have: Rev{1}.{2}.{3}, however the core expects Rev{4}.{5}.{6}", Format(TimeOfDay, "hh:mm:ss"), dbVersion, dbStructure, dbContent, coreDbVersion, coreDbStructure, coreDbContent)
                    Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}]  The server will run, but you may be missing some database fixes", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                    Return True
                Else 'Oh no they do not match
                    Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] -- FATAL ERROR: VERSION MISMATCH                            --", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}]  Your Database " & thisDatabase.SQLDBName & " requires updating.", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}]  You have: Rev{1}.{2}.{3}, however the core expects Rev{4}.{5}.{6}", Format(TimeOfDay, "hh:mm:ss"), dbVersion, dbStructure, dbContent, coreDbVersion, coreDbStructure, coreDbContent)
                    Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}]  The server is unable to run until the required updates are run", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] You must apply all updates after Rev{1}.{2}.{3} ", Format(TimeOfDay, "hh:mm:ss"), coreDbVersion, coreDbStructure, coreDbContent)
                    Console.WriteLine("[{0}] These updates are included in the sql/updates folder.", Format(TimeOfDay, "hh:mm:ss"))
                    Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                    'Console.WriteLine("*************************")
                    'Console.WriteLine("* Press any key to exit *")
                    'Console.WriteLine("*************************")
                    'Console.ReadKey()
                    Return False
                End If
            Else
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] The table `db_version` in database " & thisDatabase.SQLDBName & " is missing", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] MaNGOSVB cannot find the version info required, please update", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] your database to check that the db is up to date.", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] your database to Rev{1}.{2}.{3} ", Format(TimeOfDay, "hh:mm:ss"), coreDbVersion, coreDbStructure, coreDbContent)
                'Console.WriteLine("*************************")
                'Console.WriteLine("* Press any key to exit *")
                'Console.WriteLine("*************************")
                '                Console.ReadKey()
                Return False
            End If
        End Function
    End Module
End Namespace
