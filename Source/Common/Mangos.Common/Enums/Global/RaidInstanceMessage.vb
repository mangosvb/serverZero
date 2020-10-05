Namespace Enums.Global
    Public Enum RaidInstanceMessage As UInteger
        RAID_INSTANCE_WARNING_HOURS = 1         ' WARNING! %s is scheduled to reset in %d hour(s).
        RAID_INSTANCE_WARNING_MIN = 2           ' WARNING! %s is scheduled to reset in %d minute(s)!
        RAID_INSTANCE_WARNING_MIN_SOON = 3      ' WARNING! %s is scheduled to reset in %d minute(s). Please exit the zone or you will be returned to your bind location!
        RAID_INSTANCE_WELCOME = 4               ' Welcome to %s. This raid instance is scheduled to reset in %s.
    End Enum
End NameSpace