'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Public Module MiscEnum

    Public Enum ExpansionLevel As Byte
        NORMAL = 0      'Vanilla
    End Enum

    Public Enum AccessLevel As Byte
        Trial = 0
        Player = 1
        GameMaster = 2
        Developer = 3
        Admin = 4
    End Enum

    Public Enum TimeConstant
        MINUTE = 60
        HOUR = MINUTE * 60
        DAY = HOUR * 24
        WEEK = DAY * 7
        MONTH = DAY * 30
        YEAR = MONTH * 12
        IN_MILLISECONDS = 1000
    End Enum

    Public Enum WeatherSounds As Integer
        WEATHER_SOUND_NOSOUND = 0
        WEATHER_SOUND_RAINLIGHT = 8533
        WEATHER_SOUND_RAINMEDIUM = 8534
        WEATHER_SOUND_RAINHEAVY = 8535
        WEATHER_SOUND_SNOWLIGHT = 8536
        WEATHER_SOUND_SNOWMEDIUM = 8537
        WEATHER_SOUND_SNOWHEAVY = 8538
        WEATHER_SOUND_SANDSTORMLIGHT = 8556
        WEATHER_SOUND_SANDSTORMMEDIUM = 8557
        WEATHER_SOUND_SANDSTORMHEAVY = 8558
    End Enum

    Public Enum WeatherType As Integer
        WEATHER_FINE = 0
        WEATHER_RAIN = 1
        WEATHER_SNOW = 2
        WEATHER_SANDSTORM = 3
    End Enum

    Public Enum LANGUAGES As Integer
        LANG_GLOBAL = 0
        LANG_UNIVERSAL = 0
        LANG_ORCISH = 1
        LANG_DARNASSIAN = 2
        LANG_TAURAHE = 3
        LANG_DWARVISH = 6
        LANG_COMMON = 7
        LANG_DEMONIC = 8
        LANG_TITAN = 9
        LANG_DRACONIC = 11
        LANG_KALIMAG = 12
        LANG_GNOMISH = 13
        LANG_TROLL = 14
        LANG_GUTTERSPEAK = 33
    End Enum

    Public Enum BuyBackSlots As Byte  '12 Slots
        BUYBACK_SLOT_START = 69
        BUYBACK_SLOT_1 = 69
        BUYBACK_SLOT_2 = 70
        BUYBACK_SLOT_3 = 71
        BUYBACK_SLOT_4 = 72
        BUYBACK_SLOT_5 = 73
        BUYBACK_SLOT_6 = 74
        BUYBACK_SLOT_7 = 75
        BUYBACK_SLOT_8 = 76
        BUYBACK_SLOT_9 = 77
        BUYBACK_SLOT_10 = 78
        BUYBACK_SLOT_11 = 79
        BUYBACK_SLOT_12 = 80
        BUYBACK_SLOT_END = 81
    End Enum

    Enum POI_ICON
        ICON_POI_0 = 0                                         ' Grey ?
        ICON_POI_1 = 1                                         ' Red ?
        ICON_POI_2 = 2                                         ' Blue ?
        ICON_POI_BWTOMB = 3                                    ' Blue and White Tomb Stone
        ICON_POI_HOUSE = 4                                     ' House
        ICON_POI_TOWER = 5                                     ' Tower
        ICON_POI_REDFLAG = 6                                   ' Red Flag with Yellow !
        ICON_POI_TOMB = 7                                      ' Tomb Stone
        ICON_POI_BWTOWER = 8                                   ' Blue and White Tower
        ICON_POI_REDTOWER = 9                                  ' Red Tower
        ICON_POI_BLUETOWER = 10                                ' Blue Tower
        ICON_POI_RWTOWER = 11                                  ' Red and White Tower
        ICON_POI_REDTOMB = 12                                  ' Red Tomb Stone
        ICON_POI_RWTOMB = 13                                   ' Red and White Tomb Stone
        ICON_POI_BLUETOMB = 14                                 ' Blue Tomb Stone
        ICON_POI_NOTHING = 15                                  ' NOTHING
        ICON_POI_16 = 16                                       ' Red ?
        ICON_POI_17 = 17                                       ' Grey ?
        ICON_POI_18 = 18                                       ' Blue ?
        ICON_POI_19 = 19                                       ' Red and White ?
        ICON_POI_20 = 20                                       ' Red ?
        ICON_POI_GREYLOGS = 21                                 ' Grey Wood Logs
        ICON_POI_BWLOGS = 22                                   ' Blue and White Wood Logs
        ICON_POI_BLUELOGS = 23                                 ' Blue Wood Logs
        ICON_POI_RWLOGS = 24                                   ' Red and White Wood Logs
        ICON_POI_REDLOGS = 25                                  ' Red Wood Logs
        ICON_POI_26 = 26                                       ' Grey ?
        ICON_POI_27 = 27                                       ' Blue and White ?
        ICON_POI_28 = 28                                       ' Blue ?
        ICON_POI_29 = 29                                       ' Red and White ?
        ICON_POI_30 = 30                                       ' Red ?
        ICON_POI_GREYHOUSE = 31                                ' Grey House
        ICON_POI_BWHOUSE = 32                                  ' Blue and White House
        ICON_POI_BLUEHOUSE = 33                                ' Blue House
        ICON_POI_RWHOUSE = 34                                  ' Red and White House
        ICON_POI_REDHOUSE = 35                                 ' Red House
        ICON_POI_GREYHORSE = 36                                ' Grey Horse
        ICON_POI_BWHORSE = 37                                  ' Blue and White Horse
        ICON_POI_BLUEHORSE = 38                                ' Blue Horse
        ICON_POI_RWHORSE = 39                                  ' Red and White Horse
        ICON_POI_REDHORSE = 40                                 ' Red Horse
    End Enum

    Public Enum InvalidReason
        DontHaveReq = 0
        DontHaveReqItems = 19
        DontHaveReqMoney = 21
        NotAvailableRace = 6
        NotEnoughLevel = 1
        ReadyHaveThatQuest = 13
        ReadyHaveTimedQuest = 12
    End Enum

    Public Enum SELL_ERROR As Byte
        SELL_ERR_CANT_FIND_ITEM = 1
        SELL_ERR_CANT_SELL_ITEM = 2
        SELL_ERR_CANT_FIND_VENDOR = 3
    End Enum

    Public Enum BUY_ERROR As Byte
        'SMSG_BUY_FAILED error
        '0: cant find item
        '1: item already selled
        '2: not enought money
        '4: seller(dont Like u)
        '5: distance too far
        '8: cant carry more
        '11: level(require)
        '12: reputation(require)

        BUY_ERR_CANT_FIND_ITEM = 0
        BUY_ERR_ITEM_ALREADY_SOLD = 1
        BUY_ERR_NOT_ENOUGHT_MONEY = 2
        BUY_ERR_SELLER_DONT_LIKE_YOU = 4
        BUY_ERR_DISTANCE_TOO_FAR = 5
        BUY_ERR_CANT_CARRY_MORE = 8
        BUY_ERR_LEVEL_REQUIRE = 11
        BUY_ERR_REPUTATION_REQUIRE = 12
    End Enum

End Module
