Imports System.IO
Imports System.Xml.Serialization

Public Class XmlFileConfigurationProvider(Of T)
    Implements IConfigurationProvider(Of T)

    Private ReadOnly _filePath As String

    Sub New(filePath As String)
        _filePath = filePath
    End Sub

    Public Function GetConfigurationAsync() As Task(Of T) Implements IConfigurationProvider(Of T).GetConfigurationAsync
        Using streamReader As New StreamReader(_filePath)
            Dim xmlSerializer = New XmlSerializer(GetType(T))
            Dim configuration As T = xmlSerializer.Deserialize(streamReader)
            Return Task.FromResult(configuration)
        End Using
    End Function
End Class
