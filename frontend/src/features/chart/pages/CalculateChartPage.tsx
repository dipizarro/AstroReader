import { useState, useRef } from 'react';
import { CalculateChartForm } from '../components/CalculateChartForm';
import { ChartResult } from '../components/ChartResult';
import { chartService } from '../services/chartService';
import type { CalculateChartRequest, CalculateChartResponse } from '../types/chart.types';

export const CalculateChartPage = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [chartData, setChartData] = useState<CalculateChartResponse | null>(null);
  const [lastRequest, setLastRequest] = useState<CalculateChartRequest | null>(null);
  
  const resultRef = useRef<HTMLDivElement>(null);

  const handleSubmit = async (request: CalculateChartRequest) => {
    setLoading(true);
    setError(null);
    setChartData(null);
    setLastRequest(request);

    try {
      const response = await chartService.calculateChart(request);
      setChartData(response);
      
      // Hacer un pequeño scroll hacia abajo para mostrar el resultado si llega
      setTimeout(() => {
        resultRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }, 100);
      
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error al conectar con la bóveda astral.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-start min-h-screen p-6 lg:p-12 pb-24">
      {/* Header text */}
      <div className="mb-12 text-center max-w-2xl mx-auto">
        <h1 className="mb-4 font-display text-3xl font-semibold tracking-tight text-glow sm:text-5xl">
          Tu diseño astral
        </h1>
        <p className="text-text-muted">
          Ingresa tus datos de nacimiento precisos para calcular el estado del cosmos 
          en el instante de tu llegada, y descubre los hilos que tejen tu propósito.
        </p>
      </div>

      <div className="w-full max-w-lg">
        {error && (
          <div className="mb-6 rounded-xl border border-red-500/30 bg-red-500/10 p-4 text-center text-sm font-medium text-red-400">
            {error}
          </div>
        )}

        <CalculateChartForm onSubmit={handleSubmit} isLoading={loading} />
      </div>

      <div className="w-full max-w-4xl mt-12" ref={resultRef}>
        {chartData && !loading && (
          <ChartResult data={chartData} request={lastRequest} />
        )}
      </div>
    </div>
  );
};
