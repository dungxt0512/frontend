<template>
  <div class="auth-page">
    <form class="auth-form card" @submit.prevent="handleRegister">
      <h1>ĐĂNG KÝ</h1>
      <p class="auth-sub">Tạo tài khoản để bắt đầu mua sắm</p>

      <div class="form-group">
        <label class="form-label">Họ và tên</label>
        <input v-model="form.fullName" type="text" class="form-input" required />
      </div>

      <div class="form-group">
        <label class="form-label">Email</label>
        <input v-model="form.email" type="email" class="form-input" required />
      </div>

      <div class="form-group">
        <label class="form-label">Số điện thoại</label>
        <input v-model="form.phoneNumber" type="tel" class="form-input" />
      </div>

      <div class="form-group">
        <label class="form-label">Mật khẩu</label>
        <input v-model="form.password" type="password" class="form-input" required minlength="6" />
        <p class="hint">Ít nhất 6 ký tự</p>
      </div>

      <p v-if="error" class="form-error">{{ error }}</p>

      <button type="submit" class="btn btn-primary btn-block" :disabled="loading">
        {{ loading ? 'Đang xử lý...' : 'Đăng ký' }}
      </button>

      <p class="auth-footer">
        Đã có tài khoản? <router-link to="/login">Đăng nhập</router-link>
      </p>
    </form>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()

const form = reactive({ fullName: '', email: '', phoneNumber: '', password: '' })
const loading = ref(false)
const error = ref('')

async function handleRegister() {
  loading.value = true
  error.value = ''
  try {
    await auth.register(form.fullName, form.email, form.password, form.phoneNumber)
    router.push('/')
  } catch (err) {
    error.value = err.response?.data?.message || 'Đăng ký thất bại.'
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
.hint { font-size: 12px; color: var(--text-muted); margin-top: 4px; }
</style>
