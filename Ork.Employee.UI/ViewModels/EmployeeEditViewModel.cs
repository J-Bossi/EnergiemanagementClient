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
using System.Collections.Generic;
using Ork.Employee.DomainModelService;
using Ork.Framework;
using Ork.Setting;

namespace Ork.Employee.UI.ViewModels
{
  public class EmployeeEditViewModel : EmployeeAddViewModel
  {
    private readonly string m_OldUserName;
    private readonly Action m_RemoveEmployee;
    private readonly ISettingsProvider m_SettingsProvider;

    public EmployeeEditViewModel(DomainModelService.Employee model, IEnumerable<EmployeeGroup> allGroups, Action removeEmployeeAction, IEmployeeRepository employeeRepository,
      ICredentialsManager credentialsManager, ISettingsProvider settingsProvider)
      : base(model, allGroups, employeeRepository, credentialsManager)
    {
      m_RemoveEmployee = removeEmployeeAction;
      m_SettingsProvider = settingsProvider;
      m_OldUserName = model.UserName;
      DisplayName = TranslationProvider.Translate("TitleEmployeeEditViewModel");
    }

    public bool IsNotLoggedIn
    {
      get { return m_SettingsProvider.User != m_OldUserName; }
    }

    protected override void CreateUser()
    {
      //user name has changed; perhaps password changed too
      if (!string.IsNullOrEmpty(m_OldUserName) &&
          !string.IsNullOrEmpty(UserName) &&
          UserName != m_OldUserName)
      {
        m_CredentialsManager.UpdateUser(m_OldUserName, UserName);
        m_CredentialsManager.UpdatePassword(UserName, Password);
      }
        //user is newly created
      else if (string.IsNullOrEmpty(m_OldUserName))
      {
        base.CreateUser();
      }
        //password has changed perhaps
      else if (!string.IsNullOrEmpty(m_OldUserName) &&
               UserName == m_OldUserName)
      {
        m_CredentialsManager.UpdatePassword(UserName, Password);
      }
    }

    public void RemoveEmployee()
    {
      m_RemoveEmployee();
      if (!string.IsNullOrEmpty(m_OldUserName))
      {
        m_CredentialsManager.Delete(m_OldUserName);
      }
      TryClose();
    }
  }
}