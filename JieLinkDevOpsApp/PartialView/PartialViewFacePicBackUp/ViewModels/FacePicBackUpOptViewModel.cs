﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PartialViewInterface.Commands;
using System.Diagnostics;
using PartialViewFacePicBackUp;
using System.Configuration;
using PartialViewInterface;
using System.Windows;

namespace PartialViewFacePicBackUp.ViewModels
{
    public class FacePicBackUpOptViewModel : DependencyObject
    {
        public FacePicBackUpOptViewModel()
        {
            FacePicBackUpCommand = new DelegateCommand();
            FacePicBackUpCommand.ExecuteAction = FacePicBackUp;
            SelectPathCommand = new DelegateCommand();
            SelectPathCommand.ExecuteAction = SelectPath;
            CheckPicCommand = new DelegateCommand();
            CheckPicCommand.ExecuteAction = CheckPic;

            ReadFileServerPath();
        }

        /// <summary>
        /// 选择路径
        /// </summary>
        public DelegateCommand SelectPathCommand { get; set; }
        /// <summary>
        /// 检测人脸图片
        /// </summary>
        public DelegateCommand CheckPicCommand { get; set; }


        /// <summary>
        /// 委托
        /// </summary>
        public delegate void DeleFun(string str);
        /// <summary>
        /// 命令=>调用
        /// </summary>
        public static event DeleFun DeleEvent;
        public static event DeleFun DeleEventShowWarn;

        /// <summary>
        /// 执行备份
        /// </summary>
        public DelegateCommand FacePicBackUpCommand { get; set; }

        /// <summary>
        /// 统计人事备份情况
        /// </summary>
        public static int CountPersonAll = 0;
        public static int CountPersonNotExists = 0;
        public static int CountPersonSuccess = 0;
        public static int CountPersonFail = 0;
        public static int CountFeatureAll = 0;
        public static int CountFeatureNotExists = 0;
        public static int CountFeatureSuccess = 0;
        public static int CountFeatureFail = 0;

