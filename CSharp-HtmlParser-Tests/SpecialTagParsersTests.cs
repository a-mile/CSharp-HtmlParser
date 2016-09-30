using CSharp_HtmlParser_Library;
using CSharp_HtmlParser_Library.HtmlDocumentStructure;
using CSharp_HtmlParser_Library.HtmlParsers;
using NUnit.Framework;

namespace CSharp_HtmlParser_Tests
{
    public class SpecialTagParserTests
    {                                                                                                                
        [Test]
        public void Comment()
        {
            string input = "<!--Comment Text-->";
            TextFormatter formatter = new TextFormatter(input);

            
            SpecialTagParser parser = new SpecialTagParser("#comment", formatter, new SpecialTagParserConfiguration("<!--","-->",false));
            if(parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("#comment",node.Name);
            Assert.AreEqual("Comment Text",node.OwnText);
        } 
        [Test]
        public void ConditionalComment()
        {
            string input = "<!--[if IE6]>Conditional Comment Text<!--Nested Comment--><![endif]-->";
            TextFormatter formatter = new TextFormatter(input);

            SpecialTagParser parser = new SpecialTagParser("#conditionalcomment", formatter, new SpecialTagParserConfiguration("<!--[if","<![endif]-->",false));
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("#conditionalcomment", node.Name);
            Assert.AreEqual("Conditional Comment Text<!--Nested Comment-->", node.OwnText);                       
        }
        [Test]
        public void Conditional()
        {
            string input = "<![if IE6]>Conditional Comment Text<!--Nested Comment--><![endif]>";
            TextFormatter formatter = new TextFormatter(input);

            SpecialTagParser parser = new SpecialTagParser("#conditional", formatter, new SpecialTagParserConfiguration("<![if","<![endif]>",false));
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("#conditional", node.Name);
            Assert.AreEqual("Conditional Comment Text<!--Nested Comment-->", node.OwnText);
        }
        [Test]
        public void Doctype()
        {
            string input = "<!doctype doctype content>";
            TextFormatter formatter = new TextFormatter(input);

            SpecialTagParser parser = new SpecialTagParser("#doctype", formatter, new SpecialTagParserConfiguration("<!doctype ",">",false));
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("#doctype", node.Name);
            Assert.AreEqual("doctype content", node.OwnText);
        }
        [Test]
        public void JsComment()
        {
            string input = "/*Comment Text*/";
            TextFormatter formatter = new TextFormatter(input);

            SpecialTagParser parser = new SpecialTagParser("#jscomment", formatter, new SpecialTagParserConfiguration("/*","*/",false));
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("#jscomment", node.Name);
            Assert.AreEqual("Comment Text", node.OwnText);
        }
        [Test]
        public void XmlProcessingInstruction()
        {
            string input = "<? Instruction content ?>";
            TextFormatter formatter = new TextFormatter(input);

            SpecialTagParser parser = new SpecialTagParser("#xmlprocessinginstruction", formatter, new SpecialTagParserConfiguration("<?","?>",false));
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("#xmlprocessinginstruction", node.Name);
            Assert.AreEqual(" Instruction content ", node.OwnText);
        }
        [Test]
        public void Script()
        {
            string input = "<script>script code</script>";
            TextFormatter formatter = new TextFormatter(input);

            SpecialTagParser parser = new SpecialTagParser("#script", formatter, new SpecialTagParserConfiguration("<script","</script>",false));
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("#script", node.Name);
            Assert.AreEqual("script code", node.OwnText);
        }
    }
}