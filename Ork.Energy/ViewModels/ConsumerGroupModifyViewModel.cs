using System.Collections.Generic;
using System.Data.Services.Client;
using Caliburn.Micro;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
    public class ConsumerGroupModifyViewModel : Screen
    {
        private readonly ConsumerGroup m_Model;

        public ConsumerGroupModifyViewModel(ConsumerGroup model)
        {
            DisplayName = "Verbrauchergruppe bearbeiten...";
            m_Model = model;
        }

        public string GroupName
        {
            get { return m_Model.GroupName; }
            set { m_Model.GroupName = value; }
        }

        public DataServiceCollection<ConsumerType> ConsumerTypes
        {
            get { return m_Model.ConsumerTypes; }
            set { m_Model.ConsumerTypes = value; }
        }
    }
}