using System;

namespace BLL.Exceptions
{
    public class NotEnoughRightsException : Exception
    {
        public override string Message { get; }

        public NotEnoughRightsException() : base()
        {
            Message = "User doesn't have enough rights do perform this operation";
        }
    }
}