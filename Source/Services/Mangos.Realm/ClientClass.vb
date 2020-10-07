Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports Mangos.Common.Enums.Authentication
Imports Mangos.Common.Enums.Misc

Public NotInheritable Class ClientClass
    Implements IDisposable

    Private ReadOnly _Global_Constants As Global_Constants
    Private ReadOnly _RealmServer As RealmServer

    Sub New(globalConstants As Global_Constants, realmServer As RealmServer)
        _Global_Constants = globalConstants
        _RealmServer = realmServer
    End Sub

    Public Socket As Socket
    Public Ip As IPAddress = IPAddress.Parse("127.0.0.1")
    Public Port As Integer = 0
    Public AuthEngine As AuthEngineClass
    Public Account As String = ""
    'Public Language As String = "enGB"
    'Public Expansion As ExpansionLevel = ExpansionLevel.NORMAL
    Public UpdateFile As String = ""
    Public Access As AccessLevel = AccessLevel.Player

    Private Sub OnData(ByVal data() As Byte)
        Select Case data(0)
            Case AuthCMD.CMD_AUTH_LOGON_CHALLENGE, AuthCMD.CMD_AUTH_RECONNECT_CHALLENGE
                Console.WriteLine("[{0}] [{1}:{2}] RS_LOGON_CHALLENGE", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
                _RealmServer.On_RS_LOGON_CHALLENGE(data, Me)

            Case AuthCMD.CMD_AUTH_LOGON_PROOF, AuthCMD.CMD_AUTH_RECONNECT_PROOF
                Console.WriteLine("[{0}] [{1}:{2}] RS_LOGON_PROOF", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
                _RealmServer.On_RS_LOGON_PROOF(data, Me)

            Case AuthCMD.CMD_AUTH_REALMLIST
                Console.WriteLine("[{0}] [{1}:{2}] RS_REALMLIST", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
                _RealmServer.On_RS_REALMLIST(data, Me)

                'TODO: No Value listed for AuthCMD
                'Case CMD_AUTH_UPDATESRV
                '    Console.WriteLine("[{0}] [{1}:{2}] RS_UPDATESRV", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)

                'ToDo: Check if these packets exist in supported version
            Case AuthCMD.CMD_XFER_ACCEPT
                'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_ACCEPT", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                _RealmServer.On_CMD_XFER_ACCEPT(data, Me)

            Case AuthCMD.CMD_XFER_RESUME
                'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_RESUME", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                _RealmServer.On_CMD_XFER_RESUME(data, Me)

            Case AuthCMD.CMD_XFER_CANCEL
                'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_CANCEL", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                _RealmServer.On_CMD_XFER_CANCEL(data, Me)
            Case Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] [{1}:{2}] Unknown Opcode 0x{3}", Format(TimeOfDay, "hh:mm:ss"), Ip, Port, data(0))
                Console.ForegroundColor = ConsoleColor.Gray
                _RealmServer.DumpPacket(data, Me)
        End Select
    End Sub

    Public Sub Process()
        Dim remoteEndPoint As IPEndPoint = CType(Socket.RemoteEndPoint, IPEndPoint)
        Ip = remoteEndPoint.Address
        Port = remoteEndPoint.Port

        'DONE: Connection spam protection
        Dim ipInt As UInteger
        ipInt = _RealmServer.Ip2Int(Ip.ToString)

        If Not _RealmServer.LastSocketConnection.ContainsKey(ipInt) Then
            _RealmServer.LastSocketConnection.Add(ipInt, Now.AddSeconds(5))
        Else
            If Now > _RealmServer.LastSocketConnection(ipInt) Then
                _RealmServer.LastSocketConnection(ipInt) = Now.AddSeconds(5)
            Else
                Socket.Close()
                Dispose()
                Exit Sub
            End If
        End If

        Console.ForegroundColor = ConsoleColor.DarkGray
        Console.WriteLine("[{0}] Incoming connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
        Console.WriteLine("[{0}] [{1}:{2}] Checking for banned IP.", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
        Console.ForegroundColor = ConsoleColor.Gray
        If Not _RealmServer.AccountDatabase.QuerySQL("SELECT ip FROM ip_banned WHERE ip = '" & Ip.ToString & "';") Then

            While Not _RealmServer.RealmServer.FlagStopListen
                Thread.Sleep(_Global_Constants.ConnectionSleepTime)
                If Socket.Available > 0 Then
                    If Socket.Available > 100 Then 'DONE: Data flood protection
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("[{0}] Incoming Connection dropped for flooding", Format(TimeOfDay, "hh:mm:ss"))
                        Console.ForegroundColor = ConsoleColor.Gray
                        Exit While
                    End If


                    Dim buffer() As Byte
                    ReDim buffer(Socket.Available - 1)
                    Dim dummyBytes As Integer = Socket.Receive(buffer, buffer.Length, 0)
                    Console.WriteLine("[{0}] Incoming connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
                    Console.WriteLine("Data Packet: [{0}] ", dummyBytes)

                    OnData(buffer)
                End If
                If Not Socket.Connected Then Exit While
                If Socket.Poll(100, SelectMode.SelectRead) And (Socket.Available = 0) Then Exit While
            End While

        Else
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("[{0}] [{1}:{2}] This ip is banned.", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
            Console.ForegroundColor = ConsoleColor.Gray
        End If

        Socket.Close()

        Console.ForegroundColor = ConsoleColor.DarkGray
        Console.WriteLine("[{0}] Connection from [{1}:{2}] closed", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
        Console.ForegroundColor = ConsoleColor.Gray

        Dispose()
    End Sub

    Public Sub Send(ByVal data() As Byte, ByVal packetName As String)
        Try
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("[{0}] [{1}:{2}] ({4}) Data sent, result code {3}", Format(TimeOfDay, "hh:mm:ss"), Ip, Port, Socket.Send(data, 0, data.Length, SocketFlags.None), packetName)
            Console.ForegroundColor = ConsoleColor.Gray

        Catch err As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("[{0}] Connection from [{1}:{2}] do not exist - ERROR!!!", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
            Console.ForegroundColor = ConsoleColor.Gray
            Socket.Close()
        End Try
    End Sub

#Region "IDisposable Support"

    Private _disposedValue As Boolean

    ' To detect redundant calls

    ' IDisposable
    Private Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        _disposedValue = True
    End Sub
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region
End Class