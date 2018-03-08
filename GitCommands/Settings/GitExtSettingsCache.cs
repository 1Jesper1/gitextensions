﻿using System;
using System.Text;
using System.Xml;

namespace GitCommands.Settings
{
    public class GitExtSettingsCache : FileSettingsCache
    {
        private readonly XmlSerializableDictionary<string, string> _EncodedNameMap = new XmlSerializableDictionary<string, string>();

        public GitExtSettingsCache(string settingsFilePath, bool autoSave = true)
            : base(settingsFilePath, autoSave)
        {
        }

        public static GitExtSettingsCache FromCache(string settingsFilePath)
        {
            Lazy<GitExtSettingsCache> createSettingsCache = new Lazy<GitExtSettingsCache>(() =>
                {
                    return new GitExtSettingsCache(settingsFilePath, true);
                });

            return FileSettingsCache.FromCache(settingsFilePath, createSettingsCache);
        }

        public static GitExtSettingsCache Create(string settingsFilePath, bool allowCache = true)
        {
            if (allowCache)
            {
                return FromCache(settingsFilePath);
            }
            else
            {
                return new GitExtSettingsCache(settingsFilePath, false);
            }
        }

        protected override void ClearImpl()
        {
            _EncodedNameMap.Clear();
        }

        protected override void WriteSettings(string fileName)
        {
            using (System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(fileName, Encoding.UTF8))
            {
                xtw.Formatting = Formatting.Indented;
                xtw.WriteStartDocument();
                xtw.WriteStartElement("dictionary");

                _EncodedNameMap.WriteXml(xtw);
                xtw.WriteEndElement();
            }
        }

        protected override void ReadSettings(string fileName)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                CheckCharacters = false
            };

            using (System.Xml.XmlReader xr = XmlReader.Create(fileName, readerSettings))
            {
                _EncodedNameMap.ReadXml(xr);
            }
        }

        protected override void SetValueImpl(string key, string value)
        {
            if (value == null)
            {
                _EncodedNameMap.Remove(key);
            }
            else
            {
                _EncodedNameMap[key] = value;
            }
        }

        protected override string GetValueImpl(string key)
        {
            _EncodedNameMap.TryGetValue(key, out var value);
            return value;
        }
    }
}
