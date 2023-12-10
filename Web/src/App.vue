<script setup>
import { reactive, ref } from "vue";
import SessionList from "./components/SessionList.vue";
import SessionForm from "./components/SessionForm.vue";

const loggedIn = ref(false);
const username = ref("");

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

function toggleLogIn() {
  if (!loggedIn.value) {
    if (username.value == "") {
      return;
    }
    loggedIn.value = true;
  } else {
    loggedIn.value = false;
  }
}
</script>

<template>
  <div class="logged-in-container" v-if="loggedIn">
    <div class="form-switch-container">
      <SessionForm :username="username" />
      <button @click="toggleLogIn">Switch user</button>
    </div>
    <div class="data-container">
      <button @click="toggleShow">{{ showText }}</button>
      <Suspense v-if="show" >
        <SessionList :username="username" />
        <template #fallback>
          <p>Loading session data...</p>
        </template>
      </Suspense>
    </div>
  </div>
  <div v-else>
    <p>Not logged in. Please provide a username:</p>
    <form action="javascript:void(0)" @submit="toggleLogIn" class="login-form">
      <label for="username">Username:</label>
      <input type="text" name="username" id="username" v-model="username"/>
      <input type="submit" name="submit" id="submit" value="Enter"/>
    </form>
    <p class="error-message" v-if="username.length == 0">Username must be at least 1 character long.</p>
  </div>
</template>

<style scoped>
.logged-in-container {
  display: flex;
  gap: 2em;
}

.data-container {
  flex-grow: 1;
}

.login-form {
  display: flex;
  gap: 0.5em;
}

.error-message {
  margin-top: 0.2em;
  color: red;
}
</style>