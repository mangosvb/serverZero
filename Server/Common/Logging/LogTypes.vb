'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
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

Public Module LogTypes
    Public Enum LogType
        NETWORK                 'Network code debugging
        DEBUG                   'Packets processing
        INFORMATION             'User information
        USER                    'User actions
        SUCCESS                 'Normal operation
        WARNING                 'Warning
        FAILED                  'Processing Error
        CRITICAL                'Application Error
        DATABASE                'Database Error
    End Enum

    'Defs for Logging
    Public L() As Char = {"N", "D", "I", "U", "S", "W", "F", "C", "DB"}

    Public LogLevel As LogType = LogType.NETWORK

End Module

