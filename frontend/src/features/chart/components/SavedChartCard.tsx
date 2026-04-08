import { Link } from 'react-router-dom';
import { ArrowUpRight, CalendarDays, MapPin, Sparkles, Bookmark } from 'lucide-react';
import type { SavedChartListItem } from '../types/chart.types';
import { formatBirthDate, formatSavedDate } from '../utils/chartFormatters';

interface SavedChartCardProps {
  chart: SavedChartListItem;
}

export const SavedChartCard = ({ chart }: SavedChartCardProps) => {
  return (
    <article className="group relative overflow-hidden rounded-3xl border border-white/10 bg-gradient-to-br from-[#141419] via-[#101014] to-[#0b0b0d] p-6 shadow-[0_0_32px_rgba(0,0,0,0.28)] transition-all hover:-translate-y-1 hover:border-primary/20 hover:shadow-[0_0_42px_rgba(0,0,0,0.4)]">
      <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-primary/35 to-transparent" />
      <div className="absolute -right-16 top-6 h-28 w-28 rounded-full bg-primary/10 blur-3xl transition-opacity group-hover:opacity-100" />

      <div className="relative z-10 flex h-full flex-col gap-6">
        <div className="flex items-start justify-between gap-4">
          <div className="space-y-3">
            <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.24em] text-primary">
              <Bookmark className="h-3.5 w-3.5" />
              Carta guardada
            </div>
            <div>
              <h3 className="font-display text-2xl font-medium text-white">
                {chart.profileName}
              </h3>
              <p className="mt-2 text-sm leading-6 text-text-muted">
                {chart.sunSign} · {chart.moonSign} · {chart.ascendantSign}
              </p>
            </div>
          </div>

          <Link
            to={`/charts/saved/${chart.id}`}
            className="inline-flex h-11 w-11 items-center justify-center rounded-full border border-white/10 bg-white/[0.04] text-text-muted transition-all hover:border-primary/30 hover:bg-primary/10 hover:text-primary"
            aria-label={`Abrir carta ${chart.profileName}`}
          >
            <ArrowUpRight className="h-4 w-4" />
          </Link>
        </div>

        <div className="grid gap-3 text-sm text-text-muted sm:grid-cols-2">
          <div className="rounded-2xl border border-white/6 bg-white/[0.03] p-4">
            <div className="mb-2 flex items-center gap-2 text-xs uppercase tracking-[0.2em] text-primary/80">
              <CalendarDays className="h-3.5 w-3.5" />
              Nacimiento
            </div>
            <p className="text-white">{formatBirthDate(chart.birthDate)}</p>
          </div>

          <div className="rounded-2xl border border-white/6 bg-white/[0.03] p-4">
            <div className="mb-2 flex items-center gap-2 text-xs uppercase tracking-[0.2em] text-primary/80">
              <MapPin className="h-3.5 w-3.5" />
              Lugar
            </div>
            <p className="text-white">{chart.placeName || 'Lugar no especificado'}</p>
          </div>
        </div>

        <div className="mt-auto flex items-center justify-between border-t border-white/6 pt-4 text-sm">
          <div className="text-text-muted">
            Guardada el <span className="text-white">{formatSavedDate(chart.createdAtUtc)}</span>
          </div>

          <Link
            to={`/charts/saved/${chart.id}`}
            className="inline-flex items-center gap-2 text-sm font-medium text-primary transition-colors hover:text-primary-hover"
          >
            <Sparkles className="h-4 w-4" />
            Abrir carta
          </Link>
        </div>
      </div>
    </article>
  );
};
