export interface CalculateChartRequest {
  dateOfBirth: string; // ISO String (YYYY-MM-DD)
  timeOfBirth: string; // HH:mm
  location: string;    // Nombre de ciudad
}

export interface ChartSummary {
  sunSign: string;
  moonSign: string;
  ascendantSign: string;
}

export interface PlanetPosition {
  name: string;
  sign: string;
  degree: number;
}

export interface HouseCusp {
  houseNumber: number;
  sign: string;
  degree: number;
}

export interface ChartInterpretation {
  sunSignInterpretation: string;
  moonSignInterpretation: string;
  ascendantInterpretation: string;
}

export interface CalculateChartResponse {
  summary: ChartSummary;
  planets: PlanetPosition[];
  houses: HouseCusp[];
  interpretation: ChartInterpretation;
}
