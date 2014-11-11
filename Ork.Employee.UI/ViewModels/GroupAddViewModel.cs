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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Ork.Employee.DomainModelService;
using Ork.Framework;

namespace Ork.Employee.UI.ViewModels
{
  public class GroupAddViewModel : Screen
  {
    private readonly IEnumerable<DomainModelService.Employee> m_AllEmployees;
    private readonly ObservableCollection<SelectableEmployeeViewModel> m_EmployeeList = new ObservableCollection<SelectableEmployeeViewModel>();
    private readonly IEmployeeRepository m_EmployeeRepository;
    private readonly EmployeeGroup m_Model;
    private string m_EmployeeSearchText;

    public GroupAddViewModel(EmployeeGroup model, IEnumerable<DomainModelService.Employee> allEmployees, IEmployeeRepository employeeRepository)
    {
      m_Model = model;
      m_AllEmployees = allEmployees;
      m_EmployeeRepository = employeeRepository;
      DisplayName = TranslationProvider.Translate("TitleGroupAddViewModel");
      CreateSelectableEmployeeViewModels();
    }

    public string EmployeeSearchText
    {
      get { return m_EmployeeSearchText; }
      set
      {
        m_EmployeeSearchText = value;
        NotifyOfPropertyChange(() => FilteredEmployees);
      }
    }

    public IEnumerable<SelectableEmployeeViewModel> Employees
    {
      get { return m_EmployeeList; }
    }

    public IEnumerable<SelectableEmployeeViewModel> FilteredEmployees
    {
      get
      {
        return new ObservableCollection<SelectableEmployeeViewModel>(SearchInEmployeeList()
          .ToArray());
      }
    }


    public EmployeeGroup Model
    {
      get { return m_Model; }
    }


    public string Name
    {
      get { return m_Model.Name; }
      set { m_Model.Name = value; }
    }

    private void CreateSelectableEmployeeViewModels()
    {
      foreach (var employee in m_AllEmployees)
      {
        var sevm = new SelectableEmployeeViewModel(employee, employee.Groups.Contains(m_Model));
        m_EmployeeList.Add(sevm);
      }
    }

    private IEnumerable<SelectableEmployeeViewModel> SearchInEmployeeList()
    {
      if (string.IsNullOrEmpty(EmployeeSearchText))
      {
        return m_EmployeeList;
      }
      var searchText = EmployeeSearchText.ToLower();

      var searchResult = m_EmployeeList.Where(c => (((c.Model.FirstName != null) && (c.Model.FirstName.ToLower()
                                                                                      .Contains(searchText))) || ((c.Model.LastName != null) && (c.Model.LastName.ToLower()
                                                                                                                                                  .Contains(searchText)))) ||
                                                   ((c.Model.Number != null) && (c.Model.Number.ToLower()
                                                                                  .Contains(searchText))) ||
                                                   ((c.Model.Groups != null) && (c.Model.Groups.Contains(c.Model.Groups.FirstOrDefault(g => g.Name.ToLower()
                                                                                                                                             .Contains(searchText))))));
      return searchResult;
    }

    public void Add()
    {
      m_EmployeeRepository.ResponsibleSubjects.Add(Model);
      Accept();
    }

    public void Accept()
    {
      foreach (var selectableEmployeeViewModel in Employees)
      {
        if (selectableEmployeeViewModel.IsSelected &&
            !selectableEmployeeViewModel.Model.Groups.Contains(Model))
        {
          selectableEmployeeViewModel.Model.Groups.Add(Model);
        }
        else if (!selectableEmployeeViewModel.IsSelected &&
                 selectableEmployeeViewModel.Model.Groups.Contains(Model))
        {
          var link = m_EmployeeRepository.Context.Links.SingleOrDefault(l => l.Source == selectableEmployeeViewModel.Model && l.Target == Model);
          if (link != null)
          {
            m_EmployeeRepository.Context.DeleteLink(selectableEmployeeViewModel.Model, "Groups", Model);
          }
        }
      }
      m_EmployeeRepository.Save();
      TryClose();
    }
  }
}