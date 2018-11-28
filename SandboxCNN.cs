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
            var learningRate = 0.005;
            var outputSize = 10;
            // 入力レイヤ
            var inputLayer = new InputLayer(28 * 28);
            // 畳み込みレイヤ
            // プーリングレイヤ
            var layer00 = new ConvolutionLayer(inputLayer, 28, 1, 3, 8, 1, DLF.ReLU, u => u < 0);
            var layer01 = new PoolingLayer(layer00, 26, 8, 2, 2);
            // 畳み込みレイヤ
            // プーリングレイヤ
            var layer02 = new ConvolutionLayer(layer01, 13, 8, 3, 50, 2, DLF.ReLU, u => u < 0);
            var layer03 = new PoolingLayer(layer02, 6, 50, 2, 2);
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
            var testData = DLF.Shuffle(new MNISTLoader().Load()).ToArray();

            // 0-9を均等にピックアップ
            var pickNum = 20;
            var a = new[]{
                testData.Take(10000).Where(x => x.Name == "0").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "1").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "2").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "3").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "4").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "5").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "6").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "7").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "8").Take(pickNum),
                testData.Take(10000).Where(x => x.Name == "9").Take(pickNum),
            }.SelectMany(x => x).ToArray();
            var b = new[]{
                testData.Skip(10000).Where(x => x.Name == "0").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "1").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "2").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "3").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "4").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "5").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "6").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "7").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "8").Take(pickNum),
                testData.Skip(10000).Where(x => x.Name == "9").Take(pickNum),
            }.SelectMany(x => x).ToArray();

            machine.Learn(a, b);
        }
    }
}