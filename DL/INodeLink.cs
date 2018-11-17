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
        /// <summary>
        /// 重み
        /// </summary>
        double Weight { get; set; }

        double Slope { get; set; }


        int UpdateCount { get; set; }
        /// <summary>
        /// 参照するNode
        /// </summary>
        INode InputNode { get; }
    }
}