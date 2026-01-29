<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { Match } from '@/types'
import { api } from '@/services/api'

const matches = ref<Match[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const search = ref('')
const selectedStatus = ref<string | null>(null)
const selectedAssociation = ref<string>('WTA')
const page = ref(1)
const totalPages = ref(1)

const matchStatuses = ['Scheduled', 'InProgress', 'Completed', 'Cancelled', 'Postponed']
const associations = ['WTA', 'ATP']

async function fetchMatches() {
  loading.value = true
  error.value = null

  try {
    const params: Record<string, string | number> = {
      pageNumber: page.value,
      pageSize: 20,
      association: selectedAssociation.value,
    }

    if (search.value) {
      params.search = search.value
    }

    if (selectedStatus.value) {
      params.status = selectedStatus.value
    }

    const response = await api.get<{ items: Match[]; totalPages: number }>('/api/matches', { params })
    matches.value = response.data.items || []
    totalPages.value = response.data.totalPages || 1
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to fetch matches'
    console.error('Error fetching matches:', err)
  } finally {
    loading.value = false
  }
}

function getStatusColor(status: string): string {
  switch (status?.toLowerCase()) {
    case 'completed':
      return 'success'
    case 'inprogress':
      return 'warning'
    case 'scheduled':
      return 'info'
    case 'cancelled':
    case 'postponed':
      return 'error'
    default:
      return 'grey'
  }
}

function getStatusIcon(status: string): string {
  switch (status?.toLowerCase()) {
    case 'completed':
      return 'mdi-check-circle'
    case 'inprogress':
      return 'mdi-play-circle'
    case 'scheduled':
      return 'mdi-clock-outline'
    case 'cancelled':
      return 'mdi-close-circle'
    case 'postponed':
      return 'mdi-pause-circle'
    default:
      return 'mdi-help-circle'
  }
}

function formatMatchDate(date?: string): string {
  if (!date) return '-'
  const d = new Date(date)
  return d.toLocaleDateString('en-US', {
    weekday: 'short',
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  })
}

function formatMatchTime(date?: string): string {
  if (!date) return '-'
  const d = new Date(date)
  return d.toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
  })
}

function onPageChange(newPage: number) {
  page.value = newPage
  fetchMatches()
}

onMounted(() => {
  fetchMatches()
})
</script>

