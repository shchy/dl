using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace dl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var a = new MyClass();
            a.Foo();

        }
    }


    public class MyClass
    {
        public void Foo()
        {
            // 活性化関数
            // Func<double, double> activationFunction = u => Math.Max(0, u);
            Func<double, double> activationFunction = u => 1.0 / (1.0 + Math.Exp(-u));

            // 誤差関数
            // Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => 0.5 * x.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => 0.5 * x.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
            // 入力レイヤ
            var inputLayer = new InputLayer(2);
            var layer00 = new FullyConnectedLayer(inputLayer, activationFunction, 2, UpdateWeightOfOutputLayer);

            var machine = new Machine(0.01, 10000
                                    , errorFunction
                                    , inputLayer
                                    , layer00);
            // 学習データを生成
            var testData = shuffle(
                from x in Enumerable.Range(1, 20)
                from y in Enumerable.Range(1, 20)
                let isTrue = x + (y * 2) < 30
                select LearningData.New(new double[] { x, y }, isTrue ? new[] { 1.0, 0.0 } : new[] { 0.0, 1.0 }))
                .ToArray();

            machine.Learn(testData.ToArray());
        }

        private void UpdateWeightOfOutputLayer(ILayer layer
                                             , Func<IEnumerable<Tuple<double, double>>, double> errorFunction
                                             , ILearningData data)
        {
            var result = layer.Nodes.Select(x => x.GetValue()).ToArray();
            foreach (var item in layer.Nodes.Select((x, index) => new { x, index }))
            {
                var node = item.x;
                var index = item.index;
                // 誤差関数の偏微分
                Func<double, double> ef = (double x) =>
                {
                    var rx = result.ToArray();
                    rx[index] = x;
                    return errorFunction(rx.Zip(data.Expected, Tuple.Create));
                };

                // 活性化前の値
                var u = node.GetU();
                // 出力
                var o = node.GetValue();
                // 前の層の重み計算で使える部分
                node.Delta = ef.Derivative()(o) * layer.ActivationFunction.Derivative()(u);

                // 入力Nodeごとに重みを更新
                foreach (var link in node.Links)
                {
                    // 前の層の出力
                    var o0 = link.InputNode.GetValue();
                    // 更新用の傾きを覚えておく
                    link.Slope = node.Delta * o0;
                }
            }
        }

        private IEnumerable<T> shuffle<T>(IEnumerable<T> xs)
        {
            if (xs.Any() == false)
                yield break;

            var r = new Random(DateTime.Now.Millisecond);
            var array = xs.ToArray();
            var len = array.Length;
            var index = r.Next(len);
            yield return array[index];
            var remains = shuffle(array.Where((x, i) => i != index));
            foreach (var item in remains)
            {
                yield return item;
            }
        }
    }

    public class Machine : IMachine
    {
        private InputLayer firstLayer;
        private ILayer outputLayer;
        private readonly double learningRate;
        private readonly int epoch;
        private readonly Func<IEnumerable<Tuple<double, double>>, double> errorFunction;

        public IEnumerable<ILayer> Layers { get; set; }

        public Machine(double learningRate, int epoch, Func<IEnumerable<Tuple<double, double>>, double> errorFunction, params ILayer[] layers)
        {
            // todo 先頭はInputLayerであること
            this.firstLayer = layers.First() as InputLayer;
            this.outputLayer = layers.Last();
            this.learningRate = learningRate;
            this.epoch = epoch;
            this.errorFunction = errorFunction;
            this.Layers = layers;
        }

        public void Learn(IEnumerable<ILearningData> learningData)
        {
            var k = learningData.Count();
            var errorValueMin = double.MaxValue;
            var errorValueMinIndex = -1;
            for (int i = 0; i < epoch; i++)
            {
                var errorValue = 0.0;
                var errorValueMax = 0.0;

                // テストデータ分繰り返す
                foreach (var data in learningData)
                {
                    // 処理する
                    var result = Test(data.Data).ToArray();

                    // 誤差関数通す
                    var e = errorFunction(result.Zip(data.Expected, Tuple.Create));
                    errorValueMax = Math.Max(errorValueMax, e);
                    errorValue += e;

                    // 各Nodeの入力重みを更新
                    // todo 出力層以外も更新できるようにする今は出力層しか重みをもってないので出力層だけ考える
                    foreach (var layer in this.Layers.Skip(1).Reverse())
                    {
                        layer.UpdateWeight(errorFunction, data);
                    }

                    foreach (var node in this.Layers.SelectMany(x => x.Nodes))
                    {
                        node.Apply(this.learningRate);
                    }
                }

                if (errorValueMax < errorValueMin)
                {
                    errorValueMin = errorValueMax;
                    errorValueMinIndex = i;
                }
                if (i % 100 == 0)
                {
                    Console.WriteLine($"誤差:{(errorValue / k).ToString("0.00000")}");
                    Console.WriteLine($"最大誤差:{(errorValueMax).ToString("0.00000")}");
                }
            }
            Console.WriteLine($"最小誤差:{(errorValueMin).ToString("0.00000")} 最小誤差Index:{(errorValueMinIndex)}");
        }

        public IEnumerable<double> Test(IEnumerable<double> data)
        {
            // 入力レイヤの値を更新
            this.firstLayer.UpdateData(data);

            // 出力レイヤの値を取得
            return
                this.outputLayer.Nodes.Select(n => n.GetValue());
        }
    }
}
