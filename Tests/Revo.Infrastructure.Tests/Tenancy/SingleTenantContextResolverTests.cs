﻿using FluentAssertions;
using Revo.Domain.Tenancy;
using Revo.Infrastructure.Tenancy;
using NSubstitute;
using Xunit;

namespace Revo.Infrastructure.Tests.Tenancy
{
    public class SingleTenantContextResolverTests
    {
        private SingleTenantContextResolver sut;
        private ITenant tenant;
        
        [Fact]
        public void Tenant_ReturnsCorrectTenant()
        {
            tenant = Substitute.For<ITenant>();
            sut = new SingleTenantContextResolver(tenant);
            sut.Tenant.Should().Be(tenant);
        }
    }
}
