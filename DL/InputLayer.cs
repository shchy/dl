using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class InputLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }
        public Func<IEnumerable<double>, IEnumerable<double>> ActivationFunction { get; } = (x) => x;

        public Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> UpdateWeightFunction { get; } = (_1, _2, _3, _4) => { };
        public Func<INode, double> CalcFunction { get; set; } = (n) => n.GetValue();

        public InputLayer(int inputDataSize)
        {
            // 固定値Nodeを作成
            var inputNodes = Enumerable.Range(0, inputDataSize).Select(_ => new ValueNode());
            this.Nodes = inputNodes.ToArray();
        }

        public void UpdateData(IEnumerable<double> data)
        {
            // todo サイズチェック
            foreach (var item in Nodes.OfType<ValueNode>().Zip(data, Tuple.Create))
            {
                item.Item1.SetValue(item.Item2);
            }
        }
    }
}