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

        Func<double, double> ActivationFunction { get; }

        void UpdateWeight(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, ILearningData data, ILayer forwardLayer);
    }

    public static class LayerFunction
    {
        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes)
        {
            return
                new[] { new NodeLink { InputNode = new ValueNode { Value = 1 }, Weight = 0.01 * DLF.GetRandom() } }
                .Concat(nodes.Select(n => new NodeLink { InputNode = n, Weight = 0.01 * DLF.GetRandom() }))
                .ToArray();
        }
    }
}