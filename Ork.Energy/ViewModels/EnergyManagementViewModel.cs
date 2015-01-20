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
using Ork.Energy.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;
using Action = System.Action;

namespace Ork.Energy.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class EnergyManagementViewModel : DocumentBase, IWorkspace
  {
    private ConsumerGroupViewModel m_ConsumerGroup;
    private DistributorViewModel m_Distributor;
    private IScreen m_EditItem;
    private bool m_FlyoutActivated;
    private bool m_IsEnabled;
    private string m_SearchConsumerGroupText;
    private string m_SearchConsumerText;
    private string m_SearchDistributorText;

    private readonly BindableCollection<ConsumerGroupViewModel> m_ConsumerGroups =
      new BindableCollection<ConsumerGroupViewModel>();

    private readonly BindableCollection<ConsumerViewModel> m_Consumers = new BindableCollection<ConsumerViewModel>();
    private readonly BindableCollection<DistributorViewModel> m_Distributors = new BindableCollection<DistributorViewModel>();
    private readonly IEnergyViewModelFactory m_EnergyViewModelFactory;
    private readonly IEnergyRepository m_Repository;

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
      get { return FilteredConsumerGroups.OrderBy(cg => cg.GroupName); }
    }

    public IEnumerable<ConsumerViewModel> Consumers
    {
      get { return FilteredConsumers.OrderBy(c => c.Name); }
    }

    public IEnumerable<DistributorViewModel> Distributors
    {
      get { return FilteredDistributors.OrderBy(d => d.Name); }
    }

    private IEnumerable<ConsumerGroupViewModel> FilteredConsumerGroups
    {
      get
      {
        var filteredConsumerGroups = SearchInConsumerGroupList();
        return filteredConsumerGroups;
      }
    }

    private IEnumerable<ConsumerViewModel> FilteredConsumers
    {
      //Filters after SearchText and the available objects in other List. Only related Objects are shown
      get
      {
        var filteredConsumers = SearchInConsumerList()
          .Where(c => FilteredDistributors.Select(d => d.Model)
                                          .Contains(c.Model.Distributor) && (FilteredConsumerGroups.Select(cg => cg.Model)
                                                                                                   .Contains(
                                                                                                     c.Model.ConsumerGroup)));

        return filteredConsumers;
      }
    }

    private IEnumerable<DistributorViewModel> FilteredDistributors
    {
      get
      {
        var filteredDistributors = SearchInDistributorList()
          //.Where(d => FilteredConsumers.Select(c => c.Model.Distributor).Contains(d.Model));
          ;
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
        NotifyOfPropertyChange(() => Distributors);
        NotifyOfPropertyChange(() => Consumers);
      }
    }

    public string SearchConsumerText
    {
      get { return m_SearchConsumerText; }
      set
      {
        m_SearchConsumerText = value;
        NotifyOfPropertyChange(() => ConsumerGroups);
        NotifyOfPropertyChange(() => Consumers);
        NotifyOfPropertyChange(() => Distributors);
      }
    }

    public string SearchDistributorText
    {
      get { return m_SearchDistributorText; }
      set
      {
        m_SearchDistributorText = value;
        NotifyOfPropertyChange(() => ConsumerGroups);
        NotifyOfPropertyChange(() => Consumers);
        NotifyOfPropertyChange(() => Distributors);
      }
    }

    public string NewConsumerGroupName { get; set; }
    public string NewConsumerName { get; set; }
    public string NewDistributorName { get; set; }

    public ConsumerGroupViewModel SelectedConsumerGroup
    {
      get { return m_ConsumerGroup; }
      set
      {
        m_ConsumerGroup = value;
        NotifyOfPropertyChange(() => CanAddConsumer);
      }
    }

    public ConsumerViewModel SelectedConsumer { get; set; }

    public DistributorViewModel SelectedDistributor
    {
      get { return m_Distributor; }
      set
      {
        m_Distributor = value;
        NotifyOfPropertyChange(() => CanAddConsumer);
      }
    }

    public bool CanAddConsumer
    {
      get
      {
        if (SelectedConsumerGroup == null ||
            SelectedDistributor == null)
        {
          return false;
        }
        return true;
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

    public IEnumerable<ConsumerGroupViewModel> SearchInConsumerGroupList()
    {
      if (string.IsNullOrEmpty(SearchConsumerGroupsText))
      {
        return m_ConsumerGroups;
      }
      var searchText = SearchConsumerGroupsText.ToLower();

      return m_ConsumerGroups.Where(c => (((c.GroupName != null) && (c.GroupName.ToLower()
                                                                      .Contains(searchText)))));
    }

    public IEnumerable<ConsumerViewModel> SearchInConsumerList()
    {
      if (string.IsNullOrEmpty(SearchConsumerText))
      {
        return m_Consumers;
      }
      var searchText = SearchConsumerText.ToLower();

      return m_Consumers.Where(c => (((c.Name != null) && (c.Name.ToLower()
                                                            .Contains(searchText)) ||
                                      (c.Manufacturer != null) && (c.Manufacturer.ToLower()
                                                                    .Contains(searchText)))));
    }

    public IEnumerable<DistributorViewModel> SearchInDistributorList()
    {
      if (string.IsNullOrEmpty(SearchDistributorText))
      {
        return m_Distributors;
      }
      var searchText = SearchDistributorText.ToLower();

      return m_Distributors.Where(c => (((c.Name != null) && (c.Name.ToLower()
                                                               .Contains(searchText)))));
    }

    private void Reload()
    {
      IsEnabled = m_Repository.HasConnection;
      if (IsEnabled)
      {
        //TODO Load Data method, Show all Data method Notify of Property Chanfe
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
        CreateConsumerGroupViewModel(consumerGroup);
      }
    }

    private void LoadConsumers()
    {
      m_Consumers.Clear();
      m_Repository.Consumers.CollectionChanged += AlterConsumerCollection;
      foreach (var consumer in m_Repository.Consumers)
      {
        CreateConsumerViewModel(consumer);
      }
    }

    private void LoadDistributors()
    {
      m_Distributors.Clear();
      m_Repository.Distributors.CollectionChanged += AlterDistributorCollection;
      foreach (var distributor in m_Repository.Distributors)
      {
        CreateDistributorViewModel(distributor);
      }
    }

    private void CreateConsumerGroupViewModel(ConsumerGroup consumerGroup)
    {
      var cgvm = m_EnergyViewModelFactory.CreateFromExisting(consumerGroup);

      m_ConsumerGroups.Add(cgvm);
    }

    private void CreateConsumerViewModel(Consumer consumer)
    {
      m_Consumers.Add(m_EnergyViewModelFactory.CreateFromExisting(consumer));
    }

    private void CreateDistributorViewModel(Distributor distributor)
    {
      m_Distributors.Add(m_EnergyViewModelFactory.CreateFromExisting(distributor));
    }

    private void AlterConsumerGroupCollection(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
      switch (eventArgs.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var newConsumerGroup in eventArgs.NewItems.OfType<ConsumerGroup>())
          {
            CreateConsumerGroupViewModel(newConsumerGroup);
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
            CreateConsumerViewModel(consumer);
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
            CreateDistributorViewModel(distributor);
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
      SelectedConsumerGroup = (ConsumerGroupViewModel) dataContext;
      OpenEditor(m_EnergyViewModelFactory.CreateConsumerGroupModifyVM(SelectedConsumerGroup.Model));
    }

    public void OpenEditConsumerDialog(object dataContext)
    {
      SelectedConsumer = (ConsumerViewModel) dataContext;
      OpenEditor(m_EnergyViewModelFactory.CreateConsumerModifyVM(SelectedConsumer.Model));
    }

    public void OpenEditDistributorDialog(object dataContext)
    {
      SelectedDistributor = (DistributorViewModel) dataContext;
      OpenEditor(m_EnergyViewModelFactory.CreateDistributorModifyVM(SelectedDistributor.Model));
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

    private void DeleteObject<T>(object dataContext, T getType)
    {
      throw new NotImplementedException();
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
      m_Repository.Consumers.Add(ModelFactory.CreateConsumer(NewConsumerName, SelectedDistributor.Model,
        SelectedConsumerGroup.Model));
      m_Repository.Save();
      NewConsumerName = String.Empty;

      //TODO maybe select last Consumer 

      //LoadData();
      NotifyOfPropertyChange(() => Consumers);
      NotifyOfPropertyChange(() => ConsumerGroups);
      NotifyOfPropertyChange(() => Distributors);
      NotifyOfPropertyChange(() => NewConsumerName);
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