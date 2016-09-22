using System.Collections.Generic;
using System.Linq;

namespace CSharp_HtmlParser_Library.HtmlDocumentStructure
{
    public enum Flags
    {
        NormalTag, SelfClosing, Text, SpecialTag, ContainsClosingTag,
        ClosingTagIncudedInNodeTree, EndTag, ContainsFrontslashAtTheEnd
    }
    public class HtmlDocumentNode
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public int Line {get; set;}
        public HtmlDocumentNode ParentNode { get; set; }
        public IList<HtmlAttribute> Attributes { get; set; }
        public IList<HtmlDocumentNode> ChildNodes { get; set; }
        public IList<Flags> Flags {get; set;}  
        public IList<HtmlDocumentNode> Descendants => GetDescendants();
        public string InnerHtml => GetInnerHtml();
        public string InnerText => GetInnerText();
        public string OwnHtml => GetOwnHtml();
        private string _ownText;
        public string OwnText 
        {
            get
            {
                if(_ownText == string.Empty)
                {
                    return GetOwnText();
                }

                return _ownText;            
            }
            set
            {
                _ownText = value;
            }
        }
  

        private string _outerHtml;
        public string OuterHtml 
        {
            get
            {
                if(_outerHtml == string.Empty)
                {
                    return GetOuterHtml();
                }
                else
                {
                    return _outerHtml;
                }
            }
            set
            {
                _outerHtml = value;
            }
        }
        

        public HtmlDocumentNode()
        {
            Attributes = new List<HtmlAttribute>();
            ChildNodes = new List<HtmlDocumentNode>();
            Flags = new List<Flags>();
            Name = string.Empty;
            _ownText = string.Empty;
            _outerHtml = string.Empty;
        }       

        private string GetOuterHtml()
        {
            HtmlGenerator generator = new HtmlGenerator(this);
            generator.GenerateHtml();

            return generator.NodeHtml;
        }

        private string GetInnerHtml()
        {
            string innerHtml = string.Empty;
            foreach (var node in ChildNodes)
            {
                innerHtml += node.OuterHtml;
            }

            return innerHtml;
        }

        private string GetOwnHtml()
        {
            HtmlGenerator generator = new HtmlGenerator(this);
            generator.GenerateTag();

            return generator.NodeHtml;
        }

        private string GetOwnText()
        {
            string ownText = string.Empty;

            foreach (var node in ChildNodes)
            {
                if(node.Name == "#text")
                {
                    ownText += node.OwnText; 
                }
            }

            return ownText;
        }

        private string GetInnerText()
        {
            string text = string.Empty;
            
            foreach (var node in ChildNodes)
            {
                if (node.Name == "#text")
                {
                    text += node.OwnText;
                }
                else
                {
                    text += node.InnerText;
                }                
            }

            return text;
        }

        private List<HtmlDocumentNode> GetDescendants()
        {
            List<HtmlDocumentNode> descendants = new List<HtmlDocumentNode>();
            descendants.AddRange(ChildNodes);

            foreach (var childNode in ChildNodes)
            {
                descendants.AddRange(childNode.Descendants);
            }

            descendants = descendants.Select(x=>x).OrderBy(x=>x.Position).ToList();

            return descendants;
        }       

        public void DeleteNode(HtmlDocumentNode nodeToDelete)
        {
            List<HtmlDocumentNode> children = new List<HtmlDocumentNode>();
            children.AddRange(ChildNodes);

            foreach (var node in children)
            {
                if (node == nodeToDelete)
                {
                    ChildNodes.Remove(node);
                }
                else
                {
                    node.DeleteNode(nodeToDelete);
                }
            }
        }
    }
}
