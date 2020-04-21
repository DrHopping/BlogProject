using System;
using BLL.Exceptions.Base;

namespace BLL.Exceptions
{
    public class EntityNotFoundException : NotFoundException
    {
        public override string Message { get; }

        public EntityNotFoundException(string type, int id) : base()
        {
            Message = $"Couldn't find {type} with id {id}";
        }

    }
}