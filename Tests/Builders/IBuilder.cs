namespace Builders;

public interface IBuilder<T>
    where T : class
{
    public T Build();
}
