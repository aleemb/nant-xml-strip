using System;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

// based off code from
// http://stackoverflow.com/questions/537988/how-can-i-use-nants-xmlpoke-target-to-remove-a-node
//
// example:
// <xmlstrip xpath="//rootnode/childnode[@arg = 'b']" file="target.xml" />
//
// - modified original to remove all nodes matching xpath expression instead of single node
// - added replace attribute to allow last matched node to be replaced by new node
//

namespace XmlStrip
{
    [TaskName("xmlstrip")]
    public class XmlStrip : Task
    {
        [TaskAttribute("xpath", Required = true), StringValidator(AllowEmpty = false)]
        public string XPath { get; set; }

        [TaskAttribute("file", Required = true)]
        public FileInfo XmlFile { get; set; }

        [TaskAttribute("replace", Required = false)]
        public String NewXml { get; set; }

        protected override void ExecuteTask()
        {
            string filename = XmlFile.FullName;
            Log(Level.Info, "Attempting to load XML document in file '{0}'.", filename );
            XmlDocument document = new XmlDocument();
            document.Load(filename);
            Log(Level.Info, "XML document in file '{0}' loaded successfully.", filename );

            XmlNodeList nodes = document.SelectNodes(XPath);
            if(null == nodes)
            {
                throw new BuildException(String.Format("Node not found by XPath '{0}'", XPath));
            }
            for (Int32 i = 0, c = nodes.Count, last = c - 1; i < c; i++)
            {
                XmlNode node = nodes[i];
                if (i == last && NewXml != null)
                {
                    Log(Level.Info, "Replacing last node with new XML: {0}", NewXml);
                    XmlNode newnode = document.ReadNode(new XmlTextReader(new StringReader(NewXml)));
                    node.ParentNode.ReplaceChild(newnode, node);
                }
                else
                {
                    node.ParentNode.RemoveChild(node);
                }
            }

            // if (null != NewNode) node.ParentNode.AppendChild(NewNode);

            Log(Level.Info, "Attempting to save XML document to '{0}'.", filename );
            document.Save(filename);
            Log(Level.Info, "XML document successfully saved to '{0}'.", filename );
        }
    }
}
