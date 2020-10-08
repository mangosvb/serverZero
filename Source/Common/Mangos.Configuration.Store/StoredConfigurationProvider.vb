Public Class StoredConfigurationProvider(Of T)
    Implements IConfigurationProvider(Of T)

    Private ReadOnly _configurationProvider As IConfigurationProvider(Of T)
    Private _configuration As T

    Sub New(configurationProvider As IConfigurationProvider(Of T))
        _configurationProvider = configurationProvider
    End Sub

    Public Async Function GetConfigurationAsync() As Task(Of T) Implements IConfigurationProvider(Of T).GetConfigurationAsync
        If (_configuration Is Nothing) Then
            _configuration = Await _configurationProvider.GetConfigurationAsync()
        End If
        Return _configuration
    End Function
End Class
