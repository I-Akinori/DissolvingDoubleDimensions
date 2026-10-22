using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DissolvingDoubleDimensions
{
    public class Hecatonicosachoron : MonoBehaviour
    {
        [SerializeField] private List<Vector4> points4d = new List<Vector4>();
        [SerializeField] private Material material;
        [SerializeField, Range(0, 1)] private float alpha;
        [SerializeField] private bool vis0;
        [SerializeField] private bool vis1;
        [SerializeField] private bool vis2;
        [SerializeField] private bool vis3;
        [SerializeField] private bool vis4;
        [SerializeField] private bool vis5;
        private float time = 0f;
        private bool expanding = true;
        private int axis1 = 0;
        private int axis2 = 3;
        private int axis3 = 1;
        private int axis4 = 2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            points4d = new List<Vector4>();
            var sqr5 = Mathf.Sqrt(5);
            var phi = (1 + sqr5) * 0.5f;
            var phi2 = phi * phi;
            var phiInv = 1.0f / phi;
            var phiInv2 = 1.0f / phi2;
            // A: 24 points
            Shuffle24(ref points4d, 2f);
            // B: 64 points
            Shuffle64(ref points4d, phi, phiInv2);
            // C: 64 points
            Shuffle64(ref points4d, 1f, sqr5);
            // D: 64 points
            Shuffle64(ref points4d, phiInv, phi2);
            // E: 96 points
            Shuffle96(ref points4d, phiInv, phi, sqr5);
            // F: 96 points
            Shuffle96(ref points4d, phiInv2, 1f, phi2);
            // G: 192 points
            Shuffle192(ref points4d, phiInv, 1f, phi, 2f);
        }

        // Update is called once per frame
        void Update()
        {
            var dt = Time.deltaTime;
            time += dt;
            if (time < Mathf.PI * 2)
            {
                Rotation4d(ref points4d, dt, axis1, axis2);
                Rotation4d(ref points4d, dt * 2, axis3, axis4);
            }
            else
            {
                time = 0;
                axis1 = Random.Range(0, 4);
                axis2 = Random.Range(0, 4);
                if (axis1 == axis2)
                {
                    axis2 = (axis1 + 1) % 4;
                }

                axis3 = Random.Range(0, 4);
                axis4 = Random.Range(0, 4);
                if (axis3 == axis4)
                {
                    axis4 = (axis3 + 1) % 4;
                }
            }

            DrawEdge();
            //DrawFace();
        }

        private void Shuffle24(ref List<Vector4> points, float a)
        {
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    0, 0, a * (i < 2 ? 1 : -1), a * (i % 2 == 0 ? 1 : -1)));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    0, a * (i < 2 ? 1 : -1), 0,a * (i % 2 == 0 ? 1 : -1)));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    0, a * (i < 2 ? 1 : -1), a * (i % 2 == 0 ? 1 : -1), 0));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    a * (i < 2 ? 1 : -1), 0, 0,a * (i % 2 == 0 ? 1 : -1)));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    a * (i < 2 ? 1 : -1), 0, a * (i % 2 == 0 ? 1 : -1), 0));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    a * (i < 2 ? 1 : -1), a * (i % 2 == 0 ? 1 : -1), 0, 0));
            }
        }

        private void Shuffle64(ref List<Vector4> points, float a, float b)
        {
            for (var k = 0; k < 4; k++)
            {
                for (var i = 0; i < 16; i++)
                {
                    points.Add(new Vector4(
                        (((i >> 0) & 1) == 0 ? -1 : 1) * (k == 0 ? b : a), 
                        (((i >> 1) & 1) == 0 ? -1 : 1) * (k == 1 ? b : a), 
                        (((i >> 2) & 1) == 0 ? -1 : 1) * (k == 2 ? b : a), 
                        (((i >> 3) & 1) == 0 ? -1 : 1) * (k == 3 ? b : a)
                    ));
                }
            }
        }
        private void Shuffle96(ref List<Vector4> points, float a, float b, float c)
        {
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    0, 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                    ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    0, 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    0, 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    0, 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    0, 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    0, 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    0, 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    0, 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    0, 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    0
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    0
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    0
                ));
            }
        }

        private void Shuffle192(ref List<Vector4> points, float a, float b, float c, float d)
        {
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
        }
        private void Rotation4d(ref List<Vector4> points, float angle, int ax1, int ax2)
        {
            var matR = Matrix4x4.identity;
            matR[ax1, ax1] = Mathf.Cos(angle);
            matR[ax1, ax2] = -Mathf.Sin(angle);
            matR[ax2, ax1] = Mathf.Sin(angle);
            matR[ax2, ax2] = Mathf.Cos(angle);
            for (var i = 0; i < points.Count; i++)
            {
                points[i] = matR * points[i];
            }
        }

        private List<Vector3> ConvertTo3d(List<Vector4> points, float d)
        {
            var reduced = new List<Vector3>();
            points.ForEach(p =>
            {
                var parse = 1f / (d - p.w);
                reduced.Add(new Vector3(p.x, p.y, p.z) * parse);
            });
            return reduced;
        }


        private void DrawEdge()
        {
            var points3d = this.ConvertTo3d(points4d, 3);
            var col = Color.gray;
            float edgeDot = 0.5f;
            float eps = 1e-2f;
            var count = 0;

            float minDist2 = float.MaxValue;

            for (var i = 0; i < points3d.Count; i++)
            {
                for (var j = i + 1; j < points3d.Count; j++)
                {
                    float d2 = (points4d[i] - points4d[j]).sqrMagnitude;
                    if (d2 < minDist2)
                    {
                        minDist2 = d2;
                    }
                }
            }
            
            for (var i = 0; i < points3d.Count; i++)
            {
                for (var j = i + 1; j < points3d.Count; j++)
                {
                    var d2 = (points4d[i] - points4d[j]).sqrMagnitude;
                    if (Mathf.Abs(d2 - minDist2) < 0.1f)
                    {
                        count++;
                        Debug.DrawLine(points3d[i], points3d[j], col);
                    }
                }
            }
        }
    }
}