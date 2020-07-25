﻿using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewWiki.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PartialViewWiki
{
    /// <summary>
    /// KnowledgeWiki.xaml 的交互逻辑
    /// </summary>
    public partial class KnowledgeWiki : UserControl, IPartialView
    {
        private KnowledgeWikiViewModel viewModel;

        public string MenuName
        {
            get { return "知识百科"; }
        }

        public string TagName
        {
            get { return "KnowledgeWiki"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.None; }
        }

        public KnowledgeWiki()
        {
            InitializeComponent();
            viewModel = new KnowledgeWikiViewModel();
            DataContext = viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var knowledge in viewModel.Knowledges)
            {
                Button button = new Button();
                button.Content = knowledge.Knowledge;
                button.Width = 160;
                button.Height = 40;
                button.Margin = new Thickness(10, 5, 10, 5);
                button.Name = "btn" + knowledge.Id.Replace("-", "");
                button.Tag = knowledge.Id.ToString();
                Binding binding = new Binding()
                {
                    Source = knowledge,
                    Path = new PropertyPath("Visibility"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(button, Button.VisibilityProperty, binding);
                ButtonHelper.SetButtonStyle(button, ButtonStyle.Hollow);
                ButtonHelper.SetClickStyle(button, ClickStyle.Sink);
                ButtonHelper.SetCornerRadius(button, new CornerRadius(4));
                ButtonHelper.SetHoverBrush(button, "#00BCFF".ToColor().ToBrush());
                button.BorderBrush = "#20A0FF".ToColor().ToBrush();
                button.Foreground = "#20A0FF".ToColor().ToBrush();
                button.Click += Button_Click;
                this.content.Children.Add(button);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = BaseDirectoryPath + "plugs\\Gif\\" + button.Tag + ".gif";
            if (File.Exists(path))
            {

                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                DemonstrateWindow window = new DemonstrateWindow(path, button.Content.ToString());
                window.WindowState = WindowState.Maximized;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                (Application.Current.MainWindow as WindowX).IsMaskVisible = true;
                window.ShowDialog();
                (Application.Current.MainWindow as WindowX).IsMaskVisible = false;
            }
            else
            {
                MessageBoxHelper.MessageBoxShowWarning("文件不存在！");
            }
        }
    }
}
