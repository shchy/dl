using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dl.DL;

namespace dl.DL
{
    public interface IValidator
    {
        ILearningResult Valid(IEnumerable<Tuple<IEnumerable<double>, IEnumerable<double>>> results);
    }

    public class Validator : IValidator
    {
        public ILearningResult Valid(IEnumerable<Tuple<IEnumerable<double>, IEnumerable<double>>> results)
        {
            var k = results.Count();
            var expectCount = new Dictionary<int, int>();
            var outputCount = new Dictionary<int, int>();
            var accuracyCount = new Dictionary<int, int>();
            Action<IDictionary<int, int>, int> countup = (dic, key) =>
            {
                if (dic.ContainsKey(key) == false)
                    dic[key] = 0;
                dic[key] += 1;
            };

            foreach (var result in results)
            {
                var expected = result.Item1.ToArray();
                var output = result.Item2.ToArray();

                var expectIndex = FindMaxValueIndex(expected);
                var outputIndex = FindMaxValueIndex(output);

                // 期待値のIndexの数を記憶しておく
                countup(expectCount, expectIndex);
                countup(outputCount, outputIndex);

                // 正解率
                if (expectIndex == outputIndex)
                {
                    countup(accuracyCount, outputIndex);
                }
            }

            var learningResult = new LearningResult
            {
                Expected = expectCount.Values.ToArray(),
                Accuracy = accuracyCount.Values.Sum() / (double)k,
                Recall = accuracyCount.Values.Zip(expectCount.Values, (a, e) => a / (double)e).ToArray(),
                Precision = accuracyCount.Values.Zip(outputCount.Values, (a, e) => a / (double)e).ToArray(),
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