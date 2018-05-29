Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Module StringExtensions

    <Extension()>
    Public Function FormatWith(ByVal inputString As String, ByVal sourceObject As Object) As String
        For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(sourceObject)
            inputString = inputString.Replace("{" & prop.Name & "}", (If(prop.GetValue(sourceObject), "(null)")).ToString())
        Next

        return inputString
    End Function
End Module
