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
    public class SandboxCNN
    {
        public void Run()
        {
            var batchSize = 10;
            var epoch = 10;
            var learningRate = 0.005f;
            var model = new CNNModelTest56();
            var validator = new Validator();
            var modelData = "./models/cnn56";
            var watch = new Stopwatch();
            var machine = new Machine(model, learningRate, epoch, batchSize, validator
                            , index => 
                            {
                                Console.WriteLine((watch.ElapsedMilliseconds) / 1000.0);
                                watch.Restart();
                            });

            // 学習データを生成
            var loadCount = 30;
            var setCount = loadCount / 3;
            var bmpLoader = new BitmapLoader();
            var testData = bmpLoader.Load("./temp/mnistMove/files.xml").Take(loadCount).ToArray();
            if (model.Load(modelData) == false)
            {
                var lData = testData.Take(setCount).ToArray();
                var vData = testData.Skip(setCount).Take(setCount).ToArray();
                // 0-9を均等にピックアップ
                var pickNum = setCount / 10;
                var a = new[]{
                    lData.Where(x => x.Name == "0").Take(pickNum),
                    lData.Where(x => x.Name == "1").Take(pickNum),
                    lData.Where(x => x.Name == "2").Take(pickNum),
                    lData.Where(x => x.Name == "3").Take(pickNum),
                    lData.Where(x => x.Name == "4").Take(pickNum),
                    lData.Where(x => x.Name == "5").Take(pickNum),
                    lData.Where(x => x.Name == "6").Take(pickNum),
                    lData.Where(x => x.Name == "7").Take(pickNum),
                    lData.Where(x => x.Name == "8").Take(pickNum),
                    lData.Where(x => x.Name == "9").Take(pickNum),
                }.SelectMany(x => x).ToArray();
                var b = new[]{
                    vData.Where(x => x.Name == "0").Take(pickNum),
                    vData.Where(x => x.Name == "1").Take(pickNum),
                    vData.Where(x => x.Name == "2").Take(pickNum),
                    vData.Where(x => x.Name == "3").Take(pickNum),
                    vData.Where(x => x.Name == "4").Take(pickNum),
                    vData.Where(x => x.Name == "5").Take(pickNum),
                    vData.Where(x => x.Name == "6").Take(pickNum),
                    vData.Where(x => x.Name == "7").Take(pickNum),
                    vData.Where(x => x.Name == "8").Take(pickNum),
                    vData.Where(x => x.Name == "9").Take(pickNum),
                }.SelectMany(x => x).ToArray();
                
                watch.Start();
                machine.Learn(a, b);
                watch.Stop();
                model.Save(modelData);
            }

            var testResults = (
                from test in DLF.Shuffle(testData.Skip(setCount * 2)).Take(setCount).ToArray()
                let res = machine.Test(test.Data).ToArray()
                select Tuple.Create(test.Expected, res as IEnumerable<float>)
            ).ToArray();
            
            Console.WriteLine($"testResult:{validator.Valid(testResults)}");
        }
    }
}