<script lang="ts" module>
  let currentlyPlaying: HTMLAudioElement | null = null;

  export function stopAllOthers(except: HTMLAudioElement | null = null) {
    if (currentlyPlaying && currentlyPlaying !== except) {
      currentlyPlaying.pause();
      currentlyPlaying.currentTime = 0;
    }
    if (except) {
      currentlyPlaying = except;
    }
  }
</script>

<script lang="ts">
  import { Play, Pause } from 'lucide-svelte';
  export let src: string;
  export let title: string;
  export let color: string = '#ff679d';

  let audio: HTMLAudioElement;
  let isPlaying = false;
  let progress = 0;

  function togglePlay() {
    if (isPlaying) {
      audio.pause();
    }
    else {
      stopAllOthers(audio);
      audio.play();
    }
  }

  function onTimeUpdate() {
    if (audio.duration) {
      progress = (audio.currentTime / audio.duration) * 100;
    }
  }

  function seek(e: MouseEvent) {
    const bounds = (e.currentTarget as HTMLElement).getBoundingClientRect();
    const x = e.clientX - bounds.left;
    const percent = x / bounds.width;
    audio.currentTime = percent * audio.duration;
  }
</script>

<div class="flex items-center gap-4 bg-gray-100 dark:bg-gray-800 p-3 rounded-xl shadow-sm w-full">
  <audio 
    bind:this={audio} 
    {src} 
    on:timeupdate={onTimeUpdate} 
    on:play={() => isPlaying = true} 
    on:pause={() => isPlaying = false} 
    on:ended={() => isPlaying = false}
  ></audio>
  
  <button on:click={togglePlay} class="w-10 h-10 shrink-0 flex items-center justify-center rounded-full text-white shadow-md transition-transform hover:scale-105 focus:outline-none" style="background-color: {color}">
    {#if isPlaying}
      <Pause class="w-5 h-5" />
    {:else}
      <Play class="w-5 h-5 ml-1" />
    {/if}
  </button>

  <div class="flex-1 flex flex-col gap-1 min-w-0">
    <span class="text-sm font-bold text-gray-700 dark:text-gray-300 truncate">{title}</span>
    <!-- svelte-ignore a11y-click-events-have-key-events -->
    <!-- svelte-ignore a11y-no-static-element-interactions -->
    <div class="h-2 bg-gray-300 dark:bg-gray-600 rounded-full cursor-pointer overflow-hidden" on:click={seek}>
      <div class="h-full transition-all duration-100" style="width: {progress}%; background-color: #4ea6ea"></div>
    </div>
  </div>
</div>
