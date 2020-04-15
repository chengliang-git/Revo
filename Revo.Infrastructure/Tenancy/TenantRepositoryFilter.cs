﻿using System;
using System.Linq;
using Revo.DataAccess.Entities;
using Revo.Domain.Tenancy;

namespace Revo.Infrastructure.Tenancy
{
    public class TenantRepositoryFilter : IRepositoryFilter
    {
        private readonly ITenantContext tenantContext;

        public TenantRepositoryFilter(ITenantContext tenantContext)
        {
            this.tenantContext = tenantContext;
        }
        
        public bool NullTenantCanAccessOtherTenantsData { get; set; } = false;

        public IQueryable<T> FilterResults<T>(IQueryable<T> results) where T : class
        {
            if (typeof(ITenantOwned).IsAssignableFrom(typeof(T)))
            {
                return DoFilterResults((dynamic)results); // TODO optimize by constructing filter Expression by hand?
            }

            return results;
        }

        public T FilterResult<T>(T result) where T : class
        {
            ITenantOwned tenantOwned = result as ITenantOwned;
            if (tenantOwned != null)
            {
                Guid? tenantId = tenantContext.Tenant?.Id;
                if (tenantOwned.TenantId != null
                    && (!NullTenantCanAccessOtherTenantsData || tenantId != null)
                    && tenantOwned.TenantId != tenantId)
                {
                    return null;
                }
            }

            return result;
        }

        public void FilterAdded<T>(T added) where T : class
        {
            ITenantOwned tenantOwned = added as ITenantOwned;
            if (tenantOwned != null)
            {
                Guid? tenantId = tenantContext.Tenant?.Id;
                if (tenantOwned.TenantId != null
                    && (!NullTenantCanAccessOtherTenantsData || tenantId != null)
                    && tenantOwned.TenantId != tenantId)
                {
                    throw new InvalidOperationException(
                        $"Forbidden to add {typeof(T).FullName} entity for tenant ID {tenantOwned.TenantId.Value} from context of tenant ID {tenantId.Value}");
                }
            }
        }

        public void FilterDeleted<T>(T deleted) where T : class
        {
            ITenantOwned tenantOwned = deleted as ITenantOwned;
            if (tenantOwned != null)
            {
                Guid? tenantId = tenantContext.Tenant?.Id;
                if (tenantOwned.TenantId != null
                    && (!NullTenantCanAccessOtherTenantsData || tenantId != null)
                    && tenantOwned.TenantId != tenantId)
                {
                    throw new InvalidOperationException(
                        $"Forbidden to delete {typeof(T).FullName} entity for tenant ID {tenantOwned.TenantId.Value} from context of tenant ID {tenantId.Value}");
                }
            }
        }

        public void FilterModified<T>(T modified) where T : class
        {
            ITenantOwned tenantOwned = modified as ITenantOwned;
            if (tenantOwned != null)
            {
                Guid? tenantId = tenantContext.Tenant?.Id;
                if (tenantOwned.TenantId != null
                    && (!NullTenantCanAccessOtherTenantsData || tenantId != null)
                    && tenantOwned.TenantId != tenantId)
                {
                    throw new InvalidOperationException(
                        $"Forbidden to modify {typeof(T).FullName} entity for tenant ID {tenantOwned.TenantId.Value} from context of tenant ID {tenantId.Value}");
                }
            }
        }

        private IQueryable<T> DoFilterResults<T>(IQueryable<T> results) where T : class, ITenantOwned
        {
            Guid? tenantId = tenantContext.Tenant?.Id;
            if (tenantId == null && NullTenantCanAccessOtherTenantsData)
            {
                return results;
            }

            return results.Where(x => x.TenantId == null || x.TenantId == tenantId);
        }
    }
}
