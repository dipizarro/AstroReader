import type { ChartInterpretation, ChartSummary } from '../types/chart.types';

interface ChartInterpretationSectionProps {
  interpretation: ChartInterpretation;
  summary: ChartSummary;
}

export const ChartInterpretationSection = ({
  interpretation,
  summary,
}: ChartInterpretationSectionProps) => {
  const { coverage } = interpretation;
  const primaryBlocks = [
    interpretation.energyCore,
    interpretation.core,
    interpretation.personalDynamics,
    interpretation.essentialSummary,
  ].filter((block) => block?.mainText);

  if (!interpretation.hook && primaryBlocks.length === 0) {
    return null;
  }

  const coverageToneClassName = coverage.coverageStatus === 'complete'
    ? 'border-primary/20 bg-[radial-gradient(circle_at_top,rgba(212,175,55,0.14),transparent_44%),linear-gradient(135deg,rgba(18,18,24,0.96),rgba(10,10,12,0.98))]'
    : coverage.coverageStatus === 'partial'
      ? 'border-amber-200/15 bg-[radial-gradient(circle_at_top,rgba(245,158,11,0.12),transparent_42%),linear-gradient(135deg,rgba(18,18,24,0.96),rgba(10,10,12,0.98))]'
      : 'border-slate-300/12 bg-[radial-gradient(circle_at_top,rgba(148,163,184,0.14),transparent_42%),linear-gradient(135deg,rgba(18,18,24,0.96),rgba(10,10,12,0.98))]';

  const coverageBadgeClassName = coverage.coverageStatus === 'complete'
    ? 'border-primary/20 bg-primary/10 text-primary'
    : coverage.coverageStatus === 'partial'
      ? 'border-amber-300/15 bg-amber-400/10 text-amber-200'
      : 'border-slate-200/10 bg-slate-200/10 text-slate-200';

  const coverageLabel = coverage.coverageStatus === 'complete'
    ? 'Cobertura Premium Completa'
    : coverage.coverageStatus === 'partial'
      ? 'Cobertura Premium Parcial'
      : 'Lectura Base Disponible';

  const coverageMessage = coverage.coverageStatus === 'complete'
    ? 'Esta lectura ya integra una composición premium completa para los bloques centrales de tu carta.'
    : coverage.coverageStatus === 'partial'
      ? 'Esta lectura ya tiene base premium real, pero todavía faltan algunas piezas editoriales. Te mostramos primero lo que sí está compuesto con consistencia.'
      : 'Esta respuesta conserva una base útil de lectura, pero todavía no corresponde a una composición premium completa para tu combinación actual.';

  const coverageDetail = coverage.coverageStatus === 'complete'
    ? null
    : coverage.coverageStatus === 'partial'
      ? `Bloques compuestos: ${coverage.composedBlocks.length}. Cobertura disponible: ${coverage.coveredEntries.length}.`
      : `Cobertura disponible: ${coverage.coveredEntries.length}. Entradas pendientes: ${coverage.missingEntries.length}.`;

  const renderBlock = (
    block: ChartInterpretationSectionProps['interpretation']['energyCore'],
    tone: 'gold' | 'silver' | 'ink' | 'slate' = 'gold',
  ) => {
    if (!block?.mainText) {
      return null;
    }

    const toneClassName = tone === 'gold'
      ? 'border-primary/20 bg-gradient-to-br from-primary/10 via-white/[0.03] to-transparent'
      : tone === 'silver'
        ? 'border-slate-300/15 bg-gradient-to-br from-slate-200/[0.08] via-white/[0.03] to-transparent'
        : tone === 'ink'
          ? 'border-white/10 bg-gradient-to-br from-white/[0.04] via-white/[0.02] to-transparent'
          : 'border-slate-500/15 bg-gradient-to-br from-slate-500/[0.08] via-white/[0.03] to-transparent';

    return (
      <section
        key={block.key}
        className={`overflow-hidden rounded-[28px] border p-7 shadow-[0_0_30px_rgba(0,0,0,0.22)] ${toneClassName}`}
      >
        <div className="space-y-4">
          <div className="space-y-2">
            <span className="inline-flex rounded-full border border-white/10 bg-white/[0.04] px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-text-muted">
              {block.title}
            </span>
            <p className="text-base leading-8 text-white/88 md:text-[1.04rem]">
              {block.mainText}
            </p>
          </div>

          {block.subBlocks?.length > 0 && (
            <div className="grid gap-4 md:grid-cols-2">
              {block.subBlocks.map((subBlock) => (
                <article
                  key={subBlock.key}
                  className="rounded-2xl border border-white/8 bg-[#111116]/75 p-5"
                >
                  {subBlock.title && (
                    <h4 className="mb-3 text-sm font-semibold text-white/92">
                      {subBlock.title}
                    </h4>
                  )}
                  <p className="text-sm leading-7 text-text-muted">
                    {subBlock.text}
                  </p>
                </article>
              ))}
            </div>
          )}
        </div>
      </section>
    );
  };

  return (
    <section className="mx-auto max-w-5xl space-y-8">
      <div className={`overflow-hidden rounded-[32px] border px-7 py-10 text-center shadow-[0_0_48px_rgba(0,0,0,0.28)] md:px-10 ${coverageToneClassName}`}>
        <div className="mx-auto max-w-4xl space-y-5">
          <div className="flex flex-col items-center gap-3">
            <h3 className="text-xs uppercase tracking-[0.34em] text-primary">Lectura Premium</h3>
            <span className={`inline-flex rounded-full border px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.26em] ${coverageBadgeClassName}`}>
              {coverageLabel}
            </span>
          </div>
          {interpretation.hook && (
            <p className="text-2xl font-display leading-tight text-white/95 md:text-4xl">
              {interpretation.hook}
            </p>
          )}
          <p className="mx-auto max-w-3xl text-sm leading-7 text-text-muted md:text-[0.97rem]">
            Sol en {summary.sun}, Luna en {summary.moon} y Ascendente en {summary.ascendant} marcan la arquitectura visible de tu carta. Lo que sigue traduce esa base en una lectura más integrada y útil.
          </p>
          <div className="mx-auto max-w-3xl rounded-2xl border border-white/8 bg-white/[0.035] px-5 py-4 text-left">
            <p className="text-sm leading-7 text-white/84">
              {coverageMessage}
            </p>
            {coverageDetail && (
              <p className="mt-2 text-xs uppercase tracking-[0.2em] text-text-muted">
                {coverageDetail}
              </p>
            )}
          </div>
        </div>
      </div>

      <div className="grid gap-6">
        {renderBlock(interpretation.energyCore, 'gold')}
        {renderBlock(interpretation.core, 'silver')}
        {renderBlock(interpretation.personalDynamics, 'ink')}
        {renderBlock(interpretation.essentialSummary, 'slate')}
      </div>

      {interpretation.closing && (
        <div className="mx-auto max-w-4xl rounded-[28px] border border-white/10 bg-white/[0.03] px-6 py-7 text-center">
          <p className="text-sm leading-7 text-white/82 md:text-base">
            {interpretation.closing}
          </p>
        </div>
      )}

      {interpretation.lifeAreas?.length > 0 && (
        <section className="space-y-4 pt-2">
          <div className="flex items-center gap-3">
            <div className="h-4 w-1 rounded-full bg-white/30" />
            <h3 className="text-sm font-semibold uppercase tracking-[0.24em] text-text-muted">
              Áreas de Vida
            </h3>
          </div>
          <div className="grid gap-4 md:grid-cols-2">
            {interpretation.lifeAreas.map((block) => (
              <article
                key={block.key}
                className="rounded-2xl border border-white/8 bg-[#111116]/70 p-5"
              >
                <h4 className="mb-2 text-sm font-semibold text-white/90">{block.title}</h4>
                <p className="text-sm leading-7 text-text-muted">{block.mainText}</p>
              </article>
            ))}
          </div>
        </section>
      )}
    </section>
  );
};
