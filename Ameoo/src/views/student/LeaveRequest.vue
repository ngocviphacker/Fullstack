<script setup>
import { ref } from 'vue'
import { leaveRequests } from '../../data/mockData.js'
const form = ref({ date:'', reason:'', detail:'', file:null })
const reasons = ['Bận việc gia đình','Khám sức khoẻ','Đi công tác','Lý do khác']
const myLeaves = ref(leaveRequests.filter((l,i) => i < 3))
const statusClass = s => s==='Đã duyệt'?'badge-green':s==='Từ chối'?'badge-red':'badge-yellow'
function submit() { if(form.value.date && form.value.reason) { myLeaves.value.unshift({id:Date.now(),studentName:'Nguyễn Văn An',course:'TOEIC 600+',leaveDate:form.value.date,reason:form.value.reason,sentAt:'Vừa xong',status:'Chờ duyệt'}); form.value={date:'',reason:'',detail:'',file:null}; alert('Đã gửi đơn xin nghỉ!') } }
</script>
<template>
<div>
  <div class="page-header"><h2 class="page-title">Xin nghỉ phép</h2><span class="badge badge-yellow">Phép còn lại: 2 buổi <span style="color:#94a3b8">(tối đa 3 / 12 tuần)</span></span></div>
  <div class="grid-2" style="margin-top:4px">
    <div class="card">
      <div class="card-title" style="margin-bottom:16px">Tạo đơn xin nghỉ mới</div>
      <div style="display:flex;flex-direction:column;gap:14px">
        <div><label style="display:block;font-size:13px;font-weight:500;margin-bottom:6px;color:#374151">Ngày nghỉ</label><input type="date" v-model="form.date" style="width:100%;border:1px solid #cbd5e1;border-radius:8px;padding:10px 12px;font-size:14px;outline:none" /></div>
        <div><label style="display:block;font-size:13px;font-weight:500;margin-bottom:6px;color:#374151">Lý do</label><select v-model="form.reason" style="width:100%;border:1px solid #cbd5e1;border-radius:8px;padding:10px 12px;font-size:14px;outline:none"><option value="">-- Chọn lý do --</option><option v-for="r in reasons" :key="r">{{ r }}</option></select></div>
        <div><label style="display:block;font-size:13px;font-weight:500;margin-bottom:6px;color:#374151">Chi tiết (tuỳ chọn)</label><textarea v-model="form.detail" style="width:100%;border:1px solid #cbd5e1;border-radius:8px;padding:10px 12px;font-size:14px;outline:none;resize:vertical;min-height:80px" placeholder="Mô tả thêm..."></textarea></div>
        <div><label style="display:block;font-size:13px;font-weight:500;margin-bottom:6px;color:#374151">Đính kèm minh chứng</label><div style="border:2px dashed #cbd5e1;border-radius:8px;padding:16px;text-align:center;color:#94a3b8;cursor:pointer;font-size:13px">📎 Chọn file (JPG, PDF — tối đa 5MB)</div></div>
        <button @click="submit" style="background:#0d9488;color:#fff;border:none;padding:12px;border-radius:8px;font-size:14px;font-weight:600;cursor:pointer;transition:.15s">✈ Gửi đơn xin nghỉ</button>
      </div>
    </div>
    <div class="card">
      <div class="card-title" style="margin-bottom:16px">Lịch sử đơn xin nghỉ</div>
      <div v-for="l in myLeaves" :key="l.id" style="padding:12px;margin-bottom:8px;border-radius:10px;border:1px solid #f1f5f9;background:#f8fafc">
        <div style="display:flex;justify-content:space-between;align-items:flex-start">
          <div><div class="text-bold">{{ l.leaveDate }}</div><div class="text-muted">{{ l.reason }}</div><div class="text-muted" style="font-size:11px;margin-top:2px">Gửi: {{ l.sentAt }}</div></div>
          <span class="badge" :class="statusClass(l.status)">{{ l.status }}</span>
        </div>
      </div>
    </div>
  </div>
</div>
</template>
