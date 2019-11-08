using NpoComputer.Nomad.Contract;
using NpoComputer.Nomad.Internal.Plugins.TestRemoteSignPlugin.Settings;
using NpoComputer.Nomad.Utility;
using System.Collections.Generic;
using System.IO;

namespace NpoComputer.Nomad.Internal.Plugins.TestRemoteSignPlugin
{
  /// <summary>
  /// Конфигурация плагина.
  /// </summary>
  public class PluginConfiguration
  {
    /// <summary>
    /// Путь до файла конфигурации по умолчанию.
    /// </summary>
    private static readonly string _configFilePath =
      ConfigurationUtils.GetAssemblyConfigurationFilePath(typeof(PluginConfiguration).Assembly);

    /// <summary>
    /// Имя файла конфигурации.
    /// </summary>
    private static readonly string ConfigFileName = Path.GetFileName(_configFilePath);

    /// <summary>
    /// Менеджер конфигурации.
    /// </summary>
    private static readonly HotPlugConfigManager _сonfigManager =
      new HotPlugConfigManager(ConfigFileName,
        @"App_Data\Plugins\" + Path.GetFileName(Path.GetDirectoryName(_configFilePath)), "configuration");

    /// <summary>
    /// Возвращает секцию настроек сертификатов.
    /// </summary>
    internal static List<CertificateSetting> CertificatesSettings
    {
      get
      {
        var certificates = _сonfigManager.GetSection<CertificatesSection>("certificates").Certificates;

        foreach (var certificate in certificates)
          certificate.Thumbprint = CertificateUtils.CleanThumbprint(certificate.Thumbprint);

        return certificates;
      }
    }
  }
}