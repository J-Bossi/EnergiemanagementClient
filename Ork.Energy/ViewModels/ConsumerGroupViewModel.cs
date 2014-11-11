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

        

    }
}
