using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dl.DL;

namespace dl.DL
{
    public interface IValidator
    {
        ILearningResult Valid(IEnumerable<Tuple<ILearningData, IEnumerable<double>>> results);
    }

    public class Validator : IValidator
    {
        private int outputSize;

        public Validator(int outputSize)
        {
            this.outputSize = outputSize;
        }

        public ILearningResult Valid(IEnumerable<Tuple<ILearningData, IEnumerable<double>>> results)
        {
            var k = results.Count();
            var expectCount = new int[this.outputSize];
            var outputCount = new int[this.outputSize];
            var accuracyCount = new int[this.outputSize];

            foreach (var result in results)
            {
                var expected = result.Item1.Expected.ToArray();
                var output = result.Item2.ToArray();

                var expectIndex = FindMaxValueIndex(expected);
                var outputIndex = FindMaxValueIndex(output);

                // 期待値のIndexの数を記憶しておく
                expectCount[expectIndex] += 1;
                outputCount[outputIndex] += 1;

                // 正解率
                if (expectIndex == outputIndex)
                {
                    accuracyCount[outputIndex] += 1;
                }
            }

            var learningResult = new LearningResult
            {
                Expected = expectCount,
                Accuracy = accuracyCount.Sum() / (double)k,
                Recall = accuracyCount.Zip(expectCount, (a, e) => a / (double)e).ToArray(),
                Precision = accuracyCount.Zip(outputCount, (a, e) => a / (double)e).ToArray(),
            };
            return learningResult;
        }

        private static int FindMaxValueIndex(IEnumerable<double> xs)
        {
            var index = -1;
            var max = double.MinValue;
            foreach (var item in xs.Select((x, i) => new { x, i }))
            {
                if (max < item.x)
                {
                    index = item.i;
                    max = item.x;
                }
            }
            return index;
        }

    }

}