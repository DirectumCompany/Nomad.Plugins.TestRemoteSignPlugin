using System.Collections.Generic;

namespace NpoComputer.Nomad.Internal.Plugins.TestRemoteSignPlugin
{
  /// <summary>
  /// Хранилище результатов запросов подписи.
  /// </summary>
  public static class RequestResultsStorage
  {
    /// <summary>
    /// Результаты запросов подписи.
    /// </summary>
    private static readonly Dictionary<string, (int userId , byte[] signature)> _signRequestResults =
      new Dictionary<string, (int userId, byte[] signature)>();

    /// <summary>
    /// Операции пользователей.
    /// </summary>
    private static readonly Dictionary<int, Queue<string>> _usersOperations = new Dictionary<int, Queue<string>>();

    /// <summary>
    /// Объект блокировки.
    /// </summary>
    private static readonly object _syncObject = new object();

    /// <summary>
    /// Добавить результат запроса в хранилище.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <param name="operationId">ИД операции.</param>
    /// <param name="signature">Результат выполнения запроса.</param>
    public static void SaveRequestResult(int userId, string operationId, byte[] signature)
    {
      lock (_syncObject)
      {
        if (_usersOperations.TryGetValue(userId, out var operations))
        {
          // Храним две последние подписи для каждого пользователя.
          while (operations.Count > 2)
          {
            var expired = operations.Dequeue();
            _signRequestResults.Remove(expired);
          }
        }
        else
        {
          operations = new Queue<string>();
          _usersOperations[userId] = operations;
        }

        if (!operations.Contains(operationId))
          operations.Enqueue(operationId);

        _signRequestResults[operationId] = (userId, signature);
      }
    }

    /// <summary>
    /// Пытается получить результат выполнения запроса.
    /// </summary>
    /// <param name="operationId">ИД операции.</param>
    /// <param name="result">Результат.</param>
    /// <returns>True, если удалось получить результат, иначе false.</returns>
    public static bool TryGetSignRequestResult(string operationId, out (int userId, byte[] signature) result)
    {
      lock (_syncObject)
      {
        return _signRequestResults.TryGetValue(operationId, out result);
      }
    }
  }
}