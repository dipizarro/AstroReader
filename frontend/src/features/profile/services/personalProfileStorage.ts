import type { PersonalProfileChartContext, PersonalProfileDetail, PersonalProfileListItem } from '../types/profile.types';

const PENDING_CHART_CONTEXT_STORAGE_KEY = 'astroreader:pending-chart-profile-context';
const PENDING_CHART_CONTEXT_TTL_MS = 1000 * 60 * 60 * 2;

interface PendingChartContextEnvelope {
  context: PersonalProfileChartContext;
  savedAtUtc: string;
}

const buildChartContext = (profile: PersonalProfileDetail): PersonalProfileChartContext => ({
  profileId: profile.id,
  fullName: profile.fullName,
  birthDate: profile.birthDate,
  birthTime: profile.birthTime,
  birthPlace: profile.birthPlace,
  latitude: profile.latitude,
  longitude: profile.longitude,
  timezoneOffsetMinutes: profile.timezoneOffsetMinutes,
});

const buildChartContextFromListItem = (profile: PersonalProfileListItem): PersonalProfileChartContext => ({
  profileId: profile.id,
  fullName: profile.fullName,
  birthDate: profile.birthDate,
  birthTime: profile.birthTime,
  birthPlace: profile.birthPlace,
  latitude: profile.latitude,
  longitude: profile.longitude,
  timezoneOffsetMinutes: profile.timezoneOffsetMinutes,
});

const clearPendingChartContext = (): void => {
  try {
    window.localStorage.removeItem(PENDING_CHART_CONTEXT_STORAGE_KEY);
  } catch (error) {
    console.warn('No pudimos limpiar el contexto temporal del perfil personal.', error);
  }
};

const savePendingChartContext = (context: PersonalProfileChartContext): void => {
  try {
    const payload: PendingChartContextEnvelope = {
      context,
      savedAtUtc: new Date().toISOString(),
    };

    window.localStorage.setItem(PENDING_CHART_CONTEXT_STORAGE_KEY, JSON.stringify(payload));
  } catch (error) {
    console.warn('No pudimos persistir el contexto temporal del perfil personal.', error);
  }
};

export const personalProfileStorage = {
  buildChartContext,
  buildChartContextFromListItem,
  savePendingChartContext,

  savePendingChartContextFromProfile(profile: PersonalProfileDetail): PersonalProfileChartContext {
    const context = buildChartContext(profile);
    savePendingChartContext(context);
    return context;
  },

  getPendingChartContext(): PersonalProfileChartContext | null {
    try {
      const serialized = window.localStorage.getItem(PENDING_CHART_CONTEXT_STORAGE_KEY);
      if (!serialized) {
        return null;
      }

      const payload = JSON.parse(serialized) as PendingChartContextEnvelope;
      const savedAt = Date.parse(payload.savedAtUtc);

      if (!payload.context || Number.isNaN(savedAt)) {
        clearPendingChartContext();
        return null;
      }

      if (Date.now() - savedAt > PENDING_CHART_CONTEXT_TTL_MS) {
        clearPendingChartContext();
        return null;
      }

      return payload.context;
    } catch (error) {
      console.warn('No pudimos leer el contexto temporal del perfil personal.', error);
      return null;
    }
  },
  clearPendingChartContext,
};
