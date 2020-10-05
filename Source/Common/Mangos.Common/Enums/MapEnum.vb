'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Namespace Enums
    Public Module MapEnum

        Public Enum Axis As Integer
            X_AXIS = 0
            Y_AXIS = 1
            Z_AXIS = 2
            DETECT_AXIS = -1
        End Enum

        Public Enum MapTypes As Integer
            MAP_COMMON = 0
            MAP_INSTANCE = 1
            MAP_RAID = 2
            MAP_BATTLEGROUND = 3
        End Enum

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

        Public Enum TransferAbortReason As Short
            TRANSFER_ABORT_MAX_PLAYERS = &H1                ' Transfer Aborted: instance is full
            TRANSFER_ABORT_NOT_FOUND = &H2                  ' Transfer Aborted: instance not found
            TRANSFER_ABORT_TOO_MANY_INSTANCES = &H3         ' You have entered too many instances recently.
            TRANSFER_ABORT_ZONE_IN_COMBAT = &H5             ' Unable to zone in while an encounter is in progress.
            TRANSFER_ABORT_INSUF_EXPAN_LVL1 = &H106         ' You must have TBC expansion installed to access this area.
        End Enum

    End Module
End NameSpace