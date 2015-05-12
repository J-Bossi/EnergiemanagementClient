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

using System.Linq;
using Moq;
using NUnit.Framework;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Tests
{
  [TestFixture]
  internal class ReadingTest
  {
    private Mock<Distributor> m_DistributorModel;
    private DistributorModifyViewModel m_DistributorModifyViewModel;
    private Mock<IEnergyRepository> m_EnergyRepository;
    private Mock<IEnergyViewModelFactory> m_EnergyViewModelFactory;

    [SetUp]
    public void SetUp()
    {
      m_DistributorModel = new Mock<Distributor>();
      m_DistributorModel.SetupGet(x => x.Name)
                        .Returns("Test");
      m_EnergyRepository = new Mock<IEnergyRepository>();
      m_EnergyViewModelFactory = new Mock<IEnergyViewModelFactory>();
      m_DistributorModifyViewModel = new DistributorModifyViewModel(m_DistributorModel.Object, m_EnergyRepository.Object,
        m_EnergyViewModelFactory.Object);
    }

    [Test]
    public void AddReading()
    {
      m_DistributorModifyViewModel.AddNewReading(new object());
      Assert.IsTrue(m_DistributorModifyViewModel.Readings.Count() == 1);
    }

    [Test]
    public void GetName()
    {
      Assert.AreEqual(m_DistributorModifyViewModel.Name, "Test");
    }
  }
}