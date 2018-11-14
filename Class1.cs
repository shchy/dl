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

        Func<double, double> ActivationFunction { get; }

        void UpdateWeight(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, ILearningData data);
    }

    public class InputLayer : ILayer
    {
        public IEnumerable<INode> Nodes { get; set; }

        public Func<double, double> ActivationFunction { get; } = x => x;

        public InputLayer(int inputDataSize)
        {
            // 固定値Nodeを作成
            var inputNodes = Enumerable.Range(0, inputDataSize).Select(_ => new ValueNode());
            this.Nodes = inputNodes.ToArray();
        }

        public void UpdateWeight(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, ILearningData data) { }

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
        private Action<ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> updateWeight;

        public IEnumerable<INode> Nodes { get; set; }
        public Func<double, double> ActivationFunction { get; set; }

        public FullyConnectedLayer(ILayer before, Func<double, double> activation, int nodeCount, Action<ILayer, Func<IEnumerable<Tuple<double, double>>, double>, ILearningData> updateWeight)
        {
            this.updateWeight = updateWeight;
            this.ActivationFunction = activation;

            var nodes =
                from i in Enumerable.Range(0, nodeCount)
                let nodeLink = before.Nodes.MakeLink()
                let node = new Node(activation, nodeLink)
                select node;
            this.Nodes = nodes.ToArray();
        }

        public void UpdateWeight(Func<IEnumerable<Tuple<double, double>>, double> errorFunction, ILearningData data)
        {
            this.updateWeight(this, errorFunction, data);
        }
    }


    public static class LayerFunction
    {
        public static IEnumerable<INodeLink> MakeLink(this IEnumerable<INode> nodes)
        {
            return
                new[] { new NodeLink { InputNode = new ValueNode { Value = 1 }, Weight = 0.01 * MathExtension.GetRandom() } }
                .Concat(nodes.Select(n => new NodeLink { InputNode = n, Weight = 0.01 * MathExtension.GetRandom() }))
                .ToArray();
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

        double Slope { get; set; }
        /// <summary>
        /// 参照するNode
        /// </summary>
        INode InputNode { get; }
    }

    public class NodeLink : INodeLink
    {
        public double Weight { get; set; }

        public double Slope { get; set; }

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

        double GetU();

        double Delta { get; set; }
        void Apply(double learningRate);
    }

    public class Node : INode
    {
        private readonly Func<double, double> activation;
        public IEnumerable<INodeLink> Links { get; set; }
        private double? u;
        private double? output;
        public double Delta { get; set; }

        public Node(Func<double, double> activation, IEnumerable<INodeLink> links)
        {
            this.activation = activation;
            this.Links = links.ToArray();
            this.u = null;
            this.output = null;
        }

        public void Apply(double learningRate)
        {
            this.u = null;
            this.output = null;
            this.Delta = 0.0;
            foreach (var link in this.Links)
            {
                link.Weight -= link.Slope * learningRate;
                link.Slope = 0.0;
            }
        }

        public double GetValue()
        {
            if (this.output.HasValue == false)
                this.output = this.activation(GetU());
            return output.Value;
        }

        public double GetU()
        {
            if (u.HasValue == false)
                u = Links
                    .Select(link => link.InputNode.GetValue() * link.Weight)
                    .Sum();
            return u.Value;
        }
    }

    public class ValueNode : INode
    {
        public double Value { get; set; }
        public double Delta { get; set; }
        public IEnumerable<INodeLink> Links => Enumerable.Empty<INodeLink>();

        public ValueNode()
        {
            this.Value = 0.0;
        }

        public double GetValue() => this.Value;
        public double GetU() => this.Value;
        public void Apply(double learningRate) { }

    }

    public static class MathExtension
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
    }
}
