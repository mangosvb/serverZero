Public Class ClientClassFactory
    Private ReadOnly _Global_Constants As Global_Constants

    Sub New(globalConstants As Global_Constants)
        _Global_Constants = globalConstants
    End Sub

    Function Create(realmServer As RealmServer) As ClientClass
        Return New ClientClass(_Global_Constants, realmServer)
    End Function
End Class
