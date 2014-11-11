using System.ComponentModel.Composition;
using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
    [Export(typeof(ISubMeasureViewModelFactory))]
    class SubMeasureViewModelFactory:ISubMeasureViewModelFactory
    {
        private readonly IResponsibleSubjectViewModelFactory m_ResponsibleSubjectViewModelFactory;
        [ImportingConstructor]
        public SubMeasureViewModelFactory([Import]IResponsibleSubjectViewModelFactory responsibleSubjectViewModelFactory)
        {
            m_ResponsibleSubjectViewModelFactory = responsibleSubjectViewModelFactory;
         
        }

        public SubMeasureViewModel CreateFromExisting(DomainModelService.SubMeasure subMeasure)
        {
            return new SubMeasureViewModel(subMeasure, m_ResponsibleSubjectViewModelFactory);
        }
    }
}
