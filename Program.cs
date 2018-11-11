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
            Func<double,double> activationFunction = u => Math.Max(0, u);

            // 誤差関数
            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = x => x.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
            // 入力レイヤ
            var inputLayer = new InputLayer(8);
            var layer00 = new FullyConnectedLayer(inputLayer, activationFunction, 2);

            var machine = new Machine(errorFunction
                                    , inputLayer
                                    , layer00);

            var testData =
                from x in Enumerable.Range(0, 20)
                from y in Enumerable.Range(0, 20)
                let isTrue = x * (y * 2) < 10
                select LearningData.New(new double[] { x, y }, isTrue ? new[] { 1.0, 0.0 } : new[] { 0.0, 1.0 });
            
            machine.Learn(testData.ToArray());
        }
    }

    

    public class Machine : IMachine
    {
        private InputLayer firstLayer;
        private ILayer outputLayer;
        private readonly Func<IEnumerable<Tuple<double, double>>, double> errorFunction;

        public IEnumerable<ILayer> Layers { get; set; }

        public Machine(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, params ILayer[] layers)
        {
            // todo 先頭はInputLayerであること
            this.firstLayer = layers.First() as InputLayer;
            this.outputLayer = layers.Last();
            this.errorFunction = errorFunction;
            this.Layers = layers;
        }

        public void Learn(IEnumerable<ILearningData> learningData)
        {
            // テストデータ分繰り返す
            foreach (var data in learningData)
            {
                // 処理する
                var result = Test(data.Data).ToArray();

                // 誤差関数通す
                var errorValue = errorFunction(result.Zip(data.Expected, Tuple.Create));
                



            }
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
