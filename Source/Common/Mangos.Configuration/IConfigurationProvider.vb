Public Interface IConfigurationProvider(Of T)
    Function GetConfiguration() As T
End Interface