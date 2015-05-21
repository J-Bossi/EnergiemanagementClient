#region License

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0.html
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  
// Copyright (c) 2015, HTW Berlin

#endregion

using System;
using System.ComponentModel.Composition;
using System.Data.Services.Client;
using System.Linq;
using Ork.Energy.Domain.DomainModelService;
using Ork.Setting;

namespace Ork.RoomBook
{
  [Export(typeof (IRoomRepository))]
  public class RoomRepository : IRoomRepository
  {
    private readonly Func<DomainModelContext> m_CreateMethod;
    private DomainModelContext m_Context;

    [ImportingConstructor]
    public RoomRepository([Import] ISettingsProvider settingsContainer, [Import] Func<DomainModelContext> createMethod)
    {
      m_CreateMethod = createMethod;
      settingsContainer.ConnectionStringUpdated += (s, e) => Initialize();
      Initialize();
    }


    public DataServiceCollection<Room> Rooms { get; private set; }
    public DataServiceCollection<RoomUsage> RoomUsages { get; private set; }
    public bool HasConnection { get; private set; }

    public void DeleteObject(object objectToDelete)
    {
      m_Context.DeleteObject(objectToDelete);
    }

    public void ClearPendingChanges()
    {
      // Discard all Pending changes without reloading the context. Actually calling Initialize() would do the same
      foreach (
        var link in m_Context.Links.Where(link => link.State != EntityStates.Unchanged && link.State != EntityStates.Detached))
      {
        m_Context.DetachLink(link.Source, link.SourceProperty, link.Target);
      }
      foreach (var entity in
        m_Context.Entities.Where(entity => entity.State != EntityStates.Unchanged && entity.State != EntityStates.Detached))
      {
        m_Context.Detach(entity.Entity);
      }
      try
      {
        LoadRooms();
        LoadRoomUsages();
        HasConnection = true;
      }
      catch (Exception ex)
      {
        HasConnection = false;
      }
      RaiseEvent(ContextChanged);
    }

    public void Save()
    {
      if (m_Context.ApplyingChanges)
      {
        return;
      }

      var result = m_Context.BeginSaveChanges(SaveChangesOptions.Batch, c =>
                                                                        {
                                                                          var dmc = (DomainModelContext) c.AsyncState;
                                                                          dmc.EndSaveChanges(c);
                                                                          RaiseEvent(SaveCompleted);
                                                                        }, m_Context);
    }

    public event EventHandler ContextChanged;
    public event EventHandler SaveCompleted;

    private void Initialize()
    {
      m_Context = m_CreateMethod();

      try
      {
        LoadRooms();
        LoadRoomUsages();
        HasConnection = true;
      }
      catch (Exception ex)
      {
        HasConnection = false;
      }
      RaiseEvent(ContextChanged);
    }


    private void LoadRooms()
    {
      Rooms = new DataServiceCollection<Room>(m_Context);
      var query = m_Context.Rooms.Expand("RoomInformation");
      Rooms.Load(query);
    }

    private void LoadRoomUsages()
    {
      RoomUsages = new DataServiceCollection<RoomUsage>(m_Context);
      var query = m_Context.RoomUsages;
      RoomUsages.Load(query);
    }

    private void RaiseEvent(EventHandler eventHandler)
    {
      if (eventHandler != null)
      {
        eventHandler(this, new EventArgs());
      }
    }
  }
}