using System;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using Ork.Energy.DomainModelService;
using Ork.Setting;

namespace Ork.Energy
{
    [Export(typeof (ISubMeasureRepository))]
    class SubMeasureRepository : ISubMeasureRepository
    {
    private readonly Func<DomainModelContext> m_CreateMethod;
    private DomainModelContext m_Context;

    [ImportingConstructor]
    public SubMeasureRepository([Import] ISettingsProvider settingsContainer, [Import] Func<DomainModelContext> createMethod)
    {
      m_CreateMethod = createMethod;
      settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }

    public bool HasConnection { get; private set; }
    public DataServiceCollection<ResponsibleSubject> ResponsibleSubjects { get; private set; }
    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;


    public void Save()
    {
      if (m_Context.ApplyingChanges)
      {
        return;
      }

      var result = m_Context.BeginSaveChanges(SaveChangesOptions.Batch, r =>
                                                                        {
                                                                          var dm = (DomainModelContext) r.AsyncState;
                                                                          dm.EndSaveChanges(r);
                                                                          RaiseEvent(SaveCompleted);
                                                                        }, m_Context);
    }

    private void Initialize()
    {
      m_Context = m_CreateMethod();

      try
      {
          LoadResponsibleSubjects();
          LoadSubMeasures();
        HasConnection = true;
      }
      catch (Exception)
      {
        HasConnection = false;
      }

      RaiseEvent(ContextChanged);
    }

    private void LoadResponsibleSubjects()
    {
      ResponsibleSubjects = new DataServiceCollection<ResponsibleSubject>(m_Context);

      var query = m_Context.ResponsibleSubjects.Expand("OpenResKit.DomainModel.Employee/Groups");
      ResponsibleSubjects.Load(query);
    }

    private void LoadSubMeasures()
    {
        SubMeasures = new DataServiceCollection<SubMeasure>(m_Context);

        var query = m_Context.SubMeasures.Expand("OpenResKit.DomainModel.SubMeasure/ReleatedMeasure").Expand("OpenResKit.DomainModel.SubMeasure/ResponsibleSubject");
        SubMeasures.Load(query);
    }

    private void RaiseEvent(EventHandler eventHandler)
    {
      if (eventHandler != null)
      {
        eventHandler(this, new EventArgs());
      }
    }

    public DataServiceCollection<SubMeasure> SubMeasures { get; private set; }
    }
}
