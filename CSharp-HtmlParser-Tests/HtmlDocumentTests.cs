using System.Linq;
using CSharp_HtmlParser_Library.HtmlDocumentStructure;
using NUnit.Framework;

namespace CSharp_HtmlParser_Tests
{
    public class HtmlDocumentTests
    {
        
        [Test]
        public void NodeDeleteTest()
        {
            string input = "<html><head>Header<br></head><body>Body<div>Content<hr></div></body></html>";

            HtmlDocument doc = new HtmlDocument(input);
            doc.Parse();

            HtmlDocumentNode root = doc.RootNode;

            Assert.AreEqual(root.InnerHtml, "<html><head>Header<br></head><body>Body<div>Content<hr></div></body></html>");

            HtmlDocumentNode nodeToDelete = root.Descendants.FirstOrDefault(x => x.Name == "head");
            root.DeleteNode(nodeToDelete);

            Assert.AreEqual(root.InnerHtml, "<html><body>Body<div>Content<hr></div></body></html>");

            HtmlDocumentNode nodeToDelete2 = root.Descendants.FirstOrDefault(x => x.Name == "hr");
            root.DeleteNode(nodeToDelete2);

            Assert.AreEqual(root.InnerHtml, "<html><body>Body<div>Content</div></body></html>");

            HtmlDocumentNode nodeToDelete3 = root.Descendants.FirstOrDefault(x => x.Name == "#text" && x.OwnText == "Content");
            root.DeleteNode(nodeToDelete3);

            Assert.AreEqual(root.InnerHtml, "<html><body>Body<div></div></body></html>");
        }

        [Test]
        public void OwnTextTest()
        {
            string input =
                "Text0<div id=\"divider\" class=\"big and small\">Text1<b>Text2</b>Text3<br/><i>Text4</i><img>Text5</div>Text6Text7<br>Text8";


            HtmlDocument doc = new HtmlDocument(input);


            doc.Parse();
            HtmlDocumentNode node = doc.RootNode;
            Assert.AreEqual("Text0Text6Text7Text8", node.OwnText);
        }

        [Test]
        public void InnerTextTest()
        {
            string input =
                "Text0<div id=\"divider\" class=\"big and small\">Text1<b>Text2</b>Text3<br/><i>Text4</i><img>Text5</div>Text6Text7<br>Text8";


            HtmlDocument doc = new HtmlDocument(input);


            doc.Parse();
            HtmlDocumentNode node = doc.RootNode;
            Assert.AreEqual("Text0Text1Text2Text3Text4Text5Text6Text7Text8", node.InnerText);
        }
    }
}
