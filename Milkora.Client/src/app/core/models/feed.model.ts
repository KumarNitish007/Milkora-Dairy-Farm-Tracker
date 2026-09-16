export interface FeedItem {
  inventoryId: string;
  itemName: string;
  quantityInStock: number;
  unit?: string | null;
  purchaseDate?: string | null;
  cost?: number | null;
  dailyUsage?: number | null;
  minimumLevel: number;
  isLowStock: boolean;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFeedItemRequest {
  inventoryId?: string | null;
  itemName: string;
  quantityInStock: number;
  unit?: string | null;
  purchaseDate?: string | null;
  cost?: number | null;
  dailyUsage?: number | null;
  minimumLevel: number;
  notes?: string | null;
}

export interface UpdateStockRequest {
  quantityInStock: number;
}
