using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ComputerGrafik1
{
	// This is just another capsule primitive
	public class BeanTest : Mesh
	{
		// Inherited from Mesh
		private static int vertexArrayObject;
		private static int vertexBufferObject;
		private static int elementBufferObject;
		private static bool buffersCreated;

		float t;
		Game window;
		public BeanTest(Game window)
		{
			this.window = window;
			GenerateBuffers();
		}

		public override void Draw()
		{
			KeyboardState input = window.KeyboardState;

			if (input.IsKeyDown(Keys.W))
				t += 0.1f;
			if (input.IsKeyDown(Keys.S))
				t -= 0.1f;

			Vertices = GetBeanVertices(1, t, 16, 8, out int baseCenterIndex).ToArray();
			GL.BindVertexArray(vertexArrayObject);
			GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
			GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

			//GL.PointSize(10);
			//GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length / 5);
		}

		protected override void GenerateBuffers()
		{
			int sectorCount = 16;
			int stackCount = 8;
			float height = 4;
			float radius = 1;

			Vertices = GetBeanVertices(radius, height, sectorCount, stackCount, out int baseCenterIndex).ToArray();
			Indices = GetIndices(sectorCount, stackCount, baseCenterIndex).ToArray();

			if (buffersCreated) return;

			vertexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
			vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(vertexArrayObject);

			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			elementBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);


			buffersCreated = true;
		}

		static List<float> GetUnitCircleVertices(int sectorCount)
		{
			float sectorStep = 2 * MathF.PI / sectorCount;
			float sectorAngle;  // radian

			var vertices = new List<float>();
			for (int i = 0; i <= sectorCount; ++i)
			{
				sectorAngle = i * sectorStep;
				vertices.Add(MathF.Cos(sectorAngle)); // x
				vertices.Add(MathF.Sin(sectorAngle)); // y
				vertices.Add(0);                      // z
			}

			return vertices;
		}

		public static List<float> GetVerticesHalfSphere(float radius, float height, int sectorCount, int stackCount, bool top)
		{
			float x, y, z, xy;
			float s, t;

			float sectorStep = 2 * MathF.PI / sectorCount;
			float stackStep = MathF.PI / stackCount;
			float sectorAngle, stackAngle;

			List<float> listofVertices = new List<float>();

			int stackStart, stackEnd;
			float zOffset;

			if (top)
			{
				stackStart = 0;
				stackEnd = (stackCount / 2);
				zOffset = height / 2;
			}
			else
			{
				stackStart = stackCount / 2;
				stackEnd = stackCount;
				zOffset = -height / 2;
			}

			for (int i = stackStart; i <= stackEnd; i++)
			{
				stackAngle = MathF.PI / 2 - i * stackStep;
				xy = radius * MathF.Cos(stackAngle);
				z = (radius * MathF.Sin(stackAngle)) + zOffset;

				for (int j = 0; j <= sectorCount; j++)
				{
					sectorAngle = j * sectorStep;

					x = xy * MathF.Cos(sectorAngle);
					y = xy * MathF.Sin(sectorAngle);

					listofVertices.Add(x);
					listofVertices.Add(y);
					listofVertices.Add(z);

					s = (float)j / sectorCount;
					t = (float)i / stackCount;
					listofVertices.Add(s);
					listofVertices.Add(t);

				}

			}
			return listofVertices;
		}

		private static List<float> GetBeanVertices(float radius, float height, int sectorCount, int stackCount, out int baseCenterIndex)
		{

			List<float> vertices = new List<float>();
			List<float> unitVertices = GetUnitCircleVertices(sectorCount);

			// put side vertices to arrays
			for (int i = 0; i < 2; ++i)
			{
				float h = -height / 2.0f + i * height;           // z value; -h/2 to h/2
				float t = 1.0f - i ;                             // vertical tex coord; 1 to 0

				for (int j = 0, k = 0; j <= sectorCount; ++j, k += 3)
				{
					float ux = unitVertices[k];
					float uy = unitVertices[k + 1];

					// position vector
					vertices.Add(ux * radius);             // vx
					vertices.Add(uy * radius);             // vy
					vertices.Add(h);                       // vz

					// texture coordinate
					vertices.Add((float)j / sectorCount);  // s
					vertices.Add(t * 0.5f);                // t
				}
			}

			baseCenterIndex = (vertices.Count / 5);		   // used later for indices


			// top and bottom half-spheres
			List<float> topVerts = GetVerticesHalfSphere(radius, height, sectorCount, stackCount, true);
			List<float> bottomVerts = GetVerticesHalfSphere(radius, height, sectorCount, stackCount, false);

			vertices.AddRange(topVerts);
			vertices.AddRange(bottomVerts);


			return vertices;
		}

		private static List<uint> GetIndices(int sectorCount, int stackCount, int baseCenterIndex)
		{
			List<int> indices = new List<int>();

			int k1 = 0;                         // 1st vertex index at base
			int k2 = sectorCount + 1;           // 1st vertex index at top

			// indices for the side surface
			for (int i = 0; i < sectorCount; ++i, ++k1, ++k2)
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

			// indices for the top half-sphere
			for (int i = 0; i < stackCount / 2; i++)
			{
				k1 = (i * (sectorCount + 1)) + baseCenterIndex;
				k2 = (k1 + sectorCount + 1);

				for (int j = 0; j < sectorCount; j++, k1++, k2++)
				{
					if (i != 0)
					{
						indices.Add(k1);
						indices.Add(k2);
						indices.Add(k1 + 1);
					}

					if (i != stackCount - 1)
					{
						indices.Add(k1 + 1);
						indices.Add(k2);
						indices.Add(k2 + 1);
					}
				}
			}
			// indices for the bottom half-sphere
			for (int i = (stackCount / 2) + 1; i <= stackCount; i++)
			{
				k1 = (i * (sectorCount + 1)) + baseCenterIndex;
				k2 = (k1 + sectorCount + 1);

				for (int j = 0; j < sectorCount; j++, k1++, k2++)
				{
					if (i != 0)
					{
						indices.Add(k1);
						indices.Add(k2);
						indices.Add(k1 + 1);
					}

					if (i != stackCount)
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

	}
}
