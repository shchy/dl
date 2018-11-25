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
            var batchSize = 8;
            var epoch = 1000;
            var learningRate = 0.01;
            var outputSize = 10;
            // 入力レイヤ
            var inputLayer = new InputLayer(28 * 28);
            // 隠れレイヤ
            var layer00 = new FullyConnectedLayer(inputLayer, 20, DLF.ReLU, DLF.UpdateWeight, DLF.GetRandomWeight);
            // 隠れレイヤ
            var layer01 = new FullyConnectedLayer(layer00, 10, DLF.ReLU, DLF.UpdateWeight, DLF.GetRandomWeight);
            // 出力レイヤ
            var layer02 = new SoftmaxLayer(layer01, outputSize);

            Func<IEnumerable<Tuple<double, double>>, double> errorFunction = DLF.ErrorFunctionCrossEntropy;

            var machine = new Machine(learningRate, epoch, batchSize, new Validator(outputSize)
                                    , x => errorFunction(x) * (1.0 / batchSize)
                                    , inputLayer
                                    , layer00
                                    , layer01
                                    , layer02);
            // 学習データを生成
            var testData = new MNISTLoader().Load().ToArray();

            machine.Learn(testData.ToArray());
        }
    }
}