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
const oldSearchQuery = ref('')
const selectedTags = ref<string[]>([])
const selectedTypes = ref<string[]>([])
const selectedLanguages = ref<string[]>([])
const selectedSortType = ref<string>('popular')
const showIfNoVoicebanks = ref(true)
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
  tags.value = (await axios.get(`${import.meta.env.VITE_VISINGERS_API_BASE_URL}/tags`)).data
  types.value = (await axios.get(`${import.meta.env.VITE_VISINGERS_API_BASE_URL}/voicebankTypes`)).data
  languages.value = (await axios.get(`${import.meta.env.VITE_VISINGERS_API_BASE_URL}/voicebankLanguages`)).data
}

async function fetchSingers() {
  if (loading.value || finished.value)
    return

  loading.value = true
  try {
    let requestUrl = `${import.meta.env.VITE_VISINGERS_API_BASE_URL}/singers?page=${page.value}&sort=${selectedSortType.value}`
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
    if (showIfNoVoicebanks.value) {
      requestUrl += `&showIfNoVoicebanks=${showIfNoVoicebanks.value}`
    }

    const response = await axios.get(requestUrl)
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

function filterResultsFromBlur() {
  if (searchQuery.value !== oldSearchQuery.value) {
    oldSearchQuery.value = searchQuery.value
    filterResults()
  }
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
      <b-button v-b-toggle.collapse-1 variant="outline-secondary" class="m-2">
        {{ t('button.search') }}
      </b-button>
      <b-collapse id="collapse-1">
        <b-card>
          <b-form class="d-lg-flex" inline>
            <Multiselect
              v-model="selectedLanguages"
              :options="languages"
              mode="tags"
              :searchable="true"
              :placeholder="`${t('filters.languages')}`"
              class="languages-select mb-2 me-2"
              @update:model-value="filterResults"
            />
            <Multiselect
              v-model="selectedTypes"
              :options="types"
              mode="tags"
              :searchable="true"
              :placeholder="`${t('filters.types')}`"
              class="types-select mb-2 me-2"
              @update:model-value="filterResults"
            />
            <Multiselect
              v-model="selectedTags"
              :options="tags"
              mode="tags"
              :searchable="true"
              :placeholder="`${t('filters.tags')}`"
              class="tags-select mb-2 me-2"
              @update:model-value="filterResults"
            />
          </b-form>
          <b-form class="d-lg-flex" inline>
            <b-form-input
              v-model="searchQuery"
              class="mb-2 me-2 h-auto"
              :placeholder="`${t('filters.name')}`" @blur="filterResultsFromBlur" @keyup.enter="filterResults"
            />
            <Multiselect
              v-model="selectedSortType"
              :options="translatedSortTypes"
              :can-clear="false"
              class="tags-select mb-2 me-2"
              label-prop="wfe"
              @update:model-value="filterResults"
            />
            <b-form-group class="align-items-center d-flex mb-2 me-2" style="min-width: 250px;">
              <b-form-checkbox
                v-model="showIfNoVoicebanks"
                switch
                inline
              >
                {{ t('filters.show-if-no-voicebanks') }}
              </b-form-checkbox>
            </b-form-group>
          </b-form>
        </b-card>
      </b-collapse>
    </b-row>
    <b-row class="mb-8">
      <singers-list :singers="singers" />
    </b-row>
    <div v-if="loading" class="mb-2 mt-3 text-center">
      <b-spinner label="Loading..." />
    </div>
    <div ref="loadMoreRef" class="load-more-trigger" />
  </b-container>
</template>
