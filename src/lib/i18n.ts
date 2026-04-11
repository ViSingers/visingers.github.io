import { register, init, getLocaleFromNavigator, locale } from 'svelte-i18n';

export async function initI18n() {
  register('en', () => import('./locales/en.json'));
  register('ru', () => import('./locales/ru.json'));
  register('zh', () => import('./locales/zh.json'));
  register('ja', () => import('./locales/ja.json'));

  const savedLocale = localStorage.getItem('locale');

  await init({
    fallbackLocale: 'en',
    initialLocale: savedLocale || getLocaleFromNavigator(),
  });
}

export function setLocale(newLocale: string) {
  localStorage.setItem('locale', newLocale);
  locale.set(newLocale);
}
