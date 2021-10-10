using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        bool _isUserAgentCustomized = false;
        string _defaultUserAgent = "";

        private ObservableCollection<UrlEntity> _urlEntities = new ObservableCollection<UrlEntity>();

        //ブラウザセッティング
        CoreWebView2Settings _webViewSettings;
        CoreWebView2Settings WebViewSettings
        {
            get
            {
                if(_webViewSettings == null && webView?.CoreWebView2 != null)
                {
                    _webViewSettings = webView.CoreWebView2.Settings;
                }
                return _webViewSettings;
            }
        }

        //コンストラクタ
        public MainWindow()
        {
            InitializeComponent();
            AttachControlEventHandlers(webView);
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

        //ComboのURL再読み込み
        public static RoutedCommand UrlReload = new RoutedCommand();

        void UrlReloadCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _urlEntities.Count<UrlEntity>() > 0 &&
                IsWebViewValid() && !_isNavigating;
        }

        void UrlReloadExecute(object target, ExecutedRoutedEventArgs e)
        {
            int crIndex = urlComboBox.SelectedIndex;
            string crUrl = _urlEntities[crIndex].pageUrl;
            webView.CoreWebView2.Navigate(crUrl);
        }

        //次のURLに進む
        public static RoutedCommand UrlNext = new RoutedCommand();

        void UrlNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _urlEntities.Count<UrlEntity>() > 0 && 
                urlComboBox.SelectedIndex < (_urlEntities.Count<UrlEntity>() - 1);
        }

        void UrlNextExecuted(object target, ExecutedRoutedEventArgs e)
        {
            int crIndex = urlComboBox.SelectedIndex;
            crIndex++;
            urlComboBox.SelectedIndex = crIndex;
        }

        //前のURLに戻る
        public static RoutedCommand UrlPrev = new RoutedCommand();

        void UrlPrevCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _urlEntities.Count<UrlEntity>() > 0 && urlComboBox.SelectedIndex > 0;
        }

        void UrlPrevExecuted(object target, ExecutedRoutedEventArgs e)
        {
            int crIndex = urlComboBox.SelectedIndex;
            crIndex--;
            urlComboBox.SelectedIndex = crIndex;
        }

        //Urlファイル読込
        public static RoutedCommand LoadFile = new RoutedCommand();

        void LoadFileCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void LoadFileCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            _urlEntities = new ObservableCollection<UrlEntity>();
            FileRepository fr = new FileRepository();
            _urlEntities = fr.GetUrlEntities();
            urlComboBox.ItemsSource = _urlEntities;
            urlComboBox.SelectedIndex = 0;
        }

        //UrlComboBoxChangedイベントの処理
        public static RoutedCommand UrlComboBoxChanged = new RoutedCommand();

        void UrlComboBoxChangedCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _urlEntities.Count<UrlEntity>() > 0;
        }

        async void UrlComboBoxChangedExecute(object target, ExecutedRoutedEventArgs e)
        {
            //UrlComboBoxが初期化された場合はリターン
            if (urlComboBox.SelectedIndex == -1) return;

            var crUrl = _urlEntities[urlComboBox.SelectedIndex].pageUrl;
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Navigate(crUrl);
            url.Text = crUrl;
        }


        //CssCutシミュレーション
        public static RoutedCommand SimCssCut = new RoutedCommand();

        async void SimCssCutExecute(object target, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.css_cut());
        }


        //ImageAltシミュレーション
        public static RoutedCommand SimImageAlt = new RoutedCommand();

        async void SimImageAltExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.image_alt());
        }

        //Target属性シミュレーション
        public static RoutedCommand SimTargetBlank = new RoutedCommand();

        async void SimTargetBlankExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.target_attr());
        }

        //Structシミュレーション
        public static RoutedCommand SimStruct = new RoutedCommand();

        async void SimStructExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.semantic_check());
        }

        //lang属性シミュレーション
        public static RoutedCommand SimLangAttr = new RoutedCommand();

        async void SimLangAttrExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.lang_attr());
        }

        //label/titleシミュレーション
        public static RoutedCommand SimLabelTitleAttr = new RoutedCommand();

        async void SimLabelTitleAttrExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.tag_form_and_title_attr());
        }

        //文書リンクシミュレーション
        public static RoutedCommand SimDocLink = new RoutedCommand();

        async void SimDocLinkExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.document_link());
        }

        //superfocusシミュレーション
        public static RoutedCommand SimSuperFocus = new RoutedCommand();

        async void SimSuperFocusExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.super_focus());
        }

        //wai-ariaシミュレーション
        public static RoutedCommand SimAriaAttr = new RoutedCommand();

        async void SimAriaAttrExecute(object sender, ExecutedRoutedEventArgs e)
        {
            await webView.ExecuteScriptAsync(PresvUtil.wai_aria_attr());
        }

        //全シミュレーションの実行可否
        void SimAllCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsWebViewValid() && !_isNavigating;
        }

        //UserAgent変更シミュレーション
        public static RoutedCommand SetUserAgent = new RoutedCommand();

        void SetUserAgentExecute(object target, ExecutedRoutedEventArgs e)
        {
            //var dialog = new TextInputDialog(
            //    title: "SetUserAgent",
            //    description: "Enter UserAgent");
            //if (dialog.ShowDialog() == true)
            //{
            //    WebViewSettings.UserAgent = dialog.Input.Text;
            //}
            if(_isUserAgentCustomized == false)
            {
                _defaultUserAgent = WebViewSettings.UserAgent;
                WebViewSettings.UserAgent = CommonValue.GetUserAgentIOS();
                _isUserAgentCustomized = true;
                webView.Reload();
            }
            else
            {
                WebViewSettings.UserAgent = _defaultUserAgent;
                _isUserAgentCustomized = false;
                webView.Reload();
            }
        }


        //ブラウザ設定系コマンド実行可否
        void CoreWebView2RequiringCmdsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsWebViewValid();
        }


        //webViewインスタンスの有効判定
        bool IsWebViewValid()
        {
            try
            {
                return webView != null && webView.CoreWebView2 != null;
            }
            catch(Exception ex) when(ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                return false;
            }
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

        //ブラウザの基本コマンド
        void BackCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && webView.CanGoBack;
        }

        void BackCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.GoBack();
        }

        void ForwardCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && webView.CanGoForward;
        }

        void ForwardCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.GoForward();
        }

        void RefreshCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsWebViewValid() && !_isNavigating;
        }

        void RefreshCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.Reload();
        }

        void BrowseStopCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsWebViewValid() && _isNavigating;
        }

        void BrowseStopCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            webView.Stop();
        }

        void GoToPageCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webView != null && !_isNavigating;
        }

        async void GoToPageCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async();

            var rawUrl = (string)e.Parameter;
            Uri uri = null;

            if(Uri.IsWellFormedUriString(rawUrl, UriKind.Absolute))
            {
                uri = new Uri(rawUrl);
            }
            else if(!rawUrl.Contains(" ") && rawUrl.Contains("."))
            {
                uri = new Uri("http://" + rawUrl);
            }
            else
            {
                uri = new Uri("https://google.co,jp/search?=hl=ja&q=" +
                    String.Join("&", Uri.EscapeDataString(rawUrl).Split(new string[] { "%20" }, StringSplitOptions.RemoveEmptyEntries)));
            }

            webView.CoreWebView2.Navigate(uri.ToString());
        }

    }
}
