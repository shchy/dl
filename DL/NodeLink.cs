using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class NodeLink : INodeLink
    {
        public double Weight { get; set; }

        public double Slope { get; set; }

        public INode InputNode { get; set; }


    }
}