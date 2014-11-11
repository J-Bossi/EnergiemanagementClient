using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ork.Energy.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
    public class MeasurePrintPreviewViewModel : MeasureEditViewModel
    {
        [ImportingConstructor]
        public MeasurePrintPreviewViewModel(DomainModelService.Measure model, Action removeMeasureAction, ResponsibleSubjectViewModel[] responsibleSubjectViewModels,
     [Import] IMeasureRepository measureRepository, [Import] ISubMeasureViewModelFactory subMeasureViewModelFactory, IEnumerable<Catalog> catalogs)
      : base(model, removeMeasureAction, responsibleSubjectViewModels,  measureRepository, subMeasureViewModelFactory, catalogs)
    {
      DisplayName = TranslationProvider.Translate("TitleMeasurePrintPreviewViewModel");
      //m_Stati = Enum.GetValues(typeof (Status));
     
      SelectedResponsibleSubject = responsibleSubjectViewModels.Single(rsvm => model.ResponsibleSubject == rsvm.Model);
   
      
    }


    }
}
