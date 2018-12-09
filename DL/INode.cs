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

        void SetValue(float v);

        /// <summary>
        /// 活性化関数適応後の出力値
        /// </summary>
        float GetValue();

        void Reset();
        float Delta { get; set; }
        void Apply(float learningRate);
    }
}