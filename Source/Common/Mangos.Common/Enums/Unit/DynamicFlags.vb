Namespace Enums.Unit
    Public Enum DynamicFlags   'Dynamic flags for units
        'Unit has blinking stars effect showing lootable
        UNIT_DYNFLAG_LOOTABLE = &H1
        'Shows marked unit as small red dot on radar
        UNIT_DYNFLAG_TRACK_UNIT = &H2
        'Gray mob title marks that mob is tagged by another player
        UNIT_DYNFLAG_OTHER_TAGGER = &H4
        'Blocks player character from moving
        UNIT_DYNFLAG_ROOTED = &H8
        'Shows infos like Damage and Health of the enemy
        UNIT_DYNFLAG_SPECIALINFO = &H10
        'Unit falls on the ground and shows like dead
        UNIT_DYNFLAG_DEAD = &H20
    End Enum
End NameSpace