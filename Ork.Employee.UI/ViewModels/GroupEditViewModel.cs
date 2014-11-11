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

namespace Ork.Employee.UI.ViewModels
{
  public class GroupEditViewModel : GroupAddViewModel
  {
    private readonly Action m_RemoveEmployeeGroup;

    public GroupEditViewModel(EmployeeGroup model, IEnumerable<DomainModelService.Employee> allEmployees, Action removeEmployeeGroupAction, IEmployeeRepository employeeRepository)
      : base(model, allEmployees, employeeRepository)
    {
      m_RemoveEmployeeGroup = removeEmployeeGroupAction;
      DisplayName = TranslationProvider.Translate("TitleGroupEditViewModel");
    }

    public void RemoveEmployeeGroup()
    {
      m_RemoveEmployeeGroup();
    }
  }
}