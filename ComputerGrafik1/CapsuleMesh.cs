using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace ComputerGrafik1
{
    class CapsuleMesh : Mesh
    {

        static int vertexArrayObject;
        static int vertexBufferObject;
        static int elementBufferObject;
        private static bool buffersCreated;

        protected override void GenerateBuffers()
        {


            if (buffersCreated) return;

            buffersCreated = true;
        }

        public override void Draw()
        {
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            
        }


        private List<float> GenerateTopSphere(float radius, int sectorCount, float stackCount)
        {
            List<float> vertices = new();


            float x, y, z, xy;                              // vertex position
            //float nx, ny, nz, lengthInv = 1.0f / radius;    // vertex normal
            //float s, t;                                     // vertex texCoord

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
                    //nx = x * lengthInv;
                    //ny = y * lengthInv;
                    //nz = z * lengthInv;
                    //normals.Add(nx);
                    //normals.Add(ny);
                    //normals.Add(nz);

                    // vertex tex coord (s, t) range between [0, 1]
                    //s = (float)j / sectorCount;
                    //t = (float)i / stackCount;
                    //texCoords.Add(s);
                    //texCoords.Add(t);
                }
            }

            return vertices;

        }

        private List<float> GenerateBottomSphere(float radius, int sectorCount, float stackCount)
        {
            List<float> vertices = new();


            float x, y, z, xy;                              // vertex position
            //float nx, ny, nz, lengthInv = 1.0f / radius;    // vertex normal
            //float s, t;                                     // vertex texCoord

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
                    //normals.Add(nx);
                    //normals.Add(ny);
                    //normals.Add(nz);

                    // vertex tex coord (s, t) range between [0, 1]
                    //s = (float)j / sectorCount;
                    //t = (float)i / stackCount;
                    //texCoords.Add(s);
                    //texCoords.Add(t);
                }
            }

            return vertices;
        }

        private float[] GenerateCylinder(float topRadius, float botRadius, float height, int sectorCount, float stackCount)
        {
            List<float> vertices = new List<float>();
            List<float> topSphereVertices = GenerateTopSphere(topRadius, sectorCount, stackCount);
            List<float> botSphereVertices = GenerateBottomSphere(botRadius, sectorCount, stackCount);


            // put side vertices to arrays
            for (int i = 0; i < 2; ++i)
            {
                float h = -height / 2.0f + i * height;           // z value; -h/2 to h/2
                float t = 1.0f - i;                              // vertical tex coord; 1 to 0

                for (int j = 0, k = 0; j <= sectorCount; ++j, k += 3)
                {
                    float ux = unitVertices[k];
                    float uy = unitVertices[k + 1];
                    float uz = unitVertices[k + 2];
                    // position vector
                    vertices.Add(ux * radius);             // vx
                    vertices.Add(uy * radius);             // vy
                    vertices.Add(h);                       // vz

                    // texture coordinate
                    vertices.Add((float)j / sectorCount); // s
                    vertices.Add(t);                      // t
                }
            }

            // the starting index for the base/top surface
            //NOTE: it is used for generating indices later
            baseCenterIndex = vertices.Count / 5;

            // put base and top vertices to arrays
            for (int i = 0; i < 2; ++i)
            {
                float h = -height / 2.0f + i * height;           // z value; -h/2 to h/2
                float nz = -1 + i * 2;                           // z value of normal; -1 to 1

                // center point
                vertices.Add(0); vertices.Add(0); vertices.Add(h);
                vertices.Add(0.5f); vertices.Add(0.5f);

                for (int j = 0, k = 0; j < sectorCount; ++j, k += 3)
                {
                    float ux = unitVertices[k];
                    float uy = unitVertices[k + 1];
                    // position vector
                    vertices.Add(ux * radius);             // vx
                    vertices.Add(uy * radius);             // vy
                    vertices.Add(h);                       // vz
                                                           // texture coordinate
                    vertices.Add(-ux * 0.5f + 0.5f);      // s
                    vertices.Add(-uy * 0.5f + 0.5f);      // t
                }
            }

            return vertices;
        }

        private float[] GenerateCapsule()
        {

        }


    }
}
