import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Player, PlayerDetail, PaginatedList } from '@/types'
import { playerService, type GetPlayersParams } from '@/services/playerService'

export const usePlayerStore = defineStore('player', () => {
  // State
  const players = ref<Player[]>([])
  const currentPlayer = ref<PlayerDetail | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const pagination = ref({
    pageNumber: 1,
    totalPages: 1,
    totalCount: 0,
    pageSize: 20,
    hasPreviousPage: false,
    hasNextPage: false,
  })

  // Getters
  const hasPlayers = computed(() => players.value.length > 0)
  const topPlayers = computed(() => players.value.slice(0, 10))

  // Actions
  async function fetchPlayers(params: GetPlayersParams = {}) {
    loading.value = true
    error.value = null

    try {
      const response = await playerService.getPlayers(params)
      players.value = response.items
      pagination.value = {
        pageNumber: response.pageNumber,
        totalPages: response.totalPages,
        totalCount: response.totalCount,
        pageSize: response.pageSize,
        hasPreviousPage: response.hasPreviousPage,
        hasNextPage: response.hasNextPage,
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch players'
      console.error('Error fetching players:', err)
    } finally {
      loading.value = false
    }
  }

  async function fetchPlayer(id: number) {
    loading.value = true
    error.value = null

    try {
      currentPlayer.value = await playerService.getPlayer(id)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch player'
      console.error('Error fetching player:', err)
    } finally {
      loading.value = false
    }
  }

  async function fetchTopPlayers(association: string = 'WTA', count: number = 10) {
    loading.value = true
    error.value = null

    try {
      players.value = await playerService.getTopPlayers(association, count)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch top players'
      console.error('Error fetching top players:', err)
    } finally {
      loading.value = false
    }
  }

  function clearCurrentPlayer() {
    currentPlayer.value = null
  }

  return {
    // State
    players,
    currentPlayer,
    loading,
    error,
    pagination,
    // Getters
    hasPlayers,
    topPlayers,
    // Actions
    fetchPlayers,
    fetchPlayer,
    fetchTopPlayers,
    clearCurrentPlayer,
  }
})
