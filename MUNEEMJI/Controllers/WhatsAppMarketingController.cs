using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace MUNEEMJI.Controllers
{
    public class WhatsAppMarketingController : Controller
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;

        public WhatsAppMarketingController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser"; 
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
        public async Task<IActionResult> GetTemplates(string category = "Greetings", string filter = "All", string search = "")
        {
            var templates = await GetTemplatesAsync(category, filter, search);
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

                return File(imageBytes, "image/png", $"whatsapp-template-{DateTime.Now:yyyyMMddHHmmss}.png");
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
            // For demo purposes, create a simple image with text overlay
            // In production, you would load the actual template image and overlay text

            var width = 800;
            var height = 600;

            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);

            // Set high quality rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Create gradient background
            using var brush = new LinearGradientBrush(
                new Rectangle(0, 0, width, height),
                Color.FromArgb(74, 144, 226),
                Color.FromArgb(143, 88, 188),
                45f);

            graphics.FillRectangle(brush, 0, 0, width, height);

            // Add business name at top right
            if (!string.IsNullOrEmpty(request.BusinessName))
            {
                using var font = new Font("Arial", 16, FontStyle.Bold);
                using var textBrush = new SolidBrush(Color.White);
                var rect = new RectangleF(width - 200, 20, 180, 50);
                using var bgBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
                graphics.FillRoundedRectangle(bgBrush, Rectangle.Round(rect), 10);
                graphics.DrawString(request.BusinessName, font, textBrush, rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Add main motivational text in center
            var mainText = template.Title;
            if (!string.IsNullOrEmpty(mainText))
            {
                using var font = new Font("Arial", 24, FontStyle.Bold);
                using var textBrush = new SolidBrush(Color.White);
                var textRect = new RectangleF(50, height / 2 - 50, width - 100, 100);
                graphics.DrawString(mainText, font, textBrush, textRect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Add additional text
            if (!string.IsNullOrEmpty(request.AdditionalText))
            {
                using var font = new Font("Arial", 14, FontStyle.Regular);
                using var textBrush = new SolidBrush(Color.White);
                var rect = new RectangleF(20, height - 120, width - 40, 40);
                using var bgBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
                graphics.FillRoundedRectangle(bgBrush, Rectangle.Round(rect), 8);
                graphics.DrawString(request.AdditionalText, font, textBrush, rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Add contact info at bottom
            if (!string.IsNullOrEmpty(request.ContactPerson) || !string.IsNullOrEmpty(request.ContactNumber))
            {
                using var font = new Font("Arial", 12, FontStyle.Regular);
                using var textBrush = new SolidBrush(Color.White);
                var rect = new RectangleF(20, height - 70, width - 40, 40);
                using var bgBrush = new SolidBrush(Color.FromArgb(180, 76, 175, 80));
                graphics.FillRoundedRectangle(bgBrush, Rectangle.Round(rect), 8);

                var contactText = $"👤 {request.ContactPerson}     📞 {request.ContactNumber}";
                graphics.DrawString(contactText, font, textBrush, rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Add category tag
            using (var font = new Font("Arial", 10, FontStyle.Regular))
            using (var textBrush = new SolidBrush(Color.FromArgb(46, 125, 50)))
            {
                var rect = new RectangleF(20, height - 25, 100, 20);
                using var bgBrush = new SolidBrush(Color.FromArgb(232, 245, 233));
                graphics.FillRoundedRectangle(bgBrush, Rectangle.Round(rect), 10);
                graphics.DrawString("Motivation", font, textBrush, rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Convert to byte array
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }

        // Sample data for development/demo
        private List<TemplateViewModel> GetSampleTemplates()
        {
            return new List<TemplateViewModel>
            {
                new TemplateViewModel { Id = 1, Title = "हार मत मानो", ImageUrl = "/images/template1.jpg", Category = "Motivation", Type = "Greetings" },
                new TemplateViewModel { Id = 2, Title = "जिंदगी सिर्फ सोनात से बदलती है", ImageUrl = "/images/template2.jpg", Category = "Motivation", Type = "Greetings" },
                new TemplateViewModel { Id = 3, Title = "उड़ने का शौक रखो", ImageUrl = "/images/template3.jpg", Category = "Adventure", Type = "Greetings" },
                new TemplateViewModel { Id = 4, Title = "याद रखना कल कभी नहीं आता है", ImageUrl = "/images/template4.jpg", Category = "Memory", Type = "Greetings" },
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
