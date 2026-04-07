export interface CalculateChartRequest {
  birthDate: string; // ISO String (YYYY-MM-DD)
  birthTime: string; // HH:mm
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  placeName?: string;
}

export interface ChartSummary {
  sun: string;
  moon: string;
  ascendant: string;
}

export interface PlanetPosition {
  name: string;
  sign: string;
  degree: number;
}

export interface HouseCusp {
  house: number;
  sign: string;
}

export interface ChartInterpretation {
  headline: string;
  sun: string;
  moon: string;
  ascendant: string;
}

export interface CalculateChartResponse {
  summary: ChartSummary;
  planets: PlanetPosition[];
  houses: HouseCusp[];
  interpretation: ChartInterpretation;
}
