using System;
using System.Collections.Generic;
using dl.DL;

namespace dl
{
    class TestModel : IModel
    {
        public Func<IEnumerable<Tuple<float, float>>, float> ErrorFunction { get; set; }

        public IEnumerable<ILayer> Layers { get; set; }

        public TestModel()
        {
            // 入力レイヤ
            var inputLayer = new InputLayer(3);
            // 隠れレイヤ
            var layer00 = new FullyConnectedLayer(inputLayer, 20, DLF.ReLU, DLF.UpdateWeight(), DLF.GetRandomWeight);
            // 隠れレイヤ
            var layer01 = new FullyConnectedLayer(layer00, 10, DLF.ReLU, DLF.UpdateWeight(), DLF.GetRandomWeight);
            // 出力レイヤ
            var layer02 = new SoftmaxLayer(layer01, 3);

            this.Layers = new ILayer[]{
                inputLayer
                , layer00
                , layer01
                , layer02
            };
            this.ErrorFunction = DLF.ErrorFunctionCrossEntropy;
        }
    }
}