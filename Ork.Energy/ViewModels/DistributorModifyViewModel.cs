using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Ork.Energy.DomainModelService;

namespace Ork.Energy.ViewModels
{
    public class DistributorModifyViewModel : Screen
    {
        private readonly Distributor m_Model;

        public DistributorModifyViewModel(Distributor model)
        {
            DisplayName = "Verteiler bearbeiten...";
            m_Model = model;
        }

        public string Name
        {
            get { return m_Model.Name; }
            set { m_Model.Name = value; }
        }

        public bool IsMainDistributor
        {
            get { return m_Model.IsMainDistributor; }
            set { m_Model.IsMainDistributor = value; }
        }

        public DataServiceCollection<Reading> Readings
        {
            get { return m_Model.Readings; }
            set { m_Model.Readings = value; }
        } 
    }
}
