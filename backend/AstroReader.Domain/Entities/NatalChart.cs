using System;
using System.Collections.Generic;
using AstroReader.Domain.ValueObjects;

namespace AstroReader.Domain.Entities;

public class NatalChart
{
    public Guid Id { get; private set; }
    public BirthData BirthData { get; private set; }
    public GeoLocation Location { get; private set; }
    
    public IReadOnlyCollection<PlanetPosition> Planets { get; private set; }
    public IReadOnlyCollection<HousePosition> Houses { get; private set; }

    public NatalChart(BirthData birthData, GeoLocation location, 
                      List<PlanetPosition> planets, List<HousePosition> houses)
    {
        Id = Guid.NewGuid();
        BirthData = birthData ?? throw new ArgumentNullException(nameof(birthData));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Planets = planets?.AsReadOnly() ?? throw new ArgumentNullException(nameof(planets));
        Houses = houses?.AsReadOnly() ?? throw new ArgumentNullException(nameof(houses));
    }
}
