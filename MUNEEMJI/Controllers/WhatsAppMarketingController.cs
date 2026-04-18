using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using SkiaSharp;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class WhatsAppMarketingController : Controller
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;

        public WhatsAppMarketingController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionString = MUNEEMJI.DbConfig.ConnectionString; 
            _environment = environment;
        }

        public async Task<IActionResult> Index(string category = "Greetings", string filter = "All", string search = "")
        {
            var templates = await GetTemplatesAsync(category, filter, search);

            var viewModel = new TemplateListViewModel
            {
                Templates = templates,
                SelectedCategory = category,
                SearchTerm = search
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates(int tabid = 0 , int categoryid =  0)
        {
            var templates = await GetTemplatesAsyncForFilter(tabid, categoryid);
            return Json(templates);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequest request)
        {
            try
            {
                // Get template details
                var template = await GetTemplateByIdAsync(request.TemplateId);
                if (template == null)
                {
                    return NotFound("Template not found");
                }

                // Save customization
                await SaveCustomizationAsync(request);

                // Generate image with overlay text
                var imageBytes = await GenerateCustomImageAsync(template, request);

                // Update download count
                await UpdateDownloadCountAsync(request.TemplateId);

                return Ok();
                //return File(imageBytes, "image/png", $"whatsapp-template-{DateTime.Now:yyyyMMddHHmmss}.png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating image: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomization([FromBody] GenerateImageRequest request)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO template_customizations 
                    (template_id, business_name, contact_person, contact_number, additional_text, whatsapp_text, created_date)
                    VALUES (@templateId, @businessName, @contactPerson, @contactNumber, @additionalText, @whatsappText, @createdDate)";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@templateId", request.TemplateId);
                command.Parameters.AddWithValue("@businessName", request.BusinessName ?? "");
                command.Parameters.AddWithValue("@contactPerson", request.ContactPerson ?? "");
                command.Parameters.AddWithValue("@contactNumber", request.ContactNumber ?? "");
                command.Parameters.AddWithValue("@additionalText", request.AdditionalText ?? "");
                command.Parameters.AddWithValue("@whatsappText", request.WhatsappText ?? "");
                command.Parameters.AddWithValue("@createdDate", DateTime.Now);

                await command.ExecuteNonQueryAsync();
                return Ok("Customization saved successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving customization: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> IncrementViewCount(int templateId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "UPDATE templates SET view_count = view_count + 1 WHERE id = @templateId";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@templateId", templateId);

                await command.ExecuteNonQueryAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating view count: {ex.Message}");
            }
        }

        private async Task<List<TemplateViewModel>> GetTemplatesAsync(string category, string filter, string search)
        {
            var templates = new List<TemplateViewModel>();

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var queryBuilder = new StringBuilder(@"
                    SELECT id, title, image_url, category, type, is_active, created_date, view_count, download_count
                    FROM templates 
                    WHERE is_active = true");

                var parameters = new List<NpgsqlParameter>();

                if (category != "All" && !string.IsNullOrEmpty(category))
                {
                    queryBuilder.Append(" AND type = @category");
                    parameters.Add(new NpgsqlParameter("@category", category));
                }

                if (filter != "All" && !string.IsNullOrEmpty(filter))
                {
                    queryBuilder.Append(" AND category = @filter");
                    parameters.Add(new NpgsqlParameter("@filter", filter));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    queryBuilder.Append(" AND (title ILIKE @search OR category ILIKE @search)");
                    parameters.Add(new NpgsqlParameter("@search", $"%{search}%"));
                }

                queryBuilder.Append(" ORDER BY created_date DESC, view_count DESC");

                using var command = new NpgsqlCommand(queryBuilder.ToString(), connection);
                command.Parameters.AddRange(parameters.ToArray());

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    templates.Add(new TemplateViewModel
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        ImageUrl = reader.GetString("image_url"),
                        Category = reader.GetString("category"),
                        Type = reader.GetString("type"),
                        IsActive = reader.GetBoolean("is_active"),
                        CreatedDate = reader.GetDateTime("created_date")
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error and return sample data for development
                Console.WriteLine($"Database error: {ex.Message}");
                return GetSampleTemplates();
            }

            return templates.Any() ? templates : GetSampleTemplates();
        }
        private async Task<List<TemplateViewModel>> GetTemplatesAsyncForFilter(int tabid = 0, int categoryid = 0)
        {
            var templates = new List<TemplateViewModel>();

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var queryBuilder = new StringBuilder(@"
                    SELECT id, title, image_url, category, type, is_active, created_date, view_count, download_count
                    FROM templates 
                    WHERE is_active = true");

                var parameters = new List<NpgsqlParameter>();

                if (tabid>0)
                {
                    queryBuilder.Append(" AND tabid = @tabid");
                    parameters.Add(new NpgsqlParameter("@tabid", tabid));
                }

                if (categoryid>0)
                {
                    queryBuilder.Append(" AND categoryid = @categoryid");
                    parameters.Add(new NpgsqlParameter("@categoryid", categoryid));
                }

               

                queryBuilder.Append(" ORDER BY created_date DESC, view_count DESC");

                using var command = new NpgsqlCommand(queryBuilder.ToString(), connection);
                command.Parameters.AddRange(parameters.ToArray());

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    templates.Add(new TemplateViewModel
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        ImageUrl = reader.GetString("image_url"),
                        Category = reader.GetString("category"),
                        Type = reader.GetString("type"),
                        IsActive = reader.GetBoolean("is_active"),
                        CreatedDate = reader.GetDateTime("created_date")
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error and return sample data for development
                Console.WriteLine($"Database error: {ex.Message}");
                return GetSampleTemplates();
            }

            return templates.Any() ? templates : GetSampleTemplates();
        }

        private async Task<Template?> GetTemplateByIdAsync(int templateId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT id, title, image_url, category, type, is_active, created_date, description
                    FROM templates 
                    WHERE id = @templateId AND is_active = true";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@templateId", templateId);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Template
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        ImageUrl = reader.GetString("image_url"),
                        Category = reader.GetString("category"),
                        Type = reader.GetString("type"),
                        IsActive = reader.GetBoolean("is_active"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        Description = reader.IsDBNull("description") ? null : reader.GetString("description")
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting template: {ex.Message}");
            }

            return null;
        }

        private async Task SaveCustomizationAsync(GenerateImageRequest request)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO template_customizations 
                    (template_id, business_name, contact_person, contact_number, additional_text, whatsapp_text, created_date, is_downloaded, download_date)
                    VALUES (@templateId, @businessName, @contactPerson, @contactNumber, @additionalText, @whatsappText, @createdDate, @isDownloaded, @downloadDate)";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@templateId", request.TemplateId);
                command.Parameters.AddWithValue("@businessName", request.BusinessName ?? "");
                command.Parameters.AddWithValue("@contactPerson", request.ContactPerson ?? "");
                command.Parameters.AddWithValue("@contactNumber", request.ContactNumber ?? "");
                command.Parameters.AddWithValue("@additionalText", request.AdditionalText ?? "");
                command.Parameters.AddWithValue("@whatsappText", request.WhatsappText ?? "");
                command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                command.Parameters.AddWithValue("@isDownloaded", true);
                command.Parameters.AddWithValue("@downloadDate", DateTime.Now);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving customization: {ex.Message}");
            }
        }

        private async Task UpdateDownloadCountAsync(int templateId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "UPDATE templates SET download_count = download_count + 1 WHERE id = @templateId";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@templateId", templateId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating download count: {ex.Message}");
            }
        }

        private async Task<byte[]> GenerateCustomImageAsync(Template template, GenerateImageRequest request)
        {
            var width = 800;
            var height = 600;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            // Clear background with gradient colors (simplified)
            canvas.Clear(SKColor.Parse("#4A90E2"));

            // Create a simple gradient effect by drawing rectangles
            for (int i = 0; i < height; i++)
            {
                float ratio = (float)i / height;
                var r = (byte)(74 + (143 - 74) * ratio);
                var g = (byte)(144 + (88 - 144) * ratio);
                var b = (byte)(226 + (188 - 226) * ratio);

                using var paint = new SKPaint { Color = new SKColor(r, g, b) };
                canvas.DrawRect(0, i, width, 1, paint);
            }

            // Add business name at top right
            if (!string.IsNullOrEmpty(request.BusinessName))
            {
                using var bgPaint = new SKPaint { Color = new SKColor(0, 0, 0, 180) };
                var rect = new SKRect(width - 200, 20, width - 20, 70);
                canvas.DrawRoundRect(rect, 10, 10, bgPaint);

                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 16,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center
                };
                canvas.DrawText(request.BusinessName, width - 110, 50, textPaint);
            }

            // Add main motivational text in center
            if (!string.IsNullOrEmpty(template.Title))
            {
                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 24,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center,
                    FakeBoldText = true
                };
                canvas.DrawText(template.Title, width / 2, height / 2, textPaint);
            }

            // Add additional text
            if (!string.IsNullOrEmpty(request.AdditionalText))
            {
                using var bgPaint = new SKPaint { Color = new SKColor(0, 0, 0, 180) };
                var rect = new SKRect(20, height - 120, width - 20, height - 80);
                canvas.DrawRoundRect(rect, 8, 8, bgPaint);

                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 14,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center
                };
                canvas.DrawText(request.AdditionalText, width / 2, height - 95, textPaint);
            }

            // Add contact info
            if (!string.IsNullOrEmpty(request.ContactPerson) || !string.IsNullOrEmpty(request.ContactNumber))
            {
                using var bgPaint = new SKPaint { Color = new SKColor(76, 175, 80, 180) };
                var rect = new SKRect(20, height - 70, width - 20, height - 30);
                canvas.DrawRoundRect(rect, 8, 8, bgPaint);

                var contactText = $"Contact: {request.ContactPerson} | {request.ContactNumber}";
                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 12,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center
                };
                canvas.DrawText(contactText, width / 2, height - 45, textPaint);
            }

            // Add category tag
            using (var bgPaint = new SKPaint { Color = new SKColor(232, 245, 233) })
            {
                var rect = new SKRect(20, height - 25, 120, height - 5);
                canvas.DrawRoundRect(rect, 10, 10, bgPaint);
            }

            using (var textPaint = new SKPaint
            {
                Color = new SKColor(46, 125, 50),
                TextSize = 10,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            })
            {
                canvas.DrawText("Motivation", 70, height - 12, textPaint);
            }

            // Convert to byte array
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        // Sample data for development/demo
        private List<TemplateViewModel> GetSampleTemplates()
        {
            return new List<TemplateViewModel>
            {
                new TemplateViewModel { Id = 1, Title = "??? ?? ????", ImageUrl = "/images/template1.jpg", Category = "Motivation", Type = "Greetings" },
                new TemplateViewModel { Id = 2, Title = "?????? ????? ????? ?? ????? ??", ImageUrl = "/images/template2.jpg", Category = "Motivation", Type = "Greetings" },
                new TemplateViewModel { Id = 3, Title = "????? ?? ??? ???", ImageUrl = "/images/template3.jpg", Category = "Adventure", Type = "Greetings" },
                new TemplateViewModel { Id = 4, Title = "??? ???? ?? ??? ???? ??? ??", ImageUrl = "/images/template4.jpg", Category = "Memory", Type = "Greetings" },
                new TemplateViewModel { Id = 5, Title = "Good Night", ImageUrl = "/images/template5.jpg", Category = "Good Night", Type = "Greetings" },
                new TemplateViewModel { Id = 6, Title = "ONE DAY, WE WILL NEVER HAVE TO SAY GOODBYE", ImageUrl = "/images/template6.jpg", Category = "Good Night", Type = "Greetings" },
                new TemplateViewModel { Id = 7, Title = "Hard work beats TALENT", ImageUrl = "/images/template7.jpg", Category = "Motivation", Type = "Business" },
                new TemplateViewModel { Id = 8, Title = "YOU DON'T HAVE TO BE GREAT TO START", ImageUrl = "/images/template8.jpg", Category = "Motivation", Type = "Business" },
                new TemplateViewModel { Id = 9, Title = "Good Morning", ImageUrl = "/images/template9.jpg", Category = "Good Morning", Type = "Greetings" },
                new TemplateViewModel { Id = 10, Title = "Good Morning", ImageUrl = "/images/template10.jpg", Category = "Good Morning", Type = "Greetings" }
            };
        }
    }

    // Extension method for rounded rectangles
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle, int radius)
        {
            using var path = new GraphicsPath();
            path.AddArc(rectangle.X, rectangle.Y, radius, radius, 180, 90);
            path.AddArc(rectangle.X + rectangle.Width - radius, rectangle.Y, radius, radius, 270, 90);
            path.AddArc(rectangle.X + rectangle.Width - radius, rectangle.Y + rectangle.Height - radius, radius, radius, 0, 90);
            path.AddArc(rectangle.X, rectangle.Y + rectangle.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            graphics.FillPath(brush, path);
        }
    }
}
