'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.IO

#If VMAPS Then

Public Module VMAP_Module

    Public Class ModelContainer
        Inherits BaseModel
        Implements BaseCollision
        Implements IDisposable

        Private iBox As AABox
        Private numSubModels As Integer
        Private subModels As List(Of BaseCollision)

        Public Sub New()

        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                iTreeNodes.Clear()
                iTriangles.Clear()
                subModels.Clear()
                GC.Collect()
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

        Public ReadOnly Property Bounds() As AABox Implements BaseCollision.Bounds
            Get
                Return iBox
            End Get
        End Property

        Public Function ReadFile(ByVal fileName As String) As Boolean
            Dim f As FileStream = Nothing
            Dim b As BinaryReader = Nothing

            Dim fileVersion As String = ""
            Dim flags As UInteger
            Dim size As UInteger
            Dim ident(7) As Byte
            Dim chunk(3) As Byte

#If VMAPS_DEBUG Then
            Dim sw As New StreamWriter(fileName & "_debug.txt", False, System.Text.Encoding.UTF8)
#End If

            Try
                f = New FileStream("vmaps\" & fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 82704, FileOptions.SequentialScan)
                b = New BinaryReader(f)

                Dim result As Boolean = True
                fileVersion = Text.Encoding.ASCII.GetString(b.ReadBytes(8), 0, 8)
                Log.WriteLine(LogType.INFORMATION, "Loading map file [{0}] version [{1}]", fileName, fileVersion)

                If fileVersion <> VMAP_MAGIC Then Throw New FileLoadException()
                b.Read(ident, 0, 8)
                flags = b.ReadUInt32()
                'POS
                b.Read(chunk, 0, 4)
                Log.WriteLine(LogType.DEBUG, "POS: {0}", Text.Encoding.ASCII.GetString(chunk))
                size = b.ReadUInt32()
                iBasePosition = New Vector3(b.ReadSingle(), b.ReadSingle(), b.ReadSingle())
                Log.WriteLine(LogType.DEBUG, "Position: {0}", iBasePosition)

#If VMAPS_DEBUG Then
                sw.WriteLine(fileVersion)
                sw.WriteLine()
                sw.WriteLine("Positon: {0}", iBasePosition)
                sw.WriteLine()
#End If

                '---- Box
                b.Read(chunk, 0, 4)
                Log.WriteLine(LogType.DEBUG, "BOX: {0}", Text.Encoding.ASCII.GetString(chunk))
                size = b.ReadUInt32()
                Dim low As New Vector3(b.ReadSingle(), b.ReadSingle(), b.ReadSingle())
                Dim high As New Vector3(b.ReadSingle(), b.ReadSingle(), b.ReadSingle())
                iBox = New AABox(low, high)
                Log.WriteLine(LogType.DEBUG, "Box: {0}", iBox)

#If VMAPS_DEBUG Then
                sw.WriteLine("Bounds: {0}", iBox)
                sw.WriteLine()
#End If

                '---- TreeNodes
                b.Read(chunk, 0, 4)
                Log.WriteLine(LogType.DEBUG, "NODE: {0}", Text.Encoding.ASCII.GetString(chunk))
                size = b.ReadUInt32()

                numNodes = b.ReadInt32()
#If VMAPS_DEBUG Then
                sw.WriteLine("TreeNodes: {0}", numNodes)
                sw.WriteLine("===================")
#End If
                Log.WriteLine(LogType.DEBUG, "NumNodes: {0}", numNodes)
                iTreeNodes = New List(Of TreeNode)(numNodes)
                For i As Integer = 0 To numNodes - 1
                    iTreeNodes.Add(New TreeNode(i, b.ReadSingle(), b.ReadInt32(), b.ReadInt32(), b.ReadInt32(), b.ReadInt32(),
                                            New AABox(New Vector3(b.ReadSingle(), b.ReadSingle(), b.ReadSingle()), New Vector3(b.ReadSingle(), b.ReadSingle(), b.ReadSingle())),
                                            b.ReadUInt16(), b.ReadUInt16()))

#If VMAPS_DEBUG Then
                    sw.WriteLine(iTreeNodes(i).ToString(i))
#End If
                Next
#If VMAPS_DEBUG Then
                sw.WriteLine("===================")
                sw.WriteLine()
#End If

                '---- TriangleBoxes
                b.Read(chunk, 0, 4)
                Log.WriteLine(LogType.DEBUG, "TRIB: {0}", Text.Encoding.ASCII.GetString(chunk))
                size = b.ReadUInt32()

                numTriangles = b.ReadInt32()
#If VMAPS_DEBUG Then
                sw.WriteLine("TriangleBoxes: {0}", numTriangles)
                sw.WriteLine("===================")
#End If
                Log.WriteLine(LogType.DEBUG, "NumTriangles: {0}", numTriangles)
                iTriangles = New List(Of BaseCollision)(numTriangles)
                For i As Integer = 0 To numTriangles - 1
                    iTriangles.Add(New TriangleBox(New ShortVector(b.ReadInt16(), b.ReadInt16(), b.ReadInt16()),
                                                   New ShortVector(b.ReadInt16(), b.ReadInt16(), b.ReadInt16()),
                                                   New ShortVector(b.ReadInt16(), b.ReadInt16(), b.ReadInt16())))
#If VMAPS_DEBUG Then
                    sw.WriteLine(CType(iTriangles(i), TriangleBox).ToString(i))
#End If
                Next
#If VMAPS_DEBUG Then
                sw.WriteLine("===================")
                sw.WriteLine()
#End If

                '---- SubModel
                b.Read(chunk, 0, 4)
                Log.WriteLine(LogType.DEBUG, "SUBM: {0}", Text.Encoding.ASCII.GetString(chunk))
                size = b.ReadUInt32()

                numSubModels = b.ReadInt32()
#If VMAPS_DEBUG Then
                sw.WriteLine("SubModels: {0}", numSubModels)
                sw.WriteLine("===================")
#End If
                Log.WriteLine(LogType.DEBUG, "NumSubModels: {0}", numSubModels)
                subModels = New List(Of BaseCollision)(numSubModels)
                For i As Integer = 0 To numSubModels - 1
                    Dim newSubModel As New SubModel()
                    subModels.Add(newSubModel)
                    newSubModel.InitSubModel(b)
                    newSubModel.SetNodes(iTreeNodes)
                    newSubModel.SetTriangles(iTriangles)
#If VMAPS_DEBUG Then
                    sw.WriteLine(newSubModel.ToString(i))
#End If
                Next
#If VMAPS_DEBUG Then
                sw.WriteLine("===================")
                sw.WriteLine()

                sw.Flush()
                sw.Close()
                sw.Dispose()
#End If

                Return True

            Catch ex As FileLoadException
                Log.WriteLine(LogType.CRITICAL, "Error loading map file [{0}]. Wrong file version [{1}].", fileName, fileVersion)
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "Error loading map file [{0}].{1}{2}", fileName, vbNewLine, ex.ToString())
            End Try

            If f IsNot Nothing Then
                b.Close()
                f.Close()
                'f.Dispose()
            End If
            Return False
        End Function

        Public Overloads Sub Intersect(ByVal pRay As Ray, ByRef pMaxDist As Single, ByVal pStopAtFirstHit As Boolean) Implements BaseCollision.Intersect
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "Checking model container hit!")
#End If
            iTreeNodes(0).IntersectRay(pRay, pMaxDist, iTreeNodes, 0, subModels, 0, pStopAtFirstHit, False)
        End Sub

        Public Overloads Function Intersect(ByVal pRay As Ray, ByRef pMaxDist As Single) As Boolean
            Return MyBase.Intersect(iBox, pRay, pMaxDist)
        End Function

    End Class

    Public Interface BaseCollision
        ReadOnly Property Bounds() As AABox
        Sub Intersect(ByVal pRay As Ray, ByRef pMaxDist As Single, ByVal pStopAtFirstHit As Boolean)
    End Interface

    Public Class AABSPTree(Of T)
        Implements IDisposable

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

        Private Shared Function ComputeBounds(ByRef point As List(Of BaseCollision), ByVal beginIndex As Integer, ByVal endIndex As Integer) As AABox
            Dim lo As Vector3 = Vector3.MaxInf()
            Dim hi As Vector3 = Vector3.MinInf()

            For p As Integer = beginIndex To endIndex
                lo = lo.Min(point(p).Bounds.Low)
                hi = hi.Max(point(p).Bounds.High)
            Next

            Return New AABox(lo, hi)
        End Function

        Private Shared Sub Partition(ByRef source As List(Of BaseCollision), ByRef partitionElement As BaseCollision, ByRef ltArray As List(Of BaseCollision), ByRef eqArray As List(Of BaseCollision), ByRef gtArray As List(Of BaseCollision), ByRef comparator As IComparer(Of BaseCollision))
            'Clear the arrays
            ltArray.Clear()
            eqArray.Clear()
            gtArray.Clear()

            'Form a table of buckets for lt, eq, and gt
            Dim bucket() As List(Of BaseCollision) = {ltArray, eqArray, gtArray}

            For i As Integer = 0 To source.Count - 1
                Dim c As Integer = comparator.Compare(partitionElement, source(i))

                'Insert into the correct bucket, 0, 1, or 2
                bucket(c + 1).Add(source(i))
            Next
        End Sub

        Private Shared Sub MedianPartition(ByRef src As List(Of BaseCollision), ByRef ltMedian As List(Of BaseCollision), ByRef eqMedian As List(Of BaseCollision), ByRef gtMedian As List(Of BaseCollision), ByRef tempArray As List(Of BaseCollision), ByRef comparator As IComparer(Of BaseCollision))
            'Clear the arrays
            ltMedian.Clear()
            eqMedian.Clear()
            gtMedian.Clear()

            'Handle trivial cases first
            Select Case src.Count
                Case 0
                    Exit Sub 'Array is empty; no parition is possible
                Case 1
                    'One element
                    eqMedian.Add(src(0))
                    Exit Sub
                Case 2
                    'Two element array; median is the smaller
                    Dim objCharacter As Integer = comparator.Compare(src(0), src(1))
                    Select Case objCharacter
                        Case -1
                            'first was bigger
                            eqMedian.Add(src(1))
                            gtMedian.Add(src(0))
                        Case 0
                            'Both equal to the median
                            eqMedian.Add(src(0))
                            eqMedian.Add(src(1))
                        Case 1
                            'Last was bigger
                            eqMedian.Add(src(0))
                            gtMedian.Add(src(1))
                    End Select
                    Exit Sub
            End Select

            'All other cases use a recursive randomized median

            'Number of values less than all in the current arrays
            Dim ltBoost As Integer = 0

            'Number of values greater than all in the current arrays
            Dim gtBoost As Integer = 0

            'For even length arrays, force the gt array to be one larger than the
            'lt array:
            ' [1 2 3] size = 3, choose half = (s + 1) /2
            Dim lowerHalfSize As Integer, upperHalfSize As Integer
            If (src.Count And &H1) = 0 Then 'If even
                lowerHalfSize = src.Count \ 2
                upperHalfSize = lowerHalfSize + 1
            Else 'If odd
                lowerHalfSize = upperHalfSize = (src.Count + 1) \ 2
            End If
            Dim xPtr As BaseCollision = Nothing

            'Maintain pointers to the arrays; we'll switch these around during sorting
            'to avoid copies.
            Dim source As List(Of BaseCollision) = src
            Dim lt As List(Of BaseCollision) = ltMedian
            Dim eq As List(Of BaseCollision) = eqMedian
            Dim gt As List(Of BaseCollision) = gtMedian
            Dim extra As List(Of BaseCollision) = tempArray

            While True
                'Choose a random element -- choose the middle element; this is theoretically
                'suboptimal, but for loosly sorted array is actually the best strategy

                xPtr = source(source.Count >> 1)
                If source.Count = 1 Then
                    'Done; there's only one element left
                    Exit While
                End If

                'Note: partition (fast) clears the arrays for us
                Partition(source, xPtr, lt, eq, gt, comparator)

                Dim L As Integer = lt.Count + ltBoost + eq.Count
                Dim U As Integer = gt.Count + gtBoost + eq.Count
                If L >= lowerHalfSize AndAlso U >= upperHalfSize Then
                    'x must be the partition median
                    Exit While
                ElseIf L < lowerHalfSize Then
                    'x must be smaller than the median.  Recurse into the 'gt' array.
                    ltBoost += lt.Count + eq.Count

                    'The new gt array will be the old source array, unless
                    'that was the this pointer (i.e., unless we are on the
                    'first iteration)
                    Dim newGt As List(Of BaseCollision) = If((src Is source), extra, New List(Of BaseCollision)(source))

                    'Now set up the gt array as the new source
                    source = gt
                    gt = newGt
                Else
                    'x must be bigger than the median.  Recurse into the 'lt' array.
                    gtBoost += gt.Count + eq.Count

                    'The new lt array will be the old source array, unless
                    'that was the this pointer (i.e., unless we are on the
                    'first iteration)
                    Dim newLt As List(Of BaseCollision) = If((src Is source), extra, New List(Of BaseCollision)(source))

                    'Now set up the lt array as the new source
                    source = lt
                    lt = newLt
                End If
            End While

            'Now that we know the median, make a copy of it (since we're about to destroy the array that it
            'points into).

            'Partition the original array (note that this fast clears for us)
            Partition(src, xPtr, ltMedian, eqMedian, gtMedian, comparator)
        End Sub

        Private Class CenterComparator
            Implements IComparer(Of BaseCollision)

            Private sortAxis As Axis

            Public Sub New(ByVal sortAxis As Axis)
                Me.sortAxis = sortAxis
            End Sub

            Public Function Compare(ByVal x As BaseCollision, ByVal y As BaseCollision) As Integer Implements IComparer(Of BaseCollision).Compare
                Dim a As Single = x.Bounds.Center(sortAxis)
                Dim b As Single = y.Bounds.Center(sortAxis)

                If a < b Then
                    Return 1
                ElseIf a > b Then
                    Return -1
                Else
                    Return 0
                End If
            End Function

        End Class

        Private Class Comparator
            Implements IComparer(Of BaseCollision)

            Private sortAxis As Axis
            Private sortLocation As Single

            Public Sub New(ByVal a As Axis, ByVal l As Single)
                sortAxis = a
                sortLocation = l
            End Sub

            Public Function Compare(ByVal x As BaseCollision, ByVal y As BaseCollision) As Integer Implements IComparer(Of BaseCollision).Compare
                Dim box As AABox = y.Bounds

                If box.High(sortAxis) < sortLocation Then
                    'Box is strictly below the sort location
                    Return -1
                ElseIf box.Low(sortAxis) > sortLocation Then
                    'Box is strictly above the sort location
                    Return 1
                Else
                    'Box overlaps the sort location
                    Return 0
                End If
            End Function

        End Class

        Public Class Node
            Implements IDisposable

            'Spatial bounds on all values at this node and its children, based purely on
            'the parent's splitting planes.  May be infinite.
            Public splitBounds As AABox
            Public splitAxis As Axis
            Public splitLocation As Single 'Location along the specified axis

            'child(0) contains all values strictly
            'smaller than splitLocation along splitAxis.

            'child(1) contains all values strictly
            'larger.

            'Both may be NULL if there are not enough
            'values to bother recursing.
            Public child(1) As Node

            'Array of values at this node (i.e., values
            'straddling the split plane + all values if
            'this is a leaf node).

            'This is an array of pointers because that minimizes
            'data movement during tree building, which accounts
            'for about 15% of the time cost of tree building.
            Public valueArray As List(Of BaseCollision)

            'For each object in the value array, a copy of its bounds.
            'Packing these into an array at the node level
            'instead putting them in the valueArray improves
            'cache coherence, which is about a 3x performance
            'increase when performing intersection computations.
            Public boundsArray As List(Of AABox)

            'Creates node with NULL children
            Public Sub New()
                splitAxis = Axis.X_AXIS
                splitLocation = 0.0F
                splitBounds = New AABox(Vector3.MinInf(), Vector3.MaxInf())
                child(0) = Nothing
                child(1) = Nothing
                valueArray = New List(Of BaseCollision)
                boundsArray = New List(Of AABox)
            End Sub

            'Doesn't clone children.
            Public Sub New(ByRef other As Node)
                splitAxis = other.splitAxis
                splitLocation = other.splitLocation
                splitBounds = other.splitBounds
                child(0) = Nothing
                child(1) = Nothing
                valueArray = New List(Of BaseCollision)(other.valueArray)
                boundsArray = New List(Of AABox)(other.boundsArray)
            End Sub

            'Copies the specified subarray of pt into point, NULLs the children.
            'Assumes a second pass will set splitBounds.
            Public Sub New(ByRef pt As List(Of BaseCollision))
                splitAxis = Axis.X_AXIS
                splitLocation = 0.0F
                child(0) = Nothing
                child(1) = Nothing
                valueArray = New List(Of BaseCollision)(pt)
                boundsArray = New List(Of AABox)
                For i As Integer = 0 To valueArray.Count - 1
                    boundsArray.Add(valueArray(i).Bounds)
                Next
            End Sub

            Public Sub GetValues(ByRef values As List(Of BaseCollision))
                values.AddRange(valueArray)
                For i As Integer = 0 To 1
                    If child(i) IsNot Nothing Then
                        child(i).GetValues(values)
                    End If
                Next
            End Sub

            'Returns true if the ray intersects this node
            Public Function Intersects(ByVal ray As Ray, ByVal distance As Single) As Boolean
                'See if the ray will ever hit this node or its children
                Dim location As Vector3
                Dim alreadyInsideBounds As Boolean = False
                Dim rayWillHitBounds As Boolean = collisionLocationForMovingPointFixedAABox(ray.origin,
                            ray.direction, splitBounds, location, alreadyInsideBounds)

                Dim canHitThisNode As Boolean = (alreadyInsideBounds OrElse
                        (rayWillHitBounds AndAlso ((location - ray.origin).SquaredLength() < (distance * distance))))

                Return canHitThisNode
            End Function

            Public Sub IntersectRay(ByVal ray As Ray, ByRef distance As Single, ByVal pStopAtFirstHit As Boolean, ByVal intersectCallbackIsFast As Boolean)
                Dim enterDistance As Single = distance
                If Not Intersects(ray, distance) Then
                    'The ray doesn't hit this node, so it can't hit the children of the node.
