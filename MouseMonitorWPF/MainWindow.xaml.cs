using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;

namespace MouseMonitorWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region privateVariables

        private int  _initialStyle;
        private Rect _desktopWorkingArea;

        private MouseHookListener    _mouseListener;
        private KeyboardHookListener _keyboardListener;

        private System.Windows.Threading.DispatcherTimer _timer;
        private int  _scroll_image_age;
        private bool _scroll_up;

        private BitmapImage[] _scrollImages;

        private String[] _scrollImagesSrc = {
                                             "mouse_scrldwn.0.png",
                                             "mouse_scrldwn.1.png",
                                             "mouse_scrlup.0.png",
                                             "mouse_scrlup.1.png"
                                         };

        private Dictionary<Keys, key> _keysPressed;

        #endregion

        #region Constructor/Destructor
        public MainWindow()
        {
            InitializeComponent();
            this.Opacity = Properties.Settings.Default.Opacity;
            _desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
        }
        ~MainWindow()
        {           
            GlobalHookDeactivate();
        }
        #endregion

        #region Initialisation

        private void MouseMonitor_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            _initialStyle = User32Wrapper.GetWindowLong(handle, User32Wrapper.GWL_EXSTYLE);

            User32Wrapper.SetWindowLong(handle, User32Wrapper.GWL_EXSTYLE, (int)(_initialStyle | User32Wrapper.WS_EX_TRANSPARENT));
            
            _keysPressed = new Dictionary<Keys, key>();

            PlaceLowerRight();
            _timer = new System.Windows.Threading.DispatcherTimer();

            TrayIcon.MouseDoubleClick += new MouseButtonEventHandler(TrayDoubleClicked);

            GlobalHookActivate();

        }
        // Lädt alle benötigten Bilder
        private void LoadImages()
        {
            _scrollImages = new BitmapImage[_scrollImagesSrc.Length];
            for (int i = 0; i < _scrollImagesSrc.Length; i++)
            {
                LoadImageFromResource(ref _scrollImages[i], ref _scrollImagesSrc[i]);
            }
        }
        #endregion

        #region PrivateFunctions

        // Lädt Bilder aus der Programm-Resource in ein BitmapImage
        private void LoadImageFromResource(ref BitmapImage bi, ref String path)
        {
            bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("res/" + path, UriKind.Relative);
            bi.EndInit();
        }

        // Platziert das Fenster rechts unten auf dem ersten Bildschirm
        private void PlaceLowerRight()
        {
            //Determine "rightmost" screen
            this.Left = _desktopWorkingArea.Right - this.Width;
            this.Top = _desktopWorkingArea.Bottom - this.Height;

        }
        private void SetOpacity(float opacity)
        {
            Properties.Settings.Default.Opacity = opacity;
            Properties.Settings.Default.Save();
            this.Opacity = opacity;
        }

        public void GlobalHookActivate()
        {
            LoadImages();

            GlobalHooker gh = new GlobalHooker();

            _mouseListener = new MouseHookListener(gh);

            _mouseListener.Enabled = true;

            _mouseListener.MouseDownExt += MouseListener_MouseDownExt;
            _mouseListener.MouseUp += MouseListener_MouseUp;
            _mouseListener.MouseWheel += MouseListener_MouseWheel;

            _keyboardListener = new KeyboardHookListener(gh);

            _keyboardListener.Enabled = true;

            _keyboardListener.KeyDown += KeyboardListener_KeyDown;
            _keyboardListener.KeyUp += KeyboardListener_KeyUp;

            _timer.Tick += new EventHandler(mouseScroll_Timer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _timer.Start();
        }

        public void GlobalHookDeactivate()
        {
            //_mouseListener.Dispose();
            //_keyboardListener.Dispose();
        }

        private key GenerateKeyFromCharCode(System.Windows.Forms.KeyEventArgs e)
        {
            bool isWideKey = false;
            String name = "";

            if ((e.KeyCode == Keys.LMenu) && Properties.Settings.Default.ShowAltKey)
            {
                isWideKey = true;
                name = "Alt";
            }
            else if ((e.KeyCode == Keys.RMenu) && Properties.Settings.Default.ShowAltKey)
            {
                isWideKey = true;
                name = "Alt Gr";
            }
            else if (((e.KeyCode == Keys.LControlKey) || (e.KeyCode == Keys.RControlKey)) && Properties.Settings.Default.ShowControlKey)
            {
                isWideKey = true;
                name = "Strg";
            }
            else if (((e.KeyCode == Keys.LShiftKey) || (e.KeyCode == Keys.RShiftKey)) && Properties.Settings.Default.ShowShiftKey)
            {
                isWideKey = true;
                name = "Shift";
            }
            else if ((e.KeyCode == Keys.Enter) && Properties.Settings.Default.ShowEnterKey)
            {
                isWideKey = true;
                name = "Enter";
            }
            else if ((e.KeyCode == Keys.Escape) && Properties.Settings.Default.ShowEscKey)
            {
                name = "Esc";
            }
            else if ((e.KeyCode == Keys.Space) && Properties.Settings.Default.ShowSpaceKey)
            {
                isWideKey = true;
                name = "[Space]";
            }
            // Funktionstasten
            else if ((e.KeyCode >= Keys.F1) && (e.KeyCode <= Keys.F12) && Properties.Settings.Default.ShowFKeys)
            {
                name = "F" + (e.KeyCode - Keys.F1 + 1);
            }
            // Buchstaben
            else if ((e.KeyCode >= Keys.A) && (e.KeyCode <= Keys.Z) && Properties.Settings.Default.ShowNormalKeys)
            {
                name = "" + Convert.ToChar(e.KeyCode);
            }
            // Ziffern
            else if ((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9) && Properties.Settings.Default.ShowNumbers)
            {
                name = "" + Convert.ToChar(e.KeyCode);
            }
            if (name == "") return null;

            return new key(name, isWideKey); ;
        }

        #endregion

        #region Events

        private void onClose_Application(object sender, RoutedEventArgs e)
        {

            this.Close();

        }
        private void onPosition_BottomRight(object sender, RoutedEventArgs e)
        {
            PlaceLowerRight();
        }
        private void onOpacityChange(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem mi = e.OriginalSource as System.Windows.Controls.MenuItem;
            String s = mi.Header.ToString();
            float num = float.Parse(s.TrimEnd(new char[] { '%', ' ' })) / 100f;

            SetOpacity(num);
        }
        void TrayDoubleClicked(object sender, EventArgs e)
        {
            if (this.IsVisible)
                this.Hide();
            else
                this.Show();
        }

        private void MouseListener_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                _scroll_up = true;
                MouseImage_scroll.Source = _scrollImages[3];
            }
            else
            {
                _scroll_up = false;
                MouseImage_scroll.Source = _scrollImages[1];
            }
            MouseImage_scroll.Opacity = 1;

            _scroll_image_age = 0;
        }
        private void mouseScroll_Timer_Tick(object sender, EventArgs e) {
            if (_scroll_image_age < 0) return;

            BitmapImage img;
            int offs = ((_scroll_image_age % 2) == 0 ? 0 : 1);

            if (_scroll_up)
            {
                img = _scrollImages[2+offs];
            }
            else
            {
                img = _scrollImages[offs];
            }

            if (_scroll_image_age >= 2)
            {
                MouseImage_scroll.Opacity = 0;
                _scroll_image_age = -1;
            }
            else
            {
                MouseImage_scroll.Source = img;
                _scroll_image_age++;
            }
        }
        private void MouseListener_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    MouseImage_lmb.Opacity = 0;
                    break;
                case MouseButtons.Right:
                    MouseImage_rmb.Opacity = 0;
                    break;
                case MouseButtons.Middle:
                    MouseImage_mmb.Opacity = 0;
                    break;
            }
        }
        private void MouseListener_MouseDownExt(object sender, MouseEventExtArgs e)
        {         
            switch (e.Button)
            {
                case MouseButtons.Left:
                    MouseImage_lmb.Opacity = 1;
                    break;
                case MouseButtons.Right:
                    MouseImage_rmb.Opacity = 1;
                    break;
                case MouseButtons.Middle:
                    MouseImage_mmb.Opacity = 1;
                    break;
            }
        }

        private void KeyboardListener_KeyDown(Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (_keysPressed.ContainsKey(e.KeyCode)) return;            

            key k = GenerateKeyFromCharCode(e);
            if (k == null) return;

            _keysPressed.Add(e.KeyCode, k);

            if (k.isWideKey)
                KeysPressed.Children.Add(k);
            else
                KeysPressed.Children.Insert(0,k);
                
        }
        private void KeyboardListener_KeyUp(Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!_keysPressed.ContainsKey(e.KeyCode)) return;
            key k = _keysPressed[e.KeyCode];
            KeysPressed.Children.Remove(k);
            _keysPressed.Remove(e.KeyCode);
        }

        #endregion
    }
}
