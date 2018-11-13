using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dl
{
    /// <summary>
    /// 教師データ
    /// </summary>
    public interface ILearningData
    {
        /// <summary>
        /// 入力値
        /// </summary>
        IEnumerable<double> Data { get; }
        /// <summary>
        /// 期待値
        /// </summary>
        IEnumerable<double> Expected { get; }
    }

    public class LearningData : ILearningData
    {
        public IEnumerable<double> Data { get; set; }

        public IEnumerable<double> Expected { get; set; }

        public static ILearningData New(IEnumerable<double> data, IEnumerable<double> expected)
        {
            return new LearningData
            {
                Data = data,
                Expected = expected
            };
        }
    }


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


    public interface ILayer
    {
        /// <summary>
        /// レイヤ内のノード達
        /// </summary>
        IEnumerable<INode> Nodes { get; }
    }

    public class InputLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }

        public InputLayer(int inputDataSize)
        {
            // 固定値Nodeを作成
            var inputNodes = Enumerable.Range(0, inputDataSize).Select(_ => new ValueNode());
            this.Nodes = inputNodes.ToArray();
        }

        public void UpdateData(IEnumerable<double> data)
        {
            // todo サイズチェック
            foreach (var item in Nodes.OfType<ValueNode>().Zip(data, Tuple.Create))
            {
                item.Item1.Value = item.Item2;
            }
        }
    }

    public class FullyConnectedLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }

        public FullyConnectedLayer(ILayer before, Func<double, double> activation, int nodeCount)
        {
            var nodes =
                from i in Enumerable.Range(0, nodeCount)
                let nodeLink = before.Nodes.MakeLink()
                let node = new Node(activation, nodeLink)
                select node;
            this.Nodes = nodes.ToArray();
        }
    }

    public static class LayerFunction
    {
        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes)
        {
            return
                new[] { new NodeLink { InputNode = new ValueNode { Value = 1 }, Weight = 0.01 * GetRandom() } }
                .Concat(nodes.Select(n => new NodeLink { InputNode = n, Weight = 0.01 * GetRandom() }))
                .ToArray();
        }

        public static double GetRandom()
        {
            var r = new Random(DateTime.Now.Millisecond);
            var x = r.NextDouble();
            var y = r.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(x)) * Math.Cos(2.0 * Math.PI * y);
        }
    }

    /// <summary>
    /// Nodeをつなぐ線
    /// </summary>
    public interface INodeLink
    {
        /// <summary>
        /// 重み
        /// </summary>
        double Weight { get; set; }

        /// <summary>
        /// 参照するNode
        /// </summary>
        INode InputNode { get; }

    }

    public class NodeLink : INodeLink
    {
        public double Weight { get; set; }

        public INode InputNode { get; set; }
    }

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

        /// <summary>
        /// 重みを更新
        /// </summary>
        void UpdateWeight(double learningRate, double expected);
    }

    public class Node : INode
    {
        private readonly Func<double, double> activation;

        public IEnumerable<INodeLink> Links { get; set; }

        public Node(Func<double, double> activation, IEnumerable<INodeLink> links)
        {
            this.activation = activation;
            this.Links = links.ToArray();
        }

        public double GetValue()
        {
            var u = Links
                .Select(link => link.InputNode.GetValue() * link.Weight)
                .Sum();
            var o = this.activation(u);
            return o;
        }

        public void UpdateWeight(double learningRate, double expected)
        {
            var u = Links
                .Select(link => link.InputNode.GetValue() * link.Weight)
                .Sum();
            var h = 1.0E-10;
            // 入力Nodeごとに重みを更新
            foreach (var link in Links)
            {
                var o0 = link.InputNode.GetValue();
                var o = this.activation(u);
                var du = this.activation(u + h) - o - h;

                var slope = learningRate * ((o - expected) * (1 - o) * o * o0);
                link.Weight = link.Weight - slope;
            }
        }
    }

    public class ValueNode : INode
    {
        public double Value { get; set; }

        public IEnumerable<INodeLink> Links => Enumerable.Empty<INodeLink>();

        public ValueNode()
        {
            this.Value = 0.0;
        }

        public double GetValue() => this.Value;

        public void UpdateWeight(double learningRate, double expected)
        {
            // 固定値ノードは更新不要
        }
    }
}
