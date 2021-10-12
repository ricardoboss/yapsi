using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface ISubscription<T> : IPausable, ICancelable<ISubscription<T>>, IDisposable
    {
        delegate void PublishedEventHandler(ISubscription<T> sender, T packet);

        event PublishedEventHandler? Published;

        internal void Publish(T packet);
    }
}
