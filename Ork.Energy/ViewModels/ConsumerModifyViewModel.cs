using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using Caliburn.Micro;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
    public class ConsumerModifyViewModel : Screen
    {
        private readonly Consumer m_Model;
        private readonly IConsumerRepository m_Repository;

        public ConsumerModifyViewModel(Consumer consumer, [Import] IConsumerRepository consumerRepository)
        {
            DisplayName = "Verbraucher bearbeiten...";
            m_Model = consumer;
            m_Repository = consumerRepository;
        }

        public virtual Room Room
        {
            get { return m_Model.Room; }
            set { m_Model.Room = value; }
        }

        public virtual Distributor Distributor
        {
            get { return m_Model.Distributor; }
            set { m_Model.Distributor = value; }
        }

        public virtual ConsumerGroup ConsumerGroup
        {
            get { return m_Model.ConsumerGroup; }
            set { m_Model.ConsumerGroup = value; }
        }


        public string Name
        {
            get { return m_Model.Name; }
            set { m_Model.Name = value; }
        }

        public DataServiceCollection<Measure> Measures
        {
            get { return m_Model.Measures; }
            set { m_Model.Measures = value; }
        }

        public virtual long? PowerOutput
        {
            get { return m_Model.PowerOutput; }
            set { m_Model.PowerOutput = value; }
        }

        public virtual long? PowerCurrent
        {
            get { return m_Model.PowerCurrent; }
            set { m_Model.PowerCurrent = value; }
        }

        public virtual DataServiceCollection<Reading> Readings
        {
            get { return m_Model.Readings; }
        }

        public virtual string Comment
        {
            get { return m_Model.Comment; }
            set { m_Model.Comment = value; }
        }

        public int? Year
        {
            get { return m_Model.Year; }
            set { m_Model.Year = value; }
        }

        public virtual string Manufacturer
        {
            get { return m_Model.Manufacturer; }
            set { m_Model.Manufacturer = value; }
        }

        public IEnumerable<Distributor> AllDistributors
        {
            get { return m_Repository.Distributors; }
        }

        public IEnumerable<ConsumerGroup> AllConsumerGroups
        {
            get { return m_Repository.ConsumerGroups; }
        } 
    }
}