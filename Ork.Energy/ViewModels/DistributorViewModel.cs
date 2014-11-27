using System.Collections.Generic;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
    public class DistributorViewModel
    {
        private readonly Distributor m_Model;

        public DistributorViewModel(Distributor distributor)
        {
            m_Model = distributor;
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