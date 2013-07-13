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
    /// Used to hold vertex information for <see cref="Terrain"/>.
    /// </summary>
    public struct VertexTerrain
    {
        public Vector3 Position;
        public Vector3 Normal;

        public static int SizeInBytes = (3 + 3) * sizeof(float);
        public static VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
            new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),            
        };
    }

    /// <summary>
    /// Used for level-of-detail values throughout the <see cref="Map"/> system.
    /// </summary>
    public enum LOD
    {
        NumOfLODs = 9,
        Minimum = 8,
        Low = 4,
        Med = 2,
        High = 1
    }

    /// <summary>
    /// This is a map containing of a couple of map parts, used for buffering maps.
    /// </summary>
    public class Map : IDisposable
    {
        private LOD[] LevelToLod = new LOD[] { LOD.High, LOD.Med, LOD.Low, LOD.Minimum };
        private int[] LodToLevel = new int[] { 0, 0, 1, 0, 2, 0, 0, 0, 3 };

        private int MapPartSize = 128;
        private float MapPartScale = 1f;

        private Point size;
        private float width;
        private float height;
        private int XStride;

        private Vector3 center;
        private Vector3 position;

        private MapPart[,] parts;

        /// <summary>
        /// Default <see cref="Terrain"/> level-of-detail setting.
        /// </summary>
        public LOD DetailDefault
        {
            get { return this.detailDefault; }
            set { this.detailDefault = value; }
        }
        private LOD detailDefault = LOD.High;

        /// <summary>
        /// Current <see cref="Terrain"/> level-of-detail setting.
        /// </summary>
        public LOD Detail
        {
            get { return this.detail; }
        }
        private LOD detail = LOD.High;

        public string CurrentTechnique
        {
            get { return currentTechnique; }
            set
            {
                currentTechnique = value;
                for (int y = 0; y < parts.GetLength(0); y++)
                    for (int x = 0; x < parts.GetLength(1); x++)
                        parts[y, x].CurrentTechnique = currentTechnique;
            }
        }
        private string currentTechnique = "";

        public Map()
        {
            MapPartSize = 256;
            MapPartScale = 1f;
            currentTechnique = "Wireframed";

            position = Vector3.Zero;
            center = Vector3.Zero;

            Load();
        }

        public void Load()
        {
            parts = new MapPart[1, 1];
            parts[0, 0] = new MapPart(this, Vector3.Zero, MapPartSize + 1, MapPartScale);
            parts[0, 0].LoadMapData("0002829.map");
        }

        public float? Intersects(ref Ray ray, bool exitAtHit)
        {
            float? intersectLength = null;
            for (int y = 0; y < parts.GetLength(0); y++)
            {
                for (int x = 0; x < parts.GetLength(1); x++)
                {
                    float? rayLength;
                    parts[y, x].BoundingBox.Intersects(ref ray, out rayLength);

                    if (rayLength != null)
                    {
                        rayLength = parts[y, x].Intersect(ref ray);

                        if (rayLength != null)
                        {
                            if (exitAtHit)
                                return rayLength;

                            if (intersectLength == null || rayLength < intersectLength)
                                intersectLength = rayLength;
                        }
                    }
                }
            }

            return intersectLength;
        }

        public bool GetHeightAndNormal(Vector3 position, out float height, out Vector3 normal)
        {
            Vector3 min = new Vector3(position.X, -10000f, position.Z);
            Vector3 max = new Vector3(position.X, 10000f, position.Z);
            BoundingBox box = new BoundingBox(min, max);
            for (int y = 0; y < parts.GetLength(0); y++)
            {
                for (int x = 0; x < parts.GetLength(1); x++)
                {
                    if (parts[y, x].BoundingBox.Intersects(box))
                    {
                        Vector3 internalPos = position - parts[y, x].Position;
                        int internalX = (int)internalPos.X;
                        int internalY = (int)internalPos.Z;

                        height = parts[y, x].heightData[internalX, internalY];
                        normal = parts[y, x].normals[internalX, internalY];
                        return true;
                    }
                }
            }

            height = 0f;
            normal = Vector3.Up;
            return false;
        }

        public void Update(GameTime gameTime)
        {
            for (int y = 0; y < parts.GetLength(0); y++)
                for (int x = 0; x < parts.GetLength(1); x++)
                    parts[y, x].Update(gameTime);
        }

        public void Draw(Matrix view, Matrix projection)
        {
            for (int y = 0; y < parts.GetLength(0); y++)
            {
                for (int x = 0; x < parts.GetLength(1); x++)
                {
                    int diffAway = (int)(Vector3.Distance(MapViewer.camera.Position, parts[y, x].Position) / 128f);

                    int qualityLevel = LodToLevel[(int)LOD.High];
                    if (diffAway == 2)
                        qualityLevel += 1;
                    else if (diffAway == 3)
                        qualityLevel += 2;
                    else if (diffAway > 3)
                        qualityLevel += 3;

                    if (qualityLevel > 3)
                        qualityLevel = 3;

                    LOD usedQuality = LevelToLod[qualityLevel];

                    parts[y, x].Draw(view, projection, usedQuality);
                }
            }
        }

        public void Dispose()
        {
            for (int y = 0; y < parts.GetLength(0); y++)
                for (int x = 0; x < parts.GetLength(1); x++)
                    parts[y, x].Dispose();
        }
    }
}
