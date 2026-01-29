<script setup lang="ts">
import { ref } from 'vue'
import { useTheme } from 'vuetify'

const theme = useTheme()
const drawer = ref(true)

const menuItems = [
  { title: 'Dashboard', icon: 'mdi-view-dashboard', to: '/' },
  { title: 'Players', icon: 'mdi-account-group', to: '/players' },
  { title: 'Rankings', icon: 'mdi-trophy', to: '/rankings' },
  { title: 'Tournaments', icon: 'mdi-tournament', to: '/tournaments' },
  { title: 'Matches', icon: 'mdi-tennis', to: '/matches' },
]

const currentAssociation = ref('WTA')

function toggleTheme() {
  theme.global.name.value = theme.global.current.value.dark ? 'tennisLight' : 'tennisDark'
}
</script>

<template>
  <v-app>
    <!-- Navigation Drawer -->
    <v-navigation-drawer v-model="drawer" permanent class="nav-drawer">
      <!-- Brand Header -->
      <div class="brand-header pa-4">
        <div class="d-flex align-center">
          <img src="/logo.svg" alt="Tennis Stats" class="brand-logo mr-3" />
          <div>
            <div class="brand-title">Tennis Stats</div>
            <div class="brand-subtitle">Statistics & Analytics</div>
          </div>
        </div>
      </div>

      <v-divider></v-divider>

      <!-- Association Toggle -->
      <v-list-item class="py-3">
        <v-btn-toggle v-model="currentAssociation" mandatory class="w-100 association-toggle" color="primary">
          <v-btn value="WTA" size="small" class="flex-grow-1">
            <span class="font-weight-bold">WTA</span>
          </v-btn>
          <v-btn value="ATP" size="small" class="flex-grow-1">
            <span class="font-weight-bold">ATP</span>
          </v-btn>
        </v-btn-toggle>
      </v-list-item>

      <v-divider></v-divider>

      <v-list density="compact" nav class="py-2">
        <v-list-item
          v-for="item in menuItems"
          :key="item.title"
          :to="item.to"
          :prepend-icon="item.icon"
          :title="item.title"
          color="primary"
          rounded="lg"
          class="my-1 mx-2"
        ></v-list-item>
      </v-list>

      <template v-slot:append>
        <v-divider></v-divider>
        <div class="pa-3">
          <v-chip color="secondary" size="small" class="w-100 justify-center">
            <v-icon start size="small">mdi-database</v-icon>
            Data by balldontlie.io
          </v-chip>
        </div>
      </template>
    </v-navigation-drawer>

    <!-- App Bar -->
    <v-app-bar color="primary" elevation="2">
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>

      <v-toolbar-title class="d-flex align-center">
        <img src="/favicon.svg" alt="" class="app-bar-icon mr-2" />
        <span class="font-weight-bold">Tennis Statistics</span>
      </v-toolbar-title>

      <v-spacer></v-spacer>

      <v-chip class="mr-3" color="secondary" variant="elevated" size="small">
        <v-icon start size="small">mdi-trophy</v-icon>
        {{ currentAssociation }}
      </v-chip>

      <v-btn icon variant="text" @click="toggleTheme">
        <v-icon>{{ theme.global.current.value.dark ? 'mdi-weather-sunny' : 'mdi-weather-night' }}</v-icon>
      </v-btn>
    </v-app-bar>

    <!-- Main Content -->
    <v-main class="main-content">
      <v-container fluid class="pa-6">
        <router-view :association="currentAssociation" />
      </v-container>
    </v-main>

    <!-- Footer -->
    <v-footer app class="footer-bar">
      <v-row justify="center" no-gutters>
        <v-col class="text-center" cols="12">
          <span class="text-caption text-medium-emphasis">
            © {{ new Date().getFullYear() }} Tennis Statistics — Comprehensive WTA & ATP data and analytics
          </span>
        </v-col>
      </v-row>
    </v-footer>
  </v-app>
</template>

<style>
.nav-drawer {
  border-right: 1px solid rgba(0, 0, 0, 0.08) !important;
}

.brand-header {
  background: linear-gradient(135deg, rgba(27, 94, 32, 0.08) 0%, rgba(192, 202, 51, 0.08) 100%);
}

.brand-logo {
  width: 48px;
  height: 48px;
  border-radius: 12px;
}

.brand-title {
  font-size: 1.1rem;
  font-weight: 700;
  color: rgb(var(--v-theme-primary));
  line-height: 1.2;
}

.brand-subtitle {
  font-size: 0.75rem;
  color: rgba(var(--v-theme-on-surface), 0.6);
}

.association-toggle {
  border-radius: 8px !important;
}

.association-toggle .v-btn {
  border-radius: 6px !important;
}

.app-bar-icon {
  width: 28px;
  height: 28px;
}

.main-content {
  background: rgb(var(--v-theme-background));
  min-height: calc(100vh - 128px);
}

.footer-bar {
  border-top: 1px solid rgba(0, 0, 0, 0.08);
  background: rgb(var(--v-theme-surface)) !important;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: rgba(27, 94, 32, 0.3);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: rgba(27, 94, 32, 0.5);
}
</style>
