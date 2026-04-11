import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';
import { SvelteKitPWA } from '@vite-pwa/sveltekit';

export default defineConfig({
	plugins: [
		sveltekit(),
		SvelteKitPWA({
			registerType: 'autoUpdate',
			manifest: {
				name: 'ViSingers',
				short_name: 'ViSingers',
				description: 'Virtual singers catalog',
				theme_color: '#ffffff',
				icons: [
					{
						src: 'pwa-192x192.png',
						sizes: '192x192',
						type: 'image/png'
					}
				]
			}
		})
	],
	server: {
		port: 3000,
		host: '0.0.0.0',
		hmr: process.env.DISABLE_HMR !== 'true'
	}
});
