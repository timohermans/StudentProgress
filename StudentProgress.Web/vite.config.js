import { defineConfig } from 'vite';
import glob from 'glob';
import mkcert from 'vite-plugin-mkcert'

export default defineConfig({
    server: {
        https: true,
        cors: true,
    },
    plugins: [mkcert()],
    build: {
        manifest: true,
        outDir: 'wwwroot/scripts',
        assetsDir: '',
        sourcemap: true,
        modulePreload: {
            polyfill: false
        },
        rollupOptions: {
            input: [...glob.sync('Client/**/*.ts'), ...glob.sync('Client/**/!(_)*.scss')]
        }
    }
})
