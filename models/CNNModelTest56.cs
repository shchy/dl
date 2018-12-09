
using System;
using System.Collections.Generic;
using dl.DL;

namespace dl
{
    class CNNModelTest56 : IModel
    {
        public Func<IEnumerable<Tuple<float, float>>, float> ErrorFunction { get; set; }

        public IEnumerable<ILayer> Layers { get; set; }

        public CNNModelTest56()
        {
            // 入力レイヤ
            var inputLayer = new InputLayer(56, 56);
            // 畳み込みレイヤ
            // プーリングレイヤ
            var layer00 = new ConvolutionLayer(inputLayer, (3, 1, 20), DLF.ReLU, u => u < 0);
            var layer01 = new PoolingLayer(layer00, (2, 2));
            // 畳み込みレイヤ
            // プーリングレイヤ
            var layer02 = new ConvolutionLayer(layer01, (3, 2, 50), DLF.ReLU, u => u < 0);
            var layer03 = new PoolingLayer(layer02, (2, 2));
            // 出力レイヤ
            var layer04 = new SoftmaxLayer(layer03, 10);
            this.Layers = new ILayer[]{
                inputLayer
                , layer00
                , layer01
                , layer02
                , layer03
                , layer04
            };
            this.ErrorFunction = DLF.ErrorFunctionCrossEntropy;
        }
    }
}