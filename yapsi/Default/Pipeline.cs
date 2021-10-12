using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi.Default
{
    public class Pipeline<T> : IPipeline<T>, IDisposable
    {
        private readonly List<IContract<T>> contracts = new();
        private readonly List<ISubscription<T>> subscriptions = new();

        private bool disposedValue;

        public IReadOnlyCollection<IContract<T>> Contracts => contracts.AsReadOnly();
        public IReadOnlyCollection<ISubscription<T>> Subscriptions => subscriptions.AsReadOnly();

        public IContract<T> Bind()
        {
            var contract = new Contract<T>(this);

            contracts.Add(contract);

            contract.Cancelled += (c) => contracts.Remove(c);

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
                contracts.ToList().ForEach(c => c.Dispose());
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
