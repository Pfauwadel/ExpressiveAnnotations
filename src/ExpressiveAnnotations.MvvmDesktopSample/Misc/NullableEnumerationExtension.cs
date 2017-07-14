﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace ExpressiveAnnotations.MvvmDesktopSample.Misc
{
    public class NullableEnumerationExtension : MarkupExtension
    {
        private Type _enumType;

        public NullableEnumerationExtension(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        }

        private Type EnumType
        {
            get => _enumType;
            set
            {
                if (_enumType == value)
                    return;

                var enumType = value;
                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = enumType;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);
            var enumItems = enumValues
                .Cast<object>()
                .Select(enumValue => new EnumItem
                {
                    Value = enumValue,
                    Name = GetDisplayName(enumValue)
                })
                .ToArray();

            enumItems = new[] {new EnumItem {Value = null, Name = string.Empty}}.Concat(enumItems).ToArray();
            return enumItems;
        }

        private string GetDisplayName(object enumValue)
        {
            var displayAttrib = EnumType
                .GetField(enumValue.ToString())
                .GetCustomAttributes<DisplayAttribute>()
                .FirstOrDefault();

            return displayAttrib != null ? displayAttrib.Name : enumValue.ToString();
        }

        public class EnumItem
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }
    }
}
