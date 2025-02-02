import { NeighborhoodDto } from './neighborhood-dto';

export interface CityDto {
  id: number;
  name: string;
  neighborhoods?: NeighborhoodDto[];
}
