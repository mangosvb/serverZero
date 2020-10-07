Imports Autofac
Imports Mangos.Common.Globals

Public Module Program
    Sub Main()
        _RealmServer.Main()
    End Sub

    Function CreateContainer() As IContainer
        Dim builder As New ContainerBuilder
        builder.RegisterType(Of RealmServer).As(Of RealmServer)()
        builder.RegisterType(Of Converter).As(Of Converter)()
        builder.RegisterType(Of Global_Constants).As(Of Global_Constants)()
        builder.RegisterType(Of Functions).As(Of Functions)()
        Return builder.Build()
    End Function
End Module
