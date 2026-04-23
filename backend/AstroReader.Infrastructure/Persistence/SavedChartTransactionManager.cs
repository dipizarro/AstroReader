using AstroReader.Application.SavedCharts.Interfaces;

namespace AstroReader.Infrastructure.Persistence;

public sealed class SavedChartTransactionManager : ISavedChartTransactionManager
{
    private readonly AstroReaderDbContext _dbContext;

    public SavedChartTransactionManager(AstroReaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await operation(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}
