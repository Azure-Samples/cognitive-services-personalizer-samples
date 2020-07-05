// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using System;
using System.Text;

namespace PersonalizerChatbot.Model
{
    /// <summary>
    /// Method ultilities for Coffee <see cref="Coffees"/>.
    /// </summary>
    public static class CoffeesMethods
    {
        public static string DisplayCoffees()
        {
            var builder = new StringBuilder();
            var values = Enum.GetValues(typeof(Coffees));

            builder.Append(string.Join(' ', (Coffees[])Enum.GetValues(typeof(Coffees))));
            return builder.ToString();
        }
    }
}
