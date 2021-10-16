using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi.Default
{
    public class SingleBindPipeline<T> : IPipeline<T>, ISingleBindPipeline<T>, IDisposable
    {
        private IContract<T>? contract;
        private readonly List<ISubscription<T>> subscriptions = new();

        private bool disposedValue;

        public IContract<T>? Contract => contract;
        public IReadOnlyCollection<ISubscription<T>> Subscriptions => subscriptions.AsReadOnly();

        public IContract<T> Bind()
        {
            if (contract is not null)
                throw new InvalidOperationException("Single bind contract was already emitted. Cancel the previous contract to create a new one.");

            contract = new Contract<T>(this);

            contract.Cancelled += (c) => contract = null;

            return contract;
        }

        public ISubscription<T> Subscribe()
        {
            var subscription = new Subscription<T>();

            subscriptions.Add(subscription);

            subscription.Cancelled += (s) => subscriptions.Remove(s);

            return subscription;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                contract?.Dispose();
                subscriptions.ToList().ForEach(s => s.Dispose());
            }

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
