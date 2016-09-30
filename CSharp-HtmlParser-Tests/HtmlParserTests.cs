using CSharp_HtmlParser_Library;
using CSharp_HtmlParser_Library.HtmlDocumentStructure;
using NUnit.Framework;


namespace CSharp_HtmlParser_Tests
{    
    public class HtmlParserTests
    {
        [Test]
        public void EmptyDocument()
        {
            HtmlDocument doc = new HtmlDocument("");

            doc.Parse();

            Assert.AreEqual(0,doc.RootNode.ChildNodes.Count);
        }
        [Test]
        public void UnknownTag()
        {
            HtmlDocument doc = new HtmlDocument("<[unknown]>");

            doc.Parse();

            Assert.AreEqual(1, doc.RootNode.ChildNodes.Count);
            Assert.AreEqual("<[unknown]>", doc.RootNode.ChildNodes[0].OuterHtml);
        }
        [Test]
        public void StartTag()
        {
            HtmlDocument doc = new HtmlDocument("<p>");

            doc.Parse();

            Assert.AreEqual(1, doc.RootNode.ChildNodes.Count);
            Assert.AreEqual("p", doc.RootNode.ChildNodes[0].Name);
        }
        [Test]
        public void StartAndEndTag()
        {
            HtmlDocument doc = new HtmlDocument("<p></p>");

            doc.Parse();

            Assert.AreEqual(1, doc.RootNode.ChildNodes.Count);
            Assert.AreEqual("p", doc.RootNode.ChildNodes[0].Name);
        }
        [Test]
        public void SimpleHtmlStructure()
        {
            string input =
                "<div id=\"divider\" class=\"big and small\"><b> Some</b><i>text</i> more text</div><u> enter </u> End text";

            HtmlDocument doc = new HtmlDocument(input);

            doc.Parse();
           
            Assert.AreEqual(3, doc.RootNode.ChildNodes.Count);
            Assert.AreEqual("div", doc.RootNode.ChildNodes[0].Name);
            Assert.AreEqual("u", doc.RootNode.ChildNodes[1].Name);
            Assert.AreEqual("#text", doc.RootNode.ChildNodes[2].Name);
            Assert.AreEqual(3, doc.RootNode.ChildNodes[0].ChildNodes.Count);
            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }
        [Test]
        public void SelfClosingXHtmlTag()
        {
            string input = "<img id=\"divider\" class=\"big and small\"/>";

            HtmlDocument doc = new HtmlDocument(input);
            
            doc.Parse();
            
            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }
        [Test]
        public void SelfClosingHtmlTag()
        {
            string input = "<img id=\"divider\" class=\"big and small\">";

            HtmlDocument doc = new HtmlDocument(input);           
            
            doc.Parse();
            
            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }
        [Test]
        public void MixedHtmlStructure()
        {
            string input =
                "<div id=\"divider\" class=\"big and small\"><b> Some<br/></b><i>text</i> more text</div>enter <br> End text";

            HtmlDocument doc = new HtmlDocument(input);
            
            doc.Parse();
            
            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }
        [Test]
        public void MixedHtmlStructureWithComments()
        {
            string input =
                "<!-- comment -->Text0<div id=\"divider\" class=\"big and small\">Text1<b>Text2<!-- comment --></b>Text3<br/><i>Text4</i><img>Text5</div>Text6Text7<br>Text8";


            HtmlDocument doc = new HtmlDocument(input);     
            
            doc.Parse();

            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }
        [Test]
        public void MixedHtmlStructureWithScript()
        {
            string input =
                "<!doctype kokook>Text0<div id=\"divider\" class=\"big and small\">Text1<b><script>hlkbhb^%$#jvk{}>.<></script>Text2</b>Text3<br/><i>Text4</i><img>Text5</div>Text6Text7<br>Text8";


            HtmlDocument doc = new HtmlDocument(input);            
            
            doc.Parse();

            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }
        [Test]
        public void HtmlStructureWithOptionalClosingTag()
        {
            string input = "<div>sometext<p>paragraph</div>";

            HtmlDocument doc = new HtmlDocument(input);
            
            doc.Parse();
           
            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }

        [Test]
        public void HtmlStructureWithOptionalClosingTag2()
        {
            string input = "<div>sometext<p>paragraph<pre>pretext</pre>text</div>";

            HtmlDocument doc = new HtmlDocument(input);
            
            doc.Parse();
           
            Assert.AreEqual(input, doc.RootNode.InnerHtml);
        }
    }
}
