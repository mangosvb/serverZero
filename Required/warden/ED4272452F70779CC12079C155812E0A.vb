' Warden module script.
'
'Module name: ED4272452F70779CC12079C155812E0A

Imports System
Imports Microsoft.VisualBasic
Imports vWoW.WorldServer

Namespace Scripts
	Public Module Main

        'http://paste2.org/p/472958

        Public MD5 As Byte() = New Byte() {&HED, &H42, &H72, &H45, &H2F, &H70, &H77, &H9C, &HC1, &H20, &H79, &HC1, &H55, &H81, &H2E, &HA}
        Public RC4 As Byte() = New Byte() {&HEC, &HA9, &HA9, &HD1, &HEA, &HAE, &HFD, &H38, &HCC, &H11, &H50, &H62, &HFB, &H92, &H99, &H6E}
        Public Size As Integer = 18119

        Public MEM_CHECK As Byte = &H87
        Public PAGE_CHECK_A As Byte = &HE3 'This might be B and the one below might be A
        Public PAGE_CHECK_B As Byte = &HFA
        Public MPQ_CHECK As Byte = &H48
        Public LUA_STR_CHECK As Byte = &HCF
        Public DRIVER_CHECK As Byte = &H5D 'Not sure about this one
        Public TIMING_CHECK As Byte = &H2B
        Public PROC_CHECK As Byte = &H0 ' Unknown atm
        Public MODULE_CHECK As Byte = &H0 ' Unknown atm

		Public Sub Scan()

		End Sub

	End Module
End namespace