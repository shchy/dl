using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class ValueNode : INode
    {
        public float Value { get; set; }
        public float Delta { get; set; }
        public IEnumerable<INodeLink> Links => Enumerable.Empty<INodeLink>();

        public ValueNode()
        {
            this.Value = 0.0f;
        }

        public void Reset() { }
        public float GetValue() => this.Value;
        public void SetValue(float v) => this.Value = v;
        public void Apply(float learningRate) { }

    }
}