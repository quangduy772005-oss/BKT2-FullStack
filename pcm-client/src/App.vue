<template>
  <div class="app">
    <Navbar v-if="showNavbar" />
    <router-view />
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
import Navbar from "./components/Navbar.vue";

const route = useRoute();

const showNavbar = computed(() => {
  return route.path !== "/login";
});
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html,
body {
  height: 100%;
}

#app {
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}

.app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}
</style>
