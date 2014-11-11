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
// Copyright (c) 2013, HTW Berlin

#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Ork.Framework.Controls
{
  public class DatePickerCalendar
  {
    public static readonly DependencyProperty IsYearProperty = DependencyProperty.RegisterAttached("IsYear", typeof (bool), typeof (DatePickerCalendar), new PropertyMetadata(OnIsYearChanged));

    public static bool GetIsYear(DependencyObject dobj)
    {
      return (bool) dobj.GetValue(IsYearProperty);
    }

    public static void SetIsYear(DependencyObject dobj, bool value)
    {
      dobj.SetValue(IsYearProperty, value);
    }

    private static void OnIsYearChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
    {
      var datePicker = (DatePicker) dobj;

      Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action<DatePicker, DependencyPropertyChangedEventArgs>(SetCalendarEventHandlers), datePicker, e);
    }

    private static void SetCalendarEventHandlers(DatePicker datePicker, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
      {
        return;
      }

      if ((bool) e.NewValue)
      {
        datePicker.CalendarOpened += DatePickerOnCalendarOpened;
        datePicker.CalendarClosed += DatePickerOnCalendarClosed;
      }
      else
      {
        datePicker.CalendarOpened -= DatePickerOnCalendarOpened;
        datePicker.CalendarClosed -= DatePickerOnCalendarClosed;
      }
    }

    private static void DatePickerOnCalendarOpened(object sender, RoutedEventArgs routedEventArgs)
    {
      var calendar = GetDatePickerCalendar(sender);
      calendar.DisplayMode = CalendarMode.Decade;

      calendar.DisplayModeChanged += CalendarOnDisplayModeChanged;
    }

    private static void DatePickerOnCalendarClosed(object sender, RoutedEventArgs routedEventArgs)
    {
      var datePicker = (DatePicker) sender;
      var calendar = GetDatePickerCalendar(sender);
      datePicker.SelectedDate = calendar.SelectedDate;

      calendar.DisplayModeChanged -= CalendarOnDisplayModeChanged;
    }

    private static void CalendarOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
    {
      var calendar = (Calendar) sender;
      if (calendar.DisplayMode != CalendarMode.Year)
      {
        return;
      }

      calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);

      var datePicker = GetCalendarsDatePicker(calendar);
      datePicker.IsDropDownOpen = false;
    }

    private static Calendar GetDatePickerCalendar(object sender)
    {
      var datePicker = (DatePicker) sender;
      var popup = (Popup) datePicker.Template.FindName("PART_Popup", datePicker);
      return ((Calendar) popup.Child);
    }

    private static DatePicker GetCalendarsDatePicker(FrameworkElement child)
    {
      var parent = (FrameworkElement) child.Parent;
      if (parent.Name == "PART_Root")
      {
        return (DatePicker) parent.TemplatedParent;
      }
      return GetCalendarsDatePicker(parent);
    }

    private static DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
    {
      if (!selectedDate.HasValue)
      {
        return null;
      }
      return new DateTime(selectedDate.Value.Year, 1, 1);
    }
  }
}