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
            
            var labelFile = "train-labels-idx1-ubyte";
            var imageFile = "train-images-idx3-ubyte";
            var testData = new MNISTLoader().Load(labelFile, imageFile).ToArray();
            
            ConvertData.MNISTToBitmap(testData, 28, "./temp");

            // var bmpLoader = new BitmapLoader();
            // var learningData = bmpLoader.Load("./temp/files.xml");



            // var bmps = Directory.GetFiles("./temp").Where(x => Path.GetExtension(x) == ".bmp").ToArray();
            // var bmp = new BitmapLoader();
            // foreach (var filePath in bmps)
            // {
            //     bmp.ToPositionMoveWithMakeBackground(filePath, 56, 56, "./temp/resize");
            // }

            // var a = new CNN();
            // a.Run();
        }
    }
}
