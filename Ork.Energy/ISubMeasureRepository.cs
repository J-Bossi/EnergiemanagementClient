using System;
using System.Data.Services.Client;
using Ork.Energy.DomainModelService;

namespace Ork.Energy
{
    public interface ISubMeasureRepository
    {
        bool HasConnection { get; }
        DataServiceCollection<ResponsibleSubject> ResponsibleSubjects { get; }
        DataServiceCollection<SubMeasure> SubMeasures { get; }
        void Save();
        event EventHandler ContextChanged;
        event EventHandler SaveCompleted;
    }
}
