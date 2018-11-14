using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    /// <summary>
    /// 学習する何か
    /// </summary>
    public interface IMachine
    {
        IEnumerable<ILayer> Layers { get; }

        /// <summary>
        /// 学習
        /// </summary>
        /// <param name="learningData"></param>
        /// <returns></returns>
        void Learn(IEnumerable<ILearningData> learningData);

        /// <summary>
        /// 判断さす
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerable<double> Test(IEnumerable<double> data);
    }
}