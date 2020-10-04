Imports System.Net
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.SignalR
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Public Class ProxyServer(Of T As Hub)
    Implements IDisposable

    Private webhost As IHost

    Sub New(address As IPAddress, port As Integer, hub As T)
        Dim hostbuilder = Host.CreateDefaultBuilder()
        hostbuilder.ConfigureWebHost(Sub(x) ConfigureWebHost(x, address, port, hub))
        webhost = hostbuilder.Build()
        webhost.Start()
    End Sub

    Private Sub ConfigureWebHost(webHostBuilder As IWebHostBuilder, address As IPAddress, port As Integer, hub As T)
        webHostBuilder.UseKestrel(Sub(x) x.Listen(address, port))
        webHostBuilder.ConfigureLogging(Sub(x As ILoggingBuilder) x.ClearProviders())
        webHostBuilder.ConfigureServices(Sub(x As IServiceCollection) ConfigureServices(x, hub))
        webHostBuilder.Configure(AddressOf ConfigureApplication)
    End Sub

    Private Sub ConfigureServices(serviceCollection As IServiceCollection, hub As T)
        serviceCollection.AddSignalR(AddressOf ConfigureSignalR)
        serviceCollection.AddSingleton(hub)
    End Sub

    Private Sub ConfigureApplication(applicationBuilder As IApplicationBuilder)
        applicationBuilder.UseRouting()
        applicationBuilder.UseEndpoints(Sub(x) x.MapHub(Of T)(String.Empty))
    End Sub

    Private Sub ConfigureSignalR(hubOptions As HubOptions)
        hubOptions.EnableDetailedErrors = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        webhost.Dispose()
    End Sub
End Class
