// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using ChatbotSample.Model;
using System;

namespace ChatbotSample.ReinforcementLearning
{
    /// <summary>
    /// Represents Reinforment Features used in this demo.
    /// </summary>
    public class RLFeatures
    {
        public RLFeatures(Weather weather, DayOfWeek day)
        {
            Weather = weather;
            DayOfWeek = day;
        }

        /// <summary>
        /// Gets weather used in Rank call ContextFeatures.
        /// </summary>
        /// <value>
        /// <see cref="Weather"/>.
        /// </value>
        public Weather Weather { get; private set; }

        /// <summary>
        /// Gets DayOfWeek used in Rank call ContextFeatures.
        /// </summary>
        /// <value>
        /// integer.
        /// </value>
        public DayOfWeek DayOfWeek { get; private set; }
    }
}
