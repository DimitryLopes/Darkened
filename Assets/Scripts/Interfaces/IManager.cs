public interface IManager<T> where T : new()
{
    T CreateInstance();
}