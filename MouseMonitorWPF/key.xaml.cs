/*
 *  key.xaml.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MouseMonitorWPF
{
    /// <summary>
    /// Interaktionslogik für key.xaml
    /// </summary>
    public partial class key : UserControl
    {
        private bool _isWideKey = false;
        public key()
        {
            InitializeComponent();
        }
        public key(String v)
        {
            InitializeComponent();
            this.Value = v;
        }
        public key(String v, bool isWideKey)
        {
            InitializeComponent();
            this.Value = v;
            this.isWideKey = isWideKey;
        }
        public String Value
        {
            get { return KeyValue.Text; }
            set { KeyValue.Text = value; }
        }
        public bool isWideKey
        {
            get
            {
                return _isWideKey;
            }
            set
            {
                _isWideKey = value;
                if (value)
                {
                    KeyNormal.Opacity = 0;
                    KeyWide.Opacity = 1;
                }
                else
                {
                    KeyNormal.Opacity = 1;
                    KeyWide.Opacity = 0;
                }
            }
        }
    }
}
