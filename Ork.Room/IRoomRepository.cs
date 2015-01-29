using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using Ork.Room.DomainModelService;

namespace Ork.Room
{
  public interface IRoomRepository
  {

    DataServiceCollection<Building> Buildings { get; }

    bool HasConnection { get; }

    void Save();
    event EventHandler ContextChanged;
    event EventHandler SaveCompleted;
  }
}
