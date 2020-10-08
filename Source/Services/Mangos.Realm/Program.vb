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
        builder.Register(Function(x) New XmlFileConfigurationProvider(Of RealmServerConfiguration)("configs/RealmServer.ini")).As(Of IConfigurationProvider(Of RealmServerConfiguration))()
        builder.RegisterDecorator(Of StoredConfigurationProvider(Of RealmServerConfiguration), IConfigurationProvider(Of RealmServerConfiguration))()
    End Sub

    Sub RegisterServices(builder As ContainerBuilder)

        builder.RegisterType(Of RealmServer).As(Of RealmServer)()
        builder.RegisterType(Of RealmServerClass).As(Of RealmServerClass)()
        builder.RegisterType(Of Converter).As(Of Converter)()
        builder.RegisterType(Of Global_Constants).As(Of Global_Constants)()
        builder.RegisterType(Of Functions).As(Of Functions)()

        builder.RegisterType(Of ClientClassFactory).As(Of ClientClassFactory)()
        builder.RegisterType(Of RealmServerClassFactory).As(Of RealmServerClassFactory)()
    End Sub
End Module