#If VMAPS_DEBUG Then
                    Log.WriteLine(LogType.DEBUG, "Node wasn't hit!")
                    Log.WriteLine(LogType.DEBUG, "iBounds: {0}", splitBounds)
#End If
                    Exit Sub
                End If
#If VMAPS_DEBUG Then
                Log.WriteLine(LogType.DEBUG, "Node was hit!")
                Log.WriteLine(LogType.DEBUG, "iBounds: {0}", splitBounds)
                If child(0) IsNot Nothing Then Log.WriteLine(LogType.DEBUG, "child(0) iBounds: {0}", child(0).splitBounds)
                If child(1) IsNot Nothing Then Log.WriteLine(LogType.DEBUG, "child(1) iBounds: {0}", child(1).splitBounds)
#End If

                'Test for intersection against every object at this node.
                For v As Integer = 0 To valueArray.Count - 1
                    Dim canHitThisObject As Boolean = True

                    If Not intersectCallbackIsFast Then
                        Dim location As Vector3
                        Dim bounds As AABox = boundsArray(v)
#If VMAPS_DEBUG Then
                        Log.WriteLine(LogType.DEBUG, "valueArray({1}): {0}", bounds, v)
#End If
                        Dim alreadyInsideBounds As Boolean = False
                        Dim rayWillHitBounds As Boolean = collisionLocationForMovingPointFixedAABox(
                                ray.origin, ray.direction, bounds, location, alreadyInsideBounds)

                        canHitThisObject = (alreadyInsideBounds OrElse
                            (rayWillHitBounds AndAlso ((location - ray.origin).SquaredLength() < (distance * distance))))
                    End If

                    If canHitThisObject Then
                        'It is possible that this ray hits this object.  Look for the intersection using the
                        'callback.
                        IntersectionCallBack(valueArray(v), ray, pStopAtFirstHit, distance)
                    End If
                    If pStopAtFirstHit AndAlso distance < enterDistance Then Exit Sub
                Next

                ' There are three cases to consider next:
                '
                '  1. the ray can start on one side of the splitting plane and never enter the other,
                '  2. the ray can start on one side and enter the other, and
                '  3. the ray can travel exactly down the splitting plane

                Const NONE As Integer = -1
                Dim firstChild As Integer = NONE
                Dim secondChild As Integer = NONE

                If ray.origin(splitAxis) < splitLocation Then
                    'The ray starts on the small side
                    firstChild = 0
                    If ray.direction(splitAxis) > 0.0F Then
                        'The ray will eventually reach the other side
                        secondChild = 1
                    End If
                ElseIf ray.origin(splitAxis) > splitLocation Then
                    'The ray starts on the large side
                    firstChild = 1
                    If ray.direction(splitAxis) < 0.0F Then
                        secondChild = 0
                    End If
                Else
                    'The ray starts on the splitting plane
                    If ray.direction(splitAxis) < 0.0F Then
                        '...and goes to the small side
                        firstChild = 0
                    ElseIf ray.direction(splitAxis) > 0.0F Then
                        '...and goes to the large side
                        firstChild = 1
                    End If
                End If

                'Test on the side closer to the ray origin.
                If firstChild <> NONE AndAlso child(firstChild) IsNot Nothing Then
                    child(firstChild).IntersectRay(ray, distance, pStopAtFirstHit, intersectCallbackIsFast)
                    If pStopAtFirstHit AndAlso distance < enterDistance Then Exit Sub
                End If

                If ray.direction(splitAxis) <> 0.0F Then
                    'See if there was an intersection before hitting the splitting plane.
                    'If so, there is no need to look on the far side and recursion terminates.
                    Dim distanceToSplittingPlane As Single = (splitLocation - ray.origin(splitAxis)) / ray.direction(splitAxis)
                    If distanceToSplittingPlane < distance Then
                        'We aren't going to hit anything else before hitting the splitting plane,
                        'so don't bother looking on the far side of the splitting plane at the other
                        'child.
                        Exit Sub
                    End If
                End If

                'Test on the side farther from the ray origin.
                If secondChild <> NONE AndAlso child(secondChild) IsNot Nothing Then
                    child(secondChild).IntersectRay(ray, distance, pStopAtFirstHit, intersectCallbackIsFast)
                End If
            End Sub

            'Returns the deepest node that completely contains bounds.
            Public Function FindDeepestContainingNode(ByVal bounds As AABox) As Node
                'See which side of the splitting plane the bounds are on
                If bounds.High(splitAxis) < splitLocation Then
                    'Bounds are on the low side.  Recurse into the child
                    'if it exists.
                    If child(0) IsNot Nothing Then Return child(0).FindDeepestContainingNode(bounds)
                ElseIf bounds.High(splitAxis) < splitLocation Then
                    'Bounds are on the high side, recurse into the child
                    'if it exists.
                    If child(1) IsNot Nothing Then Return child(1).FindDeepestContainingNode(bounds)
                End If

                'There was no containing child, so this node is the
                'deepest containing node.
                Return Me
            End Function

            'Recurse through the tree, assigning splitBounds fields.
            Public Sub AssignSplitBounds(ByVal myBounds As AABox)
                splitBounds = myBounds

                Dim childBounds(1) As AABox
                myBounds.Split(splitAxis, splitLocation, childBounds(0), childBounds(1))

                For objCharacter As Integer = 0 To 1
                    If child(objCharacter) IsNot Nothing Then
                        child(objCharacter).AssignSplitBounds(childBounds(objCharacter))
                    End If
                Next
            End Sub

            Public Sub VerifyNode(ByVal lo As Vector3, ByVal hi As Vector3)
                If lo <> splitBounds.Low Then Log.WriteLine(LogType.FAILED, "[VerifyNode] splitBounds.Low <> lo [{0} - {1}]", lo, splitBounds.Low)
                If hi <> splitBounds.High Then Log.WriteLine(LogType.FAILED, "[VerifyNode] splitBounds.High <> hi [{0} - {1}]", hi, splitBounds.High)

                For i As Integer = 0 To valueArray.Count - 1
                    Dim b As AABox = valueArray(i).Bounds
                    If b <> boundsArray(i) Then Log.WriteLine(LogType.FAILED, "[VerifyNode] boundsArray({0}) <> b [{1} - {2}]", i, b, boundsArray(i))

                    For axis As Integer = 0 To 2
                        If b.Low(axis) > b.High(axis) Then Log.WriteLine(LogType.FAILED, "[VerifyNode] boundsArray({0}).Low({1}) > high [{2} - {3}]", i, axis, b.Low(axis), b.High(axis))
                        If b.Low(axis) < lo(axis) Then Log.WriteLine(LogType.FAILED, "[VerifyNode] boundsArray({0}).Low({1}) < parentLow [{2} - {3}]", i, axis, b.Low(axis), lo(axis))
                        If b.High(axis) > hi(axis) Then Log.WriteLine(LogType.FAILED, "[VerifyNode] boundsArray({0}).High({1}) > parentHigh [{2} - {3}]", i, axis, b.High(axis), hi(axis))
                    Next
                Next

                If (child(0) IsNot Nothing) OrElse (child(1) IsNot Nothing) Then
                    If lo(splitAxis) >= splitLocation Then Log.WriteLine(LogType.FAILED, "[VerifyNode] lo(splitAxis) >= splitLocation [{0} - {1}]", lo(splitAxis), splitLocation)
                    If hi(splitAxis) <= splitLocation Then Log.WriteLine(LogType.FAILED, "[VerifyNode] hi(splitAxis) <= splitLocation [{0} - {1}]", hi(splitAxis), splitLocation)
                End If

                Dim newLo As New Vector3(lo)
                newLo(splitAxis) = splitLocation
                Dim newHi As New Vector3(hi)
                newHi(splitAxis) = splitLocation

                If child(0) IsNot Nothing Then
                    child(0).VerifyNode(lo, newHi)
                End If

                If child(1) IsNot Nothing Then
                    child(1).VerifyNode(newLo, hi)
                End If
            End Sub

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            'Deletes the children (but not the values)
            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    For i As Integer = 0 To 1
                        If child(i) IsNot Nothing Then
                            child(i).Dispose()
                            child(i) = Nothing
                        End If
                    Next
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

        Private root As Node

        Public Sub Insert(ByRef valueArray As List(Of BaseCollision))
            If root Is Nothing Then
                'Optimized case for an empty tree; don't bother
                'searching or reallocating the root node's valueArray
                'as we incrementally insert.
                root = New Node()
                For i As Integer = 0 To valueArray.Count - 1
                    'Insert in opposite order so that we have the exact same
                    'data structure as if we inserted each (i.e., order is reversed
                    'from array).
                    root.valueArray.Add(valueArray(i))
                    root.boundsArray.Add(valueArray(i).Bounds)
                Next
            Else
                'Insert at appropriate tree depth.
                For i As Integer = 0 To valueArray.Count - 1
                    Insert(valueArray(i))
                Next
            End If
        End Sub

        Public Sub Insert(ByRef value As BaseCollision)
            If root Is Nothing Then
                'This is the first node; create a root node
                root = New Node()
            End If

            Dim node As Node = root.FindDeepestContainingNode(value.Bounds)

            'Insert into the node
            node.valueArray.Add(value)
            node.boundsArray.Add(value.Bounds)
        End Sub

        Public Sub Balance(Optional ByVal valuesPerNode As Integer = 5, Optional ByVal numMeanSplits As Integer = 5)
            If root Is Nothing Then Exit Sub 'Tree is empty

            'Get all handles and delete the old tree structure
            Dim oldRoot As Node = root
            For objCharacter As Integer = 0 To 1
                If root.child(objCharacter) IsNot Nothing Then
                    root.child(objCharacter).GetValues(root.valueArray)

                    'Delete the child; this will delete all structure below it
                    root.child(objCharacter).Dispose()
                    root.child(objCharacter) = Nothing
                End If
            Next

            Dim temp As New List(Of BaseCollision)
            'Make a new root.  Work with a copy of the value array because
            'makeNode clears the source array as it progresses
            Dim copy As New List(Of BaseCollision)(oldRoot.valueArray)
            root = MakeNode(copy, valuesPerNode, numMeanSplits, temp)

            'Throw away the old root node
            oldRoot.Dispose()
            oldRoot = Nothing

            'Walk the tree, assigning splitBounds.  We start with unbounded
            'space.  This will override the current member table.
            root.AssignSplitBounds(AABox.MaxFinite())

