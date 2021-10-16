using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface ISingleSubscribePipeline<T> : IPipeline<T>
    {
        ISubscription<T>? Subscription { get; }

        IReadOnlyCollection<IContract<T>> Contracts { get; }
    }
}
