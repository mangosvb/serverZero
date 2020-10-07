Namespace Enums.Authentication
    Public Enum LoginResponse As Byte
        LOGIN_OK = &HC
        LOGIN_VERSION_MISMATCH = &H14
        LOGIN_UNKNOWN_ACCOUNT = &H15
        LOGIN_WAIT_QUEUE = &H1B
    End Enum
End NameSpace