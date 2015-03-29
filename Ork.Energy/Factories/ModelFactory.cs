#region License

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0.html
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  
// Copyright (c) 2015, HTW Berlin

#endregion

using System;
using Ork.Energy.Domain.DomainModelService;

namespace Ork.Energy.Factories
{
  public class ModelFactory
  {
    public static ConsumerGroup CreateConsumerGroup(string p1)
    {
      return new ConsumerGroup
      {
        GroupName = p1,
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

    public static ConsumerType CreateConsumerType(string consumerType)
    {
      return new ConsumerType
      {
        TypeName = consumerType
      };
    }

    public static Reading CreateReading(DateTime readingDate, double counterReading, string measuringDevice)
    {
      return new Reading
      {
        CounterReading = counterReading,
        ReadingDate = readingDate,
        MeasuringDevice = measuringDevice
      };
    }
  }
}