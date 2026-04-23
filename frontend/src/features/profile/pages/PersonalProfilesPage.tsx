import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ArrowUpRight, RefreshCw, Sparkles, UserRound } from 'lucide-react';
import { personalProfileService } from '../services/personalProfileService';
import { personalProfileStorage } from '../services/personalProfileStorage';
import type { PersonalProfileListItem } from '../types/profile.types';
import { formatBirthDate, formatSavedDate } from '../../chart/utils/chartFormatters';

export const PersonalProfilesPage = () => {
  const navigate = useNavigate();
  const [profiles, setProfiles] = useState<PersonalProfileListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadProfiles = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await personalProfileService.getProfiles();
      setProfiles(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No pudimos cargar tus perfiles personales.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadProfiles();
  }, []);

  const handleResumeProfile = (profile: PersonalProfileListItem) => {
    const profileContext = personalProfileStorage.buildChartContextFromListItem(profile);
    personalProfileStorage.savePendingChartContext(profileContext);
    navigate('/chart/calculate', { state: { profileContext } });
  };

  return (
    <div className="min-h-screen px-6 py-10 lg:px-12 lg:py-14">
      <div className="mx-auto max-w-6xl">
        <section className="relative overflow-hidden rounded-[2rem] border border-white/10 bg-gradient-to-br from-[#141418] via-[#101014] to-[#09090b] px-6 py-8 shadow-[0_0_48px_rgba(0,0,0,0.32)] sm:px-8 sm:py-10">
          <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-primary/40 to-transparent" />
          <div className="absolute -right-16 top-0 h-48 w-48 rounded-full bg-primary/10 blur-3xl" />

          <div className="relative z-10 flex flex-col gap-8 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-2xl space-y-4">
              <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-primary">
                <UserRound className="h-3.5 w-3.5" />
                Biblioteca de perfiles
              </div>
              <div className="space-y-3">
                <h1 className="font-display text-4xl font-semibold tracking-tight text-glow sm:text-5xl">
                  Tus perfiles personales
                </h1>
                <p className="max-w-xl text-base leading-7 text-text-muted">
                  Retoma un perfil ya creado y vuelve al cálculo con contexto persistido. Si ese perfil ya tiene una carta asociada, también puedes abrir directamente su lectura.
                </p>
              </div>
            </div>

            <div className="flex flex-col gap-3 sm:flex-row">
              <button
                type="button"
                onClick={() => void loadProfiles()}
                disabled={loading}
                className="inline-flex items-center justify-center gap-2 rounded-full border border-white/10 bg-white/[0.04] px-5 py-3 text-sm font-medium text-white transition-all hover:bg-white/[0.08] disabled:cursor-not-allowed disabled:opacity-60"
              >
                <RefreshCw className={`h-4 w-4 ${loading ? 'animate-spin' : ''}`} />
                Recargar
              </button>

              <Link
                to="/profile/onboarding"
                className="inline-flex items-center justify-center gap-2 rounded-full bg-primary px-5 py-3 text-sm font-semibold text-[#0a0a0b] shadow-[0_0_18px_rgba(212,175,55,0.28)] transition-all hover:bg-primary-hover hover:shadow-[0_0_26px_rgba(212,175,55,0.4)]"
              >
                <Sparkles className="h-4 w-4" />
                Crear nuevo perfil
              </Link>
            </div>
          </div>
        </section>

        <section className="mt-10">
          {loading ? (
            <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
              {Array.from({ length: 4 }).map((_, index) => (
                <div key={index} className="h-64 animate-pulse rounded-3xl bg-white/[0.04]" />
              ))}
            </div>
          ) : error ? (
            <div className="rounded-3xl border border-red-500/20 bg-red-500/10 p-6 text-center">
              <h2 className="text-lg font-medium text-red-200">No pudimos abrir tus perfiles</h2>
              <p className="mt-2 text-sm text-red-200/80">{error}</p>
              <button
                type="button"
                onClick={() => void loadProfiles()}
                className="mt-5 inline-flex items-center justify-center rounded-full border border-red-300/20 bg-red-400/10 px-5 py-2.5 text-sm font-medium text-red-100 transition hover:bg-red-400/20"
              >
                Reintentar
              </button>
            </div>
          ) : profiles.length === 0 ? (
            <div className="rounded-[2rem] border border-white/8 bg-gradient-to-b from-[#121216] to-[#0c0c0f] p-10 text-center">
              <div className="mx-auto mb-5 flex h-14 w-14 items-center justify-center rounded-full bg-primary/10">
                <UserRound className="h-6 w-6 text-primary" />
              </div>
              <h2 className="text-2xl font-display text-white">Aún no hay perfiles creados</h2>
              <p className="mx-auto mt-3 max-w-lg text-sm leading-6 text-text-muted">
                Crea tu primer perfil enriquecido para volver luego a tu carta con un contexto más humano y persistido.
              </p>
              <Link
                to="/profile/onboarding"
                className="mt-6 inline-flex items-center justify-center gap-2 rounded-full bg-primary px-6 py-3 text-sm font-semibold text-[#0a0a0b] shadow-[0_0_18px_rgba(212,175,55,0.28)] transition-all hover:bg-primary-hover hover:shadow-[0_0_26px_rgba(212,175,55,0.4)]"
              >
                <Sparkles className="h-4 w-4" />
                Crear primer perfil
              </Link>
            </div>
          ) : (
            <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
              {profiles.map((profile) => (
                <article
                  key={profile.id}
                  className="group relative overflow-hidden rounded-3xl border border-white/10 bg-gradient-to-br from-[#141419] via-[#101014] to-[#0b0b0d] p-6 shadow-[0_0_32px_rgba(0,0,0,0.28)] transition-all hover:-translate-y-1 hover:border-primary/20 hover:shadow-[0_0_42px_rgba(0,0,0,0.4)]"
                >
                  <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-primary/35 to-transparent" />
                  <div className="absolute -right-16 top-6 h-28 w-28 rounded-full bg-primary/10 blur-3xl transition-opacity group-hover:opacity-100" />

                  <div className="relative z-10 flex h-full flex-col gap-6">
                    <div className="space-y-3">
                      <div className="inline-flex items-center gap-2 rounded-full border border-primary/15 bg-primary/10 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.24em] text-primary">
                        <UserRound className="h-3.5 w-3.5" />
                        Perfil persistido
                      </div>
                      <div>
                        <h3 className="font-display text-2xl font-medium text-white">{profile.fullName}</h3>
                        <p className="mt-2 text-sm leading-6 text-text-muted">
                          {profile.selfPerceptionFocus}
                        </p>
                      </div>
                    </div>

                    <div className="grid gap-3 text-sm text-text-muted sm:grid-cols-2">
                      <div className="rounded-2xl border border-white/6 bg-white/[0.03] p-4">
                        <div className="mb-2 text-xs uppercase tracking-[0.2em] text-primary/80">Nacimiento</div>
                        <p className="text-white">{formatBirthDate(profile.birthDate)}</p>
                        <p className="mt-1 text-xs text-text-muted">{profile.birthTime}</p>
                      </div>

                      <div className="rounded-2xl border border-white/6 bg-white/[0.03] p-4">
                        <div className="mb-2 text-xs uppercase tracking-[0.2em] text-primary/80">Lugar</div>
                        <p className="text-white">{profile.birthPlace}</p>
                      </div>
                    </div>

                    <div className="mt-auto flex flex-col gap-3 border-t border-white/6 pt-4">
                      <div className="text-sm text-text-muted">
                        Creado el <span className="text-white">{formatSavedDate(profile.createdAtUtc)}</span>
                      </div>

                      <div className="flex flex-col gap-3 sm:flex-row">
                        <button
                          type="button"
                          onClick={() => handleResumeProfile(profile)}
                          className="inline-flex items-center justify-center gap-2 rounded-full bg-primary px-4 py-2.5 text-sm font-semibold text-[#0a0a0b] transition-all hover:bg-primary-hover"
                        >
                          <Sparkles className="h-4 w-4" />
                          Retomar cálculo
                        </button>

                        {profile.savedChartId ? (
                          <Link
                            to={`/charts/saved/${profile.savedChartId}`}
                            className="inline-flex items-center justify-center gap-2 rounded-full border border-white/10 bg-white/[0.04] px-4 py-2.5 text-sm font-medium text-white transition hover:bg-white/[0.08]"
                          >
                            <ArrowUpRight className="h-4 w-4" />
                            Abrir lectura
                          </Link>
                        ) : (
                          <span className="inline-flex items-center justify-center rounded-full border border-white/8 bg-white/[0.03] px-4 py-2.5 text-sm text-text-muted">
                            Aún sin carta asociada
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                </article>
              ))}
            </div>
          )}
        </section>
      </div>
    </div>
  );
};
