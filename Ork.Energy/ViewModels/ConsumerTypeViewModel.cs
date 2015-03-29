using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ork.Energy.Domain.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public class ConsumerTypeViewModel
  {
    private readonly ConsumerType m_Model;

    public ConsumerTypeViewModel(ConsumerType model)
    {
      m_Model = model;
    }

    public ConsumerType Model
    {
      get { return m_Model; }
    }

    public string TypeName
    {
      get { return m_Model.TypeName; }
      set { m_Model.TypeName = value; }
    }
  }
}
