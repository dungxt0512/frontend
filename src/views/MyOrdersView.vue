<template>
  <div class="container orders-page">
    <h1 class="heading-display page-title">ĐƠN HÀNG CỦA TÔI</h1>

    <div v-if="loading" class="state-msg">Đang tải...</div>
    <div v-else-if="orders.length === 0" class="state-msg">
      Bạn chưa có đơn hàng nào. <router-link to="/products">Mua sắm ngay</router-link>
    </div>

    <div v-else class="orders-list">
      <div v-for="order in orders" :key="order.orderId" class="order-card card">
        <div class="order-header">
          <div>
            <p class="order-code">#{{ order.orderCode }}</p>
            <p class="order-date">{{ formatDate(order.createdAt) }}</p>
          </div>
          <div class="badges">
            <span class="badge" :class="statusBadgeClass(order.orderStatus)">{{ statusLabel(order.orderStatus) }}</span>
            <span class="badge" :class="order.paymentStatus === 'Paid' ? 'badge-success' : 'badge-warning'">
              {{ order.paymentStatus === 'Paid' ? 'Đã thanh toán' : 'Chưa thanh toán' }}
            </span>
          </div>
        </div>

        <div class="order-items">
          <div v-for="(d, i) in order.details" :key="i" class="order-line">
            <span>{{ d.productName }} ({{ d.size }}/{{ d.color }}) x{{ d.quantity }}</span>
            <span>{{ formatPrice(d.unitPrice * d.quantity) }}</span>
          </div>
        </div>

        <div class="order-footer">
          <span>Tổng: <strong>{{ formatPrice(order.totalAmount) }}</strong></span>
          <span class="ship-to">Giao tới: {{ order.shippingAddress }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const orders = ref([])
const loading = ref(true)

const statusMap = {
  Processing: { label: 'Đang xử lý', cls: 'badge-info' },
  Shipping: { label: 'Đang giao', cls: 'badge-warning' },
  Completed: { label: 'Hoàn tất', cls: 'badge-success' },
  Cancelled: { label: 'Đã hủy', cls: 'badge-danger' }
}

function statusLabel(s) { return statusMap[s]?.label || s }
function statusBadgeClass(s) { return statusMap[s]?.cls || 'badge-info' }

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}
function formatDate(d) {
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

onMounted(async () => {
  try {
    const { data } = await api.get('/orders/my-orders')
    orders.value = data
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.orders-page { padding: 32px 24px 80px; }
.page-title { font-size: 32px; margin-bottom: 24px; }
.state-msg { text-align: center; padding: 60px 0; color: var(--text-muted); }
.state-msg a { color: var(--accent-orange); font-weight: 600; }

.orders-list { display: flex; flex-direction: column; gap: 16px; }
.order-card { padding: 20px; }
.order-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px; padding-bottom: 16px; border-bottom: 1px solid var(--border-subtle); }
.order-code { font-weight: 700; font-size: 15px; }
.order-date { font-size: 13px; color: var(--text-muted); margin-top: 2px; }
.badges { display: flex; gap: 8px; }

.order-items { margin-bottom: 16px; }
.order-line { display: flex; justify-content: space-between; font-size: 14px; color: var(--text-secondary); margin-bottom: 8px; }

.order-footer { display: flex; justify-content: space-between; font-size: 14px; color: var(--text-secondary); }
.order-footer strong { color: var(--accent-orange); font-size: 16px; }
.ship-to { max-width: 50%; text-align: right; }

@media (max-width: 600px) {
  .order-footer { flex-direction: column; gap: 8px; }
  .ship-to { text-align: left; max-width: 100%; }
}
</style>
