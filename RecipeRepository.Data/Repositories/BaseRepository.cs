using Microsoft.EntityFrameworkCore;

namespace RecipeRepository.Data.Repositories;

public class BaseRepository<TContext> where TContext : DbContext
{
    protected TContext Context { get; }

    protected BaseRepository(TContext context)
    {
        Context = context;
    }

    #region public

    public bool Exists<T>(Func<T, bool> expression) where T : class
    {
        return Context.Set<T>().Local.Any(expression);
    }

    public void Insert<T>(T entity) where T : class
    {
        Context.Set<T>().Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
        Context.Set<T>().Remove(entity);
    }

    public async Task<bool> SaveChangesAsync(bool throwOnError = false)
    {
        try
        {
            await Context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            if (throwOnError)
                throw;

            await Console.Error.WriteLineAsync(e.Message);
            return false;
        }
    }

    #endregion

}
