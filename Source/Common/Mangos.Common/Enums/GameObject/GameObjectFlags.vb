Namespace Enums.GameObject
    <Flags>
    Public Enum GameObjectFlags As Byte
        GO_FLAG_IN_USE = &H1                        'disables interaction while animated
        GO_FLAG_LOCKED = &H2                        'require key, spell, event, etc to be opened. Makes "Locked" appear in tooltip
        GO_FLAG_INTERACT_COND = &H4                 'cannot interact (condition to interact)
        GO_FLAG_TRANSPORT = &H8                     'any kind of transport? Object can transport (elevator, boat, car)
        GO_FLAG_UNK1 = &H10                         '
        GO_FLAG_NODESPAWN = &H20                    'never despawn, typically for doors, they just change state
        GO_FLAG_TRIGGERED = &H40                    'typically, summoned objects. Triggered by spell or other events
    End Enum
End NameSpace