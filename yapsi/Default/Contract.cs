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

            IEnumerable<ISubscription<T>> subscriptions;
            if (Pipeline is IPolyPipeline<T> polyPipeline)
                subscriptions = polyPipeline.Subscriptions.Where(s => !s.IsPaused && !s.IsCancelled);
            else if (Pipeline is ISingleBindPipeline<T> singleBindPipeline)
                subscriptions = singleBindPipeline.Subscriptions.Where(s => !s.IsPaused && !s.IsCancelled);
            else if (Pipeline is ISingleSubscribePipeline<T> singleSubscribePipeline)
                subscriptions = singleSubscribePipeline.Subscription is not null ? new[] { singleSubscribePipeline.Subscription } : Enumerable.Empty<ISubscription<T>>();
            else
                throw new NotImplementedException("Pipeline type could not be determined. Please override yapsi.Default.Contract.Publish(...) to implement your custom pipeline.");

            foreach (var subscription in subscriptions)
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
