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
            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => 0.5 * x.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
            // 入力レイヤ
            var inputLayer = new InputLayer(2);
            var layer00 = new FullyConnectedLayer(inputLayer, activationFunction, 1);

            var machine = new Machine(0.01, 10000
                                    , errorFunction
                                    , inputLayer
                                    , layer00);

            var testData = shuffle(
                from x in Enumerable.Range(1, 20)
                from y in Enumerable.Range(1, 20)
                let isTrue = x + (y * 2) < 30
                select LearningData.New(new double[] { x, y }, isTrue ? new[] { 1.0 } : new[] { 0.0 }))
                .ToArray();
            var trueCount = testData.Select(x => x.Expected.Sum()).Count(x => x > 0);
            Console.WriteLine(trueCount / (double)testData.Length);

            // 重みの初期値確認
            var initweights = layer00.Nodes.SelectMany(x => x.Links).Select(x => x.Weight).ToArray();
            foreach (var w in initweights)
            {
                Console.WriteLine(w);
            }

            machine.Learn(testData.ToArray());
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
                        foreach (var item in layer.Nodes.Zip(data.Expected, Tuple.Create))
                        {
                            var node = item.Item1;
                            var expected = item.Item2;
                            node.UpdateWeight(this.learningRate, expected);
                        }
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
