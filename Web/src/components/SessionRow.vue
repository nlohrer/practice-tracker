<script setup>
import SessionDeleteButton from './SessionDeleteButton.vue';

const props = defineProps({
    session: Object
})
const emit = defineEmits(["deleteRequest"]);
</script>

<template>
    <tr>
        <td>{{ session.task }}</td>
        <td>{{ session.duration.hours }}</td>
        <td>{{ session.duration.minutes }}</td>
        <td>{{ session.date }}</td>
        <td>{{ session.time }}</td>
    </tr>
    <SessionDeleteButton :session_id="session.id" @delete-successful="(success) => success
        ? console.log('Delete request successful') 
        : console.log('Delete request not successful - this item might have been deleted already.')" />
</template>

<style scoped>
tr {
    margin: 2px 10px;
    text-align: left;
}
</style>