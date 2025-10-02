using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
namespace InputLog.Core.TemplateBuilder
{
    public class TemplateXMLReader
    {
        #region Fields
        XmlDocument xdoc;

        String Title;

        String Id;

        List<TemplateBlock> Blocks;
        
        #endregion

        public TemplateXMLReader(string path)
        {
            xdoc = new XmlDocument();
            xdoc.Load(path);
            Blocks = new List<TemplateBlock>();
        }

        public void Read()
        {
            var root = xdoc.SelectSingleNode("report");
            Title = root.SelectSingleNode("title").InnerText;
            Id = root.SelectSingleNode("id").InnerText;
            //TODO : Language (if this is important ?)

            var block_list = root.SelectNodes("block");
            foreach (XmlNode block_node in block_list)
            {
                var block_title = block_node.SelectSingleNode("title").InnerText;
                var block_introduction = "";
                var block_data_string = "";

                var prepend_format = "";
                if (block_node.SelectSingleNode("prepend") != null)
                    prepend_format = block_node.SelectSingleNode("prepend").Attributes[0].Value;

                if (prepend_format == "simple")
                {
                    block_introduction = block_node.SelectSingleNode("prepend").InnerText;
                }
                else if (prepend_format == "complex") //complex
                {
                    foreach (XmlNode prepend_node in block_node.SelectSingleNode("prepend").ChildNodes)
                    {
                        if (prepend_node.Name == "paragraph")
                            block_introduction += prepend_node.InnerText + "\r\n";
                        else if (prepend_node.Name == "list")
                            foreach(XmlNode list_item_node in prepend_node.ChildNodes)
                                block_introduction += "*)" + list_item_node.InnerText + "\r\n";
                    }
                }

                if (block_node.SelectSingleNode("elements") != null)
                {
                    var elements_list = block_node.SelectSingleNode("elements").SelectNodes("element");

                    foreach (XmlNode element_node in elements_list)
                    {
                        var value_node = element_node.SelectSingleNode("value");
                        var value_label = value_node.SelectSingleNode("label").InnerText;
                        var value_id = value_node.SelectSingleNode("value_id").InnerText;

                        block_data_string += value_label + "[[" + value_id + "]]";

                        if (value_node.SelectSingleNode("append") != null)  // This is an optional field.
                        {
                            block_data_string += value_node.SelectSingleNode("append").InnerText;
                        }

                        if (element_node != elements_list[elements_list.Count - 1])
                            block_data_string += "\r\n";
                    }
                }
                Blocks.Add(new TemplateBlock(block_title, block_introduction, block_data_string));
            }
        }

        public List<TemplateBlock> GetBlocks() 
        {
            return Blocks;
        }

        public string GetTitle()
        {
            return Title;
        }
    }
}
