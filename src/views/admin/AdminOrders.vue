<template>
  <div class="container admin-page">
    <h1 class="heading-display page-title">QUẢN LÝ ĐƠN HÀNG</h1>

    <div class="filter-tabs">
      <button v-for="tab in tabs" :key="tab.value" :class="{ active: filterStatus === tab.value }" @click="filterStatus = tab.value; loadOrders()">
        {{ tab.label }}
      </button>
    </div>

    <div v-if="loading" class="state-msg">Đang tải...</div>
    <table v-else class="admin-table card">
      <thead>
        <tr>
          <th>Mã đơn</th>
          <th>Khách hàng</th>
          <th>Tổng tiền</th>
          <th>Thanh toán</th>
          <th>Trạng thái</th>
          <th>Ngày tạo</th>
          <th>Cập nhật</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="o in orders" :key="o.orderId">
          <td>{{ o.orderCode }}</td>
          <td>{{ o.customerName }}</td>
          <td>{{ formatPrice(o.totalAmount) }}</td>
          <td>
            <span class="badge" :class="o.paymentStatus === 'Paid' ? 'badge-success' : 'badge-warning'">
              {{ o.paymentStatus === 'Paid' ? 'Đã trả' : 'Chưa trả' }}
            </span>
          </td>
          <td>{{ statusLabel(o.orderStatus) }}</td>
          <td>{{ formatDate(o.createdAt) }}</td>
          <td>
            <select class="status-select" :value="o.orderStatus" @change="updateStatus(o, $event.target.value)">
              <option value="Processing">Đang xử lý</option>
              <option value="Shipping">Đang giao</option>
              <option value="Completed">Hoàn tất</option>
              <option value="Cancelled">Đã hủy</option>
            </select>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const orders = ref([])
const loading = ref(true)
const filterStatus = ref('')

const tabs = [
  { label: 'Tất cả', value: '' },
  { label: 'Đang xử lý', value: 'Processing' },
  { label: 'Đang giao', value: 'Shipping' },
  { label: 'Hoàn tất', value: 'Completed' },
  { label: 'Đã hủy', value: 'Cancelled' }
]

const statusMap = { Processing: 'Đang xử lý', Shipping: 'Đang giao', Completed: 'Hoàn tất', Cancelled: 'Đã hủy' }
function statusLabel(s) { return statusMap[s] || s }

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}
function formatDate(d) {
  return new Date(d).toLocaleDateString('vi-VN')
}

async function loadOrders() {
  loading.value = true
  try {
    const { data } = await api.get('/orders', { params: filterStatus.value ? { status: filterStatus.value } : {} })
    orders.value = data
  } finally {
    loading.value = false
  }
}

async function updateStatus(order, newStatus) {
  try {
    await api.put(`/orders/${order.orderId}/status`, { orderStatus: newStatus })
    order.orderStatus = newStatus
  } catch (err) {
    alert('Cập nhật trạng thái thất bại.')
  }
}

onMounted(loadOrders)
</script>

<style scoped>
.admin-page { padding: 32px 24px 80px; }
.page-title { font-size: 32px; margin-bottom: 20px; }

.filter-tabs { display: flex; gap: 8px; margin-bottom: 20px; flex-wrap: wrap; }
.filter-tabs button {
  padding: 8px 16px;
  border-radius: 999px;
  font-size: 14px;
  font-weight: 600;
  color: var(--text-secondary);
  border: 1px solid var(--border-subtle);
}
.filter-tabs button.active { background: var(--accent-orange); color: #1A1006; border-color: var(--accent-orange); }

.admin-table { width: 100%; border-collapse: collapse; }
.admin-table th, .admin-table td {
  padding: 14px 16px;
  text-align: left;
  border-bottom: 1px solid var(--border-subtle);
  font-size: 14px;
}
.admin-table th { color: var(--text-muted); font-weight: 600; text-transform: uppercase; font-size: 12px; }

.status-select {
  background: var(--bg-elevated-2);
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-sm);
  color: var(--text-primary);
  padding: 6px 10px;
  font-size: 13px;
}

.state-msg { text-align: center; padding: 60px 0; color: var(--text-muted); }
</style>
