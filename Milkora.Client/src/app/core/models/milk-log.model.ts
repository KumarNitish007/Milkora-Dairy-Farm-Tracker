export interface MilkLog {
  logId: string;
  animalId?: string | null;
  tagNumber?: string | null;
  animalName?: string | null;
  logDate: string;
  morningMilk: number;
  eveningMilk: number;
  fatPercent?: number | null;
  ratePerLitre?: number | null;
  totalMilk: number;
  totalValue: number;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateMilkLogRequest {
  logId?: string | null;
  animalId?: string | null;
  logDate: string;
  morningMilk: number;
  eveningMilk: number;
  fatPercent?: number | null;
  ratePerLitre?: number | null;
  notes?: string | null;
}

export type UpdateMilkLogRequest = Omit<CreateMilkLogRequest, 'logId'>;
