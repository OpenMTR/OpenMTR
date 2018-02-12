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

        public static void ApplyGaussianBlur(Mat sourceImage, Mat destinationImage)
        {
            Cv2.GaussianBlur(sourceImage, destinationImage, new Size(3, 3), 0, 0, BorderTypes.Default);
        }

        public static void ApplyGaussianBlur(List<Meter> meterList)
        {
            foreach(Meter meter in meterList)
            {
                Cv2.GaussianBlur(meter.SourceImage, meter.ModifiedImage, new Size(3, 3), 0, 0, BorderTypes.Default);
            }
        }

        public static Mat GetKernel(Size size)
        {
            return Cv2.GetStructuringElement(MorphShapes.Rect, size);
        }

        public static void RotateImage(Mat sourceImage, Mat destinationImage, double degrees)
        {
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(new Point2f(sourceImage.Cols / 2, sourceImage.Rows / 2), degrees, 1);
            Cv2.WarpAffine(sourceImage, destinationImage, rotationMatrix, sourceImage.Size());
        }

        public static float DetectImageSkew(Mat sourceImage, Mat destinationImage)
        {
            List<LineSegmentPolar> filteredLines = new List<LineSegmentPolar>();
            float theta_min = (float)(60f * Cv2.PI / 180f),
                  theta_max = (float)(120f * Cv2.PI / 180f),
                  theta_avr = 0f,
                  theta_deg = 0f;

            LineSegmentPolar[] lines = Cv2.HoughLines(destinationImage, 1, Cv2.PI / 180, 200);

            foreach (LineSegmentPolar line in lines)
            {
                float theta = line.Theta;
                if (theta >= theta_min && theta <= theta_max)
                {
                    filteredLines.Add(line);
                    theta_avr += theta;
                }
            }

            if (filteredLines.Count > 0)
            {
                theta_avr /= filteredLines.Count;
                theta_deg = (float)(theta_avr / Cv2.PI * 180f) - 90;
                DebugUtils.Log("Detected skew: " + theta_deg);
            }
            else
            {
                DebugUtils.Log("Failed to detect skew");
            }

            //DebugUtils.DrawLines(sourceImage, filteredLines);

            return theta_deg;
        }
    }
}
