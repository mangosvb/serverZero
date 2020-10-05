Namespace Enums.Warden
    Public Enum MaievResponse As Byte
        MAIEV_RESPONSE_FAILED_OR_MISSING = &H0          'The module was either currupt or not in the cache request transfer
        MAIEV_RESPONSE_SUCCESS = &H1                    'The module was in the cache and loaded successfully
        MAIEV_RESPONSE_RESULT = &H2
        MAIEV_RESPONSE_HASH = &H4
    End Enum
End NameSpace