﻿'
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
