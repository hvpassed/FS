using FixMath.NET;
using System;

namespace BEPUutilities
{
	struct Matrix3x6
	{
		public Fix64[,] M;

		public Matrix3x6(Matrix3x3 m)
		{
			M = new Fix64[3, 6];

			M[0, 0] = m.M11;
			M[0, 1] = m.M12;
			M[0, 2] = m.M13;
			M[1, 0] = m.M21;
			M[1, 1] = m.M22;
			M[1, 2] = m.M23;
			M[2, 0] = m.M31;
			M[2, 1] = m.M32;
			M[2, 2] = m.M33;

			M[0, 3] = Fix64.One;
			M[1, 4] = Fix64.One;
			M[2, 5] = Fix64.One;
		}

		public static bool Gauss(Fix64[,] M, int m, int n)
		{
			// Perform Gauss-Jordan elimination
			for (int k = 0; k < m; k++)
			{
				Fix64 maxValue = Fix64.Abs(M[k, k]);
				int iMax = k;
				for (int i = k+1; i < m; i++)
				{
					Fix64 value = Fix64.Abs(M[i, k]);
					if (value >= maxValue)
					{
						maxValue = value;
						iMax = i;
					}
				}
				if (M[iMax, k] == 0)
					return false;
				// Swap rows k, iMax
				if (k != iMax)
				{
					for (int j = 0; j < n; j++)
					{
						Fix64 temp = M[k, j];
						M[k, j] = M[iMax, j];
						M[k, j] = temp;
					}
				}

				// Divide row by pivot
				Fix64 pivot = M[k, k];
				M[k, k] = 1;
				for (int j = k + 1; j < n; j++)
				{
					M[k, j] /= pivot;
				}

				for (int i = k + 1; i < m; i++)
				{
					Fix64 f = M[i, k] / M[k, k];
					/* Do for all remaining elements in current row: */
					for (int j = k + 1; j < n; j++)
					{
						M[i, j] = M[i, j] - M[k, j] * f;
					}
					/* Fill lower triangular matrix with zeros: */
					M[i, k] = 0;
				}
			}
			return true;
		}
		
		public bool Invert(out Matrix3x3 r)
		{
			if (!Gauss(M, 3, 6))
			{
				r = new Matrix3x3();
				return false;
			}
			r = new Matrix3x3(
				// m11...m13
				M[0, 3],
				M[0, 4],
				M[0, 5],

				// m21...m23
				M[1, 3],
				M[1, 4],
				M[1, 5],

				// m31...m33
				M[2, 3],
				M[2, 4],
				M[2, 5]
				);
			return true;
		}
	}
}
