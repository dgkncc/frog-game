using System.Collections.Generic;
using UnityEngine;

namespace FrogGame.Utils
{
    public static class ExtensionHelpers
    {
        public static List<Vector3> GenerateHermiteCurve(this List<Vector3> points, float tension, int segmentsPerPoint)
        {
            if (points.Count < 2)
                return points;

            List<Vector3> curvePoints = new List<Vector3>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 p0 = points[i];
                Vector3 p1 = points[i + 1];

                for (int j = 0; j < segmentsPerPoint; j++)
                {
                    float t = (float)j / segmentsPerPoint;
                    float t2 = t * t;
                    float t3 = t2 * t;

                    float h1 = 2 * t3 - 3 * t2 + 1;
                    float h2 = -2 * t3 + 3 * t2;
                    float h3 = t3 - 2 * t2 + t;
                    float h4 = t3 - t2;

                    Vector3 point = new Vector3(
                        h1 * p0.x + h2 * p1.x + h3 * tension * (p1.x - p0.x),
                        h1 * p0.y + h2 * p1.y + h3 * tension * (p1.y - p0.y),
                        h1 * p0.z + h2 * p1.z + h3 * tension * (p1.z - p0.z)
                    );

                    curvePoints.Add(point);
                }
            }

            curvePoints.Add(points[points.Count - 1]);

            return curvePoints;
        }
    }

}
