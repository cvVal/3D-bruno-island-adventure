using System.Collections.Generic;
using UnityEngine;

namespace RPG.Utility
{
    public static class PlayerPrefsUtility
    {
        public static void SetString(string key, List<string> values)
        {
            var formattedValue = string.Join(",", values);

            PlayerPrefs.SetString(key, formattedValue);

            Debug.Log($"Saved {key} with values: {formattedValue}");
        }

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