using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharp_HtmlParser_Library.HtmlDocumentStructure;
using CSharp_HtmlParser_Library.HtmlParsers;

namespace CSharp_HtmlParser_Library
{
    public class HtmlParser
    {        
        public HtmlParserConfiguration Configuration {get;set;}
        public HtmlDocumentNode RootNode { get; private set; }

        private HtmlDocumentNode _currentNode;
        private HtmlDocumentNode _currentParent;
        private ITagParser _currentParser;
        private readonly string _documentHtml;
        private readonly IDictionary<string, Regex> _tagCloseOpeningingTagsNames = new Dictionary<string, Regex>
        {
            {"head", new Regex(@"\b(body)\b")},
            {
                "p", new Regex(
                    @"(\baddress|article|aside|blockquote|div|dl|fieldset|footer|form|h1|h2|h3|h4|h5|h6|header|hgroup|hr|main|nav|ol|p|pre|section|table|ul)\b")
            },
            {"dt", new Regex(@"\b(dt|dd)\b")},
            {"dd", new Regex(@"\b(dd|dt)\b")},
            {"li", new Regex(@"\b(li)\b")},
            {"option", new Regex(@"\b(option|optgroup)\b")},
            {"optgroup", new Regex(@"\b(optgroup)\b")},
            {"thead", new Regex(@"\b(tbody|tfoot)\b")},
            {"tfoot", new Regex(@"\b(tbody)\b")},
            {"tbody", new Regex(@"\b(tbody|tfoot)\b")},
            {"th", new Regex(@"\b(td|th)\b")},
            {"td", new Regex(@"\b(td|th)\b")},
            {"tr", new Regex(@"\b(tr)\b")},
            {"colgroup", new Regex(@".*")}
        };
        private IEnumerable<SpecialTagParser> _specialTagParsers;
        private NormalTagParser _normalTagParser;
        private TextFormatter _textFormatter;               

        public HtmlParser(string documentHtml)
        {
            _documentHtml = documentHtml;
            Configuration = new HtmlParserConfiguration();     
        }

        public HtmlParser(string documentHtml, HtmlParserConfiguration configuration): this(documentHtml)
        {
            Configuration = configuration;
        }

        public void ParseHtml()
        {
            Initialize();             
            _textFormatter.SkipWhiteSpaces();

            if (_textFormatter.IsNotDocumentEnd())
                ParseNodes();
        }

        private void Initialize()
        {
            _textFormatter = new TextFormatter(_documentHtml);
            RootNode = new HtmlDocumentNode() { Name = "#root" };
            _currentParent = RootNode;
            _normalTagParser = new NormalTagParser(_textFormatter);
            _specialTagParsers = new List<SpecialTagParser>()
            {
                new SpecialTagParser("#doctype", _textFormatter, new SpecialTagParserConfiguration("<!doctype",">",false)),
                new SpecialTagParser("#conditional", _textFormatter, new SpecialTagParserConfiguration("<![if","<![endif]>",false)),
                new SpecialTagParser("#conditionalcomment", _textFormatter, new SpecialTagParserConfiguration("<!--[if","<![endif]-->",false)),
                new SpecialTagParser("#comment", _textFormatter, new SpecialTagParserConfiguration("<!--","-->",false)),
                new SpecialTagParser("#jscomment", _textFormatter, new SpecialTagParserConfiguration("/*","*/",false)),
                new SpecialTagParser("#xmlprocessinginstruction", _textFormatter, new SpecialTagParserConfiguration("<?","?>",false)),
                new SpecialTagParser("script", _textFormatter, new SpecialTagParserConfiguration("<script","</script>",false))
            };
        }

        private void ParseNodes()
        {
            while (_textFormatter.IsNotDocumentEnd())
            {               
                if(_normalTagParser.CanParse())
                {
                    _currentParser = _normalTagParser;
                    CreateNewNode();
                }
                else
                {
                    foreach (var parser in _specialTagParsers)
                    {                           
                        if (parser.CanParse())
                        {
                            _currentParser = parser;
                            CreateNewNode();
                            break;
                        }
                    }
                    ParseAsText();                                        
                }

                _textFormatter.SkipWhiteSpaces();
            }
        }

