export interface Sale {
  saleId: string;
  saleDate: string;
  buyerName: string;
  quantityLitres: number;
  ratePerLitre: number;
  totalAmount: number;
  paymentStatus: string;
  paymentMode?: string | null;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateSaleRequest {
  saleId?: string | null;
  saleDate: string;
  buyerName: string;
  quantityLitres: number;
  ratePerLitre: number;
  paymentStatus: string;
  paymentMode?: string | null;
  notes?: string | null;
}

export interface UpdatePaymentStatusRequest {
  paymentStatus: string;
  paymentMode?: string | null;
}
