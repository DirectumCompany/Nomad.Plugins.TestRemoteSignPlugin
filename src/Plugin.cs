using NpoComputer.Nomad.Contract.Extensions;
using NpoComputer.Nomad.Contract.Models;
using NpoComputer.Nomad.Contract.Models.RemoteSigning;
using NpoComputer.Nomad.Contract.RemoteSigning;
using Sungero.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Certificate = NpoComputer.Nomad.Contract.RemoteSigning.Certificate;

namespace NpoComputer.Nomad.Internal.Plugins.TestRemoteSignPlugin
{
  /// <summary>
  /// Плагин подписания.
  /// </summary>
  public class Plugin : IRemoteSignPlugin
  {
    #region Константы

    /// <summary>
    /// Имя плагина по умолчанию.
    /// </summary>
    private const string DefaultPluginName = "Capicom Encryption";

    /// <summary>
    /// Версия плагина по умолчанию.
    /// </summary>
    private const string DefaultPluginVersion = "2.0";

    #endregion

    #region Поля и свойства

    /// <summary>
    /// Уникальный ИД плагина.
    /// </summary>
    public Guid Id { get; } = new Guid("c5c75683-fab0-46c9-b044-9e92d0b610d3");

    #endregion

    #region Методы

    /// <summary>
    /// Выполняет инициализацию.
    /// </summary>
    public void Initialize()
    {
    }

    /// <summary>
    /// Получает пользователей СЭД, для которых настроены сертификаты подписи.
    /// </summary>
    /// <returns>Словарь, где key - ИД пользователя, value - коллекция ИД сертификатов.</returns>
    /// <remarks>Периодически вызывается Nomad.</remarks>
    public Dictionary<int, HashSet<Certificate>> GetCertificates()
    {
      var currentTenantId = GetCurrentTenantId();

      return PluginConfiguration.CertificatesSettings
        .Where(c => string.IsNullOrEmpty(c.TenantId) || currentTenantId.Equals(c.TenantId, StringComparison.OrdinalIgnoreCase))
        .GroupBy(x => x.UserId)
        .ToDictionary(x => x.Key, y => new HashSet<Certificate>(
          y.Select(c => 
            new Certificate(c.Id, c.DisplayName, c.Thumbprint, c.IsDefault)
            {
              PluginName = c.PluginName ?? DefaultPluginName,
              PluginVersion = c.PluginVersion ?? DefaultPluginVersion
            })));
    }

    /// <summary>
    /// Получает сертификаты пользвователя.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    public IReadOnlyCollection<Certificate> GetUserCertificates(int userId)
    {
      var currentTenantId = GetCurrentTenantId();

      return PluginConfiguration.CertificatesSettings
        .Where(c => c.UserId == userId)
        .Where(c => string.IsNullOrEmpty(c.TenantId) || currentTenantId.Equals(c.TenantId, StringComparison.OrdinalIgnoreCase))
        .Select(x => 
          new Certificate(x.Id, x.DisplayName, x.Thumbprint, x.IsDefault)
          {
            PluginName = x.PluginName ?? DefaultPluginName,
            PluginVersion = x.PluginVersion ?? DefaultPluginVersion
          })
        .ToArray();
    }

    /// <summary>
    /// Подписывает документ.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <param name="seanceId">ИД сеанса (клиентского подключения).</param>
    /// <param name="certificateId">ИД сертификата.</param>
    /// <param name="document">Документ.</param>
    /// <param name="data">Подписываемые данные.</param>
    /// <param name="contentType">Тип подписываемых данных.</param>
    /// <returns>ИД операции.</returns>
    public string SignDocument(int userId, int seanceId, int certificateId, DocumentModel document,
      Stream data, ContentType contentType)
    {
      byte[] signature;

      using (var certificate = CertificateManager.GetUserCertificate(userId, certificateId))
      {
        using (var ms = new MemoryStream())
        {
          data.CopyTo(ms);

          var contentInfo = new ContentInfo(ms.ToArray());
          var signedCms = new SignedCms(contentInfo, true);
          var cmsSigner = new CmsSigner(certificate);

          cmsSigner.IncludeOption = X509IncludeOption.EndCertOnly;
          cmsSigner.SignerIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
          cmsSigner.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.Now));

          signedCms.ComputeSignature(cmsSigner, true);
          signature = signedCms.Encode();
        }
      }

      var newOperationId = Guid.NewGuid().ToString();
      RequestResultsStorage.SaveRequestResult(userId, newOperationId, null);

      // Имитируем работу.
      Task.Delay(TimeSpan.FromSeconds(10))
        .ContinueWith(t =>
        {
          RequestResultsStorage.SaveRequestResult(userId, newOperationId, signature);
        });

      return newOperationId;
    }

    /// <summary>
    /// Возвращает информацию о статусе подписания.
    /// </summary>
    /// <param name="operationId">ИД операции.</param>
    /// <returns>Информация о статусе операции.</returns>
    public SignStatus GetSignStatus(string operationId)
    {
      var hasResult = RequestResultsStorage.TryGetSignRequestResult(operationId, out var signResult);

      if (!hasResult)
        return new SignStatus(SignRequestState.Failed, $"Invalid param \"{nameof(operationId)}\"");

      var result = signResult.signature != null
        ? new SignStatus(signResult.signature)
        : new SignStatus(SignRequestState.InProgress, "Operation in progress...");

      return result;
    }

    /// <summary>
    /// Получает ИД текущего тенанта.
    /// </summary>
    private string GetCurrentTenantId()
    {
      if (AppDomain.CurrentDomain.GetAssemblies().All(x => x.GetName().Name != "Sungero.Domain"))
        return string.Empty;

      return GetCurrentTenantIdFromSungero();
    }

    /// <summary>
    /// Получает ИД текущего тенанта из Sungero.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private string GetCurrentTenantIdFromSungero()
    {
      return TenantRegistry.Instance.CurrentTenant.Id;
    }

    #endregion
  }
}