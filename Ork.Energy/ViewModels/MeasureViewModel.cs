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
using Ork.Energy.Domain.DomainModelService;
using Ork.Framework;

namespace Ork.Energy.ViewModels
{
  public class MeasureViewModel
  {
    private readonly EnergyMeasure m_Model;

    public MeasureViewModel(EnergyMeasure measure)
    {
      m_Model = measure;
    }

    public DateTime? CreationDate
    {
      get { return m_Model.CreationDate; }
    }

    public string CreationDateString
    {
      get { return m_Model.CreationDate.ToShortDateString(); }
    }

    public double EvaluationRating
    {
      get { return m_Model.EvaluationRating; }
    }

    public int Id
    {
      get { return m_Model.Id; }
    }

    public string Name
    {
      get { return m_Model.Name; }
      set { m_Model.Name = value; }
    }

    public EnergyMeasure Model
    {
      get { return m_Model; }
    }

    public string Description
    {
      get { return m_Model.Description; }
    }

    public DateTime DueDate
    {
      get { return m_Model.DueDate; }
    }

    public string DueDateString
    {
      get { return m_Model.DueDate.ToShortDateString(); }
    }

    public DateTime? EntryDate
    {
      get { return m_Model.EntryDate; }
    }

    public string EntryDateString
    {
      get
      {
        if (!m_Model.EntryDate.HasValue)
        {
          return "";
        }

        return m_Model.EntryDate.GetValueOrDefault()
                      .ToShortDateString();
      }
    }

    public string Evaluation
    {
      get { return m_Model.Evaluation; }
    }

    public int Priority
    {
      get { return m_Model.Priority; }
    }

    public string PriorityName
    {
      get { return TranslationProvider.Translate(((Priority) Priority).ToString()); }
    }

    public int Status
    {
      get { return m_Model.Status; }
    }

    public string StatusName
    {
      get { return TranslationProvider.Translate(((Status) Status).ToString()); }
    }

    public ResponsibleSubject ResponsibleSubject
    {
      get { return m_Model.ResponsibleSubject; }
    }

    public string ResponsibleSubjectName
    {
      get
      {
        if (ResponsibleSubject is Employee)
        {
          var employee = (Employee) ResponsibleSubject;
          return employee.FirstName + " " + employee.LastName;
        }
        else
        {
          var group = (EmployeeGroup) ResponsibleSubject;
          return group.Name;
        }
      }
    }

    public string Kenn
    {
      get { return m_Model.Parameter; }
    }

    public string MeterDevice
    {
      get { return m_Model.Meter; }
    }

    public double SavedWattSoll
    {
      get { return m_Model.SavedWattShould; }
    }

    public double SavedMoneySoll
    {
      get { return m_Model.SavedMoneyShould; }
    }

    public double SavedWattIst
    {
      get { return m_Model.SavedWattIs; }
    }

    public double SavedMoneyIst
    {
      get { return m_Model.SavedMoneyIs; }
    }

    public double SavedMoneyAktuell
    {
      get { return m_Model.SavedMoneyAtm; }
    }

    public double SavedWattAktuell
    {
      get { return m_Model.SavedWattAtm; }
    }

    public double SavedCO2
    {
      get { return m_Model.SavedWattShould * 0.61; }
    }

    public string ConsumerUnit
    {
      get { return m_Model.ConsumerUnit; }
    }

    public double Investment
    {
      get { return m_Model.Investment; }
    }

    public double Amortisationtime
    {
      get
      {
        return Math.Round((m_Model.Investment + m_Model.FailureMoney) / m_Model.SavedMoneyShould * 365, 0);
        // Errechnet sich aus Investitionskosten+Ausfallkosten und Wert der Einsparung nach beendeter Maßnahme (SavedMoneySoll) * 365 Tage (für die Umrechnung)
      }
    }

    public ConsumerGroup ConsumerGroup
    {
      get { return m_Model.Consumer.ConsumerGroup; }
    }

    public string ConsumerName
    {
      get { return m_Model.Consumer.Name; }
    }

    public string DueDateIsDelayed
    {
      get { return TranslationProvider.Translate("DueDateIsDelayed"); }
    }

    public string EntryDateIsDelayed
    {
      get { return TranslationProvider.Translate("EntryDateIsDelayed"); }
    }

    public bool Delayed
    {
      get
      {
        if (DueDate < DateTime.Today &&
            Status != 2)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    public bool DelayedCompleted
    {
      get
      {
        if (EntryDate != null &&
            (EntryDate.Value.Date > DueDate.Date && Status == 2))
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }
  }
}