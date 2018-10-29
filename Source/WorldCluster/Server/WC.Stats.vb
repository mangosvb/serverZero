'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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
Imports System.Reflection
Imports System.Threading
Imports System.Xml
Imports mangosVB.Common
Imports mangosVB.Shared
Imports WorldCluster.Handlers

Namespace Server

    Public Module WC_Stats

        'http://www.15seconds.com/issue/050615.htm

        Private ConnectionsHandled As Integer = 0
        Private ConnectionsPeak As Integer = 0
        Private ConnectionsCurrent As Integer = 0

        Public Sub ConnectionsIncrement()
            Interlocked.Increment(ConnectionsHandled)
            If Interlocked.Increment(ConnectionsCurrent) > ConnectionsPeak Then
                ConnectionsPeak = ConnectionsCurrent
            End If
        End Sub
        Public Sub ConnectionsDecrement()
            Interlocked.Decrement(ConnectionsCurrent)
        End Sub

        Public DataTransferOut As Long = 0
        Public DataTransferIn As Long = 0

        Private ThreadsWorker As Integer = 0
        Private ThreadsComletion As Integer = 0
        Private LastCheck As Date = Now
        Private LastCPUTime As Double = 0
        Private Uptime As TimeSpan
        Private Latency As Long = 0
        Private UsageCPU As Single = 0.0F
        Private UsageMemory As Long = 0

        Private CountPlayers As Integer = 0
        Private CountPlayersAlliance As Integer = 0
        Private CountPlayersHorde As Integer = 0
        Private CountGMs As Integer = 0

        Private w As New Dictionary(Of WorldInfo, List(Of String))

        Private Function FormatUptime(time As TimeSpan) As String
            Return String.Format("{0}d {1}h {2}m {3}s {4}ms", time.Days, time.Hours, time.Minutes, time.Seconds, time.Milliseconds)
        End Function

        Public Sub CheckCpu(state As Object)
            Dim timeSinceLastCheck As TimeSpan = Now.Subtract(LastCheck)
            UsageCPU = ((Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds - LastCPUTime) / timeSinceLastCheck.TotalMilliseconds) * 100
            LastCheck = Now
            LastCPUTime = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds
        End Sub

        Private Sub PrepareStats()
            Uptime = Now.Subtract(Process.GetCurrentProcess().StartTime)

            ThreadPool.GetAvailableThreads(ThreadsWorker, ThreadsComletion)

            UsageMemory = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024)

            CountPlayers = 0
            CountPlayersHorde = 0
            CountPlayersAlliance = 0
            CountGMs = 0
            Latency = 0

            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each objCharacter As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If objCharacter.Value.IsInWorld Then
                    CountPlayers += 1

                    If objCharacter.Value.Race = Races.RACE_ORC OrElse objCharacter.Value.Race = Races.RACE_TAUREN OrElse objCharacter.Value.Race = Races.RACE_TROLL OrElse objCharacter.Value.Race = Races.RACE_UNDEAD Then
                        CountPlayersHorde += 1
                    Else
                        CountPlayersAlliance += 1
                    End If

                    If objCharacter.Value.Access > AccessLevel.Player Then CountGMs += 1
                    Latency += objCharacter.Value.Latency
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()

            If CountPlayers > 1 Then
                Latency = Latency \ CountPlayers
            End If

            For Each objCharacter As KeyValuePair(Of UInteger, WorldInfo) In WorldServer.WorldsInfo
                If Not w.ContainsKey(objCharacter.Value) Then
                    w.Add(objCharacter.Value, New List(Of String))
                End If
                w(objCharacter.Value).Add(objCharacter.Key)
            Next
        End Sub
        Public Sub GenerateStats(state As Object)
            Log.WriteLine(LogType.DEBUG, "Generating stats")
            PrepareStats()

            Dim f As XmlWriter = XmlWriter.Create(_config.StatsLocation)
            f.WriteStartDocument(True)
            f.WriteComment("generated at " & Date.Now.ToString("hh:mm:ss"))
            '<?xml-stylesheet type="text/xsl" href="stats.xsl"?>
            f.WriteProcessingInstruction("xml-stylesheet", "type=""text/xsl"" href=""stats.xsl""")
            '<server>
            f.WriteStartElement("server")

            '<cluster>
            f.WriteStartElement("cluster")

            f.WriteStartElement("platform")
            f.WriteValue(String.Format("mangosVB rev{0}", [Assembly].GetExecutingAssembly().GetName().Version))
            f.WriteEndElement()

            f.WriteStartElement("uptime")
            f.WriteValue(FormatUptime(Uptime))
            f.WriteEndElement()

            f.WriteStartElement("onlineplayers")
            f.WriteValue(CountPlayers)
            f.WriteEndElement()

            f.WriteStartElement("gmcount")
            f.WriteValue(CountGMs)
            f.WriteEndElement()

            f.WriteStartElement("alliance")
            f.WriteValue(CountPlayersAlliance)
            f.WriteEndElement()

            f.WriteStartElement("horde")
            f.WriteValue(CountPlayersHorde)
            f.WriteEndElement()

            f.WriteStartElement("cpu")
            f.WriteValue(Format(UsageCPU, "0.00"))
            f.WriteEndElement()

            f.WriteStartElement("ram")
            f.WriteValue(UsageMemory)
            f.WriteEndElement()

            f.WriteStartElement("latency")
            f.WriteValue(Latency)
            f.WriteEndElement()

            f.WriteStartElement("connaccepted")
            f.WriteValue(ConnectionsHandled)
            f.WriteEndElement()

            f.WriteStartElement("connpeak")
            f.WriteValue(ConnectionsPeak)
            f.WriteEndElement()

            f.WriteStartElement("conncurrent")
            f.WriteValue(ConnectionsCurrent)
            f.WriteEndElement()

            f.WriteStartElement("networkin")
            f.WriteValue(DataTransferIn)
            f.WriteEndElement()

            f.WriteStartElement("networkout")
            f.WriteValue(DataTransferOut)
            f.WriteEndElement()

            f.WriteStartElement("threadsw")
            f.WriteValue(ThreadsWorker)
            f.WriteEndElement()

            f.WriteStartElement("threadsc")
            f.WriteValue(ThreadsComletion)
            f.WriteEndElement()

            f.WriteStartElement("lastupdate")
            f.WriteValue(Now.ToString)
            f.WriteEndElement()

            '</cluster>
            f.WriteEndElement()

            '<world>
            f.WriteStartElement("world")
            Try
                For Each objCharacter As KeyValuePair(Of WorldInfo, List(Of String)) In w
                    f.WriteStartElement("instance")
                    f.WriteStartElement("uptime")
                    f.WriteValue(FormatUptime(Now - objCharacter.Key.Started))
                    f.WriteEndElement()
                    f.WriteStartElement("players")
                    f.WriteValue("-")
                    f.WriteEndElement()
                    f.WriteStartElement("maps")
                    f.WriteValue(Join(objCharacter.Value.ToArray, ", "))
                    f.WriteEndElement()
                    f.WriteStartElement("cpu")
                    f.WriteValue(Format(objCharacter.Key.CPUUsage, "0.00"))
                    f.WriteEndElement()
                    f.WriteStartElement("ram")
                    f.WriteValue(objCharacter.Key.MemoryUsage)
                    f.WriteEndElement()
                    f.WriteStartElement("latency")
                    f.WriteValue(objCharacter.Key.Latency)
                    f.WriteEndElement()

                    f.WriteEndElement()
                Next
            Catch ex As Exception
                Log.WriteLine(LogType.FAILED, "Error while generating stats file: {0}", ex.ToString)
            End Try
            '</world>
            f.WriteEndElement()

            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)

            f.WriteStartElement("users")
            For Each objCharacter As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If objCharacter.Value.IsInWorld AndAlso objCharacter.Value.Access >= AccessLevel.GameMaster Then
                    f.WriteStartElement("gmplayer")
                    f.WriteStartElement("name")
                    f.WriteValue(objCharacter.Value.Name)
                    f.WriteEndElement()
                    f.WriteStartElement("access")
                    f.WriteValue(objCharacter.Value.Access)
                    f.WriteEndElement()
                    f.WriteEndElement()
                End If
            Next
            f.WriteEndElement()

            f.WriteStartElement("sessions")
            For Each objCharacter As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If objCharacter.Value.IsInWorld Then
                    f.WriteStartElement("player")
                    f.WriteStartElement("name")
                    f.WriteValue(objCharacter.Value.Name)
                    f.WriteEndElement()
                    f.WriteStartElement("race")
                    f.WriteValue(objCharacter.Value.Race)
                    f.WriteEndElement()
                    f.WriteStartElement("class")
                    f.WriteValue(objCharacter.Value.Classe)
                    f.WriteEndElement()
                    f.WriteStartElement("level")
                    f.WriteValue(objCharacter.Value.Level)
                    f.WriteEndElement()
                    f.WriteStartElement("map")
                    f.WriteValue(objCharacter.Value.Map)
                    f.WriteEndElement()
                    f.WriteStartElement("zone")
                    f.WriteValue(objCharacter.Value.Zone)
                    f.WriteEndElement()
                    f.WriteStartElement("ontime")
                    f.WriteValue(FormatUptime(Now - objCharacter.Value.Time))
                    f.WriteEndElement()
                    f.WriteStartElement("latency")
                    f.WriteValue(objCharacter.Value.Latency)
                    f.WriteEndElement()

                    f.WriteEndElement()
                End If
            Next
            f.WriteEndElement()

            CHARACTERs_Lock.ReleaseReaderLock()

            '</server>
            f.WriteEndElement()
            f.WriteEndDocument()
            f.Close()

            w.Clear()
        End Sub

    End Module
End Namespace