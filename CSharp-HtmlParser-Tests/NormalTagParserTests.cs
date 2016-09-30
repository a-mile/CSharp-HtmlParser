using CSharp_HtmlParser_Library;
using CSharp_HtmlParser_Library.HtmlDocumentStructure;
using CSharp_HtmlParser_Library.HtmlParsers;
using NUnit.Framework;


namespace CSharp_HtmlParser_Tests
{
    public class NormalTagParserTests
    {        
        [Test]
        public void NormalTag()
        {
            string input = "<name>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();
            
            HtmlDocumentNode node = parser.ParsedNode;
            Assert.AreEqual("name", node.Name);
            Assert.AreEqual(0, node.Attributes.Count);
            Assert.AreEqual(1, node.Flags.Count);
            Assert.AreEqual(true,node.Flags.Contains(Flags.NormalTag));        
        }
        [Test]
        public void EndTag()
        {
            string input = "</end>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("end", node.Name);
            Assert.AreEqual(0, node.Attributes.Count);
            Assert.AreEqual(2, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.EndTag));
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
        }
        [Test]
        public void SelfClosingXHtmlTag()
        {
            string input = "<name/>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("name", node.Name);
            Assert.AreEqual(0, node.Attributes.Count);
            Assert.AreEqual(3, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
            Assert.AreEqual(true,node.Flags.Contains(Flags.ContainsFrontslashAtTheEnd));
            Assert.AreEqual(true, node.Flags.Contains(Flags.SelfClosing));
        }
        [Test]
        public void SelfClosingHtmlTag()
        {
            string input = "<meta>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("meta", node.Name);
            Assert.AreEqual(0, node.Attributes.Count);
            Assert.AreEqual(2, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
            Assert.AreEqual(true, node.Flags.Contains(Flags.SelfClosing));
        }
        [Test]
        public void AttributesWithoutValues()
        {
            string input = "<name attr1 attr2>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("name", node.Name);
            Assert.AreEqual(2,node.Attributes.Count);
            Assert.AreEqual("attr1", node.Attributes[0].Name);
            Assert.AreEqual("attr2", node.Attributes[1].Name);
            Assert.AreEqual(null,node.Attributes[0].Value);
            Assert.AreEqual(null, node.Attributes[1].Value);
            Assert.AreEqual(1, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
        }
        [Test]
        public void AttributesWithoutQuotes()
        {
            string input = "<name attr=value attr2=value2>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("name", node.Name);
            Assert.AreEqual(2, node.Attributes.Count);
            Assert.AreEqual("attr", node.Attributes[0].Name);
            Assert.AreEqual("attr2", node.Attributes[1].Name);
            Assert.AreEqual("value", node.Attributes[0].Value);           
            Assert.AreEqual("value2", node.Attributes[1].Value);
            Assert.AreEqual(1, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
        }
        [Test]
        public void AttributesWithQuotesAndValues()
        {
            string input = "<name attr=\"value\" attr2=\'value2 \"quote\"\'>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("name", node.Name);
            Assert.AreEqual(2, node.Attributes.Count);
            Assert.AreEqual("attr", node.Attributes[0].Name);
            Assert.AreEqual("attr2", node.Attributes[1].Name);
            Assert.AreEqual("value", node.Attributes[0].Value);
            Assert.AreEqual("value2 \"quote\"", node.Attributes[1].Value);
            Assert.AreEqual(1, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
        }
        [Test]
        public void AttributesWithQuotesWithoutValues()
        {
            string input = "<name attr=\"\" attr2=\'\'>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("name", node.Name);
            Assert.AreEqual(2, node.Attributes.Count);
            Assert.AreEqual("attr", node.Attributes[0].Name);
            Assert.AreEqual("attr2", node.Attributes[1].Name);
            Assert.AreEqual("", node.Attributes[0].Value);
            Assert.AreEqual("", node.Attributes[1].Value);
            Assert.AreEqual(1, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
        }
        [Test]
        public void MixedAttributes()
        {
            string input = "<name attr1=\"value\" attr2 attr3=\'\'>";

            TextFormatter formatter = new TextFormatter(input);

            NormalTagParser parser = new NormalTagParser(formatter);
            if (parser.CanParse())
                parser.Parse();

            HtmlDocumentNode node = parser.ParsedNode;

            Assert.AreEqual("name", node.Name);
            Assert.AreEqual(3, node.Attributes.Count);
            Assert.AreEqual("attr1", node.Attributes[0].Name);
            Assert.AreEqual("attr2", node.Attributes[1].Name);
            Assert.AreEqual("attr3", node.Attributes[2].Name);
            Assert.AreEqual("value", node.Attributes[0].Value);
            Assert.AreEqual(null, node.Attributes[1].Value);
            Assert.AreEqual("", node.Attributes[2].Value);
            Assert.AreEqual(1, node.Flags.Count);
            Assert.AreEqual(true, node.Flags.Contains(Flags.NormalTag));
        }
    }
}
