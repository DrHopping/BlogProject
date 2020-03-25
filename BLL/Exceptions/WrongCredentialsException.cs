using System;

namespace BLL.Exceptions
{
    public class WrongCredentialsException : ArgumentException
    {
        public override string Message { get; }

        public WrongCredentialsException() : base()
        {
            Message = "User entered wrong credentials";
        }
    }
}