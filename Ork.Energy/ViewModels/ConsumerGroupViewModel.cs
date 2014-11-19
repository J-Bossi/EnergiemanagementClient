using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
    public class ConsumerGroupViewModel
    {
        private readonly ConsumerGroup m_Model;

        public ConsumerGroupViewModel(ConsumerGroup consumerGroup)
        {
            m_Model = consumerGroup;
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
            get { return m_Model.Consumers.Count; }
        }
    }
}