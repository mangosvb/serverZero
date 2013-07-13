using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MapViewer.Extra;

namespace MapViewer.Core
{

    /// <summary>
    /// This is a small part of a larger map, used for buffered mapping.
    /// </summary>
    public class MapPart : IDisposable
    {
        private int size;
        private float scale;

        /// <summary>
        /// The parent map this part is attached to.
        /// </summary>
        private Map parentMap;

        /// <summary>
        /// Retrieves the position of this map part.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
        }
        private Vector3 position;

        /// <summary>
        /// Retrieves the bounding box of this map part.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get { return quadTree.BoundingBox; }
        }

        /// <summary>
        /// Holds map data in a quad tree.
        /// </summary>
        private QuadTree quadTree;

        /// <summary>
        /// HLSL Effect for terrain rendering.
        /// </summary>
        private Effect terrainEffect;

        public string CurrentTechnique
        {
            get { return terrainEffect.CurrentTechnique.Name; }
            set { terrainEffect.CurrentTechnique = terrainEffect.Techniques[value]; }
        }

        /// <summary>
        /// Retrieves the <see cref="VertexBuffer"/> associated with this map part.
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get { return this.vertexBuffer; }
        }

        /// <summary>
        /// Holds entire <see cref="VertexBuffer"/> for this entire map part section.
        /// </summary>
        private VertexBuffer vertexBuffer;

        /// <summary>
        /// Retrieves the <see cref="VertexDeclaration"/> associated with this map part.
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get { return this.vertDeclaration; }
        }

        /// <summary>
        /// The vertex declaration for the vertex type for this map part.
        /// </summary>
        private VertexDeclaration vertDeclaration;

        /// <summary>
        /// Holds vertices for entire <see cref="MapPart"/> section until all <see cref="QuadTree"/> sections and
        /// <see cref="TerrainPatch"/> sections have finished loading, and then this list is released.
        /// </summary>
        public List<VertexTerrain> VertexList
        {
            get { return this.vertexList; }
            set
            {
                this.vertexList = value;
            }
        }
        private List<VertexTerrain> vertexList;

        /// <summary>
        /// Holds the height (Y coordinate) of each [x, z] coordinate. Height data is loaded from a heightmap image.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public float[,] heightData;

        /// <summary>
        /// Holds information about the texture type, which is stored in each vertex after the vertex buffers 
        /// are created.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public Color[,] textureTypeData;

        /// <summary>
        /// Holds the normal vectors for each vertex in the terrain.
        /// The normals for lighting are later stored in each vertex, but
        /// we want to store these values permanentally for proper physics
        /// collisions with the ground.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public Vector3[,] normals;

        /// <summary>
        /// Holds information about billboards in on the terrain.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public int[,] billboardData;

        /// <summary>
        /// Triangles used for the ray intersection detection.
        /// </summary>
        private Tri[] triangle;
        public struct Tri
        {
            public int id;
            public Vector3 p1;
            public Vector3 p2;
            public Vector3 p3;
            public Vector3 normal;
        }

        /// <summary>
        /// Terrainmap image which is used to determine terrain texture splatting.
        /// </summary>
        /// <remarks>Can be set to <see cref="null"/> after <see cref="terrainTypeData"/> is created</remarks>
        private Texture2D terrainMap;

        /// <summary>
        /// Holds the different textures used for this <see cref="Terrain"/> section.
        /// </summary>
        private Texture2D[] terrainTextures;

        /// <summary>
        /// Holds the different texture normal map images used for this <see cref="Terrain"/> section.
        /// </summary>
        private Texture2D[] terrainTextureNormals;

        /// <summary>
        /// Index buffer for this patch. As an array it can hold an index buffer for each LOD.
        /// </summary>
        private IndexBuffer[] indexBuffers;

        /// <summary>
        /// Holds the number of triangles in this patch. As an array it can hold triangle information
        /// for multiple LODs for this patch.
        /// </summary>
        private int[] numTris;

        public MapPart(Map parentMap, Vector3 position, int size, float scale)
        {
            this.parentMap = parentMap;
            this.position = position;
            this.size = size;
            this.scale = scale;

            this.terrainEffect = MapViewer.contentManager.Load<Effect>("Effects/Terrain");
            this.terrainEffect.CurrentTechnique = this.terrainEffect.Techniques[this.parentMap.CurrentTechnique];

            this.vertexList = new List<VertexTerrain>();
            this.vertDeclaration = new VertexDeclaration(MapViewer.graphics.GraphicsDevice, VertexTerrain.VertexElements);
            this.indexBuffers = new IndexBuffer[(int)LOD.NumOfLODs];
            this.numTris = new int[(int)LOD.NumOfLODs];

            terrainTextures = new Texture2D[4];
            terrainTextureNormals = new Texture2D[4];

            /*
            Vector3 min = new Vector3(position.X, 0f, position.Z);
            Vector3 max = new Vector3(position.X + (float)(size - 1), 0f, position.Z + (float)(size - 1));
            
            triangle = new Tri[2];
            triangle[0].id = 0;
            triangle[0].p1 = min;
            triangle[0].p2 = new Vector3(min.X, min.Y, max.Z);
            triangle[0].p3 = new Vector3(max.X, min.Y, min.Z);
            triangle[0].normal = MathExtra.GetNormal(triangle[0].p1, triangle[0].p2, triangle[0].p3);

            triangle[1].id = 1;
            triangle[1].p1 = new Vector3(max.X, min.Y, min.Z);
            triangle[1].p2 = new Vector3(min.X, min.Y, max.Z);
            triangle[1].p3 = max;
            triangle[1].normal = MathExtra.GetNormal(triangle[1].p1, triangle[1].p2, triangle[1].p3);
             */
        }

        public void LoadMapData(string mapFile)
        {
            this.heightData = new float[size, size];

            FileStream fs = new FileStream("maps\\" + mapFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 82704, FileOptions.SequentialScan);
            BinaryReader br = new BinaryReader(fs);

            // AreaFlags
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                    br.ReadUInt16();

            // AreaTerrain
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                    br.ReadByte();

            // WaterLevel
            for (int x = 0; x < 128; x++)
                for (int y = 0; y < 128; y++)
                    br.ReadSingle();

            for (int x = 0; x < MapViewer.mapResolution; x++)
                for (int y = 0; y < MapViewer.mapResolution; y++)
                    this.heightData[x, y] = 0f; //this.heightData[x, y] = br.ReadSingle();

            br.Close();
            fs.Close();
            fs.Dispose();

            SetupNormals();
            SetupTerrainIndices(LOD.Minimum);
            SetupTerrainIndices(LOD.Low);
            SetupTerrainIndices(LOD.Med);
            SetupTerrainIndices(LOD.High);
            SetupTerrainVertices();
            SetupTerrainVertexBuffer();
            SetUpCollision();

            this.quadTree = new QuadTree(null, position, size - 1, triangle, 0, 0);

            InitEffects();
        }

        private void SetupNormals()
        {
            VertexTerrain[] terrainVertices = new VertexTerrain[this.size * this.size];
            this.normals = new Vector3[this.size, this.size];

            // Determine vertex positions so we can figure out normals in section below.
            for (int x = 0; x < this.size; ++x)
                for (int z = 0; z < this.size; ++z)
                {
                    terrainVertices[x + z * this.size].Position = new Vector3(x * this.scale, this.heightData[x, z], z * this.scale);
                }

            // Setup normals for lighting and physics (Credit: Riemer's method)
            int sizeMinusOne = this.size - 1;
            for (int x = 1; x < sizeMinusOne; ++x)
                for (int z = 1; z < sizeMinusOne; ++z)
                {
                    int ZTimesSize = (z * this.size);
                    Vector3 normX = new Vector3((terrainVertices[x - 1 + ZTimesSize].Position.Y - terrainVertices[x + 1 + ZTimesSize].Position.Y) / 2, 1, 0);
                    Vector3 normZ = new Vector3(0, 1, (terrainVertices[x + (z - 1) * this.size].Position.Y - terrainVertices[x + (z + 1) * this.size].Position.Y) / 2);

                    // We inline the normalize method here since it is used alot, this is faster than calling Vector3.Normalize()
                    Vector3 normal = normX + normZ;
                    float length = (float)Math.Sqrt((float)((normal.X * normal.X) + (normal.Y * normal.Y) + (normal.Z * normal.Z)));
                    float num = 1f / length;
                    normal.X *= num;
                    normal.Y *= num;
                    normal.Z *= num;

                    this.normals[x, z] = terrainVertices[x + ZTimesSize].Normal = normal;    // Stored for use in physics and for the
                    // quad-tree component to reference.
                }
        }

        public void SetupNormal(int x, int z)
        {
            if (x < 1 || z < 1)
                return;

            int ZTimesSize = (x * this.size);
            Vector3 normX = new Vector3((vertexList[x - 1 + ZTimesSize].Position.Y - vertexList[x + 1 + ZTimesSize].Position.Y) / 2, 1, 0);
            Vector3 normZ = new Vector3(0, 1, (vertexList[x + (z - 1) * this.size].Position.Y - vertexList[x + (z + 1) * this.size].Position.Y) / 2);

            // We inline the normalize method here since it is used alot, this is faster than calling Vector3.Normalize()
            Vector3 normal = normX + normZ;
            float length = (float)Math.Sqrt((float)((normal.X * normal.X) + (normal.Y * normal.Y) + (normal.Z * normal.Z)));
            float num = 1f / length;
            normal.X *= num;
            normal.Y *= num;
            normal.Z *= num;

            this.normals[x, z] = normal;    // Stored for use in physics and for the
            // quad-tree component to reference.
        }

        private void SetupTerrainIndices(LOD detailLevel)
        {
            int width = size;
            int detail = (int)detailLevel;
            int widthMinusOne = (width - 1);

            // If detail level is smaller than the quad patch, then move up to
            // the next highest detail level.
            if (detail >= widthMinusOne)
            {
                detail /= 2;
            }

            int widthMinusOneDivDetail = widthMinusOne / detail;
            int[] indices = new int[widthMinusOne * widthMinusOne * 6 / (detail * detail)];

            for (int x = 0; x < widthMinusOneDivDetail; ++x)
                for (int y = 0; y < widthMinusOneDivDetail; ++y)
                {
                    int triStartIndex = (x + y * widthMinusOneDivDetail) * 6;
                    int xPlusOne = x + 1;
                    int yTimesWidth = y * width;
                    int yPlusOneTimesWidth = (y + 1) * width;

                    indices[triStartIndex] = (xPlusOne + yPlusOneTimesWidth) * detail;
                    indices[triStartIndex + 1] = (xPlusOne + yTimesWidth) * detail;
                    indices[triStartIndex + 2] = (x + yTimesWidth) * detail;

                    indices[triStartIndex + 3] = indices[triStartIndex];
                    indices[triStartIndex + 4] = indices[triStartIndex + 2];
                    indices[triStartIndex + 5] = (x + yPlusOneTimesWidth) * detail;
                }

            int widthMinusDetail = (width - detail);
            indexBuffers[detail] = new IndexBuffer(MapViewer.graphics.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffers[detail].SetData(indices);

            numTris[detail] = indices.Length / 3;
        }

        private void SetUpCollision()
        {
            triangle = new Tri[(size - 1) * (size - 1) * 2];

            for (int y = 0; y < size - 1; y++)
            {
                for (int x = 0; x < size - 1; x++)
                {
                    int tID = (x + y * (size - 1)) * 2;
                    SetUpCollision(tID, x, y);
                }
            }
        }

        public void SetUpCollision(int tID, int x, int y)
        {
            triangle[tID] = new Tri();
            triangle[tID].p1 = GetPosition(x, y);
            triangle[tID].p2 = GetPosition(x, y + 1);
            triangle[tID].p3 = GetPosition(x + 1, y);
            triangle[tID].normal = MathExtra.GetNormal(triangle[tID].p1, triangle[tID].p2, triangle[tID].p3);
            triangle[tID].id = tID;

            triangle[tID + 1] = new Tri();
            triangle[tID + 1].p1 = GetPosition(x + 1, y);
            triangle[tID + 1].p2 = GetPosition(x, y + 1);
            triangle[tID + 1].p3 = GetPosition(x + 1, y + 1);
            triangle[tID + 1].normal = MathExtra.GetNormal(triangle[tID + 1].p1, triangle[tID + 1].p2, triangle[tID + 1].p3);
            triangle[tID + 1].id = tID + 1;
        }

        public void UpdateQuadTree()
        {
            this.quadTree.SetupNode(triangle);
        }

        private Vector3 GetPosition(int x, int y)
        {
            return position + new Vector3((float)x * scale, heightData[x, y], (float)y * scale);
        }

        private void SetupTerrainVertices()
        {
            vertexList.Clear();

            // Texture the level
            for (int x = 0; x < size; x++)
                for (int z = 0; z < size; z++)
                {
                    VertexTerrain tempVert = new VertexTerrain();
                    float offsetXtotal = (float)x;
                    float offsetZtotal = (float)z;
                    tempVert.Position = new Vector3(offsetXtotal * scale,
                                                    heightData[x, z],
                                                    offsetZtotal * scale);

                    tempVert.Normal = normals[x, z];

                    this.vertexList.Add(tempVert);
                }
        }

        public void SetupTerrainVertice(int x, int z)
        {
            VertexTerrain tempVert = new VertexTerrain();
            float offsetXtotal = (float)x;
            float offsetZtotal = (float)z;
            tempVert.Position = new Vector3(offsetXtotal * scale,
                                            heightData[x, z],
                                            offsetZtotal * scale);

            tempVert.Normal = normals[x, z];

            this.vertexList[x * size + z] = tempVert;
        }

        public void SetupTerrainVertexBuffer()
        {
            VertexTerrain[] verticesArray = new VertexTerrain[vertexList.Count];

            this.vertexList.CopyTo(verticesArray);

            this.vertexBuffer = new VertexBuffer(MapViewer.graphics.GraphicsDevice, VertexTerrain.SizeInBytes * vertexList.Count, BufferUsage.WriteOnly);
            this.vertexBuffer.SetData(verticesArray);
        }

        public float? Intersect(ref Ray ray)
        {
            List<int> triangles = new List<int>();
            quadTree.GetTriangles(ref ray, ref triangles);

            float rayLength;
            foreach (int triID in triangles)
            {
                Tri tri = triangle[triID];
                if (MathExtra.Intersects(ray, tri.p1, tri.p2, tri.p3, tri.normal, false, true, out rayLength))
                    return rayLength;
            }

            return null;
        }

        private void InitEffects()
        {
            terrainEffect.Parameters["TextureMap"].SetValue(terrainMap);
            terrainEffect.Parameters["GrassTexture"].SetValue(terrainTextures[0]);
            terrainEffect.Parameters["RockTexture"].SetValue(terrainTextures[1]);
            terrainEffect.Parameters["SandTexture"].SetValue(terrainTextures[2]);

            terrainEffect.Parameters["GrassNormal"].SetValue(terrainTextureNormals[0]);
            terrainEffect.Parameters["RockNormal"].SetValue(terrainTextureNormals[1]);
            terrainEffect.Parameters["SandNormal"].SetValue(terrainTextureNormals[2]);

            terrainEffect.Parameters["TerrainScale"].SetValue(1f); // Must be a power of 2 (1, 2, 4, 8, 16...)
            terrainEffect.Parameters["TerrainWidth"].SetValue(256f); // Must be a power of 2 (1, 2, 4, 8, 16...)

            terrainEffect.Parameters["LightDirection"].SetValue(new Vector4(0.6f, -0.2f, -0.6f, 0.0f));
            terrainEffect.Parameters["SpecularColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
            terrainEffect.Parameters["SpecularPower"].SetValue(4f);
            terrainEffect.Parameters["AmbientPower"].SetValue(1f);
            terrainEffect.Parameters["AmbientColor"].SetValue(new Vector4(0.2f, 0.2f, 0.6f, 1.0f));
            terrainEffect.Parameters["DiffuseColor"].SetValue(new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            terrainEffect.Parameters["fogNear"].SetValue(230f);
            terrainEffect.Parameters["fogFar"].SetValue(560f);
            terrainEffect.Parameters["fogAltitudeScale"].SetValue(10f);
            terrainEffect.Parameters["fogThinning"].SetValue(10f);
            terrainEffect.Parameters["fogColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
        }

        private void UpdateEffects()
        {

        }

        public void Update(GameTime gameTime)
        {
            UpdateEffects();
        }

        public void Draw(Matrix view, Matrix projection, LOD detailLevel)
        {
            int detail = (int)detailLevel;
            int vertexCount = size * size;

            MapViewer.graphics.GraphicsDevice.Indices = indexBuffers[detail];
            MapViewer.graphics.GraphicsDevice.VertexDeclaration = vertDeclaration;
            MapViewer.graphics.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertDeclaration.GetVertexStrideSize(0));

            Matrix world = Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up);
            Matrix worldView = world * view;
            Matrix worldViewProjection = worldView * projection;

            terrainEffect.Parameters["View"].SetValue(view);
            terrainEffect.Parameters["Projection"].SetValue(projection);
            terrainEffect.Parameters["World"].SetValue(world);
            terrainEffect.Parameters["WorldViewProj"].SetValue(worldViewProjection);

            terrainEffect.Parameters["CameraPos"].SetValue(new Vector4(MapViewer.camera.Position, 0f));
            terrainEffect.Parameters["CameraForward"].SetValue(new Vector4(view.Forward, 0f));

            terrainEffect.Begin();

            for (int i = 0; i < terrainEffect.CurrentTechnique.Passes.Count; ++i)
            {
                terrainEffect.CurrentTechnique.Passes[i].Begin();

                MapViewer.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, numTris[detail]);

                terrainEffect.CurrentTechnique.Passes[i].End();
            }

            terrainEffect.End();

            MapViewer.graphics.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            MapViewer.graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }

        public void Dispose()
        {

        }
    }
}
