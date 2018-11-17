using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class ValueNode : INode
    {
        public double Value { get; set; }
        public double Delta { get; set; }
        public IEnumerable<INodeLink> Links => Enumerable.Empty<INodeLink>();

        public ValueNode()
        {
            this.Value = 0.0;
        }

        public void Reset() { }
        public double GetValue() => this.Value;
        public double GetU() => this.Value;
        public void Apply(double learningRate) { }

    }
}