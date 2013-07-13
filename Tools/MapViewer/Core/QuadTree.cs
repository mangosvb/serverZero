using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapViewer.Core
{
    public class QuadTree
    {
        private const int PreferredSize = 16;

        private QuadTree parent;
        private Vector3 position;
        private int size;

        private List<QuadTree> nodes = null;
        private BoundingBox boundingBox;
        private List<int> triangles = null;

        private int startX;
        private int startY;

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
        }

        // TODO: Map scale?!
        public QuadTree(QuadTree parent, Vector3 position, int size, MapPart.Tri[] triangle, int startX, int startY)
        {
            this.parent = parent;
            this.position = position;
            this.size = size;
            this.startX = startX;
            this.startY = startY;

            // Sets the bounding box, note that the highest and lowest point is set later on
            Vector3 min = new Vector3(position.X, position.Y, position.Z);
            Vector3 max = new Vector3(position.X + (float)size, position.Y, position.Z + (float)size);
            this.boundingBox = new BoundingBox(min, max);

            SetupNode(triangle);
        }

        public void SetupNode(MapPart.Tri[] triangle)
        {
            float highestPoint = 0f;
            float lowestPoint = 0f;

            if (size > PreferredSize)
            {
                nodes = new List<QuadTree>(4);

                int nodeSize = size / 2;
                for (int i = 0; i < 4; i++)
                {
                    int nodeX = i % 2;
                    int nodeY = i / 2;
                    Vector3 nodePos = new Vector3(position.X + (float)(nodeSize * nodeX),
                                                    position.Y,
                                                    position.Z + (float)(nodeSize * nodeY));

                    QuadTree newNode = new QuadTree(this, nodePos, nodeSize, triangle, startX + (nodeSize * nodeX), startY + (nodeSize * nodeY));
                    nodes.Add(newNode);

                    if (newNode.BoundingBox.Max.Y > highestPoint)
                        highestPoint = newNode.BoundingBox.Max.Y;

                    if (newNode.BoundingBox.Min.Y < lowestPoint)
                        lowestPoint = newNode.BoundingBox.Min.Y;
                }

                this.boundingBox.Min.Y = lowestPoint;
                this.boundingBox.Max.Y = highestPoint;
            }
            else
            {
                triangles = new List<int>();

                int mapSize = GetSize();
                for (int y = startY; y < (startY + size); y++)
                {
                    for (int x = startX; x < (startX + size); x++)
                    {
                        int tID = (x + y * mapSize) * 2;

                        if (tID < triangle.Length)
                        {
                            triangles.Add(tID);
                            triangles.Add(tID + 1);

                            if (triangle[tID].p1.Y > highestPoint)
                                highestPoint = triangle[tID].p1.Y;
                            if (triangle[tID].p2.Y > highestPoint)
                                highestPoint = triangle[tID].p2.Y;
                            if (triangle[tID].p3.Y > highestPoint)
                                highestPoint = triangle[tID].p3.Y;

                            if (triangle[tID].p1.Y < lowestPoint)
                                lowestPoint = triangle[tID].p1.Y;
                            if (triangle[tID].p2.Y < lowestPoint)
                                lowestPoint = triangle[tID].p2.Y;
                            if (triangle[tID].p3.Y < lowestPoint)
                                lowestPoint = triangle[tID].p3.Y;

                            if (triangle[tID + 1].p1.Y > highestPoint)
                                highestPoint = triangle[tID + 1].p1.Y;
                            if (triangle[tID + 1].p2.Y > highestPoint)
                                highestPoint = triangle[tID + 1].p2.Y;
                            if (triangle[tID + 1].p3.Y > highestPoint)
                                highestPoint = triangle[tID + 1].p3.Y;

                            if (triangle[tID + 1].p1.Y < lowestPoint)
                                lowestPoint = triangle[tID + 1].p1.Y;
                            if (triangle[tID + 1].p2.Y < lowestPoint)
                                lowestPoint = triangle[tID + 1].p2.Y;
                            if (triangle[tID + 1].p3.Y < lowestPoint)
                                lowestPoint = triangle[tID + 1].p3.Y;
                        }
                    }
                }

                this.boundingBox.Min.Y = lowestPoint;
                this.boundingBox.Max.Y = highestPoint;
            }
        }

        public void GetTriangles(ref Ray ray, ref List<int> triangleList)
        {
            float? rayLength;
            boundingBox.Intersects(ref ray, out rayLength);

            if (rayLength != null)
            {
                if (nodes != null)
                {
                    for (int i = 0; i < 4; i++)
                        nodes[i].GetTriangles(ref ray, ref triangleList);
                }
                else if (triangles != null)
                {
                    triangleList.AddRange(triangles);
                }
            }
        }

        public int GetSize()
        {
            if (parent == null) return size;
            return parent.GetSize();
        }

    }
}
