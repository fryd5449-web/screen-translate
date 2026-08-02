using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScreenTranslate.Models;
namespace ScreenTranslate.Views
{
    public partial class FloatingButtonWindow : Window
    {
        private Point _mouseDownPoint;
        private bool _dragged;
        public FloatingButtonWindow(IEnumerable<LanguageOption> languages, string selectedCode)
        {
            InitializeComponent();
            foreach (var language in languages)
            {
                var item = new MenuItem { Header = language.DisplayName, Tag = language.Code, IsCheckable = true, IsChecked = string.Equals(language.Code, selectedCode, StringComparison.OrdinalIgnoreCase) };
                item.Click += LanguageItem_OnClick;
                LanguageMenu.Items.Add(item);
            }
            Loaded += delegate { if (double.IsNaN(Left)) Left = SystemParameters.WorkArea.Right - Width - 18; if (double.IsNaN(Top)) Top = SystemParameters.WorkArea.Top + (SystemParameters.WorkArea.Height - Height) / 2; };
        }
        public event EventHandler CaptureRequested;
        public event EventHandler OpenProgramRequested;
        public event EventHandler<string> LanguageRequested;
        public event EventHandler ExitRequested;

        private void Button_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) { _mouseDownPoint = e.GetPosition(this); _dragged = false; ButtonSurface.CaptureMouse(); }
        private void Button_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var current = e.GetPosition(this);
            if (Math.Abs(current.X - _mouseDownPoint.X) < 4 && Math.Abs(current.Y - _mouseDownPoint.Y) < 4) return;
            _dragged = true; ButtonSurface.ReleaseMouseCapture(); DragMove();
        }
        private void Button_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) { ButtonSurface.ReleaseMouseCapture(); if (!_dragged && CaptureRequested != null) CaptureRequested(this, EventArgs.Empty); }
        private void OpenProgram_OnClick(object sender, RoutedEventArgs e) { if (OpenProgramRequested != null) OpenProgramRequested(this, EventArgs.Empty); }
        private void HideButton_OnClick(object sender, RoutedEventArgs e) { Hide(); }
        private void Exit_OnClick(object sender, RoutedEventArgs e) { if (ExitRequested != null) ExitRequested(this, EventArgs.Empty); }
        private void LanguageItem_OnClick(object sender, RoutedEventArgs e)
        {
            var selected = sender as MenuItem; if (selected == null) return;
            foreach (var item in LanguageMenu.Items) { var menuItem = item as MenuItem; if (menuItem != null) menuItem.IsChecked = menuItem == selected; }
            var code = selected.Tag as string; if (LanguageRequested != null) LanguageRequested(this, code);
        }
    }
}
