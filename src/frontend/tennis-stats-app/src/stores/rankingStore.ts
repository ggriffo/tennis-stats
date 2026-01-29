import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Ranking } from '@/types'
import { rankingService } from '@/services/rankingService'

export const useRankingStore = defineStore('ranking', () => {
  // State
  const rankings = ref<Ranking[]>([])
  const playerRankingHistory = ref<Ranking[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const currentAssociation = ref<string>('WTA')

  // Getters
  const hasRankings = computed(() => rankings.value.length > 0)
  const topTen = computed(() => rankings.value.filter((r) => r.rank <= 10))

  // Actions
  async function fetchCurrentRankings(association: string = 'WTA', count: number = 100) {
    loading.value = true
    error.value = null
    currentAssociation.value = association

    try {
      rankings.value = await rankingService.getCurrentRankings(association, count)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch rankings'
      console.error('Error fetching rankings:', err)
    } finally {
      loading.value = false
    }
  }

  async function fetchPlayerRankingHistory(playerId: number) {
    loading.value = true
    error.value = null

    try {
      playerRankingHistory.value = await rankingService.getPlayerRankingHistory(playerId)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch ranking history'
      console.error('Error fetching ranking history:', err)
    } finally {
      loading.value = false
    }
  }

  function setAssociation(association: string) {
    currentAssociation.value = association
  }

  return {
    // State
    rankings,
    playerRankingHistory,
    loading,
    error,
    currentAssociation,
    // Getters
    hasRankings,
    topTen,
    // Actions
    fetchCurrentRankings,
    fetchPlayerRankingHistory,
    setAssociation,
  }
})
