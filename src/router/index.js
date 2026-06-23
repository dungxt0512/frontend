import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import HomeView from '../views/HomeView.vue'
import ProductListView from '../views/ProductListView.vue'
import ProductDetailView from '../views/ProductDetailView.vue'
import CartView from '../views/CartView.vue'
import CheckoutView from '../views/CheckoutView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import MyOrdersView from '../views/MyOrdersView.vue'
import VnPayReturnView from '../views/VnPayReturnView.vue'

import AdminDashboard from '../views/admin/AdminDashboard.vue'
import AdminProducts from '../views/admin/AdminProducts.vue'
import AdminProductForm from '../views/admin/AdminProductForm.vue'
import AdminOrders from '../views/admin/AdminOrders.vue'

const routes = [
  { path: '/', name: 'home', component: HomeView },
  { path: '/products', name: 'products', component: ProductListView },
  { path: '/products/:id', name: 'product-detail', component: ProductDetailView, props: true },
  { path: '/cart', name: 'cart', component: CartView, meta: { requiresAuth: true } },
  { path: '/checkout', name: 'checkout', component: CheckoutView, meta: { requiresAuth: true } },
  { path: '/login', name: 'login', component: LoginView },
  { path: '/register', name: 'register', component: RegisterView },
  { path: '/my-orders', name: 'my-orders', component: MyOrdersView, meta: { requiresAuth: true } },
  { path: '/vnpay-return', name: 'vnpay-return', component: VnPayReturnView },

  // Admin routes
  { path: '/admin', name: 'admin-dashboard', component: AdminDashboard, meta: { requiresAdmin: true } },
  { path: '/admin/products', name: 'admin-products', component: AdminProducts, meta: { requiresAdmin: true } },
  { path: '/admin/products/new', name: 'admin-product-new', component: AdminProductForm, meta: { requiresAdmin: true } },
  { path: '/admin/products/:id/edit', name: 'admin-product-edit', component: AdminProductForm, props: true, meta: { requiresAdmin: true } },
  { path: '/admin/orders', name: 'admin-orders', component: AdminOrders, meta: { requiresAdmin: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (to.meta.requiresAdmin) {
    if (!auth.isLoggedIn) return { name: 'login', query: { redirect: to.fullPath } }
    if (!auth.isAdmin) return { name: 'home' }
  }
})

export default router
