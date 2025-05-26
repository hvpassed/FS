using FixMath.NET;
using System;

namespace BEPUutilities
{
	struct Matrix4x8
	{
		public Fix64[,] M;

		public Matrix4x8(Matrix m)
		{
			M = new Fix64[4, 8];

			M[0, 0] = m.M11;
			M[0, 1] = m.M12;
			M[0, 2] = m.M13;
			M[0, 3] = m.M14;
			M[1, 0] = m.M21;
			M[1, 1] = m.M22;
			M[1, 2] = m.M23;
			M[1, 3] = m.M24;
			M[2, 0] = m.M31;
			M[2, 1] = m.M32;
			M[2, 2] = m.M33;
			M[2, 3] = m.M34;
			M[3, 0] = m.M41;
			M[3, 1] = m.M42;
			M[3, 2] = m.M43;
			M[3, 3] = m.M44;

			M[0, 4] = Fix64.One;
			M[1, 5] = Fix64.One;
			M[2, 6] = Fix64.One;
			M[3, 7] = Fix64.One;
		}

		public void Gauss()
		{
			// Perform Gauss-Jordan elimination
			for (int k = 0; k < 4; k++)
			{
				Fix64 maxValue = Fix64.Abs(M[k, k]);
				int iMax = k;
				for (int i = k+1; i < 4; i++)
				{
					Fix64 value = Fix64.Abs(M[i, k]);
					if (value >= maxValue)
					{
						maxValue = value;
						iMax = i;
					}
				}
				if (M[iMax, k] == 0)
					throw new ArgumentException("Matrix is singular");
				// Swap rows k, iMax
				if (k != iMax)
				{
					for (int j = 0; j < 8; j++)
					{
						Fix64 temp = M[k, j];
						M[k, j] = M[iMax, j];
						M[k, j] = temp;
					}
				}

				// Divide row by pivot
				Fix64 pivot = M[k, k];
				M[k, k] = 1;
				for (int j = k + 1; j < 6; j++)
				{
					M[k, j] /= pivot;
				}

				for (int i = k + 1; i < 3; i++)
				{
					Fix64 f = M[i, k] / M[k, k];
					/* Do for all remaining elements in current row: */
					for (int j = k + 1; j < 6; j++)
					{
						M[i, j] = M[i, j] - M[k, j] * f;
					}
					/* Fill lower triangular matrix with zeros: */
					M[i, k] = 0;
				}
			}
		}

		public bool Invert(out Matrix r)
		{
			if (!Matrix3x6.Gauss(M, 4, 8))
			{
				r = new Matrix();
				return false;
			}
			r = new Matrix(
				// m11...m14
				M[0, 4],
				M[0, 5],
				M[0, 6],
				M[0, 7],

				// m21...m24				
				M[1, 4],
				M[1, 5],
				M[1, 6],
				M[1, 7],

				// m31...m34
				M[2, 4],
				M[2, 5],
				M[2, 6],
				M[2, 7],

				// m41...m44
				M[3, 4],
				M[3, 5],
				M[3, 6],
				M[3, 7]
				);
			return true;
		}
	}
}
