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
    const response = await axios.get(`/api/Users/${userLogin}`)
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
    <b-row>
      <b-col
        v-for="singer in data.singers"
        :key="singer.repositoryName"
        class="col-md-3 mb-3"
      >
        <router-link :to="{ path: `/${singer.creatorLogin}/${singer.repositoryName}` }" class="text-decoration-none">
          <b-card
            :title="singer.name"
            :img-src="singer.avatarUrl"
            img-alt="Avatar"
            img-top
            class="singer-card h-100 shadow-md"
          >
            <b-card-subtitle>
              <router-link :to="{ path: `/users/${singer.creatorLogin}` }" class="link">
                {{ singer.creatorName }}
              </router-link>
            </b-card-subtitle>
            <b-card-body>
              <b-card-text>
                <b-badge v-for="voicebankLanguage in singer.voicebankLanguages" :key="voicebankLanguage" class="rounded-pill badge-language">
                  {{ voicebankLanguage }}
                </b-badge>
                <b-badge v-for="voicebankType in singer.voicebankTypes" :key="voicebankType" class="rounded-pill badge-voicebank">
                  {{ voicebankType }}
                </b-badge>
                <b-badge v-for="tag in singer.tags" :key="tag" class="rounded-pill">
                  {{ tag }}
                </b-badge>
              </b-card-text>
            </b-card-body>
            <b-card-body>
              <b-card-text class="stars-container">
                <div class="d-flex align-items-center justify-content-end star-text">
                  {{ singer.stars }} <span i-carbon-star-filled class="star-icon" />
                </div>
              </b-card-text>
            </b-card-body>
          </b-card>
        </router-link>
      </b-col>
    </b-row>
    <div v-if="loading" class="mt-3 text-center">
      <b-spinner label="Loading..." />
    </div>
  </b-container>
</template>

<style scoped>
.singer-card {
  border-radius: 15px;
  overflow: hidden;
  padding-bottom: 40px;
  transition:
    transform 0.3s ease-in-out,
    box-shadow 0.3s ease-in-out;
}
.stars-container {
  position: absolute; /* Абсолютное позиционирование для блока со звездами */
  bottom: 0; /* Закрепляем внизу карточки */
  left: 0;
  right: 0;
  padding: 10px; /* Добавим немного отступа */
}

.star-text {
  display: flex;
  justify-content: flex-end;
  align-items: center;
}
.singer-card .card-body {
  padding: 0px;
}

.singer-card:hover {
  transform: translateY(-7px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.2);
}

.load-more-trigger {
  height: 1px;
}

.badge {
  margin: 2px;
}

.badge-language {
  background-color: #ff679d !important;
}

.badge-voicebank {
  background-color: #4ea6ea !important;
}

.star-icon {
  color: #ffc800 !important;
}

.star-text {
  font-size: 20px;
}
</style>
