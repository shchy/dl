using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class InputLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }

        public Func<INode, double, double> ActivationFunction { get; } = (_,x) => x;

        public InputLayer(int inputDataSize)
        {
            // 固定値Nodeを作成
            var inputNodes = Enumerable.Range(0, inputDataSize).Select(i => new ValueNode(i));
            this.Nodes = inputNodes.ToArray();
        }

        public void UpdateWeight(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, ILearningData data, ILayer forwardLayer) { }

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