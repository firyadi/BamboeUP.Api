using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class WorkItemRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IWorkItemRepository
    {
        private readonly string _table = dialect.Qualify("app", "WorkItem");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<WorkItem?> GetWorkItemAsync(Guid workItemGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_OwnerUserId.PersonName AS OwnerUserName

, j_OwnerUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_OwnerUserId ON a.OwnerUserId = j_OwnerUserId.PersonId

                WHERE a.WorkItemGuid = @workItemGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<WorkItem>(sql, new { workItemGuid });
        }

        public async Task<WorkItem?> GetByPersonGuidAndWorkItemGuidAsync(Guid personGuid, Guid workItemGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_OwnerUserId.PersonName AS OwnerUserName

, j_OwnerUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_OwnerUserId ON a.OwnerUserId = j_OwnerUserId.PersonId

                WHERE j_OwnerUserId.PersonGuid = @personGuid
                  AND a.WorkItemGuid = @workItemGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<WorkItem>(sql, new { personGuid, workItemGuid });
        }

        public async Task<IEnumerable<WorkItem>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_OwnerUserId.PersonName AS OwnerUserName

, j_OwnerUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_OwnerUserId ON a.OwnerUserId = j_OwnerUserId.PersonId

                WHERE j_OwnerUserId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.WorkItemId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<WorkItem>(sql, new { personGuid });
        }

        public async Task CreateWorkItemAsync(WorkItem workItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(workItem);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (WorkItemGuid, OwnerUserId, CreatedById, StatusId, CreatedTime, [WorkItemCode], [Title], [Description], [Module], [WorkItemType], [OwnerName], [WorkflowStatus], [Priority], [CompletionPercent], [Tags], [Framework], [DueDate], [Note])
                VALUES
                (@WorkItemGuid, @OwnerUserId, @CreatedById, @StatusId, @CreatedTime, @WorkItemCode, @Title, @Description, @Module, @WorkItemType, @OwnerName, @WorkflowStatus, @Priority, @CompletionPercent, @Tags, @Framework, @DueDate, @Note){dialect.ReturnIdentitySuffix("WorkItemId")}";
            workItem.WorkItemId = await conn.QuerySingleAsync<long>(sql, workItem, transaction);
        }

        public async Task UpdateWorkItemAsync(WorkItem workItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(workItem);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [OwnerUserId] = @OwnerUserId,
                    [WorkItemCode] = @WorkItemCode,
                    [Title] = @Title,
                    [Description] = @Description,
                    [Module] = @Module,
                    [WorkItemType] = @WorkItemType,
                    [OwnerName] = @OwnerName,
                    [WorkflowStatus] = @WorkflowStatus,
                    [Priority] = @Priority,
                    [CompletionPercent] = @CompletionPercent,
                    [Tags] = @Tags,
                    [Framework] = @Framework,
                    [DueDate] = @DueDate,
                    [Note] = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE WorkItemGuid = @WorkItemGuid";
            await conn.ExecuteAsync(sql, workItem, transaction);
        }

        public async Task SoftDeleteWorkItemAsync(WorkItem workItem, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(workItem, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE WorkItemGuid = @WorkItemGuid";

            await conn.ExecuteAsync(sql, workItem, transaction);
        }

        public async Task DeleteWorkItemAsync(Guid workItemGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE WorkItemGuid = @workItemGuid";
            await conn.ExecuteAsync(sql, new { workItemGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE OwnerUserId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<WorkItem>> SearchWorkItemAsync(
            string? workItemCode,
            string? workItemCodeSearchType,
            string? title,
            string? titleSearchType,
            string? description,
            string? descriptionSearchType,
            string? module,
            string? moduleSearchType,
            string? workItemType,
            string? workItemTypeSearchType,
            string? ownerName,
            string? ownerNameSearchType,
            string? workflowStatus,
            string? workflowStatusSearchType,
            string? priority,
            string? prioritySearchType,
            string? completionPercent,
            string? completionPercentSearchType,
            string? tags,
            string? tagsSearchType,
            string? framework,
            string? frameworkSearchType,
            string? dueDate,
            string? dueDateSearchType,
            string? note,
            string? noteSearchType,
            Guid personGuid, Guid workItemGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (personGuid != Guid.Empty)
            {
                whereClauses.Add("j_OwnerUserId.PersonGuid = @personGuid");
                parameters.Add("@personGuid", personGuid);
            }
            if (workItemGuid != Guid.Empty)
            {
                whereClauses.Add("a.WorkItemGuid = @workItemGuid");
                parameters.Add("@workItemGuid", workItemGuid);
            }

            if (!string.IsNullOrWhiteSpace(workItemCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkItemCode", "@workItemCode", workItemCodeSearchType, parameters, "workItemCode", workItemCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                var param = SqlFilterHelper.BuildFilter("a.Title", "@title", titleSearchType, parameters, "title", title, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(module))
            {
                var param = SqlFilterHelper.BuildFilter("a.Module", "@module", moduleSearchType, parameters, "module", module, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(workItemType))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkItemType", "@workItemType", workItemTypeSearchType, parameters, "workItemType", workItemType, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(ownerName))
            {
                var param = SqlFilterHelper.BuildFilter("a.OwnerName", "@ownerName", ownerNameSearchType, parameters, "ownerName", ownerName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(workflowStatus))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkflowStatus", "@workflowStatus", workflowStatusSearchType, parameters, "workflowStatus", workflowStatus, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                var param = SqlFilterHelper.BuildFilter("a.Priority", "@priority", prioritySearchType, parameters, "priority", priority, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(completionPercent))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompletionPercent", "@completionPercent", completionPercentSearchType, parameters, "completionPercent", completionPercent, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(tags))
            {
                var param = SqlFilterHelper.BuildFilter("a.Tags", "@tags", tagsSearchType, parameters, "tags", tags, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(framework))
            {
                var param = SqlFilterHelper.BuildFilter("a.Framework", "@framework", frameworkSearchType, parameters, "framework", framework, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(dueDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.DueDate", "@dueDate", dueDateSearchType, parameters, "dueDate", dueDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_OwnerUserId.PersonName AS OwnerUserName

, j_OwnerUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_OwnerUserId ON a.OwnerUserId = j_OwnerUserId.PersonId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.WorkItemId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<WorkItem>(sql, parameters, transaction);
        }
    }
}
