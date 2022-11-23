/*
 *  NotifyIcon.cs
 *  
 *  Copyright 2013 - Holger Genth
 *  
 *  This file is part of MouseMonitorWPF.
 *
 *  MouseMonitorWPF is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  MouseMonitorWPF is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  Diese Datei ist Teil von MouseMonitorWPF.
 *
 *  MouseMonitorWPF ist Freie Software: Sie können es unter den Bedingungen
 *  der GNU General Public License, wie von der Free Software Foundation,
 *  Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren
 *  veröffentlichten Version, weiterverbreiten und/oder modifizieren.
 *
 *  MouseMonitorWPF wird in der Hoffnung, dass es nützlich sein wird, aber
 *  OHNE JEDE GEWÄHELEISTUNG, bereitgestellt; sogar ohne die implizite
 *  Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK.
 *  Siehe die GNU General Public License für weitere Details.
 *
 *  Sie sollten eine Kopie der GNU General Public License zusammen mit diesem
 *  Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
 */
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace MouseMonitorWPF
{
    [ContentProperty("Text"), DefaultEvent("MouseDoubleClick"),
     System.Security.Permissions.UIPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
    public class NotifyIcon : FrameworkElement, IDisposable, IAddChild
    {
        #region PrivateVariables

        private static readonly int TaskbarCreatedWindowMessage;

        private static readonly System.Security.Permissions.UIPermission _allWindowsPermission = new System.Security.Permissions.UIPermission(System.Security.Permissions.UIPermissionWindow.AllWindows);
        private static int _nextId;

        private readonly object _syncObj = new object();

        private NotifyIconHwndSource _hwndSource;
        private int _id = _nextId++;
        private bool _iconCreated;
        private bool _doubleClick;

        #endregion

        #region Events

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        public event RoutedEventHandler DoubleClick
        {
            add { AddHandler(DoubleClickEvent, value); }
            remove { RemoveHandler(DoubleClickEvent, value); }
        }

        public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent("MouseClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

        public event MouseButtonEventHandler MouseClick
        {
            add { AddHandler(MouseClickEvent, value); }
            remove { RemoveHandler(MouseClickEvent, value); }
        }

        public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent("MouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

        public event MouseButtonEventHandler MouseDoubleClick
        {
            add { AddHandler(MouseDoubleClickEvent, value); }
            remove { RemoveHandler(MouseDoubleClickEvent, value); }
        }

        #endregion

        #region Constructor/Destructor

        [SecurityCritical]
        static NotifyIcon()
        {
            TaskbarCreatedWindowMessage = User32Wrapper.RegisterWindowMessage("TaskbarCreated");

            VisibilityProperty.OverrideMetadata(typeof(NotifyIcon), new FrameworkPropertyMetadata(OnVisibilityChanged));
        }

        [SecurityCritical]
        public NotifyIcon()
        {
            UpdateIconForVisibility();

            IsVisibleChanged += OnIsVisibleChanged;
        }

        [SecurityCritical]
        ~NotifyIcon()
        {
            Dispose(false);
        }

        [SecurityCritical]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SecurityCritical]
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_hwndSource != null)
                {
                    UpdateIcon(false);
                    _hwndSource.Dispose();
                }
            }
            else if (_hwndSource != null)
            {
                User32Wrapper.PostMessage(new HandleRef(_hwndSource, _hwndSource.Handle), User32Wrapper.WindowMessage.Close, IntPtr.Zero, IntPtr.Zero);
                _hwndSource.Dispose();
            }
        }

        #endregion

        #region Private Methods

        [SecurityCritical]
        private void ShowContextMenu()
        {
            if (ContextMenu != null)
            {
                User32Wrapper.SetForegroundWindow(new HandleRef(_hwndSource, _hwndSource.Handle));

                ContextMenuService.SetPlacement(ContextMenu, PlacementMode.MousePoint);
                ContextMenu.IsOpen = true;
            }
        }

        [SecurityCritical]
        private void UpdateIconForVisibility()
        {
            UpdateIcon(true);
        }

        [SecurityCritical]
        private void UpdateIcon(bool showIconInTray)
        {
            lock (_syncObj)
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    IntPtr iconHandle = IntPtr.Zero;

                    try
                    {
                        _allWindowsPermission.Demand();

                        if (showIconInTray && _hwndSource == null)
                        {
                            _hwndSource = new NotifyIconHwndSource(this);
                        }

                        if (_hwndSource != null)
                        {
                            _hwndSource.LockReference(showIconInTray);

                            User32Wrapper.NOTIFYICONDATA pnid = new User32Wrapper.NOTIFYICONDATA
                            {
                                uCallbackMessage = (int)User32Wrapper.WindowMessage.TrayMouseMessage,
                                uFlags = User32Wrapper.NotifyIconFlags.Message | User32Wrapper.NotifyIconFlags.ToolTip,
                                hWnd = _hwndSource.Handle,
                                uID = _id,
                                szTip = Text
                            };
                            if (Icon != null)
                            {
                                iconHandle = User32Wrapper.GetHIcon(Icon);

                                pnid.uFlags |= User32Wrapper.NotifyIconFlags.Icon;
                                pnid.hIcon = iconHandle;
                            }

                            if (showIconInTray && iconHandle != null)
                            {
                                if (!_iconCreated)
                                {
                                    User32Wrapper.Shell_NotifyIcon(0, pnid);
                                    _iconCreated = true;
                                }
                                else
                                {
                                    User32Wrapper.Shell_NotifyIcon(1, pnid);
                                }
                            }
                            else if (_iconCreated)
                            {
                                User32Wrapper.Shell_NotifyIcon(2, pnid);
                                _iconCreated = false;
                            }
                        }
                    }
                    finally
                    {
                        if (iconHandle != IntPtr.Zero)
                        {
                            User32Wrapper.DestroyIcon(iconHandle);
                        }
                    }
                }
            }
        }

        [SecurityCritical]
        private static void OnVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((NotifyIcon)o).UpdateIconForVisibility();
        }

        [SecurityCritical]
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateIconForVisibility();
        }

        #endregion

        #region WndProc Methods

        private void WmMouseDown(MouseButton button, int clicks)
        {
            MouseButtonEventArgs args = null;

            if (clicks == 2)
            {
                RaiseEvent(new RoutedEventArgs(DoubleClickEvent));

                args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
                args.RoutedEvent = MouseDoubleClickEvent;
                RaiseEvent(args);

                _doubleClick = true;
            }

            args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
            args.RoutedEvent = MouseDownEvent;
            RaiseEvent(args);
        }

        private void WmMouseMove()
        {
            MouseEventArgs args = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0);
            args.RoutedEvent = MouseMoveEvent;
            RaiseEvent(args);
        }

        private void WmMouseUp(MouseButton button)
        {
            MouseButtonEventArgs args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
            args.RoutedEvent = MouseUpEvent;
            RaiseEvent(args);

            if (!_doubleClick)
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent));

                args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, button);
                args.RoutedEvent = MouseClickEvent;
                RaiseEvent(args);
            }

            _doubleClick = false;
        }

        [SecurityCritical]
        private void WmTaskbarCreated()
        {
            _iconCreated = false;
            UpdateIcon(IsVisible);
        }

        [SecurityCritical]
        private void WndProc(int message, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            handled = true;

            if (message <= (int)User32Wrapper.WindowMessage.MeasureItem)
            {
                if (message == (int)User32Wrapper.WindowMessage.Destroy)
                {
                    UpdateIcon(false);
                    return;
                }
            }
            else
            {
                if (message != (int)User32Wrapper.WindowMessage.TrayMouseMessage)
                {
                    if (message == TaskbarCreatedWindowMessage)
                    {
                        WmTaskbarCreated();
                    }
                    handled = false;
                    return;
                }
                switch ((User32Wrapper.WindowMessage)lParam)
                {
                    case User32Wrapper.WindowMessage.MouseMove:
                        WmMouseMove();
                        return;
                    case User32Wrapper.WindowMessage.MouseDown:
                        WmMouseDown(MouseButton.Left, 1);
                        return;
                    case User32Wrapper.WindowMessage.LButtonUp:
                        WmMouseUp(MouseButton.Left);
                        return;
                    case User32Wrapper.WindowMessage.LButtonDblClk:
                        WmMouseDown(MouseButton.Left, 2);
                        return;
                    case User32Wrapper.WindowMessage.RButtonDown:
                        WmMouseDown(MouseButton.Right, 1);
                        return;
                    case User32Wrapper.WindowMessage.RButtonUp:
                        ShowContextMenu();
                        WmMouseUp(MouseButton.Right);
                        return;
                    case User32Wrapper.WindowMessage.RButtonDblClk:
                        WmMouseDown(MouseButton.Right, 2);
                        return;
                    case User32Wrapper.WindowMessage.MButtonDown:
                        WmMouseDown(MouseButton.Middle, 1);
                        return;
                    case User32Wrapper.WindowMessage.MButtonUp:
                        WmMouseUp(MouseButton.Middle);
                        return;
                    case User32Wrapper.WindowMessage.MButtonDblClk:
                        WmMouseDown(MouseButton.Middle, 2);
                        return;
                }
                return;
            }
            if (message == TaskbarCreatedWindowMessage)
            {
                WmTaskbarCreated();
            }
            handled = false;
        }

        [SecurityCritical]
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            WndProc(msg, wParam, lParam, out handled);

            return IntPtr.Zero;
        }

        #endregion

        #region Properties

        #region Text

        // Tooltip Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NotifyIcon), new FrameworkPropertyMetadata(OnTextPropertyChanged, OnCoerceTextProperty), ValidateTextPropety);

        private static bool ValidateTextPropety(object baseValue)
        {
            string value = (string)baseValue;

            return value == null || value.Length <= 0x3f;
        }

        [SecurityCritical]
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon notifyIcon = (NotifyIcon)d;

            if (notifyIcon._iconCreated)
            {
                notifyIcon.UpdateIcon(true);
            }
        }

        private static object OnCoerceTextProperty(DependencyObject d, object baseValue)
        {
            string value = (string)baseValue;

            if (value == null)
            {
                value = string.Empty;
            }

            return value;
        }

        #endregion

        #region Icon

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            Window.IconProperty.AddOwner(typeof(NotifyIcon), new FrameworkPropertyMetadata(OnNotifyIconChanged) { Inherits = true });

        [SecurityCritical]
        private static void OnNotifyIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NotifyIcon notifyIcon = (NotifyIcon)o;

            notifyIcon.UpdateIcon(notifyIcon.IsVisible);
        }

        #endregion

        #endregion

        #region IAddChild Members

        void IAddChild.AddChild(object value)
        {
            throw new InvalidOperationException("IAddChild_TextOnly");
        }

        void IAddChild.AddText(string text)
        {
            Text = text;
        }

        #endregion

        #region NotifyIconNativeWindow Class

        private class NotifyIconHwndSource : HwndSource
        {
            private NotifyIcon _reference;
            private GCHandle _rootRef;

            [SecurityCritical]
            internal NotifyIconHwndSource(NotifyIcon component)
                : base(0, 0, 0, 0, 0, null, IntPtr.Zero)
            {
                _reference = component;

                AddHook(_reference.WndProc);
            }

            [SecurityCritical]
            ~NotifyIconHwndSource()
            {
                if (Handle != IntPtr.Zero)
                {
                    User32Wrapper.PostMessage(new HandleRef(this, Handle), User32Wrapper.WindowMessage.Close, IntPtr.Zero, IntPtr.Zero);
                }
            }

            public void LockReference(bool locked)
            {
                if (locked)
                {
                    if (!_rootRef.IsAllocated)
                    {
                        _rootRef = GCHandle.Alloc(_reference, GCHandleType.Normal);
                    }
                }
                else if (_rootRef.IsAllocated)
                {
                    _rootRef.Free();
                }
            }
        }

        #endregion
    }
}
