using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class FullyConnectedLayer : ILayer
    {
        private Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> updateWeight;

        public IEnumerable<INode> Nodes { get; set; }
        public Func<INode, double, double> ActivationFunction { get; set; }

        public FullyConnectedLayer(ILayer before, Func<INode,double, double> activation, int nodeCount, Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> updateWeight)
        {
            this.updateWeight = updateWeight;
            this.ActivationFunction = activation;

            var nodes =
                from i in Enumerable.Range(0, nodeCount)
                let nodeLink = before.Nodes.MakeLink()
                let node = new Node(i, activation, nodeLink)
                select node;
            this.Nodes = nodes.ToArray();
        }

        public void UpdateWeight(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, ILearningData data, ILayer forwardLayer)
        {
            this.updateWeight(this, forwardLayer, errorFunction, data);
        }
    }
}