export interface BreedingRecord {
  breedingId: string;
  animalId?: string | null;
  tagNumber?: string | null;
  animalName?: string | null;
  inseminationDate: string;
  bullDetails?: string | null;
  pregnancyStatus: string;
  expectedDeliveryDate?: string | null;
  calvingDate?: string | null;
  calfDetails?: string | null;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBreedingRequest {
  breedingId?: string | null;
  animalId?: string | null;
  inseminationDate: string;
  bullDetails?: string | null;
  pregnancyStatus: string;
  notes?: string | null;
}

export interface UpdateCalvingRequest {
  calvingDate: string;
  calfDetails?: string | null;
  pregnancyStatus: string;
}
