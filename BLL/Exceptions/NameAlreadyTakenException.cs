using System;

namespace BLL.Exceptions
{
    public class NameAlreadyTakenException : ArgumentException
    {
        public override string Message { get; }

        public NameAlreadyTakenException() : base()
        {
            Message = "Name used by user is already taken";
        }

    }
}