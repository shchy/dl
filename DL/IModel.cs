using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace dl.DL
{
    public interface IModel
    {
        Func<IEnumerable<Tuple<float, float>>, float> ErrorFunction { get; }
        IEnumerable<ILayer> Layers { get; }
    }

    public static class ModelExtension
    {
        public static void Save(this IModel model, string path)
        {
            try
            {
                var layers =
                    new XElement("model",
                            from layer in model.Layers
                            select
                                new XElement("layer",
                                    from node in layer.Nodes
                                    select
                                        new XElement("node",
                                            from link in node.Links
                                            select
                                                new XElement("linkWeight", link.Weight.Value)
                                        )
                                )
                        );
                var xml =
                    new XDocument(layers);
                if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                xml.Save(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static bool Load(this IModel model, string path)
        {
            if (File.Exists(path) == false) return false;

            try
            {
                var xml = XDocument.Load(path);
                var layers = xml.Root.Elements("layer").ToArray();

                foreach (var layer in model.Layers.Zip(layers, (layer, xLayer) => (layer, xLayer)))
                {
                    var nodes = layer.xLayer.Elements("node").ToArray();
                    foreach (var node in layer.layer.Nodes.Zip(nodes, (node, xNode) => (node, xNode)))
                    {
                        var links = node.xNode.Elements("linkWeight").ToArray();
                        foreach (var link in node.node.Links.Zip(links, (link, xLink) => (link, xLink)))
                        {
                            link.link.Weight.Value = float.Parse(link.xLink.Value);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}