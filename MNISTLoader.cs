using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dl.DL;

namespace dl
{
    class Image{
        public int[] Data {get; set;}
        public int Width { get; set;}
        public int Height { get; set;}
         
    }

    class MNISTLoader
    {
        public IEnumerable<ILearningData> Load()
        {
            var labelFile = "train-labels-idx1-ubyte";
            var imageFile = "train-images-idx3-ubyte";
            var labels = ReadLabel(labelFile).ToArray();
            var images = ReadImage(imageFile).ToArray();
            return images.Zip(labels, (i,l) =>
            {
                return new LearningData
                {
                    Data = i.Data.Select(x=>(double)x).ToArray(),
                    Expected = Enumerable.Range(0,10).Select(n=> n == l ? 1.0 : 0.0).ToArray(),
                    Name = l.ToString(),
                };
            });
        }

        IEnumerable<int> ReadLabel(string filePath)
        {
            using (var r = new BinaryReader(File.OpenRead(filePath)))
            {
                var id = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray());
                var fileCount = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray());
                for (var i = 0; i < fileCount; i++)
                {
                    yield return r.ReadSByte();
                }
            }
        }

        IEnumerable<Image> ReadImage(string filePath)
        {
            using (var r = new BinaryReader(File.OpenRead(filePath)))
            {
                var id = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray());
                var fileCount = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray());
                var width = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray());
                var height = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray());
            
                for (var i = 0; i < fileCount; i++)
                {
                    var data = 
                        from px in Enumerable.Range(0, width * height)
                        select (int)r.ReadSByte();
                    
                    yield return new Image
                    {
                        Width = width,
                        Height = height,
                        Data = data.ToArray(),
                    };
                }
            }
        }

    }


}