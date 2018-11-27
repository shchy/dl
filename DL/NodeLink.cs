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

        public static IWeight Make(double v)
        {
            return new Weight
            {
                Value = v,
                Slope = 0.0,
            };
        }
    }
}
