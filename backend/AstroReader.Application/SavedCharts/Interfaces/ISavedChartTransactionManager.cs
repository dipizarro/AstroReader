namespace AstroReader.Application.SavedCharts.Interfaces;

public interface ISavedChartTransactionManager
{
    Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);
}
