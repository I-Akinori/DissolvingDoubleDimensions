using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DissolvingDoubleDimensions
{
    public class HyperCube : MonoBehaviour
    {
        [SerializeField] private List<Vector4> points4d;
        [SerializeField] private Material material;
        [SerializeField, Range(0, 1)] private float alpha;
        private float time = 0f;
        private bool expanding = true;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (var i = 0; i < 16; i++)
            {
                points4d.Add(
                    new Vector4(
                        ((i >> 0) & 1) == 0 ? -1 : 1, 
                        ((i >> 1) & 1) == 0 ? -1 : 1, 
                        ((i >> 2) & 1) == 0 ? -1 : 1, 
                        ((i >> 3) & 1) == 0 ? -1 : 1)
                    );
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (expanding)
            {
                ExpandDimension();
            }
            else
            {
                var dt = Time.deltaTime;
                time += dt;
                if (Mathf.FloorToInt(time / (Mathf.PI * 2)) % 3 == 0)
                {
                    Rotation4d(points4d, dt, 0, 1);
                }
                if (Mathf.FloorToInt(time / (Mathf.PI * 2)) % 3 == 1)
                {
                    Rotation4d(points4d, dt, 1, 3);
                }
                if (Mathf.FloorToInt(time / (Mathf.PI * 2)) % 3 == 2)
                {
                    Rotation4d(points4d, dt, 2, 3);
                }
            }
            DrawEdge();
            DrawFace();
        }

        private void ExpandDimension()
        {
            var sx = Mathf.Clamp01(Time.time - 0);
            var sy = Mathf.Clamp01(Time.time - 1);
            var sz = Mathf.Clamp01(Time.time - 2);
            var sw = Mathf.Clamp01(Time.time - 3);
            for (var i = 0; i < 16; i++)
            {
                points4d[i] = 
                    new Vector4(
                        (((i >> 0) & 1) == 0 ? -1 : 1) * sx, 
                        (((i >> 1) & 1) == 0 ? -1 : 1) * sy, 
                        (((i >> 2) & 1) == 0 ? -1 : 1) * sz, 
                        (((i >> 3) & 1) == 0 ? -1 : 1) * sw);
            }

            if (sw > 0.999f)
            {
                expanding = false;
            }
        }

        private void Rotation4d(List<Vector4> points, float angle, int ax1, int ax2)
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
            for (var i = 0; i < 16; i++)
            {
                if (i % 2 < 1)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 1], Color.red);
                }
                if (i % 4 < 2)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 2], Color.green);
                }
                if (i % 8 < 4)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 4], Color.cyan);
                }
                if (i % 16 < 8)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 8], Color.purple);
                }
            }
        }
        private void DrawFace()
        {
            var points3d = this.ConvertTo3D(points4d, 3);
            var positions = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();
            var a = alpha; 
            var col = new Color(1, 0, 0, a);
            AddFace(points3d[0], points3d[1], points3d[3], points3d[2], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[12], points3d[13], points3d[15], points3d[14], col, ref positions, ref triangles, ref colors);
            
            col = new Color(0, 1, 0, a);
            AddFace(points3d[0], points3d[8], points3d[12], points3d[4], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[3], points3d[11], points3d[15], points3d[7], col, ref positions, ref triangles, ref colors);
            
            col = new Color(0, 1, 1, a);
            AddFace(points3d[1], points3d[3], points3d[7], points3d[5], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[8], points3d[10], points3d[14], points3d[12], col, ref positions, ref triangles, ref colors);
            
            col = new Color(1, 1, 0, a);
            AddFace(points3d[0], points3d[1], points3d[9], points3d[8], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[6], points3d[7], points3d[15], points3d[14], col, ref positions, ref triangles, ref colors);
            
            var mesh = new Mesh();
            mesh.vertices = positions.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
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