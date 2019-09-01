using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvikk
{
    public static class Parser
    {
        private static Token CurrentToken;
        private static Token NextToken () { return CurrentToken = Lexer.GetToken(); }

        public static void Parse (string input)
        {
            Lexer.SetInput(input);  // initialize lexer
            NextToken();    // prime the parser

            while(CurrentToken.Type != TokenType.EOL)
            {
                switch(CurrentToken.Type)
                {
                    case TokenType.Semicolon:
                        NextToken();
                        break;
                    case TokenType.Definition:
                        if(ParseDefinition() != null)
                        {
                            Console.WriteLine("Parsed a function definition");
                        }
                        else
                        {
                            NextToken();    // error recovery
                        }
                        break;
                    case TokenType.Unknown:
                        Console.WriteLine("Unknown or unexpected token '{0}'", CurrentToken.StringValue);
                        NextToken();
                        break;
                    default:
                        if(ParseTopLevelExpression() != null)
                        {
                            Console.WriteLine("Parsed anonymous expression");
                        }
                        else
                        {
                            NextToken();    // error recovery
                        }
                        break;
                }
            }
        }

        private static ASTExpression ParseExpression()
        {
            ASTExpression lhs = ParsePrimary();
            if (lhs == null) return null;

            return ParseRHSBinaryOperator(0, lhs);
        }

        private static ASTExpression ParsePrimary ()
        {
            switch(CurrentToken.Type)
            {
                default:
                    if (CurrentToken.StringValue == "(")
                        return ParseParenExpression();
                    else
                        return LogError("Unexpected token '" + CurrentToken.StringValue + "'");
                case TokenType.Identifier:
                    return ParseIdentifier();
                case TokenType.Number:
                    return ParseNumber();
            }
        }

        private static ASTExpression ParseNumber ()
        {
            ASTExpressionNumber result = new ASTExpressionNumber(CurrentToken.DoubleValue);
            NextToken();    // eat the value
            return result;
        }
        
        private static ASTExpression ParseParenExpression ()
        {
            NextToken();    // eat first '('
            ASTExpression exp = ParseExpression();
            if (exp == null)
                return null;

            if (CurrentToken.StringValue != ")")
                return LogError("Expected ')'");

            NextToken();    // eat ')'
            return exp;
        }

        private static ASTExpression ParseIdentifier ()
        {
            string name = CurrentToken.StringValue; // store the identifier name
            NextToken();    // eat the identifier

            if (CurrentToken.StringValue != "(")    // simple variable reference
                return new ASTExpressionVariable(name);

            NextToken();    // eat the left parens
            List<ASTExpression> args = new List<ASTExpression>();
            if(CurrentToken.StringValue != ")") // we got arguments
            {
                while(true)
                {
                    ASTExpression arg;
                    if ((arg = ParseExpression()) != null)  // got a successfully parsed expr
                        args.Add(arg);
                    else
                        return null;    // got nothing

                    if (CurrentToken.StringValue == ")")    // end of arg list
                        break;

                    if (CurrentToken.StringValue != ",")    // at this point we're expecting a comma separator
                        return LogError("Unexpected token '" + CurrentToken.StringValue + "'. Expected ',' or ')'");

                    NextToken();
                }
            }

            NextToken();    // finally, eat the ')'
            return new ASTExpressionCall(name, args);
        }

        private static ASTExpression ParseRHSBinaryOperator (int prec, ASTExpression lhs)
        {
            while(true)
            {
                int tokPrec = Grammar.GetPrecedence(CurrentToken.StringValue);

                // If this is a binop that binds at least as tightly as the current binop,
                // consume it, otherwise we are done.
                if (tokPrec < prec)
                    return lhs;

                // we now know this is a binary operator
                string binop = CurrentToken.StringValue;
                NextToken();    // consume op

                // parse primary expr after the operator
                ASTExpression rhs = ParsePrimary();
                if (rhs == null) return null;

                // If the binop binds less tightly with RHS than the operator after RHS, let
                // the pending operator take RHS as its LHS. Basically look-ahead
                int nextPrec = Grammar.GetPrecedence(CurrentToken.StringValue);
                if(tokPrec < nextPrec)
                {
                    rhs = ParseRHSBinaryOperator(prec + 1, rhs);
                    if (rhs == null)
                        return null;
                }

                // merge lhs and rhs
                lhs = new ASTExpressionBinary(binop, lhs, rhs);
            }   // end of while loop
        }

        private static ASTPrototype ParsePrototype ()
        {
            if (CurrentToken.Type != TokenType.Identifier)
                return ErrorPrototype("Expected name in function definition");

            string name = CurrentToken.StringValue;
            NextToken();    // consume the identifier

            if (CurrentToken.StringValue != "(")
                return ErrorPrototype("Expected '(' in function definition");

            NextToken();    // consume '('

            // parse possible list of arguments
            List<string> args = new List<string>();
            if (CurrentToken.StringValue != ")") // we got arguments
            {
                while (true)
                {
                    if (CurrentToken.Type == TokenType.Identifier)
                    {
                        args.Add(CurrentToken.StringValue);
                        NextToken();
                        continue;
                    }

                    if (CurrentToken.StringValue == ")")    // end of arg list
                        break;

                    if (CurrentToken.StringValue != ",")    // the only other allowed char
                        return ErrorPrototype("Unexpected token '" + CurrentToken.StringValue + "' in argument list");

                    NextToken();
                }
            }
            
            NextToken();    // finally, eat the ')'

            return new ASTPrototype(name, args);
        }

        private static ASTFunction ParseDefinition ()
        {
            NextToken();    // eat the 'def' keyword

            ASTPrototype proto = ParsePrototype();  // parse the definition
            if (proto == null)
                return null;

            ASTExpression body = ParseExpression(); // and the body
            if (body != null)
                return new ASTFunction(proto, body);

            return null;
        }

        private static ASTFunction ParseTopLevelExpression ()
        {
            ASTExpression e = ParseExpression();
            if (e == null)
                return null;

            // create an anonymous prototype to evaluate the expression
            ASTPrototype proto = new ASTPrototype("__proto", new List<string>());
            return new ASTFunction(proto, e);
        }

        /// <summary>
        /// Helper function to easily log errors
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static ASTExpression LogError (string message)
        {
            Console.WriteLine("Error:\t{0}", message);
            return null;
        }

        /// <summary>
        /// The actual error logging prototype
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static ASTPrototype ErrorPrototype (string message)
        {
            LogError(message);
            return null;
        }
    }
}
