using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    /// <summary>
    /// Node
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// 入力Nodeのリスト
        /// </summary>
        IEnumerable<INodeLink> Links { get; }

        /// <summary>
        /// 活性化関数適応後の出力値
        /// </summary>
        double GetValue();

        double GetU();

        double Delta { get; set; }
        void Apply(double learningRate);
    }
}