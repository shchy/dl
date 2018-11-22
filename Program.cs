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

            // 誤差関数
            // Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => 0.5 * x.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => -x.Sum(a => a.Item2 * Math.Log(Math.Max(a.Item1, 1e-7)));// + (1 - a.Item2) * Math.Log(1 - a.Item1));

            // 入力レイヤ
            var inputLayer = new InputLayer(3);
            // 隠れレイヤ
            var layer00 = new FullyConnectedLayer(inputLayer, 4, DLF.ReLU, DLF.UpdateWeight);
            // 隠れレイヤ
            var layer01 = new FullyConnectedLayer(layer00, 2, DLF.ReLU, DLF.UpdateWeight);
            // 出力レイヤ
            var layer02 = new SoftmaxLayer(layer01, 2);

            var machine = new Machine(0.01, 10000, 10
                                    , errorFunction
                                    , inputLayer
                                    , layer00
                                    , layer01
                                    , layer02);
            // 学習データを生成
            var testData = DLF.Shuffle(
                from x in Enumerable.Range(1, 8)
                from y in Enumerable.Range(1, 8)
                from z in Enumerable.Range(1, 8)
                let isTrue = x + (y * 2) + z < 16
                select LearningData.New(new double[] { x, y, z }, isTrue ? new[] { 1.0, 0.0 } : new[] { 0.0, 1.0 })).ToArray();

            var validData = testData.Skip(testData.Length / 2).ToArray();
            testData = testData.Take(testData.Length / 2).ToArray();

            machine.Learn(testData.ToArray());
            machine.Validate(validData);
        }
    }
}
