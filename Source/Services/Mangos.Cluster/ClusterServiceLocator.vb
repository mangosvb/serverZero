Imports Autofac
Imports Mangos.Cluster.DataStores
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Handlers
Imports Mangos.Cluster.Server
Imports Mangos.Common
Imports Mangos.Common.Globals

Public Module ClusterServiceLocator
    Public Property _Container As IContainer = CreateContainer()

    Public Property _Global_Constants As Global_Constants = _Container.Resolve(Of Global_Constants)
    Public Property _CommonGlobalFunctions As Mangos.Common.Globals.Functions = _Container.Resolve(Of Mangos.Common.Globals.Functions)
    Public Property _CommonFunctions As Mangos.Common.Functions = _Container.Resolve(Of Mangos.Common.Functions)
    Public Property _GlobalZip As GlobalZip = _Container.Resolve(Of GlobalZip)
    Public Property _NativeMethods As NativeMethods = _Container.Resolve(Of NativeMethods)

    Public Property _WorldCluster As WorldCluster = _Container.Resolve(Of WorldCluster)
    Public Property _WS_DBCDatabase As WS_DBCDatabase = _Container.Resolve(Of WS_DBCDatabase)
    Public Property _WS_DBCLoad As WS_DBCLoad = _Container.Resolve(Of WS_DBCLoad)
    Public Property _Functions As Globals.Functions = _Container.Resolve(Of Globals.Functions)
    Public Property _Packets As Packets = _Container.Resolve(Of Packets)
    Public Property _WC_Guild As WC_Guild = _Container.Resolve(Of WC_Guild)
    Public Property _WC_Stats As WC_Stats = _Container.Resolve(Of WC_Stats)
    Public Property _WC_Network As WC_Network = _Container.Resolve(Of WC_Network)
    Public Property _WC_Handlers As WC_Handlers = _Container.Resolve(Of WC_Handlers)
    Public Property _WC_Handlers_Auth As WC_Handlers_Auth = _Container.Resolve(Of WC_Handlers_Auth)
    Public Property _WC_Handlers_Battleground As WC_Handlers_Battleground = _Container.Resolve(Of WC_Handlers_Battleground)
    Public Property _WC_Handlers_Chat As WC_Handlers_Chat = _Container.Resolve(Of WC_Handlers_Chat)
    Public Property _WC_Handlers_Group As WC_Handlers_Group = _Container.Resolve(Of WC_Handlers_Group)
    Public Property _WC_Handlers_Guild As WC_Handlers_Guild = _Container.Resolve(Of WC_Handlers_Guild)
    Public Property _WC_Handlers_Misc As WC_Handlers_Misc = _Container.Resolve(Of WC_Handlers_Misc)
    Public Property _WC_Handlers_Social As WC_Handlers_Social = _Container.Resolve(Of WC_Handlers_Social)
    Public Property _WC_Handlers_Tickets As WC_Handlers_Tickets = _Container.Resolve(Of WC_Handlers_Tickets)
    Public Property _WS_Handler_Channels As WS_Handler_Channels = _Container.Resolve(Of WS_Handler_Channels)
    Public Property _WcHandlerCharacter As WcHandlerCharacter = _Container.Resolve(Of WcHandlerCharacter)
End Module
