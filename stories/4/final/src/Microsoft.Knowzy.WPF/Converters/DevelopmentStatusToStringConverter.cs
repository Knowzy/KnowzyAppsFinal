﻿// ******************************************************************

// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

// ******************************************************************

using Microsoft.Knowzy.Domain.Enums;
using Microsoft.Knowzy.WPF.Localization;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Knowzy.WPF.Converters
{
    public class DevelopmentStatusToStringConverter : IValueConverter
    {
        public const string DevelopmentStatusResourcePrefix = "DevelopmentStatus_";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || !(value is DevelopmentStatus))
                    ? string.Empty
                    : Resources.ResourceManager.GetString($"{DevelopmentStatusResourcePrefix}{value}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
