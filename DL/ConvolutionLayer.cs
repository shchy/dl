using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class ConvolutionLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }
        public Func<IEnumerable<double>, IEnumerable<double>> ActivationFunction { get; set; }
        public Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> UpdateWeightFunction { get; private set; }
        public Func<INode, double> CalcFunction { get; set; }

        public ConvolutionLayer(ILayer before, int width, int beforeLayerFilterCount, int filterSize, int filterCount
                                , Func<IEnumerable<double>, IEnumerable<double>> activation)
        {
            this.CalcFunction = DLF.CalcFunction;
            this.ActivationFunction = activation;
            this.UpdateWeightFunction = DLF.UpdateWeight();
            var height = (before.Nodes.Count() / width) / beforeLayerFilterCount;

            this.Nodes = (
                from filter in Enumerable.Range(0, filterCount)
                from y in Enumerable.Range(0, (height - filterSize) + 1)
                from x in Enumerable.Range(0, (width - filterSize) + 1)
                let links =
                    from channelOffset in Enumerable.Range(0, beforeLayerFilterCount)
                    let lx = before.Nodes.MakeLink(width, height, filterSize, x, y + (channelOffset * height), DLF.GetRandomWeight).ToArray()
                    select lx
                let node = new Node(ActivationFunction, links.SelectMany(l => l).ToArray())
                select node).ToArray();
        }

    }
}