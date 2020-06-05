﻿using System;
using System.Collections.Generic;
using System.Threading;
using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.SharedModels;
using Newtonsoft.Json.Linq;

namespace Kentico.Kontent.Delivery.ContentItems
{
    /// <summary>
    /// Response object with built-in linked items resolution.
    /// </summary>
    public abstract class AbstractItemsResponse : AbstractResponse
    {
        private readonly Lazy<List<object>> _linkedItems;

        /// <summary>
        /// Gets the linked items and their properties.
        /// </summary>
        public IReadOnlyList<object> LinkedItems => _linkedItems.Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractItemsResponse"/> class.
        /// </summary>
        /// <param name="response">The response from Kentico Kontent Delivery API that contains a content item.</param>
        /// <param name="modelProvider">The provider that can convert JSON responses into instances of .NET types.</param>
        protected AbstractItemsResponse(ApiResponse response, IModelProvider modelProvider) : base(response)
        {
            _linkedItems = new Lazy<List<object>>(() =>
            {
                var linkedItems = (JObject)response.JsonContent["modular_content"].DeepClone();
                List<object> result = new List<object>();
                foreach (var keyValuePair in linkedItems)
                {
                    result.Add(modelProvider.GetContentItemModel<object>(keyValuePair.Value, linkedItems));
                }
                return result;

            }, LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
