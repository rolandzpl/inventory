using DDD.Domain;
using System;

namespace Inventory.Domain
{
  public class Inventory : AggregateRoot<Guid>
  {
    private int actualAmount;

    public int Amount
    {
      get { return actualAmount; }
    }

    public static Inventory Create()
    {
      return new Inventory(Guid.NewGuid());
    }

    private Inventory(Guid id)
    {
      ApplyNewEvent(new InventoryCreatedEvent(id));
    }

    private Inventory() { }

    private void Apply(InventoryCreatedEvent e)
    {
      Id = e.Id;
    }

    public void Increase(int amount)
    {
      ApplyNewEvent(new InventoryIncreasedEvent(amount));
    }

    private void Apply(InventoryIncreasedEvent e)
    {
      actualAmount += e.Amount;
    }

    public void Decrease(int amount)
    {
      if (actualAmount < amount)
      {
        throw new InvalidAmountException();
      }
      ApplyNewEvent(new InventoryDecreasedEvent(amount));
    }

    private void Apply(InventoryDecreasedEvent e)
    {
      actualAmount -= e.Amount;
    }
  }
}
