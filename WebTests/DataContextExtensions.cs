using Web.Database;

namespace IntegrationTests;

public static class DataContextExtensions
{
    public static void AddEntity<T>(this DataContext dataContext, T entity)
        where T : class
    {
        dataContext.AddEntities(entity);
    }

    public static void AddEntities<T>(this DataContext dataContext, params T[] entities)
        where T : class
    {
        dataContext.Set<T>().AddRange(entities);
        dataContext.SaveChanges();
    }
}
