'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.CodeDom.Compiler
Imports System.IO
Imports System.Reflection
Imports Mangos.Common.Enums

Namespace Globals

'NOTE: How to use ScriptedObject as Function
'   Dim test As New ScriptedObject("scripts\test.vb", "test.dll")
'   test.Invoke(".TestScript", "TestMeSub")
'   x = test.Invoke(".TestScript", "TestMeFunction")
'NOTE: How to use ScriptedObject as Constructor
'   creature = test.Invoke("DefaultAI_1")
'   creature.Move()

    Public Class ScriptedObject
        Implements IDisposable

        Public ass As [Assembly]

        Public Sub New()
            Dim LastDate As Date
            Dim AssemblyFile As String = "Mangos.Scripts.dll"

            Dim AssemblySources As String() = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory() & "\Scripts\", "*.vb", SearchOption.AllDirectories)
            For Each Source As String In AssemblySources
                If LastDate < FileDateTime(Source) Then
                    LastDate = FileDateTime(Source)
                End If
            Next

            If (Dir(AssemblyFile) <> "") AndAlso (LastDate < FileDateTime(AssemblyFile)) Then
                'DONE: We have latest source compiled already
                LoadAssemblyObject(AssemblyFile)
                Return
            End If

            Log.WriteLine(GlobalEnum.LogType.SUCCESS, "Compiling: \Scripts\*.*")

            Try
                Dim vBcp As New VBCodeProvider
                'Dim CScp As New Microsoft.CSharp.CSharpCodeProvider

                Dim cParameters As New CompilerParameters
                Dim cResults As CompilerResults

                For Each include As String In Config.CompilerInclude
                    cParameters.ReferencedAssemblies.Add(include)
                Next
                cParameters.OutputAssembly = AssemblyFile
                cParameters.ReferencedAssemblies.Add(AppDomain.CurrentDomain.FriendlyName)
                cParameters.GenerateExecutable = False
                cParameters.GenerateInMemory = False
#If DEBUG Then
                cParameters.IncludeDebugInformation = True
#Else
            cParameters.IncludeDebugInformation = false
#End If

                cResults = vBcp.CompileAssemblyFromFile(cParameters, AssemblySources)

                If cResults.Errors.HasErrors = True Then
                    For Each err As CompilerError In cResults.Errors
                        Log.WriteLine(LogType.FAILED, "Compiling: Error on line {1} in {3}:{0}{2}", Environment.NewLine, err.Line, err.ErrorText, err.FileName)
                    Next
                Else
                    ass = cResults.CompiledAssembly
                End If
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Unable to compile scripts. {1}{0}", e.ToString, Environment.NewLine)
            End Try
        End Sub

        Public Sub New(ByVal AssemblySourceFile As String, ByVal AssemblyFile As String, ByVal InMemory As Boolean)
            If (Not InMemory) AndAlso (Dir(AssemblyFile) <> "") AndAlso (FileDateTime(AssemblySourceFile) < FileDateTime(AssemblyFile)) Then
                'DONE: We have latest source compiled already
                LoadAssemblyObject(AssemblyFile)
                'Dim ass As [Assembly] = [Assembly].LoadFrom("test.dll")
                Return
            End If

            Log.WriteLine(LogType.SUCCESS, "Compiling: {0}", AssemblySourceFile)

            Try
                Dim VBcp As New VBCodeProvider
                Dim CScp As New Microsoft.CSharp.CSharpCodeProvider

                Dim cParameters As New CompilerParameters
                Dim cResults As CompilerResults

                If Not InMemory Then cParameters.OutputAssembly = AssemblyFile
                For Each Include As String In Config.CompilerInclude
                    cParameters.ReferencedAssemblies.Add(Include)
                Next
                cParameters.ReferencedAssemblies.Add(AppDomain.CurrentDomain.FriendlyName)
                cParameters.GenerateExecutable = False     ' result is a .DLL
                cParameters.GenerateInMemory = InMemory
#If DEBUG Then
                cParameters.IncludeDebugInformation = True
#Else
            cParameters.IncludeDebugInformation = false
