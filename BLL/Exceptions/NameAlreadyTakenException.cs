using System;
using BLL.Exceptions.Base;

namespace BLL.Exceptions
{
    public class NameAlreadyTakenException : BadRequestException
    {
        public override string Message { get; }

        public NameAlreadyTakenException(string name) : base()
        {
            Message = $"Name '{name}' is already taken";
        }
    }
}