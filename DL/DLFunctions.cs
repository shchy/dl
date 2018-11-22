using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    public static class DLF
    {
        static Random r = new Random(DateTime.Now.Millisecond);
        const double h = 1e-5;
        public static Func<double, double> Derivative(this Func<double, double> f)
        {
            return x => (f(x + h) - f(x - h)) / (2.0 * h);
        }

        public static double GetRandom()
        {
            var x = r.NextDouble();
            var y = r.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(x)) * Math.Cos(2.0 * Math.PI * y);
        }

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> xs)
        {
            var array = xs.ToArray();
            var n = array.Length;
            for (var i = n; i > 0; i--)
            {
                var next = r.Next(i - 1);
                yield return array[next];
            }
        }

        public static IEnumerable<double> ReLU(IEnumerable<double> xs) => xs.Select(x => Math.Max(0, x)).ToArray();

        public static IEnumerable<double> Sigmoid(IEnumerable<double> xs) => xs.Select(x => 1.0 / (1.0 + Math.Exp(-x))).ToArray();

        public static double SoftMax(INode node, double x)
        {
            var inputs = (
                from link in node.Links.Skip(1)
                let yi = link.InputNode.GetValue()// * link.Weight
                select Math.Exp(yi))
                .ToArray();
            var y = inputs[node.Index];
            var sum = inputs.Sum();

            return y / sum;

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
                var u = layer.CalcFunction(node);   // todo cache
                // 出力
                var o = node.GetValue();
                // 前の層の重み計算で使える部分
                node.Delta = ef.Derivative()(o) * of.Derivative()(u);
                // ((Func<double, double>)(x => layer.ActivationFunction(node, x))).Derivative()(u);

                // 入力Nodeごとに重みを更新
                foreach (var link in node.Links)
                {
                    // 前の層の出力
                    var o0 = link.InputNode.GetValue();
                    // 更新用の傾きを覚えておく
                    link.Slope += node.Delta * o0;
                    link.UpdateCount++;
                }
            }
        }

        /// 出力層以外の重み計算
        public static void UpdateWeight(ILayer layer
                                        , ILayer forwardLayer
                                        , Func<IEnumerable<Tuple<double, double>>, double> errorFunction
                                        , ILearningData data)
        {
            var ux = layer.Nodes.Select(layer.CalcFunction).ToArray();

            foreach (var item in layer.Nodes.Select((x, index) => new { x, index }))
            {
                var node = item.x;
                var index = item.index;
                // 活性化関数の偏微分
                Func<double, double> of = (double x) =>
                {
                    var rx = ux.ToArray();
                    rx[index] = x;
                    var result = layer.ActivationFunction(rx).ToArray();
                    return result[index];
                };

                // 活性化前の値
                var u = layer.CalcFunction(node);

                // 先の層の計算結果を引き継ぐ
                var forwardCache = (
                    from n in forwardLayer.Nodes
                    let delta = n.Delta
                    let l = n.Links.FirstOrDefault(l => l.InputNode == node)
                    where l != null
                    select delta * l.Weight).Sum();

                // 前の層の重み計算で使える部分
                node.Delta = forwardCache * of.Derivative()(u);
                //((Func<double, double>)(x => layer.ActivationFunction(node, x))).Derivative()(u);

                // 入力Nodeごとに重みを更新
                foreach (var link in node.Links)
                {
                    // 前の層の出力
                    var o0 = link.InputNode.GetValue();
                    // 更新用の傾きを覚えておく
                    link.Slope += node.Delta * o0;
                    link.UpdateCount++;
                }
            }
        }
    }
}