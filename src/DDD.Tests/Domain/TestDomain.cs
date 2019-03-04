using System;

namespace DDD.Domain
{
  public class TestDomain : AggregateRoot
  {
    private TestDomain() { }

    public TestDomain(Guid id)
    {
      ApplyNewEvent(new TestDomainCreatedEvent(id));
    }

    private void Apply(TestDomainCreatedEvent e)
    {

    }
  }
}
