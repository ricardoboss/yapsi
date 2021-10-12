namespace yapsi
{
    public interface IPipeline<T>
    {
        IReadOnlyCollection<IContract<T>> Contracts { get; }

        IReadOnlyCollection<ISubscription<T>> Subscriptions { get; }

        IContract<T> Bind();

        ISubscription<T> Subscribe();
    }
}
