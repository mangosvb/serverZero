Imports Autofac
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
    Public Property _Container As IContainer = CreateContainer()

    Public Property _Global_Constants As Global_Constants = _Container.Resolve(Of Global_Constants)
    Public Property _CommonGlobalFunctions As Mangos.Common.Globals.Functions = _Container.Resolve(Of Mangos.Common.Globals.Functions)
    Public Property _CommonFunctions As Mangos.Common.Functions = _Container.Resolve(Of Mangos.Common.Functions)
    Public Property _GlobalZip As GlobalZip = _Container.Resolve(Of GlobalZip)
    Public Property _NativeMethods As Mangos.Common.NativeMethods = _Container.Resolve(Of Mangos.Common.NativeMethods)

    Public Property _WorldServer As WorldServer = _Container.Resolve(Of WorldServer)
    Public Property _Functions As Globals.Functions = _Container.Resolve(Of Globals.Functions)
    Public Property _WS_Creatures_AI As WS_Creatures_AI = _Container.Resolve(Of WS_Creatures_AI)
    Public Property _WS_Auction As WS_Auction = _Container.Resolve(Of WS_Auction)
    Public Property _WS_Battlegrounds As WS_Battlegrounds = _Container.Resolve(Of WS_Battlegrounds)
    Public Property _WS_DBCDatabase As WS_DBCDatabase = _Container.Resolve(Of WS_DBCDatabase)
    Public Property _WS_DBCLoad As WS_DBCLoad = _Container.Resolve(Of WS_DBCLoad)
    Public Property _Packets As Packets = _Container.Resolve(Of Packets)
    Public Property _WS_GuardGossip As WS_GuardGossip = _Container.Resolve(Of WS_GuardGossip)
    Public Property _WS_Loot As WS_Loot = _Container.Resolve(Of WS_Loot)
    Public Property _WS_Maps As WS_Maps = _Container.Resolve(Of WS_Maps)
    Public Property _WS_Corpses As WS_Corpses = _Container.Resolve(Of WS_Corpses)
    Public Property _WS_Creatures As WS_Creatures = _Container.Resolve(Of WS_Creatures)
    Public Property _WS_DynamicObjects As WS_DynamicObjects = _Container.Resolve(Of WS_DynamicObjects)
    Public Property _WS_GameObjects As WS_GameObjects = _Container.Resolve(Of WS_GameObjects)
    Public Property _WS_Items As WS_Items = _Container.Resolve(Of WS_Items)
    Public Property _WS_NPCs As WS_NPCs = _Container.Resolve(Of WS_NPCs)
    Public Property _WS_Pets As WS_Pets = _Container.Resolve(Of WS_Pets)
    Public Property _WS_Transports As WS_Transports = _Container.Resolve(Of WS_Transports)
    Public Property _CharManagementHandler As CharManagementHandler = _Container.Resolve(Of CharManagementHandler)
    Public Property _WS_CharMovement As WS_CharMovement = _Container.Resolve(Of WS_CharMovement)
    Public Property _WS_Combat As WS_Combat = _Container.Resolve(Of WS_Combat)
    Public Property _WS_Commands As WS_Commands = _Container.Resolve(Of WS_Commands)
    Public Property _WS_Handlers As WS_Handlers = _Container.Resolve(Of WS_Handlers)
    Public Property _WS_Handlers_Battleground As WS_Handlers_Battleground = _Container.Resolve(Of WS_Handlers_Battleground)
    Public Property _WS_Handlers_Chat As WS_Handlers_Chat = _Container.Resolve(Of WS_Handlers_Chat)
    Public Property _WS_Handlers_Gamemaster As WS_Handlers_Gamemaster = _Container.Resolve(Of WS_Handlers_Gamemaster)
    Public Property _WS_Handlers_Instance As WS_Handlers_Instance = _Container.Resolve(Of WS_Handlers_Instance)
    Public Property _WS_Handlers_Misc As WS_Handlers_Misc = _Container.Resolve(Of WS_Handlers_Misc)
    Public Property _WS_Handlers_Taxi As WS_Handlers_Taxi = _Container.Resolve(Of WS_Handlers_Taxi)
    Public Property _WS_Handlers_Trade As WS_Handlers_Trade = _Container.Resolve(Of WS_Handlers_Trade)
    Public Property _WS_Handlers_Warden As WS_Handlers_Warden = _Container.Resolve(Of WS_Handlers_Warden)
    Public Property _WS_Player_Creation As WS_Player_Creation = _Container.Resolve(Of WS_Player_Creation)
    Public Property _WS_Player_Initializator As WS_Player_Initializator = _Container.Resolve(Of WS_Player_Initializator)
    Public Property _WS_PlayerData As WS_PlayerData = _Container.Resolve(Of WS_PlayerData)
    Public Property _WS_PlayerHelper As WS_PlayerHelper = _Container.Resolve(Of WS_PlayerHelper)
    Public Property _WS_Network As WS_Network = _Container.Resolve(Of WS_Network)
    Public Property _WS_TimerBasedEvents As WS_TimerBasedEvents = _Container.Resolve(Of WS_TimerBasedEvents)
    Public Property _WS_Group As WS_Group = _Container.Resolve(Of WS_Group)
    Public Property _WS_Guilds As WS_Guilds = _Container.Resolve(Of WS_Guilds)
    Public Property _WS_Mail As WS_Mail = _Container.Resolve(Of WS_Mail)
    Public Property _WS_Spells As WS_Spells = _Container.Resolve(Of WS_Spells)
    Public Property _WS_Warden As WS_Warden = _Container.Resolve(Of WS_Warden)
    Public Property _WS_Weather As WS_Weather = _Container.Resolve(Of WS_Weather)
End Module
