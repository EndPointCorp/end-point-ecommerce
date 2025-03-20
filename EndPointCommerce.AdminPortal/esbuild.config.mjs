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
  { entry: './JavaScript/addresses.index.js', outfile: './wwwroot/js/addresses.index.js' },
  { entry: './JavaScript/categories.index.js', outfile: './wwwroot/js/categories.index.js' },
  { entry: './JavaScript/coupons.index.js', outfile: './wwwroot/js/coupons.index.js' },
  { entry: './JavaScript/customers.index.js', outfile: './wwwroot/js/customers.index.js' },
  { entry: './JavaScript/dashboard.js', outfile: './wwwroot/js/dashboard.js' },
  { entry: './JavaScript/orders.form.js', outfile: './wwwroot/js/orders.form.js' },
  { entry: './JavaScript/orders.index.js', outfile: './wwwroot/js/orders.index.js' },
  { entry: './JavaScript/products.additional-images.js', outfile: './wwwroot/js/products.additional-images.js' },
  { entry: './JavaScript/products.form.js', outfile: './wwwroot/js/products.form.js' },
  { entry: './JavaScript/products.index.js', outfile: './wwwroot/js/products.index.js' },
  { entry: './JavaScript/quotes.form.js', outfile: './wwwroot/js/quotes.form.js' },
  { entry: './JavaScript/quotes.index.js', outfile: './wwwroot/js/quotes.index.js' },
  { entry: './JavaScript/site.js', outfile: './wwwroot/js/site.js' },
  { entry: './JavaScript/users.form.js', outfile: './wwwroot/js/users.form.js' },
  { entry: './JavaScript/users.index.js', outfile: './wwwroot/js/users.index.js' },
];

jsFiles.forEach(async file => {
  await esbuild.build({
    entryPoints: [file.entry],
    bundle: true,
    sourcemap: true,
    outfile: file.outfile,
  })
});