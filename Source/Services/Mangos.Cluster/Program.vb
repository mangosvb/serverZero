Imports Autofac
Imports Mangos.Cluster.DataStores
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Handlers
Imports Mangos.Cluster.Server
Imports Mangos.Common
Imports Mangos.Common.Globals

Public Module Program
    Sub Main()
        _WorldCluster.Main()
    End Sub

    Function CreateContainer() As IContainer
        Dim builder As New ContainerBuilder

        builder.RegisterType(Of Global_Constants).As(Of Global_Constants)()
        builder.RegisterType(Of Mangos.Common.Globals.Functions).As(Of Mangos.Common.Globals.Functions)()
        builder.RegisterType(Of Mangos.Common.Functions).As(Of Mangos.Common.Functions)()
        builder.RegisterType(Of GlobalZip).As(Of GlobalZip)()
        builder.RegisterType(Of NativeMethods).As(Of NativeMethods)()

        builder.RegisterType(Of WorldCluster).As(Of WorldCluster)()
        builder.RegisterType(Of WS_DBCDatabase).As(Of WS_DBCDatabase)()
        builder.RegisterType(Of WS_DBCLoad).As(Of WS_DBCLoad)()
        builder.RegisterType(Of Globals.Functions).As(Of Globals.Functions)()
        builder.RegisterType(Of Packets).As(Of Packets)()
        builder.RegisterType(Of WC_Guild).As(Of WC_Guild)()
        builder.RegisterType(Of WC_Stats).As(Of WC_Stats)()
        builder.RegisterType(Of WC_Network).As(Of WC_Network)()
        builder.RegisterType(Of WC_Handlers).As(Of WC_Handlers)()
        builder.RegisterType(Of WC_Handlers_Auth).As(Of WC_Handlers_Auth)()
        builder.RegisterType(Of WC_Handlers_Battleground).As(Of WC_Handlers_Battleground)()
        builder.RegisterType(Of WC_Handlers_Chat).As(Of WC_Handlers_Chat)()
        builder.RegisterType(Of WC_Handlers_Group).As(Of WC_Handlers_Group)()
        builder.RegisterType(Of WC_Handlers_Guild).As(Of WC_Handlers_Guild)()
        builder.RegisterType(Of WC_Handlers_Misc).As(Of WC_Handlers_Misc)()
        builder.RegisterType(Of WC_Handlers_Social).As(Of WC_Handlers_Social)()
        builder.RegisterType(Of WC_Handlers_Tickets).As(Of WC_Handlers_Tickets)()
        builder.RegisterType(Of WS_Handler_Channels).As(Of WS_Handler_Channels)()
        builder.RegisterType(Of WcHandlerCharacter).As(Of WcHandlerCharacter)()

        Return builder.Build()
    End Function
End Module
