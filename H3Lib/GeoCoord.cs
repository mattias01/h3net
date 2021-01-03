using System;
using System.Diagnostics;
using H3Lib.Extensions;

namespace H3Lib
{
    /// <summary>
    /// Functions for working with lat/lon coordinates.
    /// </summary>
    [DebuggerDisplay("Lat: {Latitude} Lon: {Longitude}")]
    public readonly struct GeoCoord:IEquatable<GeoCoord>
    {
        public readonly double Latitude;
        public readonly double Longitude;

        private static bool _initialized;
        private static double[] _areasKm2;
        private static double[] _areasM2;
        private static double[] _edgeLengthKm;
        private static double[] _edgeLengthM;
 
        public GeoCoord(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            SetArray();
        }

        public GeoCoord(GeoCoord gc)
        {
            Latitude = gc.Latitude;
            Longitude = gc.Longitude;
            SetArray();
        }

        private static void SetArray()
        {
            if (_initialized)
            {
                return;
            }

            _areasKm2 = new[]
                        {
                            4250546.848, 607220.9782, 86745.85403, 12392.26486,
                            1770.323552, 252.9033645, 36.1290521, 5.1612932,
                            0.7373276, 0.1053325, 0.0150475, 0.0021496,
                            0.0003071, 0.0000439, 0.0000063, 0.0000009
                        };
            
            _areasM2 = new[]
                        {
                            4.25055E+12, 6.07221E+11, 86745854035, 12392264862,
                            1770323552, 252903364.5, 36129052.1, 5161293.2,
                            737327.6, 105332.5, 15047.5, 2149.6,
                            307.1, 43.9, 6.3, 0.9
                        };
            
            _edgeLengthKm = new[]
                            {
                                1107.712591, 418.6760055, 158.2446558, 59.81085794,
                                22.6063794, 8.544408276, 3.229482772, 1.220629759,
                                0.461354684, 0.174375668, 0.065907807, 0.024910561,
                                0.009415526, 0.003559893, 0.001348575, 0.000509713
                            };
            _edgeLengthM = new[]
                           {
                               1107712.591, 418676.0055, 158244.6558, 59810.85794,
                               22606.3794, 8544.408276, 3229.482772, 1220.629759,
                               461.3546837, 174.3756681, 65.90780749, 24.9105614,
                               9.415526211, 3.559893033, 1.348574562, 0.509713273
                           };


            _initialized = true;
        }

        /*
         * The following functions provide meta information about the H3 hexagons at
         * each zoom level. Since there are only 16 total levels, these are current
         * handled with hardwired static values, but it may be worthwhile to put these
         * static values into another file that can be autogenerated by source code in
         * the future.
         */
        public static double HexAreaKm2(int res)
        {
            return _areasKm2[res];
        }

        public static double HexAreaM2(int res)
        {
            return _areasM2[res];
        }

        public static double EdgeLengthKm(int res)
        {
            return _edgeLengthKm[res];
        }

        public static double EdgeLengthM(int res)
        {
            return _edgeLengthM[res];
        }

        /// <summary>
        /// Surface area in radians^2 of spherical triangle on unit sphere.
        ///
        /// For the math, see:
        /// https://en.wikipedia.org/wiki/Spherical_trigonometry#Area_and_spherical_excess
        /// </summary>
        /// <param name="a">length of triangle side A in radians</param>
        /// <param name="b">length of triangle side B in radians</param>
        /// <param name="c">length of triangle side C in radians</param>
        /// <returns>area in radians^2 of triangle on unit sphere</returns>
        private static double TriangleEdgeLengthToArea(double a, double b, double c)
        {
            double s = (a + b + c) / 2;

            a = (s - a) / 2;
            b = (s - b) / 2;
            c = (s - c) / 2;
            s /= 2;

            return 4 * Math.Atan
                       (Math.Sqrt(Math.Tan(s) *
                                  Math.Tan(a) * 
                                  Math.Tan(b) *
                                  Math.Tan(c)));
        }

        /// <summary>
        /// Compute area in radians^2 of a spherical triangle, given its vertices.
        /// </summary>
        /// <param name="a">vertex lat/lng in radians</param>
        /// <param name="b">vertex lat/lng in radians</param>
        /// <param name="c">vertex lat/lng in radians</param>
        /// <returns>area of triangle on unit sphere, in radians^2</returns>
        public static double TriangleArea(GeoCoord a, GeoCoord b, GeoCoord c)
        {
            return TriangleEdgeLengthToArea
                (a.DistanceToRadians(b),
                 b.DistanceToRadians(c),
                 c.DistanceToRadians(a));
        }

        public bool Equals(GeoCoord other)
        {
            return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
        }

        public override bool Equals(object obj)
        {
            return obj is GeoCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude);
        }

        public static bool operator ==(GeoCoord left, GeoCoord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GeoCoord left, GeoCoord right)
        {
            return !left.Equals(right);
        }
    }
}
