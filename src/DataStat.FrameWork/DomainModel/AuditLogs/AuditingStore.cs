using DataStat.FrameWork.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataStat.FrameWork.DomainModel.AuditLogs
{
    public class AuditingStore
    {
        private readonly IRepository<AuditLog,long> _auditLogRepository;

        /// <summary>
        /// Creates  a new <see cref="AuditingStore"/>.
        /// </summary>
        public AuditingStore(IRepository<AuditLog, long> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public virtual Task SaveAsync(AuditLog entity)
        {
            return _auditLogRepository.InsertAsync(entity);
        }
    }
}