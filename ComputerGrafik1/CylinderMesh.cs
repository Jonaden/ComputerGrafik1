using OpenTK.Graphics.OpenGL4;

namespace ComputerGrafik1
{
	public class CylinderMesh : Mesh
	{


		// Inherited from Mesh
		private static int vertexArrayObject;
		private static int vertexBufferObject;
		private static int elementBufferObject;
		private static bool buffersCreated;

		public override void Draw()
		{
			GL.BindVertexArray(vertexArrayObject);
			GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
			//GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length);
		}

		protected override void GenerateBuffers()
		{
			int sectorCount = 16;
			float height = 4;
			float radius = 1;

			Vertices = GetCylinderVertices(sectorCount, height, radius, out int baseCenterIndex).ToArray();
			Indices = GetCylinderIndices(sectorCount, baseCenterIndex).ToArray();

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
				vertices.Add(0);					  // z
			}

			return vertices;
		}

		private static List<float> GetCylinderVertices(int sectorCount, float height, float radius, out int baseCenterIndex)
		{

			List<float> vertices = new List<float>();
			List<float> unitVertices = GetUnitCircleVertices(sectorCount);

			// put side vertices to arrays
			for (int i = 0; i < 2; ++i)
			{
				float h = -height / 2.0f + i * height;           // z value; -h/2 to h/2
				float t = 1.0f - i;                              // vertical tex coord; 1 to 0

				for (int j = 0, k = 0; j <= sectorCount; ++j, k += 3)
				{
					float ux = unitVertices[k];
					float uy = unitVertices[k + 1];

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


		private static List<uint> GetCylinderIndices(int sectorCount, int baseCenterIndex)
		{
			List<int> indices = new List<int>();
			int topCenterIndex = baseCenterIndex + sectorCount + 1; // include center vertex

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

			// indices for the base surface
			for (int i = 0, k = baseCenterIndex + 1; i < sectorCount; ++i, ++k)
			{
				if (i < sectorCount - 1)
				{
					indices.Add(baseCenterIndex);
					indices.Add(k + 1);
					indices.Add(k);
				}
				else // last triangle
				{
					indices.Add(baseCenterIndex);
					indices.Add(baseCenterIndex + 1);
					indices.Add(k);
				}
			}

			// indices for the top surface
			for (int i = 0, k = topCenterIndex + 1; i < sectorCount; ++i, ++k)
			{
				if (i < sectorCount - 1)
				{
					indices.Add(topCenterIndex);
					indices.Add(k);
					indices.Add(k + 1);
				}
				else // last triangle
				{
					indices.Add(topCenterIndex);
					indices.Add(k);
					indices.Add(topCenterIndex + 1);
				}
			}

			List<uint> uints = indices.Select(i => (uint)i).ToList();

			return uints;
		}


	}
}
