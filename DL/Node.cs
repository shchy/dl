using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class Node : INode
    {
        public IEnumerable<INodeLink> Links { get; set; }
        private readonly Func<IEnumerable<double>, IEnumerable<double>> activation;

        private double output;
        public double Delta { get; set; }

        public Node(Func<IEnumerable<double>, IEnumerable<double>> activation, IEnumerable<INodeLink> links)
        {
            this.activation = activation;
            this.Links = links.ToArray();
            this.output = 0.0;
        }

        public void Apply(double learningRate)
        {
            Reset();
            foreach (var link in this.Links)
            {
                link.Weight.Apply(learningRate);
            }
        }

        public void Reset()
        {
            this.output = 0.0;
            this.Delta = 0.0;
        }

        public double GetValue() => this.output;

        public void SetValue(double v) => this.output = v;
    }
}