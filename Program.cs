using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dl.DL;

namespace dl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            // 入力レイヤ
            var inputLayer = new InputLayer(3);
            // 隠れレイヤ
            var layer00 = new FullyConnectedLayer(inputLayer, 20, DLF.ReLU, DLF.UpdateWeight, DLF.GetRandomWeight);
            // 隠れレイヤ
            var layer01 = new FullyConnectedLayer(layer00, 10, DLF.ReLU, DLF.UpdateWeight, DLF.GetRandomWeight);
            // 出力レイヤ
            var layer02 = new SoftmaxLayer(layer01, 3);

            var batchSize = 8;
            var epoch = 10000;
            var learningRate = 0.01;
            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = DLF.ErrorFunctionCrossEntropy;

            var machine = new Machine(learningRate, epoch, batchSize
                                    , x => errorFunction(x) * (1.0 / batchSize)
                                    , inputLayer
                                    , layer00
                                    , layer01
                                    , layer02);
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

            var learningResults = machine.Learn(testData.ToArray());

            // 評価
            foreach (var item in learningResults.Select((r, i) => new { r, i }))
            {
                if (item.i % 100 != 0)
                {
                    continue;
                }
                var k = testData.Length;

                var expectCount = new int[3];
                var outputCount = new int[3];
                var accuracyCount = new int[3];

                foreach (var result in item.r)
                {
                    var expected = result.Item1.Expected.ToArray();
                    var output = result.Item2.ToArray();

                    var expectIndex = FindMaxValueIndex(expected);
                    var outputIndex = FindMaxValueIndex(output);

                    // 期待値のIndexの数を記憶しておく
                    expectCount[expectIndex] += 1;
                    outputCount[outputIndex] += 1;

                    // 正解率
                    if (expectIndex == outputIndex)
                    {
                        accuracyCount[outputIndex] += 1;
                    }
                }

                var learningResult = new LearningResult
                {
                    Accuracy = accuracyCount.Sum() / (double)k,
                    Recall = accuracyCount.Zip(expectCount, (a, e) => a / (double)e).ToArray(),
                    Precision = accuracyCount.Zip(outputCount, (a, e) => a / (double)e).ToArray(),
                };

                Console.WriteLine($"{item.i.ToString("00000")}:{learningResult}");
                // Console.WriteLine(LearningResult.ArrayToString(expectCount.Select(a => (double)a)));
            }
        }

        private static int FindMaxValueIndex(IEnumerable<double> xs)
        {
            var index = -1;
            var max = double.MinValue;
            foreach (var item in xs.Select((x, i) => new { x, i }))
            {
                if (max < item.x)
                {
                    index = item.i;
                    max = item.x;
                }
            }
            return index;
        }
    }
}
