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
            // new MNISTLoader().ToBitmap(labelFile, imageFile, "./temp");

            var a = new CNN();
            a.Run();
        }
    }
}
