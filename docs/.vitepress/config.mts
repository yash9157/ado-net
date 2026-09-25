import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'Employee Management Guide',
  description: 'End-to-end ASP.NET Core ADO.NET, SQL Server procedures, JWT, and Angular guide',
  head: [['link', { rel: 'icon', href: '/favicon.svg', type: 'image/svg+xml' }]],
  cleanUrls: false,
  themeConfig: {
    nav: [
      { text: 'Start here', link: '/' },
      { text: 'API', link: '/api-setup.html' },
      { text: 'Angular', link: '/angular-setup.html' },
      { text: 'Run and test', link: '/run-and-test.html' }
    ],
    sidebar: [
      {
        text: 'End-to-end guide',
        items: [
          { text: 'Overview', link: '/' },
          { text: '1. API setup', link: '/api-setup.html' },
          { text: '2. Database', link: '/database.html' },
          { text: '3. Auth and JWT', link: '/api-auth.html' },
          { text: '4. Employee API', link: '/api-employees.html' },
          { text: '5. Angular setup', link: '/angular-setup.html' },
          { text: '6. Angular flow', link: '/angular-flow.html' },
          { text: '7. Run and test', link: '/run-and-test.html' }
        ]
      }
    ],
    search: {
      provider: 'local'
    }
  }
})