<template>
  <div>
    <div class="d-flex justify-space-between align-center mb-6">
      <h1 class="text-h4">Matches</h1>
      <v-btn-toggle v-model="selectedAssociation" mandatory @update:modelValue="fetchMatches">
        <v-btn v-for="assoc in associations" :key="assoc" :value="assoc">
          {{ assoc }}
        </v-btn>
      </v-btn-toggle>
    </div>

    <!-- Filters -->
    <v-card class="mb-6">
      <v-card-text>
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="search"
              prepend-inner-icon="mdi-magnify"
              label="Search matches..."
              variant="outlined"
              density="compact"
              clearable
              @update:modelValue="fetchMatches"
            ></v-text-field>
          </v-col>
          <v-col cols="12" md="6">
            <v-select
              v-model="selectedStatus"
              :items="matchStatuses"
              label="Status"
              variant="outlined"
              density="compact"
              clearable
              @update:modelValue="fetchMatches"
            ></v-select>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <!-- Loading State -->
    <v-row v-if="loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <p class="mt-4">Loading matches...</p>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-alert v-else-if="error" type="error" class="mb-4">
      {{ error }}
      <template v-slot:append>
        <v-btn color="white" variant="text" @click="fetchMatches">Retry</v-btn>
      </template>
    </v-alert>

    <!-- Matches List -->
    <template v-else-if="matches.length > 0">
      <v-card v-for="match in matches" :key="match.id" class="mb-4" hover>
        <v-card-item>
          <div class="d-flex justify-space-between align-center">
            <div>
              <v-chip :color="getStatusColor(match.status)" size="small" class="mr-2">
                <v-icon :icon="getStatusIcon(match.status)" size="small" start></v-icon>
                {{ match.status }}
              </v-chip>
              <span class="text-caption text-grey">{{ match.tournament?.name || 'Tournament' }}</span>
              <span v-if="match.round" class="text-caption text-grey ml-2">• {{ match.round }}</span>
            </div>
            <div class="text-caption text-grey">
              {{ formatMatchDate(match.scheduledDate) }} at {{ formatMatchTime(match.scheduledDate) }}
            </div>
          </div>
        </v-card-item>

        <v-divider></v-divider>

        <v-card-text>
          <v-row align="center">
            <!-- Player 1 -->
            <v-col cols="5">
              <div class="d-flex align-center justify-end">
                <div class="text-right mr-3">
                  <div class="text-h6" :class="{ 'text-primary font-weight-bold': match.winnerId === match.player1Id }">
                    {{ match.player1?.fullName || 'Player 1' }}
                  </div>
                  <div v-if="match.player1?.country" class="text-caption text-grey">
                    {{ match.player1.country }}
                  </div>
                </div>
                <v-avatar v-if="match.player1?.imageUrl" size="50">
                  <v-img :src="match.player1.imageUrl" :alt="match.player1.fullName"></v-img>
                </v-avatar>
                <v-avatar v-else size="50" color="grey-lighten-2">
                  <v-icon>mdi-account</v-icon>
                </v-avatar>
              </div>
            </v-col>

            <!-- Score -->
            <v-col cols="2" class="text-center">
              <div v-if="match.status === 'Completed' || match.status === 'InProgress'" class="score-display">
                <div class="text-h5">
                  {{ match.player1Score ?? 0 }} - {{ match.player2Score ?? 0 }}
                </div>
                <div v-if="match.sets && match.sets.length > 0" class="text-caption">
                  <span v-for="(set, index) in match.sets" :key="index">
                    {{ set.player1Games }}-{{ set.player2Games }}
                    <span v-if="index < match.sets.length - 1">, </span>
                  </span>
                </div>
              </div>
              <div v-else class="text-h6 text-grey">VS</div>
            </v-col>

            <!-- Player 2 -->
            <v-col cols="5">
              <div class="d-flex align-center">
                <v-avatar v-if="match.player2?.imageUrl" size="50">
                  <v-img :src="match.player2.imageUrl" :alt="match.player2.fullName"></v-img>
                </v-avatar>
                <v-avatar v-else size="50" color="grey-lighten-2">
                  <v-icon>mdi-account</v-icon>
                </v-avatar>
                <div class="ml-3">
                  <div class="text-h6" :class="{ 'text-primary font-weight-bold': match.winnerId === match.player2Id }">
                    {{ match.player2?.fullName || 'Player 2' }}
                  </div>
                  <div v-if="match.player2?.country" class="text-caption text-grey">
                    {{ match.player2.country }}
                  </div>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
          <v-btn color="primary" variant="text">View Details</v-btn>
          <v-spacer></v-spacer>
          <v-btn v-if="match.tournament" color="secondary" variant="text">
            <v-icon start>mdi-trophy</v-icon>
            Tournament
          </v-btn>
        </v-card-actions>
      </v-card>

      <!-- Pagination -->
      <div class="d-flex justify-center mt-6">
        <v-pagination
          v-model="page"
          :length="totalPages"
          :total-visible="7"
          @update:modelValue="onPageChange"
        ></v-pagination>
      </div>
    </template>

    <!-- Empty State -->
    <v-row v-else>
      <v-col cols="12" class="text-center">
        <v-icon size="64" color="grey">mdi-tennis</v-icon>
        <p class="text-h6 text-grey mt-4">No matches found</p>
        <p class="text-body-2 text-grey">
          Try adjusting your search filters or sync data from the API.
        </p>
      </v-col>
    </v-row>
  </div>
</template>

<style scoped>
.score-display {
  min-width: 80px;
}
</style>
