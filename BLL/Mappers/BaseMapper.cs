using System.Collections.Generic;
using BLL.Interfaces;

namespace BLL.Mappers
{
    public abstract class BaseMapper<TFirst, TSecond> : IMappper<TFirst, TSecond>
    {
        public abstract TFirst Map(TSecond element);
        public abstract TSecond Map(TFirst element);

        public ICollection<TFirst> Map(ICollection<TSecond> elements)
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

        public ICollection<TSecond> Map(ICollection<TFirst> elements)
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