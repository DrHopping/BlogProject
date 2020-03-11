using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IMappper<TFirst, TSecond>
    {
        TFirst Map(TSecond element);
        TSecond Map(TFirst element);
        ICollection<TFirst> Map(ICollection<TSecond> elements);
        ICollection<TSecond> Map(ICollection<TFirst> elements);
    }
}