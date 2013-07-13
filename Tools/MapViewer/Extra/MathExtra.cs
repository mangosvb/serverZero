using System;
using Microsoft.Xna.Framework;

namespace MapViewer.Extra
{
    public class MathExtra
    {
        /// <summary>
        /// Checks if a integer is a power-of-two value (i.e. 2, 4, 8, 16, etc...)
        /// </summary>
        /// <param name="Value">Value to check for power-of-two</param>
        /// <returns>True if number is a power-of-two, otherwise returns false</returns>
        public static bool IsPowerOfTwo(int Value)
        {
            if (Value < 2)
            {
                return false;
            }
            else if ((Value & (Value - 1)) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the normal from three referencepoints.
        /// </summary>
        /// <param name="p1">Point one.</param>
        /// <param name="p2">Point two.</param>
        /// <param name="p3">Point three.</param>
        /// <returns>The normal.</returns>
        public static Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 v1 = p2 - p1;
            Vector3 v2 = p1 - p3;

            Vector3 norm = Vector3.Cross(v1, v2);
            norm.Normalize();

            return norm;
        }

        public static bool Intersects(Ray ray, Vector3 a, Vector3 b, Vector3 c, Vector3 normal, bool positiveSide, bool negativeSide, out float t)
        {
            t = 0;
            {
                float denom = Vector3.Dot(normal, ray.Direction);

                if (denom > float.Epsilon)
                {
                    if (!negativeSide)
                        return false;
                }
                else if (denom < -float.Epsilon)
                {
                    if (!positiveSide)
                        return false;
                }
                else
                {
                    return false;
                }

                t = Vector3.Dot(normal, a - ray.Position) / denom;

                if (t < 0)
                {
                    // Interersection is behind origin
                    return false;
                }
            }

            // Calculate the largest area projection plane in X, Y or Z.
            int i0, i1;
            {
                float n0 = Math.Abs(normal.X);
                float n1 = Math.Abs(normal.Y);
                float n2 = Math.Abs(normal.Z);

                i0 = 1;
                i1 = 2;

                if (n1 > n2)
                {
                    if (n1 > n0) i0 = 0;
                }
                else
                {
                    if (n2 > n0) i1 = 0;
                }
            }

            float[] A = { a.X, a.Y, a.Z };
            float[] B = { b.X, b.Y, b.Z };
            float[] C = { c.X, c.Y, c.Z };
            float[] R = { ray.Direction.X, ray.Direction.Y, ray.Direction.Z };
            float[] RO = { ray.Position.X, ray.Position.Y, ray.Position.Z };

            // Check the intersection point is inside the triangle.
            {
                float u1 = B[i0] - A[i0];
                float v1 = B[i1] - A[i1];
                float u2 = C[i0] - A[i0];
                float v2 = C[i1] - A[i1];
                float u0 = t * R[i0] + RO[i0] - A[i0];
                float v0 = t * R[i1] + RO[i1] - A[i1];

                float alpha = u0 * v2 - u2 * v0;
                float beta = u1 * v0 - u0 * v1;
                float area = u1 * v2 - u2 * v1;

                float EPSILON = 1e-3f;

                float tolerance = EPSILON * area;

                if (area > 0)
                {
                    if (alpha < tolerance || beta < tolerance || alpha + beta > area - tolerance)
                        return false;
                }
                else
                {
                    if (alpha > tolerance || beta > tolerance || alpha + beta < area - tolerance)
                        return false;
                }
            }

            return true;
        }
    }
}