#If VMAPS_DEBUG Then
            'Ensure that the balanced tree is till correct
            root.VerifyNode(Vector3.MinFinite(), Vector3.MaxFinite())
#End If
        End Sub

        'Recursively subdivides the subarray.
        'Clears the source array as soon as it is no longer needed.
        'Call assignSplitBounds() on the root node after making a tree.
        Private Function MakeNode(ByRef source As List(Of BaseCollision), ByVal valuesPerNode As Integer, ByVal numMeanSplits As Integer, ByRef temp As List(Of BaseCollision)) As Node
            Dim node As Node = Nothing

            If source.Count <= valuesPerNode Then
                'Make a new leaf node
                node = New Node(source)

                source.Clear()
            Else
                'Make a new internal node
                node = New Node()
                Dim bounds As AABox = ComputeBounds(source, 0, source.Count - 1)
                Dim extent As Vector3 = bounds.High() - bounds.Low()

                Dim splitAxis As Axis = extent.PrimaryAxis()

                Dim splitLocation As Single

                'Arrays for holding the children
                Dim lt As New List(Of BaseCollision)
                Dim gt As New List(Of BaseCollision)

                If numMeanSplits <= 0 Then
                    MedianPartition(source, lt, node.valueArray, gt, temp, New CenterComparator(splitAxis))

                    'Choose the split location to be the center of whatever fell in the center
                    splitLocation = node.valueArray(0).Bounds.Center(splitAxis)

                    'Some of the elements in the lt or gt array might really overlap the split location.
                    'Move them as needed.
                    For i As Integer = 0 To lt.Count - 1
                        If i > (lt.Count - 1) Then Exit For
                        Dim bounds2 As AABox = lt(i).Bounds
                        If (bounds2.Low(splitAxis) <= splitLocation) AndAlso (bounds2.High(splitAxis) >= splitLocation) Then
                            node.valueArray.Add(lt(i))
                            'Remove this element and process the new one that
                            'is swapped in in its place.
                            lt.RemoveAt(i)
                            i -= 1
                        End If
                    Next

                    For i As Integer = 0 To gt.Count - 1
                        If i > (gt.Count - 1) Then Exit For
                        Dim bounds2 As AABox = gt(i).Bounds
                        If (bounds2.Low(splitAxis) <= splitLocation) AndAlso (bounds2.High(splitAxis) >= splitLocation) Then
                            node.valueArray.Add(gt(i))
                            'Remove this element and process the new one that
                            'is swapped in in its place.
                            gt.RemoveAt(i)
                            i -= 1
                        End If
                    Next

                    If (node.valueArray.Count > (source.Count \ 2)) AndAlso source.Count > 6 Then
                        'This was a bad partition; we ended up putting the splitting plane right in the middle of most of the
                        'objects.  We could try to split on a different axis, or use a different partition (e.g., the extents mean,
                        'or geometric mean).  This implementation falls back on the extents mean, since that case is already handled
                        'below.
                        numMeanSplits = 1
                    End If
                End If

                'Note: numMeanSplits may have been increased by the code in the previous case above in order to
                'force a re-partition.

                If numMeanSplits > 0 Then
                    'Split along the mean
                    splitLocation = (bounds.High(splitAxis) + bounds.Low(splitAxis)) / 2.0F

                    Partition(source, Nothing, lt, node.valueArray, gt, New Comparator(splitAxis, splitLocation))

                    'The Comparator ensures that elements are strictly on the correct side of the split
                End If

