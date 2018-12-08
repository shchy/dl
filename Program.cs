using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dl.DL;

namespace dl
{
    class Program
    {
        static void Main(string[] args)
        {

            // var labelFile = "train-labels-idx1-ubyte";
            // var imageFile = "train-images-idx3-ubyte";
            // var testData = new MNISTLoader().Load(labelFile, imageFile).ToArray();

            // ConvertData.ToBitmap(testData, 28, "./temp/mnist");

            // ConvertData.ToBitmapMoveObject(testData, 28, 56, 56, "./temp/mnistMove");


            var a = new CNN();
            a.Run();
        }
    }
}
