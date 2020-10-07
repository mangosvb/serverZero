Imports Mangos.Cluster.DataStores
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Handlers
Imports Mangos.Cluster.Server

Public Module ClusterServiceLocator
    Public Property _WorldCluster As New WorldCluster
    Public Property _WS_DBCDatabase As New WS_DBCDatabase
    Public Property _WS_DBCLoad As New WS_DBCLoad
    Public Property _Functions As New Functions
    Public Property _Packets As New Packets
    Public Property _WC_Guild As New WC_Guild
    Public Property _WC_Stats As New WC_Stats
    Public Property _WC_Network As New WC_Network
    Public Property _WC_Handlers As New WC_Handlers
    Public Property _WC_Handlers_Auth As New WC_Handlers_Auth
    Public Property _WC_Handlers_Battleground As New WC_Handlers_Battleground
    Public Property _WC_Handlers_Chat As New WC_Handlers_Chat
    Public Property _WC_Handlers_Group As New WC_Handlers_Group
    Public Property _WC_Handlers_Guild As New WC_Handlers_Guild
    Public Property _WC_Handlers_Misc As New WC_Handlers_Misc
    Public Property _WC_Handlers_Social As New WC_Handlers_Social
    Public Property _WC_Handlers_Tickets As New WC_Handlers_Tickets
    Public Property _WS_Handler_Channels As New WS_Handler_Channels
    Public Property _WcHandlerCharacter As New WcHandlerCharacter
End Module
