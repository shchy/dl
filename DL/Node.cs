using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class Node : INode
    {
        public IEnumerable<INodeLink> Links { get; set; }
        private readonly Func<IEnumerable<float>, IEnumerable<float>> activation;

        private float output;
        public float Delta { get; set; }

        public Node(Func<IEnumerable<float>, IEnumerable<float>> activation, IEnumerable<INodeLink> links)
        {
            this.activation = activation;
            this.Links = links.ToArray();
            this.output = 0.0f;
        }

        public void Apply(float learningRate)
        {
            Reset();
            foreach (var link in this.Links)
            {
                link.Weight.Apply(learningRate);
            }
        }

        public void Reset()
        {
            this.output = 0.0f;
            this.Delta = 0.0f;
        }

        public float GetValue() => this.output;

        public void SetValue(float v) => this.output = v;
    }
}