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

using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public class ResponsibleSubjectViewModel : Screen
  {
    private readonly ResponsibleSubject m_Model;

    public ResponsibleSubjectViewModel(ResponsibleSubject model)
    {
      m_Model = model;
    }

    public ResponsibleSubject Model
    {
      get { return m_Model; }
    }

    public string Infotext
    {
      get
      {
        if (m_Model is Employee)
        {
          var employee = (Employee) m_Model;
          return employee.FirstName + " " + employee.LastName;
        }
        else
        {
          var group = (EmployeeGroup) m_Model;
          return group.Name;
        }
      }
    }
  }
}