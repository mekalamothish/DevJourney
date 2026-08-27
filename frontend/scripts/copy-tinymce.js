const fs = require('fs');
const path = require('path');

const src = path.resolve(__dirname, '../node_modules/tinymce');
const dest = path.resolve(__dirname, '../public/tinymce');

function copyRecursive(srcPath, destPath) {
  if (!fs.existsSync(srcPath)) {
    console.warn(`Source not found: ${srcPath}`);
    return;
  }
  fs.rmSync(destPath, { recursive: true, force: true });
  fs.cpSync(srcPath, destPath, { recursive: true });
  console.log(`Copied TinyMCE assets from ${srcPath} to ${destPath}`);
}

try {
  copyRecursive(src, dest);
} catch (err) {
  console.error('Failed to copy TinyMCE assets:', err);
  process.exit(1);
}
