using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComputerGrafik1
{
    class CapsuleStretchMesh : Mesh
    {

        static int vertexArrayObject;
        static int vertexBufferObject;
        static int elementBufferObject;
        private static bool buffersCreated;
        float height = 3;
        float radius;
        int stackCount = 16;
        int sectorCount = 8;

        float x, y, z, xy;
        float nx, ny, nz;
        float s, t;
        protected override void GenerateBuffers()
        {
            radius = height / 3;
            //Vertices = GenerateCapsuleVertices(topRadius, botRadius, height, out baseCenterIndex, 36, 16, 6);
            //Indices = GenerateCapsuleIndices(26, 36, 6).ToArray();

            Vertices = GenerateCapsuleVertices().ToArray();
            Indices = GenerateIndices().ToArray();


            if (buffersCreated) return;

            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, (int)elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);



            buffersCreated = true;
        }

        public override void Draw()
        {
            GL.PointSize(10);
            GL.BindVertexArray(vertexArrayObject);
            //GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length / 5);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

            
        }


        private List<float> GenerateCapsuleVertices()
        {
            float lengthInv = 1.0f / radius;
            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;


            List<float> listofVertices = new List<float>();

            for (int i = 0; i <= stackCount; i++)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;
                float xy = radius * MathF.Cos(stackAngle);
                float z = radius * MathF.Sin(stackAngle);

                if(i <= stackCount / 2)
                {
                    for (int j = 0; j <= sectorCount; j++)
                    {

                        sectorAngle = j * sectorStep;

                        x = xy * MathF.Cos(sectorAngle);
                        y = xy * MathF.Sin(sectorAngle);

                        listofVertices.Add(x);
                        listofVertices.Add(y);
                        listofVertices.Add(z+height);

                        //nx = x * lengthInv;
                        //ny = y * lengthInv;
                        //nz = z * lengthInv;

                        //listOfNormals.Add(nx);
                        //listOfNormals.Add(ny);
                        //listOfNormals.Add(nz);

                        s = (float)j / sectorCount;
                        t = (float)i / stackCount;
                        listofVertices.Add(s);
                        listofVertices.Add(t);
                    }
                }
                else
                {
                     for (int j = 0; j <= sectorCount; j++)
                    {

                    
                        sectorAngle = j * sectorStep;

                        x = xy * MathF.Cos(sectorAngle);
                        y = xy * MathF.Sin(sectorAngle);

                        listofVertices.Add(x);
                        listofVertices.Add(y);
                        listofVertices.Add(z);

                        //nx = x * lengthInv;
                        //ny = y * lengthInv;
                        //nz = z * lengthInv;

                        //listOfNormals.Add(nx);
                        //listOfNormals.Add(ny);
                        //listOfNormals.Add(nz);

                        s = (float)j / sectorCount;
                        t = (float)i / stackCount;
                        listofVertices.Add(s);
                        listofVertices.Add(t);
                    

                    

                    }
                }

                

            }
            return listofVertices;
        }

        public List<uint> GenerateIndices()
        {

            List<int> indices = new List<int>();
            int k1, k2;

            for (int i = 0; i < stackCount; i++)
            {
                k1 = i * (sectorCount + 1);
                k2 = k1 + sectorCount + 1;

                for (int j = 0; j < sectorCount; j++, k1++, k2++)
                {
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    if (i != (stackCount - 1))
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

        /*
        private List<float> GenerateTopSphere(float radius, int sectorCount, float stackCount)
        {
            List<float> vertices = new();


            float x, y, z, xy;                              // vertex position
            float nx, ny, nz, lengthInv = 1.0f / radius;    // vertex normal
            float s, t;                                     // vertex texCoord

            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount / 2; i++)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;        // starting from pi/2 to -pi/2
                xy = radius * MathF.Cos(stackAngle);             // r * cos(u)
                z = radius * MathF.Sin(stackAngle);              // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= sectorCount; j++)
                {
                    sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                    // vertex position (x, y, z)
                    x = xy * MathF.Cos(sectorAngle);             // r * cos(u) * cos(v)
                    y = xy * MathF.Sin(sectorAngle);             // r * cos(u) * sin(v)
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);

                    // normalized vertex normal (nx, ny, nz)
                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;
                    vertices.Add(nx);
                    vertices.Add(ny);
                    vertices.Add(nz);

                    //vertex tex coord (s, t) range between [0, 1]
                    s = (float)j / sectorCount;
                    t = (float)i / stackCount;
                    vertices.Add(s);
                    vertices.Add(t);
                }
            }

            return vertices;

        }

        private List<float> GenerateBottomSphere(float radius, int sectorCount, float stackCount)
        {
            List<float> vertices = new();


            float x, y, z, xy;                              // vertex position
            float nx, ny, nz, lengthInv = 1.0f / radius;    // vertex normal
            float s, t;                                     // vertex texCoord

            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount / 2; i++)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;
                xy = radius * MathF.Cos(stackAngle);
                z = radius * MathF.Sin(stackAngle);

                for (int j = 0; j <= sectorCount; j++)
                {
                    sectorAngle = j * sectorStep;

                    // vertex position (x, y, z)
                    x = xy * MathF.Cos(sectorAngle);
                    y = xy * MathF.Sin(sectorAngle);
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);

                    // normalized vertex normal (nx, ny, nz)
                    //nx = x * lengthInv;
                    //ny = y * lengthInv;
                    //nz = z * lengthInv;
                    //vertices.Add(nx);
                    //vertices.Add(ny);
                    //vertices.Add(nz);

                    // vertex tex coord (s, t) range between [0, 1]
                    s = (float)j / sectorCount;
                    t = (float)i / stackCount;
                    vertices.Add(s);
                    vertices.Add(t);
                }
            }

            return vertices;
        }

        private float[] GenerateCapsuleVertices(float topRadius, float botRadius, float height, out int baseCenterIndex, int sectorCountSphere, int sectorCountCylinder, float stackCount)
        {
            List<float> vertices = new List<float>();
            List<float> topSphereVertices = GenerateTopSphere(topRadius, sectorCountSphere/2, stackCount);
            List<float> botSphereVertices = GenerateBottomSphere(botRadius, sectorCountSphere/2, stackCount);


            // put side vertices to arrays
            for (int i = 0; i < 2; ++i)
            {
                float h = -height / 2.0f + i * height;           // z value; -h/2 to h/2
                float t = 1.0f - i;                              // vertical tex coord; 1 to 0

                for (int j = 0, k = 0; j <= sectorCountCylinder; ++j, k += 3)
                {
                    
                    // position vector
                    if (i % 2 == 0)
                    {
                        float ux = topSphereVertices[k];
                        float uy = topSphereVertices[k + 1];
                        float uz = topSphereVertices[k + 2];
                        vertices.Add(ux * topRadius);             // vx
                        vertices.Add(uy * topRadius);  
                    }
                    else
                    {
                        float ux = botSphereVertices[k];
                        float uy = botSphereVertices[k + 1];
                        float uz = botSphereVertices[k + 2];
                        vertices.Add(ux * botRadius);             // vx
                        vertices.Add(uy * botRadius);
                    }
                               // vy
                    vertices.Add(h);                       // vz

                    // texture coordinate
                    vertices.Add((float)j / sectorCountCylinder); // s
                    vertices.Add(t);                      // t
                }
            }

            // the starting index for the base/top surface
            //NOTE: it is used for generating indices later
            

            // put base to arrays
            
                float h2 = -height / 2.0f + 1 * height;           // z value; -h/2 to h/2
                float nz1 = -1 + 1 * 2;                           // z value of normal; -1 to 1

                // center point
                vertices.Add(0); vertices.Add(0); vertices.Add(h2);
                vertices.Add(0.5f); vertices.Add(0.5f);

                for (int j = 0, k = 0; j < sectorCountSphere/2; ++j, k += 3)
                {
                    float ux = botSphereVertices[k];
                    float uy = botSphereVertices[k + 1];
                    // position vector
                    vertices.Add(ux * botRadius);             // vx
                    vertices.Add(uy * botRadius);             // vy
                    vertices.Add(h2);                       // vz
                                                           // texture coordinate
                    vertices.Add(-ux * 0.5f + 0.5f);      // s
                    vertices.Add(-uy * 0.5f + 0.5f);      // t
                }

            float h3 = -height / 2.0f + 2 * height;           // z value; -h/2 to h/2
            float nz2 = -1 + 2 * 2;                           // z value of normal; -1 to 1

            // center point
            vertices.Add(0); vertices.Add(0); vertices.Add(h3);
            vertices.Add(0.5f); vertices.Add(0.5f);

            for (int j = 0, k = 0; j < sectorCountSphere/2; ++j, k += 3)
            {
                float ux = topSphereVertices[k];
                float uy = topSphereVertices[k + 1];
                // position vector
                vertices.Add(ux * topRadius);             // vx
                vertices.Add(uy * topRadius);             // vy
                vertices.Add(h3);                       // vz
                                                       // texture coordinate
                vertices.Add(-ux * 0.5f + 0.5f);      // s
                vertices.Add(-uy * 0.5f + 0.5f);      // t
            }

            baseCenterIndex = vertices.Count / 5;

            return vertices.ToArray();
        }

        private List<uint> GenerateCapsuleIndices(int sectorCountSphere, int sectorCountCylinder, int stackCount)
        {
            //indices for sides
            List<int> indices = new List<int>();

            int topCenterIndex = baseCenterIndex + sectorCountCylinder + 1; // include center vertex

            int k1 = 0;                         // 1st vertex index at base
            int k2 = sectorCountCylinder + 1;           // 1st vertex index at top

            // indices for the side surface
            for (int i = 0; i < sectorCountCylinder; ++i, ++k1, ++k2)
            {
                // 2 triangles per sector
                // k1 => k1+1 => k2
                indices.Add(k1);
                indices.Add(k1 + 1);
                indices.Add(k2);

                // k2 => k1+1 => k2+1
                indices.Add(k2);
                indices.Add(k1 + 1);
                indices.Add(k2 + 1);
            }

            //indices for bot sphere
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = i * (sectorCountSphere/2 + 1);     // beginning of current stack
                k2 = k1 + sectorCountSphere/2 + 1;      // beginning of next stack

                for (int j = 0; j < sectorCountSphere/2; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (stackCount - 1))
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }
                }

            }
            //indices for top sphere
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = i * (sectorCountSphere/2 + 1);     // beginning of current stack
                k2 = k1 + sectorCountSphere/2 + 1;      // beginning of next stack

                for (int j = 0; j < sectorCountSphere/2; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (stackCount - 1))
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
        */

    }
}
