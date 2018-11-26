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

        public PoolingLayer(ILayer before, int beforeLayerWidth, int beforeLayerFilterCount, int poolingSize)
        {
            this.CalcFunction = n => n.Links.Select(l => l.InputNode.GetValue()).Max();
            this.ActivationFunction = v=>v;
            this.UpdateWeightFunction = DLF.UpdateWeight((n,l) => n.GetValue() == l.InputNode.GetValue());
            var height = (before.Nodes.Count() / beforeLayerWidth) / beforeLayerFilterCount;

            this.Nodes = (
                from filterIndex in Enumerable.Range(0, beforeLayerFilterCount)
                let filterNodes = before.Nodes.Skip(filterIndex * height * beforeLayerWidth).Take(height * beforeLayerWidth).ToArray()
                from y in Enumerable.Range(0, height).Where(i => i % poolingSize == 0)
                from x in Enumerable.Range(0, beforeLayerWidth).Where(i => i % poolingSize == 0)
                let links = filterNodes.MakeLink(beforeLayerWidth, height, poolingSize, x, y).ToArray()
                let node = new Node(ActivationFunction, links.Skip(1).ToArray())
                select node).ToArray();
        }

    }
}