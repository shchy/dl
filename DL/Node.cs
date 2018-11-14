using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class Node : INode
    {
        private readonly Func<double, double> activation;
        public IEnumerable<INodeLink> Links { get; set; }
        private double? u;
        private double? output;
        public double Delta { get; set; }

        public Node(Func<double, double> activation, IEnumerable<INodeLink> links)
        {
            this.activation = activation;
            this.Links = links.ToArray();
            this.u = null;
            this.output = null;
        }

        public void Apply(double learningRate)
        {
            this.u = null;
            this.output = null;
            this.Delta = 0.0;
            foreach (var link in this.Links)
            {
                link.Weight -= link.Slope * learningRate;
                link.Slope = 0.0;
            }
        }

        public double GetValue()
        {
            if (this.output.HasValue == false)
                this.output = this.activation(GetU());
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