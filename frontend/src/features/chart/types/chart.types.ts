export interface CalculateChartRequest {
  birthDate: string; // ISO String (YYYY-MM-DD)
  birthTime: string; // HH:mm
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  placeName?: string;
}

export interface ChartMetadata {
  calculatedForUtc: string;
  latitude: number;
  longitude: number;
  placeName?: string | null;
}

export interface ChartSummary {
  sun: string;
  moon: string;
  ascendant: string;
}

export interface PlanetPosition {
  name: string;
  sign: string;
  signDegree: number;
  absoluteDegree: number;
  isRetrograde: boolean;
}

export interface HouseCusp {
  number: number;
  sign: string;
  absoluteDegree: number;
}

export interface ChartInterpretation {
  headline: string;
  summary: string;
  core: CoreInterpretation;
  personalPlanets: PersonalPlanetsInterpretation;
  houses: HouseInterpretation[];
  profiles: InterpretationProfile[];
}

export interface CoreInterpretation {
  sun: string;
  moon: string;
  ascendant: string;
}

export interface PersonalPlanetsInterpretation {
  mercury: string;
  venus: string;
  mars: string;
}

export interface HouseInterpretation {
  houseNumber: number;
  sign: string;
  title: string;
  meaning: string;
}

export interface InterpretationProfile {
  key: string;
  title: string;
  summary: string;
}

export interface CalculateChartResponse {
  metadata?: ChartMetadata;
  summary: ChartSummary;
  planets: PlanetPosition[];
  houses: HouseCusp[];
  interpretation: ChartInterpretation;
}

export interface SaveChartRequest {
  profileName: string;
  placeName?: string;
  birthDate: string;
  birthTime: string;
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  timezoneIana?: string;
  userId?: string | null;
}

export interface SavedChartDetail {
  id: string;
  userId?: string | null;
  profileName: string;
  placeName?: string | null;
  timezoneIana?: string | null;
  birthDate: string;
  birthTime: string;
  timezoneOffsetMinutes: number;
  birthInstantUtc: string;
  latitude: number;
  longitude: number;
  sunSign: string;
  moonSign: string;
  ascendantSign: string;
  snapshotVersion: number;
  createdAtUtc: string;
  updatedAtUtc: string;
  chart: CalculateChartResponse;
}

export interface SavedChartListItem {
  id: string;
  profileName: string;
  placeName?: string | null;
  birthDate: string;
  birthTime: string;
  timezoneOffsetMinutes: number;
  sunSign: string;
  moonSign: string;
  ascendantSign: string;
  createdAtUtc: string;
}
