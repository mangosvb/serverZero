Imports Autofac
Imports Mangos.Cluster.DataStores
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Handlers
Imports Mangos.Cluster.Server
Imports Mangos.Common
Imports Mangos.Common.Globals
Imports Mangos.Configuration
Imports Mangos.Configuration.Store
Imports Mangos.Configuration.Xml

Public Module Program
    Sub Main()
        Dim container = _Container
        Dim worldCluster = container.Resolve(Of WorldCluster)
        worldCluster.Start()
    End Sub

    Function CreateContainer() As IContainer
        Dim builder As New ContainerBuilder
        RegisterConfiguration(builder)
        RegisterServices(builder)
        Return builder.Build()
    End Function

    Sub RegisterConfiguration(builder As ContainerBuilder)
        builder.Register(Function(x) New XmlFileConfigurationProvider(Of ClusterServerConfiguration)("configs/WorldCluster.ini")).As(Of IConfigurationProvider(Of ClusterServerConfiguration))().SingleInstance()
        builder.RegisterDecorator(Of StoredConfigurationProvider(Of ClusterServerConfiguration), IConfigurationProvider(Of ClusterServerConfiguration))()
    End Sub

    Sub RegisterServices(builder As ContainerBuilder)
        builder.RegisterType(Of Global_Constants).As(Of Global_Constants)().SingleInstance()
        builder.RegisterType(Of Mangos.Common.Globals.Functions).As(Of Mangos.Common.Globals.Functions)().SingleInstance()
        builder.RegisterType(Of Mangos.Common.Functions).As(Of Mangos.Common.Functions)().SingleInstance()
        builder.RegisterType(Of GlobalZip).As(Of GlobalZip)().SingleInstance()
        builder.RegisterType(Of NativeMethods).As(Of NativeMethods)().SingleInstance()

        builder.RegisterType(Of WorldCluster).As(Of WorldCluster)().SingleInstance()
        builder.RegisterType(Of WS_DBCDatabase).As(Of WS_DBCDatabase)().SingleInstance()
        builder.RegisterType(Of WS_DBCLoad).As(Of WS_DBCLoad)().SingleInstance()
        builder.RegisterType(Of Globals.Functions).As(Of Globals.Functions)().SingleInstance()
        builder.RegisterType(Of Packets).As(Of Packets)().SingleInstance()
        builder.RegisterType(Of WC_Guild).As(Of WC_Guild)().SingleInstance()
        builder.RegisterType(Of WC_Stats).As(Of WC_Stats)().SingleInstance()
        builder.RegisterType(Of WC_Network).As(Of WC_Network)().SingleInstance()
        builder.RegisterType(Of WC_Handlers).As(Of WC_Handlers)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Auth).As(Of WC_Handlers_Auth)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Battleground).As(Of WC_Handlers_Battleground)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Chat).As(Of WC_Handlers_Chat)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Group).As(Of WC_Handlers_Group)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Guild).As(Of WC_Handlers_Guild)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Misc).As(Of WC_Handlers_Misc)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Movement).As(Of WC_Handlers_Movement)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Social).As(Of WC_Handlers_Social)().SingleInstance()
        builder.RegisterType(Of WC_Handlers_Tickets).As(Of WC_Handlers_Tickets)().SingleInstance()
        builder.RegisterType(Of WS_Handler_Channels).As(Of WS_Handler_Channels)().SingleInstance()
        builder.RegisterType(Of WcHandlerCharacter).As(Of WcHandlerCharacter)().SingleInstance()
    End Sub
End Module
