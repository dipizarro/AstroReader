import { useEffect, useMemo, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ArrowLeft, CalendarDays, Clock3, MapPin, RefreshCw, Sparkles } from 'lucide-react';
import { chartService } from '../services/chartService';
import type { CalculateChartRequest, SavedChartDetail } from '../types/chart.types';
import { ChartResult } from '../components/ChartResult';
import { formatBirthDate, formatSavedDate } from '../utils/chartFormatters';
import { personalProfileService } from '../../profile/services/personalProfileService';
import type { PersonalProfileChartContext, PersonalProfileDetail } from '../../profile/types/profile.types';

export const SavedChartDetailPage = () => {
  const { chartId } = useParams<{ chartId: string }>();
  const [savedChart, setSavedChart] = useState<SavedChartDetail | null>(null);
  const [fallbackProfile, setFallbackProfile] = useState<PersonalProfileDetail | null>(null);
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
      setFallbackProfile(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No pudimos cargar esta carta guardada.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadSavedChart();
  }, [chartId]);

  useEffect(() => {
    const shouldLoadFallbackProfile =
      !!chartId &&
      !!savedChart?.personalProfileId &&
      !savedChart.personalProfile;

    if (!shouldLoadFallbackProfile) {
      return;
    }

    let cancelled = false;

    const hydrateProfile = async () => {
      try {
        const profile = await personalProfileService.getProfileBySavedChartId(chartId);

        if (!cancelled) {
          setFallbackProfile(profile);
        }
      } catch (profileError) {
        console.warn('No pudimos rehidratar el perfil asociado a esta carta guardada.', profileError);
      }
    };

    void hydrateProfile();

    return () => {
      cancelled = true;
    };
  }, [chartId, savedChart?.personalProfile, savedChart?.personalProfileId]);

  const associatedProfileSummary = useMemo(() => {
    if (savedChart?.personalProfile) {
      return savedChart.personalProfile;
    }

    if (!fallbackProfile) {
      return null;
    }

    return {
      id: fallbackProfile.id,
      fullName: fallbackProfile.fullName,
      selfPerceptionFocus: fallbackProfile.selfPerceptionFocus,
      currentChallenge: fallbackProfile.currentChallenge,
      desiredInsight: fallbackProfile.desiredInsight,
    };
  }, [fallbackProfile, savedChart?.personalProfile]);

  const profileContext = useMemo<PersonalProfileChartContext | null>(() => {
    if (!savedChart) {
      return null;
    }

    const profileId = savedChart.personalProfileId ?? fallbackProfile?.id ?? null;
    const fullName = associatedProfileSummary?.fullName ?? fallbackProfile?.fullName ?? null;

    if (!profileId || !fullName) {
      return null;
    }

    return {
      profileId,
      fullName,
      birthDate: savedChart.birthDate,
      birthTime: savedChart.birthTime,
      birthPlace: savedChart.placeName ?? 'Lugar no especificado',
      latitude: savedChart.latitude,
      longitude: savedChart.longitude,
      timezoneOffsetMinutes: savedChart.timezoneOffsetMinutes,
    };
  }, [associatedProfileSummary?.fullName, fallbackProfile?.id, fallbackProfile?.fullName, savedChart]);

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
      personalProfileId: profileContext?.profileId ?? savedChart.personalProfileId ?? null,
    };
  }, [profileContext?.profileId, savedChart]);

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
              <div className="flex flex-wrap items-center gap-3">
                <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-primary">
                  Carta abierta
                </div>
                {profileContext && (
                  <div className="inline-flex items-center gap-2 rounded-full border border-white/10 bg-white/[0.04] px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.22em] text-white/84">
                    <Sparkles className="h-3.5 w-3.5 text-primary" />
                    Perfil rehidratado
                  </div>
                )}
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

            {profileContext && associatedProfileSummary && (
              <div className="rounded-[1.75rem] border border-primary/14 bg-[linear-gradient(135deg,rgba(212,175,55,0.08),rgba(255,255,255,0.025))] p-5 shadow-[0_0_24px_rgba(0,0,0,0.16)]">
                <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
                  <div className="max-w-3xl">
                    <p className="text-xs font-semibold uppercase tracking-[0.26em] text-primary/90">
                      Contexto recuperado
                    </p>
                    <p className="mt-2 text-base text-white">
                      Esta carta conserva el perfil enriquecido asociado de <span className="font-medium">{associatedProfileSummary.fullName}</span>.
                    </p>
                    <p className="mt-2 text-sm leading-6 text-text-muted">
                      La lectura que ves aquí ya no depende de memoria local: su contexto viene del vínculo persistido entre carta guardada y perfil.
                    </p>
                  </div>

                  <Link
                    to="/chart/calculate"
                    state={{ profileContext }}
                    className="inline-flex items-center justify-center rounded-full border border-primary/20 bg-primary/10 px-4 py-2 text-sm font-medium text-primary transition hover:bg-primary/15 hover:text-primary-hover"
                  >
                    Recalcular con este perfil
                  </Link>
                </div>

                <div className="mt-5 grid gap-4 md:grid-cols-3">
                  <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
                    <p className="text-[11px] font-semibold uppercase tracking-[0.2em] text-primary/80">Qué le define hoy</p>
                    <p className="mt-2 text-sm leading-6 text-white/88">{associatedProfileSummary.selfPerceptionFocus}</p>
                  </div>
                  <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
                    <p className="text-[11px] font-semibold uppercase tracking-[0.2em] text-primary/80">Qué le cuesta hoy</p>
                    <p className="mt-2 text-sm leading-6 text-white/88">{associatedProfileSummary.currentChallenge}</p>
                  </div>
                  <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
                    <p className="text-[11px] font-semibold uppercase tracking-[0.2em] text-primary/80">Qué busca entender</p>
                    <p className="mt-2 text-sm leading-6 text-white/88">{associatedProfileSummary.desiredInsight}</p>
                  </div>
                </div>
              </div>
            )}

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
