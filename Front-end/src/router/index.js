{
  path: '/ObrasPublicas/list',
  name: 'ObrasPublicas',
  component: () => import('@/views/ServiciosPublicos/publickworks/publicWorks/PublicWorksList.vue'),
  meta: {
    requiresAuth: true
  }
},
{
  path: '/ObrasPublicas/:ObraId',
  name: 'ObrasPublicas-Edit',
  component: () => import('@/views/ServiciosPublicos/publickworks/publicWorks/EditPublicWorks.vue'),
  meta: {
    requiresAuth: true
  }
},
