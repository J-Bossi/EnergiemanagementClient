using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ork.Consumer.DomainModelService;

namespace Ork.Consumer.ViewModels
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
            get { return Model; }
        }

        public string

    }
}
