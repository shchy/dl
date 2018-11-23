using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    /// <summary>
    /// 教師データ
    /// </summary>
    public interface ILearningData
    {
        string Name { get; }
        /// <summary>
        /// 入力値
        /// </summary>
        IEnumerable<double> Data { get; }
        /// <summary>
        /// 期待値
        /// </summary>
        IEnumerable<double> Expected { get; }
    }
}