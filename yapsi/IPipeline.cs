namespace yapsi
{
    public interface IPipeline<T>
    {
        IContract<T> Bind();

        ISubscription<T> Subscribe();
    }
}
