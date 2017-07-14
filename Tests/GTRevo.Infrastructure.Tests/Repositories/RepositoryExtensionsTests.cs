﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTRevo.Infrastructure.Core.Domain.Basic;
using GTRevo.Infrastructure.Repositories;
using GTRevo.Testing.Infrastructure.Repositories;
using Xunit;

namespace GTRevo.Infrastructure.Tests.Repositories
{
    public class RepositoryExtensionsTests
    {
        private readonly FakeRepository repository;

        public RepositoryExtensionsTests()
        {
            repository = new FakeRepository();
        }

        [Fact]
        public void AddIfNew_AddsSingleById()
        {
            TestEntity entity = new TestEntity(Guid.NewGuid());
            TestEntity result = RepositoryExtensions.AddIfNew(repository, entity);
            repository.SaveChanges();

            Assert.Equal(entity, result);
            Assert.Equal(1, repository.FindAll<TestEntity>().Count());
            Assert.Contains(repository.FindAll<TestEntity>(),
                x => x == entity);
        }

        [Fact]
        public void AddIfNew_DoesntAddSingleDuplicateById()
        {
            TestEntity entity1 = new TestEntity(Guid.NewGuid());
            repository.Add(entity1);
            repository.SaveChanges();

            TestEntity entity2 = new TestEntity(entity1.Id);
            TestEntity result = RepositoryExtensions.AddIfNew(repository, entity2);
            repository.SaveChanges();

            Assert.Equal(entity1, result);
            Assert.Equal(1, repository.FindAll<TestEntity>().Count());
            Assert.Contains(repository.FindAll<TestEntity>(),
                x => x == entity1);
        }
        
        [Fact]
        public void AddIfNew_DoesntAddSingleDuplicateByProperty()
        {
            TestEntity entity1 = new TestEntity(Guid.NewGuid()) { Value = "Foo" };
            repository.Add(entity1);
            repository.SaveChanges();

            TestEntity entity2 = new TestEntity(Guid.NewGuid()) { Value = "Foo" };
            TestEntity result = RepositoryExtensions.AddIfNew(repository, x => x.Value, entity2);
            repository.SaveChanges();

            Assert.Equal(entity1, result);
            Assert.Equal(1, repository.FindAll<TestEntity>().Count());
            Assert.Contains(repository.FindAll<TestEntity>(),
                x => x == entity1);
        }

        [Fact]
        public void AddIfNew_AddsSecondByProperty()
        {
            TestEntity entity1 = new TestEntity(Guid.NewGuid()) { Value = "Foo" };
            repository.Add(entity1);
            repository.SaveChanges();

            TestEntity entity2 = new TestEntity(Guid.NewGuid()) { Value = "Bar" };
            TestEntity result = RepositoryExtensions.AddIfNew(repository, x => x.Value, entity2);
            repository.SaveChanges();

            Assert.Equal(entity2, result);
            Assert.Equal(2, repository.FindAll<TestEntity>().Count());
            Assert.Contains(repository.FindAll<TestEntity>(),
                x => x == entity1);
            Assert.Contains(repository.FindAll<TestEntity>(),
                x => x == entity2);
        }

        public class TestEntity : BasicAggregateRoot
        {
            public TestEntity(Guid id) : base(id)
            {
            }

            protected TestEntity()
            {
            }

            public string Value { get; set; }
        }
    }
}
