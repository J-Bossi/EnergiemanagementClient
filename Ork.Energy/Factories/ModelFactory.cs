﻿using System;
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
    }
}