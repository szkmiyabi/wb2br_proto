using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace wb2br_proto
{
    /// <summary>
    /// SettingsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsDialog : Window
    {
        private Settings appSettings;
        private string filename;

        public SettingsDialog()
        {
            InitializeComponent();
            filename = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\settings.config";
            appSettings = new Settings();
            LoadSettings();
        }

        //環境設定をロードする
        private void LoadSettings()
        {
            try
            {
                XmlSerializer xsz = new XmlSerializer(typeof(Settings));
                StreamReader sr = new StreamReader(
                    filename,
                    new System.Text.UTF8Encoding(false)
                );
                appSettings = (Settings)xsz.Deserialize(sr);
                sr.Close();

                IEPathText.Text = appSettings.iePath;
                FirefoxPathText.Text = appSettings.ffPath;
                ChromePathText.Text = appSettings.gcPath;
                EtcBrowserPathText.Text = appSettings.etcBrowserPath;
            }
            catch (Exception ex)
            {
            }
        }

        //環境設定を保存する
        private void saveSettings()
        {
            try
            {
                appSettings.iePath = IEPathText.Text;
                appSettings.ffPath = FirefoxPathText.Text;
                appSettings.gcPath = ChromePathText.Text;
                appSettings.etcBrowserPath = EtcBrowserPathText.Text;
                XmlSerializer xsz = new XmlSerializer(typeof(Settings));
                StreamWriter sw = new StreamWriter(
                    filename,
                    false,
                    new System.Text.UTF8Encoding(false)
                );
                xsz.Serialize(sw, appSettings);
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("設定が保存でませんでした。" + ex.Message);
            }

        }

        //環境設定を削除する
        private void DeleteSettings()
        {
            try
            {
                appSettings.iePath = "";
                appSettings.ffPath = "";
                appSettings.gcPath = "";
                appSettings.etcBrowserPath = "";
                XmlSerializer xsz = new XmlSerializer(typeof(Settings));
                StreamWriter sw = new StreamWriter(
                    filename,
                    false,
                    new System.Text.UTF8Encoding(false)
                );
                xsz.Serialize(sw, appSettings);
                sw.Close();

                IEPathText.Text = "";
                FirefoxPathText.Text = "";
                ChromePathText.Text = "";
                EtcBrowserPathText.Text = "";
                MessageBox.Show("設定が削除できました。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("設定が削除できませんでした。" + ex.Message);
            }
        }

        //IE起動パスを取得
        private void iePathDefaultLoad()
        {
            string iepath = "";
            string iepath1 = @"C:\Program Files\Internet Explorer\iexplore.exe";
            string iepath2 = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe";
            if (System.IO.File.Exists(iepath1)) iepath = iepath1;
            else if (System.IO.File.Exists(iepath2)) iepath = iepath2;
            if (iepath == "") MessageBox.Show("取得できません");
            else IEPathText.Text = iepath;
        }

        //Firefox起動パスを取得
        private void ffPathDefaultLoad()
        {
            string ffpath = "";
            string ffpath1 = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            string ffpath2 = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            if (System.IO.File.Exists(ffpath1)) ffpath = ffpath1;
            else if (System.IO.File.Exists(ffpath2)) ffpath = ffpath2;
            if (ffpath == "") MessageBox.Show("取得できません");
            else FirefoxPathText.Text = ffpath;
        }

        //Chrome起動パスを取得
        private void gcPathDefaultLoad()
        {
            string gcpath = "";
            string gcpath1 = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            string gcpath2 = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            string gcpath3 = getUserHomePath() + @"\Local Settings\Application Data\Google\Chrome\Application\chrome.exe";
            if (System.IO.File.Exists(gcpath1)) gcpath = gcpath1;
            else if (System.IO.File.Exists(gcpath2)) gcpath = gcpath2;
            else if (System.IO.File.Exists(gcpath3)) gcpath = gcpath3;
            if (gcpath == "") MessageBox.Show("取得できません");
            else ChromePathText.Text = gcpath;

        }

        //ユーザのホームフォルダパス
        private string getUserHomePath()
        {
            return System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private void ieDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            iePathDefaultLoad();
        }

        private void ffDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            ffPathDefaultLoad();
        }

        private void gcDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            gcPathDefaultLoad();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteSettingButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSettings();
        }
    }
}
