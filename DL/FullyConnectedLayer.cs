using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class FullyConnectedLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }
        public Func<IEnumerable<double>, IEnumerable<double>> ActivationFunction { get; set; }
        public Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> UpdateWeightFunction { get; private set; }

        public FullyConnectedLayer(ILayer before
                                , int nodeCount
                                , Func<IEnumerable<double>, IEnumerable<double>> activation
                                , Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> updateWeightFunction)
        {
            this.ActivationFunction = activation;
            this.UpdateWeightFunction = updateWeightFunction;

            var nodes =
                from i in Enumerable.Range(0, nodeCount)
                let nodeLink = before.Nodes.MakeLink()
                let node = new Node(i, activation, nodeLink)
                select node;
            this.Nodes = nodes.ToArray();
        }

    }
}