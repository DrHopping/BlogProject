using System;
using BLL.Exceptions.Base;

namespace BLL.Exceptions
{
    public class WrongCredentialsException : BadRequestException
    {
        public override string Message { get; }

        public WrongCredentialsException() : base()
        {
            Message = "User entered wrong credentials";
        }
    }
}