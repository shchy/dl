using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{

    /// 評価結果
    public interface ILearningResult
    {
        /// 正解率
        double Accuracy { get; }

        /// 精度
        IEnumerable<double> Precision { get; }

        /// 再現率
        IEnumerable<double> Recall { get; }
    }


    public class LearningResult : ILearningResult
    {
        /// 正解率
        public double Accuracy { get; set; }

        /// 精度
        public IEnumerable<double> Precision { get; set; }

        /// 再現率
        public IEnumerable<double> Recall { get; set; }

        public override string ToString()
        {
            return
                $"Accuracy:{Accuracy.ToString("0.00")}"
                + $" Precision:{ArrayToString(Precision)}"
                + $" Recall:{ArrayToString(Recall)}";
        }

        public static string ArrayToString(IEnumerable<double> xs)
        {
            return
                $"{{{string.Join(", ", xs.Select(x => x.ToString("0.00")))}}}";
        }

    }
}