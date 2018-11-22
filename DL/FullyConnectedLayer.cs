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
        public Func<INode, double> CalcFunction { get; }

        public FullyConnectedLayer(ILayer before
                                , Func<INode, double> calcValue
                                , Func<IEnumerable<double>, IEnumerable<double>> activation
                                , int nodeCount)
        {
            this.ActivationFunction = activation;
            this.CalcFunction = calcValue;

            var nodes =
                from i in Enumerable.Range(0, nodeCount)
                let nodeLink = before.Nodes.MakeLink()
                let node = new Node(i, activation, nodeLink)
                select node;
            this.Nodes = nodes.ToArray();
        }

    }
}