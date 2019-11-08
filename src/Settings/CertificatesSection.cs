using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NpoComputer.Nomad.Internal.Plugins.TestRemoteSignPlugin.Settings
{
  /// <summary>
  /// Секция настроек сертификатов.
  /// </summary>
  [XmlType("certificates")]
  public class CertificatesSection
  {
    /// <summary>
    /// Описание сертификата.
    /// </summary>
    [XmlElement("certificate")]
    public List<CertificateSetting> Certificates { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public CertificatesSection()
    {
      Certificates = new List<CertificateSetting>();
    }
  }

  /// <summary>
  /// Описание сертификата.
  /// </summary>
  [XmlType("certificate")]
  public class CertificateSetting
  {
    /// <summary>
    /// ИД пользователя.
    /// </summary>
    [XmlAttribute("userId")]
    public int UserId { get; set; }

    /// <summary>
    /// ИД тенанта.
    /// </summary>
    /// <remarks>
    /// Поле используется для DIRECTUM RX.
    /// </remarks>
    [XmlAttribute("tenantId")]
    [DefaultValue("")]
    public string TenantId { get; set; }

    /// <summary>
    /// ИД сертификата.
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Отображаемое имя.
    /// </summary>
    [XmlAttribute("displayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// Отпечаток сертификата.
    /// </summary>
    [XmlAttribute("thumbprint")]
    public string Thumbprint { get; set; }

    /// <summary>
    /// Признак, что данный сертификат используется по умолчанию.
    /// </summary>
    [XmlAttribute("isDefault")]
    public bool IsDefault { get; set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    [XmlAttribute("password")]
    public string Password { get; set; }

    /// <summary>
    /// Имя плагина.
    /// </summary>
    [XmlAttribute("pluginName")]
    public string PluginName { get; set; }

    /// <summary>
    /// Версия плагина.
    /// </summary>
    [XmlAttribute("pluginVersion")]
    public string PluginVersion { get; set; }
  }
}