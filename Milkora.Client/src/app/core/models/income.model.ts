export interface Income {
  incomeId: string;
  incomeDate: string;
  category: string;
  description?: string | null;
  amount: number;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateIncomeRequest {
  incomeId?: string | null;
  incomeDate: string;
  category: string;
  description?: string | null;
  amount: number;
  notes?: string | null;
}
