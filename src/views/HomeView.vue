<template>
  <div class="home">
    <!-- HERO -->
    <section class="hero">
      <div class="container hero-inner">
        <div class="hero-text">
          <p class="eyebrow">Bộ sưu tập mới 2026</p>
          <h1>CHUYỂN ĐỘNG<br />KHÔNG GIỚI HẠN</h1>
          <p class="hero-sub">
            Hàng chính hãng Nike, Adidas, Puma, New Balance. Mỗi bước chạy, mỗi cú nhảy,
            mỗi khoảnh khắc đường phố — MaxVerse có đôi giày dành cho bạn.
          </p>
          <div class="hero-actions">
            <router-link to="/products" class="btn btn-primary">Khám phá ngay</router-link>
            <router-link to="/products?sortBy=newest" class="btn btn-secondary">Hàng mới về</router-link>
          </div>
        </div>
        <div class="hero-image">
          <img src="https://bizweb.dktcdn.net/thumb/1024x1024/100/449/472/products/49ece0fd-03df-49ea-b066-6f9b16f282d5.jpg?v=1652010429983" alt="Giày thể thao MaxVerse" />
        </div>
      </div>
    </section>

    <!-- BRANDS STRIP -->
    <section class="brands-strip container">
      <span v-for="b in brands" :key="b.brandId" class="brand-pill">{{ b.brandName }}</span>
    </section>

    <!-- FEATURED PRODUCTS -->
    <section class="container featured">
      <div class="section-head">
        <div>
          <p class="eyebrow">Bán chạy nhất</p>
          <h2>SẢN PHẨM NỔI BẬT</h2>
        </div>
        <router-link to="/products" class="btn btn-secondary">Xem tất cả →</router-link>
      </div>

      <div v-if="loading" class="state-msg">Đang tải sản phẩm...</div>
      <div v-else-if="products.length === 0" class="state-msg">Chưa có sản phẩm nào.</div>
      <div v-else class="product-grid">
        <ProductCard v-for="p in products" :key="p.productId" :product="p" />
      </div>
    </section>

    <!-- VALUE PROPS -->
    <section class="container value-props">
      <div class="value-item">
        <h3>Giao hàng nhanh 2-4 ngày</h3>
        <p>Miễn phí vận chuyển cho đơn từ 1.000.000đ</p>
      </div>
      <div class="value-item">
        <h3>Đổi trả trong 7 ngày</h3>
        <p>Đổi size miễn phí nếu chưa qua sử dụng</p>
      </div>
      <div class="value-item">
        <h3>100% chính hãng</h3>
        <p>Cam kết nguồn gốc rõ ràng, có hóa đơn</p>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'
import ProductCard from '../components/ProductCard.vue'

const products = ref([])
const brands = ref([])
const loading = ref(true)

async function loadData() {
  loading.value = true
  try {
    const [productsRes, brandsRes] = await Promise.all([
      api.get('/products', { params: { sortBy: 'newest', pageSize: 8 } }),
      api.get('/products/brands')
    ])
    products.value = productsRes.data.items
    brands.value = brandsRes.data
  } catch (err) {
    console.error('Lỗi tải dữ liệu trang chủ:', err)
  } finally {
    loading.value = false
  }
}

onMounted(loadData)
</script>

<style scoped>
.hero {
  background: linear-gradient(135deg, var(--bg-base) 0%, #131826 100%);
  padding: 64px 0;
  overflow: hidden;
}
.hero-inner {
  display: grid;
  grid-template-columns: 1fr 1fr;
  align-items: center;
  gap: 48px;
}
.hero-title {
  font-size: 64px;
  margin: 16px 0 24px;
}
.hero-sub {
  color: var(--text-secondary);
  font-size: 17px;
  max-width: 460px;
  margin-bottom: 32px;
}
.hero-actions {
  display: flex;
  gap: 16px;
}
.hero-image img {
  width: 100%;
  border-radius: var(--radius-lg);
}

.brands-strip {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  justify-content: center;
  padding: 32px 24px;
  border-bottom: 1px solid var(--border-subtle);
}
.brand-pill {
  padding: 8px 20px;
  border: 1px solid var(--border-subtle);
  border-radius: 999px;
  font-weight: 700;
  font-size: 14px;
  color: var(--text-secondary);
}

.featured { padding: 64px 24px; }
.section-head {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 32px;
}
.section-title { font-size: 32px; }

.state-msg {
  text-align: center;
  color: var(--text-muted);
  padding: 48px 0;
}

.value-props {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
  padding: 32px 24px 80px;
}
.value-item {
  text-align: center;
  padding: 32px 20px;
}
.value-icon { font-size: 32px; display: block; margin-bottom: 16px; }
.value-item h3 { font-size: 17px; margin-bottom: 8px; }
.value-item p { color: var(--text-secondary); font-size: 14px; }

@media (max-width: 800px) {
  .hero-inner { grid-template-columns: 1fr; }
  .hero-title { font-size: 44px; }
  .value-props { grid-template-columns: 1fr; }
}
</style>
