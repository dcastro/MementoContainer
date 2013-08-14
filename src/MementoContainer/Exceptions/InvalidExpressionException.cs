using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer
{
    /// <summary>
    /// Thrown when an exception containing fields, variables, closures, method calls or any unexpected operation is supplied.
    /// </summary>
    public class InvalidExpressionException : Exception
    {

        private InvalidExpressionException(string msg)
            : base(msg)
        {

        }

        internal static InvalidExpressionException FieldFound
        {
            get { return new InvalidExpressionException("Expression may not contain fields, variables or closures."); }
        }

        internal static InvalidExpressionException MethodFound
        {
            get { return new InvalidExpressionException("Expression may not contain method calls."); }
        }

        internal static InvalidExpressionException UnrelatedProperty
        {
            get { return new InvalidExpressionException("Expression must map to one of the parameter's properties."); }
        }

        internal static InvalidExpressionException UnexpectedOperation
        {
            get { return new InvalidExpressionException("Unexpected operation found while parsing expression."); }
        }
    }
}
