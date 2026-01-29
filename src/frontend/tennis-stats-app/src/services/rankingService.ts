import apiClient from './api'
import type { Ranking } from '@/types'

export const rankingService = {
  async getCurrentRankings(association: string = 'WTA', count: number = 100): Promise<Ranking[]> {
    const response = await apiClient.get<Ranking[]>('/rankings', {
      params: { association, count },
    })
    return response.data
  },

  async getPlayerRankingHistory(playerId: number): Promise<Ranking[]> {
    const response = await apiClient.get<Ranking[]>(`/rankings/player/${playerId}`)
    return response.data
  },
}
