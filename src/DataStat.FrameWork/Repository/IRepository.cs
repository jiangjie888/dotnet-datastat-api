using DataStat.DBUtil;
using DataStat.FrameWork.DomainModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataStat.FrameWork.Repository
{
    /// <summary>
    /// 仓储接口定义
    /// </summary>
    //public interface IRepository
    //{


    //}

    /// <summary>
    /// 定义泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    //public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : Entity<TPrimaryKey>
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        ConnectionStringSettings DefaultConnectionStringSettings { set; }

        //TEntity Load(string id);
        Task<TEntity> GetAsync(TPrimaryKey id);

        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);

        Task<IEnumerable<TEntity>> GetAllAsync();


        //void SaveOrUpdate(TEntity entity);
        //void Update(TEntity entity);

        Task InsertAsync(TEntity T);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TPrimaryKey id);

        IEnumerable<TEntity> QueryAllbyPage(int start, int limit, string sort, string dir, List<DataFilter> filters, out long total);
    }
}
