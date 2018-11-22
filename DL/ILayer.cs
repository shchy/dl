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
        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes)
        {
            return
                new[] { new NodeLink { InputNode = new ValueNode(-1) { Value = 1 }, Weight = 0.01 * DLF.GetRandom() } }
                .Concat(nodes.Select(n => new NodeLink { InputNode = n, Weight = 0.01 * DLF.GetRandom() }))
                .ToArray();
        }
    }
}