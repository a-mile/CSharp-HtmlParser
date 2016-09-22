using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharp_HtmlParser_Library.HtmlDocumentStructure;

namespace CSharp_HtmlParser_Library.HtmlParsers
{
    public class NormalTagParser : ITagParser
    {
        public HtmlDocumentNode ParsedNode {get;set;}

        private HtmlAttribute _currentAttribute;
        private char _quoteMarkCharacter;
        private TextFormatter _source;
        private readonly Regex _selfClosingHtmlTags = new Regex(@"\b(area|base|br|col|command|embed|hr|img|input|keygen|link|meta|param|source|track|wbr)\b");

        private IList<string> tagToIgnoreNames = new List<string>()
        {
            "script"
        };

        public NormalTagParser(TextFormatter source)
        {
            _source = source;
        }

        public bool CanParse()
        {
            foreach (var name in tagToIgnoreNames)
            {
                try
                {
                    if (_source.Text.Substring(_source.Position + 1, name.Length).ToLower() == name.ToLower())
                        return false;
                }
                catch { }
            }

            try
            {
                string twoFirstSigns = _source.Text.Substring(_source.Position, 2);
                return Regex.IsMatch(twoFirstSigns, @"(<[A-Za-z])|(</)");
            }
            catch
            {
                return false;
            }
        }

        public void Parse()
        {                        
            ParsedNode = new HtmlDocumentNode()
            {
                Position = _source.Position,
                Line = _source.Line
            };

            SkipTagOpener();            

            if (IsFrontslash())
            {                
                ParseEndTag();
            }
            else
            {                
                ParseStartTag();
            }            
        }

        private void ParseStartTag()
        {
            ParsedNode.Flags.Add(Flags.NormalTag);
            SetTagName();
            SkipSpaces();

            if (IsNotTagCloser())
                ParseTagAttributes();
            if (IsFrontslash())
            {
                ParsedNode.Flags.Add(Flags.ContainsFrontslashAtTheEnd);
                ParsedNode.Flags.Add(Flags.SelfClosing);                
                SkipFrontslash();
            }
            else
            {
                if (_selfClosingHtmlTags.IsMatch(ParsedNode.Name.ToLower()))
                {
                    ParsedNode.Flags.Add(Flags.SelfClosing);      
                }                
            }
            SkipTagCloser();          
        }

        private void ParseEndTag()
        {        
            ParsedNode.Flags.Add(Flags.NormalTag);
            ParsedNode.Flags.Add(Flags.EndTag);

            SkipFrontslash();
            SkipSpaces();
            SetTagName();            
            SkipTagCloser();    
            SetEndTagOuterHtml();

        } 

        private void SetEndTagOuterHtml()
        {
            ParsedNode.OuterHtml = "</"+ParsedNode.Name+">";
        }       

        private void SetTagName()
        {
            ParsedNode.Name = _source.GetTextFromCurrentPositionToAnyStopString(" ", ">","/>").ToLower();
        }                                    

        private void ParseTagAttributes()
        {
            while (IsNotTagCloser())
            {
                ParseTagAttribute();
                AddCurrentAttributeToTag();
                SkipSpaces();
            }
        }

        private void ParseTagAttribute()
        {
            _currentAttribute = new HtmlAttribute();

            SetAttributeName();
            SkipSpaces();

            if (IsEqualCharacter())
            {
                SkipEqualCharacter();
                SkipSpaces();
                SetQuoteMarkCharacter();

                if (IsQuoteMarkCharacterNotEmpty())
                {
                    SkipQuoteMarkCharacter();

                    if(_source.GetCurrentPositionCharacter() == _quoteMarkCharacter)
                        _currentAttribute.Value = "";
                    else
                    {
                        SetAttributeValueWithQuote();
                    }                    

                    SkipQuoteMarkCharacter();
                }
                else
                {
                     SetAttributeValueWithoutQuote();
                }
            }
        }

        private bool IsQuoteMarkCharacterNotEmpty()
        {
            return _quoteMarkCharacter != '\0';
        }

        private void SetAttributeName()
        {            
            _currentAttribute.Name = _source.GetTextFromCurrentPositionToAnyStopString("=", " ", ">").ToLower();
        }

        private void SetQuoteMarkCharacter()
        {
            _quoteMarkCharacter = '\0';
            if (_source.GetCurrentPositionCharacter() == '\"')            
                _quoteMarkCharacter = '\"';                            
            if (_source.GetCurrentPositionCharacter() == '\'')                    
                _quoteMarkCharacter = '\'';
            _currentAttribute.QuoteCharacter = _quoteMarkCharacter;
        }        
                
        private void SetAttributeValueWithQuote()
        {
            _currentAttribute.Value =
                _source.GetTextFromCurrentPositionToAnyStopString(_quoteMarkCharacter.ToString());
        }

        private void SetAttributeValueWithoutQuote()
        {            
            _currentAttribute.Value = _source.GetTextFromCurrentPositionToAnyStopString(" ", ">").ToLower();
        }

        private void AddCurrentAttributeToTag()
        {
            ParsedNode.Attributes.Add(_currentAttribute);
        }

        private bool IsSpace()
        {
            return _source.IsTextOnCurrentPosition(" ",false);
        }       

        private bool IsEqualCharacter()
        {
            return _source.IsTextOnCurrentPosition("=",false);
        }

        private bool IsFrontslash()
        {
            return _source.IsTextOnCurrentPosition("/",false);
        }        

        private bool IsTagCloser()
        {
            return _source.IsTextOnCurrentPosition(">",false) || _source.IsTextOnCurrentPosition("/>",false);
        }        

        private bool IsNotTagCloser()
        {
            return !IsTagCloser();
        }

        private void SkipSpaces()
        {
            while (IsSpace())
            {
                _source.ForwardPosition();
            }
        }

        private void SkipFrontslash()
        {
            _source.SkipText("/",false);
        }

        private void SkipTagOpener()
        {
            _source.SkipText("<",false);
        }

        private void SkipTagCloser()
        {
            _source.SkipText(">",false);
        }

        private void SkipQuoteMarkCharacter()
        {
            _source.SkipText(_quoteMarkCharacter.ToString(),false);
        }

        private void SkipEqualCharacter()
        {
            _source.SkipText("=",false);
        }  
    }    
}
