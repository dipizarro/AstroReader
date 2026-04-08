import { AlertCircle, Bookmark, CheckCircle2, Loader2 } from 'lucide-react';

export type SaveStatus = 'idle' | 'loading' | 'success' | 'error';

interface SaveChartPanelProps {
  chartLabel: string;
  saveStatus: SaveStatus;
  feedbackMessage: string | null;
  savedChartId: string | null;
  onChartLabelChange: (value: string) => void;
  onSave: () => void;
}

export const SaveChartPanel = ({
  chartLabel,
  saveStatus,
  feedbackMessage,
  savedChartId,
  onChartLabelChange,
  onSave,
}: SaveChartPanelProps) => {
  const statusToneClassName = saveStatus === 'success'
    ? 'border-emerald-500/20 bg-emerald-500/10 text-emerald-200'
    : saveStatus === 'error'
      ? 'border-red-500/20 bg-red-500/10 text-red-200'
      : 'border-white/10 bg-white/[0.03] text-text-muted';

  const buttonClassName = saveStatus === 'success'
    ? 'save-success-glow bg-gradient-to-r from-emerald-300 via-emerald-200 to-emerald-300 text-[#08110d] shadow-[0_0_28px_rgba(52,211,153,0.28)]'
    : 'bg-primary text-[#0a0a0b] shadow-[0_0_18px_rgba(212,175,55,0.28)] hover:bg-primary-hover hover:shadow-[0_0_26px_rgba(212,175,55,0.4)]';

  return (
    <section className="relative overflow-hidden rounded-3xl border border-white/10 bg-gradient-to-br from-[#131318] via-[#101014] to-[#0a0a0c] p-6 shadow-[0_0_40px_rgba(0,0,0,0.45)] md:p-8">
      <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-primary/40 to-transparent" />
      <div className="absolute -right-20 top-8 h-40 w-40 rounded-full bg-primary/10 blur-3xl" />

      <div className="relative z-10 flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
        <div className="max-w-xl space-y-3">
          <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-primary">
            <Bookmark className="h-3.5 w-3.5" />
            Guardar carta
          </div>
          <div className="space-y-2">
            <h3 className="text-2xl font-display font-medium text-white md:text-3xl">
              Conserva esta lectura para volver a ella cuando quieras
            </h3>
            <p className="text-sm leading-6 text-text-muted">
              Asigna un nombre claro a esta carta y guárdala en tu colección sin salir de la lectura actual.
            </p>
          </div>
        </div>

        <div className="w-full max-w-xl space-y-4">
          <div className="space-y-2">
            <label htmlFor="chartLabel" className="text-sm font-medium text-text">
              Nombre o etiqueta
            </label>
            <input
              id="chartLabel"
              type="text"
              value={chartLabel}
              maxLength={120}
              disabled={saveStatus === 'loading'}
              onChange={(event) => onChartLabelChange(event.target.value)}
              placeholder="Ej. Carta natal de Antonia"
              className="w-full rounded-2xl border border-white/10 bg-white/[0.04] px-4 py-3 text-sm text-white outline-none transition focus:border-primary/50 focus:ring-1 focus:ring-primary/40"
            />
          </div>

          <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
            <button
              type="button"
              onClick={onSave}
              disabled={saveStatus === 'loading'}
              className={`relative inline-flex min-w-[180px] items-center justify-center gap-2 overflow-hidden rounded-2xl px-5 py-3 text-sm font-semibold transition-all disabled:cursor-not-allowed disabled:opacity-60 ${buttonClassName}`}
            >
              {saveStatus === 'success' && (
                <span className="save-success-sheen pointer-events-none absolute inset-0" aria-hidden="true" />
              )}
              {saveStatus === 'loading' ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Guardando...
                </>
              ) : saveStatus === 'success' ? (
                <>
                  <CheckCircle2 className="save-success-badge h-4 w-4" />
                  Carta guardada
                </>
              ) : (
                <>
                  <Bookmark className="h-4 w-4" />
                  Guardar carta
                </>
              )}
            </button>

            <div className={`relative inline-flex min-h-12 items-center gap-2 overflow-hidden rounded-2xl border px-4 py-3 text-sm ${statusToneClassName} ${saveStatus === 'success' ? 'save-success-panel' : ''}`}>
              {saveStatus === 'success' && (
                <span className="save-success-sheen pointer-events-none absolute inset-0 opacity-60" aria-hidden="true" />
              )}
              {saveStatus === 'success' && <CheckCircle2 className="h-4 w-4 shrink-0" />}
              {saveStatus === 'error' && <AlertCircle className="h-4 w-4 shrink-0" />}
              {saveStatus === 'loading' && <Loader2 className="h-4 w-4 shrink-0 animate-spin" />}
              <span className="relative z-10">
                {feedbackMessage || 'Podrás renombrarla más adelante cuando exista la biblioteca de cartas.'}
              </span>
            </div>
          </div>

          {savedChartId && (
            <p className="text-xs tracking-wide text-text-muted/80">
              ID de guardado: <span className="font-mono text-text-muted">{savedChartId}</span>
            </p>
          )}
        </div>
      </div>
    </section>
  );
};
