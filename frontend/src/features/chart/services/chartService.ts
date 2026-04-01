import { apiClient } from '../../../core/services/apiClient';
import type { CalculateChartRequest, CalculateChartResponse } from '../types/chart.types';

export const chartService = {
  /**
   * Envía los datos de nacimiento para calcular la carta natal en el backend .NET.
   */
  async calculateChart(request: CalculateChartRequest): Promise<CalculateChartResponse> {
    try {
      const result = await apiClient.post<CalculateChartResponse, CalculateChartRequest>('/api/charts/calculate', request);
      return result;
    } catch (e) {
      console.error('Error al calcular la carta natal:', e);
      throw e; // Lanzar hacia arriba para que el componente maneje el error visible.
    }
  }
};
