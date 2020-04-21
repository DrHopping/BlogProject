using System;
using BLL.Exceptions.Base;

namespace BLL.Exceptions
{
    public class NotEnoughRightsException : ForbiddenException
    {
        public override string Message { get; }

        public NotEnoughRightsException() : base()
        {
            Message = "User doesn't have enough rights do perform this operation";
        }
    }
}