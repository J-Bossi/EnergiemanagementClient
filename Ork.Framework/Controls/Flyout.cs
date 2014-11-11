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
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Ork.Framework.Controls
{
  public class Flyout : ContentControl
  {
    public static readonly DependencyProperty OpenedProperty = DependencyProperty.Register("Opened", typeof (bool), typeof (Flyout), new PropertyMetadata(default(bool), OpenedChanged));

    public static readonly DependencyProperty PinnedProperty = DependencyProperty.Register("Pinned", typeof (bool), typeof (Flyout), new PropertyMetadata(default(bool), PinnedChanged));

    public static readonly DependencyProperty OpenCloseAnimationProperty = DependencyProperty.Register("OpenCloseAnimation", typeof (DoubleAnimation), typeof (Flyout),
      new PropertyMetadata(default(DoubleAnimation)));

    public static readonly DependencyProperty FlyoutWidthProperty = DependencyProperty.Register("FlyoutWidth", typeof (double), typeof (Flyout), new PropertyMetadata(default(double)));

    public static readonly DependencyProperty ActivationAreaWidthProperty = DependencyProperty.Register("ActivationAreaWidth", typeof (double), typeof (Flyout), new PropertyMetadata(default(double)));

    public static readonly DependencyProperty FlyoutActualWidthProperty = DependencyProperty.Register("FlyoutActualWidth", typeof (double), typeof (Flyout), new PropertyMetadata(default(double)));

    private bool m_WasPinnedBeforeDeactivation;

    static Flyout()
    {
      //DefaultStyleKeyProperty.OverrideMetadata(typeof (Flyout), new FrameworkPropertyMetadata(typeof (Flyout)));
      EventManager.RegisterClassHandler(typeof (Flyout), Mouse.MouseEnterEvent, new MouseEventHandler(PanelMouseEnter));
      EventManager.RegisterClassHandler(typeof (Flyout), Mouse.MouseLeaveEvent, new MouseEventHandler(PanelMouseLeave));
    }

    public Flyout()
    {
      IsEnabledChanged += OnIsEnabledChanged;
      ActivationAreaWidth = 30;
      FlyoutWidth = 230;
      FlyoutActualWidth = 0;
      Width = ActivationAreaWidth;
      OpenCloseAnimation = new DoubleAnimation(FlyoutWidth, ActivationAreaWidth, new Duration(TimeSpan.FromSeconds(0.15)));
    }

    public double ActivationAreaWidth
    {
      get { return (double) GetValue(ActivationAreaWidthProperty); }
      set { SetValue(ActivationAreaWidthProperty, value); }
    }

    public double FlyoutActualWidth
    {
      get { return (double) GetValue(FlyoutActualWidthProperty); }
      private set { SetValue(FlyoutActualWidthProperty, value); }
    }

    public double FlyoutWidth
    {
      get { return (double) GetValue(FlyoutWidthProperty); }
      set { SetValue(FlyoutWidthProperty, value); }
    }

    public DoubleAnimation OpenCloseAnimation
    {
      get { return (DoubleAnimation) GetValue(OpenCloseAnimationProperty); }
      set { SetValue(OpenCloseAnimationProperty, value); }
    }

    public bool Opened
    {
      get { return (bool) GetValue(OpenedProperty); }
      set { SetValue(OpenedProperty, value); }
    }

    public bool Pinned
    {
      get { return (bool) GetValue(PinnedProperty); }
      set { SetValue(PinnedProperty, value); }
    }

    private void Close()
    {
      BeginAnimation(WidthProperty, OpenCloseAnimation);
      FlyoutActualWidth = 0;
    }

    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      if ((bool) dependencyPropertyChangedEventArgs.NewValue)
      {
        if (m_WasPinnedBeforeDeactivation)
        {
          m_WasPinnedBeforeDeactivation = false;
          Pinned = true;
        }
      }
      else
      {
        if (Pinned)
        {
          m_WasPinnedBeforeDeactivation = true;
          Pinned = false;
        }
        if (Opened)
        {
          Opened = false;
        }
      }
    }

    private void Open()
    {
      BeginAnimation(WidthProperty, null);
      FlyoutActualWidth = FlyoutWidth;
      Width = FlyoutWidth;
    }

    private static void OpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var flyout = (Flyout) d;
      if ((bool) e.NewValue)
      {
        flyout.Open();
      }
      else
      {
        if (flyout.Pinned)
        {
          flyout.Pinned = false;
        }
        flyout.Close();
      }
    }

    private static void PanelMouseEnter(object sender, MouseEventArgs e)
    {
      var flyout = (Flyout) sender;
      flyout.Opened = true;
    }

    private static void PanelMouseLeave(object sender, MouseEventArgs e)
    {
      var flyout = (Flyout) sender;
      if (!flyout.Pinned)
      {
        flyout.Opened = false;
      }
    }

    private void Pin()
    {
      //MapPositionEditorControl.Padding = new Thickness(FlyoutWidth, 0, MapPositionEditorControl.Padding.Right, 0);
    }

    private static void PinnedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var flyout = (Flyout) d;
      if ((bool) e.NewValue)
      {
        if (!flyout.Opened)
        {
          flyout.Opened = true;
        }
        flyout.Pin();
      }
      else
      {
        flyout.Unpin();
      }
    }

    private void Unpin()
    {
      //MapPositionEditorControl.Padding = new Thickness(0, 0, MapPositionEditorControl.Padding.Right, 0);
    }
  }
}