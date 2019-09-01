using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvikk
{
    public enum TokenType
    {
        Unknown,
        EOL,
        Semicolon,
        Definition,
        Lambda,
        Identifier,
        Number
    }

    public class Token
    {
        public TokenType Type;

        public double DoubleValue;
        public string StringValue;

        public Token (TokenType type)
        {
            Type = type;
        }

        public Token (double value)
        {
            DoubleValue = value;
            Type = TokenType.Number;
        }

        public Token (string value, TokenType type)
        {
            StringValue = value;
            Type = type;
        }
    }

    public static class Grammar
    {
        public static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"def", TokenType.Definition },
            {"lambda", TokenType.Lambda }
        };

        public static readonly Dictionary<string, int> Precedences = new Dictionary<string, int>
        {
            {"<", 10 },
            {"+", 20 },
            {"-", 20 },
            {"*", 40 }
        };

        public static TokenType GetKeywordType (string word)
        {
            TokenType type;
            if( Keywords.TryGetValue(word, out type) )   // get the associated type
                return type;
            return TokenType.Identifier;    // if nothing matches, this is probably an identifier
        }

        public static int GetPrecedence (string binop)
        {
            int prec = -1;
            if (binop == null) return prec;

            if (Precedences.TryGetValue(binop, out prec))   // get the precedence of the binary operator
                return prec;
            return -1;    // operator is either not part of the table or is not a char
        }
    }
}
