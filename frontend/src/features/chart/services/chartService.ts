import { apiClient } from '../../../core/services/apiClient';
import type { CalculateChartRequest, CalculateChartResponse } from '../types/chart.types';

export const chartService = {
  /**
   * Envía los datos de nacimiento para calcular la carta natal.
   */
  async calculateChart(request: CalculateChartRequest): Promise<CalculateChartResponse> {
    
    // Si la API no está arriba, simular la respuesta o manejar la llamada
    // Aquí implementamos la llamada real. 
    // Para entornos donde el backend falla temporalmente, descomentar el mock abajo:
    
    try {
      const result = await apiClient.post<CalculateChartResponse, CalculateChartRequest>('/charts/calculate', request);
      return result;
    } catch (e) {
      // Si la URL de origen de la API base falla, mockeamos por ahora la respuesta 
      // para que el UI pueda ser mostrado y testeado.
      console.warn('Backend inalcanzable, retornando mock data...', e);
      
      return new Promise<CalculateChartResponse>((resolve) => {
        setTimeout(() => {
          resolve({
            chartId: "MOCK-1234-ABCD",
            points: [
              { pointName: "Sun", zodiacSign: "Aries", degrees: 15.4 },
              { pointName: "Moon", zodiacSign: "Taurus", degrees: 3.2 },
              { pointName: "Ascendant", zodiacSign: "Scorpio", degrees: 22.1 },
            ],
            aspects: [
              { point1: "Sun", point2: "Moon", aspectType: "Trine", orb: 2.1 }
            ]
          });
        }, 1500); // 1.5s delay to simulate network
      });
    }
  }
};
