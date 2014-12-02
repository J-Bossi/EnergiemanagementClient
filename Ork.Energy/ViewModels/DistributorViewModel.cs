using System.Collections.Generic;
using System.ComponentModel.Composition;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
    public class DistributorViewModel
    {
        private readonly Distributor m_Model;
        private readonly IConsumerRepository m_Repository;

        public DistributorViewModel(Distributor distributor, [Import] IConsumerRepository consumerRepository)
        {
            m_Model = distributor;
            m_Repository = consumerRepository;
        }

        public Distributor Model
        {
            get { return m_Model; }
        }

        public string Name
        {
            get { return m_Model.Name; }
            
        }

        public ICollection<Reading> Readings
        {
            get { return m_Model.Readings; }
        }

        public bool IsMainDistributor
        {
            get { return m_Model.IsMainDistributor; }
        }

        public Room Room
        {
            get { return m_Model.Room; }
        }
    }
}