        private void ParseAsText()
        {
            int position = _textFormatter.Position;
            int line = _textFormatter.Line;
            string text = _textFormatter.GetTextFromCurrentPositionToAnyStopString("<", "/*");                

            _currentNode = new HtmlDocumentNode()
            {
                Name = "#text",
                Position = position,
                Line = line,
                OwnText = text, 
                OuterHtml = text,
                ParentNode = _currentParent         
            };
            _currentNode.Flags.Add(Flags.Text);

            AddCurrentNodeToCurrentParent();                       
        }        

        private void CreateNewNode()
        {
            _currentParser.Parse();
            _currentNode = _currentParser.ParsedNode;
            _currentNode.ParentNode = _currentParent;            
            
            if (IsEndTag())
            {
                if (EndTagMatchesCurrentParent())
                {
                    _currentParent.Flags.Add(Flags.ContainsClosingTag);

                    if(Configuration.IncludeClosingTagsInNodeTree)
                        _currentParent.Flags.Add(Flags.ClosingTagIncudedInNodeTree);

                    ChooseCurrentParentParentAsCurrentParent();
                }
                else
                {
                    HtmlDocumentNode parent = _currentParent;

                    while (EndTagDoesNotMatchCurrentParent())
                    {                     
                        ChooseCurrentParentParentAsCurrentParent();

                        if (EndTagDoesNotMatchAnyParent()) 
                        {
                            _currentParent = parent; 
                            //Can do something with this                           
                            return;
                        }
                    }

                    _currentParent.Flags.Add(Flags.ContainsClosingTag);

                    if(Configuration.IncludeClosingTagsInNodeTree)
                        _currentParent.Flags.Add(Flags.ClosingTagIncudedInNodeTree);
                        
                    ChooseCurrentParentParentAsCurrentParent();
                }
                if(Configuration.IncludeClosingTagsInNodeTree)
                    AddCurrentNodeToCurrentParent();
            }
            else
            {
                if (CurrentParentCanClosedByOpeningTag() && CurrentNodeCanCloseCurrentParent())
                {                    
                        ChooseCurrentParentParentAsCurrentParent();
                        _currentNode.ParentNode = _currentParent;                    
                }

                AddCurrentNodeToCurrentParent();

                if (CurrentNodeCanHaveChildren())                
                    ChooseCurrentNodeAsCurrentParent();                
            }
        }

        private bool IsEndTag()
        {
            return _currentNode.Flags.Contains(Flags.EndTag);
        }

        private bool EndTagMatchesCurrentParent()
        {
            return _currentParent.Name == _currentNode.Name;
        }

        private bool EndTagDoesNotMatchCurrentParent()
        {
            return !EndTagMatchesCurrentParent();
        }

        private bool EndTagDoesNotMatchAnyParent()
        {
            return _currentParent == null;
        }

        private bool CurrentParentCanClosedByOpeningTag()
        {
            return _tagCloseOpeningingTagsNames.ContainsKey(_currentParent.Name.ToLower());
        }

        private bool CurrentNodeCanCloseCurrentParent()
        {
            return _tagCloseOpeningingTagsNames[_currentParent.Name.ToLower()].IsMatch(_currentNode.Name.ToLower());
        }

        private bool CurrentNodeCanHaveChildren()
        {
            return _currentNode.Flags.Contains(Flags.NormalTag) && !_currentNode.Flags.Contains(Flags.SelfClosing);
        }

        private void AddCurrentNodeToCurrentParent()
        {            
            _currentParent.ChildNodes.Add(_currentNode);
        }                                        

        private void ChooseCurrentParentParentAsCurrentParent()
        {
            _currentParent = _currentParent.ParentNode;
        }        

        private void ChooseCurrentNodeAsCurrentParent()
        {
            _currentParent = _currentNode;
        }               
    }
}
