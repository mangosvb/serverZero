'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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
    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function BN_add(r As IntPtr, a As IntPtr, b As IntPtr) As Integer
    End Function

    Public Function BN_add(r As IntPtr, a As IntPtr, b As IntPtr, dummy As String) As Integer
        Return BN_add(r, a, b)
    End Function

    Private Declare Function BN_sub Lib "LIBEAY32" (r As IntPtr, a As IntPtr, b As IntPtr) As Integer

    Public Function BN_sub(r As IntPtr, a As IntPtr, b As IntPtr, dummy As String) As Integer
        Return BN_sub(r, a, b)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function BN_bin2bn(byteArrayIn As Byte(), length As Integer, [to] As IntPtr) As IntPtr
    End Function

    Public Function BN_bin2bn(byteArrayIn As Byte(), length As Integer, [to] As IntPtr, dummy As String) As IntPtr
        Return BN_bin2bn(byteArrayIn, length, [to])
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function BN_bn2bin(a As IntPtr, [to] As Byte()) As Integer
    End Function

    Public Function BN_bn2bin(a As IntPtr, [to] As Byte(), dummy As String) As Integer
        Return BN_bn2bin(a, [to])
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function BN_CTX_free(a As IntPtr) As Integer
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_CTX_new() As IntPtr
    End Function

    Public Function BN_CTX_new(dummy As String) As Integer
        Return BN_CTX_new()
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function BN_mod(r As IntPtr, a As IntPtr, b As IntPtr, ctx As IntPtr) As Integer
    End Function

    Public Function BN_mod(r As IntPtr, a As IntPtr, b As IntPtr, ctx As IntPtr, dummy As String) As Integer
        Return BN_mod(r, a, b, ctx)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function BN_mod_exp(res As IntPtr, a As IntPtr, p As IntPtr, m As IntPtr, ctx As IntPtr) As IntPtr
    End Function

    Public Function BN_mod_exp(res As IntPtr, a As IntPtr, p As IntPtr, m As IntPtr, ctx As IntPtr, dummy As String) As IntPtr
        Return BN_mod_exp(res, a, p, m, ctx)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function BN_mul(r As IntPtr, a As IntPtr, b As IntPtr, ctx As IntPtr) As Integer
    End Function

    Public Function BN_mul(r As IntPtr, a As IntPtr, b As IntPtr, ctx As IntPtr, dummy As String) As Integer
        Return BN_mul(r, a, b, ctx)
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)> _
    Private Function BN_new() As IntPtr
    End Function

    Public Function BN_new(dummy As String) As IntPtr
        Return BN_new()
    End Function

    <DllImport("LIBEAY32.dll", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl)>
    Private Function RAND_bytes(buf As Byte(), num As Integer) As Integer
    End Function

    Public Function RAND_bytes(buf As Byte(), num As Integer, dummy As String) As Integer
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
    Private Declare Function TimeGetTime Lib "winmm.dll" () As Integer
    Private Declare Function TimeBeginPeriod Lib "winmm.dll" (uPeriod As Integer) As Integer

    Public Function TimeGetTime(dummy As String) As Integer
        Return TimeGetTime()
    End Function
    Public Function TimeBeginPeriod(uPeriod As Integer, dummy As String) As Integer
        Return TimeBeginPeriod(uPeriod)
    End Function
