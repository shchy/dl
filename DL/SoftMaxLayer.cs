using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class SoftmaxLayer : FullyConnectedLayer
    {
        public SoftmaxLayer(ILayer before
                            , int nodeCount
                            , Action<ILayer, ILayer, Func<IEnumerable<Tuple<float, float>>, float>, ILearningData> updateWeightFunction = null)
                            : base(before, nodeCount, DLF.SoftMax, updateWeightFunction ?? DLF.UpdateWeightOfSoftMax, () => 0.0f)
        {
        }
    }
}