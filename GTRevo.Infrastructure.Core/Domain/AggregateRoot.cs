﻿using System;
using System.Collections.Generic;
using System.Linq;
using GTRevo.Infrastructure.Core.Domain.Events;

namespace GTRevo.Infrastructure.Core.Domain
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        public AggregateRoot(Guid id) : this()
        {
            Id = id;
            Version = 0;
        }

        /// <summary>
        /// Only to be used by by ORMs like EF and the second chained constructor.
        /// </summary>
        protected AggregateRoot()
        {
            EventRouter = new AggregateEventRouter(this);
        }

        public bool IsDeleted { get; private set; }
        public virtual Guid Id { get; private set; }
        public virtual int Version { get; protected set; }

        public virtual bool IsChanged => UncommitedEvents.Any();

        public virtual IEnumerable<DomainAggregateEvent> UncommitedEvents
        {
            get
            {
                return EventRouter.UncommitedEvents;
            }
        }

        protected internal IAggregateEventRouter EventRouter { get; }

        public virtual void Commit()
        {
            EventRouter.CommitEvents();
        }

        protected virtual void ApplyEvent<T>(T evt) where T : DomainAggregateEvent
        {
            if (IsDeleted)
            {
                throw new InvalidOperationException(
                    $"Cannot apply new {typeof(T).FullName} event on {GetType().FullName} aggregate because it is currently in deleted state");
            }

            EventRouter.ApplyEvent(evt);
        }

        protected void MarkDeleted()
        {
            IsDeleted = true;
        }
    }
}
