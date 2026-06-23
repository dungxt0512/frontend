import { defineStore } from 'pinia'
import api from '../services/api'

export const useCartStore = defineStore('cart', {
  state: () => ({
    items: [],
    loading: false
  }),

  getters: {
    totalItems: (state) => state.items.reduce((sum, item) => sum + item.quantity, 0),
    totalAmount: (state) => state.items.reduce((sum, item) => sum + item.unitPrice * item.quantity, 0)
  },

  actions: {
    async fetchCart() {
      this.loading = true
      try {
        const { data } = await api.get('/cart')
        this.items = data
      } finally {
        this.loading = false
      }
    },

    async addToCart(variantId, quantity = 1) {
      await api.post('/cart', { variantId, quantity })
      await this.fetchCart()
    },

    async updateQuantity(cartItemId, quantity) {
      await api.put(`/cart/${cartItemId}`, { quantity })
      await this.fetchCart()
    },

    async removeItem(cartItemId) {
      await api.delete(`/cart/${cartItemId}`)
      await this.fetchCart()
    },

    clearLocal() {
      this.items = []
    }
  }
})
