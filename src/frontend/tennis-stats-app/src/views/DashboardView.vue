<script setup lang="ts">
import { onMounted } from 'vue'
import { usePlayerStore } from '@/stores/playerStore'
import { useRankingStore } from '@/stores/rankingStore'

const props = defineProps<{
  association: string
}>()

const playerStore = usePlayerStore()
const rankingStore = useRankingStore()

onMounted(async () => {
  await Promise.all([
    playerStore.fetchTopPlayers(props.association, 10),
    rankingStore.fetchCurrentRankings(props.association, 10),
  ])
})
</script>

<template>
  <div>
    <!-- Welcome Section -->
    <v-row class="mb-6">
      <v-col cols="12">
        <v-card color="primary" variant="tonal">
          <v-card-title class="text-h4">
            <v-icon class="mr-2">mdi-tennis</v-icon>
            Welcome to Tennis Statistics
          </v-card-title>
          <v-card-subtitle class="text-h6">
            Your comprehensive source for {{ association }} tennis data and analytics
          </v-card-subtitle>
          <v-card-text>
            Track player rankings, tournament results, match statistics, and more. Currently viewing
            <strong>{{ association }}</strong> data.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Stats Cards -->
    <v-row class="mb-6">
      <v-col cols="12" md="3">
        <v-card>
          <v-card-text class="text-center">
            <v-icon size="48" color="primary">mdi-account-group</v-icon>
            <div class="text-h4 mt-2">{{ playerStore.players.length }}</div>
            <div class="text-subtitle-1">Players</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="3">
        <v-card>
          <v-card-text class="text-center">
            <v-icon size="48" color="secondary">mdi-trophy</v-icon>
            <div class="text-h4 mt-2">{{ rankingStore.rankings.length }}</div>
            <div class="text-subtitle-1">Rankings</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="3">
        <v-card>
          <v-card-text class="text-center">
            <v-icon size="48" color="accent">mdi-tournament</v-icon>
            <div class="text-h4 mt-2">-</div>
            <div class="text-subtitle-1">Tournaments</div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" md="3">
        <v-card>
          <v-card-text class="text-center">
            <v-icon size="48" color="info">mdi-tennis</v-icon>
            <div class="text-h4 mt-2">-</div>
            <div class="text-subtitle-1">Matches</div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Top Rankings Preview -->
    <v-row>
      <v-col cols="12" md="6">
        <v-card>
          <v-card-title>
            <v-icon class="mr-2">mdi-trophy</v-icon>
            Top 10 Rankings
          </v-card-title>
          <v-divider></v-divider>
          <v-card-text v-if="rankingStore.loading">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </v-card-text>
          <v-list v-else-if="rankingStore.rankings.length > 0" lines="two">
            <v-list-item
              v-for="ranking in rankingStore.rankings.slice(0, 10)"
              :key="ranking.id"
            >
              <template v-slot:prepend>
                <v-avatar color="primary" size="36">
                  <span class="text-white font-weight-bold">{{ ranking.rank }}</span>
                </v-avatar>
              </template>
              <v-list-item-title>{{ ranking.playerName || 'Unknown Player' }}</v-list-item-title>
              <v-list-item-subtitle>
                {{ ranking.country }} • {{ ranking.points.toLocaleString() }} points
              </v-list-item-subtitle>
              <template v-slot:append>
                <v-chip
                  v-if="ranking.rankChange"
                  :color="ranking.rankChange > 0 ? 'success' : ranking.rankChange < 0 ? 'error' : 'grey'"
                  size="small"
                >
                  <v-icon size="small">
                    {{ ranking.rankChange > 0 ? 'mdi-arrow-up' : ranking.rankChange < 0 ? 'mdi-arrow-down' : 'mdi-minus' }}
                  </v-icon>
                  {{ Math.abs(ranking.rankChange) }}
                </v-chip>
              </template>
            </v-list-item>
          </v-list>
          <v-card-text v-else class="text-center text-grey">
            No ranking data available. Try syncing data first.
          </v-card-text>
          <v-card-actions>
            <v-btn color="primary" variant="text" to="/rankings">View All Rankings</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- Top Players Preview -->
      <v-col cols="12" md="6">
        <v-card>
          <v-card-title>
            <v-icon class="mr-2">mdi-account-group</v-icon>
            Featured Players
          </v-card-title>
          <v-divider></v-divider>
          <v-card-text v-if="playerStore.loading">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </v-card-text>
          <v-list v-else-if="playerStore.players.length > 0" lines="two">
            <v-list-item
              v-for="player in playerStore.players.slice(0, 10)"
              :key="player.id"
              :to="`/players/${player.id}`"
            >
              <template v-slot:prepend>
                <v-avatar color="grey-lighten-2">
                  <v-img v-if="player.imageUrl" :src="player.imageUrl" :alt="player.fullName"></v-img>
                  <v-icon v-else>mdi-account</v-icon>
                </v-avatar>
              </template>
              <v-list-item-title>{{ player.fullName }}</v-list-item-title>
              <v-list-item-subtitle>
                {{ player.country }} 
                <span v-if="player.currentRank"> • Rank #{{ player.currentRank }}</span>
              </v-list-item-subtitle>
            </v-list-item>
          </v-list>
          <v-card-text v-else class="text-center text-grey">
            No player data available. Try syncing data first.
          </v-card-text>
          <v-card-actions>
            <v-btn color="primary" variant="text" to="/players">View All Players</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>
