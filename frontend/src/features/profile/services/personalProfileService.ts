import { apiClient } from '../../../core/services/apiClient';
import type { CreatePersonalProfileRequest, PersonalProfileDetail } from '../types/profile.types';

export const personalProfileService = {
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
};
