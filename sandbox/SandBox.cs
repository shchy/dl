using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dl.DL;

namespace dl
{
    class Sandbox
    {
        public void Run()
        {
            var batchSize = 8;
            var epoch = 1000;
            var learningRate = 0.01f;

            var model = new TestModel();

            var machine = new Machine(model, learningRate, epoch, batchSize, new Validator(), _=>{});
            // 学習データを生成
            var testData = DLF.Shuffle(
                from x in Enumerable.Range(1, 8)
                from y in Enumerable.Range(1, 8)
                from z in Enumerable.Range(1, 8)
                let v = x + (y * 2) + z
                let expect = v < 15 ? new[] { 1.0f, 0.0f, 0.0f }
                        : v < 20 ? new[] { 0.0f, 1.0f, 0.0f }
                        : new[] { 0.0f, 0.0f, 1.0f }
                select LearningData.New(expect.ToString(), new float[] { x, y, z }, expect))
                .ToArray();

            var validData = testData.Skip(testData.Length / 2).ToArray();
            testData = testData.Take(testData.Length / 2).ToArray();

            machine.Learn(testData.ToArray(), validData.ToArray());
        }
    }


}