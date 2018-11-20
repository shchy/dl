using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class Node : INode
    {
        public int Index { get; set; }
        public IEnumerable<INodeLink> Links { get; set; }
        private readonly Func<INode, double, double> activation;
        private double? u;
        private double? output;
        public double Delta { get; set; }

        public Node(int index, Func<INode, double, double> activation, IEnumerable<INodeLink> links)
        {
            this.Index = index;
            this.activation = activation;
            this.Links = links.ToArray();
            this.u = null;
            this.output = null;
        }

        public void Apply(double learningRate)
        {
            Reset();
            foreach (var link in this.Links)
            {
                link.Weight -= (link.Slope / link.UpdateCount) * learningRate;
                link.Slope = 0.0;
                link.UpdateCount = 0;
            }
        }

        public void Reset()
        {
            this.u = null;
            this.output = null;
            this.Delta = 0.0;
        }

        public double GetValue()
        {
            if (this.output.HasValue == false)
                this.output = this.activation(this, GetU());
            return output.Value;
        }

        public double GetU()
        {
            if (u.HasValue == false)
                u = Links
                    .Select(link => link.InputNode.GetValue() * link.Weight)
                    .Sum();
            return u.Value;
        }
    }
}