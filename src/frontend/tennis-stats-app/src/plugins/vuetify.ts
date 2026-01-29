import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

// Tennis Statistics Brand Colors
// Primary: Court Green (#1B5E20 - #2E7D32)
// Secondary: Tennis Ball Yellow (#C0CA33 - #9E9D24)
// Accent: Championship Gold (#FFD700)

export default createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'tennisLight',
    themes: {
      tennisLight: {
        dark: false,
        colors: {
          primary: '#1B5E20',
          'primary-darken-1': '#0D3B12',
          secondary: '#C0CA33',
          'secondary-darken-1': '#9E9D24',
          accent: '#FFD700',
          error: '#D32F2F',
          warning: '#F57C00',
          info: '#1976D2',
          success: '#388E3C',
          background: '#F5F5F5',
          surface: '#FFFFFF',
          'surface-variant': '#E8F5E9',
          'on-primary': '#FFFFFF',
          'on-secondary': '#1B5E20',
          'on-surface': '#212121',
          'on-background': '#212121',
        },
      },
      tennisDark: {
        dark: true,
        colors: {
          primary: '#2E7D32',
          'primary-darken-1': '#1B5E20',
          secondary: '#C0CA33',
          'secondary-darken-1': '#9E9D24',
          accent: '#FFD700',
          error: '#EF5350',
          warning: '#FFA726',
          info: '#42A5F5',
          success: '#66BB6A',
          background: '#121212',
          surface: '#1E1E1E',
          'surface-variant': '#1B3320',
          'on-primary': '#FFFFFF',
          'on-secondary': '#1B5E20',
          'on-surface': '#E0E0E0',
          'on-background': '#E0E0E0',
        },
      },
    },
  },
  defaults: {
    VCard: {
      elevation: 2,
    },
    VBtn: {
      variant: 'elevated',
    },
    VTextField: {
      variant: 'outlined',
      density: 'comfortable',
    },
    VChip: {
      variant: 'flat',
    },
  },
})
