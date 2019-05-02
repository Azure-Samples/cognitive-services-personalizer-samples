// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using System;
using System.Text;

namespace LuisBot.Model
{
    /// <summary>
    /// Method ultilities for Teas <see cref="Teas"/>.
    /// </summary>
    public static class TeaMethods
    {
        public static string DisplayTeas()
        {
            var builder = new StringBuilder();
            var values = Enum.GetValues(typeof(Teas));

            builder.Append(string.Join(' ', (Teas[])Enum.GetValues(typeof(Teas))));
            return builder.ToString();
        }
    }
}
