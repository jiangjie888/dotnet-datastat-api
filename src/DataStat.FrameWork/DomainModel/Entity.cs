using System;

namespace DataStat.FrameWork.DomainModel
{
    /// <summary>
    /// 可以持久到数据库的业务类都要继承的基类
    /// </summary>
    [Serializable]
    public abstract class Entity<TPrimaryKey>
    {
        public Entity()
        {
            //Id = Guid.NewGuid().ToString();
            CreationTime = DateTime.Now;
            //IsDelete = false;
            //UpdateTime = DateTime.Now;
        }

        public virtual TPrimaryKey Id { get; set; }

        //public virtual bool IsDelete { get; set; }


        public virtual DateTime CreationTime { get; set; }

        public virtual long? CreatorUserId { get; set; }

        public virtual DateTime? LastModificationTime { get; set; }

        public virtual long? LastModifierUserId { get; set; }

    }
}
