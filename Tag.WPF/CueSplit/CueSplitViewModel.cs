﻿
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Tag.WPF
{
    public class CueSplitViewModel : INotifyPropertyChanged
    {
        public string CueFileOpen { get; private set; } = Setting.Global.Language.CueFileOpen;
        public string CueFileSplitExecute { get; private set; } = Setting.Global.Language.CueFileSplitExecute;

        public string AlbumTitle { get => albumTitle; private set { albumTitle = value; OnPropertyChanged(); } }
        public string Barcode { get => barcode; private set { barcode = value; OnPropertyChanged(); } }
        public int AvgBytePerSecond { get => avgBytePerSecond; private set { avgBytePerSecond = value; OnPropertyChanged(); }  }
        public string Genre { get => genre; private set { genre = value; OnPropertyChanged(); } }

        int _TaskPercent = 0;
        public int TaskPercent
        {
            get { return _TaskPercent; }
            set
            {
                _TaskPercent = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CueSplitModel> Items { get; set; }

        private readonly Tag.Core.Cue.CueSpliter cueSpliter;
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string Name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public CueSplitViewModel()
        {
            Items = new ObservableCollection<CueSplitModel>();
            cueSpliter = new Core.Cue.CueSpliter();
            Items.Add(new CueSplitModel
            {
                Title = "제목",
                Artist = "아티스트",
                DurationMS = -1,
                TimeOffSet = -1
            });
        }

        void Change(int index, CueSplitModel newdata)
        {
            Items.RemoveAt(index);
            Items.Insert(index, newdata);
        }

        public void Click(int index)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var data = Items[i];
                if (i == index)
                {
                    data.IsSelect = true;
                }
                else
                {
                    data = Items[i];
                    data.IsSelect = false;
                }
                Change(i, data);
            }
        }

        public void AddFile(string filePath)
        {
            Items.Clear();
            cueSpliter.List().Clear();
            cueSpliter.AddFile(filePath);
            int index = 1;

            AlbumTitle = cueSpliter[0].Track[0].Album;
            Barcode = cueSpliter[0].Barcode ?? cueSpliter[0].REM.DiscId;
            AvgBytePerSecond = cueSpliter[0].WaveFormat.AverageBytesPerSecond;
            Genre = cueSpliter[0].REM.Genre;

            Items.Add(new CueSplitModel
            {
                Title = "제목",
                Artist = "아티스트",
                DurationMS = -1,
                TimeOffSet = -1
            });

            foreach (var value in cueSpliter[0].Track)
            {
                CueSplitModel model = new CueSplitModel(value)
                {
                    Index = index
                };
                Items.Add(model);
                index++;
            }
        }
        delegate bool Test();
        bool isTask = false;
        private string albumTitle;
        private string barcode;
        private int avgBytePerSecond;
        private string genre;

        public async Task Execute()
        {
            await Task.Factory.StartNew(() =>
            {
                if (isTask == false)
                {
                    TaskPercent = 0;
                    isTask = true;
                    foreach (var value in cueSpliter.Execute())
                    {
                        if (TaskPercent < value)
                        {
                            TaskPercent = value;
                        }
                    }
                    isTask = false;
                }

            }).ConfigureAwait(true);

        }
    }
}