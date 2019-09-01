using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvikk
{
    public class ASTExpression { }

    /// <summary>
    /// Represents literal numbers
    /// </summary>
    public class ASTExpressionNumber : ASTExpression
    {
        public double Value;

        public ASTExpressionNumber(double value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Represents a variable call
    /// </summary>
    public class ASTExpressionVariable : ASTExpression
    {
        public string Name;

        public ASTExpressionVariable (string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Represents a binary operation
    /// </summary>
    public class ASTExpressionBinary : ASTExpression
    {
        public string Operator;
        public ASTExpression LHS, RHS;

        public ASTExpressionBinary(string op, ASTExpression lhs, ASTExpression rhs)
        {
            Operator = op;
            LHS = lhs;
            RHS = rhs;
        }
    }

    /// <summary>
    /// Represents a function call with optional arguments
    /// </summary>
    public class ASTExpressionCall : ASTExpression
    {
        public string Callee;
        public List<ASTExpression> Arguments;

        public ASTExpressionCall(string callee, List<ASTExpression> args)
        {
            Callee = callee;
            Arguments = args;
        }

        public ASTExpressionCall(string callee, params ASTExpression[] args)
        {
            Callee = callee;
            Arguments = new List<ASTExpression>();
            foreach(ASTExpression exp in args)
            {
                Arguments.Add(exp);
            }
        }
    }

    /// <summary>
    /// Represents an abstract description of a function
    /// </summary>
    public class ASTPrototype
    {
        public string Name;
        public List<string> Arguments;

        public ASTPrototype (string name, List<string> args)
        {
            Name = name;
            Arguments = args;
        }
    }

    /// <summary>
    /// Represents a function in and of itself
    /// </summary>
    public class ASTFunction
    {
        public ASTPrototype Prototype;
        public ASTExpression Body;

        public ASTFunction (ASTPrototype prototype, ASTExpression body)
        {
            Prototype = prototype;
            Body = body;
        }
    }
}