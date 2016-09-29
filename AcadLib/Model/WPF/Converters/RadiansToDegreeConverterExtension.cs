﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace AcadLib.WPF.Converters
{
    public class RadiansToDegreeConverterExtension : MarkupExtension, IValueConverter
    {        
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            double date = System.Convert.ToDouble(value);
            return date.ToDegrees().ToString("N2");
        }
        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            double date = System.Convert.ToDouble(value);
            return date.ToRadians();
        }
        public override object ProvideValue (IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new RadiansToDegreeConverterExtension();
            return _converter;
        }
        private static RadiansToDegreeConverterExtension _converter = null;
    }
}