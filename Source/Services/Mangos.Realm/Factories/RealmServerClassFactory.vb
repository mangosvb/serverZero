Imports Mangos.Configuration

Namespace Factories

    Public Class RealmServerClassFactory
        Private ReadOnly _Global_Constants As Global_Constants
        Private ReadOnly _ClientClassFactory As ClientClassFactory
        Private ReadOnly _configurationProvider As IConfigurationProvider(Of RealmServerConfiguration)

        Sub New(globalConstants As Global_Constants,
                clientClassFactory As ClientClassFactory,
                configurationProvider As IConfigurationProvider(Of RealmServerConfiguration))
            _Global_Constants = globalConstants
            _ClientClassFactory = clientClassFactory
            _configurationProvider = configurationProvider
        End Sub

        Function Create(realmServer As RealmServer) As RealmServerClass
            Return New RealmServerClass(_Global_Constants,
                                        _ClientClassFactory,
                                        realmServer,
                                        _configurationProvider)
        End Function
    End Class
End Namespace