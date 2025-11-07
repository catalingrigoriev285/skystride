using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skystride.vendor
{
    internal class Model
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<int[]> faces = new List<int[]>();
        public bool Loaded { get; private set; }
        public string SourcePath { get; private set; }

        private Vector3 minBound = Vector3.Zero;
        private Vector3 maxBound = Vector3.Zero;
        private Vector3 center = Vector3.Zero;
        private Vector3 size = Vector3.Zero;

        public float BBoxHeight { get { return size.Y; } }
        public Vector3 Center { get { return center; } }

        public Model(string path)
        {
            this.SourcePath = path;

            try
            {
                LoadModel(path);

                if (vertices.Count > 0)
                {
                    minBound = vertices[0];
                    maxBound = vertices[0];
                    foreach (var v in vertices)
                    {
                        minBound = new Vector3(Math.Min(minBound.X, v.X), Math.Min(minBound.Y, v.Y), Math.Min(minBound.Z, v.Z));
                        maxBound = new Vector3(Math.Max(maxBound.X, v.X), Math.Max(maxBound.Y, v.Y), Math.Max(maxBound.Z, v.Z));
                    }
                    center = (minBound + maxBound) * 0.5f;
                    size = maxBound - minBound;
                }

                Loaded = true;
                Console.WriteLine($"Model loaded: {path} ({vertices.Count} vertices, {faces.Count} faces)");
            }
            catch
            {
                Loaded = false;
                Console.WriteLine($"Failed to load model: {path}");
            }
        }

        private void LoadModel(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            using (var sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0) continue;
                    if (line.StartsWith("v "))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                            vertices.Add(new Vector3(x, y, z));
                        }
                    }
                    else if (line.StartsWith("f "))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        List<int> idx = new List<int>();
                        for (int i = 1; i < parts.Length; i++)
                        {
                            var v = parts[i];
                            var vparts = v.Split('/');
                            int vi = int.Parse(vparts[0], CultureInfo.InvariantCulture);
                            if (vi < 0) vi = vertices.Count + vi + 1; // negative indices support
                            idx.Add(vi - 1); // obj indices are 1-based
                        }
                        if (idx.Count >= 3)
                        {
                            // triangulate polygon (fan)
                            for (int i = 1; i < idx.Count - 1; i++)
                            {
                                faces.Add(new int[] { idx[0], idx[i], idx[i + 1] });
                            }
                        }
                    }
                }
            }
        }

        public void Render(Vector3 position, float scale = 1f, float rotX = 0f, float rotY = 0f)
        {
            if (!Loaded) return;

            GL.PushMatrix();
            GL.Translate(position);

            // apply rotations: pitch (X) then yaw (Y)
            if (rotX != 0f) GL.Rotate(rotX, 1f, 0f, 0f);
            if (rotY != 0f) GL.Rotate(rotY, 0f, 1f, 0f);

            if (scale != 1f) GL.Scale(scale, scale, scale);

            GL.Color3(0.85f, 0.75f, 0.6f); // default wooden tint

            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                Vector3 v0 = vertices[f[0]] - center;
                Vector3 v1 = vertices[f[1]] - center;
                Vector3 v2 = vertices[f[2]] - center;

                // compute face normal
                Vector3 n = Vector3.Cross(v1 - v0, v2 - v0);
                if (n.LengthSquared > 0f) n.NormalizeFast();
                GL.Normal3(n);

                GL.Vertex3(v0);
                GL.Vertex3(v1);
                GL.Vertex3(v2);
            }
            GL.End();

            GL.PopMatrix();
        }
    }
}
