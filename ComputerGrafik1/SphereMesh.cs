using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGrafik1
{
    internal class SphereMesh : Mesh
    {

        float radius = 1;
        int sectorCount = 16;
        int stackCount = 8;

        private static int elementBufferObject;
        private static int vertexArrayObject;
        private static int vertexBufferObject;
        private static bool buffersCreated;
        
        private List<float> texCoords = new List<float>();
        private List<float> listOfNormals = new List<float>();

        

        float x, y, z, xy;
        float nx, ny, nz;
        float s, t;

        public List<float> GenerateVertices()
        {
            float lengthInv = 1.0f / radius;
            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;


            List<float> listofVertices = new List<float>();

            for(int i = 0; i <= stackCount; i++)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;
                float xy = radius * MathF.Cos(stackAngle);
                float z = radius * MathF.Sin(stackAngle);

                for(int j = 0; j <= sectorCount; j++)
                {
                    sectorAngle = j * sectorStep;

                    x = xy * MathF.Cos(sectorAngle);
                    y = xy * MathF.Sin(sectorAngle);

                    listofVertices.Add(x);
                    listofVertices.Add(y);
                    listofVertices.Add(z);

                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;

                    listOfNormals.Add(nx);
                    listOfNormals.Add(ny);
                    listOfNormals.Add(nz);

                    s = (float)j / sectorCount;
                    t = (float)i / stackCount;
                    listofVertices.Add(s);
                    listofVertices.Add(t);          
                            
                }  
                
            }
            return listofVertices;
        }

        public List<uint> GenerateIndices()
        {
   
            List<int> indices = new List<int>();         
            int k1, k2;

            for(int i = 0; i < stackCount; i++)
            {
                k1 = i * (sectorCount + 1);
                k2 = k1 + sectorCount + 1;

                for(int j = 0; j < sectorCount; j++, k1++, k2++)
                {
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    if(i != (stackCount-1))
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }
                }
            }
            List<uint> uints = indices.Select(i => (uint)i).ToList();

            return uints;
        }
        protected override void GenerateBuffers()
        {

            Vertices = GenerateVertices().ToArray();
            Indices = GenerateIndices().ToArray();

            if (buffersCreated)
            {
                return;
            }

            vertexBufferObject = GL.GenBuffer();


            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);          
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            // Enable variable 0 in the shader.
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, (int)(elementBufferObject));
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            buffersCreated = true;


        }
        public override void Draw()
        {
           //GL.PointSize(10);
           //GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length / 5);
           GL.BindVertexArray(vertexArrayObject);
           GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
