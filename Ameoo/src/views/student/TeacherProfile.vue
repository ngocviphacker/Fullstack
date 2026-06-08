<script setup>
const teacher = { name:'Nguyễn Thị Lan', specialty:'Tiếng Anh — IELTS/TOEIC', experience:8, rating:4.9, classes:4, students:87, email:'lan.nguyen@eduhub.vn', phone:'0901 234 567', office:'Phòng GV 201', hours:'8:00–17:00 (T2–T6)', tags:['IELTS 8.0','CELTA','TESOL','BA Ngôn ngữ'] }
</script>
<template>
<div>
  <h2 class="page-title">Giáo viên lớp</h2>
  <div class="grid-2" style="margin-top:20px">
    <div class="card" style="text-align:center">
      <div style="width:100px;height:100px;background:linear-gradient(135deg,#7c3aed,#4f46e5);border-radius:50%;display:flex;align-items:center;justify-content:center;font-size:36px;font-weight:700;color:#fff;margin:0 auto 16px">NL</div>
      <div class="text-bold" style="font-size:22px">{{ teacher.name }}</div>
      <div class="text-muted" style="margin:4px 0 12px">{{ teacher.specialty }}</div>
      <div style="display:flex;gap:4px;flex-wrap:wrap;justify-content:center;margin-bottom:16px">
        <span v-for="t in teacher.tags" :key="t" class="badge badge-purple">{{ t }}</span>
      </div>
      <div style="display:flex;gap:0;border-top:1px solid #f1f5f9;padding-top:16px;justify-content:space-around">
        <div style="text-align:center"><div class="text-bold" style="font-size:20px">{{ teacher.classes }}</div><div class="text-muted" style="font-size:12px">Lớp đang dạy</div></div>
        <div style="border-left:1px solid #e2e8f0;text-align:center;padding-left:20px"><div class="text-bold" style="font-size:20px">{{ teacher.students }}</div><div class="text-muted" style="font-size:12px">Học viên</div></div>
        <div style="border-left:1px solid #e2e8f0;text-align:center;padding-left:20px"><div class="text-bold" style="font-size:20px;color:#f59e0b">{{ teacher.rating }}★</div><div class="text-muted" style="font-size:12px">Đánh giá</div></div>
      </div>
    </div>
    <div class="card">
      <div class="card-title" style="margin-bottom:16px">Thông tin liên hệ</div>
      <div v-for="(item,i) in [{icon:'📧',label:'Email',val:teacher.email},{icon:'📞',label:'Điện thoại',val:teacher.phone},{icon:'🏫',label:'Phòng GV',val:teacher.office},{icon:'🕐',label:'Giờ làm việc',val:teacher.hours},{icon:'📅',label:'Kinh nghiệm',val:teacher.experience+' năm'}]" :key="i" style="display:flex;align-items:center;gap:14px;padding:12px 0;border-bottom:1px solid #f1f5f9">
        <span style="font-size:18px">{{ item.icon }}</span>
        <div><div class="text-muted" style="font-size:12px">{{ item.label }}</div><div class="text-bold">{{ item.val }}</div></div>
      </div>
    </div>
  </div>
</div>
</template>
