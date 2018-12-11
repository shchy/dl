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
            var loadCount = 6000;
            var (lData, vData, tData) = GetTestData(loadCount);

            var model = new CNNModelTest();
            var modelData = "./backup/cnn";

            var batchSize = 8;
            var batchCount = lData.Count() / batchSize;
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
            
            if (model.Load(modelData) == false)
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
            var learningCount = loadCount / 6 * 5;
            var validCount = loadCount / 6 /2;
            var bmpLoader = new BitmapLoader();
            var testData = bmpLoader.Load("./temp/mnist/files.xml").Take(loadCount).ToArray();

            var lData = testData.Take(learningCount).ToArray();
            var vData = testData.Skip(learningCount).Take(validCount).ToArray();
            var tData = testData.Skip(learningCount + validCount).ToArray();
            return (lData, vData, tData);
        }
    }
}