using System.Collections.Generic;
using System.Linq;
using BLL.Exceptions.Base;
using Microsoft.AspNetCore.Identity;

namespace BLL.Exceptions
{
    public class BadPasswordException : BadRequestException
    {
        public override string Message { get; }

        public BadPasswordException(IEnumerable<IdentityError> errors) : base()
        {
            Message = errors.Aggregate("", (msg, next) => msg + $"{next.Description} ");
        }
    }
}