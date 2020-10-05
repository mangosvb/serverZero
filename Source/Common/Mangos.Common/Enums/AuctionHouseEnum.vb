'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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
Namespace Enums
    Public Module AuctionHouseEnum

        Public Enum AuctionHouses As Integer
            AUCTION_UNDEFINED = 0
            AUCTION_ALLIANCE = 2
            AUCTION_HORDE = 6
            AUCTION_NEUTRAL = 7
            AUCTION_STORMWIND = 1
            AUCTION_IRONFORGE = 2
            AUCTION_DARNASSYS = 3
            AUCTION_UNDERCITY = 4
            AUCTION_THUNDER_BLUFF = 5
            AUCTION_ORGRIMMAR = 6
            AUCTION_BLACKWATER = 7
        End Enum

        Public Enum AuctionAction As Integer
            AUCTION_SELL_ITEM = 0
            AUCTION_CANCEL = 1
            AUCTION_PLACE_BID = 2
        End Enum

        Public Enum AuctionError As Integer
            AUCTION_OK = 0
            AUCTION_INTERNAL_ERROR = 2
            AUCTION_NOT_ENOUGHT_MONEY = 3
            CANNOT_BID_YOUR_AUCTION_ERROR = 10
        End Enum

        'Auction Mail Format:
        '
        'Outbid
        '       Subject -> ItemID:0:0
        '       Body    -> ""
        '       Money returned
        'Auction won
        '       Subject -> ItemID:0:1
        '       Body    -> FFFFFFFF:Bid:Buyout
        '       Item received    
        'Auction Successful
        '       Subject -> ItemID:0:2
        '       Body    -> FFFFFFFF:Bid:Buyout:0:0
        '       Money received   
        'Auction Canceled
        '       Subject -> ItemID:0:4
        '       Body    -> ""
        '       Item returned
        Public Enum MailAuctionAction As Integer
            OUTBID = 0
            AUCTION_WON = 1
            AUCTION_SUCCESSFUL = 2
            AUCTION_CANCELED = 3
        End Enum

    End Module
End NameSpace