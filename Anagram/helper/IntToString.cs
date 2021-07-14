﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Anagram.helper
{
    public class IntToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return value.ToString(); }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.TryParse((string) value, out int ret) ? ret : 0;
        }
    }
}