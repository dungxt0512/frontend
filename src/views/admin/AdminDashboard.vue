<template>
  <div class="container admin-page">
    <h1 class="heading-display page-title">QUẢN TRỊ HỆ THỐNG</h1>

    <div class="stats-grid">
      <div class="stat-card card">
        <p class="stat-label">Tổng đơn hàng</p>
        <p class="stat-value">{{ stats.totalOrders }}</p>
      </div>
      <div class="stat-card card">
        <p class="stat-label">Đơn chờ xử lý</p>
        <p class="stat-value">{{ stats.processingOrders }}</p>
      </div>
      <div class="stat-card card">
        <p class="stat-label">Doanh thu (đơn đã thanh toán)</p>
        <p class="stat-value">{{ formatPrice(stats.totalRevenue) }}</p>
      </div>
      <div class="stat-card card">
        <p class="stat-label">Tổng sản phẩm</p>
        <p class="stat-value">{{ stats.totalProducts }}</p>
      </div>
    </div>

    <div class="admin-nav">
      <router-link to="/admin/products" class="admin-nav-card card">
        <span class="icon">👟</span>
        <h3>Quản lý sản phẩm</h3>
        <p>Thêm, sửa, xóa sản phẩm và biến thể size/màu</p>
      </router-link>
      <router-link to="/admin/orders" class="admin-nav-card card">
        <span class="icon">📦</span>
        <h3>Quản lý đơn hàng</h3>
        <p>Theo dõi và cập nhật trạng thái đơn hàng</p>
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue'
import api from '../../services/api'

const stats = reactive({
  totalOrders: 0,
  processingOrders: 0,
  totalRevenue: 0,
  totalProducts: 0
})

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

async function loadStats() {
  try {
    const [ordersRes, productsRes] = await Promise.all([
      api.get('/orders'),
      api.get('/products', { params: { pageSize: 1 } })
    ])
    const orders = ordersRes.data
    stats.totalOrders = orders.length
    stats.processingOrders = orders.filter(o => o.orderStatus === 'Processing').length
    stats.totalRevenue = orders.filter(o => o.paymentStatus === 'Paid').reduce((sum, o) => sum + o.totalAmount, 0)
    stats.totalProducts = productsRes.data.totalCount
  } catch (err) {
    console.error('Lỗi tải thống kê:', err)
  }
}

onMounted(loadStats)
</script>

<style scoped>
.admin-page { padding: 32px 24px 80px; }
.page-title { font-size: 32px; margin-bottom: 28px; }

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
  margin-bottom: 40px;
}
.stat-card { padding: 24px; }
.stat-label { font-size: 13px; color: var(--text-muted); margin-bottom: 10px; }
.stat-value { font-size: 26px; font-weight: 800; color: var(--accent-orange); }

.admin-nav { display: grid; grid-template-columns: repeat(2, 1fr); gap: 20px; }
.admin-nav-card { padding: 32px; display: block; transition: transform 0.15s ease; }
.admin-nav-card:hover { transform: translateY(-4px); }
.admin-nav-card .icon { font-size: 32px; display: block; margin-bottom: 16px; }
.admin-nav-card h3 { margin-bottom: 8px; }
.admin-nav-card p { color: var(--text-secondary); font-size: 14px; }

@media (max-width: 800px) {
  .stats-grid { grid-template-columns: repeat(2, 1fr); }
  .admin-nav { grid-template-columns: 1fr; }
}
</style>
