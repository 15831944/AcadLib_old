﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace AcadLib.Registry
{
    public class RegExt: IDisposable
    {
        public const string REGAPPPATH = @"Software\Vildar\AutoCAD\";        
        private RegistryKey regKey;        

        public RegExt(string key)
        {            
            regKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGAPPPATH+key);            
        }

        public void Dispose()
        {
            regKey?.Dispose();
        }        

        public string Load(string subkey, string defValue = "")
        {
            return (string)regKey.GetValue(subkey, defValue);
        }

        public bool Load(string subkey, bool defValue = true)
        {
            var value = regKey.GetValue(subkey, defValue);
            return Convert.ToBoolean(value);
        }

        public void Save(string subkey, string value)
        {
            regKey.SetValue(subkey, value, RegistryValueKind.String);
        }
        public void Save(string subkey, bool value)
        {
            regKey.SetValue(subkey, value, RegistryValueKind.DWord);
        }
    }
}