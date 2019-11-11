# Nomad.Plugins.TestRemoteSignPlugin
Тестовый плагин удаленного подписания.

IRemoteSignPlugin позволяет реализовать собственный плагин удаленного подписания (облачная подпись, подписание на SIM).

Интерфейс _IRemoteSignPlugin_ содержит следующие методы:

**SignDocument**

Подписывает документ.

_string SignDocument(int userId, int seanceId, int certificateId, DocumentModel document, Stream data, ContentType contentType)_

Параметры:
 * userId - ИД пользователя в системе DIRECTUM;
 * seanceId - ИД текущего сеанса, связанного с устройством пользователя;
 * certificateId - ИД сертификата, которым выполняется подписание;
 * document - информация о подписываемом документе;
 * data - подписываемые бинарные данные;
 * contentType - тип подписываемых данных (Document - документ, Attributes - атрибуты).

Метод возвращает ИД операции, по которому в дальнейшем можно получить результат подписания.

**GetSignStatus**

Получает статус текущей операции подписания.

_SignStatus GetSignStatus(string operationId)_

Параметры:

* operationId - ИД операции, полученный при вызове SignDocument.

**GetCertificates**

Получает сертификаты пользователей, которые могут подписывать документы через плагин.

_Dictionary<int, HashSet<Certificate>> GetCertificates()_

Возвращает словарь, где key - ИД пользователя, value - коллекция сертификатов.

**GetUserCertificates**

Получает сертификаты одного пользователя.

IReadOnlyCollection<Certificate> GetUserCertificates(int userId)

* userId - ИД пользователя в системе DIRECTUM.