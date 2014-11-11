using System.ComponentModel.Composition;
using System.Windows;
using Ork.Energy.Factories;
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
    [Export(typeof (IWorkspace))]
    public class ConsumerManagementViewModel : DocumentBase, IWorkspace
    {
        private readonly IConsumerViewModelFactory m_ConsumerViewModelFactory;
        private readonly IConsumerRepository m_Repository;
        private bool m_IsEnabled;
        private bool m_FlyoutActivated;

        [ImportingConstructor]
        public ConsumerManagementViewModel([Import] IConsumerRepository mRepository,
            [Import] IConsumerViewModelFactory mConsumerViewModelFactory)
        {
            m_Repository = mRepository;
            m_ConsumerViewModelFactory = mConsumerViewModelFactory;
            m_Repository.ContextChanged += (s, e) => Application.Current.Dispatcher.Invoke(Reload);
            Reload();
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

        private void Reload()
        {
            IsEnabled = m_Repository.HasConnection;
            if (IsEnabled)
            {
                //TODO Load Data method, Show all Data method Notify of Property Chanfe
            }
        }

        public bool FlyoutActivated
        {
            get { return m_FlyoutActivated; }
            set
            {
                if (m_FlyoutActivated == value)
                {
                    return;
                }
                m_FlyoutActivated = value;
                NotifyOfPropertyChange(() => FlyoutActivated);
            }
        }
    }
}