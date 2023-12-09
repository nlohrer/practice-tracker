<script setup>
import { reactive, ref } from "vue";
import SessionList from "./components/SessionList.vue";
import SessionForm from "./components/SessionForm.vue";

const show = ref(false);
const showText = ref("Show all data");
function toggleShow() {
  if (show.value) {
    show.value = false;
    showText.value = "Show all data";
  } else {
    show.value = true;
    showText.value = "Hide data";
  }

}
</script>

<template>
  <div class="container">
    <SessionForm />
    <div class="data-container">
      <button @click="toggleShow">{{ showText }}</button>
      <Suspense v-if="show" >
        <SessionList />
        <template #fallback>
          <p>Loading session data...</p>
        </template>
      </Suspense>
    </div>
  </div>
</template>

<style scoped>
.container {
  display: flex;
  gap: 2em;
}

.data-container {
  flex-grow: 1;
}
</style>