using System;

namespace DDD.Domain
{
  class TestDomainCreatedEvent : Event
  {
    public TestDomainCreatedEvent(Guid id)
    {
      Id = id;
    }

    public Guid Id { get; }
  }
}
