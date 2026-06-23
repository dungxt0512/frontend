<template>
  <header class="header">
    <div class="container header-inner">
      <router-link to="/" class="logo">MAX<span>VERSE</span></router-link>

      <nav class="nav-links">
        <router-link to="/">Trang chủ</router-link>
        <router-link to="/products">Sản phẩm</router-link>
        <router-link v-if="auth.isLoggedIn" to="/my-orders">Đơn hàng</router-link>
        <router-link v-if="auth.isAdmin" to="/admin">Quản trị</router-link>
      </nav>

      <form class="search-box" @submit.prevent="search">
        <input v-model="keyword" type="text" placeholder="Tìm giày, hãng, loại..." />
        <button type="submit" aria-label="Tìm kiếm" class="btn btn-secondary">Tìm</button>
      </form>

      <div class="header-actions">
        <router-link to="/cart" class="cart-link">
          🛒
          <span v-if="cart.totalItems > 0" class="cart-badge">{{ cart.totalItems }}</span>
        </router-link>

        <div v-if="auth.isLoggedIn" class="user-menu">
          <button class="user-btn" @click="menuOpen = !menuOpen">{{ auth.user.fullName.split(' ')[0] }} ▾</button>
          <div v-if="menuOpen" class="dropdown" @click="menuOpen = false">
            <router-link to="/my-orders">Đơn hàng của tôi</router-link>
            <button @click="handleLogout">Đăng xuất</button>
          </div>
        </div>
        <router-link v-else to="/login" class="btn btn-primary">Đăng nhập</router-link>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useCartStore } from '../stores/cart'

const router = useRouter()
const auth = useAuthStore()
const cart = useCartStore()
const keyword = ref('')
const menuOpen = ref(false)

function search() {
  router.push({ path: '/products', query: { keyword: keyword.value } })
}

function handleLogout() {
  auth.logout()
  cart.clearLocal()
  router.push('/')
}

onMounted(() => {
  if (auth.isLoggedIn) cart.fetchCart()
})
</script>

<style scoped>
.header {
  position: sticky;
  top: 0;
  z-index: 50;
  background: rgba(11, 14, 20, 0.92);
  backdrop-filter: blur(10px);
  border-bottom: 1px solid var(--border-subtle);
}

.header-inner {
  display: flex;
  align-items: center;
  gap: 32px;
  height: 72px;
}

.logo {
  font-family: var(--font-display);
  font-size: 26px;
  letter-spacing: 1px;
  flex-shrink: 0;
}
.logo span { color: var(--accent-orange); }

.nav-links {
  display: flex;
  gap: 24px;
  flex-shrink: 0;
}
.nav-links a {
  font-weight: 600;
  font-size: 15px;
  color: var(--text-secondary);
  transition: color 0.15s ease;
}
.nav-links a:hover,
.nav-links a.router-link-active {
  color: var(--text-primary);
}

.search-box {
  flex: 1;
  display: flex;
  align-items: center;
  background: var(--bg-elevated-2);
  border: 1px solid var(--border-subtle);
  border-radius: 999px;
  padding: 4px 4px 4px 16px;
  max-width: 360px;
}
.search-box input {
  flex: 1;
  background: none;
  border: none;
  color: var(--text-primary);
  font-size: 14px;
  outline: none;
}
.search-box button {
  background: var(--bg-elevated);
  border-radius: 50%;
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-shrink: 0;
}

.cart-link {
  position: relative;
  font-size: 22px;
}
.cart-badge {
  position: absolute;
  top: -6px;
  right: -10px;
  background: var(--accent-orange);
  color: #1A1006;
  font-size: 11px;
  font-weight: 700;
  border-radius: 999px;
  min-width: 18px;
  height: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 4px;
}

.user-menu {
  position: relative;
}
.user-btn {
  color: var(--text-primary);
  font-weight: 600;
  font-size: 14px;
}
.dropdown {
  position: absolute;
  right: 0;
  top: calc(100% + 8px);
  background: var(--bg-elevated);
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-sm);
  box-shadow: var(--shadow-card);
  min-width: 160px;
  overflow: hidden;
}
.dropdown a, .dropdown button {
  display: block;
  width: 100%;
  text-align: left;
  padding: 12px 16px;
  font-size: 14px;
  color: var(--text-secondary);
}
.dropdown a:hover, .dropdown button:hover {
  background: var(--bg-elevated-2);
  color: var(--text-primary);
}

@media (max-width: 900px) {
  .nav-links, .search-box { display: none; }
}
</style>
