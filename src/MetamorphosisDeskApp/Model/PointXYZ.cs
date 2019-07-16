using System;
using System.Globalization;

namespace MetamorphosisDeskApp.Model
{
    public class PointXYZ
    {
        double X { get; }
        double Y { get; }
        double Z { get; }

        public PointXYZ(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static PointXYZ Center(PointXYZ a, PointXYZ b)
        {
            return new PointXYZ((a.X+b.X)/2, (a.Y + b.Y) / 2, (a.Z + b.Z) / 2);
        }

        public static PointXYZ ParsePoint(string input)
        {
            if (String.IsNullOrEmpty(input)) return null;

            string[] pieces = input.Split(',');
            if (pieces.Length != 3) return null;

            return new PointXYZ(Double.Parse(pieces[0], CultureInfo.InvariantCulture), Double.Parse(pieces[1], CultureInfo.InvariantCulture), Double.Parse(pieces[2], CultureInfo.InvariantCulture));
        }

        public static string ToString(PointXYZ point)
        {
            return $"{point.X} {point.Y} {point.Z}";
        }
    }
}