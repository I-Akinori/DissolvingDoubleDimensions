using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DissolvingDoubleDimensions
{
    public class HyperTorus : MonoBehaviour
    {
        [SerializeField] private List<Vector4> points4d;
        [SerializeField] private Material material;
        [SerializeField, Range(0, 1)] private float alpha;
        private float time = 0f;
        private bool expanding = true;
        private int axis1 = 0;
        private int axis2 = 3;
        private int axis3 = 1;
        private int axis4 = 2;
        [SerializeField, Range(0, 10)] private float rad0 = 4f;
        [SerializeField, Range(0, 10)] private float rad1 = 2f;
        [SerializeField, Range(0, 10)] private float rad2 = 1f;
        [SerializeField, Range(0, 128)] private int thetaRes = 8;
        [SerializeField, Range(0, 128)] private int phiRes = 64;
        [SerializeField, Range(0, 128)] private int psiRes = 64;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            SetPoints();
            var dt = Time.deltaTime;
            time += dt;
            if (time < Mathf.PI * 2)
            {
                Rotation4d(ref points4d, time, axis1, axis2);
                Rotation4d(ref points4d, time * 2, axis3, axis4);
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
        }

        private void SetPoints()
        {
            points4d.Clear();
            for (var i = 0; i < thetaRes; i++)
            {
                var theta = i * 2 * Mathf.PI / thetaRes;
                for (var j = 0; j < phiRes; j++)
                {
                    var phi = j * 2 * Mathf.PI / phiRes;
                    for (var k = 0; k < psiRes; k++)
                    {
                        var psi = k * 2 * Mathf.PI / psiRes;
                        points4d.Add(new Vector4(
                            (rad0 + rad1 * Mathf.Cos(theta) + rad2 * Mathf.Cos(phi)) * Mathf.Cos(psi),
                            (rad0 + rad1 * Mathf.Cos(theta) + rad2 * Mathf.Cos(phi)) * Mathf.Sin(psi),
                            (rad1 * Mathf.Sin(theta) + rad2 * Mathf.Sin(phi)) * Mathf.Cos(psi),
                            (rad1 * Mathf.Sin(theta) + rad2 * Mathf.Sin(phi)) * Mathf.Sin(psi)
                        ));
                    }   
                }
            }
        }

        private void Rotation4d(ref List<Vector4> points, float angle, int ax1, int ax2)
        {
            var matR = Matrix4x4.identity;
            matR[ax1, ax1] = Mathf.Cos(angle);
            matR[ax1, ax2] = - Mathf.Sin(angle);
            matR[ax2, ax1] = Mathf.Sin(angle);
            matR[ax2, ax2] = Mathf.Cos(angle);
            for (var i = 0; i < points.Count; i++)
            {
                points[i] = matR * points[i];
            }
        }

        private List<Vector3> ConvertTo3D(List<Vector4> points, float d)
        {
            return points.Select(p =>
            {
                var parse = 1f / (d - p.w);
                return new Vector3(p.x, p.y, p.z) * parse;
            }).ToList();
        }

        private void DrawEdge()
        {
            var points3d = this.ConvertTo3D(points4d, 3);
            for (var i = 0; i < thetaRes; i++)
            {
                for (var j = 0; j < phiRes; j++)
                {
                    for (var k = 0; k < psiRes; k++)
                    {
                        var index = i * (phiRes * psiRes) + j * psiRes;
                        var i0 = index + k;
                        var i1 = index + (k < psiRes - 1 ? k + 1 : 0);
                        var col = Color.HSVToRGB((float)i / thetaRes, 0.5f + 0.5f * Mathf.Cos(Mathf.PI * 2 * j / phiRes), 1);
                       Debug.DrawLine(points3d[i0], points3d[i1], col);
                    }   
                }
            }
        }
        private void AddFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color col, ref List<Vector3> ps, ref List<int> ts, ref List<Color> cs)
        {
            var index = ps.Count;
            ps.Add(v1); ps.Add(v2); ps.Add(v3); ps.Add(v4);
            ts.Add(index); ts.Add(index + 1); ts.Add(index + 2);
            ts.Add(index); ts.Add(index + 2); ts.Add(index + 3);
            cs.Add(col); cs.Add(col); cs.Add(col); cs.Add(col);
        }
    }
}