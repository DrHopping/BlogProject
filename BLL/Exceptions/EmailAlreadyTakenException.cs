using System;

namespace BLL.Exceptions
{
    public class EmailAlreadyTakenException : ArgumentException
    {
        public override string Message { get; }

        public EmailAlreadyTakenException() : base()
        {
            Message = "Email used by user is already taken";
        }

    }
}