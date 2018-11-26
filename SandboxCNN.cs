using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dl.DL;

namespace dl
{
    class CNN
    {
        public void Run()
        {
            var batchSize = 10;
            var epoch = 50;
            var learningRate = 0.01;
            var outputSize = 10;
            // 入力レイヤ
            var inputLayer = new InputLayer(28 * 28);
            // 畳み込みレイヤ
            var layer00 = new ConvolutionLayer(inputLayer, 28, 1, 3, 3, DLF.ReLU, DLF.UpdateWeightPooling);
            // プーリングレイヤ
            var layer01 = new PoolingLayer(layer00, 26, 3, 2, DLF.ReLU, DLF.UpdateWeight);
            // 畳み込みレイヤ
            var layer02 = new ConvolutionLayer(layer01, 13, 3, 3, 1, DLF.ReLU, DLF.UpdateWeightPooling);
            // プーリングレイヤ
            var layer03 = new PoolingLayer(layer02, 11, 3, 2, DLF.ReLU, DLF.UpdateWeight);
            // 出力レイヤ
            var layer04 = new SoftmaxLayer(layer03, outputSize);

            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = DLF.ErrorFunctionCrossEntropy;

            var machine = new Machine(learningRate, epoch, batchSize, new Validator(outputSize)
                                    , x => errorFunction(x) * (1.0 / batchSize)
                                    , inputLayer
                                    , layer00
                                    , layer01
                                    , layer02
                                    , layer03
                                    , layer04);
            // 学習データを生成
            var testData = new MNISTLoader().Load().ToArray();

            machine.Learn(testData.Take(100).ToArray());
        }
    }
}