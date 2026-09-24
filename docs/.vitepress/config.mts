import { defineConfig } from 'vitepress'

export default defineConfig({
  title: '.NET Data Access',
  description: 'Step-by-step ADO.NET, stored procedures, EF Core Identity, and JWT API example',
  head: [['link', { rel: 'icon', href: '/favicon.svg', type: 'image/svg+xml' }]],
  cleanUrls: false,
  themeConfig: {
    nav: [
      { text: 'Start here', link: '/' },
      { text: 'Build the API', link: '/setup.html' },
      { text: 'Try the API', link: '/web-api-example.html#try-the-api' }
    ],
    sidebar: [
      {
        text: 'Step-by-step guide',
        items: [
          { text: 'Overview', link: '/' },
          { text: '1. Create the API', link: '/setup.html' },
          { text: '2. EF Core and Identity', link: '/ef-core-identity.html' },
          { text: '3. Login and JWT', link: '/authentication.html' },
          { text: '4. Stored procedures', link: '/stored-procedures.html' },
          { text: '5. ADO.NET calls', link: '/ado-net-methods.html' },
          { text: '6. Run and test', link: '/web-api-example.html' }
        ]
      }
    ],
    search: {
      provider: 'local'
    }
  }
})