#If VMAPS_DEBUG Then
                If (lt.Count + node.valueArray.Count + gt.Count) <> source.Count Then Log.WriteLine(LogType.CRITICAL, "[MakeNode] Node count doesn't match! [{0} - {1}]", (lt.Count + node.valueArray.Count + gt.Count), source.Count)
                'Verify that all objects ended up on the correct side of the split.
                '(i.e., make sure that the Array partition was correct)
                For i As Integer = 0 To lt.Count - 1
                    Dim bounds2 As AABox = lt(i).Bounds
                    If bounds2.High(splitAxis) >= splitLocation Then Log.WriteLine(LogType.CRITICAL, "[MakeNode] lt({0}).Bounds.High(splitAxis) >= splitLocation [{1} - {2}]", i, bounds2.High(splitAxis), splitLocation)
                Next

                For i As Integer = 0 To gt.Count - 1
                    Dim bounds2 As AABox = gt(i).Bounds
                    If bounds2.Low(splitAxis) <= splitLocation Then Log.WriteLine(LogType.CRITICAL, "[MakeNode] gt({0}).Bounds.Low(splitAxis) <= splitLocation [{1} - {2}]", i, bounds2.Low(splitAxis), splitLocation)
                Next

                For i As Integer = 0 To node.valueArray.Count - 1
                    Dim bounds2 As AABox = node.valueArray(i).Bounds
                    If bounds2.High(splitAxis) < splitLocation Then Log.WriteLine(LogType.CRITICAL, "[MakeNode] node.valueArray({0}).Bounds.High(splitAxis) < splitLocation [{1} - {2}]", i, bounds2.High(splitAxis), splitLocation)
                    If bounds2.Low(splitAxis) > splitLocation Then Log.WriteLine(LogType.CRITICAL, "[MakeNode] node.valueArray({0}).Bounds.Low(splitAxis) > splitLocation [{1} - {2}]", i, bounds2.Low(splitAxis), splitLocation)
                Next
#End If

                'The source array is no longer needed
                source.Clear()

                node.splitAxis = splitAxis
                node.splitLocation = splitLocation

                'Update the bounds array and member table
                node.boundsArray = New List(Of AABox)
                For i As Integer = 0 To node.valueArray.Count - 1
                    node.boundsArray.Add(node.valueArray(i).Bounds)
                Next

                If lt.Count > 0 Then
                    node.child(0) = MakeNode(lt, valuesPerNode, numMeanSplits - 1, temp)
                End If

                If gt.Count > 0 Then
                    node.child(1) = MakeNode(gt, valuesPerNode, numMeanSplits - 1, temp)
                End If
            End If

            Return node
        End Function

        Public Sub IntersectRay(ByVal pRay As Ray, ByRef distance As Single, ByVal pStopAtFirstHit As Boolean, Optional ByVal intersectCallbackIsFast As Boolean = False)
            root.IntersectRay(pRay, distance, pStopAtFirstHit, intersectCallbackIsFast)
        End Sub
    End Class

    Public Class TreeNode

        Private ID As Integer
        Private iSplitLocation As Single 'Location along the specified axis
        Private iChilds(1) As Integer 'Offest or the clients
        Private iStartPosition As Integer 'Position within the TriangleBox array
        Private iSplitAxis As Axis
        Private iBounds As AABox
        Private iNumberOfValues As UShort

        Public Sub New()

        End Sub

        Public Sub New(ByVal i As Integer, ByVal iSplitLocation As Single, ByVal iChild0 As Integer, ByVal iChild1 As Integer, ByVal iStartPosition As Integer, ByVal iSplitAxis As Axis, ByVal iBounds As AABox, ByVal iNumberOfValues As UShort, ByVal dummy As UShort)
            ID = i
            Me.iSplitLocation = iSplitLocation
            iChilds(0) = iChild0
            iChilds(1) = iChild1
            Me.iStartPosition = iStartPosition
            Me.iSplitAxis = iSplitAxis
            Me.iBounds = iBounds
            Me.iNumberOfValues = iNumberOfValues
        End Sub

        'Returns true if the ray intersects this node
        Public Function Intersects(ByVal ray As Ray, ByVal distance As Single) As Boolean
            'See if the ray will ever hit this node or its children
            Dim location As Vector3
            Dim alreadyInsideBounds As Boolean = False
            Dim rayWillHitBounds As Boolean = collisionLocationForMovingPointFixedAABox(ray.origin,
                        ray.direction, iBounds, location, alreadyInsideBounds)

            Dim canHitThisNode As Boolean = (alreadyInsideBounds OrElse
                    (rayWillHitBounds AndAlso ((location - ray.origin).SquaredLength() < (distance * distance))))

            Return canHitThisNode
        End Function

        Public Sub IntersectRay(ByVal ray As Ray, ByRef distance As Single, ByRef pNodes As List(Of TreeNode), ByVal iNodeStart As Integer, ByRef pSecond As List(Of BaseCollision), ByVal iSecondStart As Integer, ByVal pStopAtFirstHit As Boolean, ByVal intersectCallbackIsFast As Boolean)
            Dim enterDistance As Single = distance
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "TreeNode ID: {0}!", ID)
#End If
            If Not Intersects(ray, distance) Then
                'The ray doesn't hit this node, so it can't hit the children of the node.
#If VMAPS_DEBUG Then
                Log.WriteLine(LogType.DEBUG, "Doesn't hit TreeNode!")
                Log.WriteLine(LogType.DEBUG, "iBounds: {0}", iBounds)
#End If
                Exit Sub
            End If
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "Hit TreeNode!")
            Log.WriteLine(LogType.DEBUG, "iBounds: {0}", iBounds)
            If iChilds(0) > 0 Then Log.WriteLine(LogType.DEBUG, "child(0) iBounds: {0}", pNodes(iNodeStart + iChilds(0)).iBounds)
            If iChilds(1) > 0 Then Log.WriteLine(LogType.DEBUG, "child(1) iBounds: {0}", pNodes(iNodeStart + iChilds(1)).iBounds)
