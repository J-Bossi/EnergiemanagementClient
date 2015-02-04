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

using Ork.Energy.Domain.DomainModelService;
using Ork.Framework;

namespace Ork.RoomBook.ViewModels
{
  public class RoomViewModel : DocumentBase
  {
    private readonly Room m_Model;

    public RoomViewModel(Room room)
    {
      m_Model = room;
    }

    public RoomViewModel()
    {
      m_Model = new Room();
    }

    public Room Model
    {
      get { return m_Model; }
    }

    public string RoomNumber
    {
      get { return m_Model.RoomNumber; }
      set { m_Model.RoomNumber = value; }
    }

    public int Floor
    {
      get { return m_Model.Floor; }
      set { m_Model.Floor = value; }
    }

    public float? Space
    {
      get { return m_Model.Space; }
      set
      {
        m_Model.Space = value;
        NotifyOfPropertyChange(() => RoomVolume);
      }
    }

    public float? Height
    {
      get { return m_Model.Height; }
      set
      {
        m_Model.Height = value;
        NotifyOfPropertyChange(() => RoomVolume);
      }
    }

    //public string RoomInformation
    //{
    //  get { return m_Model.RoomInformation; }
    //  set { m_Model.RoomInformation = value; }
    //}

    //public string RoomUsage
    //{
    //  get { return m_Model.RoomUsage; }
    //  set { m_Model.RoomUsage = value; }
    //}

    public float? RoomVolume
    {
      get { return (m_Model.Space * m_Model.Height); }
    }
  }
}