#End If

                If AssemblySourceFile.IndexOf(".cs") <> -1 Then
                    cResults = CScp.CompileAssemblyFromFile(cParameters, AppDomain.CurrentDomain.BaseDirectory() & AssemblySourceFile)
                ElseIf AssemblySourceFile.IndexOf(".vb") <> -1 Then
                    cResults = VBcp.CompileAssemblyFromFile(cParameters, AppDomain.CurrentDomain.BaseDirectory() & AssemblySourceFile)
                Else
                    Log.WriteLine(LogType.FAILED, "Compiling: Unsupported file type: {0}", AssemblySourceFile)
                    Return
                End If

                If cResults.Errors.HasErrors = True Then
                    For Each err As CodeDom.Compiler.CompilerError In cResults.Errors
                        Log.WriteLine(LogType.FAILED, "Compiling: Error on line {1}:{0}{2}", Environment.NewLine, err.Line, err.ErrorText)
                    Next
                Else
                    ass = cResults.CompiledAssembly
                End If
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Unable to compile script [{0}]. {2}{1}", AssemblySourceFile, e.ToString, Environment.NewLine)
            End Try
        End Sub
        Public Sub InvokeFunction(ByVal MyModule As String, ByVal MyMethod As String, Optional ByVal Parameters As Object = Nothing)
            Try
                Dim ty As Type = ass.GetType("Scripts." & MyModule)
                Dim mi As MethodInfo = ty.GetMethod(MyMethod)
                mi.Invoke(Nothing, Parameters)
            Catch e As TargetInvocationException
                Log.WriteLine(LogType.FAILED, "Script execution error:{1}{0}", e.GetBaseException.ToString, Environment.NewLine)
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Script Method [{0}] not found in [Scripts.{1}]!", MyMethod, MyModule)
            End Try
        End Sub
        Public Function InvokeConstructor(ByVal MyBaseClass As String, Optional ByVal Parameters As Object = Nothing) As Object
            Try
                Dim ty As Type = ass.GetType("Scripts." & MyBaseClass)
                Dim ci() As ConstructorInfo = ty.GetConstructors

                Return ci(0).Invoke(Parameters)
            Catch e As NullReferenceException
                Log.WriteLine(LogType.FAILED, "Scripted Class [{0}] not found in [Scripts]!", MyBaseClass)
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Script execution error:{1}{0}", e.GetBaseException.ToString, Environment.NewLine)
            End Try
            Return Nothing
        End Function
        Public Function InvokeProperty(ByVal MyModule As String, ByVal MyProperty As String) As Object
            Try
                Dim ty As Type = ass.GetType("Scripts." & MyModule)
                Dim pi As PropertyInfo = ty.GetProperty(MyProperty)

                Return pi.GetValue(Nothing, Nothing)
            Catch e As NullReferenceException
                Log.WriteLine(LogType.FAILED, "Scripted Property [{1}] not found in [Scripts.{1}]!", MyModule, MyProperty)
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Script execution error:{1}{0}", e.GetBaseException.ToString, Environment.NewLine)
            End Try
            Return Nothing
        End Function
        Public Function InvokeField(ByVal MyModule As String, ByVal MyField As String) As Object
            Try
                Dim ty As Type = ass.GetType("Scripts." & MyModule)
                Dim fi As FieldInfo = ty.GetField(MyField, BindingFlags.Public + BindingFlags.Static)

                Return fi.GetValue(Nothing)
            Catch e As NullReferenceException
                Log.WriteLine(LogType.FAILED, "Scripted Field [{1}] not found in [Scripts.{0}]!", MyModule, MyField)
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Script execution error:{1}{0}", e.GetBaseException.ToString, Environment.NewLine)
            End Try
            Return Nothing
        End Function
        Public Function ContainsMethod(ByVal MyModule As String, ByVal MyMethod As String) As Boolean
            Dim ty As Type = ass.GetType("Scripts." & MyModule)
            Dim mi As MethodInfo = ty.GetMethod(MyMethod)
            If mi Is Nothing Then Return False Else Return True
        End Function

        'Load an already compiled script.
        Public Sub LoadAssemblyObject(ByVal dllLocation As String)
            Try
                ass = [Assembly].LoadFrom(dllLocation)
            Catch fnfe As FileNotFoundException
                Log.WriteLine(LogType.FAILED, "DLL not found error:{1}{0}", fnfe.GetBaseException.ToString, Environment.NewLine)
            Catch ane As ArgumentNullException
                Log.WriteLine(LogType.FAILED, "DLL NULL error:{1}{0}", ane.GetBaseException.ToString, Environment.NewLine)
            Catch bife As BadImageFormatException
                Log.WriteLine(LogType.FAILED, "DLL not a valid assembly error:{1}{0}", bife.GetBaseException.ToString, Environment.NewLine)
            End Try
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            _disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End NameSpace