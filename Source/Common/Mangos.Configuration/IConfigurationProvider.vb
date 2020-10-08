
Public Interface IConfigurationProvider(Of T)
    Function GetConfigurationAsync() As Task(Of T)
End Interface
