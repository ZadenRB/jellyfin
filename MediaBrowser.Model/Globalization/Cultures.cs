using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MediaBrowser.Model.Globalization
{
    /// <summary>
    /// Class Cultures.
    /// </summary>
    public sealed class Cultures
    {
        private static readonly Assembly _assembly = typeof(Cultures).Assembly;
        private static Lazy<Cultures> _instance = new Lazy<Cultures>
            (() => new Cultures());

        /// <summary>
        /// Returns singleton instance of Cultures.
        /// </summary>
        public static Cultures Instance { get { return _instance.Value; } }
        /// <summary>
        /// List of cultures.
        /// </summary>
        private static List<CultureDto> _cultures;
        private Cultures() {
            _cultures = new List<CultureDto>();

            const string ResourcePath = "MediaBrowser.Model.Globalization.iso6392.txt";

            using (var stream = _assembly.GetManifestResourceStream(ResourcePath))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var parts = line.Split('|');

                    if (parts.Length == 5)
                    {
                        string name = parts[3];
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            continue;
                        }

                        string twoCharName = parts[2];
                        if (string.IsNullOrWhiteSpace(twoCharName))
                        {
                            continue;
                        }

                        string[] threeletterNames;
                        if (string.IsNullOrWhiteSpace(parts[1]))
                        {
                            threeletterNames = new[] { parts[0] };
                        }
                        else
                        {
                            threeletterNames = new[] { parts[0], parts[1] };
                        }

                        _cultures.Add(new CultureDto
                        {
                            DisplayName = name,
                            Name = name,
                            ThreeLetterISOLanguageNames = threeletterNames,
                            TwoLetterISOLanguageName = twoCharName
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cultures.
        /// </summary>
        /// <returns><see cref="IEnumerable{CultureDto}" />.</returns>
        public IEnumerable<CultureDto> GetCultures()
            => _cultures;

        /// <inheritdoc />
        public CultureDto FindLanguageInfo(string language)
            => GetCultures()
                .FirstOrDefault(i =>
                    string.Equals(i.DisplayName, language, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(i.Name, language, StringComparison.OrdinalIgnoreCase)
                    || i.ThreeLetterISOLanguageNames.Contains(language, StringComparer.OrdinalIgnoreCase)
                    || string.Equals(i.TwoLetterISOLanguageName, language, StringComparison.OrdinalIgnoreCase));
    }
}
