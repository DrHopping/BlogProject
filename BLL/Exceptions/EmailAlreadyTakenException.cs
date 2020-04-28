using System;
using BLL.Exceptions.Base;

namespace BLL.Exceptions
{
    public class EmailAlreadyTakenException : BadRequestException
    {
        public override string Message { get; }

        public EmailAlreadyTakenException(string email) : base()
        {
            Message = $"Email '{email}' is already taken";
        }

    }
}