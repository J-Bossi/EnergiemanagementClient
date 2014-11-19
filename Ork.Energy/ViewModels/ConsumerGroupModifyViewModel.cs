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

        public string GroupDescription
        {
            get { return m_Model.GroupDescription; }
            set { m_Model.GroupDescription = value; }
        }
    }
}