using Caliburn.Micro;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;

namespace Ork.Energy.ViewModels
{
    public class SubMeasureViewModel : PropertyChangedBase
    {
         private readonly SubMeasure m_Model;
        private IResponsibleSubjectViewModelFactory m_ResponsibleSubjectViewModelFactory;


        public SubMeasureViewModel(SubMeasure submeasure, IResponsibleSubjectViewModelFactory responsibleSubjectViewModelFactory)
    {
      m_Model = submeasure;
        m_ResponsibleSubjectViewModelFactory = responsibleSubjectViewModelFactory;
            ResponsibleSubjectViewModel = responsibleSubjectViewModelFactory.CreateFromExisting(Model.ResponsibleSubject);
    }


    public int Id
    {
        get { return m_Model.Id; }
    }

    public string Name
    {
      get { return m_Model.Name; }
    }

    public SubMeasure Model
    {
      get { return m_Model; }
    }

    public bool IsCompleted
    {
        get { return m_Model.IsCompleted; }
        set
        {
            m_Model.IsCompleted = value;
            NotifyOfPropertyChange(()=>IsCompleted);
        }
    }

    public Measure RelatedMeasure
    {
        get { return m_Model.ReleatedMeasure; }
    }

        public ResponsibleSubjectViewModel ResponsibleSubjectViewModel { get; private set; }
    
    

    }
}
