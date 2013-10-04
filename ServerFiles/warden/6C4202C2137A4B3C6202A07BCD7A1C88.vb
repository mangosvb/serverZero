' Warden module script.
'
'Module name: 6C4202C2137A4B3C6202A07BCD7A1C88

Imports System
Imports Microsoft.VisualBasic
Imports vWoW.WorldServer

Namespace Scripts
	Public Module Main

        Public MD5 As Byte() = New Byte() {&H6C, &H42, &H2, &HC2, &H13, &H7A, &H4B, &H3C, &H62, &H2, &HA0, &H7B, &HCD, &H7A, &H1C, &H88}
        Public RC4 As Byte() = New Byte() {&HCA, &HF8, &HD, &HC5, &H7B, &H8, &H9B, &H6, &H52, &H7F, &H88, &HA0, &HAD, &HA5, &HBD, &HFD}
        Public Size As Integer = 17292

        Public MEM_CHECK As Byte = &HC7
        Public PAGE_CHECK_A As Byte = &HE3 'This might be B and the one below might be A
        Public PAGE_CHECK_B As Byte = &H0
        Public MPQ_CHECK As Byte = &HC6
        Public LUA_STR_CHECK As Byte = &H54
        Public DRIVER_CHECK As Byte = &H0
        Public TIMING_CHECK As Byte = &H0
        Public PROC_CHECK As Byte = &H0
        Public MODULE_CHECK As Byte = &H0

        Public Sub Scan()

        End Sub

    End Module
End namespace