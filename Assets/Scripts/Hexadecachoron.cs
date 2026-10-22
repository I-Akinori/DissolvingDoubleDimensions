using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DissolvingDoubleDimensions
{
    public class Hexadecachoron : MonoBehaviour
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
            points4d.Add(new Vector4(1, 0, 0,0));
            points4d.Add(new Vector4(-1, 0, 0, 0));
            points4d.Add(new Vector4(0, 1, 0, 0));
            points4d.Add(new Vector4(0, -1, 0, 0));
            points4d.Add(new Vector4(0, 0, 1, 0));
            points4d.Add(new Vector4(0, 0, -1, 0));
            points4d.Add(new Vector4(0, 0, 0, 1));
            points4d.Add(new Vector4(0, 0, 0, -1));
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
                Debug.DrawLine(points3d[0], points3d[2], col);
                Debug.DrawLine(points3d[0], points3d[3], col);
                Debug.DrawLine(points3d[1], points3d[2], col);
                Debug.DrawLine(points3d[1], points3d[3], col);
            }
            if (vis1)
            {
                col = Color.yellow;
                Debug.DrawLine(points3d[0], points3d[4], col);
                Debug.DrawLine(points3d[0], points3d[5], col);
                Debug.DrawLine(points3d[1], points3d[4], col);
                Debug.DrawLine(points3d[1], points3d[5], col);
            }
            if (vis2)
            {
                col = Color.green;
                Debug.DrawLine(points3d[0], points3d[6], col);
                Debug.DrawLine(points3d[0], points3d[7], col);
                Debug.DrawLine(points3d[1], points3d[6], col);
                Debug.DrawLine(points3d[1], points3d[7], col);
            }

            if (vis3)
            {
                col = Color.cyan;
                Debug.DrawLine(points3d[2], points3d[4], col);
                Debug.DrawLine(points3d[2], points3d[5], col);
                Debug.DrawLine(points3d[3], points3d[4], col);
                Debug.DrawLine(points3d[3], points3d[5], col);
            }
            if (vis4)
            {
                col = Color.blue;
                Debug.DrawLine(points3d[2], points3d[6], col);
                Debug.DrawLine(points3d[2], points3d[7], col);
                Debug.DrawLine(points3d[3], points3d[6], col);
                Debug.DrawLine(points3d[3], points3d[7], col);
            }
            if (vis5)
            {
                col = Color.magenta;
                Debug.DrawLine(points3d[4], points3d[6], col);
                Debug.DrawLine(points3d[4], points3d[7], col);
                Debug.DrawLine(points3d[5], points3d[6], col);
                Debug.DrawLine(points3d[5], points3d[7], col);
            }
        }
    }
}