using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Pudra.Connect.Application.Dtos.Products;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Infrastructure.Repositories;

public class ProductRepository(IConfiguration configuration) : IProductRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("ProductsConnection")!;


    public async Task<string?> GetRandomBarcodeAsync()
    {
        // Veritabanından rastgele bir tane, geçerli barkod seçer.
        const string sql = @"
        SELECT TOP 1
            IB.Barcode
        FROM
            PudraDB.dbo.prItemBarcode IB
        INNER JOIN PudraDB.dbo.cdItem I ON I.ItemCode = IB.ItemCode AND I.IsBlocked = 0
        WHERE 
            --IB.BarcodeTypeCode = 'DEF' AND 
            IB.Barcode IS NOT NULL AND IB.Barcode <> ''
        ORDER BY
            NEWID();
    ";

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<string>(sql);
    }

    public async Task<ProductResultDto?> GetByBarcodeAsync(string barcode)
    {
        // --- SENİN VERECEĞİN BARKOD SORGUSU BURAYA GELECEK ---
        const string sql = @"
       SELECT
                    IB.ItemCode as ProductCode,
                    ID.ItemDescription as Name,
                    
                    ISNULL(IBP_R.Price,0) as RetailPrice,
                    ISNULL(IBP_WS.Price,0) as WholesalePrice
               
                    
                FROM
                    PudraDB.dbo.prItemBarcode IB
                INNER JOIN PudraDB.dbo.cdItem I ON I.IsBlocked = 0 AND I.ItemCode = IB.ItemCode
                INNER JOIN PudraDB.dbo.cdItemDesc ID ON ID.ItemCode = I.ItemCode AND ID.LangCode = 'TR'
                LEFT JOIN PudraDB.dbo.cdColorDesc CD ON CD.ColorCode = IB.ColorCode AND CD.LangCode = 'TR'
                LEFT JOIN PudraDB.dbo.prItemBasePrice IBP_R ON IBP_R.ItemCode = I.ItemCode AND IBP_R.CountryCode ='TR' AND IBP_R.BasePriceCode = 7
                LEFT JOIN PudraDB.dbo.prItemBasePrice IBP_WS ON IBP_WS.ItemCode = I.ItemCode AND IBP_WS.CountryCode ='TR' AND IBP_WS.BasePriceCode = 3
                WHERE
                   -- IB.BarcodeTypeCode = 'DEF' AND CONTAINS(ID.ItemDescription, @SearchTerm);
                    -- Alternatif (daha yavaş): AND 
                    --BarcodeTypeCode = 'DEF' AND  
                    Barcode = @Barcode
    ";
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<ProductResultDto>(sql, new { Barcode = barcode });
    }
    public async Task<ProductResultDto?> GetDetailByCodeAsync(string productCode)
    {
        // Bu metot, bir ürün koduna ait ana bilgileri ve fiyatları getirir.
        // GetByBarcodeAsync ile çok benzer, sadece WHERE koşulu farklı.
        const string sql = @"
         SELECT TOP 1
            U.ItemCode AS ProductCode,
            ID.ItemDescription AS Name,
            ISNULL(F.RetailPrice, 0) as RetailPrice,
            ISNULL(F.WholesalePrice, 0) as WholesalePrice
        FROM
            PudraDB.dbo.cdItem U
        INNER JOIN PudraDB.dbo.cdItemDesc ID ON U.ItemCode = ID.ItemCode AND ID.LangCode = 'TR'
        LEFT JOIN 
            (
                SELECT
                    ItemCode,
                    MAX(CASE WHEN BasePriceCode = 7 THEN Price ELSE 0 END) as RetailPrice,
                    MAX(CASE WHEN BasePriceCode = 3 THEN Price ELSE 0 END) as WholesalePrice
                FROM PudraDB.dbo.prItemBasePrice
                WHERE CountryCode = 'TR'
                GROUP BY ItemCode
            ) AS F ON U.ItemCode = F.ItemCode
        WHERE 
            U.ItemCode = @ProductCode AND U.IsBlocked = 0
    ";

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<ProductResultDto>(sql, new { ProductCode = productCode });
    }
    public async Task<IEnumerable<ProductResultDto>> SearchByNameAsync(string name)
    {
        // --- SENİN VERECEĞİN ÜRÜN ADI ARAMA SORGUSU BURAYA GELECEK ---
        const string sql = @"
       SELECT
                    I.ItemCode as ProductCode,
                    ID.ItemDescription as Name,
                  
                    ISNULL(IBP_R.Price,0) as RetailPrice,
                    ISNULL(IBP_WS.Price,0) as WholesalePrice,
                    
                    GETDATE() as OperationDate
                FROM
                      PudraDB.dbo.cdItem I 
                INNER JOIN PudraDB.dbo.cdItemDesc ID ON ID.ItemCode = I.ItemCode AND ID.LangCode = 'TR'
                
                LEFT JOIN PudraDB.dbo.prItemBasePrice IBP_R ON IBP_R.ItemCode = I.ItemCode AND IBP_R.CountryCode ='TR' AND IBP_R.BasePriceCode = 7
                LEFT JOIN PudraDB.dbo.prItemBasePrice IBP_WS ON IBP_WS.ItemCode = I.ItemCode AND IBP_WS.CountryCode ='TR' AND IBP_WS.BasePriceCode = 3
                WHERE
                     I.IsBlocked = 0 AND
                     ID.ItemDescription LIKE '%' + @SearchTerm + '%'
    ";
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<ProductResultDto>(sql, new { SearchTerm = $"%{name}%" });
    }
}