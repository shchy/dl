using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using dl.DL;

namespace dl
{
    public class DetectTest
    {
        public void Run()
        {
            // 学習データを生成
            var loadCount = 100;
            var data = GetTestData(loadCount).ToArray();

            var model = new CNNModelTest();
            var modelData = "./backup/cnn";

            var batchSize = 8;
            var batchCount = data.Count() / batchSize;
            var epoch = 3;
            var learningRate = 0.005f;
            var validator = new Validator();
            var watch = new Stopwatch();
            var lastep = 0;
            var machine = new Machine(model, learningRate, epoch, batchSize, batchCount, validator
                            , (ep, index) =>
                            {
                                Console.WriteLine($"{index.ToString("00000")}:{(watch.ElapsedMilliseconds) / 1000.0}");
                                watch.Restart();
                                // 1周したら保存しておく
                                if (lastep != ep)
                                {
                                    lastep = ep;
                                    model.Save(modelData);
                                }
                            });
            model.Load(modelData);

            
            foreach (var item in data.Select((d,i)=>(d,i)))
            {
                detect(machine, item.d, item.i.ToString("000"));
            }
        }

        void detect(IMachine machine, ILearningData targetData, string name)
        {

            var exLabel = DLF.FindMaxValueIndex(targetData.Expected);

            var orgRes = machine.Test(targetData.Data).ToArray();
            var orgLabel = DLF.FindMaxValueIndex(orgRes);
            if(orgLabel != exLabel) return;
            var orgProb = orgRes.ElementAt(exLabel);
                

            var boxes = 
                from i in Enumerable.Range(0, 14)
                select (i,i,28-(i*2),28-(i*2));
            var box = boxes.Select(b =>
            {
                var (x,y,w,h) = b;
                var cropped = Crop(targetData.Data, 28, x,y,w,h).ToArray();
                var res = machine.Test(cropped).ToArray();
                var prob = res.ElementAt(exLabel);
                return (orgProb - prob <= 0, b);
            }).TakeWhile( x=> x.Item1).LastOrDefault().b;
            {
                var (x,y,w,h) = box;
                var view = Square(targetData.Data, 28, x,y,w,h, 55).ToArray();
                ConvertData.ToBitmap(view, 28,28, $"./temp/detect/{name}_{x}_{y}_{w}_{h}_e{exLabel}.bmp");
                ConvertData.ToBitmap(targetData.Data, 28,28, $"./temp/detect/{name}_e{exLabel}.bmp");
            }
        }

        IEnumerable<float> Crop(IEnumerable<float> data, int dataWedth, int x, int y, int width, int height, float mask = 0.0f)
        {
            foreach (var item in data.Select((px,i)=> (px,i)))
            {
                var (px, index) = item;
                var xi = index % dataWedth;
                var yi = index / dataWedth;
                var isOut = 
                    xi < x || (x + width) < xi || 
                    yi < y || (y + height) < yi;

                if (isOut)
                    yield return mask;
                else
                    yield return px;
            }

        }

        IEnumerable<float> Square(IEnumerable<float> data, int dataWedth, int x, int y, int width, int height, float color)
        {
            foreach (var item in data.Select((px,i)=> (px,i)))
            {
                var (px, index) = item;
                var xi = index % dataWedth;
                var yi = index / dataWedth;
                var onLine = 
                    xi == x || (x + width) == xi || 
                    yi == y || (y + height) == yi;

                if (onLine)
                    yield return color;
                else
                    yield return px;
            }

        }

        IEnumerable<ILearningData> GetTestData(int loadCount)
        {
            var bmpLoader = new BitmapLoader();
            var testData = bmpLoader.Load("./temp/mnist/files.xml").Take(loadCount).ToArray();

            return testData;
        }
    }
}