<script lang="ts">
  import { page } from '$app/stores';
  import { onMount, tick } from 'svelte';
  import { dataService } from '$lib/dataService';
  import { t, locale } from 'svelte-i18n';
  import { Button, Card, Spinner } from 'flowbite-svelte';
  import { ExternalLink, Github, Download, Music, Video, Image as ImageIcon, Star, Info, FileText } from 'lucide-svelte';
  import AudioPlayer from '$lib/components/AudioPlayer.svelte';
  
  import lightGallery from 'lightgallery';
  import lgZoom from 'lightgallery/plugins/zoom';
  import lgRotate from 'lightgallery/plugins/rotate';
  import lgFullscreen from 'lightgallery/plugins/fullscreen';
  import lgThumbnail from 'lightgallery/plugins/thumbnail';
  import tilt from '@savy011/tilt-svelte';
  import { reveal } from 'svelte-reveal';

  import 'lightgallery/css/lightgallery.css';
  import 'lightgallery/css/lg-zoom.css';
  import 'lightgallery/css/lg-rotate.css';
  import 'lightgallery/css/lg-fullscreen.css';
  import 'lightgallery/css/lg-thumbnail.css';

  let singer: any = null;
  let loading = true;
  let galleryContainer: HTMLElement;

  onMount(async () => {
    const { user, repo } = $page.params;
    let data = await dataService.getData();
    if (!data) data = await dataService.checkUpdate();
    singer = data?.singers.find((s: any) => s.creatorLogin === user && s.repositoryName === repo);
    loading = false;

    if (singer?.imageUrls?.length) {
      await tick(); 
      lightGallery(galleryContainer, {
        plugins: [lgZoom, lgRotate, lgFullscreen, lgThumbnail],
        speed: 500,
        licenseKey: '0000-0000-000-0000',
        download: true,
        selector: '.gallery-item',
      });
    }
  });

  $: details = singer?.details[$locale?.split('-')[0]] || singer?.details['en'] || {};
  $: hasLongText = ((details?.generalInfo || []).join('').length > 640) || ((details?.termsOfUse || []).join('').length > 640);
  
  function getAvatarUrl() {
    return `https://raw.githubusercontent.com/${singer.creatorLogin}/${singer.repositoryName}/${singer.avatarUrl}`;
  }

  function getRawUrl(path: string) {
    return `https://raw.githubusercontent.com/${singer.creatorLogin}/${singer.repositoryName}/${path}`;
  }

  function getDownloadUrl(url: string) {
    if (url.startsWith('http')) return url;
    return `https://github.com/${singer.creatorLogin}/${singer.repositoryName}/releases/download/${url}`;
  }

  function formatImageTitle(path: string) {
    const filename = path.split('/').pop() || '';
    return filename.split('.')[0].replaceAll('_', ' ');
  }
</script>

