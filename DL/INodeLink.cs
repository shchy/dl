using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    /// <summary>
    /// Nodeをつなぐ線
    /// </summary>
    public interface INodeLink
    {
        IWeight Weight { get; }
        /// <summary>
        /// 参照するNode
        /// </summary>
        INode InputNode { get; }
    }

    public interface IWeight
    {
        /// <summary>
        /// 重み
        /// </summary>
        float Value { get; set; }
        /// 重みに反映する傾き
        float Slope { get; set; }

        void Apply(float learningRate);
    }
}