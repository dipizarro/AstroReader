export interface CreatePersonalProfileRequest {
  fullName: string;
  birthDate: string;
  birthTime: string;
  birthPlace: string;
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  selfPerceptionFocus: string;
  currentChallenge: string;
  desiredInsight: string;
  selfDescription?: string;
  userId?: string | null;
  savedChartId?: string | null;
}

export interface PersonalProfileDetail {
  id: string;
  userId?: string | null;
  savedChartId?: string | null;
  fullName: string;
  birthDate: string;
  birthTime: string;
  birthInstantUtc: string;
  birthPlace: string;
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  selfPerceptionFocus: string;
  currentChallenge: string;
  desiredInsight: string;
  selfDescription?: string | null;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface PersonalProfileListItem {
  id: string;
  savedChartId?: string | null;
  fullName: string;
  birthDate: string;
  birthTime: string;
  birthPlace: string;
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
  selfPerceptionFocus: string;
  createdAtUtc: string;
}

export interface PersonalProfileChartContext {
  profileId: string;
  fullName: string;
  birthDate: string;
  birthTime: string;
  birthPlace: string;
  latitude: number;
  longitude: number;
  timezoneOffsetMinutes: number;
}
