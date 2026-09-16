export interface HealthRecord {
  healthId: string;
  animalId?: string | null;
  tagNumber?: string | null;
  animalName?: string | null;
  recordDate: string;
  recordType: string;
  medicineName?: string | null;
  vetName?: string | null;
  vetContact?: string | null;
  cost?: number | null;
  nextDueDate?: string | null;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface DueReminder {
  healthId: string;
  animalId?: string | null;
  tagNumber?: string | null;
  animalName?: string | null;
  recordType: string;
  medicineName?: string | null;
  nextDueDate?: string | null;
  daysUntilDue: number;
}

export interface CreateHealthRecordRequest {
  healthId?: string | null;
  animalId?: string | null;
  recordDate: string;
  recordType: string;
  medicineName?: string | null;
  vetName?: string | null;
  vetContact?: string | null;
  cost?: number | null;
  nextDueDate?: string | null;
  notes?: string | null;
}
