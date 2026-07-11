using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MicMute
{
    /// <summary>
    /// Modelo de dados de configuração do MicMute.
    /// Serializado como JSON em %LocalAppData%\MicMute\settings.json.
    /// </summary>
    [DataContract]
    public class MicMuteSettings
    {
        // --- Dispositivo de captura ---
        [DataMember] public string DeviceId { get; set; } = "";
        [DataMember] public string DeviceName { get; set; } = "";

        // --- Teclas de atalho (formato string do HotkeyConverter) ---
        [DataMember] public string Hotkey { get; set; } = null;
        [DataMember] public string HotkeyMute { get; set; } = null;
        [DataMember] public string HotkeyUnmute { get; set; } = null;

        // --- Feedback sonoro ---
        [DataMember] public bool PlayMute { get; set; } = true;
        [DataMember] public bool PlayUnmute { get; set; } = false;
        [DataMember] public string SoundMutePath { get; set; } = "";
        [DataMember] public string SoundUnmutePath { get; set; } = "";
        [DataMember] public int SoundVolume { get; set; } = 100;

        // --- Idioma ---
        [DataMember] public string Language { get; set; } = "PT";

        // --- Iniciar com Windows ---
        [DataMember] public bool StartWithWindows { get; set; } = false;
    }

    /// <summary>
    /// Gerencia leitura e escrita do arquivo JSON de configuração.
    /// Caminho: %LocalAppData%\MicMute\settings.json
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>Caminho completo do arquivo de configuração.</summary>
        public static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MicMute",
            "settings.json");

        /// <summary>
        /// Carrega as configurações do arquivo JSON.
        /// Retorna null se o arquivo não existir ou estiver corrompido.
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
                Console.WriteLine("[SettingsManager] Erro ao carregar settings.json: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Grava as configurações no arquivo JSON.
        /// Cria o diretório se não existir.
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

                // Serializa em memória primeiro para garantir que não corrompe o arquivo em caso de erro
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
                Console.WriteLine("[SettingsManager] Erro ao gravar settings.json: " + ex.Message);
            }
        }
    }
}