        /// <summary>
        /// 文件服务器地址
        /// </summary>
        public string FileServerPath = string.Empty;
        /// <summary>
        /// 根据路径备份
        /// </summary>
        /// <param name="parameter"></param>
        public void FacePicBackUp(object parameter)
        {
            try
            {
                if (FilePath == "")
                {
                    DeleEventShowWarn("请选择有效的备份路径！");
                    return;
                }

                #region 初始化变量
                CountPersonAll = 0;
                CountPersonNotExists = 0;
                CountPersonSuccess = 0;
                CountPersonFail = 0;
                CountFeatureAll = 0;
                CountFeatureNotExists = 0;
                CountFeatureSuccess = 0;
                CountFeatureFail = 0;
                #endregion

                //读取人事资料
                string sqlstr = "select photopath,personno from control_person where photopath is not null";
                MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                while (reader.Read())
                {
                    CountPersonAll++;
                    string photopath = reader["photopath"].ToString();
                    int personno = Convert.ToInt32(reader["personno"].ToString());
                    CopyPersonFile(photopath, personno, true);
                }

                //读取人脸特征
                string sqlstrFeature = "select cpf.feature,cp.personno from control_person_face cpf inner join control_person cp on cpf.pguid = cp.pguid and cpf.feature is not null";
                MySqlDataReader Featurereader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstrFeature);
                while (Featurereader.Read())
                {
                    CountFeatureAll++;
                    string photopath = Featurereader["feature"].ToString();
                    int personno = Convert.ToInt32(Featurereader["personno"].ToString());
                    CopyPersonFile(photopath, personno, false);
                }

                DeleEvent(string.Format("获取到的文件服务器路径为：{0}", FileServerPath));
                DeleEvent(string.Format("备份人事图片共{0}个，其中成功{1}个，人事图片不存在{2}个，失败{3}个", CountPersonAll, CountPersonSuccess, CountPersonNotExists, CountPersonFail));
                DeleEvent(string.Format("备份人脸特征共{0}个，其中成功{1}个，特征图片不存在{2}个，失败{3}个", CountFeatureAll, CountFeatureSuccess, CountFeatureNotExists, CountFeatureFail));
            }
            catch (Exception ex)
            {
                DeleEventShowWarn(ex.ToString());
            }
        }


        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(FacePicBackUpOptViewModel), new PropertyMetadata(""));

        string defaultfilePath = "";
        /// <summary>
        /// 路径
        /// </summary>
        /// <param name="parameter"></param>
        private void SelectPath(object parameter)
        {
            System.Windows.Forms.FolderBrowserDialog fileDialog = new System.Windows.Forms.FolderBrowserDialog();

            var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
            if (process != null)
            {
                fileDialog.SelectedPath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, "para");
            }
            if (!string.IsNullOrEmpty(defaultfilePath))
            {
                fileDialog.SelectedPath = defaultfilePath;
            }

            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                defaultfilePath = fileDialog.SelectedPath;

                FilePath = fileDialog.SelectedPath;
            }
        }

        /// <summary>
        /// 检测图片是否存在
        /// </summary>
        /// <param name="parameter"></param>
        private void CheckPic(object parameter)
        {
            try
            {
                DeleEvent("开始检测人脸图片完整性！");

                int CheckPersonAll = 0;
                int CheckPersonNotExist = 0;
                int CheckFeatureAll = 0;
                int CheckFeatureNotExist = 0;

                //读取人事资料
                string sqlstr = "select photopath,personno from control_person where photopath is not null";
                MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                while (reader.Read())
                {
                    string photopath = reader["photopath"].ToString();
                    int personno = Convert.ToInt32(reader["personno"].ToString());
                    CheckPersonAll++;

                    if (!File.Exists(photopath.Replace("down", FileServerPath)))
                    {
                        CheckPersonNotExist++;
                        DeleEvent(string.Format("人事编号为{0}的人脸图片不存在！", personno));
                    }
                }

                //读取人脸特征
                string sqlstrFeature = "select cpf.feature,cp.personno from control_person_face cpf inner join control_person cp on cpf.pguid = cp.pguid and cpf.feature is not null";
                MySqlDataReader Featurereader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstrFeature);
                while (Featurereader.Read())
                {


                    CheckFeatureAll++;
                    string photopath = Featurereader["feature"].ToString();
                    int personno = Convert.ToInt32(Featurereader["personno"].ToString());

                    #region 人脸特征文件夹路径日期加一个/
                    if (photopath.Split('/')[1] == "pic" && photopath.Split('/')[2].Length == 8)
                        photopath = photopath.Insert(photopath.IndexOf("pic") + 10, "/");
                    else
                        DeleEvent("人脸特征路径解析有误！");
                    #endregion

                    if (!File.Exists(photopath.Replace("down", FileServerPath)))
                    {
                        CheckFeatureNotExist++;
                        DeleEvent(string.Format("人事编号为{0}的人脸特征不存在！", personno));
                    }
                }

                DeleEvent(string.Format("检测人事图片共{0}个，其中图片不存在{1}个", CheckPersonAll, CheckPersonNotExist));
                DeleEvent(string.Format("检测人脸特征共{0}个，其中特征不存在{1}个", CheckFeatureAll, CheckFeatureNotExist));

            }
            catch (Exception ex)
            {
                DeleEventShowWarn(ex.ToString());
            }
        }

        /// <summary>
        /// 复制文件到备份路径
        /// </summary>
        /// <param name="sourcePath">数据库中的文件路径</param>
        public void CopyPersonFile(string sourcePath, int personno, bool IsPerson)
        {
            try
            {
                #region 人脸特征文件夹路径日期加一个/
                if (!IsPerson)
                {
                    if (sourcePath.Split('/')[1] == "pic" && sourcePath.Split('/')[2].Length == 8)
                        sourcePath = sourcePath.Insert(sourcePath.IndexOf("pic") + 10, "/");
                    else
                        DeleEvent("人脸特征路径解析有误！");
                }
                #endregion

                sourcePath = sourcePath.Replace("/", "\\");

                //完整目标文件夹路径
                string FactSavePath = Path.GetDirectoryName(FilePath + "\\FileSavePath" + sourcePath.Replace("down", ""));
                //完整源文件路径
                string FaceSourcePath = sourcePath.Replace("down", FileServerPath);
                //完整目标文件路径
                string FaceDestPath = FilePath + "\\FileSavePath" + sourcePath.Replace("down", "");
                //如果指定的目标文件夹路径不存在，则创建该存储路径
                if (!Directory.Exists(FactSavePath))
                {
                    //创建
                    Directory.CreateDirectory(FactSavePath);
                }

                #region 保存文件
                if (!File.Exists(FaceSourcePath))
                {
                    if (IsPerson)
                    {
                        CountPersonNotExists++;
                        DeleEvent(string.Format("警告：人事编号为{0}的图片不存在！", personno));
                    }
                    else
                    {
                        CountFeatureNotExists++;
                        DeleEvent(string.Format("警告：人事编号为{0}的特征文件不存在！", personno));
                    }
                    return;
                }

                if (File.Exists(FaceDestPath))
                {
                    if (IsPerson)
                    {
                        CountPersonSuccess++;
                        DeleEvent(string.Format("备份人事编号为{0}的图片时，目标文件已存在", personno));
                    }
                    else
                    {
                        CountFeatureSuccess++;
                        DeleEvent(string.Format("备份人事编号为{0}的特征文件时，目标文件已存在", personno));
                    }
                }
                else
                {
                    File.Copy(FaceSourcePath, FaceDestPath, false);
                    if (IsPerson)
                    {
                        CountPersonSuccess++;
                        DeleEvent(string.Format("备份人事编号为{0}的图片成功", personno));
                    }
                    else
                    {
                        CountFeatureSuccess++;
                        DeleEvent(string.Format("备份人事编号为{0}的特征文件成功", personno));
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                //如果一个文件无法正常备份那么所有的都会无法正常备份，只弹窗提示一次报错
                if (CountFeatureFail == 0 && CountPersonFail == 0)
                    DeleEventShowWarn(ex.ToString());

                if (IsPerson) CountPersonFail++;
                else CountFeatureFail++;
            }
        }

        /// <summary>
        /// 读取文件服务器中文件存储路径到FileServerPath
        /// </summary>
        private void ReadFileServerPath()
        {
            #region 根据门禁服务进程或者中心进程获取文件服务器路径

            string configpath = string.Empty;
            //System.Windows.Forms.FolderBrowserDialog fileDialog = new System.Windows.Forms.FolderBrowserDialog();
            var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
            if (process == null)
            {
                DeleEvent("未发现运行门禁服务");
                process = Process.GetProcessesByName("SmartCenter").FirstOrDefault();
            }

            if (process != null)
            {
                configpath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, @"SmartFile\down\Config\AppSettings.config");
                configpath = configpath.Replace("SmartBoxDoor\\", "");
                configpath = configpath.Replace("SmartCenter\\", "");
            }
            else
            {
                DeleEvent("未发现运行中心服务");
                return;
            }
            #endregion

            #region 根据文件服务器config读取文件存储路径
            FileServerPath = new StreamReader(configpath, Encoding.UTF8).ReadToEnd().Split('\"')[27];
            FileServerPath = FileServerPath.Replace("\\\\", "\\");
            #endregion
        }

    }

}