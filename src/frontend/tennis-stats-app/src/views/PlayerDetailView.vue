<script setup lang="ts">
import { onMounted, watch, computed } from 'vue'
import { useRoute } from 'vue-router'
import { usePlayerStore } from '@/stores/playerStore'

const route = useRoute()
const playerStore = usePlayerStore()

const playerId = computed(() => Number(route.params.id))

onMounted(() => {
  playerStore.fetchPlayer(playerId.value)
})

watch(playerId, (newId) => {
  if (newId) {
    playerStore.fetchPlayer(newId)
  }
})

const player = computed(() => playerStore.currentPlayer)
</script>

<template>
  <div>
    <!-- Loading State -->
    <v-row v-if="playerStore.loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <p class="mt-4">Loading player details...</p>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-alert v-else-if="playerStore.error" type="error" class="mb-4">
      {{ playerStore.error }}
      <template v-slot:append>
        <v-btn color="white" variant="text" to="/players">Back to Players</v-btn>
      </template>
    </v-alert>

    <!-- Player Details -->
    <template v-else-if="player">
      <!-- Header Card -->
      <v-row class="mb-4">
        <v-col cols="12">
          <v-card>
            <v-card-text>
              <v-row align="center">
                <v-col cols="12" md="auto" class="text-center">
                  <v-avatar size="150" color="grey-lighten-2">
                    <v-img v-if="player.imageUrl" :src="player.imageUrl" :alt="player.fullName"></v-img>
                    <v-icon v-else size="80">mdi-account</v-icon>
                  </v-avatar>
                </v-col>
                <v-col>
                  <h1 class="text-h3 mb-2">{{ player.fullName }}</h1>
                  <div class="d-flex align-center gap-2 mb-2">
                    <v-chip color="primary" size="small">{{ player.association }}</v-chip>
                    <v-chip v-if="player.country" size="small">{{ player.country }}</v-chip>
                    <v-chip v-if="player.currentRank" color="secondary" size="small">
                      Rank #{{ player.currentRank }}
                    </v-chip>
                  </div>
                  <div v-if="player.currentPoints" class="text-subtitle-1">
                    {{ player.currentPoints.toLocaleString() }} points
                  </div>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Player Info Cards -->
      <v-row class="mb-4">
        <v-col cols="12" md="6">
          <v-card>
            <v-card-title>
              <v-icon class="mr-2">mdi-account-details</v-icon>
              Personal Information
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
              <v-list>
                <v-list-item v-if="player.dateOfBirth">
                  <template v-slot:prepend>
                    <v-icon>mdi-cake-variant</v-icon>
                  </template>
                  <v-list-item-title>Date of Birth</v-list-item-title>
                  <v-list-item-subtitle>
                    {{ new Date(player.dateOfBirth).toLocaleDateString() }}
                    <span v-if="player.age">({{ player.age }} years old)</span>
                  </v-list-item-subtitle>
                </v-list-item>
                <v-list-item v-if="player.country">
                  <template v-slot:prepend>
                    <v-icon>mdi-flag</v-icon>
                  </template>
                  <v-list-item-title>Country</v-list-item-title>
                  <v-list-item-subtitle>{{ player.country }}</v-list-item-subtitle>
                </v-list-item>
                <v-list-item v-if="player.heightCm">
                  <template v-slot:prepend>
                    <v-icon>mdi-human-male-height</v-icon>
                  </template>
                  <v-list-item-title>Height</v-list-item-title>
                  <v-list-item-subtitle>{{ player.heightCm }} cm</v-list-item-subtitle>
                </v-list-item>
                <v-list-item v-if="player.weightKg">
                  <template v-slot:prepend>
                    <v-icon>mdi-weight</v-icon>
                  </template>
                  <v-list-item-title>Weight</v-list-item-title>
                  <v-list-item-subtitle>{{ player.weightKg }} kg</v-list-item-subtitle>
                </v-list-item>
              </v-list>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" md="6">
          <v-card>
            <v-card-title>
              <v-icon class="mr-2">mdi-tennis</v-icon>
              Playing Style
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
              <v-list>
                <v-list-item>
                  <template v-slot:prepend>
                    <v-icon>mdi-hand-wave</v-icon>
                  </template>
                  <v-list-item-title>Playing Hand</v-list-item-title>
                  <v-list-item-subtitle>{{ player.hand }}</v-list-item-subtitle>
                </v-list-item>
                <v-list-item>
                  <template v-slot:prepend>
                    <v-icon>mdi-tennis-racket</v-icon>
                  </template>
                  <v-list-item-title>Backhand</v-list-item-title>
                  <v-list-item-subtitle>{{ player.backhand }}</v-list-item-subtitle>
                </v-list-item>
                <v-list-item v-if="player.turnedProYear">
                  <template v-slot:prepend>
                    <v-icon>mdi-calendar-star</v-icon>
                  </template>
                  <v-list-item-title>Turned Pro</v-list-item-title>
                  <v-list-item-subtitle>{{ player.turnedProYear }}</v-list-item-subtitle>
                </v-list-item>
              </v-list>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Season Stats -->
      <v-row v-if="player.currentSeasonStats" class="mb-4">
        <v-col cols="12">
          <v-card>
            <v-card-title>
              <v-icon class="mr-2">mdi-chart-line</v-icon>
              {{ player.currentSeasonStats.year }} Season Statistics
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
              <v-row>
                <v-col cols="6" md="2" class="text-center">
                  <div class="text-h4 text-primary">{{ player.currentSeasonStats.matchesPlayed }}</div>
                  <div class="text-caption">Matches Played</div>
                </v-col>
                <v-col cols="6" md="2" class="text-center">
                  <div class="text-h4 text-success">{{ player.currentSeasonStats.matchesWon }}</div>
                  <div class="text-caption">Wins</div>
                </v-col>
                <v-col cols="6" md="2" class="text-center">
                  <div class="text-h4 text-error">{{ player.currentSeasonStats.matchesLost }}</div>
                  <div class="text-caption">Losses</div>
                </v-col>
                <v-col cols="6" md="2" class="text-center">
                  <div class="text-h4">{{ player.currentSeasonStats.winPercentage }}%</div>
                  <div class="text-caption">Win Rate</div>
                </v-col>
                <v-col cols="6" md="2" class="text-center">
                  <div class="text-h4 text-secondary">{{ player.currentSeasonStats.titlesWon }}</div>
                  <div class="text-caption">Titles</div>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Surface-Specific Statistics -->
      <v-row v-if="player.currentSeasonStats" class="mb-4">
        <v-col cols="12">
          <v-card>
            <v-card-title>
              <v-icon class="mr-2">mdi-texture-box</v-icon>
              Performance by Surface
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
              <v-row>
                <!-- Hard Court -->
                <v-col cols="12" md="4">
                  <v-card variant="outlined" color="blue-grey-lighten-4">
                    <v-card-title class="text-center">
                      <v-icon class="mr-2" color="blue-grey">mdi-circle</v-icon>
                      Hard Court
                    </v-card-title>
                    <v-divider></v-divider>
                    <v-card-text>
                      <v-row>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-success">{{ player.currentSeasonStats.hardMatchesWon }}</div>
                          <div class="text-caption">Wins</div>
                        </v-col>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-error">{{ player.currentSeasonStats.hardMatchesLost }}</div>
                          <div class="text-caption">Losses</div>
                        </v-col>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-primary">{{ player.currentSeasonStats.hardWinPercentage }}%</div>
                          <div class="text-caption">Win Rate</div>
                        </v-col>
                      </v-row>
                    </v-card-text>
                  </v-card>
                </v-col>

                <!-- Clay Court -->
                <v-col cols="12" md="4">
                  <v-card variant="outlined" color="deep-orange-lighten-4">
                    <v-card-title class="text-center">
                      <v-icon class="mr-2" color="deep-orange">mdi-circle</v-icon>
                      Clay Court
                    </v-card-title>
                    <v-divider></v-divider>
                    <v-card-text>
                      <v-row>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-success">{{ player.currentSeasonStats.clayMatchesWon }}</div>
                          <div class="text-caption">Wins</div>
                        </v-col>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-error">{{ player.currentSeasonStats.clayMatchesLost }}</div>
                          <div class="text-caption">Losses</div>
                        </v-col>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-primary">{{ player.currentSeasonStats.clayWinPercentage }}%</div>
                          <div class="text-caption">Win Rate</div>
                        </v-col>
                      </v-row>
                    </v-card-text>
                  </v-card>
                </v-col>

                <!-- Grass Court -->
                <v-col cols="12" md="4">
                  <v-card variant="outlined" color="green-lighten-4">
                    <v-card-title class="text-center">
                      <v-icon class="mr-2" color="green">mdi-circle</v-icon>
                      Grass Court
                    </v-card-title>
                    <v-divider></v-divider>
                    <v-card-text>
                      <v-row>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-success">{{ player.currentSeasonStats.grassMatchesWon }}</div>
                          <div class="text-caption">Wins</div>
                        </v-col>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-error">{{ player.currentSeasonStats.grassMatchesLost }}</div>
                          <div class="text-caption">Losses</div>
                        </v-col>
                        <v-col cols="4" class="text-center">
                          <div class="text-h5 text-primary">{{ player.currentSeasonStats.grassWinPercentage }}%</div>
                          <div class="text-caption">Win Rate</div>
                        </v-col>
                      </v-row>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Ranking History -->
      <v-row v-if="player.rankingHistory && player.rankingHistory.length > 0">
        <v-col cols="12">
          <v-card>
            <v-card-title>
              <v-icon class="mr-2">mdi-chart-timeline-variant</v-icon>
              Ranking History
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
              <v-table>
                <thead>
                  <tr>
                    <th class="text-left">Date</th>
                    <th class="text-center">Rank</th>
                    <th class="text-right">Points</th>
                    <th class="text-center">Change</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(ranking, index) in player.rankingHistory.slice(0, 10)" :key="index">
                    <td>{{ new Date(ranking.rankingDate).toLocaleDateString() }}</td>
                    <td class="text-center">
                      <v-chip size="small" :color="ranking.rank <= 10 ? 'primary' : 'default'">
                        {{ ranking.rank }}
                      </v-chip>
                    </td>
                    <td class="text-right">{{ ranking.points.toLocaleString() }}</td>
                    <td class="text-center">
                      <v-chip
                        v-if="ranking.rankChange"
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
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </template>

    <!-- Not Found -->
    <v-row v-else>
      <v-col cols="12" class="text-center">
        <v-icon size="64" color="grey">mdi-account-off</v-icon>
        <p class="text-h6 text-grey mt-4">Player not found</p>
        <v-btn color="primary" to="/players" class="mt-4">Back to Players</v-btn>
      </v-col>
    </v-row>
  </div>
</template>
