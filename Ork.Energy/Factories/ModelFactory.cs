using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.Factories
{
    public class ModelFactory
    {
        public static ConsumerGroup CreateConsumerGroup(string p1, string p2)
        {
            return new ConsumerGroup
            {
                GroupName = p1,
                GroupDescription = p2
            };
        }

        public static ConsumerGroup CreateConsumerGroup(string p1)
        {
            return new ConsumerGroup
            {
                GroupName = p1,
                GroupDescription = null
            };
        }

        public static Consumer CreateConsumer(string p1, Distributor distributor, ConsumerGroup consumerGroup)
        {
            return new Consumer
            {
                Name = p1,
                Distributor = distributor,
                ConsumerGroup = consumerGroup
            };
        }

        public static Distributor CreateDistributor(string p1)
        {
            return new Distributor
            {
                Name = p1
            };
        }
    }
}
