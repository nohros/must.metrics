using System;
using System.Collections.Generic;
using System.Data;
using Nohros.Data;
using Nohros.Data.SqlServer;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics.Sql
{
  public class SqlMetricsDao : IMetricsDao
  {
    readonly SqlConnectionProvider sql_connection_provider_;
    readonly SqlQueryExecutor sql_query_executor_;
    readonly string schema_;

    public SqlMetricsDao(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      sql_query_executor_ = new SqlQueryExecutor(sql_connection_provider,
        CommandType.StoredProcedure);
      schema_ = sql_connection_provider_.Schema;
    }

    public IEnumerable<long> GetSeriesIds(string name, int hash, int count) {
      using (IQueryMapper<long> mapper =
        sql_query_executor_
          .ExecuteQuery(schema_ + ".mtc_get_id_of_serie",
            Mappers.Long,
            builder =>
              builder
                .AddParameter("@name", name)
                .AddParameter("@hash", hash)
                .AddParameter("@tags_count", count))) {
        return mapper.Map(false);
      }
    }

    public void RegisterMeasure(long serie_id, double value, DateTime timestamp) {
      sql_query_executor_
        .ExecuteNonQuery(schema_ + ".mtc_add_measure",
          builder =>
            builder
              .AddParameterWithValue("@measure", value)
              .AddParameter("@timestamp", timestamp)
              .AddParameter("@serie_id", serie_id));
    }

    public long RegisterSerie(string name, int hash, int count) {
      return sql_query_executor_
        .ExecuteScalar<long>(schema_ + ".mtc_add_serie",
          builder =>
            builder
              .AddParameter("@name", name)
              .AddParameter("@hash", hash)
              .AddParameter("@tags_count", count));
    }

    public void RegisterTag(string name, string value, long serie_id) {
      sql_query_executor_
        .ExecuteNonQuery(schema_ + ".mtc_add_tag",
          builder =>
            builder
              .AddParameter("@name", name)
              .AddParameter("@value", value)
              .AddParameter("@serie_id", serie_id));
    }

    public bool ContainsTag(string name, string value, long tags_id) {
      bool contains;
      sql_query_executor_
        .ExecuteScalar(schema_ + ".mtc_serie_contains_tag",
          builder =>
            builder
              .AddParameter("@serie_id", tags_id)
              .AddParameter("@name", name)
              .AddParameter("@value", value), out contains);
      return contains;
    }
  }
}
