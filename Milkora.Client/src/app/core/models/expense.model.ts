export interface Expense {
  expenseId: string;
  expenseDate: string;
  category: string;
  description?: string | null;
  amount: number;
  paymentMode?: string | null;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateExpenseRequest {
  expenseId?: string | null;
  expenseDate: string;
  category: string;
  description?: string | null;
  amount: number;
  paymentMode?: string | null;
  notes?: string | null;
}

export type UpdateExpenseRequest = Omit<CreateExpenseRequest, 'expenseId'>;
