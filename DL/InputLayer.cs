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
        public Func<INode, double> CalcFunction { get; } = _ => 0.0;

        public InputLayer(int inputDataSize)
        {
            // 固定値Nodeを作成
            var inputNodes = Enumerable.Range(0, inputDataSize).Select(i => new ValueNode(i));
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