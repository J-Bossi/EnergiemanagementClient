﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using NUnit.Framework;

namespace Ork.Energy.Tests
{
    internal class EnergyRepositoryTest
    {
      private IConsumerRepository m_Repository;

      [ImportingConstructor]
      public EnergyRepositoryTest([Import] IConsumerRepository mConsumerRepository)
      {
        m_Repository = mConsumerRepository;
      }
      
      [Test]
      public void InitializeRepository()
      {
        Assert.NotNull(m_Repository);
      }
    }
}
