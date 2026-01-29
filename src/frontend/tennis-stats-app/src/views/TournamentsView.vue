<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { Tournament } from '@/types'
import { api } from '@/services/api'

const tournaments = ref<Tournament[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const search = ref('')
const selectedSurface = ref<string | null>(null)
const selectedAssociation = ref<string>('WTA')

const surfaces = ['Hard', 'Clay', 'Grass', 'Carpet']
const associations = ['WTA', 'ATP']

async function fetchTournaments() {
  loading.value = true
  error.value = null

  try {
    const params: Record<string, string | number> = {
      pageSize: 50,
      association: selectedAssociation.value,
    }

    if (search.value) {
      params.search = search.value
    }

    if (selectedSurface.value) {
      params.surface = selectedSurface.value
    }

    const response = await api.get<{ items: Tournament[] }>('/api/tournaments', { params })
    tournaments.value = response.data.items || []
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to fetch tournaments'
    console.error('Error fetching tournaments:', err)
  } finally {
    loading.value = false
  }
}

function getSurfaceColor(surface: string): string {
  switch (surface?.toLowerCase()) {
    case 'hard':
      return 'blue'
    case 'clay':
      return 'orange'
    case 'grass':
      return 'green'
    case 'carpet':
      return 'purple'
    default:
      return 'grey'
  }
}

function getSurfaceIcon(surface: string): string {
  switch (surface?.toLowerCase()) {
    case 'hard':
      return 'mdi-square'
    case 'clay':
      return 'mdi-circle'
    case 'grass':
      return 'mdi-grass'
    case 'carpet':
      return 'mdi-rug'
    default:
      return 'mdi-help'
  }
}

onMounted(() => {
  fetchTournaments()
})
</script>

<template>
  <div>
    <div class="d-flex justify-space-between align-center mb-6">
      <h1 class="text-h4">Tournaments</h1>
      <v-btn-toggle v-model="selectedAssociation" mandatory @update:modelValue="fetchTournaments">
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
              label="Search tournaments..."
              variant="outlined"
              density="compact"
              clearable
              @update:modelValue="fetchTournaments"
            ></v-text-field>
          </v-col>
          <v-col cols="12" md="6">
            <v-select
              v-model="selectedSurface"
              :items="surfaces"
              label="Surface"
              variant="outlined"
              density="compact"
              clearable
              @update:modelValue="fetchTournaments"
            ></v-select>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <!-- Loading State -->
    <v-row v-if="loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <p class="mt-4">Loading tournaments...</p>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-alert v-else-if="error" type="error" class="mb-4">
      {{ error }}
      <template v-slot:append>
        <v-btn color="white" variant="text" @click="fetchTournaments">Retry</v-btn>
      </template>
    </v-alert>

    <!-- Tournaments Grid -->
    <v-row v-else-if="tournaments.length > 0">
      <v-col v-for="tournament in tournaments" :key="tournament.id" cols="12" md="6" lg="4">
        <v-card class="h-100" hover>
          <v-card-item>
            <template v-slot:prepend>
              <v-avatar :color="getSurfaceColor(tournament.surface)" size="50">
                <v-icon :icon="getSurfaceIcon(tournament.surface)" color="white"></v-icon>
              </v-avatar>
            </template>
            <v-card-title>{{ tournament.name }}</v-card-title>
            <v-card-subtitle>
              <v-icon size="small" class="mr-1">mdi-map-marker</v-icon>
              {{ tournament.location || 'Location TBD' }}
            </v-card-subtitle>
          </v-card-item>

          <v-divider></v-divider>

          <v-card-text>
            <v-row dense>
              <v-col cols="6">
                <div class="text-caption text-grey">Surface</div>
                <v-chip :color="getSurfaceColor(tournament.surface)" size="small" class="mt-1">
                  {{ tournament.surface }}
                </v-chip>
              </v-col>
              <v-col cols="6">
                <div class="text-caption text-grey">Category</div>
                <div class="mt-1">{{ tournament.category || '-' }}</div>
              </v-col>
            </v-row>

            <v-row dense class="mt-2">
              <v-col cols="6">
                <div class="text-caption text-grey">Start Date</div>
                <div class="mt-1">
                  {{ tournament.startDate ? new Date(tournament.startDate).toLocaleDateString() : '-' }}
                </div>
              </v-col>
              <v-col cols="6">
                <div class="text-caption text-grey">End Date</div>
                <div class="mt-1">
                  {{ tournament.endDate ? new Date(tournament.endDate).toLocaleDateString() : '-' }}
                </div>
              </v-col>
            </v-row>

            <v-row v-if="tournament.prizeMoney" dense class="mt-2">
              <v-col cols="12">
                <div class="text-caption text-grey">Prize Money</div>
                <div class="mt-1 text-h6 text-primary">
                  ${{ tournament.prizeMoney.toLocaleString() }}
                </div>
              </v-col>
            </v-row>
          </v-card-text>

          <v-divider></v-divider>

          <v-card-actions>
            <v-btn color="primary" variant="text">View Details</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="secondary" variant="text">Matches</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- Empty State -->
    <v-row v-else>
      <v-col cols="12" class="text-center">
        <v-icon size="64" color="grey">mdi-trophy-outline</v-icon>
        <p class="text-h6 text-grey mt-4">No tournaments found</p>
        <p class="text-body-2 text-grey">
          Try adjusting your search filters or sync data from the API.
        </p>
      </v-col>
    </v-row>
  </div>
</template>
