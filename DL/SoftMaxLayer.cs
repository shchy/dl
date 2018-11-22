using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class SoftmaxLayer : FullyConnectedLayer
    {
        public SoftmaxLayer(ILayer before
                            , int nodeCount)
                            : base(before, nodeCount, DLF.SoftMax, DLF.UpdateWeightOfSoftMax)
        {
        }

    }
}