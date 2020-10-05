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
    Public Module AuthenticationEnum

        Public Enum AuthCMD As Byte
            CMD_AUTH_LOGON_CHALLENGE = &H0
            CMD_AUTH_LOGON_PROOF = &H1
            CMD_AUTH_RECONNECT_CHALLENGE = &H2
            CMD_AUTH_RECONNECT_PROOF = &H3
            CMD_AUTH_REALMLIST = &H10
            CMD_XFER_INITIATE = &H30
            CMD_XFER_DATA = &H31
            CMD_XFER_ACCEPT = &H32
            CMD_XFER_RESUME = &H33
            CMD_XFER_CANCEL = &H34
        End Enum

        Public Enum AuthSrv As Byte
            CMD_GRUNT_CONN_PONG = &H11
            CMD_GRUNT_PROVESESSION = &H21
        End Enum

        Public Enum AuthResult As Byte
            WOW_SUCCESS = &H0
            WOW_FAIL_BANNED = &H3
            WOW_FAIL_UNKNOWN_ACCOUNT = &H4
            WOW_FAIL_INCORRECT_PASSWORD = &H5
            WOW_FAIL_ALREADY_ONLINE = &H6
            WOW_FAIL_NO_TIME = &H7
            WOW_FAIL_DB_BUSY = &H8
            WOW_FAIL_VERSION_INVALID = &H9
            WOW_FAIL_VERSION_UPDATE = &HA
            WOW_FAIL_INVALID_SERVER = &HB
            WOW_FAIL_SUSPENDED = &HC
            WOW_FAIL_FAIL_NOACCESS = &HD
            WOW_SUCCESS_SURVEY = &HE
            WOW_FAIL_PARENTCONTROL = &HF
            WOW_FAIL_LOCKED_ENFORCED = &H10
            WOW_FAIL_TRIAL_ENDED = &H11
            WOW_FAIL_ANTI_INDULGENCE = &H13
            WOW_FAIL_EXPIRED = &H14
            WOW_FAIL_NO_GAME_ACCOUNT = &H15
            WOW_FAIL_CHARGEBACK = &H16
            WOW_FAIL_GAME_ACCOUNT_LOCKED = &H18
            WOW_FAIL_UNLOCKABLE_LOCK = &H19
            WOW_FAIL_CONVERSION_REQUIRED = &H20
            WOW_FAIL_DISCONNECTED = &HFF
        End Enum

        Public Enum LoginResponse As Byte
            LOGIN_OK = &HC
            LOGIN_VERSION_MISMATCH = &H14
            LOGIN_UNKNOWN_ACCOUNT = &H15
            LOGIN_WAIT_QUEUE = &H1B
        End Enum

        Public Enum ATLoginFlags As Byte
            AT_LOGIN_NONE = &H0
            AT_LOGIN_RENAME = &H1
            AT_LOGIN_RESET_SPELLS = &H2
            AT_LOGIN_RESET_TALENTS = &H4
            AT_LOGIN_FIRST = &H20
        End Enum

        Public Enum LogoutResponseCode As Byte
            LOGOUT_RESPONSE_ACCEPTED = &H0
            LOGOUT_RESPONSE_DENIED = &HC
        End Enum

    End Module
End NameSpace