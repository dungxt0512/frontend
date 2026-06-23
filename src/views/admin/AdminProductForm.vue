<template>
  <div class="container admin-page">
    <h1 class="heading-display page-title">{{ isEdit ? 'SỬA SẢN PHẨM' : 'THÊM SẢN PHẨM' }}</h1>

    <form class="product-form card" @submit.prevent="handleSubmit">
      <div class="form-row">
        <div class="form-group">
          <label class="form-label">Tên sản phẩm</label>
          <input v-model="form.productName" type="text" class="form-input" required />
        </div>
        <div class="form-group">
          <label class="form-label">Ảnh đại diện (URL)</label>
          <input v-model="form.imageUrl" type="text" class="form-input" placeholder="/images/products/ten-anh.jpg" />
        </div>
      </div>

      <div class="form-group">
        <label class="form-label">Mô tả</label>
        <textarea v-model="form.description" class="form-input" rows="3"></textarea>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label class="form-label">Giá gốc (VNĐ)</label>
          <input v-model.number="form.price" type="number" class="form-input" required min="0" />
        </div>
        <div class="form-group">
          <label class="form-label">Giá khuyến mãi (nếu có)</label>
          <input v-model.number="form.discountPrice" type="number" class="form-input" min="0" />
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label class="form-label">Hãng</label>
          <select v-model.number="form.brandId" class="form-input" required>
            <option value="" disabled>Chọn hãng</option>
            <option v-for="b in brands" :key="b.brandId" :value="b.brandId">{{ b.brandName }}</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label">Loại giày</label>
          <select v-model.number="form.categoryId" class="form-input" required>
            <option value="" disabled>Chọn loại</option>
            <option v-for="c in categories" :key="c.categoryId" :value="c.categoryId">{{ c.categoryName }}</option>
          </select>
        </div>
      </div>

      <h3 class="variants-title">Biến thể (Size / Màu / Tồn kho)</h3>
      <div class="variants-list">
        <div v-for="(v, i) in form.variants" :key="i" class="variant-row">
          <input v-model="v.size" type="text" placeholder="Size (VD: 40)" class="form-input" required />
          <input v-model="v.color" type="text" placeholder="Màu (VD: Đen)" class="form-input" required />
          <input v-model.number="v.quantity" type="number" placeholder="Số lượng" class="form-input" required min="0" />
          <button type="button" class="remove-variant" @click="form.variants.splice(i, 1)">✕</button>
        </div>
      </div>
      <button type="button" class="btn btn-secondary" @click="addVariant">+ Thêm biến thể</button>

      <p v-if="error" class="form-error">{{ error }}</p>

      <div class="form-actions">
        <router-link to="/admin/products" class="btn btn-secondary">Hủy</router-link>
        <button type="submit" class="btn btn-primary" :disabled="submitting">
          {{ submitting ? 'Đang lưu...' : 'Lưu sản phẩm' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { reactive, ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../../services/api'

const props = defineProps({ id: { type: [String, Number], default: null } })
const router = useRouter()

const isEdit = computed(() => !!props.id)
const brands = ref([])
const categories = ref([])
const submitting = ref(false)
const error = ref('')

const form = reactive({
  productName: '',
  description: '',
  price: 0,
  discountPrice: null,
  brandId: '',
  categoryId: '',
  imageUrl: '',
  variants: []
})

function addVariant() {
  form.variants.push({ size: '', color: '', quantity: 0 })
}

async function loadOptions() {
  const [brandsRes, categoriesRes] = await Promise.all([
    api.get('/products/brands'),
    api.get('/products/categories')
  ])
  brands.value = brandsRes.data
  categories.value = categoriesRes.data
}

async function loadProduct() {
  if (!isEdit.value) {
    addVariant() // bắt đầu với 1 dòng trống cho sản phẩm mới
    return
  }
  const { data } = await api.get(`/products/${props.id}`)
  form.productName = data.productName
  form.description = data.description
  form.price = data.price
  form.discountPrice = data.discountPrice
  form.imageUrl = data.imageUrl
  form.variants = data.variants.map(v => ({ size: v.size, color: v.color, quantity: v.quantity }))

  const brand = brands.value.find(b => b.brandName === data.brandName)
  const category = categories.value.find(c => c.categoryName === data.categoryName)
  form.brandId = brand?.brandId || ''
  form.categoryId = category?.categoryId || ''
}

async function handleSubmit() {
  if (form.variants.length === 0) {
    error.value = 'Cần thêm ít nhất một biến thể (size/màu).'
    return
  }

  submitting.value = true
  error.value = ''
  try {
    if (isEdit.value) {
      await api.put(`/products/${props.id}`, form)
    } else {
      await api.post('/products', form)
    }
    router.push('/admin/products')
  } catch (err) {
    error.value = err.response?.data?.message || 'Lưu sản phẩm thất bại.'
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  await loadOptions()
  await loadProduct()
})
</script>

<style scoped>
.admin-page { padding: 32px 24px 80px; max-width: 800px; margin: 0 auto; }
.page-title { font-size: 32px; margin-bottom: 24px; }
.product-form { padding: 32px; }

.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }

.variants-title { font-size: 16px; margin: 24px 0 12px; }
.variants-list { margin-bottom: 16px; }
.variant-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr auto;
  gap: 10px;
  margin-bottom: 10px;
  align-items: center;
}
.remove-variant {
  width: 36px; height: 36px;
  color: var(--danger);
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-sm);
}

.form-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 28px; }

@media (max-width: 700px) {
  .form-row, .variant-row { grid-template-columns: 1fr; }
}
</style>
