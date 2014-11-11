using Ork.Energy.ViewModels;

namespace Ork.Energy.Factories
{
    public interface ISubMeasureViewModelFactory
    {
       SubMeasureViewModel CreateFromExisting(DomainModelService.SubMeasure subMeasure);
    }
}
