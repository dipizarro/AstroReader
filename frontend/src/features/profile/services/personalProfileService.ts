import { apiClient } from '../../../core/services/apiClient';
import type { CreatePersonalProfileRequest, PersonalProfileDetail, PersonalProfileListItem } from '../types/profile.types';

export const personalProfileService = {
  async getProfiles(): Promise<PersonalProfileListItem[]> {
    try {
      return await apiClient.get<PersonalProfileListItem[]>('/api/PersonalProfiles');
    } catch (error) {
      console.error('Error al obtener la lista de perfiles personales:', error);
      throw error;
    }
  },

  async createProfile(request: CreatePersonalProfileRequest): Promise<PersonalProfileDetail> {
    try {
      return await apiClient.post<PersonalProfileDetail, CreatePersonalProfileRequest>(
        '/api/PersonalProfiles',
        request,
      );
    } catch (error) {
      console.error('Error al crear el perfil personal:', error);
      throw error;
    }
  },

  async getProfileById(id: string): Promise<PersonalProfileDetail> {
    try {
      return await apiClient.get<PersonalProfileDetail>(`/api/PersonalProfiles/${id}`);
    } catch (error) {
      console.error('Error al obtener el perfil personal:', error);
      throw error;
    }
  },

  async getProfileBySavedChartId(savedChartId: string): Promise<PersonalProfileDetail> {
    try {
      return await apiClient.get<PersonalProfileDetail>(`/api/PersonalProfiles/by-saved-chart/${savedChartId}`);
    } catch (error) {
      console.error('Error al obtener el perfil asociado a la carta guardada:', error);
      throw error;
    }
  },
};