#End If

            'Test for intersection against every object at this node.
            For v As Integer = iStartPosition To (iNumberOfValues + iStartPosition) - 1
                Dim nodeValue As BaseCollision = pSecond(iSecondStart + v)
                Dim canHitThisObject As Boolean = True
                If Not intersectCallbackIsFast Then
                    Dim location As Vector3
                    Dim bounds As AABox = nodeValue.Bounds()
#If VMAPS_DEBUG Then
                    Log.WriteLine(LogType.DEBUG, "submodel({1}) iBounds: {0}", bounds, v)
#End If
                    Dim alreadyInsideBounds As Boolean = False
                    Dim rayWillHitBounds As Boolean = collisionLocationForMovingPointFixedAABox(
                        ray.origin, ray.direction, bounds, location, alreadyInsideBounds)

                    canHitThisObject = (alreadyInsideBounds OrElse
                        (rayWillHitBounds AndAlso ((location - ray.origin).SquaredLength() < (distance * distance))))
                End If

                If canHitThisObject Then
                    'It is possible that this ray hits this object.  Look for the intersection using the
                    'callback.
                    IntersectionCallBack(nodeValue, ray, pStopAtFirstHit, distance)
                End If
                If pStopAtFirstHit AndAlso distance < enterDistance Then Exit Sub
            Next

            ' There are three cases to consider next:
            '
            ' 1. the ray can start on one side of the splitting plane and never enter the other,
            ' 2. the ray can start on one side and enter the other, and
            ' 3. the ray can travel exactly down the splitting plane

            Const NONE As Integer = -1
            Dim firstChild As Integer = NONE
            Dim secondChild As Integer = NONE

            If ray.origin(iSplitAxis) < iSplitLocation Then
                'The ray starts on the small side
                firstChild = 0
                If ray.direction(iSplitAxis) > 0.0F Then
                    'The ray will eventually reach the other side
                    secondChild = 1
                End If
            ElseIf ray.origin(iSplitAxis) > iSplitLocation Then
                'The ray starts on the large side
                firstChild = 1
                If ray.direction(iSplitAxis) < 0.0F Then
                    secondChild = 0
                End If
            Else
                'The ray starts on the splitting plane
                If ray.direction(iSplitAxis) < 0.0F Then
                    '...and goes to the small side
                    firstChild = 0
                ElseIf ray.direction(iSplitAxis) > 0.0F Then
                    '...and goes to the large side
                    firstChild = 1
                End If
            End If

            'Test on the side closer to the ray origin.
            If firstChild <> NONE AndAlso iChilds(firstChild) > 0 Then
#If VMAPS_DEBUG Then
                Log.WriteLine(LogType.DEBUG, "TreeNode child({0}) testing!", firstChild)
#End If
                pNodes(iNodeStart + iChilds(firstChild)).IntersectRay(ray, distance, pNodes, iNodeStart, pSecond, iSecondStart, pStopAtFirstHit, intersectCallbackIsFast)
                If pStopAtFirstHit AndAlso distance < enterDistance Then Exit Sub
            End If
            If ray.direction(iSplitAxis) <> 0.0F Then
                'See if there was an intersection before hitting the splitting plane.
                'If so, there is no need to look on the far side and recursion terminates.
                Dim distanceToSplittingPlane As Single = (iSplitLocation - ray.origin(iSplitAxis)) / ray.direction(iSplitAxis)
                If distanceToSplittingPlane > distance Then
                    'We aren't going to hit anything else before hitting the splitting plane,
                    'so don't bother looking on the far side of the splitting plane at the other
                    'child.
                    Exit Sub
                End If
            End If
            'Test on the side farther from the ray origin.
            If secondChild <> NONE AndAlso iChilds(secondChild) > 0 Then
#If VMAPS_DEBUG Then
                Log.WriteLine(LogType.DEBUG, "TreeNode child({0}) testing!", secondChild)
