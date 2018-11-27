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

    public static class LayerFunction
    {
        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes, Func<double> getWeight)
        {
            return
                new[] { new NodeLink { InputNode = new ValueNode() { Value = 1 }, Weight = getWeight() } }
                .Concat(nodes.Select(n => new NodeLink { InputNode = n, Weight = getWeight() }))
                .ToArray();
        }

        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes, int width, int height, int filterSize, int offsetX, int offsetY, Func<double> getWeight)
        {
            var inputNodes = nodes.ToArray();
            var bias = new NodeLink { InputNode = new ValueNode() { Value = 1 }, Weight = getWeight() };
            var links = (
                from y in Enumerable.Range(0, filterSize)
                from x in Enumerable.Range(0, filterSize)
                where (offsetX + x) < width
                where (offsetY + y) < height
                let nodeIndex = ((offsetY + y) * width) + offsetX + x
                let inputNode = inputNodes[nodeIndex]
                select new NodeLink { InputNode = inputNode, Weight = getWeight() })
                .ToArray();

            return
                new[] { bias }.Concat(links).ToArray();
        }
    }
}