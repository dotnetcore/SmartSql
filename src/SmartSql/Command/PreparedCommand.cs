using SmartSql.Abstractions;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using SmartSql.Abstractions.Command;
using System.Data.Common;
using SmartSql.Abstractions.TypeHandler;
using SmartSql.Exceptions;
using SmartSql.Configuration.Statements;
using Microsoft.Extensions.Logging;

namespace SmartSql.Command
{
    public class PreparedCommand : IPreparedCommand
    {
        Regex _sqlParamsTokens;
        private readonly ILogger<PreparedCommand> _logger;
        private readonly SmartSqlContext _smartSqlContext;

        public event OnPreparedHandler OnPrepared;

        public PreparedCommand(
            ILogger<PreparedCommand> logger
            , SmartSqlContext smartSqlContext)
        {
            _logger = logger;
            _smartSqlContext = smartSqlContext;
            string dbPrefixs = $"{smartSqlContext.DbPrefix}{smartSqlContext.SmartDbPrefix}";
            var regOptions = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
            if (smartSqlContext.IgnoreParameterCase)
            {
                regOptions = regOptions | RegexOptions.IgnoreCase;
            }
            _sqlParamsTokens = new Regex(@"[" + dbPrefixs + @"]([\p{L}\p{N}_]+)", regOptions);
        }
        public IDbCommand Prepare(IDbConnectionSession dbSession, RequestContext context)
        {
            var dbCommand = dbSession.Connection.CreateCommand();
            dbCommand.CommandType = context.CommandType;
            dbCommand.Transaction = dbSession.Transaction;
            switch (dbCommand.CommandType)
            {
                case CommandType.Text:
                    {
                        string sql = context.RealSql;
                        if (_sqlParamsTokens.IsMatch(sql))
                        {
                            sql = _sqlParamsTokens.Replace(sql, match =>
                              {
                                  string paramName = match.Groups[1].Value;
                                  var paramMap = context.Statement?.ParameterMap?.Parameters?.FirstOrDefault(p => p.Name == paramName);
                                  var propertyName = paramMap != null ? paramMap.Property : paramName;

                                  if (!context.RequestParameters.Contains(propertyName))
                                  {
                                      return match.Value;
                                  }
                                  var dbParameter = context.RequestParameters.Get(propertyName);
                                  ITypeHandler typeHandler = paramMap?.Handler;
                                  if (typeHandler != null)
                                  {
                                      AddDbParameter(dbCommand, dbParameter, typeHandler);
                                      return match.Value;
                                  }
                                  var paramVal = dbParameter.Value;
                                  bool isString = paramVal is String;
                                  if (paramVal is IEnumerable && !isString)
                                  {
                                      var enumParams = paramVal as IEnumerable;
                                      StringBuilder inParamSql = new StringBuilder();
                                      inParamSql.Append("(");
                                      int item_Index = 0;
                                      foreach (var itemVal in enumParams)
                                      {
                                          string itemParamName = $"{_smartSqlContext.DbPrefix}{paramName}_{item_Index}";
                                          inParamSql.AppendFormat("{0},", itemParamName);
                                          AddDbParameter(dbCommand, new DbParameter
                                          {
                                              Name = itemParamName,
                                              Value = itemVal
                                          }, typeHandler);
                                          item_Index++;
                                      }
                                      inParamSql.Remove(inParamSql.Length - 1, 1);
                                      inParamSql.Append(")");
                                      return inParamSql.ToString();
                                  }
                                  else
                                  {
                                      AddDbParameter(dbCommand, dbParameter);
                                      return match.Value;
                                  }
                              });
                        }
                        dbCommand.CommandText = sql;
                        break;
                    }
                case CommandType.StoredProcedure:
                    {
                        AddDbParameterCollection(dbCommand, context.RequestParameters);
                        dbCommand.CommandText = context.RealSql;
                        break;
                    }
            }
            OnPrepared?.Invoke(this, new OnPreparedEventArgs
            {
                RequestContext = context,
                DbSession = dbSession,
                DbCommand = dbCommand
            });
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                StringBuilder dbParameterStr = new StringBuilder();
                foreach (IDbDataParameter dbParameter in dbCommand.Parameters)
                {
                    dbParameterStr.AppendFormat("{0}={1},", dbParameter.ParameterName, dbParameter.Value);
                }
                _logger.LogDebug($"PreparedCommand.Prepare->Statement.Id:[{context.FullSqlId}],Sql:[{dbCommand.CommandText}],Parameters:[{dbParameterStr}]");
            }
            return dbCommand;
        }

        private void AddDbParameterCollection(IDbCommand dbCommand, DbParameterCollection reqParams)
        {
            foreach (var paramName in reqParams.ParameterNames)
            {
                var dbParameter = reqParams.Get(paramName);
                AddDbParameter(dbCommand, dbParameter);
            }
        }

        private void AddDbParameter(IDbCommand dbCommand, DbParameter dbParameter, ITypeHandler typeHandler = null)
        {
            if (dbCommand.Parameters.Contains(dbParameter.Name)) { return; }
            var sourceParam = dbCommand.CreateParameter();
            sourceParam.ParameterName = dbParameter.Name;
            var paramVal = dbParameter.Value;
            if (paramVal == null)
            {
                sourceParam.Value = DBNull.Value;
            }
            else
            {
                if (typeHandler != null)
                {
                    typeHandler.SetParameter(sourceParam, paramVal);
                }
                else
                {
                    if (paramVal is Enum)
                    {
                        paramVal = paramVal.GetHashCode();
                    }
                    sourceParam.Value = paramVal;
                }
            }
            dbParameter.SourceParameter = sourceParam;
            if (dbParameter.DbType.HasValue)
            {
                sourceParam.DbType = dbParameter.DbType.Value;
            }
            if (dbParameter.Direction.HasValue)
            {
                sourceParam.Direction = dbParameter.Direction.Value;
            }
            if (dbParameter.Precision.HasValue)
            {
                sourceParam.Precision = dbParameter.Precision.Value;
            }
            if (dbParameter.Scale.HasValue)
            {
                sourceParam.Scale = dbParameter.Scale.Value;
            }
            if (dbParameter.Size.HasValue)
            {
                sourceParam.Size = dbParameter.Size.Value;
            }
            dbCommand.Parameters.Add(sourceParam);
        }

    }
}
