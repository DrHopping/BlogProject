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

    public class NotEnoughRightsException : ArgumentException
    {
        public override string Message { get; }

        public NotEnoughRightsException() : base()
        {
            Message = "User doesn't have enough rights do perform this operation";
        }
    }
}