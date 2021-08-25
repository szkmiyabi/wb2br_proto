using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace wb2br_proto
{
    public class FileRepository
    {
        //UrlEntityのコレクション
        private ObservableCollection<UrlEntity> _urlEntities;

        //コンストラクタ
        public FileRepository()
        {
            _urlEntities = new ObservableCollection<UrlEntity>();
            Initialized();
        }

        //UrlEntityの初期化
        private void Initialized()
        {
            try
            {
                OpenFileDialog f = new OpenFileDialog();
                f.Filter = "テキストファイル(*.txt)|*.txt";
                bool? result = f.ShowDialog();
                if (result == true)
                {
                    string filepath = f.FileName;
                    StreamReader sr = new StreamReader(filepath, System.Text.Encoding.GetEncoding("UTF-8"));
                    string text = sr.ReadToEnd();
                    sr.Close();
                    char[] delimiter = { '\t', ',' };
                    StringReader line_sr = new StringReader(text);
                    while (line_sr.Peek() > -1)
                    {
                        string line = line_sr.ReadLine();
                        string[] tmp = line.Split(delimiter);
                        _urlEntities.Add(new UrlEntity { pageId = tmp[0], pageUrl = tmp[1] });
                    }
                }
            }
            catch(Exception ex){}
        }

        //UrlEntityコレクションを取得
        public ObservableCollection<UrlEntity> GetUrlEntities()
        {
            return _urlEntities;
        }

    }

    //UrlEntityインラインクラス
    public sealed class UrlEntity
    {
        public string pageId { get; set; }
        public string pageUrl { get; set; }
    }
}
