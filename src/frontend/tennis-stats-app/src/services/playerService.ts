import apiClient from './api'
import type { Player, PlayerDetail, PaginatedList, Association } from '@/types'

export interface GetPlayersParams {
  association?: string
  page?: number
  pageSize?: number
  search?: string
  country?: string
}

export const playerService = {
  async getPlayers(params: GetPlayersParams = {}): Promise<PaginatedList<Player>> {
    const response = await apiClient.get<PaginatedList<Player>>('/players', { params })
    return response.data
  },

  async getPlayer(id: number): Promise<PlayerDetail> {
    const response = await apiClient.get<PlayerDetail>(`/players/${id}`)
    return response.data
  },

  async getTopPlayers(association: string = 'WTA', count: number = 10): Promise<Player[]> {
    const response = await apiClient.get<Player[]>('/players/top', {
      params: { association, count },
    })
    return response.data
  },
}
