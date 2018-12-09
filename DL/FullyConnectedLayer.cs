using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class FullyConnectedLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }
        public Func<IEnumerable<float>, IEnumerable<float>> ActivationFunction { get; set; }
        public Action<ILayer, ILayer, Func<IEnumerable<Tuple<float, float>>, float>, ILearningData> UpdateWeightFunction { get; private set; }
        public Func<INode, float> CalcFunction { get; set; }
        public FullyConnectedLayer(ILayer before
                                , int nodeCount
                                , Func<IEnumerable<float>, IEnumerable<float>> activation
                                , Action<ILayer, ILayer, Func<IEnumerable<Tuple<float, float>>, float>, ILearningData> updateWeightFunction
                                , Func<float> getWeight)
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