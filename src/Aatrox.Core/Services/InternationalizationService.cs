using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Aatrox.Data.Enums;
using Newtonsoft.Json;

namespace Aatrox.Core.Services
{
    public static class InternationalizationService
    {
        private static ReadOnlyDictionary<Lang, ReadOnlyDictionary<string, string>> Strings { get; set; }

        static InternationalizationService()
        {
            Setup();
        }

        public static void Setup()
        {
            var strings = new Dictionary<Lang, ReadOnlyDictionary<string, string>>();

            foreach (var file in Directory.GetFiles("i18n"))
            {
                var content = File.ReadAllText(file);
                var elements = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                var lang = Enum.Parse(typeof(Lang), file.Substring(file.Length - 7, 2), true);

                strings.Add((Lang)lang, new ReadOnlyDictionary<string, string>(elements));
            }

            Strings = new ReadOnlyDictionary<Lang, ReadOnlyDictionary<string, string>>(strings);
        }

        public static string GetLocalization(string key, Lang lang = Lang.En, params object[] parameters)
        {
            if (!Strings.ContainsKey(lang))
            {
                lang = Lang.En;
            }

            if (!Strings[lang].ContainsKey(key))
            {
                lang = Lang.En;
            }

            return string.Format(Strings[lang][key], parameters);
        }
    }
}
