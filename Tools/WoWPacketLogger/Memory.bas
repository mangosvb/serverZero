Attribute VB_Name = "Memory"
'
' Copyright (C) 2009 vWoW <http://www.vanilla-wow.com/>
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

Public Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hWnd As Long, lpdwProcessId As Long) As Long
Public Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long
Public Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As Long, ByVal lpBaseAddress As Any, lpBuffer As Any, ByVal nSize As Long, lpNumberOfBytesWritten As Long) As Long
Public Declare Function ReadProcessMemory Lib "kernel32" (ByVal hProcess As Long, ByVal lpBaseAddress As Any, ByRef lpBuffer As Any, ByVal nSize As Long, lpNumberOfBytesWritten As Long) As Long
Public Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long
Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal Classname As String, ByVal WindowName As String) As Long
Public Declare Function SetSecurityInfo Lib "advapi32.dll" (ByVal Handle As Long, ByVal SE_OBJECT_TYPE As Long, ByVal SECURITY_INFORMATION As Long, psidOwner As Long, psidGroup As Long, pDACL As Long, pSacl As Long) As Long
Public Declare Function GetSecurityInfo Lib "advapi32.dll" (ByVal Handle As Long, ByVal SE_OBJECT_TYPE As Long, ByVal SECURITY_INFORMATION As Long, psidOwner As Long, psidGroup As Long, pDACL As Long, pSacl As Long, PSECURITY_DESCRIPTOR As Long) As Long
Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal Length As Long)
Public Declare Function GetCurrentProcess Lib "kernel32" () As Long
Public Declare Function SetPriorityClass Lib "kernel32" (ByVal hProcess As Long, ByVal dwPriorityClass As Long) As Long
Public Declare Function timeGetTime Lib "winmm.dll" () As Long
Public Declare Function timeBeginPeriod Lib "winmm.dll" (ByVal uPeriod As Long) As Long

Public Const PROCESS_ALL_ACCESS As Long = &H1F0FFF
Public Const WRITE_DAC As Long = &H40000

Public Enum SE_OBJECT_TYPE
    SE_UNKNOWN_OBJECT_TYPE = 0
    SE_FILE_OBJECT
    SE_SERVICE
    SE_PRINTER
    SE_REGISTRY_KEY
    SE_LMSHARE
    SE_KERNEL_OBJECT
    SE_WINDOW_OBJECT
    SE_DS_OBJECT
    SE_DS_OBJECT_ALL
    SE_PROVIDER_DEFINED_OBJECT
    SE_WMIGUID_OBJECT
    SE_REGISTRY_WOW64_32
End Enum
Public Enum SECURITY_INFORMATION
    OWNER_SECURITY_INFORMATION = 1
    GROUP_SECURITY_INFORMATION = 2
    DACL_SECURITY_INFORMATION = 4
    SACL_SECURITY_INFORMATION = 8
    PROTECTED_SACL_SECURITY_INFORMATION = 16
    PROTECTED_DACL_SECURITY_INFORMATION = 32
    UNPROTECTED_SACL_SECURITY_INFORMATION = 64
    UNPROTECTED_DACL_SECURITY_INFORMATION = 128
End Enum
Public Type ACL
    AclRevision As Byte
    Sbz1 As Byte
    AclSize As Integer
    AceCount As Integer
    Sbz2 As Integer
End Type
Public Type SECURITY_DESCRIPTOR
    Revision As Byte
    Sbz1 As Byte
    Control As Long
    Owner As Long
    Group As Long
    Sacl As ACL
    Dacl As ACL
End Type

Public lngHWnd As Long, lngPHandle As Long
Public SS_Hash() As Byte, ServerKey(0 To 3) As Byte, ClientKey(0 To 3) As Byte
Public SSHashPointer As Long

