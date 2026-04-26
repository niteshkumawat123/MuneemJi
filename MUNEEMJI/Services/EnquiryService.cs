using MUNEEMJI.Models;
using Npgsql;
using Dapper;

namespace MUNEEMJI.Services
{
    public class EnquiryService : IEnquiryService
    {
        private readonly string _connectionString;

        public EnquiryService()
        {
            _connectionString = DbConfig.ConnectionString;
        }

        public async Task<List<Enquiry>> GetAllAsync(int companyId, string sectionType)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT enquiry_id AS EnquiryId, company_id AS CompanyId, enquiry_source AS EnquirySource,
                               customer_name AS CustomerName, customer_phone AS CustomerPhone, customer_email AS CustomerEmail,
                               subject, message, status, reason, assigned_to AS AssignedTo,
                               created_at AS CreatedAt, updated_at AS UpdatedAt, is_deleted AS IsDeleted, section_type AS SectionType
                        FROM enquiries
                        WHERE company_id = @companyId AND section_type = @sectionType AND is_deleted = false
                        ORDER BY created_at DESC";

            var result = await conn.QueryAsync<Enquiry>(sql, new { companyId, sectionType });
            return result.ToList();
        }

        public async Task<Enquiry?> GetByIdAsync(int enquiryId, int companyId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT enquiry_id AS EnquiryId, company_id AS CompanyId, enquiry_source AS EnquirySource,
                               customer_name AS CustomerName, customer_phone AS CustomerPhone, customer_email AS CustomerEmail,
                               subject, message, status, reason, assigned_to AS AssignedTo,
                               created_at AS CreatedAt, updated_at AS UpdatedAt, is_deleted AS IsDeleted, section_type AS SectionType
                        FROM enquiries
                        WHERE enquiry_id = @enquiryId AND company_id = @companyId AND is_deleted = false";

            return await conn.QuerySingleOrDefaultAsync<Enquiry>(sql, new { enquiryId, companyId });
        }

        public async Task<int> CreateAsync(Enquiry enquiry)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"INSERT INTO enquiries
                            (company_id, enquiry_source, customer_name, customer_phone, customer_email,
                             subject, message, status, assigned_to, created_at, updated_at, is_deleted, section_type)
                        VALUES
                            (@CompanyId, @EnquirySource, @CustomerName, @CustomerPhone, @CustomerEmail,
                             @Subject, @Message, @Status, @AssignedTo, NOW(), NOW(), false, @SectionType)
                        RETURNING enquiry_id";

            return await conn.ExecuteScalarAsync<int>(sql, enquiry);
        }

        public async Task<bool> UpdateAsync(Enquiry enquiry)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"UPDATE enquiries SET
                            customer_name = @CustomerName, customer_phone = @CustomerPhone, customer_email = @CustomerEmail,
                            subject = @Subject, message = @Message, enquiry_source = @EnquirySource,
                            assigned_to = @AssignedTo, updated_at = NOW()
                        WHERE enquiry_id = @EnquiryId AND company_id = @CompanyId AND is_deleted = false";

            return await conn.ExecuteAsync(sql, enquiry) > 0;
        }

        public async Task<bool> SoftDeleteAsync(int enquiryId, int companyId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"UPDATE enquiries SET is_deleted = true, updated_at = NOW()
                        WHERE enquiry_id = @enquiryId AND company_id = @companyId";

            return await conn.ExecuteAsync(sql, new { enquiryId, companyId }) > 0;
        }

        public async Task<bool> UpdateStatusAsync(int enquiryId, int companyId, string status, string? reason)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var tx = await conn.BeginTransactionAsync();
            try
            {
                // Get old status for audit
                var oldStatus = await conn.ExecuteScalarAsync<string>(
                    "SELECT status FROM enquiries WHERE enquiry_id = @enquiryId AND company_id = @companyId",
                    new { enquiryId, companyId }, tx);

                // Update status
                var sql = @"UPDATE enquiries SET status = @status, reason = @reason, updated_at = NOW()
                            WHERE enquiry_id = @enquiryId AND company_id = @companyId AND is_deleted = false";
                var rows = await conn.ExecuteAsync(sql, new { status, reason, enquiryId, companyId }, tx);

                // Audit log
                if (rows > 0)
                {
                    var logSql = @"INSERT INTO enquiry_status_logs (enquiry_id, old_status, new_status, reason, changed_at)
                                   VALUES (@enquiryId, @oldStatus, @status, @reason, NOW())";
                    await conn.ExecuteAsync(logSql, new { enquiryId, oldStatus, status, reason }, tx);
                }

                await tx.CommitAsync();
                return rows > 0;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
