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
using System.Windows.Shapes;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace wb2br_proto
{
    /// <summary>
    /// NewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class HtmlView : Window
    {

        bool _isNavigating = false;
        string _sourceUrl = "";

        //ブラウザセッティング
        CoreWebView2Settings _webViewSettings;
        CoreWebView2Settings WebViewSettings
        {
            get
            {
                if (_webViewSettings == null && webView?.CoreWebView2 != null)
                {
                    _webViewSettings = webView.CoreWebView2.Settings;
                }
                return _webViewSettings;
            }
        }

        public HtmlView(string sourceUrl)
        {
            InitializeComponent();
            AttachControlEventHandlers(webView);
            _sourceUrl = "view-source:" + sourceUrl;
            ViewHtmlSourceInitialize();
        }

        //初期ロード時イベント紐付け（ラッパー）
        void AttachControlEventHandlers(WebView2 control)
        {
            //ナビゲーション開始イベント
            control.NavigationStarting += WebView_NavigationStarting;
            //ナビゲーション完了イベント
            control.NavigationCompleted += WebView_NavigationCompleted;
            //control.CoreWebView2InitializationCompleted +=
            //control.KeyDown +=
        }

        //ナビゲーション開始イベント
        void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            _isNavigating = true;
            RequeryCommands();
        }

        //ナビゲーション完了イベント
        void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _isNavigating = false;
            RequeryCommands();
        }

        //CanExecuteChangedを検知するメソッド
        void RequeryCommands()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        //初期ロード
        async void ViewHtmlSourceInitialize()
        {
            await webView.EnsureCoreWebView2Async();

            Uri uri =new Uri(_sourceUrl);
            webView.CoreWebView2.Navigate(uri.ToString());
            srcViewUrl.Text = _sourceUrl;
        }

        //更新ロード
        void GoToPageCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && !_isNavigating;
        }

        async void GoToPageCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async();

            var rawUrl = (string)e.Parameter;
            Uri uri = new Uri(rawUrl);
            webView.CoreWebView2.Navigate("view-source:" + uri.ToString());
        }

    }
}
