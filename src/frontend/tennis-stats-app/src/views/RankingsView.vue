<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { useRankingStore } from '@/stores/rankingStore'

const props = defineProps<{
  association: string
}>()

const rankingStore = useRankingStore()

async function loadRankings() {
  await rankingStore.fetchCurrentRankings(props.association, 100)
}

onMounted(loadRankings)

watch([() => props.association], loadRankings)
</script>

<template>
  <div>
    <v-row class="mb-4">
      <v-col cols="12">
        <v-card>
          <v-card-title class="text-h5">
            <v-icon class="mr-2">mdi-trophy</v-icon>
            {{ association }} Rankings
          </v-card-title>
          <v-card-subtitle>
            Current world rankings
          </v-card-subtitle>
        </v-card>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-row v-if="rankingStore.loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <p class="mt-4">Loading rankings...</p>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-alert v-else-if="rankingStore.error" type="error" class="mb-4">
      {{ rankingStore.error }}
    </v-alert>

    <!-- Rankings Table -->
    <v-card v-else-if="rankingStore.rankings.length > 0">
      <v-table>
        <thead>
          <tr>
            <th class="text-left">Rank</th>
            <th class="text-left">Player</th>
            <th class="text-left">Country</th>
            <th class="text-right">Points</th>
            <th class="text-center">Change</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="ranking in rankingStore.rankings" :key="ranking.id">
            <td>
              <v-chip
                :color="ranking.rank <= 10 ? 'primary' : 'default'"
                size="small"
              >
                {{ ranking.rank }}
              </v-chip>
            </td>
            <td>
              <router-link
                v-if="ranking.playerId"
                :to="`/players/${ranking.playerId}`"
                class="text-decoration-none"
              >
                {{ ranking.playerName || 'Unknown Player' }}
              </router-link>
              <span v-else>{{ ranking.playerName || 'Unknown Player' }}</span>
            </td>
            <td>{{ ranking.country || '-' }}</td>
            <td class="text-right font-weight-bold">
              {{ ranking.points.toLocaleString() }}
            </td>
            <td class="text-center">
              <v-chip
                v-if="ranking.rankChange !== undefined && ranking.rankChange !== null"
                :color="ranking.rankChange > 0 ? 'success' : ranking.rankChange < 0 ? 'error' : 'grey'"
                size="small"
                variant="outlined"
              >
                <v-icon size="small" start>
                  {{ ranking.rankChange > 0 ? 'mdi-arrow-up' : ranking.rankChange < 0 ? 'mdi-arrow-down' : 'mdi-minus' }}
                </v-icon>
                {{ Math.abs(ranking.rankChange) }}
              </v-chip>
              <span v-else>-</span>
            </td>
          </tr>
        </tbody>
      </v-table>
    </v-card>

    <!-- Empty State -->
    <v-row v-else>
      <v-col cols="12" class="text-center">
        <v-icon size="64" color="grey">mdi-trophy-outline</v-icon>
        <p class="text-h6 text-grey mt-4">No rankings available</p>
        <p class="text-grey">Rankings data needs to be synced from the API.</p>
      </v-col>
    </v-row>
  </div>
</template>
