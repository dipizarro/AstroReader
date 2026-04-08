import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Bookmark, RefreshCw, Sparkles } from 'lucide-react';
import { chartService } from '../services/chartService';
import type { SavedChartListItem } from '../types/chart.types';
import { SavedChartCard } from '../components/SavedChartCard';
import { SavedChartsSkeleton } from '../components/SavedChartsSkeleton';

export const SavedChartsPage = () => {
  const [charts, setCharts] = useState<SavedChartListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadSavedCharts = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await chartService.getSavedCharts();
      setCharts(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No pudimos cargar las cartas guardadas.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadSavedCharts();
  }, []);

  return (
    <div className="min-h-screen px-6 py-10 lg:px-12 lg:py-14">
      <div className="mx-auto max-w-6xl">
        <section className="relative overflow-hidden rounded-[2rem] border border-white/10 bg-gradient-to-br from-[#141418] via-[#101014] to-[#09090b] px-6 py-8 shadow-[0_0_48px_rgba(0,0,0,0.32)] sm:px-8 sm:py-10">
          <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-primary/40 to-transparent" />
          <div className="absolute -right-16 top-0 h-48 w-48 rounded-full bg-primary/10 blur-3xl" />

          <div className="relative z-10 flex flex-col gap-8 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-2xl space-y-4">
              <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-primary">
                <Bookmark className="h-3.5 w-3.5" />
                Biblioteca astral
              </div>
              <div className="space-y-3">
                <h1 className="font-display text-4xl font-semibold tracking-tight text-glow sm:text-5xl">
                  Tus cartas guardadas
                </h1>
                <p className="max-w-xl text-base leading-7 text-text-muted">
                  Una primera colección sobria para revisar tus lecturas, volver a un perfil y retomar el hilo de cada carta.
                </p>
              </div>
            </div>

            <div className="flex flex-col gap-3 sm:flex-row">
              <button
                type="button"
                onClick={() => void loadSavedCharts()}
                disabled={loading}
                className="inline-flex items-center justify-center gap-2 rounded-full border border-white/10 bg-white/[0.04] px-5 py-3 text-sm font-medium text-white transition-all hover:bg-white/[0.08] disabled:cursor-not-allowed disabled:opacity-60"
              >
                <RefreshCw className={`h-4 w-4 ${loading ? 'animate-spin' : ''}`} />
                Recargar
              </button>

              <Link
                to="/chart/calculate"
                className="inline-flex items-center justify-center gap-2 rounded-full bg-primary px-5 py-3 text-sm font-semibold text-[#0a0a0b] shadow-[0_0_18px_rgba(212,175,55,0.28)] transition-all hover:bg-primary-hover hover:shadow-[0_0_26px_rgba(212,175,55,0.4)]"
              >
                <Sparkles className="h-4 w-4" />
                Nueva carta
              </Link>
            </div>
          </div>
        </section>

        <section className="mt-10">
          {loading ? (
            <SavedChartsSkeleton />
          ) : error ? (
            <div className="rounded-3xl border border-red-500/20 bg-red-500/10 p-6 text-center">
              <h2 className="text-lg font-medium text-red-200">No pudimos abrir tu biblioteca astral</h2>
              <p className="mt-2 text-sm text-red-200/80">{error}</p>
              <button
                type="button"
                onClick={() => void loadSavedCharts()}
                className="mt-5 inline-flex items-center justify-center rounded-full border border-red-300/20 bg-red-400/10 px-5 py-2.5 text-sm font-medium text-red-100 transition hover:bg-red-400/20"
              >
                Reintentar
              </button>
            </div>
          ) : charts.length === 0 ? (
            <div className="rounded-[2rem] border border-white/8 bg-gradient-to-b from-[#121216] to-[#0c0c0f] p-10 text-center">
              <div className="mx-auto mb-5 flex h-14 w-14 items-center justify-center rounded-full bg-primary/10">
                <Bookmark className="h-6 w-6 text-primary" />
              </div>
              <h2 className="text-2xl font-display text-white">Aún no hay cartas guardadas</h2>
              <p className="mx-auto mt-3 max-w-lg text-sm leading-6 text-text-muted">
                Calcula una carta y guárdala para comenzar a construir tu biblioteca personal dentro de AstroReader.
              </p>
              <Link
                to="/chart/calculate"
                className="mt-6 inline-flex items-center justify-center gap-2 rounded-full bg-primary px-6 py-3 text-sm font-semibold text-[#0a0a0b] shadow-[0_0_18px_rgba(212,175,55,0.28)] transition-all hover:bg-primary-hover hover:shadow-[0_0_26px_rgba(212,175,55,0.4)]"
              >
                <Sparkles className="h-4 w-4" />
                Calcular primera carta
              </Link>
            </div>
          ) : (
            <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
              {charts.map((chart) => (
                <SavedChartCard key={chart.id} chart={chart} />
              ))}
            </div>
          )}
        </section>
      </div>
    </div>
  );
};
