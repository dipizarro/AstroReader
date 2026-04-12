# SwissEph External Reference Cases

These reference checks are intentionally pragmatic.

Goal:
- confirm AstroReader's Swiss Ephemeris integration is no longer just "variable"
- confirm it matches well-known external chart references within a reasonable tolerance

Reference source strategy:
1. Use Astro-Seek snippets as the source of birth inputs:
   - UTC date/time
   - latitude/longitude
   - Placidus house system
2. Use Astrotheme snippets as the source of expected chart values:
   - Sun longitude
   - Moon longitude
   - Ascendant
   - selected house cusps

Why two sources:
- Astro-Seek search snippets expose UTC and coordinates very clearly.
- Astrotheme search snippets expose exact degrees for planets, Ascendant, and houses.

Current reference cases:

## Linda Cristal
- Astro-Seek: https://www.astro-seek.com/birth-chart/linda-cristal-horoscope
- Astrotheme: https://www.astrotheme.com/astrology/Linda_Cristal
- Inputs used:
  - UTC: 1931-02-24 15:00
  - Lat/Lon: 34°36'S, 58°23'W
- Expected values used:
  - Sun: 5°03' Pisces
  - Moon: 22°32' Taurus
  - Ascendant: 11°37' Taurus
  - House 4: 17°38' Leo
  - House 7: 11°37' Scorpio

## Whoopi Goldberg
- Astro-Seek: https://www.astro-seek.com/birth-chart/whoopi-goldberg-horoscope
- Astrotheme: https://www.astrotheme.com/astrology/Whoopi_Goldberg
- Inputs used:
  - UTC: 1955-11-13 17:48
  - Lat/Lon: 40°43'N, 74°0'W
- Expected values used:
  - Sun: 20°34' Scorpio
  - Moon: 12°06' Scorpio
  - Ascendant: 19°16' Aquarius
  - House 4: 6°54' Gemini
  - House 10: 6°54' Sagittarius

## Mark Twain
- Astro-Seek: https://www.astro-seek.com/birth-chart/mark-twain-horoscope
- Astrotheme: https://www.astrotheme.com/astrology/Mark_Twain
- Inputs used:
  - UTC: 1835-11-30 10:52
  - Lat/Lon: 39°30'N, 91°47'W
- Expected values used:
  - Sun: 7°34' Sagittarius
  - Moon: 15°37' Aries
  - Ascendant: 9°42' Scorpio
  - House 4: 17°22' Aquarius
  - House 10: 17°22' Leo

## Tolerances
- Planets: 0.5°
- Ascendant: 1.0°
- House cusps: 1.0°

Rationale:
- external sites round chart values to arcminutes, not infinite precision
- birth city coordinates may vary slightly by provider
- historical timezone handling and LMT cases can differ subtly between public calculators
- for MVP confidence, this tolerance is tight enough to catch wrong calculations but tolerant enough to avoid false failures from tiny source differences
