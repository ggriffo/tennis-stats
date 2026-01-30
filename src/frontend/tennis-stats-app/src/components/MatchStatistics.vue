<script setup lang="ts">
import { computed } from 'vue'
import type { MatchStatistics } from '@/types'

interface Props {
  statistics: MatchStatistics
  player1Name: string
  player2Name: string
}

const props = defineProps<Props>()

const stats = computed(() => props.statistics)

// Helper function to format percentages
const formatPercentage = (value?: number): string => {
  return value !== undefined && value !== null ? `${value.toFixed(1)}%` : '-'
}
</script>

<template>
  <v-card>
    <v-card-title>
      <v-icon class="mr-2">mdi-chart-bar</v-icon>
      Match Statistics
    </v-card-title>
    <v-divider></v-divider>
    <v-card-text>
      <!-- Serve Statistics -->
      <div class="mb-6">
        <h3 class="text-subtitle-1 mb-3">
          <v-icon size="small" class="mr-1">mdi-tennis</v-icon>
          Serve Statistics
        </h3>
        <v-table density="compact">
          <thead>
            <tr>
              <th class="text-left">Statistic</th>
              <th class="text-center">{{ player1Name }}</th>
              <th class="text-center">{{ player2Name }}</th>
            </tr>
          </thead>
          <tbody>
            <!-- Aces -->
            <tr>
              <td>Aces</td>
              <td class="text-center font-weight-bold">{{ stats.player1Aces ?? '-' }}</td>
              <td class="text-center font-weight-bold">{{ stats.player2Aces ?? '-' }}</td>
            </tr>
            <!-- Double Faults -->
            <tr>
              <td>Double Faults</td>
              <td class="text-center">{{ stats.player1DoubleFaults ?? '-' }}</td>
              <td class="text-center">{{ stats.player2DoubleFaults ?? '-' }}</td>
            </tr>
            <!-- 1st Serve % -->
            <tr>
              <td>1st Serve %</td>
              <td class="text-center">
                {{ formatPercentage(stats.player1FirstServePercentageCalc) }}
                <span v-if="stats.player1FirstServesIn && stats.player1FirstServesTotal" class="text-caption text-grey">
                  ({{ stats.player1FirstServesIn }}/{{ stats.player1FirstServesTotal }})
                </span>
              </td>
              <td class="text-center">
                {{ formatPercentage(stats.player2FirstServePercentageCalc) }}
                <span v-if="stats.player2FirstServesIn && stats.player2FirstServesTotal" class="text-caption text-grey">
                  ({{ stats.player2FirstServesIn }}/{{ stats.player2FirstServesTotal }})
                </span>
              </td>
            </tr>
            <!-- 1st Serve Points Won % -->
            <tr>
              <td>1st Serve Points Won %</td>
              <td class="text-center">
                {{ formatPercentage(stats.player1FirstServePointsWonPercentage) }}
                <span v-if="stats.player1FirstServePointsWon && stats.player1FirstServePointsTotal" class="text-caption text-grey">
                  ({{ stats.player1FirstServePointsWon }}/{{ stats.player1FirstServePointsTotal }})
                </span>
              </td>
              <td class="text-center">
                {{ formatPercentage(stats.player2FirstServePointsWonPercentage) }}
                <span v-if="stats.player2FirstServePointsWon && stats.player2FirstServePointsTotal" class="text-caption text-grey">
                  ({{ stats.player2FirstServePointsWon }}/{{ stats.player2FirstServePointsTotal }})
                </span>
              </td>
            </tr>
            <!-- 2nd Serve Points Won % -->
            <tr>
              <td>2nd Serve Points Won %</td>
              <td class="text-center">
                {{ formatPercentage(stats.player1SecondServePointsWonPercentage) }}
                <span v-if="stats.player1SecondServePointsWon && stats.player1SecondServePointsTotal" class="text-caption text-grey">
                  ({{ stats.player1SecondServePointsWon }}/{{ stats.player1SecondServePointsTotal }})
                </span>
              </td>
              <td class="text-center">
                {{ formatPercentage(stats.player2SecondServePointsWonPercentage) }}
                <span v-if="stats.player2SecondServePointsWon && stats.player2SecondServePointsTotal" class="text-caption text-grey">
                  ({{ stats.player2SecondServePointsWon }}/{{ stats.player2SecondServePointsTotal }})
                </span>
              </td>
            </tr>
            <!-- Break Points Saved % -->
            <tr>
              <td>Break Points Saved %</td>
              <td class="text-center">
                {{ formatPercentage(stats.player1BreakPointsSavedPercentage) }}
                <span v-if="stats.player1BreakPointsSaved !== undefined && stats.player1BreakPointsFaced !== undefined" class="text-caption text-grey">
                  ({{ stats.player1BreakPointsSaved }}/{{ stats.player1BreakPointsFaced }})
                </span>
              </td>
              <td class="text-center">
                {{ formatPercentage(stats.player2BreakPointsSavedPercentage) }}
                <span v-if="stats.player2BreakPointsSaved !== undefined && stats.player2BreakPointsFaced !== undefined" class="text-caption text-grey">
                  ({{ stats.player2BreakPointsSaved }}/{{ stats.player2BreakPointsFaced }})
                </span>
              </td>
            </tr>
          </tbody>
        </v-table>
      </div>

      <!-- Shot Statistics -->
      <div v-if="stats.player1Winners !== undefined || stats.player2Winners !== undefined">
        <h3 class="text-subtitle-1 mb-3">
          <v-icon size="small" class="mr-1">mdi-target</v-icon>
          Shot Statistics
        </h3>
        <v-table density="compact">
          <thead>
            <tr>
              <th class="text-left">Statistic</th>
              <th class="text-center">{{ player1Name }}</th>
              <th class="text-center">{{ player2Name }}</th>
            </tr>
          </thead>
          <tbody>
            <!-- Winners -->
            <tr>
              <td>Winners</td>
              <td class="text-center text-success font-weight-bold">{{ stats.player1Winners ?? '-' }}</td>
              <td class="text-center text-success font-weight-bold">{{ stats.player2Winners ?? '-' }}</td>
            </tr>
            <!-- Unforced Errors -->
            <tr>
              <td>Unforced Errors</td>
              <td class="text-center text-error">{{ stats.player1UnforcedErrors ?? '-' }}</td>
              <td class="text-center text-error">{{ stats.player2UnforcedErrors ?? '-' }}</td>
            </tr>
          </tbody>
        </v-table>
      </div>
    </v-card-text>
  </v-card>
</template>

<style scoped>
th {
  font-weight: 600 !important;
}

tr:hover {
  background-color: rgba(0, 0, 0, 0.02);
}
</style>
