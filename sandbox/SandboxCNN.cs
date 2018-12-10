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
            // 学習データを生成
            var loadCount = 3000;
            var (lData, vData, tData) = GetTestData(loadCount);


            var model = new CNNModelTest56();
            var modelData = "./models/cnn56";

            var batchSize = 8;
            var epoch = 20;
            var learningRate = 0.005f;
            var validator = new Validator();
            var watch = new Stopwatch();
            var lastep = 0;
            var machine = new Machine(model, learningRate, epoch, batchSize, 125, validator
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
            // if (model.Load(modelData) == false)
            {
                watch.Start();
                machine.Learn(lData, vData);
                watch.Stop();
                model.Save(modelData);
            }

            var testResults = (
                from test in tData
                let res = machine.Test(test.Data).ToArray()
                select (test.Expected, res as IEnumerable<float>)
            ).ToArray();

            Console.WriteLine($"testResult:{validator.Valid(testResults)}");
        }

        (IEnumerable<ILearningData>, IEnumerable<ILearningData>, IEnumerable<ILearningData>) GetTestData(int loadCount)
        {
            var setCount = loadCount / 3;
            var bmpLoader = new BitmapLoader();
            var testData = bmpLoader.Load("./temp/mnistMove/files.xml").Take(loadCount).ToArray();

            var lData = testData.Take(setCount).ToArray();
            var vData = testData.Skip(setCount).Take(setCount).ToArray();
            var tData = testData.Skip(setCount * 2).Take(setCount).ToArray();
            return (lData, vData, tData);
        }
    }
}