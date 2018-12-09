using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class ConvolutionLayer : I2DLayer
    {
        public IEnumerable<INode> Nodes { get; set; }
        public Func<IEnumerable<float>, IEnumerable<float>> ActivationFunction { get; set; }
        public Action<ILayer, ILayer, Func<IEnumerable<Tuple<float, float>>, float>, ILearningData> UpdateWeightFunction { get; private set; }
        public Func<INode, float> CalcFunction { get; set; }

        public int OutputWidth { get; set; }

        public int OutputHeight { get; set; }

        public int OutputCh { get; set; }

        public ConvolutionLayer(ILayer before, ValueTuple<int, int, int> filter
                                , Func<IEnumerable<float>, IEnumerable<float>> activation
                                , Func<float, bool> ignoreUpdate = null)
        {
            (int filterSize, int stride, int filterCount) = filter;
            this.CalcFunction = DLF.CalcFunction;
            this.ActivationFunction = activation;
            this.UpdateWeightFunction = DLF.UpdateWeight(ignoreUpdate);

            int width, height, chSize = 0;
            if (before is I2DLayer dLayer)
            {
                width = dLayer.OutputWidth;
                height = dLayer.OutputHeight;
                chSize = dLayer.OutputCh;
            }
            else
            {
                width = before.Nodes.Count();
                height = 1;
                chSize = 1;
            }

            var xSize = (int)Math.Ceiling((width - filterSize) / (double)stride);
            var ySize = (int)Math.Ceiling((height - filterSize) / (double)stride);

            this.OutputWidth = xSize;
            this.OutputHeight = ySize;
            this.OutputCh = filterCount;

            this.Nodes = (
                from filterIndex in Enumerable.Range(0, filterCount)
                let weights = (
                    from fx in Enumerable.Range(0, filterSize)
                    from fy in Enumerable.Range(0, filterSize)
                    select Weight.Make(DLF.GetRandomWeight(), xSize * ySize * chSize))
                    .ToArray()
                from y in Enumerable.Range(0, height - filterSize).Where(i => i % stride == 0)
                from x in Enumerable.Range(0, width - filterSize).Where(i => i % stride == 0)
                let links =
                    from channelOffset in Enumerable.Range(0, chSize)
                    let lx = before.Nodes.MakeLink(width, filterSize, x, y + (channelOffset * height), (wx, wy) => weights[(wy * filterSize) + wx]).ToArray()
                    select lx
                let node = new Node(ActivationFunction, links.SelectMany(l => l).ToArray())
                select node).ToArray();
        }

    }
}