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
    }
}