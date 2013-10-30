'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
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

Imports mangosVB.Common.BaseWriter

Public Module WS_Battlegrounds

    Public BATTLEFIELDs As New Dictionary(Of Integer, Battlefield)

    Public Class Battlefield
        Implements IDisposable

        Public MembersTeam1 As New List(Of CharacterObject)
        Public MembersTeam2 As New List(Of CharacterObject)

        Public ID As Integer
        Public Map As UInteger
        Public MapType As BattlefieldMapType

        Public Sub New(ByVal rMapType As BattlefieldMapType, ByVal rMap As UInteger)
            BATTLEFIELDs.Add(ID, Me)
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                BATTLEFIELDs.Remove(ID)
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

        Public Sub Update(ByVal State As Object)
        End Sub

        Public Sub Broadcast(ByVal p As PacketClass)
            BroadcastTeam1(p)
            BroadcastTeam2(p)
        End Sub
        Public Sub BroadcastTeam1(ByVal p As PacketClass)
            For Each objCharacter As CharacterObject In MembersTeam1.ToArray
                objCharacter.Client.SendMultiplyPackets(p)
            Next
        End Sub
        Public Sub BroadcastTeam2(ByVal p As PacketClass)
            For Each objCharacter As CharacterObject In MembersTeam2.ToArray
                objCharacter.Client.SendMultiplyPackets(p)
            Next
        End Sub

    End Class



End Module