#End If

    Private Declare Function BN_div Lib "LIBEAY32" (dv As IntPtr, remainder As IntPtr, a As IntPtr, d As IntPtr, ctx As IntPtr) As Integer

    Public Function BN_div(dv As IntPtr, remainder As IntPtr, a As IntPtr, d As IntPtr, ctx As IntPtr, dummy As String) As Integer
        Return BN_div(dv, remainder, a, d, ctx)
    End Function

    Private Declare Function BN_sqr Lib "LIBEAY32" (r As IntPtr, a As IntPtr, ctx As IntPtr) As Integer

    Public Function BN_sqr(r As IntPtr, a As IntPtr, ctx As IntPtr, dummy As String) As Integer
        Return BN_sqr(r, a, ctx)
    End Function

    Private Declare Function BN_exp Lib "LIBEAY32" (r As IntPtr, a As IntPtr, p As IntPtr, ctx As IntPtr) As Integer

    Public Function BN_exp(r As IntPtr, a As IntPtr, p As IntPtr, ctx As IntPtr, dummy As String) As Integer
        Return BN_exp(r, a, p, ctx)
    End Function

    Private Declare Function BN_lshift Lib "LIBEAY32" (r As IntPtr, a As IntPtr, n As Integer) As Integer

    Public Function BN_lshift(r As IntPtr, a As IntPtr, n As Integer, dummy As String) As Integer
        Return BN_lshift(r, a, n)
    End Function

    Private Declare Function BN_rshift Lib "LIBEAY32" (r As IntPtr, a As IntPtr, n As Integer) As Integer

    Public Function BN_rshift(r As IntPtr, a As IntPtr, n As Integer, dummy As String) As Integer
        Return BN_rshift(r, a, n)
    End Function

    Private Declare Function BN_num_bits Lib "LIBEAY32" (a As IntPtr) As Integer

    Public Function BN_num_bits(a As IntPtr, dummy As String) As Integer
        Return BN_num_bits(a)
    End Function

    Private Declare Function LoadLibrary Lib "kernel32.dll" Alias "LoadLibraryA" (lpLibFileName As String) As Integer

    Public Function LoadLibrary(lpLibFileName As String, dummy As String) As Integer
        Return LoadLibrary(lpLibFileName)
    End Function

    '    Private Declare Function GetProcAddress Lib "kernel32.dll" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer
    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True)>
    Private Function GetProcAddress(hModule As IntPtr, procName As String) As UIntPtr
    End Function

    Public Function GetProcAddress(hModule As IntPtr, lpProcName As String, dummy As String) As UIntPtr
        Return GetProcAddress(hModule, lpProcName)
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function CloseHandle(hObject As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    'Declare Function CloseHandle Lib "kernel32" Alias "CloseHandle" (ByVal hObject As Integer) As Integer

    Public Function CloseHandle(hObject As IntPtr, dummy As String) As Integer
        Return CloseHandle(hObject)
    End Function

    Private Declare Function VirtualProtect Lib "kernel32.dll" (lpAddress As Integer, dwSize As Integer, flNewProtect As Integer, ByRef lpflOldProtect As Integer) As Integer

    Public Function VirtualProtect(lpAddress As Integer, dwSize As Integer, flNewProtect As Integer, ByRef lpflOldProtect As Integer, dummy As String) As Integer
        Return VirtualProtect(lpAddress, dwSize, flNewProtect, lpflOldProtect)
    End Function

    Private Declare Function FlushInstructionCache Lib "kernel32.dll" (hProcess As Integer, lpBaseAddress As Integer, dwSize As Integer) As Integer

    Public Function FlushInstructionCache(hProcess As Integer, lpBaseAddress As Integer, dwSize As Integer, dummy As String) As Integer
        Return FlushInstructionCache(hProcess, lpBaseAddress, dwSize)
    End Function

    Private Declare Function GetCurrentProcess Lib "kernel32.dll" () As Integer

    Public Function GetCurrentProcess(dummy As String) As Integer
        Return GetCurrentProcess()
    End Function

    Private Declare Function VirtualFree Lib "kernel32.dll" (lpAddress As Integer, dwSize As Integer, dwFreeType As Integer) As Integer
    Public Function VirtualFree(lpAddress As Integer, dwSize As Integer, dwFreeType As Integer, dummy As String) As Integer
        Return VirtualFree(lpAddress, dwSize, dwFreeType)
    End Function

    Private Declare Function GlobalLock Lib "kernel32.dll" (hMem As Integer) As Integer
    Public Function GlobalLock(hMem As Integer, dummy As String) As Integer
        Return GlobalLock(hMem)
    End Function

    Private Declare Function GlobalUnlock Lib "kernel32.dll" (hMem As Integer) As Integer
    Public Function GlobalUnlock(hMem As Integer, dummy As String) As Integer
        Return GlobalUnlock(hMem)
    End Function
End Module