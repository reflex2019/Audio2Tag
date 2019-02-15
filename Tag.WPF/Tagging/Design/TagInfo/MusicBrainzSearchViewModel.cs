﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tag.Core.Cue;
using Tag.Core.Tagging;
using Tag.Core.Tagging.Library;

namespace Tag.WPF
{
    class MusicBrainzSearchViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<BrainzInfo> Items { get; private set; }
        public ImageSource ImageSource { get => _image; private set { _image = value; OnPropertyChanged(); } }
        List<TrackInfo> user;


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string Name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public MusicBrainzSearchViewModel(List<TrackInfo> users)
        {
            Items = new ObservableCollection<BrainzInfo>();
            search = new MusicBrain();
            this.user = users;
        }

        MusicBrain search;
        private ImageSource _image;

        public void SearchAlbum(TagInfo SearchInfo)
        {
            foreach (var value in search.GetAlbumInfo(SearchInfo))
            {
                Items.Add(value);
            }
        }

        public async void Yes_Click(int index)
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
            await Task.Delay(500);

            CheckTagging view = new CheckTagging
            {

            };
            view.SetValue(search.GetTrackInfo(Items[index]), user);
            await DialogHost.Show(view);
        }



        int TaskIdentified = 0;
        public async void SelectItem(int index, Control control)
        {
            await Task.Run(() =>
            {
                TaskIdentified++;
                int id = TaskIdentified;
                var bin = search.GetTrackInfo(Items[index])[0].Image[0].Data.Data;
                if (id == TaskIdentified)
                {
                    control?.Dispatcher?.Invoke(() =>
                    {
                        var image = new Bitmap(System.Drawing.Image.FromStream(new MemoryStream(bin)));
                        var data = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                             image.GetHbitmap(),
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
                        ImageSource = data;
                    });
                }
            });
        }

    }
}