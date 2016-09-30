using CSharp_HtmlParser_Library;
using NUnit.Framework;

namespace CSharp_HtmlParser_Tests
{
    public class TextFormatterTests
    {     
        [Test]
        public void GetCurrentPositionCharacterAndForwardPosition()
        {
            string input  = "input";
            TextFormatter formatter = new TextFormatter(input);

            Assert.AreEqual('i',formatter.GetCurrentPositionCharacter());
            Assert.AreEqual(0,formatter.Position);
            formatter.ForwardPosition();

            Assert.AreEqual('n',formatter.GetCurrentPositionCharacter());
            Assert.AreEqual(1,formatter.Position);
            formatter.ForwardPosition();

            Assert.AreEqual('p',formatter.GetCurrentPositionCharacter());
            Assert.AreEqual(2,formatter.Position);
            formatter.ForwardPosition();

            Assert.AreEqual('u',formatter.GetCurrentPositionCharacter());
            Assert.AreEqual(3,formatter.Position);
            formatter.ForwardPosition();

            Assert.AreEqual('t',formatter.GetCurrentPositionCharacter());
            Assert.AreEqual(4,formatter.Position);
        }   
    }
}