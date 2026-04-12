<script lang="ts">
  import { Navbar, NavBrand, NavHamburger, NavUl, NavLi, Dropdown, DropdownItem, Button, DarkMode } from 'flowbite-svelte';
  import { t, locale } from 'svelte-i18n';
  import { setLocale } from '$lib/i18n';
  import { Globe, Info, Mic, Users } from 'lucide-svelte';

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

<Navbar rounded color="default" class="sticky top-0 z-50 shadow-sm">
  <NavBrand href="/">
    <div class="h-10 flex items-center justify-center overflow-hidden">
      <img src="/logo.png" alt="Logo" class="h-full object-contain" />
    </div>
  </NavBrand>
  <div class="flex items-center md:order-2 gap-2">
    <DarkMode size="sm" />
    
    <Button color="alternative" class="p-2 !ring-0">
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
