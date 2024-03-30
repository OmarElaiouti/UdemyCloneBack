namespace Udemy.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        IEnumerable<T> GetById(int Id, Func<T, bool> predicate);



    }
}
