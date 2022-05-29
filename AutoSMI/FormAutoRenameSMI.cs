using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PlyaerSMIAuto
{
    public partial class FormAutoRenameSMI : Form
    {
        public FormAutoRenameSMI()
        {
            InitializeComponent();
        }

        void TextBox_Click(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    tb.Text = ofd.FileName;
                }
            }
        }

        void ButtonRename_Click(object sender, EventArgs e)
        {
            string strVideo = textBoxVideo.Text;
            string strSubTitle = textBoxSubTitle.Text;

            if (string.IsNullOrWhiteSpace(strVideo) || string.IsNullOrWhiteSpace(strSubTitle))
            {
                return;
            }
            if (strVideo == strSubTitle)
            {
                return;
            }

            string[] playList = GetFile(strVideo);
            string[] subList = GetFile(strSubTitle);
            if (playList == null || playList.Length <= 0 || subList == null || subList.Length <= 0)
            {
                return;
            }

            if (playList.Length != subList.Length)
            {
                if (MessageBox.Show("파일 수 다름 실행?", "확인", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            RunRename(playList, subList);
        }

        /// <summary>
        /// 동영상 파일 이름으로 자막 파일 이름 변경 <para></para>
        /// 1:1 순서대로 진행
        /// </summary>
        /// <param name="playList"></param>
        /// <param name="subList"></param>
        void RunRename(string[] playList, string[] subList)
        {
            string folder = Path.GetDirectoryName(subList[0]);

            string subExtension = Path.GetExtension(subList[0]);

            int maxCount = playList.Length > subList.Length ? subList.Length : playList.Length;

            for (int index = 0; index < maxCount; index++)
            {
                //동영상 파일 명 확장자 없이 가져오기
                string playName = Path.GetFileNameWithoutExtension(playList[index]);

                try
                {
                    File.Move(subList[index], folder + "\\" + playName + subExtension);
                }
                catch
                {
                }
            }
        }


        /// <summary>
        /// 탐색기 정렬과 동일하게 가져오기
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        string[] GetFile(string fullName)
        {
            string extension = Path.GetExtension(fullName);
            string[] list = Directory.GetFiles(Path.GetDirectoryName(fullName), "*" + extension, SearchOption.TopDirectoryOnly);
            if (list != null && list.Length >= 2)
            {
                Array.Sort(list, new MyComparer());
            }
            return list;
        }

        public class MyComparer : IComparer<string>
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            static extern int StrCmpLogicalW(String x, String y);

            public int Compare(string x, string y)
            {
                return StrCmpLogicalW(x, y);
            }
        }





    }
}
