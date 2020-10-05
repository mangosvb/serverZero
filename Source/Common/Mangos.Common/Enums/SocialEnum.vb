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
    Public Module SocialEnum

        Public Enum SocialList As Byte
            MAX_FRIENDS_ON_LIST = 50
            MAX_IGNORES_ON_LIST = 25
        End Enum

        Public Enum FriendStatus As Byte
            FRIEND_STATUS_OFFLINE = 0
            FRIEND_STATUS_ONLINE = 1
            FRIEND_STATUS_AFK = 2
            FRIEND_STATUS_UNK3 = 3
            FRIEND_STATUS_DND = 4
        End Enum

        Public Enum FriendResult As Byte
            FRIEND_DB_ERROR = &H0
            FRIEND_LIST_FULL = &H1
            FRIEND_ONLINE = &H2
            FRIEND_OFFLINE = &H3
            FRIEND_NOT_FOUND = &H4
            FRIEND_REMOVED = &H5
            FRIEND_ADDED_ONLINE = &H6
            FRIEND_ADDED_OFFLINE = &H7
            FRIEND_ALREADY = &H8
            FRIEND_SELF = &H9
            FRIEND_ENEMY = &HA
            FRIEND_IGNORE_FULL = &HB
            FRIEND_IGNORE_SELF = &HC
            FRIEND_IGNORE_NOT_FOUND = &HD
            FRIEND_IGNORE_ALREADY = &HE
            FRIEND_IGNORE_ADDED = &HF
            FRIEND_IGNORE_REMOVED = &H10
        End Enum

        Public Enum SocialFlag As Byte
            SOCIAL_FLAG_FRIEND = &H1
            SOCIAL_FLAG_IGNORED = &H2
        End Enum

        Public Enum MailResult
            MAIL_SENT = 0
            MAIL_MONEY_REMOVED = 1
            MAIL_ITEM_REMOVED = 2
            MAIL_RETURNED = 3
            MAIL_DELETED = 4
            MAIL_MADE_PERMANENT = 5
        End Enum

        Public Enum MailSentError
            NO_ERROR = 0
            BAG_FULL = 1
            CANNOT_SEND_TO_SELF = 2
            NOT_ENOUGHT_MONEY = 3
            CHARACTER_NOT_FOUND = 4
            NOT_YOUR_ALLIANCE = 5
            INTERNAL_ERROR = 6
        End Enum

        Public Enum MailReadInfo As Byte
            Unread = 0
            Read = 1
            Auction = 4
            COD = 8
        End Enum

        Public Enum MailTypeInfo As Byte
            NORMAL = 0
            GMMAIL = 1
            AUCTION = 2
        End Enum

    End Module
End NameSpace