export interface ForecastPoint {
  step: number;
  date: string;
  value: number;
  lowerBound: number;
  upperBound: number;
}

export interface MilkForecast {
  days: number;
  projectedTotal: number;
  dailyAverage: number;
  basis: string;              // 'ssa' | 'average' | 'none'
  message?: string | null;
  points: ForecastPoint[];
}

export interface MilkAnomaly {
  animalId?: string | null;
  tagNumber?: string | null;
  animalName?: string | null;
  date: string;
  value: number;
  expected: number;
  dropPercent: number;
  severity: string;           // Low | Medium | High
}

export interface AnomalyReport {
  animalsAnalysed: number;
  message?: string | null;
  anomalies: MilkAnomaly[];
}

export interface MilkHealth {
  animalId: string;
  tagNumber?: string | null;
  animalName?: string | null;
  status: string;             // Healthy | Watch | AtRisk | Unknown
  score: number;
  reasons: string[];
  baselineAvg: number;
  recentAvg: number;
  trendPercent: number;
  dropPercent: number;
}
