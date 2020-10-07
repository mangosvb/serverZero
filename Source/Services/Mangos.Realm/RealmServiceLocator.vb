Imports Autofac
Imports Mangos.Common.Globals

Public Module RealmServiceLocator
    Public Property _Container As IContainer = CreateContainer()
    Public Property _Global_Constants As Global_Constants = _Container.Resolve(Of Global_Constants)
    Public Property _RealmServer As RealmServer = _Container.Resolve(Of RealmServer)
    Public Property _Converter As Converter = _Container.Resolve(Of Converter)
    Public Property _CommonGlobalFunctions As Functions = _Container.Resolve(Of Functions)
End Module
