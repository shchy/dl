using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    using UpdateWeightFunc = Action<ILayer, ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData>;

    public static class DLF
    {
        static Random r = new Random(DateTime.Now.Millisecond);
        const double h = 1e-5;
        const double hh = h * 2.0;
        public static Func<double, double> Derivative(this Func<double, double> f)
        {
            return x => (f(x + h) - f(x - h)) / hh;
        }

        public static double GetRandom()
        {
            var x = r.NextDouble();
            var y = r.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(x)) * Math.Cos(2.0 * Math.PI * y);
        }

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> xs)
        {
            var array = xs.ToList();
            var n = array.Count;
            for (var i = n; i > 0; i--)
            {
                var next = r.Next(i - 1);
                yield return array[next];
                array.RemoveAt(next);
            }
        }


        public static double CalcFunction(INode node)
        {
            return
                node.Links
                    .Select(link => link.InputNode.GetValue() * link.Weight.Value)
                    .Sum();
        }

        public static double GetRandomWeight() => 0.01 * DLF.GetRandom();

        public static double ErrorFunctionCrossEntropy(IEnumerable<Tuple<double, double>> result)
        {
            return -result.Sum(a => a.Item2 * Math.Log(Math.Max(a.Item1, 1e-7)));// + (1 - a.Item2) * Math.Log(1 - a.Item1));
        }
        public static double ErrorFunction(IEnumerable<Tuple<double, double>> result)
        {
            return 0.5 * result.Sum(a => Math.Pow(a.Item1 - a.Item2, 2));
        }

        public static IEnumerable<double> ReLU(IEnumerable<double> xs) => xs.Select(x => Math.Max(0, x)).ToArray();

        public static IEnumerable<double> Sigmoid(IEnumerable<double> xs) => xs.Select(x => 1.0 / (1.0 + Math.Exp(-x))).ToArray();

        public static IEnumerable<double> SoftMax(IEnumerable<double> xs)
        {
            var max = xs.Max();
            var exps = xs.Select(x => Math.Exp(x - max)).ToArray();
            var sum = exps.Sum();
            return exps.Select(x => x / sum).ToArray();

        }

        /// 出力層の重み計算
        public static void UpdateWeightOfOutputLayer(ILayer layer
                                                    , ILayer _
                                                    , Func<IEnumerable<Tuple<double, double>>, double> errorFunction
                                                    , ILearningData data)
        {
            var ox = layer.Nodes.Select(x => x.GetValue()).ToArray();
            var ux = layer.Nodes.Select(layer.CalcFunction).ToArray();

            foreach (var item in layer.Nodes.Select((x, index) => new { x, index }))
            {
                var node = item.x;
                var index = item.index;
                // 誤差関数の偏微分
                Func<double, double> ef = (double x) =>
                {
                    var rx = ox.ToArray();
                    rx[index] = x;
                    return errorFunction(rx.Zip(data.Expected, Tuple.Create));
                };
                // 活性化関数の偏微分
                Func<double, double> of = (double x) =>
                {
                    var rx = ux.ToArray();
                    rx[index] = x;
                    var result = layer.ActivationFunction(rx).ToArray();
                    return result[index];
                };

                // 活性化前の値
                var u = ux[index];   // todo cache
                // 出力
                var o = ox[index];
                // 前の層の重み計算で使える部分
                var delta = ef.Derivative()(o) * of.Derivative()(u);

                // 入力Nodeごとに重みを更新
                foreach (var link in node.Links)
                {
                    Update(node, link, delta);
                }
            }
        }

        public static void UpdateWeightOfSoftMax(ILayer layer
                                                    , ILayer _
                                                    , Func<IEnumerable<Tuple<double, double>>, double> errorFunction
                                                    , ILearningData data)
        {
            var ys = layer.Nodes.Select(x => x.GetValue()).ToArray();
            var ts = data.Expected.ToArray();
            var deltas = ys.Zip(ts, (y, t) => (y - t)).ToArray();

            foreach (var item in layer.Nodes.Zip(deltas, Tuple.Create))
            {
                var node = item.Item1;
                var delta = item.Item2;

                // 入力Nodeごとに重みを更新
                foreach (var link in node.Links)
                {
                    Update(node, link, delta);
                }
            }
        }

        /// 出力層以外の重み計算
        public static UpdateWeightFunc UpdateWeight(Func<double, bool> ignore = null, Action<INode, INodeLink, double> update = null)
        {
            ignore = ignore ?? (_ => false);
            update = update ?? Update;
            return (layer, forwardLayer, errorFunction, data) =>
            {
                var ux = layer.Nodes.Select(layer.CalcFunction).ToArray();

                // 活性化関数の偏微分
                Func<int, double> of = (int index) =>
                {
                    var temp = ux[index];
                    ux[index] = temp + h;
                    var left = layer.ActivationFunction(ux).ToArray();
                    ux[index] = temp - h;
                    var right = layer.ActivationFunction(ux).ToArray();
                    var div = (left[index] - right[index]) / hh;
                    ux[index] = temp;
                    return div;
                };
                var nodes = layer.Nodes.Select((node, index) => new { node, index })
                    .ToArray();


                foreach (var item in nodes)
                {
                    var index = item.index;

                    if (ignore(ux[index])) continue;

                    var node = item.node;
                    if (node.Delta == 0.0) continue;

                    // 前の層の重み計算で使える部分
                    var delta = node.Delta * of(index);

                    if (delta == 0.0) continue;

                    // 入力Nodeごとに重みを更新
                    foreach (var link in node.Links)
                    {
                        update(node, link, delta);
                    }
                }
            };
        }

        static void Update(INode node, INodeLink link, double delta)
        {
            // 前の層の出力
            var o0 = link.InputNode.GetValue();
            // 更新用の傾きを覚えておく
            link.Weight.Slope += delta * o0;
            link.InputNode.Delta += delta * link.Weight.Value;
        }

        public static int FindMaxValueIndex(IEnumerable<double> xs)
        {
            var index = -1;
            var max = double.MinValue;
            foreach (var item in xs.Select((x, i) => new { x, i }))
            {
                if (max < item.x)
                {
                    index = item.i;
                    max = item.x;
                }
            }
            return index;
        }
    }
}