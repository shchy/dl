using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public interface IModel
    {
        Func<IEnumerable<Tuple<double, double>>, double> ErrorFunction { get; }
        IEnumerable<ILayer> Layers { get; }
    }
}