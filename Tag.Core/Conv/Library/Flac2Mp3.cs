﻿using NAudio.Flac;
using NAudio.Lame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tag.Core.Conv.Library
{
    class Flac2Mp3 : IConv
    {
        private IEnumerable<int> Execute(string filePath, string resultPath, LAMEPreset preset = LAMEPreset.ABR_320)
        {
            int percent = 0;
            using (var flac = new FlacReader(filePath))
            {
                if (!Directory.Exists(resultPath))
                {
                    Directory.CreateDirectory(resultPath);
                }
                using (var mp3 = new LameMP3FileWriter($"{resultPath}\\{Path.GetFileNameWithoutExtension(filePath)}.mp3", flac.WaveFormat, preset))
                {
                    byte[] buffer = new byte[flac.WaveFormat.BlockAlign * 100];
                    while (flac.Position < flac.Length)
                    {
                        int bytesRequired = (int)(flac.Length - flac.Position);
                        if (bytesRequired > 0)
                        {
                            int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                            int bytesRead = flac.Read(buffer, 0, bytesToRead);
                            if (bytesRead > 0)
                            {
                                mp3.Write(buffer, 0, bytesRead);
                            }
                        }

                        var value = (int)(flac.Position * 100.0 / flac.Length);
                        if (percent == value)
                        {
                            continue;
                        }
                        if (percent != value)
                        {
                            percent = value;
                            yield return percent;
                        }
                    }
           //         flac.CopyTo(mp3);
                }
            }
            yield return 100;
        }

        public IEnumerable<int> Execute(ConvInfo info)
        {
            LAMEPreset preset = new LAMEPreset();

            foreach (var value in info.Parameter)
            {
                if (value is LAMEPreset)
                {
                    preset |= (LAMEPreset)value;
                }
            }
            

            foreach (var value in Execute(info.FilePath, info.ResultPath, preset == 0 ? LAMEPreset.ABR_320 : preset))
            {
                yield return value;
            }
        }

    }
}
