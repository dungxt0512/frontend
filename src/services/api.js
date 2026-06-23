import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5067/api'
})

// Tự động gắn JWT token vào header của mọi request nếu user đã đăng nhập
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('maxverse_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Nếu token hết hạn (401), tự động đăng xuất và đưa về trang login
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('maxverse_token')
      localStorage.removeItem('maxverse_user')
      // Chỉ redirect nếu không đang ở trang login (tránh loop)
      if (!window.location.pathname.includes('/login')) {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

export default api
