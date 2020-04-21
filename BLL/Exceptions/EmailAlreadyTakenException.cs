using System;

namespace BLL.Exceptions
{
    public class EmailAlreadyTakenException : ArgumentException
    {
        public override string Message { get; }

        public EmailAlreadyTakenException(string email) : base()
        {
            Message = $"Email '{email}' is already taken";
        }

    }
}