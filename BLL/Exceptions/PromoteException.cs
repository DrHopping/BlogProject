using BLL.Exceptions.Base;

namespace BLL.Exceptions
{
    public class PromoteException : BadRequestException
    {
        public override string Message { get; }

        public PromoteException(string errorMessage) : base()
        {
            Message = errorMessage;
        }
    }
}