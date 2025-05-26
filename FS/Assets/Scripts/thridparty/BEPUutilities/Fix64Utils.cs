// Pow, Log2 function based on FixedPointy:
/* FixedPointy - A simple fixed-point math library for C#.
 * 
 * Copyright (c) 2013 Jameson Ernst
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */


using FixMath.NET;
using System;

namespace BEPUutilities
{
	public static class Fix64Utils
	{
		public static readonly Fix64 PointNineNine = (Fix64).99m;
		public static readonly Fix64 PointNine = (Fix64).9m;
		public static readonly Fix64 OnePointFive = (Fix64)1.5m;
		public static readonly Fix64 PointFive = (Fix64)0.5m;
		public static readonly Fix64 PointOne = (Fix64)0.1m;
		public static readonly Fix64 PointSevenFive = (Fix64)0.75m;
		public static readonly Fix64 PointOneFive = (Fix64)0.15m;
		public static readonly Fix64 PointZeroSixTwoFive = (Fix64)0.0625m;
		public static readonly Fix64 OnePointOne = (Fix64)1.1m;
		public static readonly Fix64 OneThird = Fix64.One / 3;
		public static readonly Fix64 OneEighth = Fix64.One / 8;
		public static readonly Fix64 FourThirds = new Fix64(4) / 3;
		public static readonly Fix64 TwoFifths = new Fix64(2) / 5;
		public static readonly Fix64 PointTwoFive = (Fix64)0.25m;
		public static readonly Fix64 PointTwo = (Fix64)0.2m;
		public static readonly Fix64 PointThree = (Fix64)0.3m;
		public static readonly Fix64 PointEight = (Fix64)0.8m;
		public static readonly Fix64 PointZeroOne = (Fix64)0.01m;
		public static readonly Fix64 PointZeroZeroOne = (Fix64)0.001m;
		public static readonly Fix64 EMinusFourteen = (Fix64)1e-14m;
		public static readonly Fix64 EMinusNine = (Fix64)1e-9m;
		public static readonly Fix64 EMinusSeven = (Fix64)1e-7m;
		public static readonly Fix64 EMinusFive = (Fix64)1e-5m;
		public static readonly Fix64 EMinusFour = (Fix64)1e-4m;
		public static readonly Fix64 EMinusTen = (Fix64)1e-10m;
		public static readonly Fix64 MinusPointTwoFive = (Fix64)(-0.25m);
		public static readonly Fix64 MinusEMinusNine = (Fix64)(-1e-9m);
		public static readonly Fix64 MinusPointNineNineNineNine = (Fix64)(-0.9999m);
		public static readonly Fix64 OneMinusEMinus12 = 1 - (Fix64)1e-12m;
		public static readonly Fix64 GoldenRatio = 1 + Fix64.Sqrt((Fix64)5) / 2;
		public static readonly Fix64 OneOverTwelve = 1 / (Fix64)12;
		public static readonly Fix64 PointZeroEightThrees = (Fix64).0833333333m;
		
		public static Fix64 Sign(Fix64 v)
		{
			long raw = v.RawValue;
			return
				raw < 0 ? -1 :
				raw > 0 ? 1 :
				0;
		}
	}
}
