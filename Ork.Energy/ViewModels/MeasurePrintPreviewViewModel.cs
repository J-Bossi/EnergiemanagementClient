using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ork.Energy.Domain.DomainModelService;
using Ork.Energy.Factories;
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
    public class MeasurePrintPreviewViewModel : MeasureEditViewModel
    {
        [ImportingConstructor]
        public MeasurePrintPreviewViewModel(EnergyMeasure model, Action removeMeasureAction, ResponsibleSubjectViewModel[] responsibleSubjectViewModels,
     [Import] IEnergyRepository energyRepository, [Import] IViewModelFactory subMeasureViewModelFactory)
      : base(model, removeMeasureAction, responsibleSubjectViewModels,  energyRepository, subMeasureViewModelFactory)
    {  
      SelectedResponsibleSubject = responsibleSubjectViewModels.Single(rsvm => model.ResponsibleSubject == rsvm.Model);

    }


    }
}
