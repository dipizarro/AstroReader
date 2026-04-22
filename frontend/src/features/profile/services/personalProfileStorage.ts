import type { PersonalProfileDetail } from '../types/profile.types';

const LAST_PROFILE_STORAGE_KEY = 'astroreader:last-personal-profile';

export const personalProfileStorage = {
  saveLastProfile(profile: PersonalProfileDetail): void {
    try {
      window.localStorage.setItem(LAST_PROFILE_STORAGE_KEY, JSON.stringify(profile));
    } catch (error) {
      console.warn('No pudimos persistir el último perfil personal en localStorage.', error);
    }
  },

  getLastProfile(): PersonalProfileDetail | null {
    try {
      const serialized = window.localStorage.getItem(LAST_PROFILE_STORAGE_KEY);
      if (!serialized) {
        return null;
      }

      return JSON.parse(serialized) as PersonalProfileDetail;
    } catch (error) {
      console.warn('No pudimos leer el último perfil personal desde localStorage.', error);
      return null;
    }
  },
};
