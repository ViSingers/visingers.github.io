<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue'
import axios from 'axios'
import 'vidstack/bundle'

const loading = ref(true)
const data = ref<any>(null)
const settingsStore = useSettingsStore()
const { t } = useI18n()
const currentIndex = ref(1)
const visible = ref(false)
async function fetchData() {
  try {
    const route = useRoute('/[user]/[repository]')

    const userLogin = route.params.user
    const repositoryName = route.params.repository
    const response = await axios.get(`${import.meta.env.VITE_VISINGERS_API_BASE_URL}/singers/${userLogin}/${repositoryName}`)
    data.value = response.data
  }
  catch (error) {
    console.error('Error fetching data:', error)
  }
  finally {
    loading.value = false
  }
}

onMounted(fetchData)

function showImg(index: number) {
  currentIndex.value = index
  visible.value = true
}

function handleHide() {
  visible.value = false
}

const currentLanguageDetails = computed(() => {
  const details = data.value.details[settingsStore.language] || data.value.details.en
  details.generalInfoTable = details.generalInfo?.map((row: string) => row.split(':')) ?? []
  details.termsOfUseTable = details.termsOfUse?.map((row: string) => row.split(':')) ?? []
  return details
})

const images = computed(() => {
  return data.value.imageUrls.map((url: string) => {
    return Object({ src: url, title: url.split('/').pop()?.split('.')[0].replace('_', ' ') })
  })
})
</script>

<template>
  <div v-if="loading" class="mt-3 text-center">
    <b-spinner label="Loading..." />
  </div>

  <b-container v-else class="bg-background text-foreground p-6">
    <b-row class="flex-column flex-md-row mb-8">
      <b-col cols="12" md="auto" class="mb-md-0 mr-md-6 mb-4">
        <b-img lazy :src="data.avatarUrl" alt="Avatar" rounded="circle" class="mb-md-0 avatar object-fit-cover mb-4" />
      </b-col>
      <b-col class="align-items-center">
        <h1 class="display-4 font-weight-bold mb-2">
          {{ data.name }}
        </h1>
        <p class="text-muted-foreground">
          <span style="white-space: pre-line">{{ currentLanguageDetails.description }}</span>
        </p>
        <p>
          {{ t('singer.author') }}: <router-link class="link" :to="{ path: `/users/${data.creatorLogin}` }">
            {{ data.creatorName }}
          </router-link>
        </p>
        <p v-if="data.siteUrl">
          {{ t('singer.site') }}: <a class="link" :href="data.siteUrl">{{ t('singer.siteLink') }}</a>
        </p>
        <p>
          {{ t('singer.repository') }}: <a class="link" :href="`https://github.com/${data.creatorLogin}/${data.repositoryName}`">{{ t('singer.siteLink') }}</a>
        </p>
      </b-col>
    </b-row>

    <b-row v-if="currentLanguageDetails.generalInfoTable.length !== 0" class="mb-8">
      <h3>
        {{ t("singer.information") }}
      </h3>
      <div class="overflow-x-auto">
        <b-table :items="currentLanguageDetails.generalInfoTable" thead-class="d-none" striped hover />
      </div>
    </b-row>

    <b-row v-if="currentLanguageDetails.termsOfUseTable.length !== 0" class="mb-8">
      <h3>
        {{ t("singer.terms-of-use") }}
      </h3>
      <div class="overflow-x-auto">
        <b-table :items="currentLanguageDetails.termsOfUseTable" thead-class="d-none" striped hover />
      </div>
    </b-row>

    <b-row class="mb-8">
      <h3>{{ t('singer.voicebanks') }}</h3>
      <b-card v-for="voicebank in data.voicebanks" :key="voicebank.name" class="card-shadow mb-3 rounded-4 p-3">
        <b-card-title>
          {{ voicebank.name }}
          <b-badge v-for="voicebankLanguage in voicebank.languages" :key="voicebankLanguage" class="rounded-pill badge-language">
            {{ voicebankLanguage }}
          </b-badge>
          <b-badge class="rounded-pill badge-voicebank">
            {{ voicebank.type }}
          </b-badge>
        </b-card-title>
        <b-card-text>{{ voicebank.description[settingsStore.language] || voicebank.description.en }}</b-card-text>
        <div v-if="voicebank.sampleUrls.length > 0">
          <h5>{{ t('singer.samples') }}</h5>
          <div v-for="sampleUrl in voicebank.sampleUrls" :key="sampleUrl">
            <media-player :src="sampleUrl" class="mt-2" view-type="audio" :title="sampleUrl.split('/').pop().split('.')[0].replace('_', ' ')">
              <media-provider />
              <media-audio-layout />
            </media-player>
          </div>
        </div>
        <div class="mt-4">
          <b-button :href="voicebank.url">
            {{ t('singer.download') }}
          </b-button>
        </div>
      </b-card>
    </b-row>

    <b-row v-if="data.videoUrls.length > 0" fluid class="mb-8">
      <h3>{{ t('singer.videos') }}</h3>
      <b-col
        v-for="video in data.videoUrls"
        :key="video"
        class="embed-responsive embed-responsive-16by9 col-xl-3"
      >
        <iframe
          class="embed-responsive-item card-shadow rounded-4"
          :src="`https://www.youtube.com/embed/${video}`"
          allowfullscreen
          width="100%"
        />
      </b-col>
    </b-row>
    <vue-easy-lightbox
      esc-disabled
      move-disabled
      :imgs="images"
      :index="currentIndex"
      :visible="visible"
      @hide="handleHide"
    />
    <b-row v-if="data.imageUrls.length > 0" class="mb-8">
      <h3>
        {{ t('singer.gallery') }}
      </h3>
      <masonry-wall :items="data.imageUrls" :column-width="300" :gap="16">
        <template #default="{ item, index }">
          <b-img lazy class="pic card-shadow rounded-3" :src="`${item as string}`" @click="() => showImg(index)" />
        </template>
      </masonry-wall>
    </b-row>
  </b-container>
</template>

<style scoped>
.badge {
  margin: 2px;
  padding: 5px 15px;
}

.badge-language {
  background-color: #ff679d !important;
}

.badge-voicebank {
  background-color: #4ea6ea !important;
}
.embed-responsive {
  padding: 10px;
}

.card-shadow {
  box-shadow: 0 0.3rem 0.7rem rgba(0, 0, 0, 0.15);
}

.avatar {
  width: 200px;
  height: 200px;
}

.pic {
  cursor: pointer;
  transition:
    transform 0.3s ease-in-out,
    box-shadow 0.3s ease-in-out;
}

.pic:hover {
  transform: translateY(-7px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.2);
}

h3 {
  margin-bottom: 20px;
}
</style>
