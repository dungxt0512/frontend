<template>
  <div class="container return-page">
    <div class="result-card card">
      <div v-if="loading" class="state">
        <p>Đang xác nhận kết quả thanh toán...</p>
      </div>
      <div v-else-if="success" class="state success">
        <h2>Thanh toán thành công!</h2>
        <p>Mã đơn hàng: <strong>{{ orderCode }}</strong></p>
        <router-link to="/my-orders" class="btn btn-primary">Xem đơn hàng của tôi</router-link>
      </div>
      <div v-else class="state failed">
        <h2>Thanh toán không thành công</h2>
        <p>{{ errorMessage }}</p>
        <router-link to="/cart" class="btn btn-secondary">Quay lại giỏ hàng</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '../services/api'

const route = useRoute()
const loading = ref(true)
const success = ref(false)
const orderCode = ref('')
const errorMessage = ref('')

onMounted(async () => {
  try {
    const queryString = window.location.search
    const { data } = await api.get(`/orders/vnpay-return${queryString}`)
    success.value = data.success
    orderCode.value = data.orderCode
    if (!data.success) errorMessage.value = data.message
  } catch (err) {
    success.value = false
    errorMessage.value = err.response?.data?.message || 'Không thể xác nhận giao dịch.'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.return-page { padding: 64px 24px; display: flex; justify-content: center; }
.result-card { padding: 48px; max-width: 480px; text-align: center; }
.icon { font-size: 48px; display: block; margin-bottom: 16px; }
.state h2 { margin-bottom: 12px; }
.state p { color: var(--text-secondary); margin-bottom: 24px; }
</style>
