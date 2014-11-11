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
using System.Linq;
using Caliburn.Micro;
using Ork.Employee.DomainModelService;
using Ork.Framework;

namespace Ork.Employee.UI.ViewModels
{
  public class EmployeeAddViewModel : Screen
  {
    private readonly IEnumerable<EmployeeGroup> m_AllGroups;
    protected readonly ICredentialsManager m_CredentialsManager;
    private readonly IEmployeeRepository m_EmployeeRepository;
    private readonly List<SelectableGroupViewModel> m_GroupList;
    private readonly DomainModelService.Employee m_Model;
    private string m_GroupSearchText;

    public EmployeeAddViewModel(DomainModelService.Employee model, IEnumerable<EmployeeGroup> allGroups, IEmployeeRepository employeeRepository, ICredentialsManager credentialsManager)
    {
      m_Model = model;
      Password = string.Empty;
      m_AllGroups = allGroups;
      m_EmployeeRepository = employeeRepository;
      m_CredentialsManager = credentialsManager;
      DisplayName = TranslationProvider.Translate("TitleEmployeeAddViewModel");
      m_GroupList = new List<SelectableGroupViewModel>();
      CreateSelectableGroupViewModels();
    }

    public IEnumerable<SelectableGroupViewModel> FilteredGroups
    {
      get
      {
        return SearchInGroupList()
          .ToArray();
      }
    }

    public string FirstName
    {
      get { return m_Model.FirstName; }
      set { m_Model.FirstName = value; }
    }

    public string GroupSearchText
    {
      get { return m_GroupSearchText; }
      set
      {
        m_GroupSearchText = value;
        NotifyOfPropertyChange(() => FilteredGroups);
      }
    }

    public IEnumerable<SelectableGroupViewModel> Groups
    {
      get { return m_GroupList; }
    }

    public string LastName
    {
      get { return m_Model.LastName; }
      set { m_Model.LastName = value; }
    }

    public string Password { get; set; }

    public string UserName
    {
      get { return m_Model.UserName; }
      set { m_Model.UserName = value; }
    }

    public DomainModelService.Employee Model
    {
      get { return m_Model; }
    }

    public string Number
    {
      get { return m_Model.Number; }
      set { m_Model.Number = value; }
    }

    private void CreateSelectableGroupViewModels()
    {
      foreach (var employeeGroup in m_AllGroups)
      {
        var sgvm = new SelectableGroupViewModel(employeeGroup, m_Model.Groups.Contains(employeeGroup));
        m_GroupList.Add(sgvm);
      }
    }

    private IEnumerable<SelectableGroupViewModel> SearchInGroupList()
    {
      if (string.IsNullOrEmpty(GroupSearchText))
      {
        return m_GroupList;
      }
      var searchText = GroupSearchText.ToLower();
      var searchResult = m_GroupList.Where(c => (((c.Model.Name != null) && (c.Model.Name.ToLower()
                                                                              .Contains(searchText)))));
      return searchResult;
    }

    public void Add()
    {
      m_EmployeeRepository.ResponsibleSubjects.Add(Model);
      Accept();
    }

    public void Accept()
    {
      CreateUser();

      foreach (var selectableGroupViewModel in Groups)
      {
        if (selectableGroupViewModel.IsSelected &&
            !Model.Groups.Contains(selectableGroupViewModel.Model))
        {
          Model.Groups.Add(selectableGroupViewModel.Model);
        }
        else if (!selectableGroupViewModel.IsSelected &&
                 Model.Groups.Contains(selectableGroupViewModel.Model))
        {
          var link = m_EmployeeRepository.Context.Links.SingleOrDefault(l => l.Source == Model && l.Target == selectableGroupViewModel.Model);
          if (link != null)
          {
            m_EmployeeRepository.Context.DeleteLink(Model, "Groups", selectableGroupViewModel.Model);
          }
        }
      }

      m_EmployeeRepository.Save();

      TryClose();
    }

    protected virtual void CreateUser()
    {
      if (string.IsNullOrEmpty(UserName))
      {
        return;
      }
      m_CredentialsManager.Create(UserName, Password);
    }
  }
}