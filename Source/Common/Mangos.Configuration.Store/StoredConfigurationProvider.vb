Public Class StoredConfigurationProvider(Of T)
    Implements IConfigurationProvider(Of T)

    Private ReadOnly _configurationProvider As IConfigurationProvider(Of T)
    Private _configuration As T

    Sub New(configurationProvider As IConfigurationProvider(Of T))
        _configurationProvider = configurationProvider
    End Sub

    Public Function GetConfiguration() As T Implements IConfigurationProvider(Of T).GetConfiguration
        If (_configuration Is Nothing) Then
            _configuration = _configurationProvider.GetConfiguration()
        End If
        Return _configuration
    End Function
End Class
