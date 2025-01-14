﻿using System;
using System.Text;
using Camino.Core.Contracts.Providers;
using System.IO;
using System.Reflection;
using Camino.Infrastructure.Files.Contracts;
using Newtonsoft.Json;

namespace Module.Setup.WebAdmin.Providers
{
    public class SetupProvider : ISetupProvider
    {
        private readonly IFileProvider _fileProvider;
        private readonly SetupSettings _setupSettings;

        public SetupProvider(IFileProvider fileProvider, SetupSettings setupSettings)
        {
            _fileProvider = fileProvider;
            _setupSettings = setupSettings;
        }

        public bool HasDatabaseSetup()
        {
            return LoadSettings().HasSetupDatabase;
        }

        public bool HasDataSeeded()
        {
            return LoadSettings().HasSeededData;
        }

        public bool HasInitializedSetup()
        {
            return LoadSettings().IsInitialized;
        }

        public string LoadFileText(string relativePath)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return _fileProvider.ReadText(Path.Combine(assemblyDirectory, relativePath), Encoding.Default);
        }

        public void SetDatabaseHasBeenSetup(string filePath = null)
        {
            filePath ??= SetupConfigs.SetupConfigurationPath;

            try
            {
                var setupSettings = LoadSettings();
                setupSettings.HasSetupDatabase = true;
                SaveSettings(setupSettings, filePath);
            }
            catch (Exception)
            {
                _setupSettings.HasSetupDatabase = false;
            }
        }

        public void SetDataHasBeenSeeded(string filePath = null)
        {
            filePath ??= SetupConfigs.SetupConfigurationPath;

            try
            {
                _setupSettings.HasSeededData = true;
                SaveSettings(_setupSettings, filePath);
            }
            catch (Exception)
            {
                _setupSettings.HasSeededData = false;
            }
        }

        public SetupSettings LoadSettings(string setupSettingsFilePath = null)
        {
            if (_setupSettings != null && _setupSettings.HasSetupDatabase && _setupSettings.HasSeededData)
            {
                return _setupSettings;
            }

            setupSettingsFilePath ??= SetupConfigs.SetupConfigurationPath;
            if (!_fileProvider.FileExists(setupSettingsFilePath))
            {
                var installSettings = new SetupSettings
                {
                    IsInitialized = true,
                    SeedDataJsonFilePath = SetupConfigs.PrepareDataPath
                };

                SaveSettings(installSettings, setupSettingsFilePath);
                _setupSettings.IsInitialized = true;
                return installSettings;
            }

            var settingText = _fileProvider.ReadText(setupSettingsFilePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(settingText))
            {
                return new SetupSettings();
            }

            var setupSettings = JsonConvert.DeserializeObject<SetupSettings>(settingText);
            _setupSettings.HasSetupDatabase = setupSettings.HasSetupDatabase;
            _setupSettings.HasSeededData = setupSettings.HasSeededData;
            return setupSettings;
        }

        public void DeleteSetupSettings(string setupSettingsFilePath = null)
        {
            setupSettingsFilePath ??= SetupConfigs.SetupConfigurationPath;
            _fileProvider.DeleteFile(setupSettingsFilePath);
        }

        public void SaveSettings(SetupSettings settings, string setupSettingsFilePath)
        {
            var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

           _fileProvider.CreateFile(setupSettingsFilePath, settingsJson);
        }
    }
}
