namespace CSharp_HtmlParser_Library
{
    public class HtmlParserConfiguration
    {   
        public bool IncludeClosingTagsInNodeTree {get;set;}

        public HtmlParserConfiguration()
        {
            IncludeClosingTagsInNodeTree = false;
        }
    }
}