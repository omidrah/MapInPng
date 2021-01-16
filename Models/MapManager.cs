using System;
using System.Collections.Generic;
using System.Drawing;

namespace ReportGenerator.Models
{
    public class MapManager
    {
        const double LogEarth = 7.6028737118484695295545741009651;  // Math.Log10(EarthRadius * 2 * PI)
        const double Log256 = 2.4082399653118495617099111577959;    // Math.Log10(256)
        const double Log2 = 0.30102999566398119521373889472449;     //Math.Log10(2)
        const double EarthRadius = 6378137;
        const double EarthPerimeter = EarthRadius * 2 * Math.PI;

        public int ZoomLevel { get; private set; }
        private double zoomPow2;
        public MapManager(double upp)
        {
            ZoomLevel = (int)(Math.Round((LogEarth - Log256 - Math.Log10(upp)) / Log2));
            zoomPow2 = Math.Pow(2, ZoomLevel);
        }

        const double MaxUPP = 39135.758482010240383961736095118; //Google map zoom = 2
        const double MinUPP = 0.29858214173896972949189556957335;//Google map zoom = 19
        public static double CorrectUPP(double upp)
        {//real zoom range is between 2 and 19
            if (upp < MinUPP)
                return MinUPP;
            if (upp > MaxUPP)
                return MaxUPP;
            for (double u = MinUPP; u <= MaxUPP; u *= 2)
            {
                if (u > upp || Math.Abs(u - upp) < 0.0001)
                    return u;
            }
            return upp;
        }

        public double CalcUpp()
        {
            return Math.Pow(10, LogEarth - ZoomLevel * Log2 - Log256);
        }
        int CalcTileX(double x)
        {
            return (int)((0.5 + (x / EarthPerimeter)) * zoomPow2);
        }

        int CalcTileY(double y)
        {
            return (int)((0.5 - (y / EarthPerimeter)) * zoomPow2);
        }

        double CalcX(int tileX)
        {
            return ((tileX / zoomPow2) - 0.5) * EarthPerimeter;
        }

        private double CalcY(int tileY)
        {
            return (0.5 - (tileY / zoomPow2)) * EarthPerimeter;
        }
        public RectangleD CalcTileBound(int tileX, int tileY)
        {
            double left = CalcX(tileX);
            double top = CalcY(tileY);
            double right = CalcX(tileX + 1);
            double bottom = CalcY(tileY + 1);
            return new RectangleD(left, bottom, right - left, top - bottom);
        }

        public List<Point> FindNeededTiles(RectangleD boundry)
        {
            int left = CalcTileX(boundry.Left);
            int right = CalcTileX(boundry.Right);
            int top = CalcTileY(boundry.Top);
            int bottom = CalcTileY(boundry.Bottom);
            List<Point> points = new List<Point>();
            for (int x = left; x <= right; ++x)
            {
                for (int y = top; y <= bottom; ++y)
                    points.Add(new Point(x, y));
            }
            return points;
        }
    }
    public enum AddObjectModes { AddOnly, DrawIt, EnsureVisible }
}
