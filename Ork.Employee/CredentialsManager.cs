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

using System.ComponentModel.Composition;
using Ork.Employee.CredentialsManagement;
using Ork.Setting;

namespace Ork.Employee
{
  [Export(typeof (ICredentialsManager))]
  internal class CredentialsManager : ICredentialsManager
  {
    private readonly ISettingsProvider m_SettingsProvider;
    private CredentialsDatabaseAccessorClient m_Client;

    [ImportingConstructor]
    public CredentialsManager([Import] ISettingsProvider settingsProvider)
    {
      m_SettingsProvider = settingsProvider;
      m_SettingsProvider.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }

    public void Create(string user, string password)
    {
      m_Client.Create(user, password);
    }

    public void UpdateUser(string oldUser, string newUser)
    {
      m_Client.UpdateUser(oldUser, newUser);
    }

    public void UpdatePassword(string user, string password)
    {
      m_Client.UpdatePassword(user, password);
    }

    public void Delete(string user)
    {
      m_Client.Delete(user);
    }

    private void Initialize()
    {
      m_Client = new CredentialsDatabaseAccessorClient("BasicHttpBinding_CredentialsDatabaseAccessor", m_SettingsProvider.ConnectionString + "/CredentialsManagement");
      m_Client.ClientCredentials.UserName.UserName = m_SettingsProvider.User;
      m_Client.ClientCredentials.UserName.Password = m_SettingsProvider.Password;
    }
  }
}