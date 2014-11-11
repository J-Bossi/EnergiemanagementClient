using System;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using Ork.Consumer.DomainModelService;
using Ork.Setting;

namespace Ork.Consumer
{
    [Export(typeof (IConsumerRepository))]
    internal class ConsumerRepository : IConsumerRepository
    {
        private readonly Func<DomainModelContext> m_CreateMethod;
        private DomainModelContext m_Context;

        [ImportingConstructor]
        public ConsumerRepository([Import] ISettingsProvider settingsContainer,
            [Import] Func<DomainModelContext> createMethod)
        {
            m_CreateMethod = createMethod;
            settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();
            Initialize();
        }


        public DataServiceCollection<ConsumerGroup> ConsumerGroups { get; private set; }
        public DataServiceCollection<Building> Buildings { get; private set; }
        public DataServiceCollection<Distributor> Distributors { get; private set; }
        public DataServiceCollection<Reading> Readings { get; private set; }
        public bool HasConnection { get; private set; }

        public void Save()
        {
            if (m_Context.ApplyingChanges)
            {
                return;
            }

            IAsyncResult result = m_Context.BeginSaveChanges(SaveChangesOptions.Batch, c =>
            {
                var dmc = (DomainModelContext) c.AsyncState;
                dmc.EndSaveChanges(c);
                RaiseEvent(SaveCompleted);
            }, m_Context);
        }


        public event EventHandler ContextChanged;
        public event EventHandler SaveCompleted;

        private void Initialize()
        {
            m_Context = m_CreateMethod();

            try
            {
                LoadConsumerGroups();
                LoadBuildings();
                LoadDistributors();
                LoadReadings();
                HasConnection = true;
            }
            catch (Exception)
            {
                HasConnection = false;
            }
        }

        private void LoadConsumerGroups()
        {
            ConsumerGroups = new DataServiceCollection<ConsumerGroup>(m_Context);

            DataServiceQuery<ConsumerGroup> query = m_Context.ConsumerGroups.Expand("OpenResKit.DomainModel.Consumers");
        }

        private void LoadReadings()
        {
            Readings = new DataServiceCollection<Reading>(m_Context);

            DataServiceQuery<Reading> query = m_Context.Readings;
        }

        private void LoadDistributors()
        {
            Distributors = new DataServiceCollection<Distributor>(m_Context);

            DataServiceQuery<Distributor> query = m_Context.Distributors;
        }

        private void LoadBuildings()
        {
            Buildings = new DataServiceCollection<Building>(m_Context);

            DataServiceQuery<Building> query = m_Context.Buildings.Expand("OpenResKit.DomainModel.Rooms");
        }

        private void RaiseEvent(EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgs());
            }
        }
    }
}