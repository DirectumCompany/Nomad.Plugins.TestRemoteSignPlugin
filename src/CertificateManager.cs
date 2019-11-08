using NpoComputer.Nomad.Utility;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace NpoComputer.Nomad.Internal.Plugins.TestRemoteSignPlugin
{
  /// <summary>
  /// Менеджер сертификатов.
  /// </summary>
  internal static class CertificateManager
  {
    /// <summary>
    /// Возврвщает сертификат пользователя.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <param name="certificateId">ИД сертификата</param>
    public static X509Certificate2 GetUserCertificate(int userId, int certificateId)
    {
      var configCert = PluginConfiguration.CertificatesSettings
        .FirstOrDefault(c => c.UserId == userId && c.Id == certificateId);

      if (configCert == null)
        throw new InvalidOperationException("Сертификат не найден");

      if (configCert.Thumbprint.IsNullOrEmpty())
        throw new InvalidOperationException("Не указан отпечаток сертификата");

      var certificate = GetUserCertificateFromStorage(configCert.Thumbprint);

      if (certificate == null)
        throw new InvalidOperationException("Сертификат не найден в локальном хранилище компьютера");

      return certificate;
    }

    /// <summary>
    /// Получает сертификат пользователя из хранилища.
    /// </summary>
    /// <param name="thumbprint">Отпечаток сертификата.</param>
    private static X509Certificate2 GetUserCertificateFromStorage(string thumbprint)
    {
      using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
      {
        store.Open(OpenFlags.ReadWrite);

        var collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

        return collection.Count > 0
          ? collection[0]
          : null;
      }
    }
  }
}