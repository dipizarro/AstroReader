export interface CalculateChartRequest {
  dateOfBirth: string; // ISO String (YYYY-MM-DD)
  timeOfBirth: string; // HH:mm
  location: string;    // Nombre de ciudad o coords
}

export interface ChartPoint {
  pointName: string;
  zodiacSign: string;
  degrees: number;
}

export interface ChartAspect {
  point1: string;
  point2: string;
  aspectType: string;
  orb: number;
}

export interface CalculateChartResponse {
  chartId: string;
  points: ChartPoint[];
  aspects: ChartAspect[];
}
