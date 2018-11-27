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

        public ConvolutionLayer(ILayer before, int width, int chSize, int filterSize, int filterCount
                                , Func<IEnumerable<double>, IEnumerable<double>> activation
                                , Func<double, bool> ignoreUpdate = null)
        {
            this.CalcFunction = DLF.CalcFunction;
            this.ActivationFunction = activation;
            this.UpdateWeightFunction = DLF.UpdateWeight(ignoreUpdate);
            var height = (before.Nodes.Count() / width) / chSize;
            var xSize = (width - filterSize) + 1;
            var ySize = (height - filterSize) + 1;

            this.Nodes = (
                from filter in Enumerable.Range(0, filterCount)
                let weights = (
                    from fx in Enumerable.Range(0, filterSize)
                    from fy in Enumerable.Range(0, filterSize)
                    select Weight.Make(DLF.GetRandomWeight(), xSize * ySize * chSize))
                    .ToArray()
                from y in Enumerable.Range(0, ySize)
                from x in Enumerable.Range(0, xSize)
                let links =
                    from channelOffset in Enumerable.Range(0, chSize)
                    let lx = before.Nodes.MakeLink(width, filterSize, x, y + (channelOffset * height), (wx, wy) =>
                            // Weight.Make(DLF.GetRandomWeight())
                            weights[(wy * filterSize) + wx]
                            ).ToArray()
                    select lx
                let node = new Node(ActivationFunction, links.SelectMany(l => l).ToArray())
                select node).ToArray();
        }

    }
}