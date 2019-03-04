using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Domain
{
  class RepositoryTests
  {
    [Test]
    public void GetItemById_GivenHistory_ReturnsInstantInFinalState()
    {
      var id = Guid.NewGuid();
      var history = new Event[]
      {
        new TestDomainCreatedEvent(id),
      };
      var repository = new Repository<TestDomain>(_ => history, _ => { });
      var obj = repository.GetItemById(id);
      Assert.That(obj, Is.Not.Null);
    }

    [Test]
    public void Save_GivenHistory_ReturnsInstantInFinalState()
    {
      var savedEvents = new List<Event>();
      var repository = new Repository<TestDomain>(_ => Enumerable.Empty<Event>(), _ => savedEvents.AddRange(_));
      var id = Guid.NewGuid();
      var obj = new TestDomain(id);
      repository.Save(obj);
      Assert.That(savedEvents, Is.Not.Empty);
    }

    [Test]
    public void Save_Successfull_UncommittedChangesAreEmpty()
    {
      var id = Guid.NewGuid();
      var obj = new TestDomain(id);
      var repository = new Repository<TestDomain>(_ => Enumerable.Empty<Event>(), _ => { });
      repository.Save(obj);
      Assert.That(obj.GetUncommittedChanges(), Is.Empty);
    }

    [Test]
    public void Save_OnError_AllChangesRemainInInstance()
    {
      var id = Guid.NewGuid();
      var obj = new TestDomain(id);
      var repository = new Repository<TestDomain>(_ => Enumerable.Empty<Event>(), _ => throw new Exception());
      try { repository.Save(obj); } catch { }
      Assert.That(obj.GetUncommittedChanges(), Is.Not.Empty);
    }
  }
}
