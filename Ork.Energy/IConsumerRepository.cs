using System;
using System.Data.Services.Client;
using Ork.Energy.DomainModelService;

namespace Ork.Energy
{
    public interface IConsumerRepository
        
    {
        DataServiceCollection<ConsumerGroup> ConsumerGroups { get; }
        DataServiceCollection<Building> Buildings { get; }
        DataServiceCollection<Distributor> Distributors { get; }
        DataServiceCollection<Reading> Readings { get; }
        bool HasConnection { get; }
        void Save();
        event EventHandler ContextChanged;
        event EventHandler SaveCompleted;
    }
}
