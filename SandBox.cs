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
            var learningRate = 0.01;

            var model = new TestModel();

            var machine = new Machine(learningRate, epoch, batchSize, new Validator(), model);
            // 学習データを生成
            var testData = DLF.Shuffle(
                from x in Enumerable.Range(1, 8)
                from y in Enumerable.Range(1, 8)
                from z in Enumerable.Range(1, 8)
                let v = x + (y * 2) + z
                let expect = v < 15 ? new[] { 1.0, 0.0, 0.0 }
                        : v < 20 ? new[] { 0.0, 1.0, 0.0 }
                        : new[] { 0.0, 0.0, 1.0 }
                select LearningData.New(expect.ToString(), new double[] { x, y, z }, expect))
                .ToArray();

            var validData = testData.Skip(testData.Length / 2).ToArray();
            testData = testData.Take(testData.Length / 2).ToArray();

            machine.Learn(testData.ToArray(), validData.ToArray());
        }
    }

    class TestModel : IModel
    {
        public Func<IEnumerable<Tuple<double, double>>, double> ErrorFunction { get; set; }

        public IEnumerable<ILayer> Layers { get; set; }

        public TestModel()
        {
            // 入力レイヤ
            var inputLayer = new InputLayer(3);
            // 隠れレイヤ
            var layer00 = new FullyConnectedLayer(inputLayer, 20, DLF.ReLU, DLF.UpdateWeight(), DLF.GetRandomWeight);
            // 隠れレイヤ
            var layer01 = new FullyConnectedLayer(layer00, 10, DLF.ReLU, DLF.UpdateWeight(), DLF.GetRandomWeight);
            // 出力レイヤ
            var layer02 = new SoftmaxLayer(layer01, 3);

            this.Layers = new ILayer[]{
                inputLayer
                , layer00
                , layer01
                , layer02
            };
            this.ErrorFunction = DLF.ErrorFunctionCrossEntropy;
        }
    }
}