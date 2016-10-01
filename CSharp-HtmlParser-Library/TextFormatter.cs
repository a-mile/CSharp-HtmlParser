using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharp_HtmlParser_Library
{
    public class TextFormatter
    {
        public string Text { get;}
        public int Position { get; private set; } 
        public int Line { get; private set; } = 1;

        private readonly IEnumerable<char> _whiteSpaces = new List<char>()
        {
            ' ','\n','\r','\t'
        };

        public TextFormatter(string text)
        {
            Text = text;      
        }

        public void ForwardPosition()
        {            
            if(IsNotDocumentEnd())
            {                
                if(Text[Position] == '\n')
                {                    
                    Line++;
                }
                Position++;     
            }                  
        }

        public void ForwardPosition(int length)
        {
            while (length > 0)
            {
                ForwardPosition();
                length--;
            }
        }        

        public void ResetPosition()
        {
            Position = 0;
        }

        public char GetCurrentPositionCharacter()
        {
            if (IsNotDocumentEnd())
            {
                return Text[Position];
            }
            return '\0';
        }

        public bool IsDocumentEnd()
        {
            return Position == Text.Length;
        }

        public bool IsNotDocumentEnd()
        {
            return !IsDocumentEnd();
        }

        public void SkipText(string textToSkip, bool caseSensitive)
        {
            try
            {
                string currentText = Text.Substring(Position, textToSkip.Length);

                if(caseSensitive)
                {
                    if (currentText == textToSkip)
                        ForwardPosition(textToSkip.Length);
                }
                else
                {
                    if (currentText.ToLower() == textToSkip.ToLower())
                        ForwardPosition(textToSkip.Length);
                }
             }
            catch (Exception)
            {
                // ignored
            }
        }

        public bool IsTextOnCurrentPosition(string textToCheck, bool caseSensitive)
        {
            try
            {
                string currentText = Text.Substring(Position, textToCheck.Length);
                //currentText = Regex.Replace(currentText, @"\t|\n|\r", " ");
                if(caseSensitive)
                {
                    return currentText == textToCheck;
                }

                return currentText.ToLower() == textToCheck.ToLower();                 
            }
            catch (Exception)
            {
                return false;
            }
        }           

        public string GetTextFromCurrentPositionToAnyStopString(params string[] stopStrings)
        {
            StringBuilder value = new StringBuilder();

            bool stop = false;

            if(IsNotDocumentEnd())
            {
                value.Append(GetCurrentPositionCharacter());
                ForwardPosition();
            }

            while (IsNotDocumentEnd())
            {
                foreach (var stopString in stopStrings)
                {
                    if (IsTextOnCurrentPosition(stopString, false))
                    {
                        stop = true;
                        break;
                    }                    
                }

                if(stop)
                    break;

                value.Append(GetCurrentPositionCharacter());
                ForwardPosition();
            }            

            return value.ToString();
        }

        public void SkipWhiteSpaces()
        {
            while (IsNotDocumentEnd() && IsWhiteSpace())
            {
                ForwardPosition();
            }
        }

        private bool IsWhiteSpace()
        {
            return _whiteSpaces.Contains(GetCurrentPositionCharacter());
        }
    }
}
