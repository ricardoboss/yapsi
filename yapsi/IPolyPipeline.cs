using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface IPolyPipeline<T> : IPipeline<T>
    {
        IReadOnlyCollection<IContract<T>> Contracts { get; }

        IReadOnlyCollection<ISubscription<T>> Subscriptions { get; }
    }
}
