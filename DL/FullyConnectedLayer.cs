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
        public Func<INode, double> CalcFunction { get; set; }
        public FullyConnectedLayer(ILayer before
                                , int nodeCount
                                , Func<IEnumerable<double>, IEnumerable<double>> activation
                                , Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> updateWeightFunction
                                , Func<double> getWeight)
        {
            this.CalcFunction = DLF.CalcFunction;
            this.ActivationFunction = activation;
            this.UpdateWeightFunction = updateWeightFunction;

            var nodes =
                from i in Enumerable.Range(0, nodeCount)
                let nodeLink = before.Nodes.MakeLink(getWeight)
                let node = new Node(activation, nodeLink)
                select node;
            this.Nodes = nodes.ToArray();
        }

    }
}