using System.Collections.Generic;
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
        private readonly IConsumerRepository m_Repository;

        private bool m_FlyoutActivated;
        private bool m_IsEnabled;

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

        public ICollection<ConsumerGroupViewModel> AllConsumerGroups
        {
            get { return m_ConsumerGroups; }
        }

        public string NewConsumerGroupName { get; set; }

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

        private void CreateConsumerGroupViewModel(ConsumerGroup consumerGroup)
        {
            m_ConsumerGroups.Add(m_ConsumerViewModelFactory.CreateFromExisting(consumerGroup));
            //Todo: Create Childs (i.e. Consumers)
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

        public void OpenEditConsumerGroupDialog(object dataContext)
        {
            //TODO Edit it
        }

        public void DeleteConsumerGroup(object dataContext)
        {
            //TODO Delete ConsumerGroup and Check on Childs
        }

        public void AddNewConsumerGroup()
        {
            m_Repository.ConsumerGroups.Add(ModelFactory.CreateConsumerGroup(NewConsumerGroupName));
            m_Repository.Save();

            //TODO maybe select last Consumer Group

            //LoadData();
            NotifyOfPropertyChange(() => AllConsumerGroups);
        }
    }
}