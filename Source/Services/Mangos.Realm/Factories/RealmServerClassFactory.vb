Public Class RealmServerClassFactory
    Private ReadOnly _Global_Constants As Global_Constants
    Private ReadOnly _ClientClassFactory As ClientClassFactory

    Sub New(globalConstants As Global_Constants, clientClassFactory As ClientClassFactory)
        _Global_Constants = globalConstants
        _ClientClassFactory = clientClassFactory
    End Sub

    Function Create(realmServer As RealmServer) As RealmServerClass
        Return New RealmServerClass(_Global_Constants, _ClientClassFactory, realmServer)
    End Function
End Class
