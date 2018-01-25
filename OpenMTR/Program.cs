﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace OpenMTR
{
    partial class Program
    {
        public static string DirectoryPath { get; set; }
        public const bool DEBUG = true;

        static void Main(string[] args)
        {
            Welcome.WelcomeUser();
            Console.Read();
        }
    }
}
