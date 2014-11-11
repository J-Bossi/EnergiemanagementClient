using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ork.Consumer.DomainModelService;

namespace Ork.Consumer
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
