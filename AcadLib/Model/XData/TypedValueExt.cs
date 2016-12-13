﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using NetLib;

namespace AcadLib
{
    public static class TypedValueExt
    {
        public static T GetValue<T>(this Dictionary<string, object> dictValues, string name, T defaultValue)
        {
            object value;
            if (dictValues.TryGetValue(name, out value))
            {
                try
                {
                    return value.GetValue<T>();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"TypedValueExt, GetValue из словаря значений - по имени параметра '{name}'. object = {value.ToString()} тип {value.GetType()}");
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public static Dictionary<string, object> ToDictionary(this IEnumerable<TypedValue> values)
        {
            var dictValues = new Dictionary<string, object>();
            string name = string.Empty;            
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    dictValues.Add(name, item.Value);
                    name = string.Empty;
                }
                else
                {
                    name = item.GetTvValue<string>();
                }
            }
            return dictValues;
        }

        /// <summary>
        /// Возвращает значение TypedValue
        /// Типы - int, bool, byte, double, string - те которые возможны в TypedValue DxfCode
        /// Не проверяется соответствие типа значения и номера кода DxfCode !!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tv"></param>
        /// <returns></returns>
        public static T GetTvValue<T> (this TypedValue tv)
        {
            T res;
            try
            {
                var value = tv.Value;
                if (typeof(T) == typeof(bool))
                {
                    // 0 - false, иначе true
                    value = (int)tv.Value == 0 ? false : true;
                }
                res = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                res = default(T);
            }
            return res;
        }

        /// <summary>
        /// Создание TypedValue для сохранение в расширенные данные DxfCode.ExtendedData
        /// bool, byte, int, double, string
        /// </summary>        
        public static TypedValue GetTvExtData(object value)
        {
            var typeObj = value.GetType();

            int code = 0;
            object tvValue = value;

            if (typeObj == typeof(bool))
            {
                code = (int)DxfCode.ExtendedDataInteger32;
                tvValue = (bool)value ? 1 : 0;
            }
            else if (typeObj == typeof (int) || typeObj.IsEnum)
            {
                code = (int)DxfCode.ExtendedDataInteger32;
                tvValue = (int)value;
            }
            else if (typeObj == typeof(byte))
            {
                code = (int)DxfCode.ExtendedDataInteger32;                
            }
            else if (typeObj == typeof(double))
            {
                code = (int)DxfCode.ExtendedDataReal;
            }
            else if (typeObj == typeof(string))
            {
                code = (int)DxfCode.ExtendedDataAsciiString;                
            }

            if (code == 0)
                throw new Exception("Invalid TypedValue");

            return new TypedValue(code, tvValue);
        }
    }
}
