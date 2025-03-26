using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGrafik1
{
    public class ModelLoader: Mesh
    {
        GameObject gameObject;

        Vector3[] vertices;
        Vector3[] colors;
        Vector2[] textureCoords;

        private static int vertexArrayObject;
        private static int vertexBufferObject;
        private static int elementBufferObject;
        private static bool buffersCreated;



        List<Tuple<int, int, int>> faces = new List<Tuple<int, int, int>>();

        public override float[] Vertices { get { return (vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray()); } }
        public override uint[] Indices { get { return faces.SelectMany (f => new uint[] { (uint)f.Item1 * 3, (uint)f.Item2 * 3 , (uint)f.Item3 * 3 }).ToArray(); } }

       

        public int ColorDataCount { get { return (int)colors.Length; } }

        public Vector3[] GetVerts()
        {
            return vertices;

        }

        public override void Draw()
        {
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length);
        }

        public int[] GetIndices (int offset = 0)
        {
            List<int> temp = new List<int>();

            foreach(var face in faces)
            {
                temp.Add(face.Item1 + offset);

            }

            return temp.ToArray();
        }

        protected override void GenerateBuffers()
        {
            vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, (int)vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float),
                Vertices, BufferUsageHint.StaticDraw);
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.EnableVertexAttribArray(0);           

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, (int)elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
        }
        public Vector3[] GetColorData()
        {
            return colors;
        }
        public Vector2[] GetTextureCoords()
        {
            return textureCoords;
        }

        public void CalculateModelMatrix()
        {
            gameObject.transform.CalculateModel();
        }

        public List<float> Vec3ArrToFlatList(Vector3[] vec3Arr)
        {
            List<float> floats = new List<float>();

            for (int i = 0; i < vec3Arr.Length; i++)
            {
                floats.Add(vec3Arr[i].X);
                floats.Add(vec3Arr[i].Y);
                floats.Add(vec3Arr[i].Z);

            }

            return floats;
        }
        public static ModelLoader LoadFromFile(string filename)
        {
            ModelLoader obj = new ModelLoader();

            try
            {
                using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    obj = LoadFromString(reader.ReadToEnd());
                }
            }

            catch(FileNotFoundException e)
            {
                Console.WriteLine("File not found: {0}", filename);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error loading file: {0}", filename);
            }
            obj.GenerateBuffers();
            return obj;
        }

        public static ModelLoader LoadFromString(string obj)
        {
            List<String> lines = new List<String>(obj.Split('\n'));

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> colors = new List<Vector3>();
            List<Vector2> texs = new List<Vector2>();

            List<Tuple<int, int, int>> faces = new List<Tuple<int, int, int>>();

            foreach(string line in lines)
            {
                if (line.StartsWith("v "))
                {
                    String temp = line.Substring(2);

                    Vector3 vec = new Vector3();

                    if (temp.Count((char c) => c == ' ') == 2)
                    {
                        String[] vertparts = temp.Split(' ');

                        bool succes = float.TryParse(vertparts[0], out vec.X);
                        succes &= float.TryParse(vertparts[1], out vec.Y);
                        succes &= float.TryParse(vertparts[2], out vec.Z);

                        colors.Add(new Vector3((float)Math.Sin(vec.Z), (float)Math.Sin(vec.Z), (float)Math.Sin(vec.Z)));
                        texs.Add(new Vector2((float)Math.Sin(vec.Z), (float)Math.Sin(vec.Z)));

                        if (!succes)
                        {
                            Console.WriteLine("Error parsing vertex: {0}", line);
                        }
                    }

                    verts.Add(vec);
                }
                else if(line.StartsWith("f "))
                {
                    String temp = line.Substring(2);

                    Tuple<int, int, int> face = new Tuple<int, int, int>(0, 0, 0);

                    if (temp.Count((char c) => c == ' ') == 2)
                    {
                        string[] faceParts = temp.Split(' ');

                        int i1, i2, i3;

                        bool succes = int.TryParse(faceParts[0], out i1);
                        succes &= int.TryParse(faceParts[1], out i2);
                        succes &= int.TryParse(faceParts[2], out i3);

                        if (!succes)
                        {
                            Console.WriteLine("Error parsing face {0}", line);
                        }
                        else
                        {
                            face = new Tuple<int, int, int>(i1 - 1, i2 - 1, i3 - 1);
                            faces.Add(face);
                        }
                    }
                }
            }

            ModelLoader modelLoader = new ModelLoader();
            modelLoader.vertices = verts.ToArray();
            modelLoader.faces = new List<Tuple<int, int, int>>(faces);
            modelLoader.colors = colors.ToArray();
            modelLoader.textureCoords = texs.ToArray();

            return modelLoader;
        }
    }
}
