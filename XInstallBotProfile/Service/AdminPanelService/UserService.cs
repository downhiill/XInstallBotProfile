using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Security.Claims;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using XInstallBotProfile.Context;
using XInstallBotProfile.Controllers;
using XInstallBotProfile.Exepction;
using XInstallBotProfile.Generate;
using XInstallBotProfile.Models;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;
using XInstallBotProfile.Service.AdminPanelService.Models.Response;

namespace XInstallBotProfile.Service.AdminPanelService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<FileContentResult> ExportStatisticInExcel(GetStatisticRequest request)
        {
            // Получаем данные для экспорта
            var statisticData = await GetStatisticForExport(request);

            // Создаем Excel файл
            using (var package = new ExcelPackage())
            {
                // Создаем лист
                var worksheet = package.Workbook.Worksheets.Add("Статистика");

                // Заголовки столбцов
                worksheet.Cells[1, 1].Value = "Дата";
                worksheet.Cells[1, 2].Value = "Total";
                worksheet.Cells[1, 3].Value = "Ack";
                worksheet.Cells[1, 4].Value = "Win";
                worksheet.Cells[1, 5].Value = "ImpsCount";
                worksheet.Cells[1, 6].Value = "ClicksCount";
                worksheet.Cells[1, 7].Value = "StartsCount";
                worksheet.Cells[1, 8].Value = "CompletesCount";
                worksheet.Cells[1, 9].Value = "ShowRate %";
                worksheet.Cells[1, 10].Value = "CTR %";
                worksheet.Cells[1, 11].Value = "VTR";

                // Заполняем данные
                int row = 2;
                foreach (var stat in statisticData.UserStatistics)
                {
                    worksheet.Cells[row, 1].Value = stat.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = stat.Total;
                    worksheet.Cells[row, 3].Value = stat.Ack;
                    worksheet.Cells[row, 4].Value = stat.Win;
                    worksheet.Cells[row, 5].Value = stat.ImpsCount;
                    worksheet.Cells[row, 6].Value = stat.ClicksCount;
                    worksheet.Cells[row, 7].Value = stat.StartsCount;
                    worksheet.Cells[row, 8].Value = stat.CompletesCount;
                    worksheet.Cells[row, 9].Value = stat.ShowRate;
                    worksheet.Cells[row, 10].Value = stat.Ctr;
                    worksheet.Cells[row, 11].Value = stat.Vtr;
                    row++;
                }

                // Добавляем итоговые значения
                worksheet.Cells[row, 1].Value = "Итого";
                worksheet.Cells[row, 2].Value = statisticData.Total.Total;
                worksheet.Cells[row, 3].Value = statisticData.Total.Ack;
                worksheet.Cells[row, 4].Value = statisticData.Total.Win;
                worksheet.Cells[row, 5].Value = statisticData.Total.ImpsCount;
                worksheet.Cells[row, 6].Value = statisticData.Total.ClicksCount;
                worksheet.Cells[row, 7].Value = statisticData.Total.StartsCount;
                worksheet.Cells[row, 8].Value = statisticData.Total.CompletesCount;
                worksheet.Cells[row, 9].Value = statisticData.Total.ShowRate + "%";
                worksheet.Cells[row, 10].Value = statisticData.Total.Ctr + "%";
                worksheet.Cells[row, 11].Value = statisticData.Total.Vtr;

                // Добавляем средние значения
                row++;
                worksheet.Cells[row, 1].Value = "Средние";
                worksheet.Cells[row, 2].Value = statisticData.Averages.Total;
                worksheet.Cells[row, 3].Value = statisticData.Averages.Ack;
                worksheet.Cells[row, 4].Value = statisticData.Averages.Win;
                worksheet.Cells[row, 5].Value = statisticData.Averages.ImpsCount;
                worksheet.Cells[row, 6].Value = statisticData.Averages.ClicksCount;
                worksheet.Cells[row, 7].Value = statisticData.Averages.StartsCount;
                worksheet.Cells[row, 8].Value = statisticData.Averages.CompletesCount;
                worksheet.Cells[row, 9].Value = statisticData.Averages.ShowRate + "%";
                worksheet.Cells[row, 10].Value = statisticData.Averages.Ctr + "%";
                worksheet.Cells[row, 11].Value = statisticData.Averages.Vtr;

                // Форматируем заголовки
                using (var range = worksheet.Cells[1, 1, 1, 14])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Автонастройка ширины столбцов
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Преобразуем в массив байтов
                var fileContents = package.GetAsByteArray();

                // Возвращаем файл
                return new FileContentResult(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Статистика_{request.StartDate?.ToString("yyyy-MM-dd")}_{request.EndDate?.ToString("yyyy-MM-dd")}.xlsx"
                };
            }
        }

        public async Task<FileContentResult> ExportStatisticInPdf(GetStatisticRequest request)
        {
            // Включение режима отладки для выявления проблем с макетом
            QuestPDF.Settings.EnableDebugging = true;
            QuestPDF.Settings.License = LicenseType.Community;

            // Получаем данные для экспорта
            var statisticData = await GetStatisticForExport(request);

            // Загрузка изображения
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "svglogo_instal 2.png");
            byte[] watermarkImageBytes = File.ReadAllBytes(imagePath);

            // Создаем PDF документ
            var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Увеличиваем размер страницы и уменьшаем поля
                    page.Size(PageSizes.A2.Landscape()); // Горизонтальная ориентация
                    page.Margin(2, Unit.Centimetre); // Уменьшенные поля
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10)); // Уменьшаем размер шрифта

                    page.Content().Column(column =>
                    {
                        // Водяной знак (прозрачный и меньшего размера)
                        column.Item()
                            .AlignRight()
                            .AlignTop()
                            .Width(150) // Фиксированная ширина
                            .Image(watermarkImageBytes, ImageScaling.FitWidth);

                        // Заголовок
                        column.Item()
                            .PaddingBottom(15)
                            .Text("Статистика")
                            .SemiBold()
                            .FontSize(18)
                            .FontColor(Colors.Blue.Medium);

                        // Таблица с данными
                        column.Item()
                            .MinimalBox() // Предотвращает переполнение
                            .Table(table =>
                            {
                                // Уменьшаем ширину столбцов
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80); // Дата (фиксированная ширина)
                                    columns.RelativeColumn(1.5f); // Total
                                    columns.RelativeColumn(1.5f); // Ack
                                    columns.RelativeColumn(1.5f); // Win
                                    columns.RelativeColumn(1.5f); // ImpsCount
                                    columns.RelativeColumn(1.5f); // ClicksCount
                                    columns.RelativeColumn(1.5f); // StartsCount
                                    columns.RelativeColumn(1.5f); // CompletesCount
                                    columns.RelativeColumn(1.5f); // ShowRate
                                    columns.RelativeColumn(1.5f); // CTR
                                    columns.RelativeColumn(1.5f); // VTR
                                    columns.RelativeColumn(2); // Тип рекламы (шире)
                                });

                                // Заголовки столбцов (уменьшенный шрифт)
                                table.Header(header =>
                                {
                                    foreach (var headerName in new[] { "Дата", "Total", "Ack", "Win", "ImpsCount", "ClicksCount", "StartsCount", "CompletesCount", "ShowRate %", "CTR %", "VTR", "Тип рекламы" })
                                    {
                                        header.Cell()
                                            .Background(Colors.Grey.Lighten3)
                                            .PaddingVertical(5)
                                            .Text(headerName)
                                            .FontSize(9)
                                            .SemiBold();
                                    }
                                });

                                // Данные (уменьшенный шрифт)
                                foreach (var stat in statisticData.UserStatistics)
                                {
                                    table.Cell().Text(stat.Date.ToString("yyyy-MM-dd")).FontSize(8);
                                    table.Cell().Text(stat.Total.ToString()).FontSize(8);
                                    table.Cell().Text(stat.Ack.ToString()).FontSize(8);
                                    table.Cell().Text(stat.Win.ToString()).FontSize(8);
                                    table.Cell().Text(stat.ImpsCount.ToString()).FontSize(8);
                                    table.Cell().Text(stat.ClicksCount.ToString()).FontSize(8);
                                    table.Cell().Text(stat.StartsCount.ToString()).FontSize(8);
                                    table.Cell().Text(stat.CompletesCount.ToString()).FontSize(8);
                                    table.Cell().Text(stat.ShowRate.ToString("F2")).FontSize(8);
                                    table.Cell().Text(stat.Ctr.ToString("F2")).FontSize(8);
                                    table.Cell().Text(stat.Vtr.ToString("F2")).FontSize(8);

                                    string adType = "";
                                    if (stat.IsDsp) adType += "DSP; ";
                                    if (stat.IsDspInApp) adType += "In-App; ";
                                    if (stat.IsDspBanner) adType += "Banner; ";
                                    table.Cell().Text(adType.TrimEnd(';', ' ')).FontSize(8);
                                }
                            });


                        // Заголовок
                        column.Item()
                            .PaddingBottom(15)
                            .Text("Итог")
                            .SemiBold()
                            .FontSize(18)
                            .FontColor(Colors.Blue.Medium);

                        // Итоговые значения
                        column.Item()
                            .MinimalBox()
                            .Table(total =>
                            {
                                total.ColumnsDefinition(totalcolumns =>
                                {
                                    totalcolumns.RelativeColumn(1.5f); // Total
                                    totalcolumns.RelativeColumn(1.5f); // Ack
                                    totalcolumns.RelativeColumn(1.5f); // Win
                                    totalcolumns.RelativeColumn(1.5f); // ImpsCount
                                    totalcolumns.RelativeColumn(1.5f); // ClicksCount
                                    totalcolumns.RelativeColumn(1.5f); // StartsCount
                                    totalcolumns.RelativeColumn(1.5f); // CompletesCount
                                    totalcolumns.RelativeColumn(1.5f); // ShowRate
                                    totalcolumns.RelativeColumn(1.5f); // CTR
                                    totalcolumns.RelativeColumn(1.5f); // VTR
                                });

                                // Заголовки столбцов (уменьшенный шрифт)
                                total.Header(header =>
                                {
                                    foreach (var headerName in new[] { "Total", "Ack", "Win", "ImpsCount", "ClicksCount", "StartsCount", "CompletesCount", "ShowRate %", "CTR %", "VTR" })
                                    {
                                        header.Cell()
                                            .Background(Colors.Grey.Lighten3)
                                            .PaddingVertical(5)
                                            .Text(headerName)
                                            .FontSize(9)
                                            .SemiBold();
                                    }
                                });

                                    total.Cell().Text(statisticData.Total.Total.ToString()).FontSize(8);
                                    total.Cell().Text(statisticData.Total.Ack.ToString()).FontSize(8);
                                    total.Cell().Text(statisticData.Total.Win.ToString()).FontSize(8);
                                    total.Cell().Text(statisticData.Total.ImpsCount.ToString()).FontSize(8);
                                    total.Cell().Text(statisticData.Total.ClicksCount.ToString()).FontSize(8);
                                    total.Cell().Text(statisticData.Total.StartsCount.ToString()).FontSize(8);
                                    total.Cell().Text(statisticData.Total.CompletesCount.ToString()).FontSize(8);
                                    total.Cell().Text(statisticData.Total.ShowRate.ToString("F2")).FontSize(8);
                                    total.Cell().Text(statisticData.Total.Ctr.ToString("F2")).FontSize(8);
                                    total.Cell().Text(statisticData.Total.Vtr.ToString("F2")).FontSize(8);

                            });
                    });
                });
            })
            .GeneratePdf();

            // Возвращаем файл
            return new FileContentResult(pdfBytes, "application/pdf")
            {
                FileDownloadName = $"Статистика_{request.StartDate?.ToString("yyyy-MM-dd")}_{request.EndDate?.ToString("yyyy-MM-dd")}.pdf"
            };
        }


        public async Task<GetAllUsersResponse> GetAllUsers()
        {
            

            return new GetAllUsersResponse
            {
                UserResponse = await _dbContext.Users
                    .Select(u => new UserModel
                    {
                        Id = u.Id,
                        Username = u.Nickname,
                        CreatedAt = u.CreatedAt,
                        Login = u.Login, // Логин
                        Password = u.PasswordHash, 
                        IsDsp = u.IsDsp,
                        IsDspInApp = u.IsDspInApp,
                        IsDspBanner = u.IsDspBanner
                    })
                    .ToListAsync()


            };
        }

        public async Task<GetUserByIdResponse> GetUserById(int id)
        {
            // Проверка, что ID валидный
            if (id <= 0)
            {
                throw new ArgumentException("Неверный ID пользователя.");
            }

            // Получаем пользователя из базы данных по ID
            var user = await _dbContext.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            // Если пользователь не найден
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            // Создаем объект ответа
            var response = new GetUserByIdResponse
            {
                UserId = user.Id,
                username = user.Nickname,
                IsDsp = user.IsDsp,
                IsDspInApp = user.IsDspInApp,
                IsDspBanner = user.IsDspBanner,
                IsXInstallApp = user.IsXInstallApp
            };

            return response;
        }


        public async Task<GenerateUserResponse> GenerateUser()
        {
                // Получаем максимальный ID из базы данных
                var maxUserId = await _dbContext.Users.MaxAsync(u => (int?)u.Id) ?? 0;

                // Генерируем новый ID как максимальный + 1
                var userId = maxUserId + 1;

                // Генерация логина, пароля и имени
                var userLogin = "user" + userId;
                var userPassword = "GeneratedPassword" + userId;
                var userName = "User" + userId;

                // Создаем объект пользователя
                var user = new XInstallBotProfile.Models.User
                {
                    Id = userId,
                    Login = userLogin,
                    PasswordHash = HashPassword(userPassword), 
                    Nickname = userName
                };

                return new GenerateUserResponse
                {
                    Id = user.Id,
                    Login = user.Login,
                    Password = userPassword,
                    Nickname = user.Nickname
                };
            
        }


        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<GetStatisticResponse> GetStatistic(GetStatisticRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserId == 0)
            {
                throw new UnauthorizedAccessException("Необходима авторизация для выполнения запроса.");
            }

            if (currentUserRole != "Admin" && request.UserId != currentUserId)
            {
                // Если пользователь не администратор и пытается запросить чужую информацию
                throw new ForbiddenAccessException("У вас нет прав для запроса этой информации.");
            }

            var statisticsQuery = _dbContext.UserStatistics
                 .Where(us => us.UserId == request.UserId)
                 .Where(us => us.IsDsp == request.IsDsp)
                 .Where(us => us.IsDspInApp == request.IsDspInApp)
                 .Where(us => us.IsDspBanner == request.IsDspBanner);

            
            var totalShowRate = await _dbContext.UserStatistics
                .Where(us => us.UserId == request.UserId)
                .Where(us => us.IsDsp == request.IsDsp)
                .Where(us => us.IsDspInApp == request.IsDspInApp)
                .Where(us => us.IsDspBanner == request.IsDspBanner)
                .SumAsync(us => us.Total); // Сумма Total за всё время

            // Фильтрация по дате, если параметры заданы
            if (request.StartDate.HasValue)
            {
                statisticsQuery = statisticsQuery.Where(us => us.Date >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                statisticsQuery = statisticsQuery.Where(us => us.Date <= request.EndDate.Value);
            }

            var statistics = await statisticsQuery.ToListAsync();


            var statisticTotal = new StatisticTotal
            {
                Total = statistics.Sum(us => us.Total),
                Ack = statistics.Sum(us => us.Ack),
                Win = statistics.Sum(us => us.Win),
                ImpsCount = statistics.Sum(us => us.ImpsCount),
                ClicksCount = statistics.Sum(us => us.ClicksCount),
                ShowRate = statistics.Sum(us => us.ShowRate),
                Ctr = statistics.Sum(us => us.Ctr),
                Vtr = statistics.Sum(us => us.Vtr),
                StartsCount = statistics.Sum(us => us.StartsCount),
                CompletesCount = statistics.Sum(us => us.CompletesCount),
            };

            var statisticAverages = new StatisticAverages
            {
                Total = statistics.Any() ? statistics.Average(us => us.Total) : 0,
                Ack = statistics.Any() ? statistics.Average(us => us.Ack) : 0,
                Win = statistics.Any() ? statistics.Average(us => us.Win) : 0,
                Ctr = statistics.Any() ? statistics.Average(us => us.Ctr) : 0,
                Vtr = statistics.Any() ? statistics.Average(us => us.Vtr) : 0,
                ShowRate = statistics.Any() ? statistics.Average(us => us.ShowRate) : 0,
                ImpsCount = statistics.Any() ? statistics.Average(us => us.ImpsCount) : 0,
                ClicksCount = statistics.Any() ? statistics.Average(us => us.ClicksCount) : 0,
                StartsCount = statistics.Any() ? statistics.Average(us => us.StartsCount) : 0,
                CompletesCount = statistics.Any() ? statistics.Average(us => us.CompletesCount) : 0,
            };

            

            return new GetStatisticResponse
            {
                UserStatistics = statistics,
                Total = statisticTotal,
                TotalAllTime = totalShowRate,
                Averages = statisticAverages // Добавляем средние показатели в ответ
            };
        }

        public async Task<GetStatisticXInstallAppResponse> GetStatisticXInstallApp(GetStatisticXInstallAppRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserId == 0)
            {
                throw new UnauthorizedAccessException("Необходима авторизация для выполнения запроса.");
            }

            if (currentUserRole != "Admin" && request.UserId != currentUserId)
            {
                // Если пользователь не администратор и пытается запросить чужую информацию
                throw new ForbiddenAccessException("У вас нет прав для запроса этой информации.");
            }

            var statisticsQuery = _dbContext.XInstallAppUserStats
                 .Where(us => us.UserId == request.UserId);


            var totalShowRate = await _dbContext.XInstallAppUserStats
                .Where(us => us.UserId == request.UserId)
                .SumAsync(us => us.Total);

            // Фильтрация по дате, если параметры заданы
            if (request.StartDate.HasValue)
            {
                statisticsQuery = statisticsQuery.Where(us => us.Date >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                statisticsQuery = statisticsQuery.Where(us => us.Date <= request.EndDate.Value);
            }

            var statistics = await statisticsQuery.ToListAsync();


            var statisticTotal = new StatisticTotalXInstall
            {
                Total = statistics.Sum(us => us.Total),
                TotalIntasll = statistics.Sum(us => us.TotalInstall),
            };



            return new GetStatisticXInstallAppResponse
            {
                UserStatistics = statistics,
                Total = statisticTotal,
                TotalAllTime = totalShowRate
            };
        }

        private async Task<GetStatisticResponse> GetStatisticForExport(GetStatisticRequest request)
        {
            var statisticsQuery = _dbContext.UserStatistics
                 .Where(us => us.UserId == request.UserId)
                 .Where(us => us.IsDsp == request.IsDsp)
                 .Where(us => us.IsDspInApp == request.IsDspInApp)
                 .Where(us => us.IsDspBanner == request.IsDspBanner);


            var totalShowRate = await _dbContext.UserStatistics
                .Where(us => us.UserId == request.UserId)
                .Where(us => us.IsDsp == request.IsDsp)
                .Where(us => us.IsDspInApp == request.IsDspInApp)
                .Where(us => us.IsDspBanner == request.IsDspBanner)
                .SumAsync(us => us.Total); // Сумма Total за всё время

            // Фильтрация по дате, если параметры заданы
            if (request.StartDate.HasValue)
            {
                statisticsQuery = statisticsQuery.Where(us => us.Date >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                statisticsQuery = statisticsQuery.Where(us => us.Date <= request.EndDate.Value);
            }

            var statistics = await statisticsQuery.ToListAsync();


            var statisticTotal = new StatisticTotal
            {
                Total = statistics.Sum(us => us.Total),
                Ack = statistics.Sum(us => us.Ack),
                Win = statistics.Sum(us => us.Win),
                ImpsCount = statistics.Sum(us => us.ImpsCount),
                ClicksCount = statistics.Sum(us => us.ClicksCount),
                ShowRate = statistics.Sum(us => us.ShowRate),
                Ctr = statistics.Sum(us => us.Ctr),
                Vtr = statistics.Sum(us => us.Vtr),
                StartsCount = statistics.Sum(us => us.StartsCount),
                CompletesCount = statistics.Sum(us => us.CompletesCount),
            };

            var statisticAverages = new StatisticAverages
            {
                Total = statistics.Any() ? statistics.Average(us => us.Total) : 0,
                Ack = statistics.Any() ? statistics.Average(us => us.Ack) : 0,
                Win = statistics.Any() ? statistics.Average(us => us.Win) : 0,
                Ctr = statistics.Any() ? statistics.Average(us => us.Ctr) : 0,
                Vtr = statistics.Any() ? statistics.Average(us => us.Vtr) : 0,
                ShowRate = statistics.Any() ? statistics.Average(us => us.ShowRate) : 0,
                ImpsCount = statistics.Any() ? statistics.Average(us => us.ImpsCount) : 0,
                ClicksCount = statistics.Any() ? statistics.Average(us => us.ClicksCount) : 0,
                StartsCount = statistics.Any() ? statistics.Average(us => us.StartsCount) : 0,
                CompletesCount = statistics.Any() ? statistics.Average(us => us.CompletesCount) : 0,
            };



            return new GetStatisticResponse
            {
                UserStatistics = statistics,
                Total = statisticTotal,
                TotalAllTime = totalShowRate,
                Averages = statisticAverages // Добавляем средние показатели в ответ
            };
        }


        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            // Проверяем, занят ли уже ник
            var existingUser = await _dbContext.Users
                .Where(u => u.Nickname == request.Username)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                throw new Exception("Никнейм уже занят");
            }

            // Генерируем логин и пароль
            var login = request.Username;
            var password = "GeneratedPassword123";


            // Создаем пользователя
            var user = new XInstallBotProfile.Models.User
            {
                Nickname = request.Username,
                CreatedAt = DateTime.UtcNow,
                Login = login,
                Role = "User",
                IsDsp = true,
                IsDspInApp = false,
                IsDspBanner = false,
                IsXInstallApp = false
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Возвращаем ответ
            return new CreateUserResponse
            {
                Id = user.Id,
                Username = user.Nickname,
                CreatedAt = user.CreatedAt,
                Login = user.Login,
                IsDsp = user.IsDsp,
                IsDspInApp = user.IsDspInApp,
                IsDspBanner = user.IsDspBanner,
                IsXInstallApp = user.IsXInstallApp
            };
        }

        public async Task<bool> CreateUserRecord(int UserId, CreateUserRecordRequest request)
        {
            var recordUser = new UserStatistic
            {
                UserId = UserId,
                Date = request.Date.ToUniversalTime(), 
                Total = request.Total,
                Ack = request.Ack,
                Win = request.Win,
                ImpsCount = request.ImpsCount,
                ClicksCount = request.ClicksCount,
                StartsCount = request.StartsCount,
                CompletesCount = request.CompletesCount,
                IsDsp = request.IsDsp,
                IsDspInApp = request.IsDspInApp,
                IsDspBanner = request.IsDspBanner,
            };

            _dbContext.UserStatistics.Add(recordUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<UpdateUsernameResponse> UpdateUsername(int id, UpdateUsernameRequest request)
        {
            var user = await GetUserByIdAsync(id);

            // Проверяем, есть ли другой пользователь с таким ником
            bool nicknameTaken = await _dbContext.Users.AnyAsync(u => u.Nickname == request.Username && u.Id != id);
            if (nicknameTaken)
            {
                throw new Exception("Никнейм уже занят");
            }

            user.Nickname = request.Username;
            await _dbContext.SaveChangesAsync();

            return new UpdateUsernameResponse { Id = user.Id, Username = user.Nickname };
        }

        public async Task<UpdateFlagsResponse> UpdateUserFlags(int id, UpdateFlagsRequest request)
        {
            var user = await GetUserByIdAsync(id);

            user.IsDsp = request.IsDsp;
            user.IsDspInApp = request.IsDspInApp;
            user.IsDspBanner = request.IsDspBanner;
            user.IsXInstallApp = request.IsXInstallApp;

            await _dbContext.SaveChangesAsync();

            return new UpdateFlagsResponse { Id = user.Id, IsDsp = user.IsDsp, IsDspInApp = user.IsDspInApp, IsDspBanner = user.IsDspBanner, IsXInstallApp = user.IsXInstallApp };
        }

        public async Task<bool> UpdateStatistic(UpdateStatisticRequest request)
        {
            // Находим статистику по ID
            var statistic = await _dbContext.UserStatistics
                .FirstOrDefaultAsync(us => us.Id == request.Id);

            if (statistic == null)
            {
                // Возвращаем false, если статистика не найдена
                return false;
            }


            // В зависимости от ключа, обновляем нужное поле
            switch (request.Key.ToLower())
            {
                case "total":
                    statistic.Total = long.Parse(request.Value);
                    break;
                case "ack":
                    statistic.Ack = long.Parse(request.Value);
                    break;
                case "win":
                    statistic.Win = long.Parse(request.Value);
                    break;
                case "date":
                    var dateValue = DateTime.Parse(request.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    statistic.Date = dateValue.ToUniversalTime(); // Преобразуем в UTC
                    break;
                case "impscount":
                    statistic.ImpsCount = long.Parse(request.Value);
                    break;
                case "showrate":
                    statistic.ShowRate = decimal.Parse(request.Value, CultureInfo.InvariantCulture);
                    break;
                case "clickscount":
                    statistic.ClicksCount = long.Parse(request.Value);
                    break;
                case "ctr":
                    statistic.Ctr = decimal.Parse(request.Value, CultureInfo.InvariantCulture);
                    break;
                case "startscount":
                    statistic.StartsCount = long.Parse(request.Value);
                    break;
                case "completescount":
                    statistic.CompletesCount = long.Parse(request.Value);
                    break;
                case "vtr":
                    statistic.Vtr = decimal.Parse(request.Value, CultureInfo.InvariantCulture);
                    break;
                default:
                    return false; // Возвращаем false, если ключ некорректен
            }




            // Сохраняем изменения в базе данных
            await _dbContext.SaveChangesAsync();

            // Возвращаем true, если операция прошла успешно
            return true;
        }

        public async Task<bool> UpdateStatisticXInstallApp(UpdateStatisticXInstallAppRequest request)
        {
            var statistic = await _dbContext.XInstallAppUserStats
                .FirstOrDefaultAsync(us => us.Id == request.Id);

            if(statistic == null)
            {
                return false;
            }

            switch (request.Key.ToLower())
            {
                case "total":
                    statistic.Total = long.Parse(request.Value);
                    break;
                case "app_link":
                    statistic.AppLink = request.Value;
                    break;
                case "app_name":
                    statistic.AppName = request.Value;
                    break;
                case "date":
                    var dateValue = DateTime.Parse(request.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    statistic.Date = dateValue.ToUniversalTime(); // Преобразуем в UTC
                    break;
                case "region":
                    statistic.Region = request.Value;
                    break;
                case "keywords":
                    statistic.Keywords = JsonConvert.DeserializeObject<List<string>>(request.Value);
                    break;
                case "totalinstall":
                    statistic.TotalInstall = long.Parse(request.Value);
                    break;
                case "complited":
                    statistic.Complited = decimal.Parse(request.Value, CultureInfo.InvariantCulture);
                    break;
                default:
                    return false;
            }
            // Сохраняем изменения в базе данных
            await _dbContext.SaveChangesAsync();

            // Возвращаем true, если операция прошла успешно
            return true;

        }

        public async Task DeleteUser(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
                throw new ArgumentException("Список ID пользователей не может быть пустым.");

            // Получаем всех пользователей с указанными ID
            var users = await _dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            if (users.Count == 0)
                throw new Exception("Пользователи с указанными ID не найдены.");

            // Удаляем пользователей
            _dbContext.Users.RemoveRange(users);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserRecords(List<long> ids)
        {
            // Находим записи, которые есть в списке
            var recordsToDelete = await _dbContext.UserStatistics
                .Where(us => ids.Contains(us.Id))
                .ToListAsync();

            // Если нет записей для удаления, возвращаем false
            if (!recordsToDelete.Any())
            {
                return false;
            }

            // Удаляем найденные записи
            _dbContext.UserStatistics.RemoveRange(recordsToDelete);

            // Сохраняем изменения в базе данных
            await _dbContext.SaveChangesAsync();

            return true;
        }



        public async Task SaveUserChanges(SaveUserRequest request)
        {
            var user = await GetUserByIdAsync(request.Id);

            // Проверяем, есть ли изменения
            if (!HasUserChanged(user, request.UsernameRequest, request.FlagsRequest))
            {
                return; // Ничего не изменилось — сохранять не нужно
            }

            // Обновляем никнейм, если передан запрос на его изменение
            if (request.UsernameRequest != null)
            {
                bool nicknameTaken = await _dbContext.Users.AnyAsync(u => u.Nickname == request.UsernameRequest.Username && u.Id != request.Id);
                if (nicknameTaken)
                {
                    throw new Exception("Никнейм уже занят");
                }
                user.Nickname = request.UsernameRequest.Username;
            }

            // Обновляем флаги, если передан запрос на их изменение
            if (request.FlagsRequest != null)
            {
                user.IsDsp = request.FlagsRequest.IsDsp;
                user.IsDspInApp = request.FlagsRequest.IsDspInApp;
                user.IsDspBanner = request.FlagsRequest.IsDspBanner;
            }

            // Сохраняем только если были изменения
            await _dbContext.SaveChangesAsync();
        }

        public void SaveUserAsync(CreateUserRequest request)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);  // Для безопасности храните хэш пароля

            // Получаем максимальный userId из базы данных и инкрементируем его для нового пользователя
            var userId = _dbContext.Users.Max(u => (int?)u.Id) ?? 0 + 1;  // Если пользователей нет, начнем с 1

            var role = "User";  // Пример роли, можно сделать динамическим

            // Генерация JWT токена с userId и ролью
            var jwtToken = TokenGenerator.GenerateAccessToken(request.Login, userId, role);

            var user = new XInstallBotProfile.Models.User
            {
                Login = request.Login,
                PasswordHash = passwordHash,
                Nickname = request.Username,
                JwtToken = jwtToken,
                IsDsp = true
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        private async Task<XInstallBotProfile.Models.User> GetUserByIdAsync(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }
            return user;
        }

        private bool HasUserChanged(XInstallBotProfile.Models.User user, UpdateUsernameRequest usernameRequest, UpdateFlagsRequest flagsRequest)
        {
            if (usernameRequest != null && user.Nickname != usernameRequest.Username)
            {
                return true;
            }
            if (flagsRequest != null &&
                (user.IsDsp != flagsRequest.IsDsp ||
                 user.IsDspInApp != flagsRequest.IsDspInApp ||
                 user.IsDspBanner != flagsRequest.IsDspBanner ||
                 user.IsXInstallApp != flagsRequest.IsXInstallApp))
            {
                return true;
            }
            return false;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }

    }
}
