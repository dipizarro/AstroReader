using AstroReader.Domain.Entities;

namespace AstroReader.Application.PersonalProfiles.Interfaces;

public interface IPersonalProfileRepository
{
    Task<PersonalProfile> AddAsync(PersonalProfile personalProfile, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PersonalProfile>> GetListAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default);
    Task<PersonalProfile?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default);
    Task<PersonalProfile?> GetTrackedByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default);
    Task<PersonalProfile?> GetBySavedChartIdAsync(Guid savedChartId, Guid? ownerUserId = null, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
