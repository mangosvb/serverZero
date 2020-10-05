Namespace Enums.Global
    Public Enum AccountState As Byte
        'RealmServ Error Codes
        LOGIN_OK = &H0
        LOGIN_FAILED = &H1          'Unable to connect
        LOGIN_BANNED = &H3          'This World of Warcraft account has been closed and is no longer in service -- Please check the registered email address of this account for further information.
        LOGIN_UNKNOWN_ACCOUNT = &H4 'The information you have entered is not valid.  Please check the spelling of the account name and password.  If you need help in retrieving a lost or stolen password and account, see www.worldofwarcraft.com for more information.
        LOGIN_BAD_PASS = &H5        'The information you have entered is not valid.  Please check the spelling of the account name and password.  If you need help in retrieving a lost or stolen password and account, see www.worldofwarcraft.com for more information.
        LOGIN_ALREADYONLINE = &H6   'This account is already logged into World of Warcraft.  Please check the spelling and try again.
        LOGIN_NOTIME = &H7          'You have used up your prepaid time for this account. Please purchase more to continue playing.
        LOGIN_DBBUSY = &H8          'Could not log in to World of Warcraft at this time.  Please try again later.
        LOGIN_BADVERSION = &H9      'Unable to validate game version.  This may be caused by file corruption or the interference of another program.  Please visit www.blizzard.com/support/wow/ for more information and possible solutions to this issue.
        LOGIN_DOWNLOADFILE = &HA
        LOGIN_SUSPENDED = &HC       'This World Of Warcraft account has been temporarily suspended. Please go to http://www.wow-europe.com/en/misc/banned.html for further information.
        LOGIN_PARENTALCONTROL = &HF 'Access to this account has been blocked by parental controls.  Your settings may be changed in your account preferences at http://www.worldofwarcraft.com.
    End Enum
End NameSpace