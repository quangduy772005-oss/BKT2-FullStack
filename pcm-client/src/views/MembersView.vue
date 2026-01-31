<template>
  <div>
    <h1>Members Page</h1>
    <button @click="logout">Logout</button>

    <ul>
      <li v-for="m in members" :key="m.id">
        {{ m.name }}
        <button @click="remove(m.id)">X</button>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { getMembers, deleteMember } from "../api/memberApi";
import { useRouter } from "vue-router";

const members = ref([]);
const router = useRouter();

onMounted(async () => {
  const res = await getMembers();
  members.value = res.data;
});

const remove = async (id) => {
  await deleteMember(id);
  members.value = members.value.filter(m => m.id !== id);
};

const logout = () => {
  localStorage.removeItem("token");
  router.push("/login");
};
</script>
