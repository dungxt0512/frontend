<template>
  <div class="container admin-page">
    <div class="page-header">
      <h1 class="heading-display page-title">QUẢN LÝ SẢN PHẨM</h1>
      <router-link to="/admin/products/new" class="btn btn-primary">+ Thêm sản phẩm</router-link>
    </div>

    <div v-if="loading" class="state-msg">Đang tải...</div>
    <table v-else class="admin-table card">
      <thead>
        <tr>
          <th>Ảnh</th>
          <th>Tên sản phẩm</th>
          <th>Hãng</th>
          <th>Giá</th>
          <th>Tồn kho</th>
          <th>Hành động</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="p in products" :key="p.productId">
          <td><img :src="p.imageUrl || placeholder" class="row-img" /></td>
          <td>{{ p.productName }}</td>
          <td>{{ p.brandName }}</td>
          <td>{{ formatPrice(p.discountPrice || p.price) }}</td>
          <td>
            <span class="badge" :class="p.inStock ? 'badge-success' : 'badge-danger'">
              {{ p.inStock ? 'Còn hàng' : 'Hết hàng' }}
            </span>
          </td>
          <td class="actions">
            <router-link :to="`/admin/products/${p.productId}/edit`" class="action-btn edit">Sửa</router-link>
            <button class="action-btn delete" @click="confirmDelete(p)">Xóa</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="totalPages > 1" class="pagination">
      <button :disabled="page <= 1" @click="page--; loadProducts()">‹ Trước</button>
      <span>Trang {{ page }} / {{ totalPages }}</span>
      <button :disabled="page >= totalPages" @click="page++; loadProducts()">Sau ›</button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../../services/api'

const products = ref([])
const loading = ref(true)
const page = ref(1)
const totalPages = ref(1)
const placeholder = 'https://placehold.co/100x100/161B26/6B7280?text=MV'

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

async function loadProducts() {
  loading.value = true
  try {
    const { data } = await api.get('/products', { params: { page: page.value, pageSize: 10, sortBy: 'newest' } })
    products.value = data.items
    totalPages.value = data.totalPages
  } finally {
    loading.value = false
  }
}

async function confirmDelete(product) {
  if (!window.confirm(`Xóa sản phẩm "${product.productName}"? Hành động này sẽ ẩn sản phẩm khỏi cửa hàng.`)) return
  try {
    await api.delete(`/products/${product.productId}`)
    await loadProducts()
  } catch (err) {
    alert(err.response?.data?.message || 'Xóa sản phẩm thất bại.')
  }
}

onMounted(loadProducts)
</script>

<style scoped>
.admin-page { padding: 32px 24px 80px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
.page-title { font-size: 32px; }

.admin-table { width: 100%; border-collapse: collapse; overflow: hidden; }
.admin-table th, .admin-table td {
  padding: 14px 16px;
  text-align: left;
  border-bottom: 1px solid var(--border-subtle);
  font-size: 14px;
}
.admin-table th { color: var(--text-muted); font-weight: 600; text-transform: uppercase; font-size: 12px; }
.row-img { width: 48px; height: 48px; object-fit: cover; border-radius: var(--radius-sm); }

.actions { display: flex; gap: 8px; }
.action-btn { padding: 6px 12px; border-radius: var(--radius-sm); font-size: 13px; font-weight: 600; }
.action-btn.edit { background: rgba(61,90,254,0.15); color: var(--accent-blue); }
.action-btn.delete { background: rgba(231,76,60,0.15); color: var(--danger); }

.pagination { display: flex; justify-content: center; align-items: center; gap: 20px; margin-top: 24px; }
.pagination button { padding: 8px 16px; border: 1px solid var(--border-subtle); border-radius: var(--radius-sm); color: var(--text-secondary); }
.pagination button:disabled { opacity: 0.4; }

.state-msg { text-align: center; padding: 60px 0; color: var(--text-muted); }
</style>
