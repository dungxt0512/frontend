import { defineStore } from 'pinia'
import api from '../services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('maxverse_token') || null,
    user: JSON.parse(localStorage.getItem('maxverse_user') || 'null')
  }),

  getters: {
    isLoggedIn: (state) => !!state.token,
    isAdmin: (state) => state.user?.role === 'Admin'
  },

  actions: {
    async login(email, password) {
      const { data } = await api.post('/auth/login', { email, password })
      this._setSession(data)
      return data
    },

    async register(fullName, email, password, phoneNumber) {
      const { data } = await api.post('/auth/register', {
        fullName, email, password, phoneNumber
      })
      this._setSession(data)
      return data
    },

    logout() {
      this.token = null
      this.user = null
      localStorage.removeItem('maxverse_token')
      localStorage.removeItem('maxverse_user')
    },

    _setSession(data) {
      this.token = data.token
      this.user = { fullName: data.fullName, email: data.email, role: data.role }
      localStorage.setItem('maxverse_token', data.token)
      localStorage.setItem('maxverse_user', JSON.stringify(this.user))
    }
  }
})
