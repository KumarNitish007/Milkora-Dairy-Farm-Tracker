export interface Animal {
  animalId: string;
  tagNumber: string;
  name?: string | null;
  type?: string | null;
  breed?: string | null;
  gender?: string | null;
  dateOfBirth?: string | null;
  purchaseDate?: string | null;
  purchasePrice?: number | null;
  status: string;
  dailyMilkYield?: number | null;
  photoPath?: string | null;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAnimalRequest {
  animalId?: string | null;
  tagNumber: string;
  name?: string | null;
  type?: string | null;
  breed?: string | null;
  gender?: string | null;
  dateOfBirth?: string | null;
  purchaseDate?: string | null;
  purchasePrice?: number | null;
  status: string;
  dailyMilkYield?: number | null;
  photoPath?: string | null;
  notes?: string | null;
}

export type UpdateAnimalRequest = Omit<CreateAnimalRequest, 'animalId'>;
