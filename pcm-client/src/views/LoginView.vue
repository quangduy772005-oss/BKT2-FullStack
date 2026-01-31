<template>
  <div>
    <h2>Login</h2>
    <input v-model="username" placeholder="Username" />
    <input v-model="password" type="password" placeholder="Password" />
    <button @click="login">Login</button>
  </div>
</template>

<script setup>
import { ref } from "vue";
import axios from "../api/axios";
import { useRouter } from "vue-router";

const username = ref("");
const password = ref("");
const router = useRouter();

const login = async () => {
  const res = await axios.post("/Auth/login", {
    username: username.value,
    password: password.value,
  });

  localStorage.setItem("token", res.data.token);
  router.push("/members");
};
</script>
