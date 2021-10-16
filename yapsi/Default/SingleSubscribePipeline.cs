using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi.Default
{
    public class SingleSubscribePipeline<T> : IPipeline<T>, ISingleSubscribePipeline<T>, IDisposable
    {
        private readonly List<IContract<T>> contracts = new();
        private ISubscription<T>? subscription = null;

        private bool disposedValue;

        public IReadOnlyCollection<IContract<T>> Contracts => contracts.AsReadOnly();
        public ISubscription<T>? Subscription => subscription;

        public IContract<T> Bind()
        {
            var contract = new Contract<T>(this);

            contracts.Add(contract);

            contract.Cancelled += (c) => contracts.Remove(c);

            return contract;
        }

        public ISubscription<T> Subscribe()
        {
            if (subscription is not null)
                throw new InvalidOperationException("Single subscription was already emitted. Cancel the previous subscription to create a new one.");

            subscription = new Subscription<T>();

            subscription.Cancelled += (s) => subscription = null;

            return subscription;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                contracts.ToList().ForEach(c => c.Dispose());
                subscription?.Dispose();
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