#End If
                pNodes(iNodeStart + iChilds(secondChild)).IntersectRay(ray, distance, pNodes, iNodeStart, pSecond, iSecondStart, pStopAtFirstHit, intersectCallbackIsFast)
            End If
        End Sub

        Public Overloads Function ToString(ByVal i As Integer) As String
            Return String.Format("{7} - SplitLocation:{0} Childs(0):{1} Childs(1):{2} StartPosition:{3} SplitAxis:{4} Bounds:{5} NumberOfValues:{6}", iSplitLocation, iChilds(0), iChilds(1), iStartPosition, iSplitAxis, iBounds, iNumberOfValues, i)
        End Function
    End Class

    Public Class TriangleBox
        Implements BaseCollision

        Private _vertex(2) As ShortVector

        Public Sub New()

        End Sub

        Public Sub New(ByVal pV1 As ShortVector, ByVal pV2 As ShortVector, ByVal pV3 As ShortVector)
            _vertex(0) = pV1
            _vertex(1) = pV2
            _vertex(2) = pV3
        End Sub

        Public ReadOnly Property Vertex(ByVal n As Integer) As ShortVector
            Get
                Return _vertex(n)
            End Get
        End Property

        Public ReadOnly Property GetBounds() As ShortBox
            Get
                Dim box As New ShortBox()
                Dim lo As ShortVector = _vertex(0)
                Dim hi As ShortVector = lo

                For i As Integer = 1 To 2
                    lo = lo.Min(_vertex(i))
                    hi = hi.Max(_vertex(i))
                Next
                box.SetLow(lo)
                box.SetHigh(hi)
                Return box
            End Get
        End Property

        Public ReadOnly Property GetAABoxBounds() As AABox Implements BaseCollision.Bounds
            Get
                Dim box As ShortBox = GetBounds()
                Return New AABox(box.Low.GetVector3(), box.High.GetVector3())
            End Get
        End Property

        Public Sub Intersect(ByVal pRay As Ray, ByRef pMaxDist As Single, ByVal pStopAtFirstHit As Boolean) Implements BaseCollision.Intersect
            Static epsilon As Double = 0.00001
            Dim testT As New Triangle(Vertex(0).GetVector3, Vertex(1).GetVector3, Vertex(2).GetVector3)
            Dim t As Single = pRay.IntersectionTime(testT)
            If (t < pMaxDist) OrElse t < (pMaxDist + epsilon) Then
                pMaxDist = t
            Else
                testT = New Triangle(Vertex(2).GetVector3, Vertex(1).GetVector3, Vertex(0).GetVector3)
                t = pRay.IntersectionTime(testT)
                If (t < pMaxDist) OrElse t < (pMaxDist + epsilon) Then pMaxDist = t
            End If
        End Sub

        Public Shared Operator =(ByVal a As TriangleBox, ByVal b As TriangleBox) As Boolean
            Return (a._vertex(0) = b._vertex(0)) AndAlso (a._vertex(1) = b._vertex(1)) AndAlso (a._vertex(2) = b._vertex(2))
        End Operator

        Public Shared Operator <>(ByVal a As TriangleBox, ByVal b As TriangleBox) As Boolean
            Return Not ((a._vertex(0) = b._vertex(0)) AndAlso (a._vertex(1) = b._vertex(1)) AndAlso (a._vertex(2) = b._vertex(2)))
        End Operator

        Public Overloads Function ToString(ByVal i As Integer) As String
            Return String.Format("{0} - vertex(0):{1} vertex(1):{2} vertex(2):{3}", i, _vertex(0).GetVector3, _vertex(1).GetVector3, _vertex(2).GetVector3)
        End Function

    End Class

    Public Class BaseModel
        Protected numNodes As Integer
        Protected numTriangles As Integer
        Protected iTreeNodes As List(Of TreeNode)
        Protected iTriangles As List(Of BaseCollision)
        Protected iBasePosition As Vector3

        Public Sub New()
            iTriangles = Nothing
            iTreeNodes = Nothing
        End Sub

        Public Sub New(ByVal pNNodes As Integer, ByVal pNTriangles As Integer)
            Init(pNNodes, pNTriangles)
        End Sub

        Public Sub New(ByRef pTreeNode As List(Of TreeNode), ByRef pTriangleBox As List(Of BaseCollision))
            iTreeNodes = pTreeNode
            iTriangles = pTriangleBox
        End Sub

        Public ReadOnly Property GetBasePosition() As Vector3
            Get
                Return iBasePosition
            End Get
        End Property

        Public Sub Free()
            iTreeNodes.Clear()
            iTreeNodes = Nothing
            iTriangles.Clear()
            iTriangles = Nothing
        End Sub

        Public Sub Init(ByVal pNNodes As Integer, ByVal pNTriangles As Integer)
            If pNNodes > 0 Then iTreeNodes = New List(Of TreeNode)(pNNodes)
            If pNTriangles > 0 Then iTriangles = New List(Of BaseCollision)(pNTriangles)
        End Sub

        Public Sub GetMembers(ByRef pMembers As List(Of TriangleBox))
            For i As Integer = 0 To iTriangles.Count - 1
                pMembers.Add(iTriangles(i))
            Next
        End Sub

        Public Sub Intersect(ByVal pBox As AABox, ByVal pRay As Ray, ByRef pMaxDist As Single, ByRef pOutLocation As Vector3)
            Dim inside As Boolean = False

            Dim d As Single = collisionLocationForMovingPointFixedAABox(pRay.origin, pRay.direction,
                        pBox, pOutLocation, inside)
            If inside = False AndAlso d > 0.0F AndAlso d < pMaxDist Then
                pMaxDist = d
            End If
        End Sub

        Public Function Intersect(ByVal pBox As AABox, ByVal pRay As Ray, ByRef pMaxDist As Single) As Boolean
            'See if the ray will ever hit this node or its children
            Dim location As Vector3
            Dim alreadyInsideBounds As Boolean = False
            Dim rayWillHitBounds As Boolean = collisionLocationForMovingPointFixedAABox(pRay.origin, pRay.direction,
                        pBox, location, alreadyInsideBounds)

            Dim canHitThisNode As Boolean = (alreadyInsideBounds OrElse
                     (rayWillHitBounds AndAlso ((location - pRay.origin).SquaredLength() < (pMaxDist * pMaxDist))))

            Return canHitThisNode
        End Function

    End Class

    Public Class SubModel
        Inherits BaseModel
        Implements BaseCollision

        Private iNodesPos As Integer
        Private iTrianglesPos As Integer
        Private iHasInternalMemAlloc As Boolean
        Private iBox As ShortBox

        Public Sub New()
            MyBase.New()
        End Sub

        Public ReadOnly Property Bounds() As AABox Implements BaseCollision.Bounds
            Get
                Return New AABox(iBox.Low().GetVector3() + iBasePosition, iBox.High().GetVector3() + iBasePosition)
            End Get
        End Property

        Public Sub SetNodes(ByRef nodes As List(Of TreeNode))
            iTreeNodes = nodes
        End Sub

        Public Sub SetTriangles(ByRef triangles As List(Of BaseCollision))
            iTriangles = triangles
        End Sub

        Public Sub InitSubModel(ByRef b As BinaryReader)
            b.ReadInt64() '(0)
            numTriangles = b.ReadInt32() '(8)
            numNodes = b.ReadInt32() '(12)
            iBasePosition = New Vector3(b.ReadSingle(), b.ReadSingle(), b.ReadSingle()) '(16)
            iNodesPos = b.ReadInt32() '(28)
            iTrianglesPos = b.ReadInt32() '(32)
            iHasInternalMemAlloc = (b.ReadByte() <> 0) '(36)
            b.ReadByte() '(37)
            iBox = New ShortBox(New ShortVector(b.ReadInt16(), b.ReadInt16(), b.ReadInt16()),
                                New ShortVector(b.ReadInt16(), b.ReadInt16(), b.ReadInt16())) '(38)
            b.ReadInt16()
        End Sub

        Public Overloads Sub Intersect(ByVal pRay As Ray, ByRef pMaxDist As Single, ByVal pStopAtFirstHit As Boolean) Implements BaseCollision.Intersect
            Dim relativeRay As New Ray(pRay.origin - iBasePosition, pRay.direction)
            iTreeNodes(iNodesPos).IntersectRay(relativeRay, pMaxDist, iTreeNodes, iNodesPos, iTriangles, iTrianglesPos, pStopAtFirstHit, False)
        End Sub

        Public Overloads Function ToString(ByVal i As Integer) As String
            Return String.Format("{0} - numTriangles:{1} numNodes:{2} BasePosition:{3} NodesPos:{4} TrianglesPos:{5} HasInternalMemAlloc:{6} Box:{7}", i, numTriangles, numNodes, iBasePosition, iNodesPos, iTrianglesPos, iHasInternalMemAlloc, iBox)
        End Function

    End Class

    Public Structure Vector3



        Public x As Single
        Public y As Single
        Public z As Single

        Public Sub New(ByVal v As Vector3)
            x = v.x
            y = v.y
            z = v.z
        End Sub

        Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single)
            Me.x = x
            Me.y = y
            Me.z = z
        End Sub

        Public Shared ReadOnly Property Zero() As Vector3
            Get
                Return New Vector3(0.0F, 0.0F, 0.0F)
            End Get
        End Property

        Public Shared ReadOnly Property One() As Vector3
            Get
                Return New Vector3(1.0F, 1.0F, 1.0F)
            End Get
        End Property

        Public Shared ReadOnly Property UnitX() As Vector3
            Get
                Return New Vector3(1.0F, 0.0F, 0.0F)
            End Get
        End Property

        Public Shared ReadOnly Property UnitY() As Vector3
            Get
                Return New Vector3(0.0F, 1.0F, 0.0F)
            End Get
        End Property

        Public Shared ReadOnly Property UnitZ() As Vector3
            Get
                Return New Vector3(0.0F, 0.0F, 1.0F)
            End Get
        End Property

        Public Shared ReadOnly Property NaN() As Vector3
            Get
                Return New Vector3(Single.NaN, Single.NaN, Single.NaN)
            End Get
        End Property

        Public Shared ReadOnly Property MinFinite() As Vector3
            Get
                Return New Vector3(Single.MinValue, Single.MinValue, Single.MinValue)
            End Get
        End Property

        Public Shared ReadOnly Property MaxFinite() As Vector3
            Get
                Return New Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue)
            End Get
        End Property

        Public Shared ReadOnly Property MinInf() As Vector3
            Get
                Return New Vector3(Single.NegativeInfinity, Single.NegativeInfinity, Single.NegativeInfinity)
            End Get
        End Property

        Public Shared ReadOnly Property MaxInf() As Vector3
            Get
                Return New Vector3(Single.PositiveInfinity, Single.PositiveInfinity, Single.PositiveInfinity)
            End Get
        End Property

        Default Public Property Item(ByVal a As Integer) As Single
            Get
                If a = 0 Then
                    Return x
                ElseIf a = 1 Then
                    Return y
                ElseIf a = 2 Then
                    Return z
                Else
                    Return Single.NaN
                End If
            End Get
            Set(ByVal value As Single)
                If a = 0 Then
                    x = value
                ElseIf a = 1 Then
                    y = value
                ElseIf a = 2 Then
                    z = value
                End If
            End Set
        End Property

        Public ReadOnly Property PrimaryAxis() As Axis
            Get
                Dim a As Axis = Axis.X_AXIS

                Dim nx As Single = Math.Abs(x)
                Dim ny As Single = Math.Abs(y)
                Dim nz As Single = Math.Abs(z)

                If nx > ny Then
                    If nx > nz Then
                        a = Axis.X_AXIS
                    Else
                        a = Axis.Z_AXIS
                    End If
                Else
                    If ny > nz Then
                        a = Axis.Y_AXIS
                    Else
                        a = Axis.Z_AXIS
                    End If
                End If

                Return a
            End Get
        End Property

        Public ReadOnly Property Sum() As Single
            Get
                Return x + y + z
            End Get
        End Property

        Public ReadOnly Property Average() As Single
            Get
                Return Sum() / 3.0F
            End Get
        End Property

        Public ReadOnly Property SquaredMagnitude() As Single
            Get
                Return x * x + y * y + z * z
            End Get
        End Property

        Public ReadOnly Property SquaredLength() As Single
            Get
                Return SquaredMagnitude()
            End Get
        End Property

        Public ReadOnly Property Magnitude() As Single
            Get
                Return Math.Sqrt(x * x + y * y + z * z)
            End Get
        End Property

        Public ReadOnly Property Length() As Single
            Get
                Return Magnitude()
            End Get
        End Property

        Public ReadOnly Property Direction() As Vector3
            Get
                Dim lenSquared As Single = SquaredMagnitude()
                Dim invSqrt As Single = 1.0F / Math.Sqrt(lenSquared)
                Return New Vector3(x * invSqrt, y * invSqrt, z * invSqrt)
            End Get
        End Property

        Public Function Min(ByRef v As Vector3) As Vector3
            Return New Vector3(Math.Min(v.x, x), Math.Min(v.y, y), Math.Min(v.z, z))
        End Function

        Public Function Max(ByRef v As Vector3) As Vector3
            Return New Vector3(Math.Max(v.x, x), Math.Max(v.y, y), Math.Max(v.z, z))
        End Function

        Public Function Cross(ByRef rkVector As Vector3) As Vector3
            Return New Vector3(y * rkVector.z - z * rkVector.y, z * rkVector.x - x * rkVector.z,
                       x * rkVector.y - y * rkVector.x)
        End Function

        Public Function Dot(ByRef rkVector As Vector3) As Single
            Return x * rkVector.x + y * rkVector.y + z * rkVector.z
        End Function

        Public Shared Operator +(ByVal a As Vector3, ByVal b As Vector3) As Vector3
            Return New Vector3(a.x + b.x, a.y + b.y, a.z + b.z)
        End Operator

        Public Shared Operator -(ByVal a As Vector3, ByVal b As Vector3) As Vector3
            Return New Vector3(a.x - b.x, a.y - b.y, a.z - b.z)
        End Operator

        Public Shared Operator -(ByVal a As Vector3) As Vector3
            Return New Vector3(-a.x, -a.y, -a.z)
        End Operator

        Public Shared Operator *(ByVal a As Vector3, ByVal b As Vector3) As Vector3
            Return New Vector3(a.x * b.x, a.y * b.y, a.z * b.z)
        End Operator

        Public Shared Operator /(ByVal a As Vector3, ByVal b As Vector3) As Vector3
            Return New Vector3(a.x / b.x, a.y / b.y, a.z / b.z)
        End Operator

        Public Shared Operator *(ByVal a As Vector3, ByVal b As Single) As Vector3
            Return New Vector3(a.x * b, a.y * b, a.z * b)
        End Operator

        Public Shared Operator /(ByVal a As Vector3, ByVal b As Single) As Vector3
            Return New Vector3(a.x / b, a.y / b, a.z / b)
        End Operator

        Public Shared Operator <(ByVal a As Vector3, ByVal b As Vector3) As Boolean
            Return (a.x < b.x) AndAlso (a.y < b.y) AndAlso (a.z < b.z)
        End Operator

        Public Shared Operator >(ByVal a As Vector3, ByVal b As Vector3) As Boolean
            Return (a.x > b.x) AndAlso (a.y > b.y) AndAlso (a.z > b.z)
        End Operator

        Public Shared Operator <=(ByVal a As Vector3, ByVal b As Vector3) As Boolean
            Return (a.x <= b.x) AndAlso (a.y <= b.y) AndAlso (a.z <= b.z)
        End Operator

        Public Shared Operator >=(ByVal a As Vector3, ByVal b As Vector3) As Boolean
            Return (a.x >= b.x) AndAlso (a.y >= b.y) AndAlso (a.z >= b.z)
        End Operator

        Public Shared Operator =(ByVal a As Vector3, ByVal b As Vector3) As Boolean
            Return (a.x = b.x) AndAlso (a.y = b.y) AndAlso (a.z = b.z)
        End Operator

        Public Shared Operator <>(ByVal a As Vector3, ByVal b As Vector3) As Boolean
            Return Not ((a.x = b.x) AndAlso (a.y = b.y) AndAlso (a.z = b.z))
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Return (x.GetHashCode() + CLng(y.GetHashCode()) + z.GetHashCode()) And &HFFFFFFFF
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("({0} {1} {2})", x, y, z)
        End Function

    End Structure

    Public Structure ShortVector
        Private iX As Short
        Private iY As Short
        Private iZ As Short

        Private Const maxvalue As Short = Short.MaxValue
        Private Const minvalue As Short = Short.MinValue
        Private Const fixpointdiv As Integer = 16
        Private Const fixpoint_maxvalue As Short = maxvalue \ fixpointdiv
        Private Const fixpoint_minvalue As Short = minvalue \ fixpointdiv

        Public ReadOnly Property getFX() As Single
            Get
                Return short2Float(iX)
            End Get
        End Property

        Public ReadOnly Property getFY() As Single
            Get
                Return short2Float(iY)
            End Get
        End Property

        Public ReadOnly Property getFZ() As Single
            Get
                Return short2Float(iZ)
            End Get
        End Property

        Public ReadOnly Property getX() As Single
            Get
                Return iX
            End Get
        End Property

        Public ReadOnly Property getY() As Single
            Get
                Return iY
            End Get
        End Property

        Public ReadOnly Property getZ() As Single
            Get
                Return iZ
            End Get
        End Property

        Public ReadOnly Property GetVector3() As Vector3
            Get
                Return New Vector3(getFX(), getFY(), getFZ())
            End Get
        End Property

        Private Function float2Short(ByVal fv As Single) As Short
            Dim sv As Short
            If fv >= fixpoint_maxvalue Then
                sv = maxvalue
            ElseIf fv <= fixpoint_minvalue Then
                sv = minvalue
            Else
                sv = fv * fixpointdiv + 0.5F
            End If
            Return sv
        End Function

        Private Function short2Float(ByVal sv As Single) As Short
            Dim fv As Single
            If sv >= maxvalue Then
                fv = Single.PositiveInfinity
            ElseIf sv <= minvalue Then
                fv = Single.NegativeInfinity
            Else
                fv = sv / fixpointdiv
            End If
            Return fv
        End Function

        Public Sub New(ByRef v As Vector3)
            iX = float2Short(v.x)
            iY = float2Short(v.y)
            iZ = float2Short(v.z)
        End Sub

        Public Sub New(ByVal pX As Short, ByVal pY As Short, ByVal pZ As Short)
            iX = pX
            iY = pY
            iZ = pZ
        End Sub

        Public Function Min(ByRef pShortVector As ShortVector) As ShortVector
            Return New ShortVector(Math.Min(iX, pShortVector.iX), Math.Min(iY, pShortVector.iY), Math.Min(iZ, pShortVector.iZ))
        End Function

        Public Function Max(ByRef pShortVector As ShortVector) As ShortVector
            Return New ShortVector(Math.Max(iX, pShortVector.iX), Math.Max(iY, pShortVector.iY), Math.Max(iZ, pShortVector.iZ))
        End Function

        Public Shared Operator =(ByVal a As ShortVector, ByVal b As ShortVector) As Boolean
            Return (a.iX = b.iX) AndAlso (a.iY = b.iY) AndAlso (a.iZ = b.iZ)
        End Operator

        Public Shared Operator <>(ByVal a As ShortVector, ByVal b As ShortVector) As Boolean
            Return Not ((a.iX = b.iX) AndAlso (a.iY = b.iY) AndAlso (a.iZ = b.iZ))
        End Operator

    End Structure

    Public Structure ShortBox
        Private iV1 As ShortVector
        Private iV2 As ShortVector

        Public Sub New(ByVal lo As ShortVector, ByVal hi As ShortVector)
            iV1 = lo
            iV2 = hi
        End Sub

        Public ReadOnly Property Low() As ShortVector
            Get
                Return iV1
            End Get
        End Property

        Public ReadOnly Property High() As ShortVector
            Get
                Return iV2
            End Get
        End Property

        Public Sub SetLow(ByVal pV As ShortVector)
            iV1 = pV
        End Sub

        Public Sub SetHigh(ByVal pV As ShortVector)
            iV2 = pV
        End Sub

        Public Sub SetLow(ByVal pV As Vector3)
            iV1 = New ShortVector(pV)
        End Sub

        Public Sub SetHigh(ByVal pV As Vector3)
            iV2 = New ShortVector(pV)
        End Sub

        Public Shared Operator =(ByVal a As ShortBox, ByVal b As ShortBox) As Boolean
            Return (a.iV1 = b.iV1) AndAlso (a.iV2 = b.iV2)
        End Operator

        Public Shared Operator <>(ByVal a As ShortBox, ByVal b As ShortBox) As Boolean
            Return Not ((a.iV1 = b.iV1) AndAlso (a.iV2 = b.iV2))
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("{0}-{1}", iV1.GetVector3, iV2.GetVector3)
        End Function

    End Structure

    'Axis-aligned box
    Public Structure AABox

        Private lo As Vector3
        Private hi As Vector3

        Public Shared ReadOnly Property MaxFinite() As AABox
            Get
                Return New AABox(Vector3.MinFinite(), Vector3.MaxFinite())
            End Get
        End Property

        'Zero-area box
        Public Sub New(ByVal lo As Vector3)
            Me.lo = lo
            hi = lo
        End Sub

        Public Sub New(ByVal lo As Vector3, ByVal hi As Vector3)
            Me.lo = lo
            Me.hi = hi
        End Sub

        Public ReadOnly Property Low() As Vector3
            Get
                Return lo
            End Get
        End Property

        Public ReadOnly Property High() As Vector3
            Get
                Return hi
            End Get
        End Property

        Public ReadOnly Property Center() As Vector3
            Get
                Return (lo + hi) * 0.5
            End Get
        End Property

        Public ReadOnly Property Area() As Single
            Get
                Dim diag As Vector3 = hi - lo
                Return 2.0F * (diag.x * diag.y + diag.y * diag.z + diag.x * diag.z)
            End Get
        End Property

        Public ReadOnly Property Volume() As Single
            Get
                Dim diag As Vector3 = hi - lo
                Return (diag.x * diag.y * diag.z)
            End Get
        End Property

        Public Function Contains(ByRef point As Vector3) As Boolean
            Return (point.x >= lo.x) AndAlso
                (point.y >= lo.y) AndAlso
                (point.z >= lo.z) AndAlso
                (point.x <= hi.x) AndAlso
                (point.y <= hi.y) AndAlso
                (point.z <= hi.z)
        End Function

        'Returns true if there is any overlap
        Public Function Intersects(ByRef other As AABox) As Boolean
            For a As Integer = 0 To 2
                If lo(a) > other.hi(a) OrElse
                    hi(a) < other.lo(a) Then Return False
            Next
            Return True
        End Function

        'Returns the intersection of both boxes
        Public Function Intersect(ByRef other As AABox) As AABox
            Dim H As Vector3 = hi.Min(other.hi)
            Dim L As Vector3 = lo.Max(other.lo).Min(H)
            Return New AABox(H, L)
        End Function

        Public Sub Split(ByVal axis As Axis, ByVal location As Single, ByRef low As AABox, ByRef high As AABox)
            'Low, medium, and high along the chosen axis
            Dim L As Single = Math.Min(location, lo(axis))
            Dim M As Single = Math.Min(Math.Max(location, lo(axis)), hi(axis))
            Dim H As Single = Math.Max(location, hi(axis))

            'Copy over this box.
            high = New AABox(lo, hi)
            low = New AABox(lo, hi)

            'Now move the split points along the special axis
            low.lo(axis) = L
            low.hi(axis) = M
            high.lo(axis) = M
            high.hi(axis) = H
        End Sub

        Public Overrides Function GetHashCode() As Integer
            Return (lo.GetHashCode() + CLng(hi.GetHashCode())) And &HFFFFFFFF
        End Function

        Public Shared Operator =(ByVal a As AABox, ByVal b As AABox) As Boolean
            Return (a.lo = b.lo) AndAlso (a.hi = b.hi)
        End Operator

        Public Shared Operator <>(ByVal a As AABox, ByVal b As AABox) As Boolean
            Return Not ((a.lo = b.lo) AndAlso (a.hi = b.hi))
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("({0} {1} {2})-({3} {4} {5})", lo.x, lo.y, lo.z, hi.x, hi.y, hi.z)
        End Function

    End Structure

    Public Structure Plane
        'normal.Dot(x,y,z) = distance
        Private _normal As Vector3
        Private _distance As Single

        'Assumes the normal has unit length.
        Public Sub New(ByVal n As Vector3, ByVal d As Single)
            _normal = n
            _distance = d
        End Sub

        Public Sub New(ByVal point0 As Vector3, ByVal point1 As Vector3, ByVal point2 As Vector3)
            _normal = (point1 - point0).Cross(point2 - point0).Direction()
            _distance = _normal.Dot(point0)
        End Sub

        Public Sub New(ByVal normal As Vector3, ByVal point As Vector3)
            _normal = normal
            _distance = _normal.Dot(point)
        End Sub

        Public ReadOnly Property Normal() As Vector3
            Get
                Return _normal
            End Get
        End Property
    End Structure

    Public Structure Triangle
        Private _vertex() As Vector3

        'edgeDirection(i) is the normalized vector v(i+1) - v(i)
        Private edgeDirection() As Vector3
        Private edgeMagnitude() As Double

        Private _plane As Plane
        Private _primaryAxis As Axis

        'vertex[1] - vertex[0]
        Friend edge01 As Vector3

        'vertex[2] - vertex[0]
        Friend edge02 As Vector3

        Private _area As Single

        Private Sub Init(ByVal v0 As Vector3, ByVal v1 As Vector3, ByVal v2 As Vector3)
            _plane = New Plane(v0, v1, v2)
            _vertex(0) = v0
            _vertex(1) = v1
            _vertex(2) = v2

            Dim nxt() As Integer = {1, 2, 0}
            For i As Integer = 0 To 2
                Dim e As Vector3 = _vertex(nxt(i)) - _vertex(i)
                edgeMagnitude(i) = e.Magnitude()

                If edgeMagnitude(i) = 0.0F Then
                    edgeDirection(i) = Vector3.Zero
                Else
                    edgeDirection(i) = e / edgeMagnitude(i)
                End If
            Next

            edge01 = _vertex(1) - _vertex(0)
            edge02 = _vertex(2) - _vertex(0)

            _primaryAxis = _plane.Normal().PrimaryAxis()
            _area = edgeDirection(0).Cross(edgeDirection(2)).Magnitude() * (edgeMagnitude(0) * edgeMagnitude(2))
        End Sub

        Public Sub New(ByVal v0 As Vector3, ByVal v1 As Vector3, ByVal v2 As Vector3)
            _vertex = New Vector3(3) {}
            edgeDirection = New Vector3(3) {}
            edgeMagnitude = New Double(3) {}
            Init(v0, v1, v2)
        End Sub

        Public ReadOnly Property vertex(ByVal n As Integer) As Vector3
            Get
                Return _vertex(n)
            End Get
        End Property

        Public ReadOnly Property Center() As Vector3
            Get
                Return (_vertex(0) + _vertex(1) + _vertex(2)) / 3.0F
            End Get
        End Property
    End Structure

    Public Structure Ray

        Public origin As Vector3
        Public direction As Vector3

        Public Sub New(ByRef origin As Vector3, ByRef direction As Vector3)
            Me.origin = origin
            Me.direction = direction
        End Sub

        Public Function ClosestPoint(ByRef point As Vector3) As Vector3
            Dim t As Single = direction.Dot(point - origin)
            If t < 0.0F Then
                Return origin
            Else
                Return origin + direction * t
            End If
        End Function

        Public Function Distance(ByRef point As Vector3) As Single
            Return (ClosestPoint(point) - point).Magnitude()
        End Function

        'One-sided triangle
        Public Function IntersectionTime(ByVal triangle As Triangle) As Single
            Return IntersectionTime(triangle.vertex(0), triangle.vertex(1), triangle.vertex(2), triangle.edge01, triangle.edge02)
        End Function

        Public Function IntersectionTime(ByVal vert0 As Vector3, ByVal vert1 As Vector3, ByVal vert2 As Vector3, ByVal edge1 As Vector3, ByVal edge2 As Vector3) As Single
            Const EPSILON As Double = 0.000001

            'Barycenteric coords
            Dim u As Single, v As Single

            Dim tvec(2) As Single, pvec(2) As Single, qvec(2) As Single

            'begin calculating determinant - also used to calculate U parameter
            CROSS(pvec, direction, edge2)

            'if determinant is near zero, ray lies in plane of triangle
            Dim det As Single = DOT(edge1, pvec)

            If det < EPSILON Then
                Return Single.PositiveInfinity
            End If

            'calculate distance from vert0 to ray origin
            SUBT(tvec, origin, vert0)

            'calculate U parameter and test bounds
            u = DOT(tvec, pvec)
            If (u < 0.0F) OrElse (u > det) Then
                '// Hit the plane outside the triangle
                Return Single.PositiveInfinity
            End If

            'prepare to test V parameter
            CROSS(qvec, tvec, edge1)

            'calculate V parameter and test bounds
            v = DOT(direction, qvec)
            If (v < 0.0F) OrElse (u + v > det) Then
                'Hit the plane outside the triangle
                Return Single.PositiveInfinity
            End If

            'Case where we don't need correct (u, v):
            Dim t As Single = DOT(edge2, qvec)
            If t >= 0.0F Then
                'Note that det must be positive
                Return t / det
            Else
                'We had to travel backwards in time to intersect
                Return Single.PositiveInfinity
            End If
        End Function

        Private Shared Sub CROSS(ByRef dest() As Single, ByVal v1 As Vector3, ByVal v2 As Vector3)
            dest(0) = v1(1) * v2(2) - v1(2) * v2(1)
            dest(1) = v1(2) * v2(0) - v1(0) * v2(2)
            dest(2) = v1(0) * v2(1) - v1(1) * v2(0)
        End Sub

        Private Shared Sub CROSS(ByRef dest() As Single, ByVal v1() As Single, ByVal v2 As Vector3)
            dest(0) = v1(1) * v2(2) - v1(2) * v2(1)
            dest(1) = v1(2) * v2(0) - v1(0) * v2(2)
            dest(2) = v1(0) * v2(1) - v1(1) * v2(0)
        End Sub

        Private Shared Function DOT(ByVal v1 As Vector3, ByVal v2() As Single) As Single
            Return (v1(0) * v2(0) + v1(1) * v2(1) + v1(2) * v2(2))
        End Function

        Private Shared Function DOT(ByVal v1() As Single, ByVal v2() As Single) As Single
            Return (v1(0) * v2(0) + v1(1) * v2(1) + v1(2) * v2(2))
        End Function

        Private Shared Sub SUBT(ByRef dest() As Single, ByVal v1 As Vector3, ByVal v2 As Vector3)
            dest(0) = v1(0) - v2(0)
            dest(1) = v1(1) - v2(1)
            dest(2) = v1(2) - v2(2)
        End Sub

    End Structure

    Public Function convertPositionToInternalRep(ByVal x As Single, ByVal y As Single, ByVal z As Single) As Vector3
        Dim x2 As Single = x
        Dim y2 As Single = z
        Dim z2 As Single = y
        Dim full As Double = 64.0 * 533.33333333
        Dim mid As Double = full / 2.0
        x2 = full - (x2 + mid)
        z2 = full - (z2 + mid)
        Return New Vector3(z2, y2, x2)
    End Function

    Public Function convertPositionToNormalRep(ByVal x As Single, ByVal y As Single, ByVal z As Single) As Vector3
        Dim x2 As Single = z
        Dim y2 As Single = x
        Dim z2 As Single = y
        Dim full As Double = 64.0 * 533.33333333
        Dim mid As Double = full / 2.0
        x2 = -((mid + x2) - full)
        y2 = -((mid + y2) - full)
        Return New Vector3(x2, y2, z2)
    End Function

    'Collision detection
    Private Function collisionLocationForMovingPointFixedAABox(ByRef origin As Vector3, ByRef dir As Vector3, ByRef box As AABox, ByRef location As Vector3, ByRef Inside As Boolean) As Boolean
        Inside = True
        Dim MinB As Vector3 = box.Low()
        Dim MaxB As Vector3 = box.High()
        Dim MaxT As New Vector3(-1.0F, -1.0F, -1.0F)

        'Find candidate planes.
        For i As Integer = 0 To 2
            If origin(i) < MinB(i) Then
                location(i) = MinB(i)
                Inside = False

                'Calculate T distances to candidate planes
                If dir(i) <> 0.0F Then
                    MaxT(i) = (MinB(i) - origin(i)) / dir(i)
                End If
            ElseIf origin(i) > MaxB(i) Then
                location(i) = MaxB(i)
                Inside = False

                'Calculate T distances to candidate planes
                If dir(i) <> 0.0F Then
                    MaxT(i) = (MaxB(i) - origin(i)) / dir(i)
                End If
            End If
        Next

        If Inside Then 'definite hit
            location = origin
            Return True
        End If

        'Get largest of the maxT's for final choice of intersection
        Dim WhichPlane As Integer = 0
        If MaxT(1) > MaxT(WhichPlane) Then
            WhichPlane = 1
        End If
        If MaxT(2) > MaxT(WhichPlane) Then
            WhichPlane = 2
        End If

        'Check final candidate actually inside box
        If MaxT(WhichPlane) < 0.0F Then
            'Miss the box
            Return False
        End If

        For i As Integer = 0 To 2
            If i <> WhichPlane Then
                location(i) = origin(i) + MaxT(WhichPlane) * dir(i)
                If location(i) < MinB(i) OrElse location(i) > MaxB(i) Then
                    'On this plane we're outside the box extents, so we miss the box
                    Return False
                End If
            End If
        Next

        Return True
    End Function

    Public Sub IntersectionCallBack(ByRef entity As BaseCollision, ByVal ray As Ray, ByVal pStopAtFirstHit As Boolean, ByRef distance As Single)
        entity.Intersect(ray, distance, pStopAtFirstHit)
    End Sub

End Module

#End If