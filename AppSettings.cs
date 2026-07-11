using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MicMute
{
    /// <summary>
    /// MicMute configuration data model.
    /// Serialized as JSON in %LocalAppData%\MicMute\settings.json.
    /// </summary>
    [DataContract]
    public class MicMuteSettings
    {
        // --- Capture Device ---
        [DataMember] public string DeviceId { get; set; } = "";
        [DataMember] public string DeviceName { get; set; } = "";

        // --- Shortcut Keys (HotkeyConverter string format) ---
        [DataMember] public string Hotkey { get; set; } = null;
        [DataMember] public string HotkeyMute { get; set; } = null;
        [DataMember] public string HotkeyUnmute { get; set; } = null;

        // --- Sound Feedback ---
        [DataMember] public bool PlayMute { get; set; } = true;
        [DataMember] public bool PlayUnmute { get; set; } = false;
        [DataMember] public string SoundMutePath { get; set; } = "";
        [DataMember] public string SoundUnmutePath { get; set; } = "";
        [DataMember] public int SoundVolume { get; set; } = 100;

        // --- Language ---
        [DataMember] public string Language { get; set; } = "PT";

        // --- Start with Windows ---
        [DataMember] public bool StartWithWindows { get; set; } = false;
    }

    /// <summary>
    /// Manages reading and writing of the JSON configuration file.
    /// Path: %LocalAppData%\MicMute\settings.json
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>Full path to the configuration file.</summary>
        public static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MicMute",
            "settings.json");

        /// <summary>
        /// Loads settings from the JSON file.
        /// Returns null if the file does not exist or is corrupted.
        /// </summary>
        public static MicMuteSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    return null;

                var serializer = new DataContractJsonSerializer(typeof(MicMuteSettings));
                using (var stream = new FileStream(SettingsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return (MicMuteSettings)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SettingsManager] Error loading settings.json: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Writes settings to the JSON file.
        /// Creates the directory if it does not exist.
        /// </summary>
        public static void Save(MicMuteSettings settings)
        {
            if (settings == null) return;
            try
            {
                string dir = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var serializer = new DataContractJsonSerializer(typeof(MicMuteSettings));

                // Serialize to memory first to ensure the file is not corrupted in case of error
                byte[] jsonBytes;
                using (var ms = new MemoryStream())
                {
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, false, true, "  "))
                    {
                        serializer.WriteObject(writer, settings);
                    }
                    jsonBytes = ms.ToArray();
                }

                File.WriteAllBytes(SettingsFilePath, jsonBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SettingsManager] Error writing settings.json: " + ex.Message);
            }
        }
    }
}
