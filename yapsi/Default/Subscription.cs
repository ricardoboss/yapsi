using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi.Default
{
    public class Subscription<T> : ISubscription<T>
    {
        private bool disposedValue;

        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }

        public event ISubscription<T>.PublishedEventHandler? Published;
        public event IPausable.PausedEventHandler? Paused;
        public event IPausable.ResumedEventHandler? Resumed;
        public event ICancelable<ISubscription<T>>.CancelledEventHandler? Cancelled;

        public void Cancel()
        {
            IsCancelled = true;

            Cancelled?.Invoke(this);
        }

        public void Pause()
        {
            IsPaused = true;

            Paused?.Invoke(this);
        }

        public void Resume()
        {
            IsPaused = false;

            Resumed?.Invoke(this);
        }

        void ISubscription<T>.Publish(T packet)
        {
            if (IsCancelled)
                throw new OperationCanceledException("Cannot broadcast packets on a cancelled subscription!");

            Published?.Invoke(this, packet);
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
