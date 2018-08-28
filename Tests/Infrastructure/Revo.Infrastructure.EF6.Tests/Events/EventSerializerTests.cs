﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Revo.Core.Events;
using Revo.Core.Types;
using Revo.Infrastructure.EF6.Events;
using Xunit;

namespace Revo.Infrastructure.EF6.Tests.Events
{
    public class EventSerializerTests
    {
        private EventSerializer sut;
        private IVersionedTypeRegistry versionedTypeRegistry;

        public EventSerializerTests()
        {
            versionedTypeRegistry = Substitute.For<IVersionedTypeRegistry>();
            versionedTypeRegistry.GetTypeInfo<IEvent>(typeof(Event1))
                .Returns(new VersionedType(new VersionedTypeId("Event1", 1), typeof(Event1)));
            versionedTypeRegistry.GetTypeInfo<IEvent>(new VersionedTypeId("Event1", 1))
                .Returns(new VersionedType(new VersionedTypeId("Event1", 1), typeof(Event1)));

            versionedTypeRegistry.GetTypeInfo<IEvent>(typeof(Event2))
                .Returns(new VersionedType(new VersionedTypeId("Event2", 1), typeof(Event2)));
            versionedTypeRegistry.GetTypeInfo<IEvent>(new VersionedTypeId("Event2", 1))
                .Returns(new VersionedType(new VersionedTypeId("Event2", 1), typeof(Event2)));

            sut = new EventSerializer(versionedTypeRegistry);
        }

        [Fact]
        public void DeserializeEvent()
        {
            var result = sut.DeserializeEvent("{\"foo\":123}", new VersionedTypeId("Event1", 1));

            result.Should().BeOfType<Event1>();
            ((Event1)result).Foo.Should().Be(123);
        }

        [Fact]
        public void DeserializeEventMetadata()
        {
            var result = sut.DeserializeEventMetadata("{\"Bar\":\"123\"}");
            result.Should().HaveCount(1);
            result.Should().Contain(new KeyValuePair<string, string>("Bar", "123"));
        }

        [Fact]
        public void SerializeEvent()
        {
            var result = sut.SerializeEvent(new Event1(123));

            JObject json = JObject.Parse(result.EventJson);
            json.Should().HaveCount(1);
            json.Should().Contain("foo", 123);
            result.TypeId.Should().Be(new VersionedTypeId("Event1", 1));
        }

        [Fact]
        public void SerializeEventMetadata()
        {
            var result = sut.SerializeEventMetadata(new Dictionary<string, string>() { { "Bar", "123" } });
            JObject json = JObject.Parse(result);
            json.Should().HaveCount(1);
            json.Should().Contain("Bar", "123");
        }

        [Fact]
        public void SerializeEvent_DictionariesOriginalCase()
        {
            var result = sut.SerializeEvent(new Event2(
                new Dictionary<string, string>()
                {
                    {"Key", "Value"},
                    {"key", "value"}
                }.ToImmutableDictionary()));

            JObject json = JObject.Parse(result.EventJson);
            json.Should().ContainKey("pairs");
            json["pairs"].Children<JProperty>().Should().Contain(x => x.Name == "Key" && x.Value.ToString() == "Value");
            json["pairs"].Children<JProperty>().Should().Contain(x => x.Name == "key" && x.Value.ToString() == "value");
        }
        
        public class Event1 : IEvent
        {
            public Event1(int foo)
            {
                Foo = foo;
            }

            public int Foo { get; }
        }

        public class Event2 : IEvent
        {
            public Event2(ImmutableDictionary<string, string> pairs)
            {
                Pairs = pairs;
            }

            public ImmutableDictionary<string, string> Pairs { get; }
        }
    }
}
