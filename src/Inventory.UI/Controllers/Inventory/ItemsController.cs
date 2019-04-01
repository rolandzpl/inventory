using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Inventory.UI.Controllers.Inventory
{
  [Route("api/Inventory/Items")]
  [ApiController]
  public class InventoryController : Controller
  {
    [HttpGet]
    public GetInventoryResponse GetInventory()
    {
      var response = new GetInventoryResponse
      {
        Items = new[] {
          new InventoryItem{ Symbol = "BC107", Category = "NPN Transistors", Description = "Low signal, low power NPN transistor", Quantity = 125 },
          new InventoryItem{ Symbol = "1N2007", Category = "Diodes", Description = "Diode", Quantity = 5 },
          new InventoryItem{ Symbol = "NE555", Category = "ICs", Description = "General purpose timer", Quantity = 5 },
        }
      };
      response.Refs.Add("_this", "api/Inventory/Items");
      foreach (var item in response.Items)
      {
        item.Refs.Add("_this", $"api/Inventory/Items/{item.Symbol}");
        item.Refs.Add("category", $"api/Inventory/Categories{item.Category}");
      }
      return response;
    }

    public class RestData
    {
      public RestData()
      {
        Refs = new Dictionary<string, string>();
      }

      public Dictionary<string, string> Refs { get; set; }
    }

    public class GetInventoryResponse : RestData
    {
      public InventoryItem[] Items { get; set; }

      public GrantedAction[] GrantedActions { get; set; }
    }

    public class GrantedAction : RestData
    {
      public string Name { get; set; }
    }

    public class InventoryItem : RestData
    {
      public string Symbol { get; set; }

      public string Category { get; internal set; }

      public int Quantity { get; set; }

      public string Description { get; set; }
    }
  }
}