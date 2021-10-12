using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi.Default
{
    public class Contract<T> : IContract<T>
    {
        private bool disposedValue;

        public bool IsCancelled { get; private set; }
        public IPipeline<T> Pipeline { get; }

        public event IContract<T>.CancelledEventHandler? Cancelled;

        public Contract(IPipeline<T> pipeline)
        {
            Pipeline = pipeline;
        }

        public void Cancel()
        {
            IsCancelled = true;

            Cancelled?.Invoke(this);
        }

        public void Publish(T packet)
        {
            if (IsCancelled)
                throw new OperationCanceledException("Cannot publish packets on a cancelled contract!");

            foreach (var subscription in Pipeline.Subscriptions.Where(s => !s.IsPaused && !s.IsCancelled))
                subscription.Publish(packet);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                Cancel();

            disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
