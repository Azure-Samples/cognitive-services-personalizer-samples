using PersonalizerDemo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalizerDemo.ActionFeaturizer
{
    public interface IActionFeaturizer
    {
        /// <summary>
        /// Get features from an <see cref="Article"/> objects.
        /// </summary>
        Task<List<Object>> FeaturizeActionsAsync(Article article);
    }
}