using CSharp_HtmlParser_Library.HtmlDocumentStructure;

namespace CSharp_HtmlParser_Library
{
    public class HtmlGenerator
    {
        public string NodeHtml;
        private readonly HtmlDocumentNode _node;
        private HtmlAttribute _currentAttribute;        
        private string _tagHtml;       

        public HtmlGenerator(HtmlDocumentNode node)
        {
            _node = node;
        }

        public void GenerateHtml()
        {
            NodeHtml = string.Empty;           
            GenerateNode();                      
        }

        public void GenerateTag()
        {
            NodeHtml = string.Empty;           
            GenerateOpeningTag();                  
        }
        
        private void GenerateNode()
        {
            GenerateOpeningTag();

            foreach (var childNode in _node.ChildNodes)
            {                
                NodeHtml += childNode.OuterHtml;
            }
            
            if(_node.Flags.Contains(Flags.ContainsClosingTag) && !_node.Flags.Contains(Flags.ClosingTagIncudedInNodeTree))
                GenerateClosingTag();
        }        

        private void GenerateOpeningTag()
        {
            _tagHtml = string.Empty;

            AddNodeOpenerToTag();
            AddNodeNameToTag();

            if (_node.Attributes.Count > 0)
            {
                AddSpaceToTag();
                AddAttributesToTag();
            }

            if(_node.Flags.Contains(Flags.ContainsFrontslashAtTheEnd))
                AddFrontslashToTag();
            AddNodeCloserToTag();

            NodeHtml += _tagHtml;
        }

        private void GenerateClosingTag()
        {
            _tagHtml = string.Empty;

            AddNodeOpenerToTag();
            AddFrontslashToTag();
            AddNodeNameToTag();
            AddNodeCloserToTag();

            NodeHtml += _tagHtml;
        }                        

        private void AddNodeOpenerToTag()
        {
            _tagHtml += '<';
        }

        private void AddFrontslashToTag()
        {
            _tagHtml += '/';
        }

        private void AddNodeCloserToTag()
        {
            _tagHtml += '>';
        }
        
        private void AddNodeNameToTag()
        {
            _tagHtml += _node.Name;
        }

        private void AddSpaceToTag()
        {
            _tagHtml += ' ';
        }

        private void AddAttributesToTag()
        {
            foreach (var attribute in _node.Attributes)
            {
                _currentAttribute = attribute;

                AddAttributeNameToTag();
                if (_currentAttribute.Value != null)
                {                    
                    AddEqualAndQuoteOpenerCharacterToTag();
                    AddAttributeValueToTag();
                    AddQuoteCloserToTag();
                }
                AddSpaceToTag();
            }

            RemoveLastSpace();
        }

        private void AddAttributeNameToTag()
        {
            _tagHtml += _currentAttribute.Name;
        }

        private void AddEqualAndQuoteOpenerCharacterToTag()
        {
            if (_currentAttribute.QuoteCharacter != '\0')
            {
                _tagHtml += "=" + _currentAttribute.QuoteCharacter;
            }
            else
            {
                _tagHtml += "=";
            }
        }

        private void AddAttributeValueToTag()
        {            
            _tagHtml += _currentAttribute.Value;
        }

        private void RemoveLastSpace()
        {
            _tagHtml = _tagHtml.Substring(0, _tagHtml.Length - 1);
        }

        private void AddQuoteCloserToTag()
        {
            if(_currentAttribute.QuoteCharacter != '\0')
                _tagHtml += _currentAttribute.QuoteCharacter;
        }
    }
}
