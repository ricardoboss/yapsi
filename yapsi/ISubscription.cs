using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface ISubscription<T> : IPausable
    {
        public delegate void PublishedEventHandler(ISubscription<T> sender, T packet);

        public delegate void CancelledEventHandler(ISubscription<T> sender);

        event PublishedEventHandler? Published;

        event CancelledEventHandler? Cancelled;

        bool IsCancelled { get; }

        void Cancel();

        internal void Publish(T packet);
    }
}
