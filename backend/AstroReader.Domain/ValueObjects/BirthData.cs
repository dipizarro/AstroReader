using System;

namespace AstroReader.Domain.ValueObjects;

public record BirthData(DateTime UtcDate, string TimeZoneId);
