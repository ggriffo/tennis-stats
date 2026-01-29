import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { title: 'Dashboard' },
    },
    {
      path: '/players',
      name: 'players',
      component: () => import('../views/PlayersView.vue'),
      meta: { title: 'Players' },
    },
    {
      path: '/players/:id',
      name: 'player-detail',
      component: () => import('../views/PlayerDetailView.vue'),
      meta: { title: 'Player Details' },
    },
    {
      path: '/rankings',
      name: 'rankings',
      component: () => import('../views/RankingsView.vue'),
      meta: { title: 'Rankings' },
    },
    {
      path: '/tournaments',
      name: 'tournaments',
      component: () => import('../views/TournamentsView.vue'),
      meta: { title: 'Tournaments' },
    },
    {
      path: '/matches',
      name: 'matches',
      component: () => import('../views/MatchesView.vue'),
      meta: { title: 'Matches' },
    },
  ],
})

// Update document title on navigation
router.beforeEach((to, _from, next) => {
  document.title = `${to.meta.title || 'Tennis Stats'} | Tennis Statistics`
  next()
})

export default router
