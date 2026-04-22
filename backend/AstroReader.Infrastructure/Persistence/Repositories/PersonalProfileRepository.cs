using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AstroReader.Infrastructure.Persistence.Repositories;

public class PersonalProfileRepository : IPersonalProfileRepository
{
    private readonly AstroReaderDbContext _dbContext;

    public PersonalProfileRepository(AstroReaderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PersonalProfile> AddAsync(PersonalProfile personalProfile, CancellationToken cancellationToken = default)
    {
        _dbContext.PersonalProfiles.Add(personalProfile);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return personalProfile;
    }

    public async Task<PersonalProfile?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PersonalProfiles
            .AsNoTracking()
            .Where(x => x.Id == id);

        if (ownerUserId.HasValue)
        {
            query = query.Where(x => x.UserId == ownerUserId.Value);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PersonalProfile?> GetTrackedByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PersonalProfiles
            .Where(x => x.Id == id);

        if (ownerUserId.HasValue)
        {
            query = query.Where(x => x.UserId == ownerUserId.Value);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PersonalProfile?> GetBySavedChartIdAsync(Guid savedChartId, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PersonalProfiles
            .AsNoTracking()
            .Where(x => x.SavedChartId == savedChartId);

        if (ownerUserId.HasValue)
        {
            query = query.Where(x => x.UserId == ownerUserId.Value);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
