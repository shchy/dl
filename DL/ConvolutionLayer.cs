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
                                , Func<IEnumerable<double>, IEnumerable<double>> activation
                                , Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> updateWeightFunction)
        {
            this.CalcFunction = DLF.CalcFunction;
            this.ActivationFunction = activation;
            this.UpdateWeightFunction = updateWeightFunction;
            var height = (before.Nodes.Count() / width) / beforeLayerFilterCount;

            this.Nodes = (
                from filterIndex in Enumerable.Range(0, beforeLayerFilterCount)
                let filterNodes = before.Nodes.Skip(filterIndex * height * width).Take(height * width).ToArray()
                from filter in Enumerable.Range(0, filterCount)
                from y in Enumerable.Range(0, (height - filterSize) + 1)
                from x in Enumerable.Range(0, (width - filterSize) + 1)
                let links = filterNodes.MakeLink(width, height, filterSize, x, y).ToArray()
                let node = new Node(ActivationFunction, links)
                select node).ToArray();
        }

    }
}