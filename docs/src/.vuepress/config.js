module.exports = {
    lang: 'en-US',
    title: 'DbgCensus Documentation',
    description: "Unofficial C# wrappers for Daybreak Game Company's Census API.",
    bundler: '@vuepress/bundler-vite',

    themeConfig: {
      logo: '/images/logo.png',
      repo: 'carlst99/DbgCensus',
      docsDir: 'docs',

      navbar: [
        {
          text: 'Core Info',
          link: '/core-info'
        },
        {
          text: 'Making Queries',
          link: '/rest/'
        }
      ]
    },
  }