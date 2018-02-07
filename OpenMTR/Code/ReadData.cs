﻿using System.IO;
using System;
using System.Text.RegularExpressions;
using OpenCvSharp;
using System.Collections.Generic;

namespace OpenMTR
{
    public static class ReadData
    {
        // Get a list of data objects built from all the images found in a directory
        public static List<Meter> GetDataObjectList(string directoryPath)
        {
            List<Meter> data = new List<Meter>();
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (Regex.IsMatch(filePath, @"\.(jpe?g|png)$", RegexOptions.IgnoreCase))
                {
                    data.Add(GetDataObject(filePath));
                }
            }
            return data;
        }
        public static void GetDataObjectList(string directoryPath, List<Meter> data)
        {
            data = GetDataObjectList(directoryPath);
        }

        // Get a single data object from a path to the image file
        public static Meter GetDataObject(string imagePath)
        {
            string Filename = Path.GetFileNameWithoutExtension(imagePath);
            return new Meter(Filename, new Mat(imagePath), new Mat(imagePath), ReadMetaData(imagePath));
        }

        // Get a metadata file path with the same name as the provided file
        private static int ReadMetaData(string imagePath)
        {
            string metaPath = Regex.Replace(Path.GetFullPath(imagePath), @"\.(jpe?g|png)$", ".txt", RegexOptions.IgnoreCase);
            try
            {
                return int.Parse(File.ReadAllText(metaPath));
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
