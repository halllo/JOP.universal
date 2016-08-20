﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JustObjectsPrototype.Universal.Shell
{
	public class NullIsVisible : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

	public class FalseIsVisible : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((bool)value == false)
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

	public class TrueIsVisible : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((bool)value == true)
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
