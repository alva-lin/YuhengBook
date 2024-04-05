/** @type {import('prettier').Config} */
module.exports = {
  printWidth: 100,
  singleQuote: true,
  trailingComma: 'es5',
  semi: true,

  importOrder: ['^@/(.*)$', '^./(.*)$'],
  importOrderSeparation: true,
  importOrderSortSpecifiers: true,

  plugins: ["@trivago/prettier-plugin-sort-imports"],
};
