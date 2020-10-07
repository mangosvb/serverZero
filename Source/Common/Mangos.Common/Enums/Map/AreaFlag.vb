Namespace Enums.Map
    Public Enum AreaFlag As Integer
        AREA_FLAG_SNOW = &H1                ' snow (only Dun Morogh, Naxxramas, Razorfen Downs and Winterspring)
        AREA_FLAG_UNK1 = &H2                ' unknown, (only Naxxramas and Razorfen Downs)
        AREA_FLAG_UNK2 = &H4                ' Only used on development map
        AREA_FLAG_SLAVE_CAPITAL = &H8       ' slave capital city flag?
        AREA_FLAG_UNK3 = &H10               ' unknown
        AREA_FLAG_SLAVE_CAPITAL2 = &H20     ' slave capital city flag?
        AREA_FLAG_UNK4 = &H40               ' many zones have this flag
        AREA_FLAG_ARENA = &H80              ' arena, both instanced and world arenas
        AREA_FLAG_CAPITAL = &H100           ' main capital city flag
        AREA_FLAG_CITY = &H200              ' only for one zone named "City" (where it located?)
        AREA_FLAG_SANCTUARY = &H800         ' sanctuary area (PvP disabled)
        AREA_FLAG_NEED_FLY = &H1000         ' only Netherwing Ledge, Socrethar's Seat, Tempest Keep, The Arcatraz, The Botanica, The Mechanar, Sorrow Wing Point, Dragonspine Ridge, Netherwing Mines, Dragonmaw Base Camp, Dragonmaw Skyway
        AREA_FLAG_UNUSED1 = &H2000          ' not used now (no area/zones with this flag set in 2.4.2)
        AREA_FLAG_PVP = &H8000              ' pvp objective area? (Death's Door also has this flag although it's no pvp object area)
        AREA_FLAG_ARENA_INSTANCE = &H10000  ' used by instanced arenas only
        AREA_FLAG_UNUSED2 = &H20000         ' not used now (no area/zones with this flag set in 2.4.2)
        AREA_FLAG_UNK5 = &H40000            ' just used for Amani Pass, Hatchet Hills
        AREA_FLAG_LOWLEVEL = &H100000       ' used for some starting areas with area_level <=15
    End Enum
End NameSpace