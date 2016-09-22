# HtmlParser
Simple parser fully compatible with HTML4.01, HTML5, XHTML and XML

##Usage
```
string source = string.Empty;

using (var client = new HttpClient())
{
  var response = client.GetByteArrayAsync("https://github.com/a-mile/HtmlParser").Result;                     
  var responseString = Encoding.UTF8.GetString(response, 0, response.Length - 1);
  source = responseString;                
}

HTMLDocument doc = new HTMLDocument(source);
doc.Parse();

List<HTMLDocumentNode> allNodes = doc.RootNode.Descendants;
```
