using System.ComponentModel.Composition;
using System.Linq;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
    public class ConsumerGroupViewModel
    {
        private readonly ConsumerGroup m_Model;
        private readonly IConsumerRepository m_Repository;

        public ConsumerGroupViewModel(ConsumerGroup consumerGroup, [Import] IConsumerRepository consumerRepository)
        {
            m_Model = consumerGroup;
            m_Repository = consumerRepository;
        }

        public ConsumerGroup Model
        {
            get { return m_Model; }
        }

        public string GroupName
        {
            get { return m_Model.GroupName; }
            set { m_Model.GroupName = value; }
        }

        public string GroupDescription
        {
            get { return m_Model.GroupDescription; }
            set { m_Model.GroupDescription = value; }
        }

        public int RelatedConsumers
        {
            get { return m_Repository.Consumers.Count(c => c.ConsumerGroup == Model); }
        }
    }
}