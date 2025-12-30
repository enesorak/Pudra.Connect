using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Pudra.Connect.Application.Dtos.Dashboard;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly string _connectionString;

    public DashboardRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ProductsConnection");
    }


    public async Task<IEnumerable<DailyActivityDto>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        // SENİN VERDİĞİN SORGUNUN TARİH FİLTRESİ EKLENMİŞ HALİ
        const string sql = @"
              SELECT
                    ISNULL(SP.FirstLastName,'GENEL') as Seller,
                    AI.StoreCode + ' ' + CASE WHEN PH.ProductHierarchyLevel01 = 'Dış Giyim' THEN 'DG' ELSE '' END as StoreId,
					
                    AI.InvoiceDate,
                    AI.IsReturn,
                    SUM(AI.Qty1) as Quantity,
                    SUM(AI.Doc_NetAmount) as NetAmountWithTax,
					  SUM(Doc_TDiscountVITotal) + SUM(Doc_LDiscountVITotal) as TotalDiscount

                FROM 
                    AllInvoices AI
				INNER JOIN cdItem I ON I.ItemCode = AI.ItemCode aND I.ItemTypeCode = AI.ItemTypeCode 
				INNER JOIN ProductHierarchy('TR') PH ON PH.ProductHierarchyID = I.ProductHierarchyID
                INNER JOIN 
                    cdSalesperson SP ON SP.SalespersonCode = AI.SalespersonCode AND AI.StoreCode = SP.StoreCode
                WHERE 
                    AI.ProcessCode = 'R' 
                    AND AI.IsCompleted = 1
                    AND AI.InvoiceDate BETWEEN @StartDate AND @EndDate
                GROUP BY 
                    SP.FirstLastName,
                    AI.StoreCode, 
                    AI.InvoiceDate,
					PH.ProductHierarchyLevel01,
                    AI.IsReturn; 
            ";

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<DailyActivityDto>(sql, new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<KpiDto> GetTodaysKpisAsync()
    {
        const string sql = @"
            SELECT
                ISNULL(SUM(CASE WHEN IsReturn = 0 THEN Doc_NetAmount ELSE 0 END), 0) as TodaysTurnover,
                ISNULL(SUM(CASE WHEN IsReturn = 0 THEN Qty1 ELSE 0 END), 0) as TodaysSaleCount,
                ISNULL(SUM(CASE WHEN IsReturn = 1 THEN Qty1 ELSE 0 END), 0) as TodaysReturnCount
                ISNULL(SUM(ToplamIskonto), 0) as TodaysDiscount -- ToplamIskonto kolonu varsayımsaldır

            FROM AllInvoices
            WHERE ProcessCode = 'R' AND IsCompleted = 1 AND CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE);
        ";
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleAsync<KpiDto>(sql);
    }

    // YENİ METOT: Son 7 günün ciro grafiği için sorgu
    public async Task<IEnumerable<TrendPointDto>> GetWeeklyTrendAsync()
    {
        const string sql = @"
            SELECT
                CAST(InvoiceDate AS DATE) as Date,
                SUM(Doc_NetAmount) as Amount
            FROM AllInvoices
            WHERE 
                ProcessCode = 'R' 
                AND IsCompleted = 1
                AND IsReturn = 0
                AND InvoiceDate >= DATEADD(day, -6, CAST(GETDATE() AS DATE))
            GROUP BY CAST(InvoiceDate AS DATE)
            ORDER BY Date;
        ";
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<TrendPointDto>(sql);
    }


    public async Task<IEnumerable<CategoryPerformanceDto>> GetCategoryPerformanceAsync(DateTime startDate,
        DateTime endDate)
    {
        // --- SENİN VERECEĞİN SQL SORGUSU BURAYA GELECEK ---
        const string sql = @"
        SELECT
            PF.ProductHierarchyLevel01,
                ISNULL(SUM(CASE WHEN IsReturn = 0 THEN Doc_NetAmount ELSE 0 END), 0) as TodaysTurnover,
                ISNULL(SUM(CASE WHEN IsReturn = 0 THEN Qty1 ELSE 0 END), 0) as TodaysSaleCount,
                ISNULL(SUM(CASE WHEN IsReturn = 1 THEN Qty1 ELSE 0 END), 0) as TodaysReturnCount
            FROM AllInvoices
			INNER JOIN ProductFilterWithDescription('TR') PF ON PF.ProductCode = AllInvoices.ItemCode
            WHERE ProcessCode = 'R'
			AND IsCompleted = 1 
			AND CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)
			GROUP BY   PF.ProductHierarchyLevel01
    ";

        await using var connection = new SqlConnection(_connectionString);
       
         return await connection.QueryAsync<CategoryPerformanceDto>(sql, new { StartDate = startDate, EndDate = endDate });

        
    }
    
    
    public async Task<IEnumerable<StoreKpiDto>> GetTodaysKpisByStoreAsync()
    {
        const string sql = @"
         SELECT
                    AI.StoreCode + ' ' + CASE WHEN PH.ProductHierarchyLevel01 = 'Dış Giyim' THEN 'DG' ELSE '' END as StoreId,
            ISNULL(SUM(CASE WHEN  AI.IsReturn = 0 THEN  AI.Doc_NetAmount ELSE 0 END), 0) as TodaysTurnover,
            ISNULL(SUM(CASE WHEN  AI.IsReturn = 0 THEN 1 ELSE 0 END), 0) as TodaysSaleCount,
            ISNULL(SUM(CASE WHEN  AI.IsReturn = 1 THEN  AI.Doc_NetAmount ELSE 0 END), 0) as TodaysReturnAmount,
            ISNULL(SUM(CASE WHEN  AI.IsReturn = 1 THEN 1 ELSE 0 END), 0) as TodaysReturnCount,
			ISNULL(SUM(Doc_TDiscountVITotal + Doc_LDiscountVITotal), 0) as TodaysDiscount       
			FROM AllInvoices AI
			INNER JOIN cdItem I ON I.ItemCode = AI.ItemCode aND I.ItemTypeCode = AI.ItemTypeCode 
				INNER JOIN ProductHierarchy('TR') PH ON PH.ProductHierarchyID = I.ProductHierarchyID
             
        WHERE ProcessCode = 'R' AND IsCompleted = 1 AND CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)
        GROUP BY StoreCode,ProductHierarchyLevel01
        ORDER BY StoreCode;
    ";
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<StoreKpiDto>(sql);
    }
}