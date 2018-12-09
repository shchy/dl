using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class InputLayer : I2DLayer
    {
        public IEnumerable<INode> Nodes { get; set; }
        public Func<IEnumerable<float>, IEnumerable<float>> ActivationFunction { get; } = (x) => x;

        public Action<ILayer, ILayer, Func<IEnumerable<Tuple<float, float>>, float>, ILearningData> UpdateWeightFunction { get; } = (_1, _2, _3, _4) => { };
        public Func<INode, float> CalcFunction { get; set; } = (n) => n.GetValue();

        public int OutputWidth { get; set; }

        public int OutputHeight { get; set; }

        public int OutputCh { get; set; }

        public InputLayer(int inputDataSize)
        {
            // 固定値Nodeを作成
            var inputNodes = Enumerable.Range(0, inputDataSize).Select(_ => new ValueNode());
            this.Nodes = inputNodes.ToArray();
            this.OutputWidth = Nodes.Count();
            this.OutputHeight = 1;
            this.OutputCh = 1;
        }

        public InputLayer(int width, int height)
            : this(width * height)
        {
            this.OutputWidth = width;
            this.OutputHeight = height;
            this.OutputCh = 1;
        }

        public void UpdateData(IEnumerable<float> data)
        {
            // todo サイズチェック
            foreach (var item in Nodes.OfType<ValueNode>().Zip(data, Tuple.Create))
            {
                item.Item1.SetValue(item.Item2);
            }
        }
    }
}