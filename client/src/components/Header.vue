<script setup lang="ts">
import { useColorMode } from 'bootstrap-vue-next'
import { availableLocales, loadLanguageAsync } from '~/modules/i18n'

const { t, locale } = useI18n()
const settingsStore = useSettingsStore()

loadLanguageAsync(settingsStore.language).then(() => {
  locale.value = settingsStore.language
})

const target = document.querySelector('html') as HTMLElement | null

const mode = useColorMode({
  selector: target,
})

mode.value = settingsStore.theme

async function toggleLocales() {
  const locales = availableLocales
  const newLocale = locales[(locales.indexOf(locale.value) + 1) % locales.length]
  await loadLanguageAsync(newLocale)
  locale.value = newLocale
  settingsStore.setLanguage(newLocale)
}

function toggleDark() {
  mode.value = mode.value === 'dark' ? 'light' : 'dark'
  settingsStore.toggleTheme()
}

function isStandalone() {
  return true
  return window.matchMedia('(display-mode: standalone)').matches
}
</script>

<template>
  <b-navbar class="sticky-top" expand="lg" toggleable="md">
    <b-navbar-brand to="/">
      <img
        src="/logo.png"
        alt="Logo"
        width="150px"
        style="min-width: 150px;"
        class="d-inline-block align-top"
      >
    </b-navbar-brand>

    <b-navbar-toggle target="nav-collapse" />

    <b-collapse id="nav-collapse" is-nav>
      <b-navbar-nav>
        <b-nav-item v-if="isStandalone()" to="/voicebanks">
          {{ t('nav.voicebanks') }}
        </b-nav-item>
        <b-nav-item to="/">
          {{ t('nav.singers') }}
        </b-nav-item>
        <b-nav-item to="/groups">
          {{ t('nav.groups') }}
        </b-nav-item>
        <b-nav-item to="/info">
          {{ t('nav.info') }}
        </b-nav-item>
      </b-navbar-nav>
      <b-navbar-nav ml-auto>
        <b-nav-item>
          <button icon-btn :title="t('button.toggle_dark')" @click="toggleDark()">
            <div v-if="mode === 'light'" i="carbon-sun" />
            <div v-else i="carbon-moon" />
          </button>
        </b-nav-item>
        <b-nav-item>
          <a icon-btn :title="t('button.toggle_langs')" @click="toggleLocales()">
            <div i-carbon-language />
          </a>
        </b-nav-item>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar>
</template>
