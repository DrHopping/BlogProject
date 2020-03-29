using System.Collections.Generic;
using BLL.Interfaces;

namespace BLL.Mappers
{
    public abstract class BaseMapper<TFirst, TSecond> : IMapper<TFirst, TSecond>
    {
        public abstract TFirst Map(TSecond element);
        public abstract TSecond Map(TFirst element);

        public IEnumerable<TFirst> Map(IEnumerable<TSecond> elements)
        {
            var mappedCollection = new List<TFirst>();
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    TFirst mappedElement = Map(element);
                    if (mappedElement != null)
                    {
                        mappedCollection.Add(mappedElement);
                    }
                }
            }

            return mappedCollection;
        }

        public IEnumerable<TSecond> Map(IEnumerable<TFirst> elements)
        {
            var mappedCollection = new List<TSecond>();
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    TSecond mappedElement = Map(element);
                    if (mappedElement != null)
                    {
                        mappedCollection.Add(mappedElement);
                    }
                }
            }

            return mappedCollection;
        }
    }
}