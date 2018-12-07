using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IValidator validator;
        private readonly Func<IEnumerable<Tuple<double, double>>, double> errorFunction;

        public IEnumerable<ILayer> Layers { get; set; }

        public Machine(double learningRate, int epoch, int miniBatch, IValidator validator
                    , IModel model)
        {
            this.Layers = model.Layers;
            // todo 先頭はInputLayerであること
            this.firstLayer = Layers.First() as InputLayer;
            this.outputLayer = Layers.Last();
            this.learningRate = learningRate;
            this.epoch = epoch;
            this.miniBatch = miniBatch;
            this.validator = validator;
            this.errorFunction = x => model.ErrorFunction(x) * (1.0 / miniBatch);

        }

        public void Learn(IEnumerable<ILearningData> learningData, IEnumerable<ILearningData> validateData)
        {
            for (int i = 0; i < epoch; i++)
            {
                var results = Learn(i, learningData).ToArray();
                var result = this.validator.Valid(results);
                Console.WriteLine($"{i.ToString("00000")}:{result}");
            }
        }

        private IEnumerable<Tuple<IEnumerable<double>, IEnumerable<double>>> Learn(int i, IEnumerable<ILearningData> learningData)
        {
            var shuffled = DLF.Shuffle(learningData).ToArray();
            var dataCount = 0;
            var dataIndex = 0;
            var allNodes = this.Layers.SelectMany(x => x.Nodes).Where(x => !(x is ValueNode)).ToArray();
            var watch = new Stopwatch();
            watch.Start();
            // テストデータ分繰り返す
            foreach (var data in shuffled)
            {
                // 処理する
                var result = Test(data.Data).ToArray();

                // 各Nodeの入力重みを更新
                UpdateWeight(data);

                dataCount = (dataCount + 1) % (this.miniBatch);
                var isBatchUpdate = dataCount == 0;
                if (isBatchUpdate)
                {
                    foreach (var node in allNodes)
                    {
                        node.Apply(this.learningRate);
                    }
                }
                else
                {
                    foreach (var node in allNodes)
                    {
                        node.Reset();
                    }
                }
                dataIndex++;

                if (isBatchUpdate)
                {
                    Console.WriteLine(watch.ElapsedMilliseconds / miniBatch / 1000.0);
                    watch.Restart();
                }
                var ret = Tuple.Create(data.Expected, result as IEnumerable<double>);
                yield return ret;
            }
        }

        public IEnumerable<double> Test(IEnumerable<double> data)
        {
            // 入力レイヤの値を更新
            this.firstLayer.UpdateData(data);
            // 入力レイヤ以降の更新
            foreach (var l in this.Layers.Skip(1))
            {
                // uを求める
                var ux = l.Nodes.Select(l.CalcFunction).ToArray();
                // oを求める
                var ox = l.ActivationFunction(ux);
                // Nodeを更新
                foreach (var item in l.Nodes.Zip(ox, Tuple.Create))
                {
                    item.Item1.SetValue(item.Item2);
                }
            }

            // 出力レイヤの値を取得
            return
                this.outputLayer.Nodes.Select(n => n.GetValue());
        }

        void UpdateWeight(ILearningData data)
        {
            // 各Nodeの入力重みを更新
            ILayer forwardLayer = outputLayer;
            foreach (var layer in this.Layers.Skip(1).Reverse())
            {
                layer.UpdateWeightFunction(layer, forwardLayer, errorFunction, data);
                forwardLayer = layer;
            }
        }
    }
}