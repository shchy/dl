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
    // class Image
    // {
    //     public int[] Data { get; set; }
    //     public int Width { get; set; }
    //     public int Height { get; set; }
    // }

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

        public void ToPositionMoveWithMakeBackground(string bitmapFile, int backgroundWidth, int backgroundHeight, string saveFolder)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var bg = new byte[backgroundWidth * backgroundHeight];
            for (var i = 0; i < bg.Length; i++)
            {
                bg[i] = (byte)random.Next(256);
            }

            // 元画像を描画
            using (var bmp = (Bitmap)Bitmap.FromFile(bitmapFile))
            {
                var width = bmp.Width;
                var height = bmp.Height;
                // 移動位置をランダムに決定
                var moveX = random.Next(backgroundWidth - width);
                var moveY = random.Next(backgroundHeight - height);

                for (var x = 0; x < width; x++)
                {
                    var bgx = moveX + x;
                    for (var y = 0; y < height; y++)
                    {
                        var bgy = moveY + y;
                        bg[bgy * backgroundWidth + bgx] = bmp.GetPixel(x, y).R;
                    }
                }
            }

            using (var bmp = new Bitmap(backgroundWidth, backgroundHeight, PixelFormat.Format8bppIndexed))
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
                    new Rectangle(0, 0, backgroundWidth, backgroundHeight),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed
                );
                Marshal.Copy(bg, 0, bmpdata.Scan0, bg.Length);
                bmp.UnlockBits(bmpdata);

                if (Directory.Exists(saveFolder) == false)
                    Directory.CreateDirectory(saveFolder);
                bmp.Save(Path.Combine(saveFolder, Path.GetFileName(bitmapFile)));
            }
        }
    }
}