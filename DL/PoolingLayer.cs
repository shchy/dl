using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class PoolingLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }
        public Func<IEnumerable<double>, IEnumerable<double>> ActivationFunction { get; set; }
        public Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> UpdateWeightFunction { get; private set; }

        public Func<INode, double> CalcFunction { get; set; }

        public PoolingLayer(ILayer before, int width, int chSize, int poolingSize, int stride)
        {
            this.CalcFunction = n => n.Links.Select(l => l.InputNode.GetValue()).Max();
            this.ActivationFunction = v => v;
            this.UpdateWeightFunction = DLF.UpdateWeight(null, (n, l, d) =>
            {
                if (n.GetValue() == l.InputNode.GetValue())
                    l.InputNode.Delta += d * l.Weight.Value;
            });
            var height = (before.Nodes.Count() / width) / chSize;

            this.Nodes = (
                from filterIndex in Enumerable.Range(0, chSize)
                let filterNodes = before.Nodes.Skip(filterIndex * height * width).Take(height * width).ToArray()
                from y in Enumerable.Range(0, height - poolingSize).Where(i => i % stride == 0)
                from x in Enumerable.Range(0, width - poolingSize).Where(i => i % stride == 0)
                let links = filterNodes.MakeLink(width, poolingSize, x, y, (wx, wy) => Weight.Make(1.0)).ToArray()
                let node = new Node(ActivationFunction, links.Skip(1).ToArray())
                select node).ToArray();
        }

    }
}