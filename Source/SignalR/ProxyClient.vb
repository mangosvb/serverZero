Imports System.Reflection
Imports Microsoft.AspNetCore.SignalR.Client

Public Class ProxyClient
    Inherits DispatchProxy

    Private hubConnection As HubConnection

    Protected Overrides Function Invoke(targetMethod As MethodInfo, args() As Object) As Object
        If targetMethod.ReturnType.Name = "Void" Then
            hubConnection.InvokeCoreAsync(targetMethod.Name, args).Wait()
            Return Nothing
        Else
            Return hubConnection.InvokeCoreAsync(targetMethod.Name, targetMethod.ReturnType, args).Result
        End If
    End Function

    Public Shared Function Create(Of T)(url As String) As T
        Dim hubConnectionBuilder = New HubConnectionBuilder()
        hubConnectionBuilder.WithUrl(url)
        Dim hubConnection = hubConnectionBuilder.Build()
        hubConnection.StartAsync().Wait()

        Dim proxy = DispatchProxy.Create(Of T, ProxyClient)()
        TryCast(proxy, ProxyClient).hubConnection = hubConnection
        Return proxy
    End Function

End Class
