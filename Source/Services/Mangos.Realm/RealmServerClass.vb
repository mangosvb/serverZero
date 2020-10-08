Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports Mangos.Configuration
Imports Mangos.Realm.Factories

Public Class RealmServerClass
    Implements IDisposable

    Private ReadOnly _configurationProvider As IConfigurationProvider(Of RealmServerConfiguration)

    Private ReadOnly _Global_Constants As Global_Constants
    Private ReadOnly _ClientClassFactory As ClientClassFactory
    Private ReadOnly _RealmServer As RealmServer

    Public Sub New(globalConstants As Global_Constants,
                   clientClassFactory As ClientClassFactory,
                   realmServer As RealmServer,
                   configurationProvider As IConfigurationProvider(Of RealmServerConfiguration))
        _Global_Constants = globalConstants
        _ClientClassFactory = clientClassFactory
        _RealmServer = realmServer
        _configurationProvider = configurationProvider
    End Sub

    Public Async Function StartAsync() As Task
        Dim configuration = Await _configurationProvider.GetConfigurationAsync()
        LstHost = IPAddress.Parse(configuration.RealmServerAddress)
        Try
            Dim tcpListener As TcpListener = New TcpListener(LstHost, configuration.RealmServerPort)
            LstConnection = tcpListener
            LstConnection.Start()

            Dim rsListenThread As Thread
            Dim thread As Thread = New Thread(AddressOf AcceptConnection) With {
                    .Name = "Realm Server, Listening"
                    }
            rsListenThread = thread
            rsListenThread.Start()

            Console.WriteLine("[{0}] Listening on {1} on port {2}", Format(TimeOfDay, "hh:mm:ss"), LstHost, configuration.RealmServerPort)
        Catch e As Exception
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("[{0}] Error in {2}: {1}.", Format(TimeOfDay, "hh:mm:ss"), e.Message, e.Source)
            Console.ForegroundColor = ConsoleColor.Gray
        End Try
    End Function

    Private Sub AcceptConnection()
        Do While Not FlagStopListen
            Thread.Sleep(_Global_Constants.ConnectionSleepTime)
            If LstConnection.Pending() Then
                Dim client = _ClientClassFactory.Create(_RealmServer)
                client.Socket = LstConnection.AcceptSocket
                Call New Thread(AddressOf client.Process).Start()
            End If
        Loop
    End Sub

#Region "IDisposable Support"

    Private _disposedValue As Boolean ' To detect redundant calls

    Public Property FlagStopListen As Boolean = False
    Public Property LstConnection As TcpListener
    Public Property LstHost As IPAddress

    ' IDisposable
    'Default Functions
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            FlagStopListen = True
            LstConnection.Stop()
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