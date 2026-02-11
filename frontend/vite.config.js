import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [vue(), tailwindcss()],
  server: {
    proxy: {
      '/api': 'http://localhost:5237',
      '/hubs': {
        target: 'http://localhost:5237',
        ws: true,
        changeOrigin: true,
        secure: false
      }
    }
  }
})
