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
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Ork.Employee.DomainModelService;
using Ork.Employee.UI.Factories;
using Ork.Framework;
using Ork.Setting;

namespace Ork.Employee.UI.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class EmployeeManagementViewModel : DocumentBase, IWorkspace
  {
    private readonly IEmployeeViewModelFactory m_EmployeeViewModelFactory;
    private readonly BindableCollection<EmployeeViewModel> m_Employees = new BindableCollection<EmployeeViewModel>();
    private readonly IGroupViewModelFactory m_GroupViewModelFactory;
    private readonly BindableCollection<GroupViewModel> m_Groups = new BindableCollection<GroupViewModel>();
    private readonly IEmployeeRepository m_Repository;
    private readonly ISettingsProvider m_SettingsProvider;
    private IScreen m_EditItem;
    private string m_EmployeeGroupSearchText;
    private string m_EmployeeSearchText;
    private Visibility m_HaveEditsBeenMade = Visibility.Collapsed;
    private bool m_IsEnabled;
    private EmployeeViewModel m_SelectedEmployee;
    private GroupViewModel m_SelectedEmployeeGroup;

    [ImportingConstructor]
    public EmployeeManagementViewModel([Import] IEmployeeRepository contextRepository, [Import] IEmployeeViewModelFactory employeeViewModelFactory,
      [Import] IGroupViewModelFactory groupViewModelFactory, [Import] ISettingsProvider settingsProvider)
    {
      m_Repository = contextRepository;
      m_EmployeeViewModelFactory = employeeViewModelFactory;
      m_GroupViewModelFactory = groupViewModelFactory;
      m_SettingsProvider = settingsProvider;
      m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);
      m_Repository.SaveCompleted += (s, e) => ShowInfoBox();

      Reload();
    }

    public Visibility HaveEditsBeenMade
    {
      get { return m_HaveEditsBeenMade; }
      set
      {
        m_HaveEditsBeenMade = value;
        NotifyOfPropertyChange(() => HaveEditsBeenMade);
      }
    }

    public string EmployeeGroupSearchText
    {
      get { return m_EmployeeGroupSearchText; }
      set
      {
        m_EmployeeGroupSearchText = value;
        NotifyOfPropertyChange(() => FilteredEmployeeGroups);
      }
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

    public IEnumerable<GroupViewModel> FilteredEmployeeGroups
    {
      get
      {
        return SearchInGroupList()
          .ToArray();
      }
    }

    public IEnumerable<EmployeeViewModel> FilteredEmployees
    {
      get
      {
        return SearchInEmployeeList()
          .ToArray();
      }
    }

    public override bool IsDirty
    {
      get { return m_EditItem != null && m_EditItem.IsActive; }
      set { base.IsDirty = value; }
    }

    public EmployeeViewModel SelectedEmployee
    {
      get { return m_SelectedEmployee; }
      set
      {
        if (m_SelectedEmployee == value)
        {
          return;
        }
        m_SelectedEmployee = value;
        NotifyOfPropertyChange(() => SelectedEmployee);
      }
    }

    public GroupViewModel SelectedEmployeeGroup
    {
      get { return m_SelectedEmployeeGroup; }
      set
      {
        if (m_SelectedEmployeeGroup == value)
        {
          return;
        }
        m_SelectedEmployeeGroup = value;
        NotifyOfPropertyChange(() => SelectedEmployeeGroup);
      }
    }

    public int Index
    {
      get { return 500; }
    }

    public bool IsEnabled
    {
      get { return m_IsEnabled; }
      private set
      {
        m_IsEnabled = value;
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }

    public string Title
    {
      get { return TranslationProvider.Translate("TitleEmployeeManagementViewModel"); }
    }

    public void RefreshDataSource()
    {
      m_SettingsProvider.Refresh();
      HaveEditsBeenMade = Visibility.Collapsed;
    }

    private void ShowInfoBox()
    {
      HaveEditsBeenMade = Visibility.Visible;
    }

    private void Reload()
    {
      IsEnabled = m_Repository.HasConnection;
      if (IsEnabled)
      {
        LoadData();
      }
    }

    public void AddEmployee()
    {
      OpenAddDialog(m_EmployeeViewModelFactory.CreateNew());
    }

    public void AddEmployeeGroup()
    {
      OpenAddDialog(m_GroupViewModelFactory.CreateNew());
    }

    private void CloseEditor()
    {
      m_EditItem.TryClose();
    }

    public void OpenEditor(object dataContext)
    {
      m_EditItem = dataContext is GroupViewModel
        ? (IScreen) m_GroupViewModelFactory.CreateEditViewModelFromExisting(((GroupViewModel) dataContext).Model, m_Employees.Select(g => g.Model)
                                                                                                                             .ToArray(), RemoveEmployeeGroup)
        : m_EmployeeViewModelFactory.CreateEditViewModelFromExisting(((EmployeeViewModel) dataContext).Model, m_Groups.Select(g => g.Model)
                                                                                                                      .ToArray(), RemoveEmployee);
      Dialogs.ShowDialog(m_EditItem);
    }

    public void OpenEditor(object dataContext, MouseButtonEventArgs e)
    {
      if (e.ClickCount >= 2)
      {
        OpenEditor(dataContext);
      }
    }

    public void RemoveEmployee()
    {
      var evm = SelectedEmployee;
      if (evm == null)
      {
        return;
      }
      foreach (var employeeGroup in evm.Model.Groups)
      {
        m_Repository.Context.DeleteLink(evm.Model, "Groups", employeeGroup);
      }
      m_Repository.ResponsibleSubjects.Remove(evm.Model);
      Save();
    }

    public void RemoveEmployeeGroup()
    {
      var egvm = SelectedEmployeeGroup;
      if (egvm == null)
      {
        return;
      }
      foreach (var responsibleSubject in m_Repository.ResponsibleSubjects.OfType<DomainModelService.Employee>())
      {
        if (responsibleSubject.Groups.Contains(egvm.Model))
        {
          var link = m_Repository.Context.Links.SingleOrDefault(l => l.Source == responsibleSubject && l.Target == egvm.Model);
          if (link != null)
          {
            if (link.State == EntityStates.Unchanged)
            {
              m_Repository.Context.DeleteLink(responsibleSubject, "Groups", egvm.Model);
            }
            else if (link.State == EntityStates.Added)
            {
              responsibleSubject.Groups.Remove(egvm.Model);
            }
          }
        }
      }
      m_Repository.ResponsibleSubjects.Remove(egvm.Model);
      CloseEditor();
      Save();
    }

    public void Cancel()
    {
      CloseEditor();
    }

    private void Save()
    {
      if (m_Repository.Context.Entities.Any(ed => ed.State != EntityStates.Unchanged) ||
          m_Repository.Context.Links.Any(ed => ed.State != EntityStates.Unchanged))
      {
        m_Repository.Save();
      }
    }

    private void CreateEmployeeGroupViewModel(EmployeeGroup employeeGroup)
    {
      var egvm = m_GroupViewModelFactory.CreateFromExisting(employeeGroup);
      m_Groups.Add(egvm);
    }

    private void CreateEmployeeViewModel(DomainModelService.Employee employee)
    {
      var evm = m_EmployeeViewModelFactory.CreateFromExisting(employee);
      m_Employees.Add(evm);
    }

    private void LoadData()
    {
      m_Employees.Clear();
      m_Groups.Clear();
      LoadResponsibleSubjects();
    }

    private void LoadResponsibleSubjects()
    {
      foreach (var responsibleSubject in m_Repository.ResponsibleSubjects)
      {
        if (responsibleSubject is EmployeeGroup)
        {
          CreateEmployeeGroupViewModel((EmployeeGroup) responsibleSubject);
        }
        else
        {
          CreateEmployeeViewModel((DomainModelService.Employee) responsibleSubject);
        }
      }
      NotifyOfPropertyChange(() => FilteredEmployeeGroups);
      NotifyOfPropertyChange(() => FilteredEmployees);
    }

    private void OpenAddDialog(object dataContext)
    {
      m_EditItem = dataContext is GroupViewModel
        ? (IScreen) m_GroupViewModelFactory.CreateAddViewModelFromExisting(((GroupViewModel) dataContext).Model, m_Employees.Select(g => g.Model)
                                                                                                                            .ToArray())
        : m_EmployeeViewModelFactory.CreateAddViewModelFromExisting(((EmployeeViewModel) dataContext).Model, m_Groups.Select(g => g.Model)
                                                                                                                     .ToArray());
      Dialogs.ShowDialog(m_EditItem);
    }

    private IEnumerable<EmployeeViewModel> SearchInEmployeeList()
    {
      if (string.IsNullOrEmpty(EmployeeSearchText))
      {
        return m_Employees;
      }
      var searchText = EmployeeSearchText.ToLower();

      var searchResult = m_Employees.Where(c => (((c.FirstName != null) && (c.FirstName.ToLower()
                                                                             .Contains(searchText))) || ((c.LastName != null) && (c.LastName.ToLower()
                                                                                                                                   .Contains(searchText)))) ||
                                                ((c.Number != null) && (c.Number.ToLower()
                                                                         .Contains(searchText))) || ((c.Groups != null) && (c.Groups.Contains(c.Groups.FirstOrDefault(g => g.Name.ToLower()
                                                                                                                                                                            .Contains(searchText))))));
      return searchResult;
    }

    private IEnumerable<GroupViewModel> SearchInGroupList()
    {
      if (string.IsNullOrEmpty(EmployeeGroupSearchText))
      {
        return m_Groups;
      }
      var searchText = EmployeeGroupSearchText.ToLower();

      var searchResult = m_Groups.Where(g => (((g.Name != null) && (g.Name.ToLower()
                                                                     .Contains(searchText))) || (m_Employees.Any(e => (((e.FirstName != null) && (e.FirstName.ToLower()
                                                                                                                                                   .Contains(searchText)) ||
                                                                                                                        ((e.LastName != null) && (e.LastName.ToLower()
                                                                                                                                                   .Contains(searchText))) ||
                                                                                                                        ((e.Number != null) && (e.Number.ToLower()
                                                                                                                                                 .Contains(searchText)))) &&
                                                                                                                       e.Model.Groups.Contains(g.Model))))));

      return searchResult;
    }
  }
}