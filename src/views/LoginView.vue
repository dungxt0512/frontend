<template>
  <div class="auth-page">
    <form class="auth-form card" @submit.prevent="handleLogin">
      <h1>ĐĂNG NHẬP</h1>
      <p class="auth-sub">Chào mừng trở lại MaxVerse</p>

      <div class="form-group">
        <label class="form-label">Email</label>
        <input v-model="email" type="email" class="form-input" required />
      </div>

      <div class="form-group">
        <label class="form-label">Mật khẩu</label>
        <input v-model="password" type="password" class="form-input" required />
      </div>

      <p v-if="error" class="form-error">{{ error }}</p>

      <button type="submit" class="btn btn-primary btn-block" :disabled="loading">
        {{ loading ? 'Đang đăng nhập...' : 'Đăng nhập' }}
      </button>

      <p class="auth-footer">
        Chưa có tài khoản? <router-link to="/register">Đăng ký ngay</router-link>
      </p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useCartStore } from '../stores/cart'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()
const cart = useCartStore()

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')

async function handleLogin() {
  loading.value = true
  error.value = ''
  try {
    await auth.login(email.value, password.value)
    await cart.fetchCart()
    router.push(route.query.redirect || '/')
  } catch (err) {
    error.value = err.response?.data?.message || 'Đăng nhập thất bại.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 70vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 24px;
}
.auth-form { width: 100%; max-width: 400px; padding: 40px; }
.auth-title { font-size: 32px; text-align: center; }
.auth-sub { text-align: center; color: var(--text-secondary); margin-bottom: 28px; }
.auth-footer { text-align: center; margin-top: 20px; font-size: 14px; color: var(--text-secondary); }
.auth-footer a { color: var(--accent-orange); font-weight: 600; }
</style>
