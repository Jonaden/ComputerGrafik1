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
        int sectorCount = 2;
        int stackCount = 3;


        private List<float> listofVertices = new List<float>();
        private List<uint> listofIndices = new List<uint>();
        private List<float> texCoords = new List<float>();
        private List<float> listOfNormals = new List<float>();

        public SphereMesh(float radius, int sectorCount, int stackCount)
        {
            this.radius = radius;
            this.sectorCount = sectorCount;
            this.stackCount = stackCount;
            
        }

        
        protected override uint[] Indices => base.Indices;
        protected override float[] Vertices => new float[]
         {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f,  0.5f, 0.0f  //Top vertex
        };
        static int vertexArrayObject;
        static int vertexBufferObject;
        private static bool buffersCreated;

        public void GenerateVertices()
        {
            float lengthInv = 1.0f / radius;
            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;
            

            for(int i = 0; i <= stackCount; i++)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;
                float xy = radius * MathF.Cosh(stackAngle);
                float z = radius * MathF.Sinh(stackAngle);

                for(int j = 0; j <= sectorCount; j++)
                {
                    sectorAngle = j * sectorStep;

                    float x = xy * MathF.Cosh(sectorAngle);
                    float y = xy * MathF.Sinh(sectorAngle);

                    listofVertices.Add(x);
                    listofVertices.Add(y);
                    listofVertices.Add(z);

                    float nx = x * lengthInv;
                    float ny = y * lengthInv;
                    float nz = z * lengthInv;

                    listOfNormals.Add(nx);
                    listOfNormals.Add(ny);
                    listOfNormals.Add(nz);

                    float s = (float)j / sectorCount;
                    float t = (float)i / stackCount;
                    texCoords.Add(s);
                    texCoords.Add(t);                   
                }              
            }
        }

        public void GenerateIndices()
        {
            List<int> indices = new List<int>();
            List<int> lineIndices = new List<int>();
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

                    if(i != (stackCount - 1))
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + k1);
                    }

                    lineIndices.Add(k1);
                    lineIndices.Add(k2);

                    if(i != 0)
                    {
                        lineIndices.Add(k1);
                        lineIndices.Add(k1 + 1);
                    }
                }
            }
       
        }
        protected override void GenerateBuffers()
        {

            if (buffersCreated)
            {
                return;
            }
            vertexBufferObject = GL.GenBuffer();

  

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);          
            GL.BufferData(BufferTarget.ArrayBuffer, listofVertices.Count * sizeof(float), listofVertices.ToArray(), BufferUsageHint.StaticDraw);
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

       
            // Enable variable 0 in the shader.
            GL.EnableVertexAttribArray(0);
            buffersCreated = true;


        }
        public override void Draw()
        {
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, listofVertices.Count, DrawElementsType.UnsignedInt, listofIndices.Count);
        }
    }
}
