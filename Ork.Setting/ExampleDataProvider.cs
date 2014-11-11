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
// Copyright (c) 2013, HTW Berlin

#endregion

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Ork.Setting.ExampleDataSeederService;

namespace Ork.Setting
{
  [Export]
  public class ExampleDataProvider
  {
    private readonly ISettingsProvider m_SettingsProvider;
    public EventHandler SeedCompleted;
    private ExampleDataSeederServiceClient m_ExampleDataSeederServiceClient;

    [ImportingConstructor]
    public ExampleDataProvider([Import] ISettingsProvider settingsProvider)
    {
      m_SettingsProvider = settingsProvider;
      m_SettingsProvider.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }

    private void DataSeedCompleted(object sender, AsyncCompletedEventArgs e)
    {
      if (e == null ||
          e.Cancelled == true ||
          e.Error != null)
      {
        return;
      }

      SeedCompleted(this, new EventArgs());
    }

    private void Initialize()
    {
      m_ExampleDataSeederServiceClient = new ExampleDataSeederServiceClient("BasicHttpBinding_ExampleDataSeederService", m_SettingsProvider.ConnectionString + "/ExampleDataSeeder");
      m_ExampleDataSeederServiceClient.ClientCredentials.UserName.UserName = m_SettingsProvider.User;
      m_ExampleDataSeederServiceClient.ClientCredentials.UserName.Password = m_SettingsProvider.Password;
      m_ExampleDataSeederServiceClient.SeedCompleted += DataSeedCompleted;
    }

    public void CreateExampleData()
    {
      m_ExampleDataSeederServiceClient.SeedAsync();
    }
  }
}