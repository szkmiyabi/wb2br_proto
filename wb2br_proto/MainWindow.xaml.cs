using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace wb2br_proto
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //メンバ
        bool _isNavigating = false;

        //コンストラクタ
        public MainWindow()
        {
            InitializeComponent();
            AttachControlEventHandlers(webView);
        }

        //初期ロード時イベント紐付け（ラッパー）
        void AttachControlEventHandlers(WebView2 control)
        {
            //control.NavigationStarting +=
            //control.NavigationCompleted +=
            //control.CoreWebView2InitializationCompleted +=
            //control.KeyDown +=
        }
    }
}
