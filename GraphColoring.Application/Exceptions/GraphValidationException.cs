using System;

namespace GraphColoring.Application.Exceptions
{
    public class GraphValidationException : Exception
    {
        public GraphValidationException(string message) : base(message)
        {
            
        }
    }
}