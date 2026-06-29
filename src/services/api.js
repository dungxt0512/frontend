import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5067/api'
})
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('maxverse_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('maxverse_token')
      localStorage.removeItem('maxverse_user')
      if (!window.location.pathname.includes('/login')) {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

export default api
