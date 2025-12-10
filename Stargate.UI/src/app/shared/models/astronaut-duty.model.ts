import { BaseResponse } from './base-response.model';

export interface CreateAstronautDutyRequest {
  name: string;
  rank: string;
  dutyTitle: string;
  dutyStartDate: Date | string;
}

export interface AstronautDutyRecord {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: Date | string;
  dutyEndDate?: Date | string;
}

export interface PersonInfo {
  id: number;
  name: string;
  currentRank: string;
  currentDutyTitle: string;
  careerStartDate?: Date | string;
  careerEndDate?: Date | string;
}

export interface PersonWithDuties {
  person: PersonInfo;
  astronautDuties: AstronautDutyRecord[];
}

export interface AstronautDuty extends BaseResponse {
  id?: number;
  name: string;
  rank: string;
  dutyTitle: string;
  dutyStartDate: Date | string;
  dutyEndDate?: Date | string;
}
