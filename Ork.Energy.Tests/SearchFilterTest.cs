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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using System.Runtime.InteropServices;
using Moq;
using NUnit.Framework;
using Ork.Energy.DomainModelService;
using Ork.Energy.Factories;
using Ork.Energy.ViewModels;
using Ork.Framework;

namespace Ork.Energy.Tests
{
  [TestFixture]
  internal class SearchFilterTest
  {
    private EnergyManagementViewModel m_EnergyManagementViewModel;
    private Mock<IDialogManager> m_DialogManagerMock;
    private Mock<IEnergyViewModelFactory> m_EnergyViewModelFactoryMock;
    private Mock<IEnergyRepository> m_Repository;
    private Mock<DomainModelContext> m_Context;

    [SetUp]
    public void SetUp()
    {
      m_DialogManagerMock = new Mock<IDialogManager>();
      m_EnergyViewModelFactoryMock = new Mock<IEnergyViewModelFactory>();
      m_Repository = new Mock<IEnergyRepository>();


      var items = new Consumer[]
      {new Consumer(), new Consumer(), }.AsQueryable();

      DataServiceCollection<Consumer> sdf = new DataServiceCollection<Consumer>();
      sdf.Load(items);

       m_Repository.SetupGet(r => r.Consumers).Returns(sdf);
   
      m_EnergyManagementViewModel = new EnergyManagementViewModel(m_Repository.Object, m_EnergyViewModelFactoryMock.Object, m_DialogManagerMock.Object);

      m_EnergyManagementViewModel.ClickedConsumerGroup = new ConsumerGroupViewModel(new ConsumerGroup() {GroupName = "Consumer"}, m_Repository.Object);
      m_EnergyManagementViewModel.ClickedDistributor = new DistributorViewModel(new Distributor(){Name = "Verteiler"}, m_Repository.Object);
      m_EnergyManagementViewModel.NewConsumerName = "NeuerVerbraucher";
 
      m_EnergyManagementViewModel.AddNewConsumer();

      
    }

    [Test]
    public void NoRestrictions()
    {
      Assert.IsTrue(m_EnergyManagementViewModel.Consumers.Count() == 1);
    }
   

  }
}