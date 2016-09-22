using CSharp_HtmlParser_Library.HtmlDocumentStructure;

namespace CSharp_HtmlParser_Library.HtmlParsers
{
    public interface ITagParser
    {
        HtmlDocumentNode ParsedNode {get; set;}
        bool CanParse();
        void Parse();
    }
}