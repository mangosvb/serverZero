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

Public Module Program
    Sub Main()
        _WorldServer.Main()
    End Sub

    Function CreateContainer() As IContainer
        Dim builder As New ContainerBuilder

        builder.RegisterType(Of Global_Constants).As(Of Global_Constants)()
        builder.RegisterType(Of Mangos.Common.Globals.Functions).As(Of Mangos.Common.Globals.Functions)()
        builder.RegisterType(Of Mangos.Common.Functions).As(Of Mangos.Common.Functions)()
        builder.RegisterType(Of GlobalZip).As(Of GlobalZip)()
        builder.RegisterType(Of Mangos.Common.NativeMethods).As(Of Mangos.Common.NativeMethods)()

        builder.RegisterType(Of WorldServer).As(Of WorldServer)()
        builder.RegisterType(Of Globals.Functions).As(Of Globals.Functions)()
        builder.RegisterType(Of WS_Creatures_AI).As(Of WS_Creatures_AI)()
        builder.RegisterType(Of WS_Auction).As(Of WS_Auction)()
        builder.RegisterType(Of WS_Battlegrounds).As(Of WS_Battlegrounds)()
        builder.RegisterType(Of WS_DBCDatabase).As(Of WS_DBCDatabase)()
        builder.RegisterType(Of WS_DBCLoad).As(Of WS_DBCLoad)()
        builder.RegisterType(Of Packets).As(Of Packets)()
        builder.RegisterType(Of WS_GuardGossip).As(Of WS_GuardGossip)()
        builder.RegisterType(Of WS_Loot).As(Of WS_Loot)()
        builder.RegisterType(Of WS_Maps).As(Of WS_Maps)()
        builder.RegisterType(Of WS_Corpses).As(Of WS_Corpses)()
        builder.RegisterType(Of WS_Creatures).As(Of WS_Creatures)()
        builder.RegisterType(Of WS_DynamicObjects).As(Of WS_DynamicObjects)()
        builder.RegisterType(Of WS_GameObjects).As(Of WS_GameObjects)()
        builder.RegisterType(Of WS_Items).As(Of WS_Items)()
        builder.RegisterType(Of WS_NPCs).As(Of WS_NPCs)()
        builder.RegisterType(Of WS_Pets).As(Of WS_Pets)()
        builder.RegisterType(Of WS_Transports).As(Of WS_Transports)()
        builder.RegisterType(Of CharManagementHandler).As(Of CharManagementHandler)()
        builder.RegisterType(Of WS_CharMovement).As(Of WS_CharMovement)()
        builder.RegisterType(Of WS_Combat).As(Of WS_Combat)()
        builder.RegisterType(Of WS_Commands).As(Of WS_Commands)()
        builder.RegisterType(Of WS_Handlers).As(Of WS_Handlers)()
        builder.RegisterType(Of WS_Handlers_Battleground).As(Of WS_Handlers_Battleground)()
        builder.RegisterType(Of WS_Handlers_Chat).As(Of WS_Handlers_Chat)()
        builder.RegisterType(Of WS_Handlers_Gamemaster).As(Of WS_Handlers_Gamemaster)()
        builder.RegisterType(Of WS_Handlers_Instance).As(Of WS_Handlers_Instance)()
        builder.RegisterType(Of WS_Handlers_Misc).As(Of WS_Handlers_Misc)()
        builder.RegisterType(Of WS_Handlers_Taxi).As(Of WS_Handlers_Taxi)()
        builder.RegisterType(Of WS_Handlers_Trade).As(Of WS_Handlers_Trade)()

        builder.RegisterType(Of WS_Handlers_Warden).As(Of WS_Handlers_Warden)()
        builder.RegisterType(Of WS_Player_Creation).As(Of WS_Player_Creation)()
        builder.RegisterType(Of WS_Player_Initializator).As(Of WS_Player_Initializator)()
        builder.RegisterType(Of WS_PlayerData).As(Of WS_PlayerData)()
        builder.RegisterType(Of WS_PlayerHelper).As(Of WS_PlayerHelper)()
        builder.RegisterType(Of WS_Network).As(Of WS_Network)()
        builder.RegisterType(Of WS_TimerBasedEvents).As(Of WS_TimerBasedEvents)()

        builder.RegisterType(Of WS_Group).As(Of WS_Group)()
        builder.RegisterType(Of WS_Guilds).As(Of WS_Guilds)()
        builder.RegisterType(Of WS_Mail).As(Of WS_Mail)()
        builder.RegisterType(Of WS_Spells).As(Of WS_Spells)()
        builder.RegisterType(Of WS_Warden).As(Of WS_Warden)()
        builder.RegisterType(Of WS_Weather).As(Of WS_Weather)()

        Return builder.Build()
    End Function
End Module
