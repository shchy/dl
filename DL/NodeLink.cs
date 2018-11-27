using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class NodeLink : INodeLink
    {
        public IWeight Weight { get; set; }
        public INode InputNode { get; set; }


    }

    public class Weight : IWeight
    {

        public double Value { get; set; }

        public double Slope { get; set; }

        private int refCounter;


        public void Apply(double learningRate)
        {
            if (Slope != 0.0)
                Value -= (Slope / refCounter) * learningRate;
            Slope = 0.0;
        }

        public static IWeight Make(double v, int refCounter = 1)
        {
            return new Weight
            {
                Value = v,
                Slope = 0.0,
                refCounter = refCounter,
            };
        }
    }
}
