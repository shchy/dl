using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{

    /// 評価結果
    public interface ILearningResult
    {
        /// 期待値の数
        IEnumerable<int> Expected { get; set; }
        /// 正解率
        float Accuracy { get; }

        /// 精度
        IEnumerable<float> Precision { get; }

        /// 再現率
        IEnumerable<float> Recall { get; }
    }


    public class LearningResult : ILearningResult
    {
        public IEnumerable<int> Expected { get; set; }
        /// 正解率
        public float Accuracy { get; set; }

        /// 精度
        public IEnumerable<float> Precision { get; set; }

        /// 再現率
        public IEnumerable<float> Recall { get; set; }

        public override string ToString()
        {
            return
                $"Accuracy:{Accuracy.ToString("0.00")}"
                + Environment.NewLine + $" Recall:{ArrayToString(Recall)}"
                + Environment.NewLine + $" Precision:{ArrayToString(Precision)}"
                + Environment.NewLine + $" Expected:{ArrayToString(Expected)}"
                ;
        }

        public static string ArrayToString(IEnumerable<float> xs)
        {
            return
                ArrayToString(xs, x => x.ToString("0.00"));
        }

        public static string ArrayToString(IEnumerable<int> xs)
        {
            return
                ArrayToString(xs, x => x.ToString("0000"));
        }

        public static string ArrayToString<T>(IEnumerable<T> xs, Func<T, string> toString)
        {
            return
                $"{{{string.Join(", ", xs.Select(x => toString(x)))}}}";
        }

    }
}