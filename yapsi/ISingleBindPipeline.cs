using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface ISingleBindPipeline<T> : IPipeline<T>
    {
        IContract<T>? Contract { get; }

        IReadOnlyCollection<ISubscription<T>> Subscriptions { get; }
    }
}
