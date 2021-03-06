﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Library
{
    class CapacityManage
    {
        static private readonly string[] Unit = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static public string Change(BigInteger capcity)
        {
            int i = 0;

            for (; ; i += 1)
            {
                if (capcity < 1024 * 2) break;
                capcity /= 1024;
            }

            return $"{capcity} {Unit[i]}";
        }
        static public BigInteger? ReverseChange(BigInteger capacity, String Unit)
        {
            double k = -1;
            for (int i = 0; i < CapacityManage.Unit.Length; i++)
            {
                if (Unit == CapacityManage.Unit[i])
                {
                    k = Math.Pow(1024, i);
                    break;
                }
            }
            if (k == -1)
            {
                // Log.FileWrite("ReverseChange 실패 Capacity : " + capacity + "  Unit : " + Unit, Code.Error);
            }
            return new BigInteger(k) * capacity;
        }
    }
}