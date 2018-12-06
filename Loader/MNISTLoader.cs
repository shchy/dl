using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using dl.DL;

namespace dl
{
    class Image
    {
        public int[] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    class MNISTLoader
    {
        public IEnumerable<ILearningData> Load(string labelFile, string imageFile)
        {
            var labels = ReadLabel(labelFile).ToArray();
            var images = ReadImage(imageFile).ToArray();
            return images.Zip(labels, (i, l) =>
            {
                return new LearningData
                {
                    Data = i.Data.Select(x => (double)x).ToArray(),
                    Expected = Enumerable.Range(0, 10).Select(n => n == l ? 1.0 : 0.0).ToArray(),
                    Name = l.ToString(),
                };
            });
        }

        IEnumerable<int> ReadLabel(string filePath)
        {
            using (var r = new BinaryReader(File.OpenRead(filePath)))
            {
                var id = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
                var fileCount = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
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
                var id = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
                var fileCount = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
                var width = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
                var height = BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);

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

        public void ToBitmap(string labelFile, string imageFile, string saveFolder)
        {
            var labels = ReadLabel(labelFile).ToArray();
            var images = ReadImage(imageFile).ToArray();
            var zipped = images.Zip(labels, (img, l) => (img, l)).ToArray();


            if (Directory.Exists(saveFolder))
                Directory.Delete(saveFolder, true);
            for (var i = 0; i < zipped.Length; i++)
            {
                (Image img, int label) = zipped[i];
                var data = img.Data.Select(x => (byte)x).ToArray();

                using (var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed))
                {
                    // カラーパレットを設定
                    var pal = bmp.Palette;
                    for (int j = 0; j < 256; ++j)
                    {
                        pal.Entries[j] = Color.FromArgb(j, j, j);
                    }
                    bmp.Palette = pal;

                    // BitmapDataに用意したbyte配列を一気に書き込む
                    var bmpdata = bmp.LockBits(
                        new Rectangle(0, 0, img.Width, img.Height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed
                    );
                    Marshal.Copy(data, 0, bmpdata.Scan0, data.Length);
                    bmp.UnlockBits(bmpdata);


                    if (Directory.Exists(saveFolder) == false)
                        Directory.CreateDirectory(saveFolder);
                    bmp.Save(Path.Combine(saveFolder, $"{i.ToString("00000")}_{label}.bmp"));
                }
            }
        }
    }
}