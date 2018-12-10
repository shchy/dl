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
        private readonly float learningRate;
        private readonly int epoch;
        private readonly int batchSize;
        private readonly int miniBatch;
        private readonly IValidator validator;
        private readonly Func<IEnumerable<Tuple<float, float>>, float> errorFunction;
        private readonly Action<int, int> logger;

        public IEnumerable<ILayer> Layers { get; set; }

        public Machine(IModel model, float learningRate, int epoch, int batchSize, int miniBatch
                    , IValidator validator
                    , Action<int, int> logger)
        {
            this.Layers = model.Layers;
            // todo 先頭はInputLayerであること
            this.firstLayer = Layers.First() as InputLayer;
            this.outputLayer = Layers.Last();
            this.learningRate = learningRate;
            this.epoch = epoch;
            this.batchSize = batchSize;
            this.miniBatch = miniBatch;
            this.validator = validator;
            this.errorFunction = x => model.ErrorFunction(x) * (1.0f / batchSize);
            this.logger = logger;

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

        private IEnumerable<(IEnumerable<float>, IEnumerable<float>)> Learn(int i, IEnumerable<ILearningData> learningData)
        {
            var shuffled = (
                from bi in Enumerable.Range(0, this.miniBatch)
                let batchBlock = DLF.Shuffle(learningData).Take(this.batchSize).ToArray()
                select batchBlock)
                .ToArray();
            var allNodes = this.Layers.SelectMany(x => x.Nodes).Where(x => !(x is ValueNode)).ToArray();
            // テストデータ分繰り返す
            var dataIndex = 0;
            foreach (var batchBlock in shuffled)
            {
                foreach (var data in batchBlock)
                {
                    // 処理する
                    var result = Test(data.Data).ToArray();

                    // 各Nodeの入力重みを更新
                    UpdateWeight(data);
                    foreach (var node in allNodes)
                    {
                        node.Reset();
                    }
                    yield return (data.Expected, result);
                    dataIndex++;
                    this.logger(i, dataIndex);
                }

                foreach (var node in allNodes)
                {
                    node.Apply(this.learningRate);
                }
            }
        }

        public IEnumerable<float> Test(IEnumerable<float> data)
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