using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Threading.Tasks;
using DataStat.DBUtil;
using Dapper;
using Dapper.Contrib.Extensions;
using DataStat.FrameWork.DomainModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataStat.FrameWork.Repository
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
    }

    

    public class RepositoryImpl<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        private string _tableName { set; get; }

        private ConnectionStringSettings connectionStringSettings;

        public RepositoryImpl()
        {
            _tableName = typeof(TEntity).GetAttributeValue((System.ComponentModel.DataAnnotations.Schema.TableAttribute ta) => ta.Name);
            _tableName = string.IsNullOrWhiteSpace(_tableName) ? typeof(TEntity).Name: _tableName;
        }

        public ConnectionStringSettings DefaultConnectionStringSettings
        {
            set
            {
                connectionStringSettings = value;
            }
        }


        //protected virtual ISession Session
        //{
        //    get { return SessionBuilder.CreateSession(); }
        //}

        //#region IRepository<T> 成员
        //public virtual T Load(string id)
        //{
        //    try
        //    {
        //        T reslut = Session.Load<T>(id);
        //        if (reslut == null)
        //            throw new RepositoryException("返回实体为空");
        //        else
        //            return reslut;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new RepositoryException("获取实体失败", ex);
        //    }
        //}

        //private T GetInNewSession(string id)
        //{
        //    ISession Session = SessionBuilder.CreateSessionFix();
        //    T result = Session.Get<T>(id);
        //    Session.Close();
        //    return result;
        //}

        public async virtual Task<TEntity> GetAsync(TPrimaryKey id)
        {
            try
            {
                var cmdText = string.Format(@"select * from {0} where Id=@Id;", _tableName);
                using (var conn = new DBConnection().GetDbConnection(connectionStringSettings))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var reslut = await conn.QuerySingleAsync<TEntity>(cmdText, new { @Id = id }, commandType: CommandType.Text);
                    if (reslut == null)
                        throw new Exception("操作失败：返回实体为空");
                    else
                        return reslut;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败：DB获取实体失败");
            }

        }

        public async virtual Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            try
            {
                var cmdText = string.Format(@"select * from {0} where Id=@Id;", _tableName);
                using (var conn = new DBConnection().GetDbConnection(connectionStringSettings))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var reslut = await conn.QueryFirstOrDefaultAsync<TEntity>(cmdText, new { @Id = id }, commandType: CommandType.Text);
                    return reslut;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败：DB获取实体失败");
            }

        }

        public async virtual Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                var cmdText = string.Format(@"select * from {0};", _tableName);
                using (var conn = new DBConnection().GetDbConnection(connectionStringSettings))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var reslut = await conn.QueryAsync<TEntity>(cmdText);
                    return reslut;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败：DB数据获取失败");
            }
        }
        

        public async virtual Task InsertAsync(TEntity entity)
        {
            try
            {
                using (var conn = new DBConnection().GetDbConnection(connectionStringSettings))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    int result = await conn.InsertAsync(entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败：DB数据插入失败");
            }
        }

        public async virtual Task UpdateAsync(TEntity entity)
        {
            try
            {
                using (var conn = new DBConnection().GetDbConnection(connectionStringSettings))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    bool result = await conn.UpdateAsync(entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败：DB数据更新失败");
            }
        }

        public async virtual Task DeleteAsync(TPrimaryKey id)
        {
            try
            {
                using (var conn = new DBConnection().GetDbConnection(connectionStringSettings))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var cmdText = string.Format(@"delete from {0} where Id=@Id;", _tableName);
                    int result = await conn.ExecuteAsync(cmdText, new { @Id = id });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败：DB数据删除失败");
            }
        }
        //#endregion

        ////判断字段的值是否存在 如果是插入id赋值-1或者new Guid,如果是修改id赋值 要修改项的值

        //public virtual bool IsFieldExist(string fieldName, string fieldValue, string id, string where)
        //{
        //    if (!string.IsNullOrEmpty(where))
        //        where = @" and " + where;
        //    var query = Session.CreateQuery(
        //        string.Format(@"select count(*) from {0} as o where o.{1}='{2}' and o.Id<>'{3}'" + where,
        //        typeof(T).Name,
        //        fieldName,
        //        fieldValue, id));

        //    return query.UniqueResult<long>() > 0;
        //}

        ///// <summary>
        ///// 通用的检查数据库值是否已存在,传入where字串
        ///// </summary>
        ///// <param name="where"></param>
        ///// <returns></returns>
        //public virtual bool IsFieldExist(string where)
        //{
        //    if (!string.IsNullOrEmpty(where))
        //        where = @" where " + where;
        //    var query = Session.CreateQuery(
        //        string.Format(@"select count(*) from {0} as o  " + where,
        //        typeof(T).Name));

        //    return query.UniqueResult<long>() > 0;
        //}

        

        ///// <summary>
        ///// 查询所有泛型对象集合,带分页,过滤条件,排序功能,适用于Grid展示,以后只要继承了此对象的类均可直接调用
        ///// </summary>
        ///// <param name="start"></param>
        ///// <param name="limit"></param>
        ///// <param name="sort"></param>
        ///// <param name="dir"></param>
        ///// <param name="filters"></param>
        ///// <param name="total"></param>
        ///// <returns></returns>
        public IEnumerable<TEntity> QueryAllbyPage(int start, int limit, string sort, string dir, List<DataFilter> filters, out long total)
        {
            var strFilter = GetHqlstrByExtFilter(filters, "t");
            sort = "t." + sort;
            string where = string.IsNullOrEmpty(strFilter) ? string.Empty : " where " + strFilter;

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select count(1) from {0} " + where, _tableName);
            sql.AppendFormat(@"select * FROM {0} " + where + " order by {1} {2} limit {3}, {4};", _tableName, sort, dir, start, limit);

            try
            {
                using (var conn = new DBConnection().GetDbConnection(connectionStringSettings))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var reader = conn.QueryMultiple(sql.ToString());
                    total = reader.ReadFirst<int>();
                    return reader.Read<TEntity>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作失败：DB数据获取失败");
            }
        }

        #region
        ////处理字段过滤
        public string GetHqlstrByExtFilter(List<DataFilter> filters, string a)
        {
            if ((filters == null) || (filters.Count == 0))
                return string.Empty;
            StringBuilder result = new StringBuilder();
            //type=string
            var stringList = from f in filters where f.type == "string" select f; //List类的方法
            foreach (var i in stringList)
            {
                if ((i.comparison != null) && (i.comparison.Trim().Length > 0)) //当字串有比较符号时,用比较符号,以提升效率.
                {
                    result.Append(a + "." + i.field + this.GetComparison(i.comparison) + "'" + i.value + "'" + " and ");
                }
                else
                    result.Append(a + "." + i.field + " like " + "'%" + i.value + "%'" + " and ");
            }
            //type=boolean
            var booleanList = from f in filters where f.type == "boolean" select f;
            foreach (var i in booleanList)
            {
                result.Append(a + "." + i.field + "=" + i.value + " and ");
            }
            //type=numeric
            var numericList = from f in filters where f.type == "numeric" group f by f.field into g select g;
            foreach (var i in numericList)
            {
                result.Append("( ");
                string iiStr = string.Empty;
                foreach (var ii in i)
                {
                    iiStr += a + "." + ii.field + GetComparison(ii.comparison) + ii.value + " and ";
                }
                result.Append(iiStr.Substring(0, iiStr.Length - 4));
                result.Append(" )");
                result.Append(" and ");
            }
            //type=date
            var dateList = from f in filters where f.type == "date" group f by f.field into g select g;
            foreach (var i in dateList)
            {
                result.Append("( ");
                string iiStr = string.Empty;
                foreach (var ii in i)
                {
                    //对于SQL Servre数据库不能使用to_date函数
                    iiStr += a + "." + ii.field + GetComparison(ii.comparison) + " to_date('" + ii.value + "', 'mm/dd/yyyy')" + " and ";
                    //iiStr += a + "." + ii.field + GetComparison(ii.comparison) + " '" + ii.value + "' " + " and ";
                }
                result.Append(iiStr.Substring(0, iiStr.Length - 4));
                result.Append(" )");
                result.Append(" and ");
            }
            //type=list  :["1","2"]
            var listList = from f in filters where f.type == "list" select f;
            foreach (var i in listList)
            {
                result.Append(a + "." + i.field + " in " + i.value.Replace("[", "( ").Replace("]", " )").Replace("\"", "'") + " and ");
            }

            return result.ToString().Substring(0, result.Length - 4);
        }

        private string GetComparison(string comparison)
        {
            string res = string.Empty;
            switch (comparison)
            {
                case "lt":
                    res = "<";
                    break;
                case "gt":
                    res = ">";
                    break;
                case "eq":
                    res = "=";
                    break;
                case "lt and eq":
                    res = "<=";
                    break;
                case "gt and eq":
                    res = ">=";
                    break;
            }
            return res;
        }
        #endregion

        #region
        /// <summary>
        /// dapper通用分页方法
        /// </summary>
        /// <typeparam name="T">泛型集合实体类</typeparam>
        /// <param name="conn">数据库连接池连接对象</param>
        /// <param name="files">列</param>
        /// <param name="tableName">表</param>
        /// <param name="where">条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">当前页显示条数</param>
        /// <param name="total">结果集总数</param>
        /// <returns></returns>
        //public static IEnumerable<T> GetPageList<T>(IDbConnection conn, string files, string tableName, string where, string orderby, int pageIndex, int pageSize, out int total)
        //{
        //    int skip = 1;
        //    if (pageIndex > 0)
        //    {
        //        skip = (pageIndex - 1) * pageSize + 1;
        //    }
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("SELECT COUNT(1) FROM {0} where {1};", tableName, where);
        //    sb.AppendFormat(@"SELECT  {0}
        //                        FROM(SELECT ROW_NUMBER() OVER(ORDER BY {3}) AS RowNum,{0}
        //                                  FROM  {1}
        //                                  WHERE {2}
        //                                ) AS result
        //                        WHERE  RowNum >= {4}   AND RowNum <= {5}
        //                        ORDER BY {3}", files, tableName, where, orderby, skip, pageIndex * pageSize);
        //    using (var reader = conn.QueryMultiple(sb.ToString()))
        //    {
        //        total = reader.ReadFirst<int>();
        //        return reader.Read<T>();
        //    }
        #endregion

    }

}
