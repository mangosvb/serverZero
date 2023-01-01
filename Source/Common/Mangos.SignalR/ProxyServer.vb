'
' Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
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

Imports System.Net
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.SignalR
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Public Class ProxyServer(Of T As Hub)
    Implements IDisposable

    Private ReadOnly webhost As IHost

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
