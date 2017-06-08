using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege
{
    class ParabolicCurveHelper
    {
        private static List<Vector2> getParabolicPoints(double distance, double precision)
        {
            const double g = 10.0;
            const double angle = 45.0;

            List<Vector2> pointsList = new List<Vector2>();

            double v = Math.Sqrt((distance * g) / Math.Sin(2 * angle));

            for (double x = 0; x < distance; x += precision)
            {
                double y = (Math.Tan(angle) * x) - (g / (2 * Math.Pow(v,2) * Math.Pow(Math.Cos(distance),2)) * Math.Pow(x, 2));
                pointsList.Add(new Vector2((float)x, (float)y));
            }

            return pointsList;
        }

        //Used for generating the mesh for the curve
        //First object is vertex data, second is indices (both as arrays)
        public static object[] computeCurve3D()
        {
            List<VertexPositionTexture> path = new List<VertexPositionTexture>();
            List<int> indices = new List<int>();

            List<Vector2> curvePoints = new List<Vector2>();
            curvePoints = getParabolicPoints(6, 1.0);

            float curveWidth = 0.003f;

            for (int x = 0; x < curvePoints.Count; x++)
            {
                Vector2 normal;

                if (x == 0)
                {
                    //First point, Take normal from first line segment
                    normal = getNormalizedVector(getLineNormal(curvePoints[x + 1] - curvePoints[x]));
                }
                else if (x + 1 == curvePoints.Count)
                {
                    //Last point, take normal from last line segment
                    normal = getNormalizedVector(getLineNormal(curvePoints[x] - curvePoints[x - 1]));
                }
                else
                {
                    //Middle point, interpolate normals from adjacent line segments
                    normal = getNormalizedVertexNormal(getLineNormal(curvePoints[x] - curvePoints[x - 1]), getLineNormal(curvePoints[x + 1] - curvePoints[x]));
                }

                path.Add(new VertexPositionTexture(new Vector3(curvePoints[x] + normal * curveWidth, 0), new Vector2()));
                path.Add(new VertexPositionTexture(new Vector3(curvePoints[x] + normal * -curveWidth, 0), new Vector2()));
            }

            for (int x = 0; x < curvePoints.Count - 1; x++)
            {
                indices.Add(2 * x + 0);
                indices.Add(2 * x + 1);
                indices.Add(2 * x + 2);

                indices.Add(2 * x + 1);
                indices.Add(2 * x + 3);
                indices.Add(2 * x + 2);
            }

            return new object[] {
                path.ToArray(),
                indices.ToArray()
            };
        }

        public static void drawCurve3D(Object[] curveData)
        {
            ToylandSiege.GetInstance().GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                (VertexPositionTexture[])curveData[0],
                0,
                ((VertexPositionTexture[])curveData[0]).Length,
                (int[])curveData[1],
                0,
                ((int[])curveData[1]).Length / 3);
        }

        //Gets the interpolated Vector2 based on t
        private static Vector2 interpolatedPoint(Vector2 p1, Vector2 p2, float t)
        {
            return Vector2.Multiply(p2 - p1, t) + p1;
        }

        //Gets the normalized normal of a vertex, given two adjacent normals (2D)
        private static Vector2 getNormalizedVertexNormal(Vector2 v1, Vector2 v2) //v1 and v2 are normals
        {
            return getNormalizedVector(v1 + v2);
        }

        //Normalizes the given Vector2
        private static Vector2 getNormalizedVector(Vector2 v)
        {
            Vector2 temp = new Vector2(v.X, v.Y);
            v.Normalize();
            return v;
        }

        //Gets the normal of a given Vector2
        private static Vector2 getLineNormal(Vector2 v)
        {
            Vector2 normal = new Vector2(v.Y, -v.X);
            return normal;
        }

    }
}
