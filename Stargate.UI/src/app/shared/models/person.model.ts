export interface PersonRequest {
  id: number;
  name: string;
}

export interface PersonResponse {
  personId: number;
  name: string;
  currentRank?: string;
  currentDutyTitle?: string;
  careerStartDate?: string;
  careerEndDate?: string;
  success?: boolean;
  message?: string;
  responseCode?: number;
}

export interface PersonListResponse {
  people: PersonResponse[];
  success: boolean;
  message: string;
  responseCode: number;
}
