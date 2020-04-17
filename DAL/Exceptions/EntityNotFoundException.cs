using System;

namespace BLL.Exceptions
{
    public class EntityNotFoundException : ArgumentException
    {
        public override string Message { get; }

        public EntityNotFoundException(string type, int id) : base()
        {
            Message = $"Couldn't find {type} with id {id}";
        }

    }
}