'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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
Imports System.Runtime.InteropServices

Public Module NativeMethods
    Private Declare Function LoadLibrary Lib "kernel32.dll" Alias "LoadLibraryA" (ByVal lpLibFileName As String) As Integer

    Public Function LoadLibrary(ByVal lpLibFileName As String, ByVal dummy As String) As Integer
        Return LoadLibrary(lpLibFileName)
    End Function

    '    Private Declare Function GetProcAddress Lib "kernel32.dll" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer
    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True)>
    Private Function GetProcAddress(ByVal hModule As IntPtr, ByVal procName As String) As UIntPtr
    End Function

    Public Function GetProcAddress(ByVal hModule As IntPtr, ByVal lpProcName As String, ByVal dummy As String) As UIntPtr
        Return GetProcAddress(hModule, lpProcName)
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function CloseHandle(ByVal hObject As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    'Declare Function CloseHandle Lib "kernel32" Alias "CloseHandle" (ByVal hObject As Integer) As Integer

    Public Function CloseHandle(ByVal hObject As IntPtr, ByVal dummy As String) As Integer
        Return CloseHandle(hObject)
    End Function

    Private Declare Function VirtualProtect Lib "kernel32.dll" (ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal flNewProtect As Integer, ByRef lpflOldProtect As Integer) As Integer

    Public Function VirtualProtect(ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal flNewProtect As Integer, ByRef lpflOldProtect As Integer, ByVal dummy As String) As Integer
        Return VirtualProtect(lpAddress, dwSize, flNewProtect, lpflOldProtect)
    End Function

    Private Declare Function FlushInstructionCache Lib "kernel32.dll" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal dwSize As Integer) As Integer

    Public Function FlushInstructionCache(ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal dwSize As Integer, ByVal dummy As String) As Integer
        Return FlushInstructionCache(hProcess, lpBaseAddress, dwSize)
    End Function

    Private Declare Function GetCurrentProcess Lib "kernel32.dll" () As Integer

    Public Function GetCurrentProcess(ByVal dummy As String) As Integer
        Return GetCurrentProcess()
    End Function

    Private Declare Function VirtualFree Lib "kernel32.dll" (ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal dwFreeType As Integer) As Integer
    Public Function VirtualFree(ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal dwFreeType As Integer, ByVal dummy As String) As Integer
        Return VirtualFree(lpAddress, dwSize, dwFreeType)
    End Function

    Private Declare Function GlobalLock Lib "kernel32.dll" (ByVal hMem As Integer) As Integer
    Public Function GlobalLock(ByVal hMem As Integer, ByVal dummy As String) As Integer
        Return GlobalLock(hMem)
    End Function

    Private Declare Function GlobalUnlock Lib "kernel32.dll" (ByVal hMem As Integer) As Integer
    Public Function GlobalUnlock(ByVal hMem As Integer, ByVal dummy As String) As Integer
        Return GlobalUnlock(hMem)
    End Function
End Module