Imports Autofac
Imports Mangos.Common.Globals
Imports Mangos.Configuration
Imports Mangos.Configuration.Store
Imports Mangos.Configuration.Xml
Imports Mangos.Realm.Factories

Public Module Program
    Sub Main()
        Dim container = CreateContainer()
        Dim realmServer = container.Resolve(Of RealmServer)
        realmServer.StartAsync().Wait()
    End Sub

    Function CreateContainer() As IContainer
        Dim builder As New ContainerBuilder
        RegisterConfiguration(builder)
        RegisterServices(builder)
        Return builder.Build()
    End Function

    Sub RegisterConfiguration(builder As ContainerBuilder)
        builder.Register(Function(x) New XmlFileConfigurationProvider(Of RealmServerConfiguration)("configs/RealmServer.ini")).As(Of IConfigurationProvider(Of RealmServerConfiguration))().SingleInstance()
        builder.RegisterDecorator(Of StoredConfigurationProvider(Of RealmServerConfiguration), IConfigurationProvider(Of RealmServerConfiguration))()
    End Sub

    Sub RegisterServices(builder As ContainerBuilder)
        builder.RegisterType(Of RealmServer).As(Of RealmServer)().SingleInstance()
        builder.RegisterType(Of RealmServerClass).As(Of RealmServerClass)()
        builder.RegisterType(Of Converter).As(Of Converter)().SingleInstance()
        builder.RegisterType(Of Global_Constants).As(Of Global_Constants)().SingleInstance()
        builder.RegisterType(Of Functions).As(Of Functions)().SingleInstance()

        builder.RegisterType(Of ClientClassFactory).As(Of ClientClassFactory)().SingleInstance()
        builder.RegisterType(Of RealmServerClassFactory).As(Of RealmServerClassFactory)().SingleInstance()
    End Sub
End Module
