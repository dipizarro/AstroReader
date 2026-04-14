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
  coverage: InterpretationCoverage;
  hook: string;
  energyCore: InterpretationContentBlock;
  core: InterpretationContentBlock;
  personalDynamics: InterpretationContentBlock;
  essentialSummary: InterpretationContentBlock;
  tensionsAndPotential: InterpretationContentBlock[];
  lifeAreas: InterpretationContentBlock[];
  profiles: InterpretationProfile[];
  closing: string;
}

export type InterpretationCoverageStatus = 'complete' | 'partial' | 'fallback';

export interface InterpretationCoverage {
  coverageStatus: InterpretationCoverageStatus;
  isPremiumResult: boolean;
  isFallback: boolean;
  coveredEntries: string[];
  missingEntries: string[];
  composedBlocks: string[];
}

export interface InterpretationContentBlock {
  key: string;
  title: string;
  mainText: string;
  subBlocks: InterpretationSubBlock[];
}

export interface InterpretationSubBlock {
  key: string;
  title: string;
  text: string;
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
