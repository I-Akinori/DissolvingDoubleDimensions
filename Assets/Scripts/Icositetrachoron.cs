using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DissolvingDoubleDimensions
{
    public class Icositetrachoron : MonoBehaviour
    {
        [SerializeField] private List<Vector4> points4d  = new List<Vector4>();
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

            points4d.Add(new Vector4(0, 0, 1, 1));
            points4d.Add(new Vector4(0, 0, 1, -1));
            points4d.Add(new Vector4(0, 0, -1, 1));
            points4d.Add(new Vector4(0, 0, -1, -1));
            
            points4d.Add(new Vector4(0, 1, 0, 1));
            points4d.Add(new Vector4(0, 1, 0, -1));
            points4d.Add(new Vector4(0, -1, 0, 1));
            points4d.Add(new Vector4(0, -1, 0, -1));
            
            points4d.Add(new Vector4(0, 1, 1, 0));
            points4d.Add(new Vector4(0, 1, -1, 0));
            points4d.Add(new Vector4(0, -1, 1, 0));
            points4d.Add(new Vector4(0, -1, -1, 0));
            
            points4d.Add(new Vector4(1, 0, 0,1));
            points4d.Add(new Vector4(1, 0, 0,-1));
            points4d.Add(new Vector4(-1, 0, 0, 1));
            points4d.Add(new Vector4(-1, 0, 0, -1));
            
            points4d.Add(new Vector4(1, 0, 1, 0));
            points4d.Add(new Vector4(1, 0, -1, 0));
            points4d.Add(new Vector4(-1, 0, 1, 0));
            points4d.Add(new Vector4(-1, 0,  -1, 0));
            
            points4d.Add(new Vector4(1, 1, 0, 0));
            points4d.Add(new Vector4(1, -1, 0, 0));
            points4d.Add(new Vector4(-1, 1, 0, 0));
            points4d.Add(new Vector4(-1, -1, 0, 0));
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
            if (vis0)
            {
                col = Color.red;
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[4 + i / 2], 
                        points3d[12 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2], col);
                }
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[8 + i / 2], 
                        points3d[16 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2], col);
                }
            }

            if (vis1)
            {
                col = Color.yellow;
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[0 + i / 2], 
                        points3d[12 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2], col);
                }
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[8 + i / 2], 
                        points3d[20 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2], col);
                }
            }

            if (vis2)
            {
                col = Color.green;
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[0 + i / 2], 
                        points3d[16 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2], col);
                }
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[4 + i / 2], 
                        points3d[20 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2], col);
                }
            }

            if (vis3)
            {
                col = Color.cyan;
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[0 + i / 2], 
                        points3d[4 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2], col);
                }
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[16 + i / 2], 
                        points3d[20 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1], col);
                }
            }
            if (vis4)
            {
                col = Color.blue;
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[0 + i / 2], 
                        points3d[8 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2], col);
                }
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[12 + i / 2], 
                        points3d[20 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1], col);
                }
            }
            if (vis5)
            {
                col = Color.magenta;
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[4 + i / 2], 
                        points3d[8 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1], col);
                }
                for (var i = 0; i < 8; i++)
                {
                    Debug.DrawLine(points3d[12 + i / 2], 
                        points3d[16 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1], col);
                }
            }
        }
    }
}