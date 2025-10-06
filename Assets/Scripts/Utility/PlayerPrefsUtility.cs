using System.Collections.Generic;
using UnityEngine;

namespace RPG.Utility
{
    /// <summary>
    /// Utility class for storing and retrieving lists of strings using Unity's PlayerPrefs system.
    /// Provides helper methods to serialize and deserialize string lists as comma-separated values.
    /// </summary>
    public static class PlayerPrefsUtility
    {
        /// <summary>
        /// Saves a list of strings to PlayerPrefs by joining them with commas.
        /// </summary>
        /// <param name="key">The key under which to store the values in PlayerPrefs.</param>
        /// <param name="values">The list of strings to save.</param>
        public static void SetString(string key, List<string> values)
        {
            var formattedValue = string.Join(",", values);

            PlayerPrefs.SetString(key, formattedValue);

            Debug.Log($"Saved {key} with values: {formattedValue}");
        }

        /// <summary>
        /// Retrieves a list of strings from PlayerPrefs that were saved as comma-separated values.
        /// </summary>
        /// <param name="key">The key under which the values are stored in PlayerPrefs.</param>
        /// <returns>
        /// A list of strings parsed from the stored comma-separated value.
        /// Returns an empty list if the key doesn't exist or the value is empty.
        /// </returns>
        public static List<string> GetString(string key)
        {
            var unformattedValue = PlayerPrefs.GetString(key);

            if (string.IsNullOrEmpty(unformattedValue))
            {
                return new List<string>();
            }

            var formattedValues = new List<string>(
                unformattedValue.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
            );

            return formattedValues;
        }
    }
}