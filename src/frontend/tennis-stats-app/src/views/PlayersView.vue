<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { usePlayerStore } from '@/stores/playerStore'

const props = defineProps<{
  association: string
}>()

const playerStore = usePlayerStore()

const search = ref('')
const country = ref('')
const page = ref(1)
const pageSize = ref(20)

async function loadPlayers() {
  await playerStore.fetchPlayers({
    association: props.association,
    page: page.value,
    pageSize: pageSize.value,
    search: search.value || undefined,
    country: country.value || undefined,
  })
}

onMounted(loadPlayers)

watch([() => props.association], () => {
  page.value = 1
  loadPlayers()
})

function onSearch() {
  page.value = 1
  loadPlayers()
}

function onPageChange(newPage: number) {
  page.value = newPage
  loadPlayers()
}
</script>

<template>
  <div>
    <v-row class="mb-4">
      <v-col cols="12">
        <v-card>
          <v-card-title class="text-h5">
            <v-icon class="mr-2">mdi-account-group</v-icon>
            {{ association }} Players
          </v-card-title>
        </v-card>
      </v-col>
    </v-row>

    <!-- Search and Filters -->
    <v-row class="mb-4">
      <v-col cols="12" md="6">
        <v-text-field
          v-model="search"
          label="Search players"
          prepend-inner-icon="mdi-magnify"
          clearable
          @keyup.enter="onSearch"
          @click:clear="onSearch"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="4">
        <v-text-field
          v-model="country"
          label="Filter by country"
          prepend-inner-icon="mdi-flag"
          clearable
          @keyup.enter="onSearch"
          @click:clear="onSearch"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="2">
        <v-btn color="primary" block @click="onSearch">
          <v-icon left>mdi-magnify</v-icon>
          Search
        </v-btn>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-row v-if="playerStore.loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <p class="mt-4">Loading players...</p>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-alert v-else-if="playerStore.error" type="error" class="mb-4">
      {{ playerStore.error }}
    </v-alert>

    <!-- Players Grid -->
    <v-row v-else-if="playerStore.players.length > 0">
      <v-col
        v-for="player in playerStore.players"
        :key="player.id"
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <v-card :to="`/players/${player.id}`" hover>
          <v-card-text class="text-center">
            <v-avatar size="80" color="grey-lighten-2" class="mb-3">
              <v-img v-if="player.imageUrl" :src="player.imageUrl" :alt="player.fullName"></v-img>
              <v-icon v-else size="48">mdi-account</v-icon>
            </v-avatar>
            <div class="text-h6">{{ player.fullName }}</div>
            <div class="text-subtitle-1 text-grey">{{ player.country || 'Unknown' }}</div>
            <v-chip
              v-if="player.currentRank"
              color="primary"
              size="small"
              class="mt-2"
            >
              Rank #{{ player.currentRank }}
            </v-chip>
          </v-card-text>
          <v-divider></v-divider>
          <v-card-text>
            <v-row dense>
              <v-col cols="6">
                <div class="text-caption text-grey">Hand</div>
                <div>{{ player.hand }}</div>
              </v-col>
              <v-col cols="6">
                <div class="text-caption text-grey">Age</div>
                <div>{{ player.age || '-' }}</div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Empty State -->
    <v-row v-else>
      <v-col cols="12" class="text-center">
        <v-icon size="64" color="grey">mdi-account-off</v-icon>
        <p class="text-h6 text-grey mt-4">No players found</p>
        <p class="text-grey">Try adjusting your search criteria or sync data from the API.</p>
      </v-col>
    </v-row>

    <!-- Pagination -->
    <v-row v-if="playerStore.pagination.totalPages > 1" class="mt-4">
      <v-col cols="12" class="d-flex justify-center">
        <v-pagination
          v-model="page"
          :length="playerStore.pagination.totalPages"
          :total-visible="7"
          @update:model-value="onPageChange"
        ></v-pagination>
      </v-col>
    </v-row>
  </div>
</template>
