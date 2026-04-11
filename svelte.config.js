import adapter from '@sveltejs/adapter-auto';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	preprocess: vitePreprocess(),

	kit: {
		adapter: adapter({
			fallback: 'index.html'
		}),
		alias: {
			$lib: './src/lib',
			$components: './src/lib/components',
			$locales: './src/lib/locales'
		},
		prerender: {
      		handleUnseenRoutes: 'ignore'
    	}
	}
};

export default config;
