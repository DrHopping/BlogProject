using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IMapper<TFirst, TSecond>
    {
        TFirst Map(TSecond element);
        TSecond Map(TFirst element);
        IEnumerable<TFirst> Map(IEnumerable<TSecond> elements);
        IEnumerable<TSecond> Map(IEnumerable<TFirst> elements);
    }
}