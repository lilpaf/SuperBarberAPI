export interface BarbershopsListRequest {
  city: string;
  neighborhood?: string;
  barberShopName?: string;
  date: string;
  currentPage: number;
}
