' Warden module script.
'
'Module name: 5F947B8AB100C93D206D1FC3EA1FE6DD

Imports System
Imports Microsoft.VisualBasic
Imports vWoW.WorldServer

Namespace Scripts
	Public Module Main

        'http://paste2.org/p/474078

        Public MD5 As Byte() = New Byte() {&H5F, &H94, &H7B, &H8A, &HB1, &H0, &HC9, &H3D, &H20, &H6D, &H1F, &HC3, &HEA, &H1F, &HE6, &HDD}
        Public RC4 As Byte() = New Byte() {&H6C, &H81, &H96, &HDF, &HAD, &HC6, &HAB, &H34, &HD9, &HD2, &HFF, &H1F, &H62, &HB, &H5F, &H47}
        Public Size As Integer = 17978

        Public MEM_CHECK As Byte = &H9B
        Public PAGE_CHECK_A As Byte = &H7 'This might be B and the one below might be A
        Public PAGE_CHECK_B As Byte = &HA2
        Public MPQ_CHECK As Byte = &HD8
        Public LUA_STR_CHECK As Byte = &H73
        Public DRIVER_CHECK As Byte = &HA9
        Public TIMING_CHECK As Byte = &HDF
        Public PROC_CHECK As Byte = &H0 'Unknown
        Public MODULE_CHECK As Byte = &H0 'Unknown

		Public Sub Scan()

		End Sub

	End Module
End namespace