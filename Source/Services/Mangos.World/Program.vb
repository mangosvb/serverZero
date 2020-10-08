Imports Autofac
Imports Mangos.Common.Globals
Imports Mangos.Configuration
Imports Mangos.Configuration.Store
Imports Mangos.Configuration.Xml
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

Public Module Program
    Sub Main()
        Dim container = _Container
        Dim worldServer = container.Resolve(Of WorldServer)
        worldServer.Start()
    End Sub

    Function CreateContainer() As IContainer
        Dim builder As New ContainerBuilder
        RegisterConfiguration(builder)
        RegisterServices(builder)
        Return builder.Build()
    End Function

    Sub RegisterConfiguration(builder As ContainerBuilder)
        builder.Register(Function(x) New XmlFileConfigurationProvider(Of WorldServerConfiguration)("configs/WorldServer.ini")).As(Of IConfigurationProvider(Of WorldServerConfiguration))().SingleInstance()
        builder.RegisterDecorator(Of StoredConfigurationProvider(Of WorldServerConfiguration), IConfigurationProvider(Of WorldServerConfiguration))()
    End Sub

    Sub RegisterServices(builder As ContainerBuilder)
        builder.RegisterType(Of Global_Constants).As(Of Global_Constants)().SingleInstance()
        builder.RegisterType(Of Mangos.Common.Globals.Functions).As(Of Mangos.Common.Globals.Functions)().SingleInstance()
        builder.RegisterType(Of Mangos.Common.Functions).As(Of Mangos.Common.Functions)().SingleInstance()
        builder.RegisterType(Of GlobalZip).As(Of GlobalZip)().SingleInstance()
        builder.RegisterType(Of Mangos.Common.NativeMethods).As(Of Mangos.Common.NativeMethods)().SingleInstance()

        builder.RegisterType(Of WorldServer).As(Of WorldServer)().SingleInstance()
        builder.RegisterType(Of Globals.Functions).As(Of Globals.Functions)().SingleInstance()
        builder.RegisterType(Of WS_Creatures_AI).As(Of WS_Creatures_AI)().SingleInstance()
        builder.RegisterType(Of WS_Auction).As(Of WS_Auction)().SingleInstance()
        builder.RegisterType(Of WS_Battlegrounds).As(Of WS_Battlegrounds)().SingleInstance()
        builder.RegisterType(Of WS_DBCDatabase).As(Of WS_DBCDatabase)().SingleInstance()
        builder.RegisterType(Of WS_DBCLoad).As(Of WS_DBCLoad)().SingleInstance()
        builder.RegisterType(Of Packets).As(Of Packets)().SingleInstance()
        builder.RegisterType(Of WS_GuardGossip).As(Of WS_GuardGossip)().SingleInstance()
        builder.RegisterType(Of WS_Loot).As(Of WS_Loot)().SingleInstance()
        builder.RegisterType(Of WS_Maps).As(Of WS_Maps)().SingleInstance()
        builder.RegisterType(Of WS_Corpses).As(Of WS_Corpses)().SingleInstance()
        builder.RegisterType(Of WS_Creatures).As(Of WS_Creatures)().SingleInstance()
        builder.RegisterType(Of WS_DynamicObjects).As(Of WS_DynamicObjects)().SingleInstance()
        builder.RegisterType(Of WS_GameObjects).As(Of WS_GameObjects)().SingleInstance()
        builder.RegisterType(Of WS_Items).As(Of WS_Items)().SingleInstance()
        builder.RegisterType(Of WS_NPCs).As(Of WS_NPCs)().SingleInstance()
        builder.RegisterType(Of WS_Pets).As(Of WS_Pets)().SingleInstance()
        builder.RegisterType(Of WS_Transports).As(Of WS_Transports)().SingleInstance()
        builder.RegisterType(Of CharManagementHandler).As(Of CharManagementHandler)().SingleInstance()
        builder.RegisterType(Of WS_CharMovement).As(Of WS_CharMovement)().SingleInstance()
        builder.RegisterType(Of WS_Combat).As(Of WS_Combat)().SingleInstance()
        builder.RegisterType(Of WS_Commands).As(Of WS_Commands)().SingleInstance()
        builder.RegisterType(Of WS_Handlers).As(Of WS_Handlers)().SingleInstance()
        builder.RegisterType(Of WS_Handlers_Battleground).As(Of WS_Handlers_Battleground)().SingleInstance()
        builder.RegisterType(Of WS_Handlers_Chat).As(Of WS_Handlers_Chat)().SingleInstance()
        builder.RegisterType(Of WS_Handlers_Gamemaster).As(Of WS_Handlers_Gamemaster)().SingleInstance()
        builder.RegisterType(Of WS_Handlers_Instance).As(Of WS_Handlers_Instance)().SingleInstance()
        builder.RegisterType(Of WS_Handlers_Misc).As(Of WS_Handlers_Misc)().SingleInstance()
        builder.RegisterType(Of WS_Handlers_Taxi).As(Of WS_Handlers_Taxi)().SingleInstance()
        builder.RegisterType(Of WS_Handlers_Trade).As(Of WS_Handlers_Trade)().SingleInstance()

        builder.RegisterType(Of WS_Handlers_Warden).As(Of WS_Handlers_Warden)().SingleInstance()
        builder.RegisterType(Of WS_Player_Creation).As(Of WS_Player_Creation)().SingleInstance()
        builder.RegisterType(Of WS_Player_Initializator).As(Of WS_Player_Initializator)().SingleInstance()
        builder.RegisterType(Of WS_PlayerData).As(Of WS_PlayerData)().SingleInstance()
        builder.RegisterType(Of WS_PlayerHelper).As(Of WS_PlayerHelper)().SingleInstance()
        builder.RegisterType(Of WS_Network).As(Of WS_Network)().SingleInstance()
        builder.RegisterType(Of WS_TimerBasedEvents).As(Of WS_TimerBasedEvents)().SingleInstance()

        builder.RegisterType(Of WS_Group).As(Of WS_Group)().SingleInstance()
        builder.RegisterType(Of WS_Guilds).As(Of WS_Guilds)().SingleInstance()
        builder.RegisterType(Of WS_Mail).As(Of WS_Mail)().SingleInstance()
        builder.RegisterType(Of WS_Spells).As(Of WS_Spells)().SingleInstance()
        builder.RegisterType(Of WS_Warden).As(Of WS_Warden)().SingleInstance()
        builder.RegisterType(Of WS_Weather).As(Of WS_Weather)().SingleInstance()
    End Sub
End Module
