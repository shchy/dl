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
            var model = new CNNModelTest();

            var machine = new Machine(learningRate, epoch, batchSize, new Validator(), model);

            // 学習データを生成
            // var labelFile = "train-labels-idx1-ubyte";
            // var imageFile = "train-images-idx3-ubyte";
            // var testData = DLF.Shuffle(new MNISTLoader().Load(labelFile, imageFile)).ToArray();
            var bmpLoader = new BitmapLoader();
            var testData = DLF.Shuffle(bmpLoader.Load("./temp/files.xml")).ToArray();

            // 0-9を均等にピックアップ
            var pickNum = 10;
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




    class CNNModelTest : IModel
    {
        public Func<IEnumerable<Tuple<double, double>>, double> ErrorFunction { get; set; }

        public IEnumerable<ILayer> Layers { get; set; }

        public CNNModelTest()
        {
            // 入力レイヤ
            var inputLayer = new InputLayer(28, 28);
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