<script lang="ts" setup>
const { t } = useI18n()
</script>

<template>
  <b-container p="6">
    <h1 class="display-4 mb-10">
      {{ t('info.creators.title') }}
    </h1>
    <b-row class="flex-column flex-md-row align-items-center" mb-4>
      <b-col cols="12" lg="6" mb-8>
        <b-row>
          <b-col cols="12" md="auto" class="mb-md-0 mr-md-6">
            <b-img lazy src="images/avatar1.jpg" alt="Avatar" rounded="circle" class="mb-md-0 avatar object-fit-cover mb-4" />
          </b-col>
          <b-col>
            <h1 class="display-4 mb-2">
              msan
            </h1>
            <p class="text-muted-foreground">
              {{ t('info.creators.first-description') }}
            </p>
            <p>
              Github: <a class="link" href="http://github.com/Megageorgio">{{ t('singer.siteLink') }}</a>
            </p>
          </b-col>
        </b-row>
      </b-col>
      <b-col cols="12" lg="6" mb-8>
        <b-row>
          <b-col cols="12" md="auto" class="mb-md-0 mr-md-6">
            <b-img lazy src="images/avatar2.jpg" alt="Avatar" rounded="circle" class="mb-md-0 avatar object-fit-cover mb-4" />
          </b-col>
          <b-col>
            <h1 class="display-4 mb-2">
              HHS_kt
            </h1>
            <p class="text-muted-foreground">
              {{ t('info.creators.second-description') }}
            </p>
            <p>
              Youtube: <a class="link" href="https://www.youtube.com/@HHS_kt">{{ t('singer.siteLink') }}</a>
            </p>
          </b-col>
        </b-row>
      </b-col>
    </b-row>

    <b-row class="mb-8">
      <h2 class="display-4 mb-4">
        {{ t('info.rules.title') }}
      </h2>
      <b-list-group>
        <b-list-group-item>
          {{ t('info.rules.no-prohibited-content') }}
        </b-list-group-item>
        <b-list-group-item>
          {{ t('info.rules.no-disinformation') }}
        </b-list-group-item>
        <b-list-group-item>
          {{ t('info.rules.no-repository-spam') }}
        </b-list-group-item>
        <b-list-group-item>
          {{ t('info.rules.no-tags-spam') }}
        </b-list-group-item>
      </b-list-group>
    </b-row>

    <b-row class="mb-4">
      <h2 class="display-4 mb-10">
        {{ t('info.videos.title') }}
      </h2>
      <b-col lg="6" cols-12 mb-8>
        <h4>{{ t('info.videos.first-name') }}</h4>
        <iframe
          class="card-shadow rounded-4"
          width="100%"
          height="315"
          src="https://www.youtube.com/embed/i1hXV4UWMyU"
          allowfullscreen
        />
      </b-col>
      <b-col lg="6" cols-12 mb-8>
        <h4>{{ t('info.videos.second-name') }}</h4>
        <iframe
          class="card-shadow rounded-4"
          width="100%"
          height="315"
          src="https://youtube.com/embed/qnUS5jaFu8c"
          allowfullscreen
        />
      </b-col>
    </b-row>
  </b-container>
</template>

<style scoped>
.carousel-inner img {
  width: 100%;
  height: auto;
}
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
