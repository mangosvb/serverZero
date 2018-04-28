'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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

Public Module WardenEnum

    Public Enum CheckTypes As Byte
        MEM_CHECK = 0
        PAGE_CHECK_A_B = 1
        MPQ_CHECK = 2
        LUA_STR_CHECK = 3
        DRIVER_CHECK = 4
        TIMING_CHECK = 5
        PROC_CHECK = 6
        MODULE_CHECK = 7
    End Enum

    Public Enum MaievResponse As Byte
        MAIEV_RESPONSE_FAILED_OR_MISSING = &H0          'The module was either currupt or not in the cache request transfer
        MAIEV_RESPONSE_SUCCESS = &H1                    'The module was in the cache and loaded successfully
        MAIEV_RESPONSE_RESULT = &H2
        MAIEV_RESPONSE_HASH = &H4
    End Enum

    Public Enum MaievOpcode As Byte
        MAIEV_MODULE_INFORMATION = 0
        MAIEV_MODULE_TRANSFER = 1
        MAIEV_MODULE_RUN = 2
        MAIEV_MODULE_UNK = 3
        MAIEV_MODULE_SEED = 5
    End Enum

End Module