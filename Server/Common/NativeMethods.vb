Imports System.Runtime.InteropServices

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
Public Module NativeMethods
    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_add(ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr) As Integer
    End Function

    Public Function BN_add(ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal dummy As String) As Integer
        Return BN_add(r, a, b)
    End Function

    Private Declare Function BN_sub Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr) As Integer

    Public Function BN_sub(ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal dummy As String) As Integer
        Return BN_sub(r, a, b)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_bin2bn(ByVal ByteArrayIn As Byte(), ByVal length As Integer, ByVal [to] As IntPtr) As IntPtr
    End Function

    Public Function BN_bin2bn(ByVal ByteArrayIn As Byte(), ByVal length As Integer, ByVal [to] As IntPtr, ByVal dummy As String) As IntPtr
        Return BN_bin2bn(ByteArrayIn, length, [to])
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_bn2bin(ByVal a As IntPtr, ByVal [to] As Byte()) As Integer
    End Function

    Public Function BN_bn2bin(ByVal a As IntPtr, ByVal [to] As Byte(), ByVal dummy As String) As Integer
        Return BN_bn2bin(a, [to])
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_CTX_free(ByVal a As IntPtr) As Integer
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_CTX_new() As IntPtr
    End Function

    Public Function BN_CTX_new(ByVal dummy As String) As Integer
        Return BN_CTX_new()
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_mod(ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr) As Integer
    End Function

    Public Function BN_mod(ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr, ByVal dummy As String) As Integer
        Return BN_mod(r, a, b, ctx)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_mod_exp(ByVal res As IntPtr, ByVal a As IntPtr, ByVal p As IntPtr, ByVal m As IntPtr, ByVal ctx As IntPtr) As IntPtr
    End Function

    Public Function BN_mod_exp(ByVal res As IntPtr, ByVal a As IntPtr, ByVal p As IntPtr, ByVal m As IntPtr, ByVal ctx As IntPtr, ByVal dummy As String) As IntPtr
        Return BN_mod_exp(res, a, p, m, ctx)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_mul(ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr) As Integer
    End Function

    Public Function BN_mul(ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr, ByVal dummy As String) As Integer
        Return BN_mul(r, a, b, ctx)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_new() As IntPtr
    End Function

    Public Function BN_new(ByVal dummy As String) As IntPtr
        Return BN_new()
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function RAND_bytes(ByVal buf As Byte(), ByVal num As Integer) As Integer
    End Function

    Public Function RAND_bytes(ByVal buf As Byte(), ByVal num As Integer, ByVal dummy As String) As Integer
        Return RAND_bytes(buf, num)
    End Function

#If LINUX Then
    Public Function timeGetTime("")() As Integer
        Return System.Environment.TickCount()
    End Function
    Public Function timeBeginPeriod(ByVal uPeriod As Integer) As Integer
        Return 0
    End Function
#Else
    Private Declare Function timeGetTime Lib "winmm.dll" () As Integer
    Private Declare Function timeBeginPeriod Lib "winmm.dll" (ByVal uPeriod As Integer) As Integer

    Public Function timeGetTime(ByVal dummy As String) As Integer
        Return timeGetTime()
    End Function
    Public Function timeBeginPeriod(ByVal uPeriod As Integer, ByVal dummy As String) As Integer
        Return timeBeginPeriod(uPeriod)
    End Function
#End If

    Private Declare Function BN_div Lib "LIBEAY32" (ByVal dv As IntPtr, ByVal remainder As IntPtr, ByVal a As IntPtr, ByVal d As IntPtr, ByVal ctx As IntPtr) As Integer

    Public Function BN_div(ByVal dv As IntPtr, ByVal remainder As IntPtr, ByVal a As IntPtr, ByVal d As IntPtr, ByVal ctx As IntPtr, ByVal dummy As String) As Integer
        Return BN_div(dv, remainder, a, d, ctx)
    End Function

    Private Declare Function BN_sqr Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal ctx As IntPtr) As Integer

    Public Function BN_sqr(ByVal r As IntPtr, ByVal a As IntPtr, ByVal ctx As IntPtr, ByVal dummy As String) As Integer
        Return BN_sqr(r, a, ctx)
    End Function

    Private Declare Function BN_exp Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal p As IntPtr, ByVal ctx As IntPtr) As Integer

    Public Function BN_exp(ByVal r As IntPtr, ByVal a As IntPtr, ByVal p As IntPtr, ByVal ctx As IntPtr, ByVal dummy As String) As Integer
        Return BN_exp(r, a, p, ctx)
    End Function

    Private Declare Function BN_lshift Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal n As Integer) As Integer

    Public Function BN_lshift(ByVal r As IntPtr, ByVal a As IntPtr, ByVal n As Integer, ByVal dummy As String) As Integer
        Return BN_lshift(r, a, n)
    End Function

    Private Declare Function BN_rshift Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal n As Integer) As Integer

    Public Function BN_rshift(ByVal r As IntPtr, ByVal a As IntPtr, ByVal n As Integer, ByVal dummy As String) As Integer
        Return BN_rshift(r, a, n)
    End Function

    Private Declare Function BN_num_bits Lib "LIBEAY32" (ByVal a As IntPtr) As Integer

    Public Function BN_num_bits(ByVal a As IntPtr, ByVal dummy As String) As Integer
        Return BN_num_bits(a)
    End Function

    Private Declare Function LoadLibrary Lib "kernel32.dll" Alias "LoadLibraryA" (ByVal lpLibFileName As String) As Integer

    Public Function LoadLibrary(ByVal lpLibFileName As String, ByVal dummy As String) As Integer
        Return LoadLibrary(lpLibFileName)
    End Function

    '    Private Declare Function GetProcAddress Lib "kernel32.dll" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer
    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True)> _
    Private Function GetProcAddress(ByVal hModule As IntPtr, ByVal procName As String) As UIntPtr
    End Function

    Public Function GetProcAddress(ByVal hModule As IntPtr, ByVal lpProcName As String, ByVal dummy As String) As UIntPtr
        Return GetProcAddress(hModule, lpProcName)
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)> _
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