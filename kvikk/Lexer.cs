using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvikk
{
    public static class Lexer
    {
        private static string input = "";
        private static int index = 0;
        private static char curChar;
        private static char getChar () { char rtn = index < input.Length ? input[index] : '\r'; index++; return curChar = rtn; }

        public static void SetInput (string Input)
        {
            input = Input;
            index = 0;
            getChar();
        }

        public static Token GetToken ()
        {
            if (curChar == '\r')                    // reached end of line, time to get off
                return new Token(TokenType.EOL);

            while (char.IsWhiteSpace(curChar))      // skip white spaces, let them breathe
                getChar();

            if(char.IsLetter(curChar))              // [a-zA-Z][a-zA-Z0-9]*     catch keywords or identifiers
            {
                string word = curChar.ToString();
                while (char.IsLetterOrDigit(getChar()))
                    word += curChar;

                return new Token(word, Grammar.GetKeywordType(word));
            }

            if(curChar == '.' || char.IsDigit(curChar)) // parse numbers, NAIVELY
            {
                string val = "";
                while (curChar == '.' || char.IsDigit(curChar))
                {
                    val += curChar;
                    getChar();
                }

                double number = 0;
                double.TryParse(val, out number);
                return new Token(number);
            }

            if(curChar == '/')                      // handle comments with //
            {
                if (getChar() == '/') return new Token(TokenType.EOL);
            }

            if (curChar == ';')
            {
                getChar();  // eat the character
                return new Token(TokenType.Semicolon);
            }

            string c = curChar.ToString();
            getChar();  // eat the unknown char

            return new Token(c, TokenType.Unknown);    // unkown character for now, let the parser take care of it
        }
    }
}
