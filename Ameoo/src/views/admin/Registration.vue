<script setup>
import { ref } from 'vue'
import { registrations } from '../../data/mockData.js'
const regs = ref([...registrations])
function approve(r) { r.status = 'Đã duyệt' }
function reject(r) { r.status = 'Từ chối' }
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Đăng ký khóa học</h2><p class="page-desc">Duyệt đơn đăng ký của học viên</p></div>
    <span class="badge badge-yellow" style="font-size:14px;padding:6px 14px">{{ regs.filter(r=>r.status==='Chờ duyệt').length }} chờ duyệt</span>
  </div>
  <div class="card">
    <table class="data-table">
      <thead><tr><th>HỌC VIÊN</th><th>KHÓA HỌC</th><th>CA HỌC</th><th>NGÀY ĐK</th><th>GHI CHÚ</th><th>TRẠNG THÁI</th><th>HÀNH ĐỘNG</th></tr></thead>
      <tbody>
        <tr v-for="r in regs" :key="r.id">
          <td class="text-bold">{{ r.studentName }}</td><td>{{ r.courseName }}</td><td>{{ r.requestClass }}</td>
          <td class="text-muted">{{ r.registerDate }}</td><td class="text-muted">{{ r.note || '—' }}</td>
          <td><span class="badge" :class="r.status==='Đã duyệt'?'badge-green':r.status==='Từ chối'?'badge-red':'badge-yellow'">{{ r.status }}</span></td>
          <td>
            <div v-if="r.status==='Chờ duyệt'" style="display:flex;gap:6px">
              <button class="btn-approve" @click="approve(r)">✓ Duyệt</button>
              <button class="btn-reject" @click="reject(r)">✗ Từ chối</button>
            </div>
            <span v-else class="text-muted">Đã xử lý</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
