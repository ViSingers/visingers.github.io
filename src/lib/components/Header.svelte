<script lang="ts">
  import { Navbar, NavBrand, NavHamburger, NavUl, NavLi, Dropdown, DropdownItem, Button, DarkMode } from 'flowbite-svelte';
  import { t, locale } from 'svelte-i18n';
  import { setLocale } from '$lib/i18n';
  import { Globe, Info, Mic, Users } from 'lucide-svelte';
  import { SiDiscord } from "@icons-pack/svelte-simple-icons";

  let languages = [
    { code: 'en', name: 'English' },
    { code: 'ru', name: 'Русский' },
    { code: 'ja', name: '日本語' },
    { code: 'zh', name: '中国人' }
  ];

  function changeLanguage(code: string) {
    setLocale(code);
  }
</script>

<Navbar color="default" class="sticky top-0 pb-1 z-50 bg-white/50 dark:bg-gray-800/50 backdrop-blur-xl border-b border-black/20 shadow-md">
  <NavBrand href="/">
    <div class="h-10 flex items-center justify-center overflow-hidden">
      <img src="/logo.png" alt="Logo" class="h-full object-contain" />
    </div>
  </NavBrand>
  <div class="flex items-center md:order-2 gap-1">
    <a 
    href="https://discord.gg/y5YsY9UfBG" 
    target="_blank"
    rel="noopener noreferrer"
    class="p-2 text-gray-600 dark:text-gray-400 hover:text-[#5865F2] dark:hover:text-[#5865F2] 
           transition-colors duration-200 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700"
  >
    <SiDiscord class="h-5 w-5" />
  </a>
    <DarkMode size="sm" />
    <Button color="alternative" class="p-2 !ring-0 bg-transparent">
      <Globe class="h-5 w-5" />
    </Button>
    <Dropdown placement="bottom" class="z-50">
      {#each languages as lang}
        <DropdownItem on:click={() => changeLanguage(lang.code)} class={$locale === lang.code ? 'text-[#ff679d] font-bold' : ''}>
          {lang.name}
        </DropdownItem>
      {/each}
    </Dropdown>
    <NavHamburger />
  </div>
  <NavUl>
    <NavLi href="/">
      <div class="flex items-center gap-2">
        <Mic class="h-5 w-5" />
        <span>{$t('nav.singers')}</span>
      </div>
    </NavLi>
    <NavLi href="/groups">
      <div class="flex items-center gap-2">
        <Users class="h-5 w-5" />
        <span>{$t('nav.groups')}</span>
      </div>
    </NavLi>
    <NavLi href="/info">
      <div class="flex items-center gap-2">
        <Info class="h-5 w-5" />
        <span>{$t('nav.info')}</span>
      </div>
    </NavLi>
    <!-- NavLi href="/voicebanks"}>{$t('nav.voicebanks')}</!-->
  </NavUl>
</Navbar>
