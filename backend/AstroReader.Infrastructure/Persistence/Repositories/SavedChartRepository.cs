using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AstroReader.Infrastructure.Persistence.Repositories;

public class SavedChartRepository : ISavedChartRepository
{
    private readonly AstroReaderDbContext _dbContext;

    public SavedChartRepository(AstroReaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SavedChart> AddAsync(SavedChart savedChart, CancellationToken cancellationToken = default)
    {
        _dbContext.SavedCharts.Add(savedChart);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return savedChart;
    }

    public async Task<SavedChart?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SavedCharts
            .AsNoTracking()
            .Where(x => x.Id == id);

        if (ownerUserId.HasValue)
        {
            query = query.Where(x => x.UserId == ownerUserId.Value);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SavedChartListItemDto>> GetListItemsAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SavedCharts
            .AsNoTracking()
            .AsQueryable();

        if (ownerUserId.HasValue)
        {
            query = query.Where(x => x.UserId == ownerUserId.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new SavedChartListItemDto
            {
                Id = x.Id,
                ProfileName = x.ProfileName,
                PlaceName = x.PlaceName,
                BirthDate = x.BirthDate.ToString("yyyy-MM-dd"),
                BirthTime = x.BirthTime.ToString("HH:mm"),
                TimezoneOffsetMinutes = x.TimezoneOffsetMinutes,
                SunSign = x.SunSign,
                MoonSign = x.MoonSign,
                AscendantSign = x.AscendantSign,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
