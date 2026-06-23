<template>
  <div class="container product-page">
    <h1>DANH MỤC SẢN PHẨM</h1>

    <div class="layout">
      <aside class="filters card">
        <div class="filter-group">
          <h4>Hãng</h4>
          <label v-for="b in brands" :key="b.brandId" class="filter-check">
            <input
              type="checkbox"
              :checked="filters.brandId === b.brandId"
              @change="applyBrand(b.brandId)"
            />
            {{ b.brandName }}
          </label>
        </div>

        <div class="filter-group">
          <h4>Loại giày</h4>
          <label v-for="c in categories" :key="c.categoryId" class="filter-check">
            <input type="radio" name="category" :value="c.categoryId" v-model="filters.categoryId" @change="fetchProducts" />
            {{ c.categoryName }}
          </label>
          <button class="clear-link" @click="filters.categoryId = null; fetchProducts()">Bỏ lọc loại giày</button>
        </div>

        <div class="filter-group">
          <h4>Khoảng giá</h4>
          <div class="price-inputs">
            <input type="number" v-model.number="filters.minPrice" placeholder="Từ" class="form-input" />
            <input type="number" v-model.number="filters.maxPrice" placeholder="Đến" class="form-input" />
          </div>
          <button class="btn btn-secondary btn-block" style="margin-top:10px" @click="fetchProducts">Áp dụng</button>
        </div>

        <button class="clear-all" @click="resetFilters">Xóa tất cả bộ lọc</button>
      </aside>

      <!-- PRODUCT GRID -->
      <div class="content">
        <div class="toolbar">
          <p class="result-count">{{ totalCount }} sản phẩm</p>
          <select v-model="filters.sortBy" @change="fetchProducts" class="form-input sort-select">
            <option value="newest">Mới nhất</option>
            <option value="price_asc">Giá tăng dần</option>
            <option value="price_desc">Giá giảm dần</option>
          </select>
        </div>

        <div v-if="loading" class="state-msg">Đang tải...</div>
        <div v-else-if="products.length === 0" class="state-msg">
          Không tìm thấy sản phẩm phù hợp. Thử bỏ vài bộ lọc xem sao.
        </div>
        <div v-else class="product-grid">
          <ProductCard v-for="p in products" :key="p.productId" :product="p" />
        </div>

        <div v-if="totalPages > 1" class="pagination">
          <button :disabled="filters.page <= 1" @click="goToPage(filters.page - 1)">‹ Trước</button>
          <span>Trang {{ filters.page }} / {{ totalPages }}</span>
          <button :disabled="filters.page >= totalPages" @click="goToPage(filters.page + 1)">Sau ›</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import api from '../services/api'
import ProductCard from '../components/ProductCard.vue'

const route = useRoute()

const products = ref([])
const brands = ref([])
const categories = ref([])
const loading = ref(true)
const totalCount = ref(0)
const totalPages = ref(1)

const filters = reactive({
  keyword: route.query.keyword || '',
  brandId: route.query.brandId ? Number(route.query.brandId) : null,
  categoryId: route.query.categoryId ? Number(route.query.categoryId) : null,
  minPrice: null,
  maxPrice: null,
  sortBy: route.query.sortBy || 'newest',
  page: 1,
  pageSize: 12
})

function applyBrand(brandId) {
  filters.brandId = filters.brandId === brandId ? null : brandId
  filters.page = 1
  fetchProducts()
}

async function fetchProducts() {
  loading.value = true
  try {
    const { data } = await api.get('/products', { params: filters })
    products.value = data.items
    totalCount.value = data.totalCount
    totalPages.value = data.totalPages
  } catch (err) {
    console.error('Lỗi tải sản phẩm:', err)
  } finally {
    loading.value = false
  }
}

function goToPage(page) {
  filters.page = page
  fetchProducts()
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function resetFilters() {
  filters.keyword = ''
  filters.brandId = null
  filters.categoryId = null
  filters.minPrice = null
  filters.maxPrice = null
  filters.sortBy = 'newest'
  filters.page = 1
  fetchProducts()
}

async function loadFilterOptions() {
  const [brandsRes, categoriesRes] = await Promise.all([
    api.get('/products/brands'),
    api.get('/products/categories')
  ])
  brands.value = brandsRes.data
  categories.value = categoriesRes.data
}

watch(() => route.query, (q) => {
  filters.keyword = q.keyword || ''
  fetchProducts()
})

onMounted(() => {
  loadFilterOptions()
  fetchProducts()
})
</script>

<style scoped>
.product-page { padding: 32px 24px 80px; }
.page-title { font-size: 32px; margin-bottom: 24px; }

.layout {
  display: grid;
  grid-template-columns: 260px 1fr;
  gap: 32px;
}

.filters {
  padding: 24px;
  align-self: start;
  position: sticky;
  top: 96px;
}
.filter-group {
  margin-bottom: 28px;
  padding-bottom: 20px;
  border-bottom: 1px solid var(--border-subtle);
}
.filter-group h4 {
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: var(--text-muted);
  margin-bottom: 12px;
}
.filter-check {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: var(--text-secondary);
  margin-bottom: 10px;
  cursor: pointer;
}
.clear-link {
  font-size: 13px;
  color: var(--accent-blue);
  margin-top: 4px;
}
.price-inputs {
  display: flex;
  gap: 8px;
}
.clear-all {
  font-size: 14px;
  font-weight: 600;
  color: var(--accent-orange);
  width: 100%;
  text-align: center;
  padding: 10px 0;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.result-count { color: var(--text-secondary); font-size: 14px; }
.sort-select { width: 180px; }

.state-msg {
  text-align: center;
  color: var(--text-muted);
  padding: 60px 0;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 40px;
}
.pagination button {
  padding: 8px 16px;
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-sm);
  color: var(--text-secondary);
}
.pagination button:disabled { opacity: 0.4; cursor: not-allowed; }
.pagination button:not(:disabled):hover { border-color: var(--accent-orange); color: var(--accent-orange); }

@media (max-width: 900px) {
  .layout { grid-template-columns: 1fr; }
  .filters { position: static; }
}
</style>