Public Function OpenSecureProcess(WindowName As String, ByVal urWindowHandle, ByVal dwDesiredAccess As Long) As Long
    Dim hWnd As Long, pHwnd As Long ' restricted proc handle, unrestricted proc handle
    Dim ProcessID As Long, pProcessID As Long ' process IDs for unr and res processes
    Dim ProcessHandle As Long, processhandle2 As Long ' processhandles "
    Dim pOldACL As Long, pOldSecDesc As Long ' pointers to the security structs
    hWnd = FindWindow(vbNullString, WindowName) ' get window handle for w/e process
    pHwnd = urWindowHandle ' handle of program that you have unrestricted rights to (: "form1.hwnd" :)
    If hWnd = 0 Then ' window text wasn't found, exit function
        OpenSecureProcess = 0 ' set function = 0 since no proc was opened
        Exit Function ' exit function
    End If
    ' process window was found---
    GetWindowThreadProcessId hWnd, ProcessID ' get process id for restricted program
    GetWindowThreadProcessId pHwnd, pProcessID ' get process id for unrestricted "
    ProcessHandle = OpenProcess(dwDesiredAccess, False, ProcessID) ' attempt opening the process normally without modifying security permissions
    If ProcessHandle <> 0 Then ' if process was opened, then
        OpenSecureProcess = ProcessHandle ' return the opened process handle
        Exit Function ' exit function..
    End If
    ' if process wasn't opened normally then
    '     ---
    processhandle2 = OpenProcess(dwDesiredAccess, False, pProcessID) ' open unrestricted process
    If GetSecurityInfo(processhandle2, SE_KERNEL_OBJECT, DACL_SECURITY_INFORMATION, 0, 0, pOldACL, 0, pOldSecDesc) = 0 Then
        ' get security info for unrestricted pro
        '     cess: this reads the pointer to the ACL
        '     structure into pOldDACL and pointer to t
        '     he security descriptor into pOldSecDesc
        ProcessHandle = OpenProcess(WRITE_DAC, False, ProcessID)
        ' open process for writing the ACL, this
        '     shouldn't fail.
        If ProcessHandle = 0 Then ' if it fails,
            OpenSecureProcess = 0 ' return 0 and exit (no proc opened)
        Else ' if proc was opened for writing ACL then...
            If SetSecurityInfo(ProcessHandle, SE_KERNEL_OBJECT, DACL_SECURITY_INFORMATION Or UNPROTECTED_DACL_SECURITY_INFORMATION, 0, 0, ByVal pOldACL, 0) = 0 Then
            ' this sets the security descriptor poin
            '     ter and the ACL pointer to the unrestric
            '     ted process
            ' (this is the part that copies the unre
            '     stricted permissions to the restricted p
            '     rocess)
            CloseHandle ProcessHandle ' if the permissions were set, then close the process
            ProcessHandle = OpenProcess(dwDesiredAccess, False, ProcessID) ' and reopen the process like we wanted to in the beginning
            OpenSecureProcess = ProcessHandle ' finally return the process handle of the unrestricted process
            Else ' if proc wasn't opened for writing ACL then
                OpenSecureProcess = 0 ' return 0, no proc opened - exit function
            End If
        End If
    Else ' if getting security info from unrestricted process doesn't work then..
        OpenSecureProcess = 0 ' return 0, no proc opened
        Exit Function ' exit function
    End If
End Function

Public Function Reverse(strString As String) As String
    Dim i As Integer
    Dim output As String
    output = ""
    For i = Len(strString) - 1 To 1 Step -2
        output = output + Mid(strString, i, 2)
    Next i
    Reverse = output
End Function

Public Function StringToHex(sString As String) As String
    Dim i As Integer, tmpHex As String
    tmpHex = ""
    For i = 1 To Len(sString)
        tmpHex = tmpHex & FormatHex(hex(Asc(Mid(sString, i, 1))), 2)
    Next i
    StringToHex = tmpHex
End Function

Public Function BytesToHex(Bytes() As Byte) As String
    Dim i As Integer, tmpHex As String
    tmpHex = ""
    For i = 0 To UBound(Bytes)
        tmpHex = tmpHex & FormatHex(hex(Bytes(i)), 2)
    Next i
    BytesToHex = tmpHex
End Function

Public Function HexToBytes(sHex As String) As Byte()
    Dim i As Integer, tmpBytes() As Byte
    ReDim tmpBytes(Int(Len(sHex) / 2) - 1)
    For i = 1 To Len(sHex) Step 2
        tmpBytes(Int(i / 2)) = Val("&H" & Mid(sHex, i, 2))
    Next i
    HexToBytes = tmpBytes
End Function

Public Function FormatHex(ByVal sHex As String, ByVal iLength As Integer) As String
    Dim i As Integer, tmpHex As String
    tmpHex = sHex
    If Len(sHex) < iLength Then
        For i = Len(sHex) + 1 To iLength
            tmpHex = "0" & tmpHex
        Next i
    End If
    FormatHex = tmpHex
End Function

Public Function GetMemByte(ByVal iAdr As Long) As Byte
On Error Resume Next
    Static bVal As Byte
    If ReadProcessMemory(lngPHandle, iAdr, bVal, 1&, 0&) <> 0 Then
        GetMemByte = bVal
    Else
        GetMemByte = 0
    End If
End Function

Public Function GetMemLong(ByVal iAdr As Long) As Long
On Error Resume Next
    Static lVal As Long
    If ReadProcessMemory(lngPHandle, iAdr, lVal, 4&, 0&) <> 0 Then
        GetMemLong = lVal
    Else
        GetMemLong = 0
    End If
End Function

Public Function GetMemInt(ByVal iAdr As Long) As Integer
On Error Resume Next
    Static iVal As Integer
    If ReadProcessMemory(lngPHandle, iAdr, iVal, 2&, 0&) <> 0 Then
        GetMemInt = iVal
    Else
        GetMemInt = 0
    End If
End Function

Public Function GetMemFloat(ByVal iAdr As Long) As Single
On Error Resume Next
    Static fVal As Single
    If ReadProcessMemory(lngPHandle, iAdr, fVal, 4&, 0&) <> 0 Then
        GetMemFloat = fVal
    Else
        GetMemFloat = 0
    End If
End Function

Public Function GetMemString(ByVal iAdr As Long, ByVal iLenght As Integer) As String
On Error Resume Next
    Dim sVal As String
    Dim iByteCtr As Long
    Dim bChunk As Byte
    For iByteCtr = 0 To iLenght - 1
        ReadProcessMemory lngPHandle, iAdr + iByteCtr, bChunk, 1&, 1&
        sVal = sVal & Chr(bChunk)
    Next iByteCtr
    GetMemString = sVal
End Function
