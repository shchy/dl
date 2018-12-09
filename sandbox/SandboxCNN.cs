using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using dl.DL;

namespace dl
{
    class CNN
    {
        public void Run()
        {
            var batchSize = 10;
            var epoch = 10;
            var learningRate = 0.005;
            var model = new CNNModelTest56();
            var validator = new Validator();
            var modelData = "./models/cnn56";
            var machine = new Machine(learningRate, epoch, batchSize, validator, model);

            // 学習データを生成
            var bmpLoader = new BitmapLoader();
            var testData = DLF.Shuffle(bmpLoader.Load("./temp/mnistMove/files.xml")).ToArray();
            if (model.Load(modelData) == false)
            {
                // 0-9を均等にピックアップ
                var pickNum = 1000;
                var a = new[]{
                    testData.Take(10000).Where(x => x.Name == "0").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "1").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "2").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "3").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "4").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "5").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "6").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "7").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "8").Take(pickNum),
                    testData.Take(10000).Where(x => x.Name == "9").Take(pickNum),
                }.SelectMany(x => x).ToArray();
                var b = new[]{
                    testData.Skip(10000).Where(x => x.Name == "0").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "1").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "2").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "3").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "4").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "5").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "6").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "7").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "8").Take(pickNum),
                    testData.Skip(10000).Where(x => x.Name == "9").Take(pickNum),
                }.SelectMany(x => x).ToArray();

                machine.Learn(a, b);
                model.Save(modelData);
            }

            var testCount = 10;
            var test = DLF.Shuffle(testData.Skip(20000)).Take(testCount).ToArray();
            for (var i = 0; i < testCount; i++)
            {
                var result = machine.Test(test[i].Data);
                var a = DLF.FindMaxValueIndex(result);
                Console.WriteLine($"ex={test[i].Name} result={a}");
            }

        }
    }





}