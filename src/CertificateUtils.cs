using System.Linq;

namespace NpoComputer.Nomad.Internal.Plugins.TestRemoteSignPlugin
{
  /// <summary>
  /// Утилиты для работы с сертификатами.
  /// </summary>
  public static class CertificateUtils
  {
    /// <summary>
    /// Удаляет из отпечатка сертификата невалидные символы.
    /// </summary>
    /// <param name="thumbprint">Отпечаток сертификата.</param>
    public static string CleanThumbprint(string thumbprint)
    {
      return new string(thumbprint.Where(char.IsLetterOrDigit).ToArray());
    }
  }
}
