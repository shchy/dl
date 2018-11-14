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

            // 活性化関数
            // Func<double, double> activationFunction = u => Math.Max(0, u);
            Func<double, double> activationFunction = u => 1.0 / (1.0 + Math.Exp(-u));

            // 誤差関数
            // Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => 0.5 * x.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => 0.5 * x.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
            // 入力レイヤ
            var inputLayer = new InputLayer(2);
            // 隠れレイヤ
            var layer00 = new FullyConnectedLayer(inputLayer, u => Math.Max(0, u), 4, DLF.UpdateWeight);
            // 出力レイヤ
            var layer01 = new FullyConnectedLayer(inputLayer, activationFunction, 2, DLF.UpdateWeightOfOutputLayer);

            var machine = new Machine(0.01, 10000
                                    , errorFunction
                                    , inputLayer
                                    , layer00
                                    , layer01);
            // 学習データを生成
            var testData = DLF.Shuffle(
                from x in Enumerable.Range(1, 20)
                from y in Enumerable.Range(1, 20)
                let isTrue = x + (y * 2) < 30
                select LearningData.New(new double[] { x, y }, isTrue ? new[] { 1.0, 0.0 } : new[] { 0.0, 1.0 }))
                .ToArray();

            machine.Learn(testData.ToArray());

        }
    }
}
