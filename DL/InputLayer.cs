using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class InputLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }

        public Func<double, double> ActivationFunction { get; } = x => x;

        public InputLayer(int inputDataSize)
        {
            // 固定値Nodeを作成
            var inputNodes = Enumerable.Range(0, inputDataSize).Select(_ => new ValueNode());
            this.Nodes = inputNodes.ToArray();
        }

        public void UpdateWeight(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, ILearningData data) { }

        public void UpdateData(IEnumerable<double> data)
        {
            // todo サイズチェック
            foreach (var item in Nodes.OfType<ValueNode>().Zip(data, Tuple.Create))
            {
                item.Item1.Value = item.Item2;
            }
        }
    }
}