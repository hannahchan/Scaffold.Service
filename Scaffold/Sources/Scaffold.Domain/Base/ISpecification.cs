namespace Scaffold.Domain.Base;

public interface ISpecification<in T>
{
    public bool IsSatisfiedBy(T obj);
}
