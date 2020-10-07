Imports Mangos.Common.Globals

Public Module RealmServiceLocator
    Public Property _Global_Constants As New Global_Constants
    Public Property _RealmServer As New RealmServer
    Public Property _Converter As New Converter
    Public Property _CommonGlobalFunctions As Mangos.Common.Globals.Functions

    Sub New()
        _CommonGlobalFunctions = New Mangos.Common.Globals.Functions(_Global_Constants)
    End Sub
End Module
