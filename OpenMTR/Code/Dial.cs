﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMTR
{
    public static class Dial
    {
        public static string Read(Meter meter, List<CircleSegment> dials)
        {
            return ExtractDialDigits(meter, SortDials(dials));
        }

        private static List<CircleSegment> SortDials(List<CircleSegment> dials)
        {
            return dials.OrderBy(dial => dial.Center.X).ToList();
        }

        private static Point DetectNeedleTip(Mat dial, Point centerOfNeedle)
        {
            double distance = 0f;
            Point needlePoint = new Point();
            ImageUtils.ColorToGray(dial, dial);
            Cv2.Dilate(dial, dial, ImageUtils.GetKernel(new Size(5, 5)));
            Cv2.MorphologyEx(dial, dial, MorphTypes.Open, ImageUtils.GetKernel(new Size(3, 3)));
            Cv2.MorphologyEx(dial, dial, MorphTypes.Close, ImageUtils.GetKernel(new Size(3, 3)));
            Cv2.Canny(dial, dial, 100, 200);
            Cv2.FindContours(dial, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);
            foreach (Point[] points in contours)
            {
                foreach (Point point in points)
                {
                    if (centerOfNeedle.DistanceTo(point) > distance)
                    {
                        needlePoint = point;
                        distance = centerOfNeedle.DistanceTo(point);
                    }
                }
            }
            return needlePoint;
        }

        private static string ExtractDialDigits(Meter meter, List<CircleSegment> circles)
        {
            StringBuilder readValue = new StringBuilder("????");
            for (int i = 0; i < circles.Count; i++)
            {
                CircleSegment circle = circles[i];
                int rectX = (int)(circle.Center.X - circle.Radius),
                    rectY = (int)(circle.Center.Y - circle.Radius),
                    rectW = (int)(circle.Radius * 2 + 1),
                    rectH = (int)(circle.Radius * 2 + 1);
                if ((rectX + rectW) > meter.SourceImage.Width)
                {
                    rectW = (rectX + rectW) - meter.SourceImage.Width;
                }
                if ((rectY + rectH) > meter.SourceImage.Height)
                {
                    rectW = (rectY + rectH) - meter.SourceImage.Height;
                }
                Mat dial = new Mat(meter.SourceImage, new Rect(rectX, rectY, rectW, rectH));
                Point center = new Point(dial.Width / 2, dial.Height / 2);
                Point needleTip = DetectNeedleTip(dial, center);
                foreach (KeyValuePair<char, List<Point>> numberPosition in (MathUtils.IsEven(i)) ? CreateCounterClockwiseSegments(dial, center) : CreateClockwiseSegments(dial, center))
                {
                    if (MathUtils.IsPointInTriangle(needleTip, center, numberPosition.Value[0], numberPosition.Value[1]))
                    {
                        readValue[i] = numberPosition.Key;
                    }
                }
            }
            return readValue.ToString();
        }

        private static Dictionary<char, List<Point>> CreateClockwiseSegments(Mat dial, Point center)
        {
            return new Dictionary<char, List<Point>>()
            {
                { '0', new List<Point>() { new Point(center.X * 0.30, 0), new Point(center.X + (center.X * 0.70), 0) } },
                { '1', new List<Point>() { new Point(center.X + (center.X * 0.70), 0), new Point(dial.Width, center.Y * 0.65) } },
                { '2', new List<Point>() { new Point(dial.Width, center.Y * 0.65), new Point(dial.Width, center.Y + (center.Y * 0.30)) } },
                { '3', new List<Point>() { new Point(dial.Width, center.Y + (center.Y * 0.30)), new Point(center.X + (center.X * 0.65), dial.Height) } },
                { '4', new List<Point>() { new Point(center.X + (center.X * 0.65), dial.Height), new Point(center.X, dial.Width) } },
                { '5', new List<Point>() { new Point(center.X, dial.Width), new Point(center.X * 0.20, dial.Height) } },
                { '6', new List<Point>() { new Point(center.X * 0.20, dial.Height), new Point(0, center.Y + (center.Y * 0.30)) } },
                { '7', new List<Point>() { new Point(0, center.Y + (center.Y * 0.30)), new Point(0, center.Y * 0.65) } },
                { '8', new List<Point>() { new Point(0, center.Y * 0.65), new Point(center.X * 0.30, 0) } },
                { '9', new List<Point>() { new Point(center.X * 0.30, 0), new Point(center.X, 0) } },
            };
        }

        private static Dictionary<char, List<Point>> CreateCounterClockwiseSegments(Mat dial, Point center)
        {
            return new Dictionary<char, List<Point>>()
            {
                { '0', new List<Point>() { new Point(center.X, 0), new Point(center.X * 0.30, 0) } },
                { '1', new List<Point>() { new Point(center.X * 0.30, 0), new Point(0, center.Y * 0.65) } },
                { '2', new List<Point>() { new Point(0, center.Y * 0.65), new Point(0, center.Y + (center.Y * 0.30)) } },
                { '3', new List<Point>() { new Point(0, center.Y + (center.Y * 0.30)), new Point(center.X * 0.20, dial.Height) } },
                { '4', new List<Point>() { new Point(center.X * 0.20, dial.Height), new Point(center.X, dial.Width) } },
                { '5', new List<Point>() { new Point(center.X, dial.Width), new Point(center.X + (center.X * 0.65), dial.Height) } },
                { '6', new List<Point>() { new Point(center.X + (center.X * 0.65), dial.Height), new Point(dial.Width, center.Y + (center.Y * 0.30)) } },
                { '7', new List<Point>() { new Point(dial.Width, center.Y + (center.Y * 0.30)), new Point(dial.Width, center.Y * 0.65) } },
                { '8', new List<Point>() { new Point(dial.Width, center.Y * 0.65), new Point(center.X + (center.X * 0.70), 0) } },
                { '9', new List<Point>() { new Point(center.X + (center.X * 0.70), 0), new Point(center.X, 0) } }
            };
        }
    }
}
