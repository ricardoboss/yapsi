using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface IContract<T>
    {
        public delegate void CancelledEventHandler(IContract<T> sender);

        event CancelledEventHandler? Cancelled;

        bool IsCancelled { get; }

        void Cancel();

        void Publish(T packet);
    }
}
