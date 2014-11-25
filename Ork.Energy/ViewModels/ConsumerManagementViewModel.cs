﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Ork.Energy.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
    [Export(typeof (IWorkspace))]
    public class ConsumerManagementViewModel : DocumentBase, IWorkspace
    {
        private readonly BindableCollection<ConsumerGroupViewModel> m_ConsumerGroups =
            new BindableCollection<ConsumerGroupViewModel>();

        private readonly IConsumerViewModelFactory m_ConsumerViewModelFactory;
        private readonly BindableCollection<ConsumerViewModel> m_Consumers = new BindableCollection<ConsumerViewModel>();

        private readonly BindableCollection<DistributorViewModel> m_Distributors =
            new BindableCollection<DistributorViewModel>();

        private readonly IConsumerRepository m_Repository;
        private IScreen m_EditItem;
        private bool m_FlyoutActivated;
        private bool m_IsEnabled;
        private string m_SearchConsumerGroupText;
        private string m_SearchConsumerText;
        private string m_SearchDistributorText;

        [ImportingConstructor]
        public ConsumerManagementViewModel([Import] IConsumerRepository mRepository,
            [Import] IConsumerViewModelFactory mConsumerViewModelFactory, [Import] IDialogManager dialogs)
        {
            Dialogs = dialogs;
            m_Repository = mRepository;
            m_ConsumerViewModelFactory = mConsumerViewModelFactory;


            m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);
            Reload();
        }

        public new IDialogManager Dialogs { get; private set; }

        public IEnumerable<ConsumerGroupViewModel> ConsumerGroups
        {
            get { return FilteredConsumerGroups; }
        }

        public IEnumerable<ConsumerViewModel> Consumers
        {
            get { return FilteredConsumers; }
        }

        public IEnumerable<DistributorViewModel> Distributors
        {
            get { return FilteredDistributors; }
        } 

        private IEnumerable<ConsumerGroupViewModel> FilteredConsumerGroups
        {
            get
            {
                ConsumerGroupViewModel[] filteredConsumerGroups = SearchInConsumerGroupList()
                    .ToArray();
                return filteredConsumerGroups;
            }
        }

        private IEnumerable<ConsumerViewModel> FilteredConsumers
        {
            get
            {
                ConsumerViewModel[] filteredConsumers = SearchInConsumerList()
                    .ToArray();
                return filteredConsumers;
            }
        }

        private IEnumerable<DistributorViewModel> FilteredDistributors
        {
            get
            {
                DistributorViewModel[] filteredDistributors = SearchInDistributorList()
                    .ToArray();
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
                NotifyOfPropertyChange(() => Consumers);
                NotifyOfPropertyChange(() => Distributors);
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
        public ConsumerGroupViewModel SelectedConsumerGroup { get; set; }

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
            string searchText = SearchConsumerGroupsText.ToLower();


            return m_ConsumerGroups.Where(c => (((c.GroupName != null) && (c.GroupName.ToLower()
                .Contains(searchText)) || (c.GroupDescription != null) && (c.GroupDescription.ToLower()
                    .Contains(searchText)))));
        }

        public IEnumerable<ConsumerViewModel> SearchInConsumerList()
        {
            if (string.IsNullOrEmpty(SearchConsumerText))
            {
                return m_Consumers;
            }
            string searchText = SearchConsumerText.ToLower();


            return m_Consumers.Where(c => (((c.Name != null) && (c.Name.ToLower()
                .Contains(searchText)) || (c.Manufacturer != null) && (c.Manufacturer.ToLower()
                    .Contains(searchText)))));
        }

        public IEnumerable<DistributorViewModel> SearchInDistributorList()
        {
            if (string.IsNullOrEmpty(SearchDistributorText))
            {
                return m_Distributors;
            }
            string searchText = SearchDistributorText.ToLower();


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
            foreach (ConsumerGroup consumerGroup in m_Repository.ConsumerGroups)
            {
                CreateConsumerGroupViewModel(consumerGroup);
            }
        }

        private void LoadConsumers()
        {
            m_Consumers.Clear();
            m_Repository.Consumers.CollectionChanged += AlterConsumerCollection;
            foreach (Consumer consumer in m_Repository.Consumers)
            {
                CreateConsumerViewModel(consumer);
            }
        }

        private void LoadDistributors()
        {
            m_Distributors.Clear();
            m_Repository.Distributors.CollectionChanged += AlterDistributorCollection;
            foreach (Distributor distributor in m_Repository.Distributors)
            {
                CreateDistributorViewModel(distributor);
            }
        }

        private void CreateConsumerGroupViewModel(ConsumerGroup consumerGroup)
        {
            m_ConsumerGroups.Add(m_ConsumerViewModelFactory.CreateFromExisting(consumerGroup));
            
        }

        private void CreateConsumerViewModel(Consumer consumer)
        {
            m_Consumers.Add(m_ConsumerViewModelFactory.CreateFromExisting(consumer));

        }

        private void CreateDistributorViewModel(Distributor distributor)
        {
            m_Distributors.Add(m_ConsumerViewModelFactory.CreateFromExisting(distributor));

        }

        private void AlterConsumerGroupCollection(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ConsumerGroup newConsumerGroup in eventArgs.NewItems.OfType<ConsumerGroup>())
                    {
                        CreateConsumerGroupViewModel(newConsumerGroup);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (
                        ConsumerGroupViewModel oldConsumerGroup in
                            eventArgs.OldItems.OfType<ConsumerGroup>()
                                .Select(oldConsumerGroup => m_ConsumerGroups.Single(cg => cg.Model == oldConsumerGroup))
                        )
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
                    foreach (Consumer consumer in eventArgs.NewItems.OfType<Consumer>())
                    {
                        CreateConsumerViewModel(consumer);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ConsumerViewModel oldConsumer in eventArgs.OldItems.OfType<Consumer>().Select(oldConsumer => m_Consumers.Single(c => c.Model == oldConsumer)))
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
                    foreach (Distributor distributor in eventArgs.NewItems.OfType<Distributor>())
                    {
                        CreateDistributorViewModel(distributor);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (DistributorViewModel oldDistributor in eventArgs.OldItems.OfType<Distributor>().Select(oldDistributor => m_Distributors.Single(c => c.Model == oldDistributor)))
                    {
                        m_Distributors.Remove(oldDistributor);
                    }
                    break;
            }
        }

        public void OpenEditConsumerGroupDialog(object dataContext)
        {
            SelectedConsumerGroup = (ConsumerGroupViewModel) dataContext;
            OpenEditor(m_ConsumerViewModelFactory.CreateConsumerGroupModifyVM(SelectedConsumerGroup.Model));
        }

        private void OpenEditor(object dataContext)
        {
            m_EditItem = (IScreen) dataContext;
            Dialogs.ShowDialog(m_EditItem);
        }

        public void DeleteConsumerGroup(object dataContext)
        {
            //TODO Delete ConsumerGroup and Check on Childs
            m_Repository.ConsumerGroups.Remove(((ConsumerGroupViewModel) dataContext).Model);
            m_Repository.Save();

            NotifyOfPropertyChange(() => ConsumerGroups);
        }

        public void SaveConsumerGroup(object dataContext)
        {
            Save();
            NotifyOfPropertyChange(() => ConsumerGroups);
        }

        private void CloseEditor()
        {
            m_EditItem.TryClose();
            m_EditItem = null;
        }

        private void Save()
        {
            CloseEditor();
            m_Repository.Save();
        }

        public void AddNewConsumerGroup()
        {
            m_Repository.ConsumerGroups.Add(ModelFactory.CreateConsumerGroup(NewConsumerGroupName));
            m_Repository.Save();

            //TODO maybe select last Consumer Group

            //LoadData();
            NotifyOfPropertyChange(() => ConsumerGroups);
        }
    }
}