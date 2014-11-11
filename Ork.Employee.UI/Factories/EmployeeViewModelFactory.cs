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
using System.ComponentModel.Composition;
using Ork.Employee.DomainModelService;
using Ork.Employee.UI.ViewModels;
using Ork.Setting;

namespace Ork.Employee.UI.Factories
{
  [Export(typeof (IEmployeeViewModelFactory))]
  public class EmployeeViewModelFactory : IEmployeeViewModelFactory
  {
    private readonly ICredentialsManager m_CredentialsManager;
    private readonly IEmployeeRepository m_EmployeeRepository;
    private readonly ISettingsProvider m_SettingsProvider;

    [ImportingConstructor]
    public EmployeeViewModelFactory([Import] IEmployeeRepository employeeRepository, [Import] ICredentialsManager credentialsManager, [Import] ISettingsProvider settingsProvider)
    {
      m_EmployeeRepository = employeeRepository;
      m_CredentialsManager = credentialsManager;
      m_SettingsProvider = settingsProvider;
    }

    public EmployeeAddViewModel CreateAddViewModelFromExisting(DomainModelService.Employee model, IEnumerable<EmployeeGroup> allGroups)
    {
      return new EmployeeAddViewModel(model, allGroups, m_EmployeeRepository, m_CredentialsManager);
    }

    public EmployeeEditViewModel CreateEditViewModelFromExisting(DomainModelService.Employee model, IEnumerable<EmployeeGroup> allGroups, Action removeEmployeeAction)
    {
      return new EmployeeEditViewModel(model, allGroups, removeEmployeeAction, m_EmployeeRepository, m_CredentialsManager, m_SettingsProvider);
    }

    public EmployeeViewModel CreateFromExisting(DomainModelService.Employee employee)
    {
      return new EmployeeViewModel(employee);
    }

    public EmployeeViewModel CreateNew()
    {
      return CreateFromExisting(new DomainModelService.Employee());
    }
  }
}