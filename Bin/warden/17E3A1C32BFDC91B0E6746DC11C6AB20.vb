' Warden module script.
'
'Module name: 17E3A1C32BFDC91B0E6746DC11C6AB20

Imports System
Imports Microsoft.VisualBasic
Imports vWoW.WorldServer

Namespace Scripts
    Public Module Main

        'http://paste2.org/p/474089

        Public MD5 As Byte() = New Byte() {&H17, &HE3, &HA1, &HC3, &H2B, &HFD, &HC9, &H1B, &HE, &H67, &H46, &HDC, &H11, &HC6, &HAB, &H20}
        Public RC4 As Byte() = New Byte() {&H75, &HA4, &HD9, &H7B, &H23, &H5A, &H69, &H97, &H0, &HCE, &H52, &H2B, &H40, &HAE, &H2D, &H0}
        Public Size As Integer = 18508

        Public MEM_CHECK As Byte = &H59
        Public PAGE_CHECK_A As Byte = &H3D 'This might be B and the one below might be A
        Public PAGE_CHECK_B As Byte = &H36
        Public MPQ_CHECK As Byte = &H78
        Public LUA_STR_CHECK As Byte = &H91
        Public DRIVER_CHECK As Byte = &H23
        Public TIMING_CHECK As Byte = &H75
        Public PROC_CHECK As Byte = &H8A
        Public MODULE_CHECK As Byte = &H0 'Unknown

        Public Sub Scan()

        End Sub

    End Module
End namespace