{#if loading}
  <div class="flex flex-col justify-center items-center h-64 space-y-4">
    <Spinner size="12" color="gray" />
    <p class="text-gray-500">Loading singer details...</p>
  </div>
{:else if !singer}
  <div class="text-center py-20 space-y-6">
    <div class="text-6xl">😕</div>
    <h2 class="text-3xl font-bold text-gray-900 dark:text-white">Singer not found</h2>
    <p class="text-gray-500">The singer you are looking for doesn't exist in our database.</p>
    <Button href="/" pill color="dark">Back to List</Button>
  </div>
{:else}
  <div class="max-w-5xl mx-auto space-y-16 pb-20">
    <section class="flex flex-col md:flex-row gap-12 items-center md:items-start text-center md:text-left pt-10">
      <div class="relative group shrink-0">
        <img src={getAvatarUrl()} alt={singer.name} class="w-64 h-64 rounded-[2.5rem] object-cover shadow-2xl transition-transform duration-500 group-hover:scale-105 ring-8 ring-gray-50 dark:ring-gray-800" />
        <div class="absolute -bottom-4 -right-4 bg-white dark:bg-gray-800 p-3 rounded-2xl shadow-xl flex items-center gap-2">
           <Star class="w-5 h-5 text-yellow-400 fill-yellow-400" />
           <span class="font-bold text-lg">{singer.stars}</span>
        </div>
      </div>
      
      <div class="flex-1 space-y-6">
        <div class="space-y-3">
          <h1 class="text-5xl md:text-6xl font-black tracking-tighter text-gray-900 dark:text-white">{singer.name}</h1>
          <div class="flex items-center justify-center md:justify-start gap-3">
            <span class="text-gray-400 font-medium">{$t('singer.author')}</span>
            <a href="/users/{singer.creatorLogin}" class="text-xl font-bold text-gray-600 hover:text-gray-900 dark:text-gray-300 dark:hover:text-white transition-colors">{singer.creatorName}</a>
          </div>
        </div>

        <div class="flex flex-wrap justify-center md:justify-start gap-1.5">
          {#each singer.tags as tag}
            <span class="px-3 py-1 text-xs font-bold rounded-full text-white bg-gray-500">{tag.name}</span>
          {/each}
        </div>

        <p class="text-gray-600 dark:text-gray-400 leading-relaxed whitespace-pre-line max-w-3xl">
          {details.description || ''}
        </p>

        <div class="flex flex-wrap justify-center md:justify-start gap-4 pt-2">
          {#if singer.siteUrl}
            <Button href={singer.siteUrl} target="_blank" color="alternative" pill class="gap-2 px-6 shadow-sm">
              <ExternalLink class="w-4 h-4" /> {$t('singer.site')}
            </Button>
          {/if}
          <Button href="https://github.com/{singer.creatorLogin}/{singer.repositoryName}" target="_blank" color="dark" pill class="gap-2 px-6 shadow-sm">
            <Github class="w-4 h-4" /> {$t('singer.repository')}
          </Button>
        </div>
      </div>
    </section>

    <div class="grid grid-cols-1 {hasLongText ? '' : 'md:grid-cols-2'} gap-8">
      {#if details.generalInfo && details.generalInfo.length > 0}
        <section class="bg-gray-50 dark:bg-gray-800/50 p-8 rounded-[2.5rem] space-y-6" {@attach tilt({ max: 4, reverse: true })}>
          <div class="flex items-center gap-3 border-b border-gray-200 dark:border-gray-700 pb-4">
            <Info class="w-6 h-6 text-gray-500" />
            <h3 class="text-xl font-bold">{$t('singer.information') || 'Information'}</h3>
          </div>
          <div class="space-y-4">
            {#each details.generalInfo as info}
              {@const parts = info.split(':')}
              <div class="flex flex-col gap-1">
                <span class="text-xs font-bold uppercase tracking-widest text-gray-500">{parts[0]}</span>
                <span class="text-gray-900 dark:text-white font-medium">{parts.slice(1).join(':').trim()}</span>
              </div>
            {/each}
          </div>
        </section>
      {/if}

      {#if details.termsOfUse && details.termsOfUse.length > 0}
        <section class="bg-gray-50 dark:bg-gray-800/50 p-8 rounded-[2.5rem] space-y-6" {@attach tilt({ max: 4, reverse: true })}>
          <div class="flex items-center gap-3 border-b border-gray-200 dark:border-gray-700 pb-4">
            <FileText class="w-6 h-6 text-gray-500" />
            <h3 class="text-xl font-bold">{$t('singer.terms-of-use') || 'Terms of Use'}</h3>
          </div>
          <div class="space-y-4">
            {#each details.termsOfUse as info}
              {@const parts = info.split(':')}
              <div class="flex flex-col gap-1">
                <span class="text-xs font-bold uppercase tracking-widest text-gray-500">{parts[0]}</span>
                <span class="text-gray-900 dark:text-white font-medium">{parts.slice(1).join(':').trim()}</span>
              </div>
            {/each}
          </div>
        </section>
      {/if}
    </div>

    <section class="space-y-8">
      <div class="flex items-center gap-4" use:reveal>
        <div class="p-3 bg-gray-100 dark:bg-gray-800 rounded-2xl">
          <Music class="w-6 h-6 text-gray-600 dark:text-gray-300" />
        </div>
        <h2 class="text-3xl font-bold">{$t('singer.voicebanks') || 'Voicebanks'}</h2>
      </div>
      
      <div class="space-y-6">
        {#if singer.voicebanks && singer.voicebanks.length > 0}
          {#each singer.voicebanks as vb}
            <div use:reveal>
            <Card class="max-w-none border-none bg-gray-50 dark:bg-gray-800/50 hover:bg-gray-100 dark:hover:bg-gray-800/70 transition-colors shadow-sm p-6 rounded-3xl">
              <div class="flex flex-col space-y-5 w-full">
                
                <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
                  <div class="flex flex-wrap items-center gap-2">
                    <h3 class="text-2xl font-extrabold text-gray-900 dark:text-white mr-2">{vb.name}</h3>
                    <span class="px-3 py-1 text-xs lowercase font-bold tracking-wide rounded-full text-white" style="background-color: #4ea6ea;">{vb.type}</span>
                    {#each vb.languages as lang}
                      <span class="px-3 py-1 text-xs lowercase font-bold tracking-wide rounded-full text-white" style="background-color: #ff679d;">{lang}</span>
                    {/each}
                  </div>
                  
                  <Button href={getDownloadUrl(vb.url)} target="_blank" color="dark" pill class="gap-2 px-8 py-3 text-lg font-bold shadow-lg shrink-0 w-full md:w-auto">
                    <Download class="w-5 h-5" /> {$t('singer.download') || 'Download'}
                  </Button>
                </div>

                <p class="text-gray-600 dark:text-gray-400 text-lg leading-relaxed w-full">
                  {vb.description[$locale?.split('-')[0]] || vb.description['en'] || 'No description provided.'}
                </p>
                
                {#if vb.sampleUrls && vb.sampleUrls.length > 0}
                  <div class="space-y-3 pt-2 w-full">
                    <h4 class="text-sm font-bold text-gray-500 uppercase tracking-wider">{$t('singer.samples')}</h4>
                    <div class="grid grid-cols-1 sm:grid-cols-2  gap-4">
                      {#each vb.sampleUrls as sample}
                        <AudioPlayer src={getDownloadUrl(sample)} title={sample.split('/').pop()?.split('.')[0]?.replaceAll('_', ' ') || 'Sample'} color="#ff679d" />
                      {/each}
                    </div>
                  </div>
                {/if}

              </div>
            </Card>
            </div>
          {/each}
        {:else}
          <p class="text-gray-500 italic">No voicebanks listed for this singer.</p>
        {/if}
      </div>
    </section>

    {#if singer.imageUrls && singer.imageUrls.length > 0}
      <section class="space-y-8">
        <div class="flex items-center gap-4" use:reveal>
          <div class="p-3 bg-gray-100 dark:bg-gray-800 rounded-2xl">
            <ImageIcon class="w-6 h-6 text-gray-600 dark:text-gray-300" />
          </div>
          <h2 class="text-3xl font-bold">{$t('singer.gallery')}</h2>
        </div>
        
        <div 
          bind:this={galleryContainer}
          class="columns-1 sm:columns-2 md:columns-3 lg:columns-4 gap-4 space-y-4"
        >
          {#each singer.imageUrls as img}
            <!-- svelte-ignore a11y-missing-attribute -->
            <a 
              class="gallery-item block cursor-pointer"
              data-src={getRawUrl(img)}
              data-sub-html="<div class='lg-sub-title'>{formatImageTitle(img)}</div>"
            >
            <div {@attach tilt({reverse: true})} use:reveal>
              <img 
                src={getRawUrl(img)} 
                alt={formatImageTitle(img)} 
                class="w-full rounded-2xl transition-transform duration-300 break-inside-avoid"
                loading="lazy"
                onerror={(e) => {
                  if (e.currentTarget.parentElement) {
                    e.currentTarget.parentElement.style.display = 'none';
                  }
                }}
              />
              </div>
            </a>
          {/each}
        </div>
      </section>
    {/if}

    {#if singer.videoUrls && singer.videoUrls.length > 0}
      <section class="space-y-8">
        <div class="flex items-center gap-4" use:reveal>
          <div class="p-3 bg-gray-100 dark:bg-gray-800 rounded-2xl">
            <Video class="w-6 h-6 text-gray-600 dark:text-gray-300" />
          </div>
          <h2 class="text-3xl font-bold">{$t('singer.videos') || 'Videos'}</h2>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          {#each singer.videoUrls as videoId}
            <div class="aspect-video rounded-[2rem] overflow-hidden shadow-2xl ring-1 ring-black/5" use:reveal>
              <iframe
                width="100%"
                height="100%"
                src="https://www.youtube.com/embed/{videoId}"
                title="YouTube video player"
                frameborder="0"
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                allowfullscreen
              ></iframe>
            </div>
          {/each}
        </div>
      </section>
    {/if}
  </div>
{/if}

<style>
  :global(.lg-sub-title) {
    color: #fff;
    font-size: 1.5rem;
    font-weight: bold;
    text-align: center;
    padding: 10px;
    background: rgba(0,0,0,0.5);
    border-radius: 12px;
    backdrop-filter: blur(4px);
  }
  :global(.lg-backdrop) {
    background-color: rgba(0, 0, 0, 0.9) !important;
  }
</style>
