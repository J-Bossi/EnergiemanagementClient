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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;
using Action = System.Action;

namespace Ork.Energy.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class EnergyManagementViewModel : DocumentBase, IWorkspace
  {
    private readonly BindableCollection<ConsumerGroupViewModel> m_ConsumerGroups =
      new BindableCollection<ConsumerGroupViewModel>();

    private readonly BindableCollection<ConsumerViewModel> m_Consumers = new BindableCollection<ConsumerViewModel>();
    private readonly BindableCollection<DistributorViewModel> m_Distributors = new BindableCollection<DistributorViewModel>();
    private readonly IEnergyViewModelFactory m_EnergyViewModelFactory;
    private readonly IEnergyRepository m_Repository;
    private IScreen m_EditItem;
    private bool m_FlyoutActivated;
    private bool m_IsEnabled;
    private string m_SearchConsumerGroupText;
    private string m_SearchConsumerText;
    private string m_SearchDistributorText;
    private ConsumerViewModel m_SelectedConsumer;
    private ConsumerGroupViewModel m_SelectedConsumerGroup;
    private DistributorViewModel m_SelectedDistributor;

    [ImportingConstructor]
    public EnergyManagementViewModel([Import] IEnergyRepository mRepository,
                                     [Import] IEnergyViewModelFactory mEnergyViewModelFactory, [Import] IDialogManager dialogs)
    {
      Dialogs = dialogs;
      m_Repository = mRepository;
      m_EnergyViewModelFactory = mEnergyViewModelFactory;

      m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);
      Reload();
    }

    public new IDialogManager Dialogs { get; private set; }

    public IEnumerable<ConsumerGroupViewModel> ConsumerGroups
    {
      get { return SearchInConsumerGroupList(FilteredConsumerGroups.OrderBy(cg => cg.GroupName)); }
    }

    public IEnumerable<ConsumerViewModel> Consumers
    {
      get { return SearchInConsumerList(FilteredConsumers.OrderBy(c => c.Name)); }
    }

    public IEnumerable<DistributorViewModel> Distributors
    {
      get { return SearchInDistributorList(FilteredDistributors.OrderBy(d => d.Name)); }
    }

    private IEnumerable<ConsumerGroupViewModel> FilteredConsumerGroups
    {
      get
      {
        var filteredConsumerGroups = m_ConsumerGroups;

        if (SelectedDistributor != null)
        {
          var consumerList = m_Consumers.Where(cg => cg.Model.Distributor.Equals(SelectedDistributor.Model));
          var returnList = new List<ConsumerGroupViewModel>();
          foreach (var consumerViewModel in consumerList)
          {
            returnList.AddRange(filteredConsumerGroups.Where(cg => cg.Model.Equals(consumerViewModel.Model.ConsumerGroup)));
          }
          return returnList.Distinct();
        }

        return filteredConsumerGroups;
      }
    }

    private IEnumerable<ConsumerViewModel> FilteredConsumers
    {
      //Filters after SearchText and the available objects in other List. Only related Objects are shown
      get
      {
        var filteredConsumers = m_Consumers;
        if (SelectedConsumerGroup == null &&
            SelectedDistributor == null)
        {
          return filteredConsumers;
        }
        if (SelectedConsumerGroup != null &&
            SelectedDistributor == null)
        {
          return filteredConsumers.Where(c => c.Model.ConsumerGroup.Equals(SelectedConsumerGroup.Model));
        }
        if (SelectedDistributor != null &&
            SelectedConsumerGroup == null)
        {
          return filteredConsumers.Where(c => c.Model.Distributor.Equals(SelectedDistributor.Model));
        }

        return
          filteredConsumers.Where(
            c =>
              c.Model.Distributor.Equals(SelectedDistributor.Model) && c.Model.ConsumerGroup.Equals(SelectedConsumerGroup.Model));
      }
    }

    private IEnumerable<DistributorViewModel> FilteredDistributors
    {
      get
      {
        var filteredDistributors = m_Distributors;

        if (SelectedConsumerGroup != null)
        {
          var consumerList = m_Consumers.Where(cg => cg.Model.ConsumerGroup.Equals(SelectedConsumerGroup.Model));
          var returnList = new List<DistributorViewModel>();
          foreach (var consumerViewModel in consumerList)
          {
            returnList.AddRange(filteredDistributors.Where(d => d.Model.Equals(consumerViewModel.Model.Distributor)));
          }
          return returnList.Distinct();
        }

        return filteredDistributors;
      }
    }

    public string SearchConsumerGroupsText
    {
      get { return m_SearchConsumerGroupText; }
      set
      {
        m_SearchConsumerGroupText = value;

        NotifyOfPropertyChange(() => ConsumerGroups);
      }
    }

    public string SearchConsumerText
    {
      get { return m_SearchConsumerText; }
      set
      {
        m_SearchConsumerText = value;

        NotifyOfPropertyChange(() => Consumers);
      }
    }

    public string SearchDistributorText
    {
      get { return m_SearchDistributorText; }
      set
      {
        m_SearchDistributorText = value;

        NotifyOfPropertyChange(() => Distributors);
      }
    }

    public string NewConsumerGroupName { get; set; }
    public string NewConsumerName { get; set; }
    public string NewDistributorName { get; set; }

    public ConsumerGroupViewModel SelectedConsumerGroup
    {
      get { return m_SelectedConsumerGroup; }
      set
      {
        m_SelectedConsumerGroup = value;
        NotifyOfPropertyChange(() => Consumers);
        NotifyOfPropertyChange(() => Distributors);
      }
    }

    public ConsumerViewModel SelectedConsumer
    {
      get { return m_SelectedConsumer; }
      set
      {
        m_SelectedConsumer = value;
        NotifyOfPropertyChange(() => ConsumerGroups);
        NotifyOfPropertyChange(() => Distributors);
      }
    }

    public DistributorViewModel SelectedDistributor
    {
      get { return m_SelectedDistributor; }
      set
      {
        m_SelectedDistributor = value;
        NotifyOfPropertyChange(() => ConsumerGroups);
        NotifyOfPropertyChange(() => Consumers);
      }
    }

    public int Index
    {
      get { return 400; }
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
      get { return "Verbraucher"; }
    }

    public IEnumerable<ConsumerGroupViewModel> SearchInConsumerGroupList(
      IEnumerable<ConsumerGroupViewModel> filteredConsumerGroups)
    {
      if (string.IsNullOrEmpty(SearchConsumerGroupsText))
      {
        return filteredConsumerGroups;
      }
      var searchText = SearchConsumerGroupsText.ToLower();

      return filteredConsumerGroups.Where(c => (((c.GroupName != null) && (c.GroupName.ToLower()
                                                                            .Contains(searchText)))));
    }

    public IEnumerable<ConsumerViewModel> SearchInConsumerList(IEnumerable<ConsumerViewModel> filteredConsumers)
    {
      if (string.IsNullOrEmpty(SearchConsumerText))
      {
        return filteredConsumers;
      }
      var searchText = SearchConsumerText.ToLower();

      return filteredConsumers.Where(c => (((c.Name != null) && (c.Name.ToLower()
                                                                  .Contains(searchText)) ||
                                            (c.Manufacturer != null) && (c.Manufacturer.ToLower()
                                                                          .Contains(searchText)))));
    }

    public IEnumerable<DistributorViewModel> SearchInDistributorList(IEnumerable<DistributorViewModel> filteredDistributors)
    {
      if (string.IsNullOrEmpty(SearchDistributorText))
      {
        return filteredDistributors;
      }
      var searchText = SearchDistributorText.ToLower();

      return filteredDistributors.Where(c => (((c.Name != null) && (c.Name.ToLower()
                                                                     .Contains(searchText)))));
    }

    private void Reload()
    {
      IsEnabled = m_Repository.HasConnection;
      if (IsEnabled)
      {
        LoadData();
      }
    }

    private void LoadData()
    {
      LoadConsumerGroups();
      LoadConsumers();
      LoadDistributors();
    }

    private void LoadConsumerGroups()
    {
      m_ConsumerGroups.Clear();
      m_Repository.ConsumerGroups.CollectionChanged += AlterConsumerGroupCollection;
      foreach (var consumerGroup in m_Repository.ConsumerGroups)
      {
        m_ConsumerGroups.Add(CreateConsumerGroupViewModel(consumerGroup));
      }
    }

    private void LoadConsumers()
    {
      m_Consumers.Clear();
      m_Repository.Consumers.CollectionChanged += AlterConsumerCollection;
      foreach (var consumer in m_Repository.Consumers)
      {
        m_Consumers.Add(CreateConsumerViewModel(consumer));
      }
    }

    private void LoadDistributors()
    {
      m_Distributors.Clear();
      m_Repository.Distributors.CollectionChanged += AlterDistributorCollection;
      foreach (var distributor in m_Repository.Distributors)
      {
        m_Distributors.Add(CreateDistributorViewModel(distributor));
      }
    }

    private ConsumerGroupViewModel CreateConsumerGroupViewModel(ConsumerGroup consumerGroup)
    {
      return m_EnergyViewModelFactory.CreateFromExisting(consumerGroup);
    }

    private ConsumerViewModel CreateConsumerViewModel(Consumer consumer)
    {
      return (m_EnergyViewModelFactory.CreateFromExisting(consumer));
    }

    private DistributorViewModel CreateDistributorViewModel(Distributor distributor)
    {
      return (m_EnergyViewModelFactory.CreateFromExisting(distributor));
    }

    private void AlterConsumerGroupCollection(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
      switch (eventArgs.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var newConsumerGroup in eventArgs.NewItems.OfType<ConsumerGroup>())
          {
            m_ConsumerGroups.Add(CreateConsumerGroupViewModel(newConsumerGroup));
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var oldConsumerGroup in
            eventArgs.OldItems.OfType<ConsumerGroup>()
                     .Select(oldConsumerGroup => m_ConsumerGroups.Single(cg => cg.Model == oldConsumerGroup)))
          {
            m_ConsumerGroups.Remove(oldConsumerGroup);
          }
          break;
      }
    }

    private void AlterConsumerCollection(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
      switch (eventArgs.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var consumer in eventArgs.NewItems.OfType<Consumer>())
          {
            m_Consumers.Add(CreateConsumerViewModel(consumer));
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var oldConsumer in
            eventArgs.OldItems.OfType<Consumer>()
                     .Select(oldConsumer => m_Consumers.Single(c => c.Model == oldConsumer)))
          {
            m_Consumers.Remove(oldConsumer);
          }
          break;
      }
    }

    private void AlterDistributorCollection(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
      switch (eventArgs.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var distributor in eventArgs.NewItems.OfType<Distributor>())
          {
            m_Distributors.Add(CreateDistributorViewModel(distributor));
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var oldDistributor in
            eventArgs.OldItems.OfType<Distributor>()
                     .Select(oldDistributor => m_Distributors.Single(c => c.Model == oldDistributor)))
          {
            m_Distributors.Remove(oldDistributor);
          }
          break;
      }
    }

    public void OpenEditConsumerGroupDialog(object dataContext)
    {
      OpenEditor(m_EnergyViewModelFactory.CreateConsumerGroupModifyVM(((ConsumerGroupViewModel) dataContext).Model));
    }

    public void OpenEditConsumerDialog(object dataContext)
    {
      OpenEditor(m_EnergyViewModelFactory.CreateConsumerModifyVM(((ConsumerViewModel) dataContext).Model));
    }

    public void OpenEditDistributorDialog(object dataContext)
    {
      OpenEditor(m_EnergyViewModelFactory.CreateDistributorModifyVM(((DistributorViewModel) dataContext).Model));
    }

    private void OpenEditor(object dataContext)
    {
      m_EditItem = (IScreen) dataContext;
      Dialogs.ShowDialog(m_EditItem);
    }

    private void DeleteConsumerGroup(ConsumerGroup dataContext)
    {
      if (ObjectHasConstraints(dataContext))
      {
        ShowDeletionFailureDialog(dataContext);
      }
      else
      {
        m_Repository.ConsumerGroups.Remove((dataContext));
        m_Repository.Save();

        NotifyOfPropertyChange(() => ConsumerGroups);
      }
    }

    private void DeleteConsumer(Consumer dataContext)
    {
      if (ObjectHasConstraints(dataContext))
      {
        ShowDeletionFailureDialog(dataContext);
      }
      else
      {
        m_Repository.Consumers.Remove(dataContext);
        m_Repository.Save();
        NotifyOfPropertyChange(() => Consumers);
      }
    }

    private void ShowDeletionFailureDialog<T>(T dataContext)
    {
      var constraint = m_Repository.Links.Where(c => c.Target != null && c.Target.Equals(dataContext));
      Dialogs.ShowMessageBox(
        "Das Objekt kann nicht gelöscht werden, da folgendes andere Objekt abhängig ist. " +
        String.Join(" ", (constraint.Select(c => (c.Source)))), "Datenbankfehler");
    }

    private void DeleteDistributor(Distributor dataContext)
    {
      if (ObjectHasConstraints(dataContext))
      {
        ShowDeletionFailureDialog(dataContext);
      }
      else
      {
        m_Repository.Distributors.Remove(dataContext);
        m_Repository.Save();

        NotifyOfPropertyChange(() => Distributors);
      }
    }

    private bool ObjectHasConstraints(object dataContext)
    {
      return m_Repository.Links.Any(o => o.Target == dataContext);
    }

    private void CloseEditor()
    {
      if (m_EditItem != null)
      {
        m_EditItem.TryClose();
      }
      m_EditItem = null;
    }

    public void Save(object dataContext)
    {
      CloseEditor();
      m_Repository.Save();
      NotifyOfPropertyChange(() => Consumers);
      NotifyOfPropertyChange(() => ConsumerGroups);
      NotifyOfPropertyChange(() => Distributors);
    }

    public void Cancel(object dataContext)
    {
      CloseEditor();
    }

    public void Delete(object dataContext)
    {
      CloseEditor();
      try
      {
        var deletionDictionary = new Dictionary<Type, Action>()
        {
          {
            typeof (DistributorModifyViewModel), () => DeleteDistributor(((DistributorModifyViewModel) dataContext).Model)
          },
          {
            typeof (ConsumerGroupModifyViewModel), () => DeleteConsumerGroup(((ConsumerGroupModifyViewModel) dataContext).Model)
          },
          {
            typeof (ConsumerModifyViewModel), () => DeleteConsumer(((ConsumerModifyViewModel) dataContext).Model)
          },
          {
            typeof (DistributorViewModel), () => DeleteDistributor(((DistributorViewModel) dataContext).Model)
          },
          {
            typeof (ConsumerGroupViewModel), () => DeleteConsumerGroup(((ConsumerGroupViewModel) dataContext).Model)
          },
          {
            typeof (ConsumerViewModel), () => DeleteConsumer(((ConsumerViewModel) dataContext).Model)
          }
        };
        deletionDictionary[dataContext.GetType()]();
      }
      catch (KeyNotFoundException ex)
      {
        Console.WriteLine(ex);
      }

      // UpdateView();
      NotifyOfPropertyChange(() => Consumers);
      NotifyOfPropertyChange(() => ConsumerGroups);
      NotifyOfPropertyChange(() => Distributors);
    }

    public void AddNewConsumerGroup()
    {
      m_Repository.ConsumerGroups.Add(ModelFactory.CreateConsumerGroup(NewConsumerGroupName));
      m_Repository.Save();
      NewConsumerGroupName = String.Empty;
      //TODO maybe select last Consumer Group

      NotifyOfPropertyChange(() => ConsumerGroups);
      NotifyOfPropertyChange(() => NewConsumerGroupName);
    }

    public void AddNewConsumer()
    {

      if (!m_Distributors.Any() ||
          !m_ConsumerGroups.Any())
      {
        
        Dialogs.ShowMessageBox("Bitte legen Sie zunächst Verteiler und Verbrauchergruppen an.", "Unvollständige Auswahl");
      }
      else
      {
        m_Repository.Consumers.Add(ModelFactory.CreateConsumer(NewConsumerName, m_Distributors.First().Model,
          m_ConsumerGroups.First().Model));
        m_Repository.Save();
        NewConsumerName = String.Empty;
        NotifyOfPropertyChange(() => Consumers);
        NotifyOfPropertyChange(() => ConsumerGroups);
        NotifyOfPropertyChange(() => Distributors);
        NotifyOfPropertyChange(() => NewConsumerName);
      }
    }

    public void AddNewDistributor()
    {
      m_Repository.Distributors.Add(ModelFactory.CreateDistributor(NewDistributorName));
      m_Repository.Save();

      NewDistributorName = String.Empty;
      //TODO maybe select last Consumer Group

      NotifyOfPropertyChange(() => Distributors);
      NotifyOfPropertyChange(() => NewDistributorName);
    }
  }
}