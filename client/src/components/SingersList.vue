<script setup lang="ts">
import { defineProps } from 'vue'

const props = defineProps<Props>()

interface Props {
  singers: any[]
}
</script>

<template>
  <b-container>
    <b-row>
      <b-col
        v-for="singer in props.singers"
        :key="singer.repositoryName"
        class="mb-3"
        cols="12"
        sm="12"
        md="6"
        lg="4"
        xl="3"
      >
        <router-link :to="{ path: `/${singer.creatorLogin}/${singer.repositoryName}` }" class="text-decoration-none">
          <b-card
            :title="singer.name"
            header-tag="header"
            img-height="200px"
            img-width="200px"
            img-alt="Avatar"
            img-top
            class="singer-card h-100 shadow-md"
            lazy
          >
            <template #header>
              <b-card-img lazy :src="singer.avatarUrl" top />
            </template>
            <b-card-subtitle>
              <router-link :to="{ path: `/users/${singer.creatorLogin}` }" class="link">
                {{ singer.creatorName }}
              </router-link>
            </b-card-subtitle>
            <b-card-body class="card-tags">
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
  </b-container>
</template>

<style src="@vueform/multiselect/themes/default.css"></style>

<style scoped>
.card-body {
  padding: 0px;
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
</style>
