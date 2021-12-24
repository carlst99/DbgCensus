module.exports = {
    lang: 'en-US',
    title: 'DbgCensus Documentation',
    description: 'Documentation for the DbgCensus libraries',
    bundler: '@vuepress/bundler-vite',

    themeConfig: {
      logo: '/images/logo.png',
      repo: 'carlst99/DbgCensus',
      docsDir: 'docs',

      navbar: [
        {
          text: 'Making Queries',
          link: '/rest/'
        }
      ]
    },
  }