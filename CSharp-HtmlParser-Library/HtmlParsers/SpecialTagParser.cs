using CSharp_HtmlParser_Library.HtmlDocumentStructure;

namespace CSharp_HtmlParser_Library.HtmlParsers
{
    public class SpecialTagParser : ITagParser
    {
        
        public HtmlDocumentNode ParsedNode {get;set;}

        private TextFormatter _source;
        private SpecialTagParserConfiguration _configuration;
        private string _name; 

        public SpecialTagParser(string name, TextFormatter source, SpecialTagParserConfiguration configuration)
        {
            _name = name;
            _source = source;
            _configuration = configuration;
        }

        public bool CanParse()
        {
            return _source.IsTextOnCurrentPosition(_configuration.TagOpener, _configuration.CaseSensitive);
        }

        public void Parse()
        {
            ParsedNode = new HtmlDocumentNode
            {
                Name = _name,
                Position = _source.Position,
                Line = _source.Line
            };

            ParsedNode.Flags.Add(Flags.SpecialTag);

            AddAndSkipTagOpener();          
            AddAndSkipTagContent();
            AddAndSkipTagCloser();
        }

        public void AddAndSkipTagOpener()
        {
            ParsedNode.OuterHtml = _configuration.TagOpener;
            _source.SkipText(_configuration.TagOpener, _configuration.CaseSensitive);
        }

        public void AddAndSkipTagCloser()
        {
            ParsedNode.OuterHtml += _configuration.TagCloser;
            _source.SkipText(_configuration.TagCloser, _configuration.CaseSensitive);
        }

        public void AddAndSkipTagContent()
        {
            ParsedNode.OuterHtml += _source.GetTextFromCurrentPositionToAnyStopString(_configuration.TagCloser);
        }
    }

    public class SpecialTagParserConfiguration
    {
        public string TagOpener {get;set;}
        public string TagCloser {get;set;}

        public bool CaseSensitive {get;set;}

        public SpecialTagParserConfiguration(string tagOpener, string tagCloser, bool caseSensitive)
        {
            TagOpener = tagOpener;
            TagCloser = tagCloser;
            CaseSensitive = caseSensitive;
        }
    }
}
