<script setup>
import { ref } from 'vue'
import { scoresTeacher } from '../../data/mockData.js'
const scores = ref(scoresTeacher.map(s => ({...s})))
const gradeClass = g => g==='Giỏi'?'badge-green':g==='Khá'?'badge-blue':'badge-yellow'
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Nhập điểm kiểm tra</h2><p class="page-desc">Kiểm tra giữa kỳ — IELTS Intermediate</p></div>
    <div class="btn-actions">
      <select style="border:1px solid #cbd5e1;border-radius:8px;padding:8px 12px;font-size:13px"><option>IELTS Intermediate</option></select>
      <select style="border:1px solid #cbd5e1;border-radius:8px;padding:8px 12px;font-size:13px"><option>KT Giữa kỳ</option></select>
      <button class="btn-primary">💾 Lưu điểm</button>
    </div>
  </div>
  <div class="card">
    <table class="data-table">
      <thead><tr><th>#</th><th>HỌ TÊN</th><th>MÃ HV</th><th>LISTENING</th><th>READING</th><th>WRITING</th><th>SPEAKING</th><th>TỔNG</th><th>XẾP LOẠI</th></tr></thead>
      <tbody>
        <tr v-for="(s,i) in scores" :key="s.id">
          <td class="text-muted">{{ i+1 }}</td>
          <td><div class="user-cell"><div class="avatar purple">{{ s.name.split(' ').map(w=>w[0]).slice(-2).join('') }}</div><span class="text-bold">{{ s.name }}</span></div></td>
          <td class="text-muted">{{ s.code }}</td>
          <td><input class="input-score" v-model.number="s.listening" /></td>
          <td><input class="input-score" v-model.number="s.reading" /></td>
          <td><input class="input-score" v-model.number="s.writing" /></td>
          <td><input class="input-score" v-model.number="s.speaking" /></td>
          <td><span class="text-bold" :class="s.total>=8?'text-green':'text-blue'" style="font-size:18px">{{ s.total }}</span></td>
          <td><span class="badge" :class="gradeClass(s.grade)">{{ s.grade }}</span></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
