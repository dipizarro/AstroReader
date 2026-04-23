import { useState, useRef } from 'react';
import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { RefreshCw, Sparkles, X } from 'lucide-react';
import { CalculateChartForm } from '../components/CalculateChartForm';
import { ChartResult } from '../components/ChartResult';
import { chartService } from '../services/chartService';
import type { CalculateChartRequest, CalculateChartResponse } from '../types/chart.types';
import { personalProfileStorage } from '../../profile/services/personalProfileStorage';
import type { PersonalProfileChartContext } from '../../profile/types/profile.types';

interface CalculateChartPageLocationState {
  profileContext?: PersonalProfileChartContext;
}

export const CalculateChartPage = () => {
  const location = useLocation();
  const navigationProfileContext = (location.state as CalculateChartPageLocationState | null)?.profileContext ?? null;
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [chartData, setChartData] = useState<CalculateChartResponse | null>(null);
  const [lastRequest, setLastRequest] = useState<CalculateChartRequest | null>(null);
  const [profileContext, setProfileContext] = useState<PersonalProfileChartContext | null>(
    () => navigationProfileContext ?? null);
  const [recoverableProfileContext, setRecoverableProfileContext] = useState<PersonalProfileChartContext | null>(
    () => navigationProfileContext ? null : personalProfileStorage.getPendingChartContext());
  
  const resultRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!navigationProfileContext) {
      setRecoverableProfileContext(personalProfileStorage.getPendingChartContext());
      return;
    }

    personalProfileStorage.savePendingChartContext(navigationProfileContext);
    setProfileContext(navigationProfileContext);
    setRecoverableProfileContext(null);
  }, [navigationProfileContext]);

  const handleClearProfileContext = () => {
    setProfileContext(null);
  };

  const handleRestorePendingProfileContext = () => {
    const pendingContext = recoverableProfileContext ?? personalProfileStorage.getPendingChartContext();

    if (!pendingContext) {
      setRecoverableProfileContext(null);
      return;
    }

    setProfileContext(pendingContext);
    setRecoverableProfileContext(null);
  };

  const handleDismissPendingProfileContext = () => {
    personalProfileStorage.clearPendingChartContext();
    setRecoverableProfileContext(null);
  };

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

        {!profileContext && recoverableProfileContext && (
          <div className="mb-6 rounded-[1.75rem] border border-primary/14 bg-[linear-gradient(135deg,rgba(212,175,55,0.08),rgba(255,255,255,0.02))] p-5 shadow-[0_0_24px_rgba(0,0,0,0.16)]">
            <div className="flex items-start justify-between gap-4">
              <div className="flex gap-3">
                <div className="mt-0.5 flex h-10 w-10 shrink-0 items-center justify-center rounded-full border border-primary/20 bg-primary/10 text-primary">
                  <Sparkles className="h-4 w-4" />
                </div>
                <div>
                  <p className="text-xs font-semibold uppercase tracking-[0.24em] text-primary/90">
                    Recuperación disponible
                  </p>
                  <p className="mt-2 text-sm leading-6 text-white">
                    Encontramos un perfil reciente de <span className="font-medium">{recoverableProfileContext.fullName}</span>.
                  </p>
                  <p className="mt-2 text-sm leading-6 text-text-muted">
                    No lo aplicamos automáticamente para evitar contexto fantasma. Si quieres, puedes recuperarlo y seguir el cálculo con esos datos.
                  </p>
                </div>
              </div>

              <button
                type="button"
                onClick={handleDismissPendingProfileContext}
                className="inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-full border border-white/10 bg-white/[0.04] text-text-muted transition hover:bg-white/[0.08] hover:text-white"
                aria-label="Descartar contexto pendiente"
              >
                <X className="h-4 w-4" />
              </button>
            </div>

            <div className="mt-4 flex flex-col gap-3 sm:flex-row">
              <button
                type="button"
                onClick={handleRestorePendingProfileContext}
                className="inline-flex items-center justify-center gap-2 rounded-full border border-primary/20 bg-primary/10 px-4 py-2.5 text-sm font-medium text-primary transition hover:bg-primary/15 hover:text-primary-hover"
              >
                <RefreshCw className="h-4 w-4" />
                Recuperar este perfil
              </button>
              <button
                type="button"
                onClick={handleDismissPendingProfileContext}
                className="inline-flex items-center justify-center rounded-full border border-white/10 bg-white/[0.04] px-4 py-2.5 text-sm font-medium text-white transition hover:bg-white/[0.08]"
              >
                Empezar sin perfil
              </button>
            </div>
          </div>
        )}

        <CalculateChartForm
          onSubmit={handleSubmit}
          isLoading={loading}
          profileContext={profileContext}
          onClearProfileContext={handleClearProfileContext}
        />
      </div>

      <div className="w-full max-w-4xl mt-12" ref={resultRef}>
        {chartData && !loading && (
          <ChartResult data={chartData} request={lastRequest} />
        )}
      </div>
    </div>
  );
};
