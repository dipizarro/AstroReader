import { useEffect, useMemo, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ArrowLeft, CalendarDays, Clock3, MapPin, RefreshCw } from 'lucide-react';
import { chartService } from '../services/chartService';
import type { CalculateChartRequest, SavedChartDetail } from '../types/chart.types';
import { ChartResult } from '../components/ChartResult';
import { formatBirthDate, formatSavedDate } from '../utils/chartFormatters';

export const SavedChartDetailPage = () => {
  const { chartId } = useParams<{ chartId: string }>();
  const [savedChart, setSavedChart] = useState<SavedChartDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadSavedChart = async () => {
    if (!chartId) {
      setError('No encontramos el identificador de la carta solicitada.');
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const response = await chartService.getSavedChartById(chartId);
      setSavedChart(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No pudimos cargar esta carta guardada.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadSavedChart();
  }, [chartId]);

  const chartRequest = useMemo<CalculateChartRequest | null>(() => {
    if (!savedChart) {
      return null;
    }

    return {
      birthDate: savedChart.birthDate,
      birthTime: savedChart.birthTime,
      latitude: savedChart.latitude,
      longitude: savedChart.longitude,
      timezoneOffsetMinutes: savedChart.timezoneOffsetMinutes,
      placeName: savedChart.placeName ?? undefined,
    };
  }, [savedChart]);

  if (loading) {
    return (
      <div className="min-h-screen px-6 py-10 lg:px-12 lg:py-14">
        <div className="mx-auto max-w-6xl space-y-6">
          <div className="h-12 w-40 animate-pulse rounded-full bg-white/8" />
          <div className="rounded-[2rem] border border-white/8 bg-white/[0.03] p-8">
            <div className="mb-5 h-8 w-72 animate-pulse rounded-xl bg-white/10" />
            <div className="grid gap-4 md:grid-cols-4">
              {Array.from({ length: 4 }).map((_, index) => (
                <div key={index} className="h-24 animate-pulse rounded-2xl bg-white/8" />
              ))}
            </div>
          </div>
          <div className="h-[28rem] animate-pulse rounded-[2rem] bg-white/[0.03]" />
        </div>
      </div>
    );
  }

  if (error || !savedChart) {
    return (
      <div className="min-h-screen px-6 py-10 lg:px-12 lg:py-14">
        <div className="mx-auto max-w-3xl rounded-[2rem] border border-red-500/20 bg-red-500/10 p-8 text-center">
          <h1 className="text-2xl font-display text-red-100">No pudimos abrir esta carta</h1>
          <p className="mt-3 text-sm leading-6 text-red-100/80">
            {error || 'La carta solicitada no está disponible en este momento.'}
          </p>
          <div className="mt-6 flex flex-col justify-center gap-3 sm:flex-row">
            <button
              type="button"
              onClick={() => void loadSavedChart()}
              className="inline-flex items-center justify-center gap-2 rounded-full border border-red-300/20 bg-red-400/10 px-5 py-2.5 text-sm font-medium text-red-100 transition hover:bg-red-400/20"
            >
              <RefreshCw className="h-4 w-4" />
              Reintentar
            </button>
            <Link
              to="/charts/saved"
              className="inline-flex items-center justify-center gap-2 rounded-full border border-white/10 bg-white/[0.04] px-5 py-2.5 text-sm font-medium text-white transition hover:bg-white/[0.08]"
            >
              <ArrowLeft className="h-4 w-4" />
              Volver a la biblioteca
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen px-6 py-10 lg:px-12 lg:py-14">
      <div className="mx-auto max-w-6xl">
        <div className="mb-6">
          <Link
            to="/charts/saved"
            className="inline-flex items-center gap-2 text-sm font-medium text-text-muted transition-colors hover:text-primary"
          >
            <ArrowLeft className="h-4 w-4" />
            Volver a cartas guardadas
          </Link>
        </div>

        <section className="relative overflow-hidden rounded-[2rem] border border-white/10 bg-gradient-to-br from-[#15151a] via-[#111116] to-[#09090c] p-6 shadow-[0_0_48px_rgba(0,0,0,0.32)] md:p-8">
          <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-primary/40 to-transparent" />
          <div className="absolute -right-20 top-4 h-44 w-44 rounded-full bg-primary/10 blur-3xl" />

          <div className="relative z-10 flex flex-col gap-8">
            <div className="space-y-3">
              <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-primary">
                Carta abierta
              </div>
              <div>
                <h1 className="font-display text-4xl font-semibold tracking-tight text-white md:text-5xl">
                  {savedChart.profileName}
                </h1>
                <p className="mt-3 max-w-2xl text-sm leading-6 text-text-muted">
                  Guardada el {formatSavedDate(savedChart.createdAtUtc)}. Puedes recorrer nuevamente su lectura completa desde esta vista.
                </p>
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-4">
              <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
                <div className="mb-2 flex items-center gap-2 text-xs uppercase tracking-[0.2em] text-primary/80">
                  <CalendarDays className="h-3.5 w-3.5" />
                  Nacimiento
                </div>
                <p className="text-sm text-white">{formatBirthDate(savedChart.birthDate)}</p>
              </div>

              <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
                <div className="mb-2 flex items-center gap-2 text-xs uppercase tracking-[0.2em] text-primary/80">
                  <Clock3 className="h-3.5 w-3.5" />
                  Hora
                </div>
                <p className="text-sm text-white">{savedChart.birthTime}</p>
              </div>

              <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
                <div className="mb-2 flex items-center gap-2 text-xs uppercase tracking-[0.2em] text-primary/80">
                  <MapPin className="h-3.5 w-3.5" />
                  Lugar
                </div>
                <p className="text-sm text-white">{savedChart.placeName || 'Lugar no especificado'}</p>
              </div>

              <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
                <div className="mb-2 text-xs uppercase tracking-[0.2em] text-primary/80">
                  Tríada central
                </div>
                <p className="text-sm text-white">
                  {savedChart.sunSign} · {savedChart.moonSign} · {savedChart.ascendantSign}
                </p>
              </div>
            </div>
          </div>
        </section>

        <ChartResult data={savedChart.chart} request={chartRequest} showSavePanel={false} />
      </div>
    </div>
  );
};
