using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl.DL
{
    using UpdateWeightFunc = Action<ILayer, ILayer, Func<IEnumerable<Tuple<float, float>>, float>, ILearningData>;

    public static class DLF
    {
        static Random r = new Random(DateTime.Now.Millisecond);
        const float h = 1e-5f;
        const float hh = h * 2.0f;
        public static Func<float, float> Derivative(this Func<float, float> f)
        {
            return x => (f(x + h) - f(x - h)) / hh;
        }

        public static float GetRandom()
        {
            var x = r.NextDouble();
            var y = r.NextDouble();
            return (float)Math.Sqrt(-2.0 * Math.Log(x)) * (float)Math.Cos(2.0 * Math.PI * y);
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


        public static float CalcFunction(INode node)
        {
            return
                node.Links
                    .Select(link => link.InputNode.GetValue() * link.Weight.Value)
                    .Sum();
        }

        public static float GetRandomWeight() => 0.01f * DLF.GetRandom();

        public static float ErrorFunctionCrossEntropy(IEnumerable<Tuple<float, float>> result)
        {
            return (float)-result.Sum(a => a.Item2 * Math.Log(Math.Max(a.Item1, 1e-7)));// + (1 - a.Item2) * Math.Log(1 - a.Item1));
        }
        public static float ErrorFunction(IEnumerable<Tuple<float, float>> result)
        {
            return 0.5f * result.Sum(a => (float)Math.Pow(a.Item1 - a.Item2, 2));
        }

        public static IEnumerable<float> ReLU(IEnumerable<float> xs) => xs.Select(x => Math.Max(0, x)).ToArray();

        public static IEnumerable<float> Sigmoid(IEnumerable<float> xs) => xs.Select(x => 1.0f / (1.0f + (float)Math.Exp(-x))).ToArray();

        public static IEnumerable<float> SoftMax(IEnumerable<float> xs)
        {
            var max = xs.Max();
            var exps = xs.Select(x => (float)Math.Exp(x - max)).ToArray();
            var sum = exps.Sum();
            return exps.Select(x => x / sum).ToArray();

        }

        /// 出力層の重み計算
        public static void UpdateWeightOfOutputLayer(ILayer layer
                                                    , ILayer _
                                                    , Func<IEnumerable<Tuple<float, float>>, float> errorFunction
                                                    , ILearningData data)
        {
            var ox = layer.Nodes.Select(x => x.GetValue()).ToArray();
            var ux = layer.Nodes.Select(layer.CalcFunction).ToArray();

            foreach (var item in layer.Nodes.Select((x, index) => new { x, index }))
            {
                var node = item.x;
                var index = item.index;
                // 誤差関数の偏微分
                Func<float, float> ef = (float x) =>
                {
                    var rx = ox.ToArray();
                    rx[index] = x;
                    return errorFunction(rx.Zip(data.Expected, Tuple.Create));
                };
                // 活性化関数の偏微分
                Func<float, float> of = (float x) =>
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
                                                    , Func<IEnumerable<Tuple<float, float>>, float> errorFunction
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
        public static UpdateWeightFunc UpdateWeight(Func<float, bool> ignore = null, Action<INode, INodeLink, float> update = null)
        {
            ignore = ignore ?? (_ => false);
            update = update ?? Update;
            return (layer, forwardLayer, errorFunction, data) =>
            {
                var ux = layer.Nodes.Select(layer.CalcFunction).ToArray();

                // 活性化関数の偏微分
                Func<int, float> of = (int index) =>
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

        static void Update(INode node, INodeLink link, float delta)
        {
            // 前の層の出力
            var o0 = link.InputNode.GetValue();
            // 更新用の傾きを覚えておく
            link.Weight.Slope += delta * o0;
            link.InputNode.Delta += delta * link.Weight.Value;
        }

        public static int FindMaxValueIndex(IEnumerable<float> xs)
        {
            var index = -1;
            var max = float.MinValue;
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