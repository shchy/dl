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
    public class ConvertData
    {
        public static void ToBitmap(ILearningData[] data, int size, string saveFolder)
        {
            Func<int, string> makeSavePath = (int index) => Path.Combine(saveFolder, $"{index.ToString("00000")}.bmp");

            var loadFile =
                new XDocument(
                    new XElement("dataset",
                        from item in data.Select((img, index) => new { img, index })
                        select
                            new XElement("data",
                                new XElement("path", makeSavePath(item.index)),
                                new XElement("size",
                                    new XAttribute("width", size),
                                    new XAttribute("height", size)),
                                new XElement("objects",
                                    new XElement("object",
                                        new XElement("name", DLF.FindMaxValueIndex(item.img.Expected)),
                                        new XElement("position",
                                            new XAttribute("x", 0),
                                            new XAttribute("y", 0),
                                            new XAttribute("width", size),
                                            new XAttribute("height", size)
                                        )
                                    )
                                )
                            )
                    )
                );

            if (Directory.Exists(saveFolder))
                Directory.Delete(saveFolder, true);
            for (var i = 0; i < data.Length; i++)
            {
                var img = data[i].Data;
                var bitmapData = img.Select(x => (byte)x).ToArray();

                using (var bmp = new Bitmap(size, size, PixelFormat.Format8bppIndexed))
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
                        new Rectangle(0, 0, size, size),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed
                    );
                    Marshal.Copy(bitmapData, 0, bmpdata.Scan0, bitmapData.Length);
                    bmp.UnlockBits(bmpdata);

                    if (Directory.Exists(saveFolder) == false)
                        Directory.CreateDirectory(saveFolder);
                    bmp.Save(makeSavePath(i));
                }
            }

            loadFile.Save(Path.Combine(saveFolder, $"files.xml"));
        }

        public static void ToBitmapMoveObject(ILearningData[] data, int size
                                            , int backgroundWidth, int backgroundHeight
                                            , string saveFolder)
        {
            Func<int, string> makeSavePath = (int index) => Path.Combine(saveFolder, $"{index.ToString("00000")}.bmp");


            if (Directory.Exists(saveFolder))
                Directory.Delete(saveFolder, true);
            if (Directory.Exists(saveFolder) == false)
                Directory.CreateDirectory(saveFolder);

            for (var i = 0; i < data.Length; i++)
            {
                var img = data[i].Data.ToArray();
                (string path, int moveX, int moveY, int width, int height) = Move(img, size, backgroundWidth, backgroundHeight, makeSavePath(i));
            }
            var loadFile =
                new XDocument(
                    new XElement("dataset",
                        from item in data.Select((img, index) => new { img, index })
                        let move = Move(item.img.Data.ToArray(), size, backgroundWidth, backgroundHeight, makeSavePath(item.index))
                        select
                            new XElement("data",
                                new XElement("path", move.path),
                                new XElement("size",
                                    new XAttribute("width", backgroundWidth),
                                    new XAttribute("height", backgroundHeight)),
                                new XElement("objects",
                                    new XElement("object",
                                        new XElement("name", DLF.FindMaxValueIndex(item.img.Expected)),
                                        new XElement("position",
                                            new XAttribute("x", move.moveX),
                                            new XAttribute("y", move.moveY),
                                            new XAttribute("width", move.width),
                                            new XAttribute("height", move.height)
                                        )
                                    )
                                )
                            )
                    )
                );


            loadFile.Save(Path.Combine(saveFolder, $"files.xml"));
        }

        static (string path, int moveX, int moveY, int width, int height) Move(double[] img, int size
                                                                            , int backgroundWidth, int backgroundHeight
                                                                            , string savePath)
        {
            var random = new Random(DateTime.Now.Millisecond);
            // 背景
            var bg = new byte[backgroundWidth * backgroundHeight];
            for (var p = 0; p < bg.Length; p++)
            {
                bg[p] = (byte)random.Next(256);
            }

            var width = size;
            var height = size;
            // 移動位置をランダムに決定
            var moveX = random.Next(backgroundWidth - width);
            var moveY = random.Next(backgroundHeight - height);

            for (var x = 0; x < width; x++)
            {
                var bgx = moveX + x;
                for (var y = 0; y < height; y++)
                {
                    var bgy = moveY + y;
                    bg[bgy * backgroundWidth + bgx] = (byte)img[y * width + x];
                }
            }

            // Bitmap化
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

                bmp.Save(savePath);
            }
            return (savePath, moveX, moveY, width, height);
        }
    }
}