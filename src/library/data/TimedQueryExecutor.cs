using System;
using System.Data;
using Nohros.Data;

namespace Nohros.Metrics.Data
{
  /// <summary>
  /// A timer metric for tracking how much time data access is taking.
  /// </summary>
  /// <remarks>
  /// The <see cref="TimedQueryExecutor"/> wraps a
  /// <see cref="IQueryExecutor"/> and track the time that was spent during
  /// the execution of that method through a <see cref="ITimer"/>.
  /// </remarks>
  public class TimedQueryExecutor : IQueryExecutor
  {
    readonly IQueryExecutor executor_;
    readonly ITimer timer_;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimedQueryExecutor"/>
    /// class by using the given <see cref="IQueryMapper{T}"/> and
    /// <see cref="ITimer"/>.
    /// </summary>
    /// <param name="executor">
    /// A <see cref="IQueryExecutor"/> that is used to execute queries against
    /// a database.
    /// </param>
    /// <param name="timer">
    /// A <see cref="ITimer"/> to be used to track the duration of queries
    /// executed over a given <see cref="IQueryExecutor"/>.
    /// </param>
    public TimedQueryExecutor(IQueryExecutor executor, ITimer timer) {
      executor_ = executor;
      timer_ = timer;
    }

    public int ExecuteNonQuery(string query,
      Action<CommandBuilder> set_parameters) {
      return timer_.Time(() => executor_.ExecuteNonQuery(query, set_parameters));
    }

    public int ExecuteNonQuery(string query) {
      return timer_.Time(() => executor_.ExecuteNonQuery(query));
    }

    public int ExecuteNonQuery(string query, CommandType command_type) {
      return timer_.Time(() => executor_.ExecuteNonQuery(query, command_type));
    }

    public int ExecuteNonQuery(string query,
      Action<CommandBuilder> set_parameters,
      CommandType command_type) {
      return
        timer_
          .Time(() => executor_
            .ExecuteNonQuery(query, set_parameters, command_type));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper, Action<CommandBuilder> set_parameters,
      CommandType command_type) {
      return
        timer_.Time(() => executor_
          .ExecuteQuery(query, mapper, set_parameters, command_type));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query) {
      return timer_.Time(() => executor_.ExecuteQuery<T>(query));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper, Action<CommandBuilder> set_parameters) {
      return
        timer_
          .Time(() => executor_.ExecuteQuery(query, mapper, set_parameters));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query,
      Action<CommandBuilder> set_parameters) {
      return timer_.Time(() => executor_.ExecuteQuery<T>(query, set_parameters));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query,
      CommandType command_type) {
      return timer_.Time(() => executor_.ExecuteQuery<T>(query, command_type));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query,
      Action<CommandBuilder> set_parameters,
      CommandType command_type) {
      return
        timer_
          .Time(() => executor_
            .ExecuteQuery<T>(query, set_parameters, command_type));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper, CommandType command_type) {
      return
        timer_
          .Time(() => executor_.ExecuteQuery(query, mapper, command_type));
    }

    public IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper) {
      return timer_.Time(() => executor_.ExecuteQuery(query, mapper));
    }

    public T ExecuteScalar<T>(string query,
      Action<CommandBuilder> set_parameters) {
      return timer_.Time(() => executor_.ExecuteScalar<T>(query, set_parameters));
    }

    public T ExecuteScalar<T>(string query,
      Action<CommandBuilder> set_parameters, CommandType command_type) {
      return
        timer_
          .Time(() => executor_
            .ExecuteScalar<T>(query, set_parameters, command_type));
    }

    public T ExecuteScalar<T>(string query) {
      return timer_.Time(() => executor_.ExecuteScalar<T>(query));
    }

    public T ExecuteScalar<T>(string query, CommandType command_type) {
      return timer_.Time(() => executor_.ExecuteScalar<T>(query, command_type));
    }

    public bool ExecuteScalar<T>(string query,
      Action<CommandBuilder> set_parameters, out T t) {
      TimerContext context = timer_.Time();
      bool result = executor_.ExecuteScalar(query, set_parameters, out t);
      context.Stop();
      return result;
    }

    public bool ExecuteScalar<T>(string query,
      Action<CommandBuilder> set_parameters,
      CommandType command_type, out T t) {
      TimerContext context = timer_.Time();
      bool result =
        executor_
          .ExecuteScalar(query, set_parameters, command_type, out t);
      context.Stop();
      return result;
    }

    public bool ExecuteScalar<T>(string query, out T t) {
      TimerContext context = timer_.Time();
      bool result = executor_.ExecuteScalar(query, out t);
      context.Stop();
      return result;
    }

    public bool ExecuteScalar<T>(string query, CommandType command_type, out T t) {
      TimerContext context = timer_.Time();
      bool result = executor_.ExecuteScalar(query, command_type, out t);
      context.Stop();
      return result;
    }

    public MetricConfig Config {
      get { return timer_.Config; }
    }
  }
}
