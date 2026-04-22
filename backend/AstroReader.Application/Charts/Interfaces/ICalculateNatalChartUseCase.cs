using AstroReader.Application.Charts.DTOs;

namespace AstroReader.Application.Charts.Interfaces;

public interface ICalculateNatalChartUseCase
{
    Task<CalculateChartResponse> ExecuteAsync(CalculateChartRequest request, CancellationToken cancellationToken = default);
}
