namespace yapsi
{
    public interface IPipeline<T> : IContract<T>, ISubscription<T>
    {
        IReadOnlyCollection<IContract<T>> Contracts { get; }

        IReadOnlyCollection<ISubscription<T>> Subscriptions { get; }

        IContract<T> Bind();

        ISubscription<T> Subscribe();
    }
}
