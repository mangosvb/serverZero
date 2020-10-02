Imports System.Net
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.SignalR
Imports Microsoft.Extensions.DependencyInjection

Public Class ProxyServer(Of T As Hub)
    Implements IDisposable

    Private webhost As IWebHost

    Sub New(address As IPAddress, port As Integer, hub As T)
        Dim hostbuilder = New WebHostBuilder()
        hostbuilder.UseKestrel(Sub(x) x.Listen(address, port))
        hostbuilder.ConfigureServices(Sub(x As IServiceCollection) x.AddSignalR(AddressOf ConfigureSignalR))
        hostbuilder.ConfigureServices(Sub(x As IServiceCollection) x.AddSingleton(hub))
        hostbuilder.Configure(Sub(x As IApplicationBuilder) x.UseSignalR(Sub(y) y.MapHub(Of T)(New PathString(String.Empty))))
        webhost = hostbuilder.Build()
        webhost.Start()
    End Sub

    Private Sub ConfigureSignalR(hubOptions As HubOptions)
        hubOptions.EnableDetailedErrors = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        webhost.Dispose()
    End Sub
End Class
