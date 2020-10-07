Imports Mangos.Common.Globals
Imports Mangos.World.AI
Imports Mangos.World.Auction
Imports Mangos.World.Battlegrounds
Imports Mangos.World.DataStores
Imports Mangos.World.Globals
Imports Mangos.World.Gossip
Imports Mangos.World.Handlers
Imports Mangos.World.Loots
Imports Mangos.World.Maps
Imports Mangos.World.Objects
Imports Mangos.World.Player
Imports Mangos.World.Server
Imports Mangos.World.Social
Imports Mangos.World.Spells
Imports Mangos.World.Warden
Imports Mangos.World.Weather

Public Module WorldServiceLocator
    Public Property _Global_Constants As New Global_Constants
    Public Property _CommonGlobalFunctions As Mangos.Common.Globals.Functions
    Public Property _CommonFunctions As New Mangos.Common.Functions
    Public Property _GlobalZip As New GlobalZip
    Public Property _NativeMethods As New Mangos.Common.NativeMethods

    Public Property _WorldServer As New WorldServer
    Public Property _Functions As New Globals.Functions
    Public Property _WS_Creatures_AI As New WS_Creatures_AI
    Public Property _WS_Auction As New WS_Auction
    Public Property _WS_Battlegrounds As New WS_Battlegrounds
    Public Property _WS_DBCDatabase As New WS_DBCDatabase
    Public Property _WS_DBCLoad As New WS_DBCLoad
    Public Property _Packets As New Packets
    Public Property _WS_GuardGossip As New WS_GuardGossip
    Public Property _WS_Loot As New WS_Loot
    Public Property _WS_Maps As New WS_Maps
    Public Property _WS_Corpses As New WS_Corpses
    Public Property _WS_Creatures As New WS_Creatures
    Public Property _WS_DynamicObjects As New WS_DynamicObjects
    Public Property _WS_GameObjects As New WS_GameObjects
    Public Property _WS_Items As New WS_Items
    Public Property _WS_NPCs As New WS_NPCs
    Public Property _WS_Pets As New WS_Pets
    Public Property _WS_Transports As New WS_Transports
    Public Property _CharManagementHandler As New CharManagementHandler
    Public Property _WS_CharMovement As New WS_CharMovement
    Public Property _WS_Combat As New WS_Combat
    Public Property _WS_Commands As New WS_Commands
    Public Property _WS_Handlers As New WS_Handlers
    Public Property _WS_Handlers_Battleground As New WS_Handlers_Battleground
    Public Property _WS_Handlers_Chat As New WS_Handlers_Chat
    Public Property _WS_Handlers_Gamemaster As New WS_Handlers_Gamemaster
    Public Property _WS_Handlers_Instance As New WS_Handlers_Instance
    Public Property _WS_Handlers_Misc As New WS_Handlers_Misc
    Public Property _WS_Handlers_Taxi As New WS_Handlers_Taxi
    Public Property _WS_Handlers_Trade As New WS_Handlers_Trade
    Public Property _WS_Handlers_Warden As New WS_Handlers_Warden
    Public Property _WS_Player_Creation As New WS_Player_Creation
    Public Property _WS_Player_Initializator As New WS_Player_Initializator
    Public Property _WS_PlayerData As New WS_PlayerData
    Public Property _WS_PlayerHelper As New WS_PlayerHelper
    Public Property _WS_Network As New WS_Network
    Public Property _WS_TimerBasedEvents As New WS_TimerBasedEvents
    Public Property _WS_Group As New WS_Group
    Public Property _WS_Guilds As New WS_Guilds
    Public Property _WS_Mail As New WS_Mail
    Public Property _WS_Spells As New WS_Spells
    Public Property _WS_Warden As New WS_Warden
    Public Property _WS_Weather As New WS_Weather

    Sub New()
        _CommonGlobalFunctions = New Mangos.Common.Globals.Functions(_Global_Constants)
    End Sub
End Module
