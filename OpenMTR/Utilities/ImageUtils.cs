﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMTR
{
    public static class ImageUtils
    {
        public static void ColorToGray(Mat sourceImage, Mat destinationImage)
        {
            Cv2.CvtColor(sourceImage, destinationImage, ColorConversionCodes.BGR2GRAY);
        }

        public static void ColorToGray(List<Meter> meterList)
        {
            foreach (Meter meter in meterList)
            {
                Cv2.CvtColor(meter.SourceImage, meter.ModifiedImage, ColorConversionCodes.BGR2GRAY);
            }
        }

        public static Mat GetKernel(Size size)
        {
            return Cv2.GetStructuringElement(MorphShapes.Rect, size);
        }

        public static void AdjustImageSkew(Meter meter)
        {
            RotateImage(meter.SourceImage, meter.ModifiedImage, GetAngle(meter.SourceImage));
        }

        public static void AdjustImageSkew(Mat imageToAdjust)
        {
            RotateImage(imageToAdjust, imageToAdjust, GetAngle(imageToAdjust));
        }

        private static void RotateImage(Mat sourceImage, Mat destinationImage, double degrees)
        {
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(new Point2f(sourceImage.Cols / 2, sourceImage.Rows / 2), degrees, 1);
            Cv2.WarpAffine(sourceImage, destinationImage, rotationMatrix, sourceImage.Size());
        }

        private static double GetAngle(Mat imageToAdjust)
        {
            Mat handler = imageToAdjust.Clone();
            if (imageToAdjust.Type().ToString() != "CV_8UC1")
            {
                ColorToGray(handler, handler);
            }
            
            Cv2.Canny(handler, handler, 100, 100, 3);
            LineSegmentPoint[] lineSegmentPoints = Cv2.HoughLinesP(handler, 1, Cv2.PI / 180.0, 100, minLineLength: 100, maxLineGap: 5);

            for (int i = 0; i < lineSegmentPoints.Length; i++)
            {
                if (lineSegmentPoints[i].P1 == null || lineSegmentPoints[i].P2 == null)
                {
                    continue;
                }
                return Math.Atan2(lineSegmentPoints[i].P2.Y - lineSegmentPoints[i].P1.Y, lineSegmentPoints[i].P2.X - lineSegmentPoints[i].P1.X) * (180 / Math.PI);
            }
            return 0; 
        }
    }
}
