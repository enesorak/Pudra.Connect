using Microsoft.Extensions.Caching.Memory;
using Pudra.Connect.Application.Dtos.Dashboard;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Infrastructure.Repositories;

 /// <summary>
    /// Admin Dashboard'u için gerekli olan tüm raporlama ve analitik verilerini üreten ana servis.
    /// Performansı artırmak için geçmişe dönük verilerde akıllı bir önbellekleme (caching) stratejisi kullanır.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        // Bağımlılıklar (Dependencies)
        private readonly IDashboardRepository _dashboardRepository; // Harici veritabanına sorgu atmak için
        private readonly IMemoryCache _memoryCache; // Verileri bellekte geçici olarak saklamak için

        /// <summary>
        /// Servisin constructor'ı. Gerekli bağımlılıklar Dependency Injection ile otomatik olarak sağlanır.
        /// </summary>
        /// <param name="dashboardRepository">Veritabanı sorgularını yürüten repository.</param>
        /// <param name="memoryCache">Bellek içi önbellek servisi.</param>
        public DashboardService(IDashboardRepository dashboardRepository, IMemoryCache memoryCache)
        {
            _dashboardRepository = dashboardRepository;
            _memoryCache = memoryCache;
        }

      
        
        public Task<IEnumerable<StoreKpiDto>> GetKpisAsync()
        {
            return _dashboardRepository.GetTodaysKpisByStoreAsync();
        }

        /// <summary>
        /// Son 7 günün ciro trendini, bir çizgi grafiği için uygun formatta alır.
        /// Bu metot da genellikle anlık veri beklendiği için önbellek kullanmaz.
        /// </summary>
        /// <returns>Her bir gün için tarih ve ciro bilgisi içeren bir liste.</returns>
        public Task<IEnumerable<TrendPointDto>> GetWeeklyTrendAsync()
        {
            return _dashboardRepository.GetWeeklyTrendAsync();
        }

        /// <summary>
        /// Belirtilen tarih aralığı için detaylı mağaza ve personel bazlı raporları oluşturur.
        /// Bu metot, akıllı önbellekleme mantığını kullanarak performansı optimize eder.
        /// </summary>
        /// <param name="startDate">Raporun başlangıç tarihi.</param>
        /// <param name="endDate">Raporun bitiş tarihi.</param>
        /// <returns>İşlenmiş ve gruplanmış detaylı rapor verisi.</returns>
        public async Task<DetailedReportDto> GetDetailedReportAsync(DateTime startDate, DateTime endDate)
        {
            // 1. Adım: Önbelleği kullanarak veya veritabanından çekerek ham aktivite verisini al.
            var activities = await GetActivitiesWithCacheAsync(startDate, endDate);

            // 2. Adım: Alınan bu ham veriyi, istenen rapor formatına dönüştür.
            var report = new DetailedReportDto
            {
                // Mağaza raporunu oluştur: Mağazaya göre grupla, satış ve iadeleri ayrı ayrı topla.
                StoreReport = activities
                    .GroupBy(a => a.StoreId)
                    .Select(g => new StorePerformanceDto {
                        StoreId = g.Key,
                        TotalSales = g.Where(s => !s.IsReturn).Sum(s => s.NetAmountWithTax),
                        SaleCount = g.Where(s => !s.IsReturn).Sum(s => s.Quantity),
                        TotalReturns = g.Where(s => s.IsReturn).Sum(s => s.NetAmountWithTax),
                        TotalDiscount = g.Sum(s => s.TotalDiscount),
                        ReturnCount = g.Where(s => s.IsReturn).Sum(s => s.Quantity)
                    }).OrderBy(s => s.StoreId).ToList(),

                // Personel raporunu oluştur: Personele göre grupla, sadece satışlarını topla ve en çok satandan en aza doğru sırala.
                SellerReport = activities
                    .Where(a => !a.IsReturn) // Sadece satışları dikkate al
                    .GroupBy(a => a.Seller)
                    .Select(g => new SellerPerformanceDto {
                        Seller = g.Key,
                        TotalSales = g.Sum(s => s.NetAmountWithTax),
                        SaleCount = g.Sum(s => s.Quantity)
                    }).OrderByDescending(s => s.TotalSales).ToList()
            };

            return report;
        }

        /// <summary>
        /// Akıllı önbellekleme mantığını uygulayan özel metot.
        /// Strateji: "Bugün" her zaman canlı, "geçmiş" ise önbellekten okunur.
        /// </summary>
        private async Task<List<DailyActivityDto>> GetActivitiesWithCacheAsync(DateTime startDate, DateTime endDate)
        {
            var today = DateTime.UtcNow.Date;
            var allResults = new List<DailyActivityDto>();

            // 1. Bugünün verisini her zaman canlı olarak veritabanından çek (eğer tarih aralığı bugünü içeriyorsa).
            if (endDate.Date >= today)
            {
                var todayData = await _dashboardRepository.GetActivitiesByDateRangeAsync(today, endDate);
                allResults.AddRange(todayData);
            }
            
            // 2. Sorgulanacak geçmiş tarih aralığını hesapla.
            var historicalEndDate = (endDate.Date < today) ? endDate : today.AddDays(-1);

            if (startDate.Date <= historicalEndDate.Date)
            {
                var missingDays = new List<DateTime>();
                // 3. Her bir geçmiş gün için ÖNCE önbelleği kontrol et.
                for (var day = startDate.Date; day <= historicalEndDate.Date; day = day.AddDays(1))
                {
                    string cacheKey = $"DailyActivity_{day:yyyy-MM-dd}";
                    if (_memoryCache.TryGetValue(cacheKey, out List<DailyActivityDto>? cachedData))
                    {
                        // Cache'te varsa, direkt sonuca ekle.
                        if (cachedData != null) allResults.AddRange(cachedData);
                    }
                    else
                    {
                        // Cache'te yoksa, bu günü daha sonra veritabanından istemek üzere listeye ekle.
                        missingDays.Add(day);
                    }
                }

                // 4. Eğer önbellekte bulunamayan günler varsa, onlar için toplu bir sorgu at.
                if (missingDays.Any())
                {
                    var newFetchedData = await _dashboardRepository.GetActivitiesByDateRangeAsync(missingDays.Min(), missingDays.Max());
                    
                    // 5. Gelen yeni verileri hem ana sonuç listesine ekle, hem de gün bazında önbelleğe kaydet.
                    foreach (var dayGroup in newFetchedData.GroupBy(d => d.InvoiceDate.Date))
                    {
                        string cacheKey = $"DailyActivity_{dayGroup.Key:yyyy-MM-dd}";
                        var dailyData = dayGroup.ToList();
                        allResults.AddRange(dailyData);
                        _memoryCache.Set(cacheKey, dailyData, TimeSpan.FromHours(24)); // Veriyi 24 saatliğine cache'le
                    }
                }
            }

            return allResults;
        }
        
        /// <summary>
        /// Kategori bazlı performans raporunu, önbellek mekanizmasını kullanarak alır.
        /// </summary>
        public async Task<IEnumerable<CategoryPerformanceDto>> GetCategoryPerformanceAsync(DateTime startDate, DateTime endDate)
        {
            // 1. Her tarih aralığı için benzersiz bir önbellek anahtarı (cache key) oluştur.
            string cacheKey = $"CategoryPerformance_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}";

            // 2. Önce önbellekte bu anahtarla bir veri var mı diye kontrol et.
            // out var cachedReport ifadesi, veri varsa onu cachedReport değişkenine atar.
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<CategoryPerformanceDto> cachedReport))
            {
                // Eğer veri önbellekte varsa, veritabanına hiç gitmeden, sonucu anında döndür.
                return cachedReport;
            }

            // 3. Eğer veri önbellekte yoksa (Cache Miss):
            // Veritabanına git ve sorguyu çalıştır.
            var reportFromDb = await _dashboardRepository.GetCategoryPerformanceAsync(startDate, endDate);

            // 4. Veritabanından gelen sonucu, bir sonraki sefer için önbelleğe kaydet.
            // Burada, veriyi 15 dakika boyunca bellekte tutacak şekilde ayarlıyoruz.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15));
            
            _memoryCache.Set(cacheKey, reportFromDb, cacheEntryOptions);

            // 5. Sonucu kullanıcıya döndür.
            return reportFromDb;
        }
  
    }