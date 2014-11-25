using System;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using Caliburn.Micro;
using Ork.Energy.DomainModelService;

using Ork.Setting;

namespace Ork.Energy
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
        public DataServiceCollection<Consumer> Consumers { get; private set; }
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
                LoadConsumers();
                LoadDistributors();
                LoadReadings();
                HasConnection = true;
            }
            catch (Exception ex)
            {
                HasConnection = false;
                var message = ex.Message;
                message += message + Environment.NewLine + ex.InnerException.Message;

                
            }
            RaiseEvent(ContextChanged);

          


        }




        private void LoadConsumerGroups()
        {
            ConsumerGroups = new DataServiceCollection<ConsumerGroup>(m_Context);

            DataServiceQuery<ConsumerGroup> query = m_Context.ConsumerGroups;
            ConsumerGroups.Load(query);
        }

        private void LoadConsumers()
        {
            Consumers = new DataServiceCollection<Consumer>(m_Context);

            DataServiceQuery<Consumer> query = m_Context.Consumers;
            Consumers.Load(query);
        }


        private void LoadReadings()
        {
            Readings = new DataServiceCollection<Reading>(m_Context);

            DataServiceQuery<Reading> query = m_Context.Readings;
            Readings.Load(query);
        }

        private void LoadDistributors()
        {
            Distributors = new DataServiceCollection<Distributor>(m_Context);

            DataServiceQuery<Distributor> query = m_Context.Distributors;
            Distributors.Load(query);
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