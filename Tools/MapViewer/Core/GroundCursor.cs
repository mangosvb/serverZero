using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapViewer.Core
{
    public class GroundCursor
    {
        public enum CursorShape
        {
            Circle = 0,
            Square = 1,
            Triangle = 2
        }

        /// <summary>
        /// The visibility state of the cursor.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        private bool visible = false;

        /// <summary>
        /// The position of the cursor.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
        }
        private Vector3 position = Vector3.Zero;

        /// <summary>
        /// The size of the cursor.
        /// </summary>
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                UpdateCursor();
            }
        }
        private int size = 50;

        public CursorShape Shape
        {
            get { return shape; }
            set
            {
                shape = value;
                this.cursorEffect.Parameters["Shape"].SetValue((int)shape);
            }
        }
        private CursorShape shape = CursorShape.Circle;

        private float highestPoint = 0f;
        private float lowestPoint = 0f;

        /// <summary>
        /// HLSL Effect for terrain rendering.
        /// </summary>
        private Effect cursorEffect;

        /// <summary>
        /// Holds entire <see cref="VertexBuffer"/> for this cursor.
        /// </summary>
        private VertexBuffer vertexBuffer;

        /// <summary>
        /// The vertex declaration for the vertex type for this cursor.
        /// </summary>
        private VertexDeclaration vertDeclaration;

        /// <summary>
        /// Holds vertices for this cursor.
        /// </summary>
        private List<VertexTerrain> vertexList;

        /// <summary>
        /// Index buffer for this cursor.
        /// </summary>
        private IndexBuffer indexBuffer;

        /// <summary>
        /// Holds the number of triangles in this cursor.
        /// </summary>
        private int numTris;

        /// <summary>
        /// Holds the height (Y coordinate) of each [x, z] coordinate.
        /// </summary>
        private float[,] heightData;

        /// <summary>
        /// Holds the normal vectors for each vertex.
        /// </summary>
        private Vector3[,] normals;

        public GroundCursor()
        {
            this.cursorEffect = MapViewer.contentManager.Load<Effect>("Effects/Cursor");
            this.cursorEffect.CurrentTechnique = this.cursorEffect.Techniques["Cursor"];
            this.cursorEffect.Parameters["Shape"].SetValue((int)shape);

            this.vertexList = new List<VertexTerrain>();
            this.vertDeclaration = new VertexDeclaration(MapViewer.graphics.GraphicsDevice, VertexTerrain.VertexElements);

            UpdateCursor();
        }

        private void UpdateCursor() // Call this if size or position is changed
        {
            this.heightData = new float[size, size];
            this.normals = new Vector3[size, size];
            UpdateHeightData();

            SetupTerrainIndices();
            SetupTerrainVertices();
            SetupTerrainVertexBuffer();
        }

        private void UpdateHeightData()
        {
            if (MapViewer.map == null)
                return;

            Vector3 pointPos;
            float height;
            Vector3 normal;

            Vector3 refPoint = position - new Vector3((float)size / 2f, 0f, (float)size / 2f);
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    pointPos = refPoint + new Vector3((float)x, 0f, (float)z);
                    MapViewer.map.GetHeightAndNormal(pointPos, out height, out normal);
                    height += 1f;
                    heightData[x, z] = height;
                    normals[x, z] = normal;

                    if (height > highestPoint)
                        highestPoint = height;
                    else if (height < lowestPoint)
                        lowestPoint = height;
                }
            }
        }

        private void SetupTerrainIndices()
        {
            int width = size;
            int widthMinusOne = (width - 1);

            int[] indices = new int[widthMinusOne * widthMinusOne * 6];

            for (int x = 0; x < widthMinusOne; ++x)
                for (int y = 0; y < widthMinusOne; ++y)
                {
                    int triStartIndex = (x + y * widthMinusOne) * 6;
                    int xPlusOne = x + 1;
                    int yTimesWidth = y * width;
                    int yPlusOneTimesWidth = (y + 1) * width;

                    indices[triStartIndex] = (xPlusOne + yPlusOneTimesWidth);
                    indices[triStartIndex + 1] = (xPlusOne + yTimesWidth);
                    indices[triStartIndex + 2] = (x + yTimesWidth);

                    indices[triStartIndex + 3] = indices[triStartIndex];
                    indices[triStartIndex + 4] = indices[triStartIndex + 2];
                    indices[triStartIndex + 5] = (x + yPlusOneTimesWidth);
                }

            indexBuffer = new IndexBuffer(MapViewer.graphics.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            numTris = indices.Length / 3;
        }

        private void SetupTerrainVertices()
        {
            vertexList.Clear();

            // Texture the level
            Vector3 refPoint = new Vector3((float)size / 2f, 0f, (float)size / 2f);
            for (int x = 0; x < size; ++x)
                for (int z = 0; z < size; ++z)
                {
                    VertexTerrain tempVert = new VertexTerrain();
                    float offsetXtotal = (float)x - refPoint.X;
                    float offsetZtotal = (float)z - refPoint.Z;
                    tempVert.Position = new Vector3(offsetXtotal,
                                                    heightData[x, z],
                                                    offsetZtotal);

                    tempVert.Normal = normals[x, z];

                    this.vertexList.Add(tempVert);
                }
        }

        private void SetupTerrainVertexBuffer()
        {
            VertexTerrain[] verticesArray = new VertexTerrain[vertexList.Count];

            this.vertexList.CopyTo(verticesArray);

            this.vertexBuffer = new VertexBuffer(MapViewer.graphics.GraphicsDevice, VertexTerrain.SizeInBytes * vertexList.Count, BufferUsage.WriteOnly);
            this.vertexBuffer.SetData(verticesArray);
        }

        public void Update()
        {
            Ray mouseRay = GetMouseRay();
            if (mouseRay.Position == Vector3.Zero && mouseRay.Direction == Vector3.Zero)
                return;

            float? rayLength = MapViewer.map.Intersects(ref mouseRay, false);
            if (rayLength == null)
                return;

            Vector3 rayTarget = mouseRay.Position + mouseRay.Direction * (float)rayLength;
            position = rayTarget;

            //UpdateCursor();
        }

        private Ray GetMouseRay()
        {
            MouseState ms = Mouse.GetState();
            int x = ms.X;
            int y = ms.Y;
            float width = (float)MapViewer.graphics.GraphicsDevice.Viewport.Width;
            float height = (float)MapViewer.graphics.GraphicsDevice.Viewport.Height;
            Matrix invView = Matrix.Invert(MapViewer.camera.ViewMatrix);

            double screenSpaceX = ((float)x / (width / 2) - 1.0f) * MapViewer.camera.aspectRatio;
            double screenSpaceY = (1.0f - (float)y / (height / 2));

            double viewRatio = Math.Tan(MapViewer.camera.fov / 2);

            screenSpaceX = screenSpaceX * viewRatio;
            screenSpaceY = screenSpaceY * viewRatio;

            Vector3 cameraSpaceNear = new Vector3((float)(screenSpaceX * MapViewer.camera.NearPlane), (float)(screenSpaceY * MapViewer.camera.NearPlane), (float)(-MapViewer.camera.NearPlane));
            Vector3 cameraSpaceFar = new Vector3((float)(screenSpaceX * MapViewer.camera.FarPlane), (float)(screenSpaceY * MapViewer.camera.FarPlane), (float)(-MapViewer.camera.FarPlane));

            Vector3 worldSpaceNear = Vector3.Transform(cameraSpaceNear, invView);
            Vector3 worldSpaceFar = Vector3.Transform(cameraSpaceFar, invView);

            return new Ray(worldSpaceNear, Vector3.Normalize(worldSpaceFar - worldSpaceNear));
        }

        public void Draw(Matrix view, Matrix projection)
        {
            if (!visible)
                return;

            int vertexCount = size * size;

            MapViewer.graphics.GraphicsDevice.Indices = indexBuffer;
            MapViewer.graphics.GraphicsDevice.VertexDeclaration = vertDeclaration;
            MapViewer.graphics.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertDeclaration.GetVertexStrideSize(0));

            MapViewer.graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            Matrix world = Matrix.CreateWorld(new Vector3(position.X, 0f, position.Z), Vector3.Forward, Vector3.Up);

            cursorEffect.Parameters["View"].SetValue(view);
            cursorEffect.Parameters["Projection"].SetValue(projection);
            cursorEffect.Parameters["World"].SetValue(world);

            cursorEffect.Begin();

            for (int i = 0; i < cursorEffect.CurrentTechnique.Passes.Count; ++i)
            {
                cursorEffect.CurrentTechnique.Passes[i].Begin();

                MapViewer.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, numTris);

                cursorEffect.CurrentTechnique.Passes[i].End();
            }

            cursorEffect.End();

            MapViewer.graphics.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            MapViewer.graphics.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            MapViewer.graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }
    }
}
