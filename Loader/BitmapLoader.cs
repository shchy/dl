using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using dl.DL;

namespace dl
{
    class BitmapLoader
    {
        public IEnumerable<ILearningData> Load(string loadFile)
        {
            var doc = XDocument.Load(loadFile);

            return
                from data in doc.Root.Elements("data")
                let path = data.Element("path").Value
                let name = data.Element("objects").Elements("object").First().Element("name").Value
                let label = int.Parse(name)
                let ds = LoadBitmap(path).ToArray()
                select new LearningData
                {
                    Data = ds,
                    Name = name,
                    Expected = Enumerable.Range(0, 10).Select(n => n == label ? 1.0 : 0.0).ToArray(),
                };
        }

        private IEnumerable<double> LoadBitmap(string path)
        {
            using (var bmp = (Bitmap)Bitmap.FromFile(path))
            {
                var width = bmp.Width;
                var height = bmp.Height;

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        yield return bmp.GetPixel(x, y).R;
                    }
                }
            }
        }

    }
}