﻿using MaterialDesignThemes.Wpf;
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
using Tag.Core.Cue;
using Tag.Core.Tagging;
using Tag.Setting;

namespace Tag.WPF
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class GetTagInfo : UserControl
    {
        public GetTagInfo(TagInfo SearchInfo, ObservableCollection<TaggingModel> user)
        {
            InitializeComponent();
            this.SearchInfo = SearchInfo;
            this.userinfo = user;
        }

        TagInfo SearchInfo;
        ObservableCollection<TaggingModel> userinfo;

        void OpenBrainzEvent(object sender, DialogOpenedEventArgs e)
        {
            brainz.Search(SearchInfo);
        }
        void OpenVgmEvent(object sender, DialogOpenedEventArgs e)
        {
            vgm.Search(SearchInfo);
        }

        MusicBrainzSearch brainz;
        VgmDbSearch vgm;

        private async void MusicBrainz_Search(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(false, null);

            brainz = new MusicBrainzSearch(userinfo)
            {
                Width = 800,
                Height = 280,

            };
            var t = await DialogHost.Show(brainz, Global.DialogIdentifier.BrainzSearch, OpenBrainzEvent);
        }

        private async void VGMDB_Search(object sender, RoutedEventArgs e)
        {
            vgm = new VgmDbSearch(userinfo)
            {
                Width = 800,
                Height = 280,
            };

            DialogHost.CloseDialogCommand.Execute(false, null);

            await DialogHost.Show(vgm, Global.DialogIdentifier.VgmSearch, OpenVgmEvent);

        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                DialogHost.CloseDialogCommand.Execute(false, null);
                Global.DialogIdentifier.TaggingEnable = true;
            }
        }
    }
}
