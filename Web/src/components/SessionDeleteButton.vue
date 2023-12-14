<script setup>
const props = defineProps({
    session_id: Number
})
const emit = defineEmits(["deleteSuccessful"]);

async function deleteSession() {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/${props.session_id}`, { method: "DELETE" })
    if (response.status == 204) {
        emit("deleteSuccessful", true)
    } else {
        emit("deleteSuccessful", false)
    }
}
</script>

<template>
    <button @click="deleteSession">Delete</button>
</template>