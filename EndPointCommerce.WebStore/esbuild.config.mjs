import * as esbuild from 'esbuild';
import { sassPlugin } from 'esbuild-sass-plugin';

// CSS
await esbuild.build({
  entryPoints: ['./Stylesheets/site.scss'],
  bundle: true,
  sourcemap: true,
  loader: { '.woff': 'file', '.woff2': 'file' },
  outfile: './wwwroot/css/site.css',
  plugins: [sassPlugin()]
});

// JavaScript
const jsFiles = [
  { entry: './JavaScript/site.js', outfile: './wwwroot/js/site.js' }
];

jsFiles.forEach(async file => {
  await esbuild.build({
    entryPoints: [file.entry],
    bundle: true,
    sourcemap: true,
    outfile: file.outfile,
  })
});