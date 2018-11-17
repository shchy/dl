using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class Machine : IMachine
    {
        private InputLayer firstLayer;
        private ILayer outputLayer;
        private readonly double learningRate;
        private readonly int epoch;
        private readonly int miniBatch;
        private readonly Func<IEnumerable<Tuple<double, double>>, double> errorFunction;

        public IEnumerable<ILayer> Layers { get; set; }

        public Machine(double learningRate, int epoch, int miniBatch, Func<IEnumerable<Tuple<double, double>>, double> errorFunction, params ILayer[] layers)
        {
            // todo 先頭はInputLayerであること
            this.firstLayer = layers.First() as InputLayer;
            this.outputLayer = layers.Last();
            this.learningRate = learningRate;
            this.epoch = epoch;
            this.miniBatch = miniBatch;
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

                var shuffled = DLF.Shuffle(learningData);
                var dataCount = 0;

                // テストデータ分繰り返す
                foreach (var data in shuffled)
                {
                    // 処理する
                    var result = Test(data.Data).ToArray();

                    // 誤差関数通す
                    var e = errorFunction(result.Zip(data.Expected, Tuple.Create));
                    errorValueMax = Math.Max(errorValueMax, e);
                    errorValue += e;

                    // 各Nodeの入力重みを更新
                    ILayer forwardLayer = null;
                    foreach (var layer in this.Layers.Skip(1).Reverse())
                    {
                        layer.UpdateWeight(errorFunction, data, forwardLayer);
                        forwardLayer = layer;
                    }

                    dataCount = (dataCount + 1) % (this.miniBatch);

                    foreach (var node in this.Layers.SelectMany(x => x.Nodes))
                    {
                        if (dataCount == 0)
                        {
                            node.Apply(this.learningRate);
                        }
                        else
                        {
                            node.Reset();
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
                    Console.WriteLine($"{i.ToString("00000")} 誤差   :{(errorValue / k).ToString("0.00000")}");
                    Console.WriteLine($"{i.ToString("00000")} 最大誤差:{(errorValueMax).ToString("0.00000")}");
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