using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class LearningData : ILearningData
    {
        public IEnumerable<double> Data { get; set; }

        public IEnumerable<double> Expected { get; set; }

        public static ILearningData New(IEnumerable<double> data, IEnumerable<double> expected)
        {
            return new LearningData
            {
                Data = data,
                Expected = expected
            };
        }
    }
}