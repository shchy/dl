using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public class LearningData : ILearningData
    {
        public string Name { get; set; }

        public IEnumerable<float> Data { get; set; }

        public IEnumerable<float> Expected { get; set; }

        public static ILearningData New(string name, IEnumerable<float> data, IEnumerable<float> expected)
        {
            return new LearningData
            {
                Name = name,
                Data = data,
                Expected = expected
            };
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}