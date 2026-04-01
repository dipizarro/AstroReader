using AstroReader.Application.Charts.DTOs;

namespace AstroReader.Application.Charts.Interfaces;

public interface ICalculateNatalChartUseCase
{
    CalculateChartResponse Execute(CalculateChartRequest request);
}
