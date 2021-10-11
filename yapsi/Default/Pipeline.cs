using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi.Default
{
    public class Pipeline<T> : IPipeline<T>
    {
        private readonly List<IContract<T>> contracts = new();
        private readonly List<ISubscription<T>> subscriptions = new();

        private bool isContractCancelled;
        private bool isSubscriptionCancelled;

        private event IContract<T>.CancelledEventHandler? ContractCancelled;
        private event ISubscription<T>.CancelledEventHandler? SubscriptionCancelled;

        public IReadOnlyCollection<IContract<T>> Contracts => contracts.AsReadOnly();
        public IReadOnlyCollection<ISubscription<T>> Subscriptions => subscriptions.AsReadOnly();

        public bool IsPaused { get; private set; }
        bool IContract<T>.IsCancelled => isContractCancelled;
        bool ISubscription<T>.IsCancelled => isSubscriptionCancelled;

        public event ISubscription<T>.PublishedEventHandler? Published;
        public event IPausable.PausedEventHandler? Paused;
        public event IPausable.ResumedEventHandler? Resumed;

        event IContract<T>.CancelledEventHandler? IContract<T>.Cancelled
        {
            add
            {
                ContractCancelled += value;
            }

            remove
            {
                ContractCancelled -= value;
            }
        }

        event ISubscription<T>.CancelledEventHandler? ISubscription<T>.Cancelled
        {
            add
            {
                SubscriptionCancelled += value;
            }

            remove
            {
                SubscriptionCancelled -= value;
            }
        }

        void IContract<T>.Cancel()
        {
            isContractCancelled = true;

            ContractCancelled?.Invoke(this);
        }

        void ISubscription<T>.Cancel()
        {
            isSubscriptionCancelled = true;

            SubscriptionCancelled?.Invoke(this);
        }

        public IContract<T> Bind()
        {
            contracts.Add(this);

            ContractCancelled += (c) => contracts.Remove(c);

            return this;
        }

        public ISubscription<T> Subscribe()
        {
            subscriptions.Add(this);

            SubscriptionCancelled += (s) => subscriptions.Remove(s);

            return this;
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

        void IContract<T>.Publish(T packet)
        {
            if (isContractCancelled)
                throw new OperationCanceledException("Cannot publish packets on a cancelled contract!");

            var activeSubscriptions = subscriptions.Where(s => !s.IsPaused).ToList();
            for (var i = 0; i < activeSubscriptions.Count; i++)
            {
                var subscriber = activeSubscriptions[i];
                if (subscriber.IsCancelled)
                    subscriptions.Remove(subscriber);
                else
                    subscriber.Publish(packet);
            }
        }

        void ISubscription<T>.Publish(T packet)
        {
            if (isSubscriptionCancelled)
                throw new OperationCanceledException("Cannot broadcast packets on a cancelled subscription!");

            Published?.Invoke(this, packet);
        }
    }
}
