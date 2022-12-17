import { defineConfig } from 'vite';
import glob from 'glob';

export default defineConfig({
    build: {
        manifest: true,
        outDir: 'wwwroot/scripts',
        assetsDir: '',
        sourcemap: true,
        modulePreload: {
            polyfill: false
        },
        rollupOptions: {
            input: [...glob.sync('Client/**/*.ts'), ...glob.sync('Client/**/*.scss')]
        }
    }
})
