<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useIntersectionObserver } from '@vueuse/core'
import axios from 'axios'
import Multiselect from '@vueform/multiselect'

const { t } = useI18n()

const singers = ref<any[]>([])
const page = ref(1)
const loading = ref(false)
const finished = ref(false)
const searchQuery = ref('')
const selectedTags = ref<string[]>([])
const selectedTypes = ref<string[]>([])
const selectedLanguages = ref<string[]>([])
const selectedSortType = ref<string>('popular')
const tags = ref<string[]>([])
const types = ref<string[]>([])
const languages = ref<string[]>([])
const sortTypes = ['popular', 'voicebank-count', 'recently-updated', 'new', 'old']

const translatedSortTypes = computed(() =>
  sortTypes.map(sortType => ({
    value: sortType,
    label: t(`sort.${sortType}`),
  })),
)

async function fetchData() {
  tags.value = (await axios.get(`api/tags`, {
    baseURL: 'https://visingers-api.megageorgio.ru/',
  })).data
  types.value = (await axios.get(`api/voicebankTypes`, {
    baseURL: 'https://visingers-api.megageorgio.ru/',
  })).data
  languages.value = (await axios.get(`api/voicebankLanguages`, {
    baseURL: 'https://visingers-api.megageorgio.ru/',
  })).data
}

async function fetchSingers() {
  if (loading.value || finished.value)
    return

  loading.value = true
  try {
    let requestUrl = `api/singers?page=${page.value}&sort=${selectedSortType.value}`
    if (searchQuery.value) {
      requestUrl += `&name=${searchQuery.value}`
    }
    if (selectedTags.value.length > 0) {
      selectedTags.value.forEach((tag) => {
        requestUrl += `&tags=${tag}`
      })
    }
    if (selectedTypes.value.length > 0) {
      selectedTypes.value.forEach((type) => {
        requestUrl += `&types=${type}`
      })
    }
    if (selectedLanguages.value.length > 0) {
      selectedLanguages.value.forEach((lang) => {
        requestUrl += `&languages=${lang}`
      })
    }

    const response = await axios.get(requestUrl, {
      baseURL: 'https://visingers-api.megageorgio.ru/',
    })
    if (response.data.items.length === 0) {
      finished.value = true
    }
    else {
      singers.value.push(...response.data.items)
      page.value++
    }
  }
  catch (error) {
    console.error('Error fetching singers:', error)
  }
  finally {
    loading.value = false
  }
}

function filterResults() {
  page.value = 1
  singers.value = []
  finished.value = false
  fetchSingers()
}

const loadMoreRef = ref<HTMLElement | null>(null)
useIntersectionObserver(
  loadMoreRef,
  ([{ isIntersecting }]) => {
    if (isIntersecting) {
      fetchSingers()
    }
  },
  {
    threshold: 0.1,
  },
)

onMounted(() => {
  fetchData()
  fetchSingers()
})
</script>

<template>
  <b-container class="mt-3">
    <b-row class="mb-8">
      <b-form class="d-flex align-items-center" inline>
        <b-form-input
          v-model="searchQuery"
          style="height: 40px"
          :placeholder="`${t('placeholder.name')}`" class="me-2" @blur="filterResults" @keyup.enter="filterResults"
        />
        <Multiselect
          v-model="selectedLanguages"
          :options="languages"
          mode="tags"
          :searchable="true"
          :placeholder="`${t('placeholder.languages')}`"
          class="languages-select me-2"
          @update:model-value="filterResults"
        />
        <Multiselect
          v-model="selectedTypes"
          :options="types"
          mode="tags"
          :searchable="true"
          :placeholder="`${t('placeholder.types')}`"
          class="types-select me-2"
          @update:model-value="filterResults"
        />
        <Multiselect
          v-model="selectedTags"
          :options="tags"
          mode="tags"
          :searchable="true"
          :placeholder="`${t('placeholder.tags')}`"
          class="tags-select me-2"
          @update:model-value="filterResults"
        />

        <Multiselect
          v-model="selectedSortType"
          :options="translatedSortTypes"
          :can-clear="false"
          class="tags-select me-2"
          label-prop="wfe"
          @update:model-value="filterResults"
        />
      </b-form>
    </b-row>
    <b-row class="mb-8">
      <b-col
        v-for="singer in singers"
        :key="singer.repositoryName"
        class="col-md-3 mb-3"
      >
        <router-link :to="{ path: `/${singer.creatorLogin}/${singer.repositoryName}` }" class="text-decoration-none">
          <b-card
            :title="singer.name"
            :img-src="singer.avatarUrl"
            img-height="200px"
            img-width="200px"
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
    <div ref="loadMoreRef" class="load-more-trigger" />
  </b-container>
</template>

<style src="@vueform/multiselect/themes/default.css"></style>

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

.card-img-top {
  width: 100%;
  height: 500px;
  object-fit: cover;
}
</style>
