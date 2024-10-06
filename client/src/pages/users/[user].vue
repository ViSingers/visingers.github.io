<script setup lang="ts">
import { onMounted, ref } from 'vue'
import axios from 'axios'

const { t } = useI18n()

const data = ref<any>(null)
const page = ref(1)
const loading = ref(true)

async function fetchUser() {
  try {
    const route = useRoute('/users/[user]')

    const userLogin = route.params.user
    const response = await axios.get(`${import.meta.env.VITE_VISINGERS_API_BASE_URL}/Users/${userLogin}`)
    data.value = response.data
    page.value++
  }
  catch (error) {
    console.error('Error fetching user:', error)
  }
  finally {
    loading.value = false
  }
}

onMounted(fetchUser)
</script>

<template>
  <div v-if="loading" class="mt-3 text-center">
    <b-spinner label="Loading..." />
  </div>
  <b-container v-else class="mt-3">
    <h1 class="text-center" m-8>
      {{ `${t('user')}: ${data.name}` }}
    </h1>
    <b-row class="mb-8">
      <singers-list :singers="data.singers" />
    </b-row>
    <div v-if="loading" class="mt-3 text-center">
      <b-spinner label="Loading..." />
    </div>
  </b-container>
</template>
