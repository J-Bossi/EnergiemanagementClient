using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using Ork.Room.DomainModelService;
using Ork.Setting;

namespace Ork.Room
{
  [Export(typeof(IRoomRepository))]
  public class RoomRepository : IRoomRepository
  {

        private DomainModelContext m_Context;
    private readonly Func<DomainModelContext> m_CreateMethod;

    [ImportingConstructor]
    public RoomRepository([Import] ISettingsProvider settingsContainer, [Import] Func<DomainModelContext> createMethod)
    {
      m_CreateMethod = createMethod;
      settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }

    private void Initialize()
    {
      m_Context = m_CreateMethod();

      try
      {
        LoadBuildings();
        HasConnection = true;
      }
      catch (Exception ex)
      {
        HasConnection = false;
      }
      RaiseEvent(ContextChanged);
    }
    private void LoadBuildings()
    {
      Buildings = new DataServiceCollection<Building>(m_Context);

      DataServiceQuery<Building> query = m_Context.Buildings.Expand("OpenResKit.DomainModel.Rooms");
      Buildings.Load(query);
    }

    private void RaiseEvent(EventHandler eventHandler)
    {
      if (eventHandler != null)
      {
        eventHandler(this, new EventArgs());
      }
    }
    public DataServiceCollection<Building> Buildings { get; private set; }
    public bool HasConnection { get; private set; }
    public void Save()
    {
      if (m_Context.ApplyingChanges)
      {
        return;
      }

      IAsyncResult result = m_Context.BeginSaveChanges(SaveChangesOptions.Batch, c =>
      {
        var dmc = (DomainModelContext)c.AsyncState;
        dmc.EndSaveChanges(c);
        RaiseEvent(SaveCompleted);
      }, m_Context);
    }

    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;
  }
}
