export interface DashboardSummary {
  date: string;
  totalMilkLitres: number;
  totalIncome: number;
  totalExpense: number;
  profitOrLoss: number;
  activeAnimals: number;
  sickAnimals: number;
  dueReminders: number;
  lowStockItems: number;
  pendingPayments: number;
}

export interface MonthlyReport {
  year: number;
  month: number;
  startDate: string;
  endDate: string;
  totalMilkLitres: number;
  milkValue: number;
  totalIncome: number;
  totalExpense: number;
  profitOrLoss: number;
  salesCount: number;
}

export interface CategoryAmount {
  category: string;
  amount: number;
}

export interface ProfitLossReport {
  startDate: string;
  endDate: string;
  totalIncome: number;
  totalExpense: number;
  profitOrLoss: number;
  incomeByCategory: CategoryAmount[];
  expenseByCategory: CategoryAmount[];
}

export interface PerAnimalYield {
  animalId: string;
  tagNumber: string;
  animalName?: string | null;
  status?: string | null;
  totalMilkLitres: number;
  totalValue: number;
  daysRecorded: number;
  avgDailyYield: number;
}
