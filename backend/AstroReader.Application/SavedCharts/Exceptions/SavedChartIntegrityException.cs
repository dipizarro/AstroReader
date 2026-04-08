namespace AstroReader.Application.SavedCharts.Exceptions;

public sealed class SavedChartIntegrityException : Exception
{
    public SavedChartIntegrityException(string message)
        : base(message)
    {
    }
}
