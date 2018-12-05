using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public interface ILayer
    {
        /// <summary>
        /// レイヤ内のノード達
        /// </summary>
        IEnumerable<INode> Nodes { get; }
        Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> UpdateWeightFunction { get; }
        Func<IEnumerable<double>, IEnumerable<double>> ActivationFunction { get; }
        Func<INode, double> CalcFunction { get; }
    }

    public interface I2DLayer : ILayer
    {

        int OutputWidth { get; }
        int OutputHeight { get; }
        int OutputCh { get; }

    }

    public static class LayerFunction
    {
        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes, Func<double> getWeight)
        {
            return
                new[] { new NodeLink { InputNode = new ValueNode() { Value = 1 }, Weight = Weight.Make(getWeight()) } }
                .Concat(nodes.Select(n => new NodeLink { InputNode = n, Weight = Weight.Make(getWeight()) }))
                .ToArray();
        }

        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes, int width, int filterSize, int offsetX, int offsetY, Func<int, int, IWeight> getWeight)
        {
            var inputNodes = nodes.ToArray();
            var height = inputNodes.Length / width;
            var bias = new NodeLink { InputNode = new ValueNode() { Value = 1 }, Weight = Weight.Make(DLF.GetRandomWeight()) };
            var links = (
                from y in Enumerable.Range(0, filterSize)
                from x in Enumerable.Range(0, filterSize)
                where (offsetX + x) < width
                where (offsetY + y) < height
                let nodeIndex = ((offsetY + y) * width) + offsetX + x
                let inputNode = inputNodes[nodeIndex]
                select new NodeLink { InputNode = inputNode, Weight = getWeight(x, y) })
                .ToArray();

            return
                new[] { bias }.Concat(links).ToArray();
        }
    }
}