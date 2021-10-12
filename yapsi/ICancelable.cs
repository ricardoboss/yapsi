using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface ICancelable<T>
    {
        delegate void CancelledEventHandler(T sender);

        event CancelledEventHandler? Cancelled;

        bool IsCancelled { get; }

        void Cancel();
    }
}
