using System.Net.Http.Json;
using System.Text.Json;

namespace Demo
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("http://192.168.0.211:48280")
        };

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public Form1()
        {
            InitializeComponent();
            InitDataGrid();
            LoadImages();
        }

        private void InitDataGrid()
        {
            dgvData.AutoGenerateColumns = false;
            dgvData.CellFormatting += DgvData_CellFormatting;
            dgvData.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    Name = "colNumber",
                    HeaderText = "#",
                    DataPropertyName = "Number",
                    FillWeight = 8,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colMeasureName",
                    HeaderText = "Показатель",
                    DataPropertyName = "MeasureName",
                    FillWeight = 55
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colRawScore",
                    HeaderText = "Raw Score",
                    DataPropertyName = "RawScore",
                    FillWeight = 20,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F4" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colScalarScore",
                    HeaderText = "Scalar",
                    DataPropertyName = "ScalarScore",
                    FillWeight = 17,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                }
            );
        }

        private void LoadImages()
        {
            string imagesFolder = Path.Combine(AppContext.BaseDirectory, "images");
            if (!Directory.Exists(imagesFolder))
                return;

            string[] files = Directory.GetFiles(imagesFolder, "*.*")
                .Where(IsImageFile)
                .ToArray();

            lstImages.Items.Clear();
            foreach (string file in files)
                lstImages.Items.Add(Path.GetFileName(file));

            lstImages.Tag = imagesFolder;
        }

        private static bool IsImageFile(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".tiff" or ".tif" or ".webp";
        }

        private async void LstImages_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstImages.SelectedItem is not string fileName)
                return;

            string folder = lstImages.Tag as string ?? string.Empty;
            string filePath = Path.Combine(folder, fileName);

            dgvData.DataSource = null;
            lstImages.Enabled = false;

            using var waitForm = new WaitForm();
            waitForm.Show(this);

            try
            {
                await ProcessImageAsync(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"API error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                waitForm.Close();
                lstImages.Enabled = true;
            }
        }

        private async Task ProcessImageAsync(string filePath)
        {
            // Read bytes once — avoids file locking issues with Image.FromFile
            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);

            // Show raw image immediately while API processes
            SetPicture(BitmapFromBytes(imageBytes));

            using var content = new MultipartFormDataContent();
            using var byteContent = new ByteArrayContent(imageBytes);
            content.Add(byteContent, "file", Path.GetFileName(filePath));

            using var response = await _httpClient.PostAsync("/api/Quality/vector", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<VectorQualityResponse>(_jsonOptions);
            if (result is null)
                return;

            // Draw bounding box, ICAO crop margins, and ideal head size zone
            var annotated = BitmapFromBytes(imageBytes);
            DrawIdealHeadSizeBox(annotated, result.BoundingBox);
            DrawBoundingBox(annotated, result.BoundingBox);
            DrawCropMargins(annotated, result.BoundingBox);
            SetPicture(annotated);

            // Populate grid — Code excluded, MeasureName translated, row numbered
            dgvData.DataSource = result.Assessments
                .Select((a, i) => new AssessmentRow(i + 1, TranslateMeasure(a.MeasureName), a.RawScore, a.ScalarScore))
                .ToList();
        }

        /// <summary>Creates an independent Bitmap from raw bytes so the source stream can be safely disposed.</summary>
        private static Bitmap BitmapFromBytes(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var img = Image.FromStream(ms);
            return new Bitmap(img);
        }

        private static void DrawBoundingBox(Bitmap bitmap, BoundingBoxDto bbox)
        {
            using var g = Graphics.FromImage(bitmap);
            using var pen = new Pen(Color.Red, 3);
            g.DrawRectangle(pen, bbox.X, bbox.Y, bbox.Width, bbox.Height);
        }

        /// <summary>
        /// Draws the ICAO-compliant head size target zone (50–69% of image height)
        /// as two green rectangles centered on the detected face.
        /// The zone between them is the acceptable range per ISO/IEC 29794-5 §7.4.9.
        /// </summary>
        private static void DrawIdealHeadSizeBox(Bitmap bitmap, BoundingBoxDto bbox)
        {
            int imgW = bitmap.Width;
            int imgH = bitmap.Height;

            float centerX = bbox.X + bbox.Width / 2f;
            float centerY = bbox.Y + bbox.Height / 2f;
            float aspectRatio = (float)bbox.Width / bbox.Height;

            // ICAO: face height = 50–69% of image height
            float minH = imgH * 0.50f;
            float maxH = imgH * 0.69f;
            float minW = minH * aspectRatio;
            float maxW = maxH * aspectRatio;

            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Semi-transparent fill between the two zones
            using var zoneBrush = new SolidBrush(Color.FromArgb(30, 0, 200, 0));
            RectangleF outerRect = new(centerX - maxW / 2f, centerY - maxH / 2f, maxW, maxH);
            g.FillRectangle(zoneBrush, outerRect);

            // Max box (69%) — outer boundary
            using var maxPen = new Pen(Color.LimeGreen, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot };
            g.DrawRectangle(maxPen, outerRect.X, outerRect.Y, outerRect.Width, outerRect.Height);

            // Min box (50%) — inner boundary
            using var minPen = new Pen(Color.LimeGreen, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            RectangleF innerRect = new(centerX - minW / 2f, centerY - minH / 2f, minW, minH);
            g.DrawRectangle(minPen, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);

            // Labels
            using var font = new Font("Segoe UI", Math.Max(imgH / 80f, 9f), FontStyle.Bold);
            using var brush = new SolidBrush(Color.LimeGreen);
            using var bgBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));

            DrawLabel(g, font, brush, bgBrush, "min 50%", new PointF(innerRect.Right + 4, innerRect.Y));
            DrawLabel(g, font, brush, bgBrush, "max 69%", new PointF(outerRect.Right + 4, outerRect.Y));
        }

        /// <summary>
        /// Draws ICAO crop margin guides and head size ratio based on the bounding box.
        /// ISO/IEC 29794-5 §7.4.9 Head Size: face height = 50–69% of image height.
        /// ISO/IEC 29794-5 §7.4.10 Crop: four margins (left, right, above, below).
        /// </summary>
        private static void DrawCropMargins(Bitmap bitmap, BoundingBoxDto bbox)
        {
            int imgW = bitmap.Width;
            int imgH = bitmap.Height;

            double marginLeft   = (double)bbox.X / imgW * 100;
            double marginRight  = (double)(imgW - bbox.X - bbox.Width) / imgW * 100;
            double marginAbove  = (double)bbox.Y / imgH * 100;
            double marginBelow  = (double)(imgH - bbox.Y - bbox.Height) / imgH * 100;
            double headSizePct  = (double)bbox.Height / imgH * 100;

            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var dashPen = new Pen(Color.Cyan, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            using var font = new Font("Segoe UI", Math.Max(imgH / 60f, 10f), FontStyle.Bold);
            using var brush = new SolidBrush(Color.Cyan);
            using var bgBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));

            // Left margin line
            g.DrawLine(dashPen, bbox.X, 0, bbox.X, imgH);

            // Right margin line
            int rightX = bbox.X + bbox.Width;
            g.DrawLine(dashPen, rightX, 0, rightX, imgH);

            // Top margin line
            g.DrawLine(dashPen, 0, bbox.Y, imgW, bbox.Y);

            // Bottom margin line
            int bottomY = bbox.Y + bbox.Height;
            g.DrawLine(dashPen, 0, bottomY, imgW, bottomY);

            // Labels
            DrawLabel(g, font, brush, bgBrush, $"← {marginLeft:F1}%", new PointF(4, bbox.Y + bbox.Height / 2f));
            DrawLabel(g, font, brush, bgBrush, $"{marginRight:F1}% →", new PointF(rightX + 4, bbox.Y + bbox.Height / 2f));
            DrawLabel(g, font, brush, bgBrush, $"↑ {marginAbove:F1}%", new PointF(bbox.X + bbox.Width / 2f - 30, 4));
            DrawLabel(g, font, brush, bgBrush, $"↓ {marginBelow:F1}%", new PointF(bbox.X + bbox.Width / 2f - 30, bottomY + 4));

            // Head size indicator (right edge)
            bool headSizeOk = headSizePct is >= 50 and <= 69;
            Color hsColor = headSizeOk ? Color.LimeGreen : Color.OrangeRed;

            using var hsPen = new Pen(hsColor, 2);
            int hsX = rightX + 8;
            g.DrawLine(hsPen, hsX, bbox.Y, hsX, bottomY);
            g.DrawLine(hsPen, hsX - 4, bbox.Y, hsX + 4, bbox.Y);
            g.DrawLine(hsPen, hsX - 4, bottomY, hsX + 4, bottomY);

            using var hsBrush = new SolidBrush(hsColor);
            string hsText = $"Head {headSizePct:F0}%";
            DrawLabel(g, font, hsBrush, bgBrush, hsText, new PointF(hsX + 6, bbox.Y + bbox.Height / 2f - 8));
        }

        /// <summary>Draws text with a semi-transparent background for readability.</summary>
        private static void DrawLabel(Graphics g, Font font, Brush textBrush, Brush bgBrush, string text, PointF pt)
        {
            SizeF size = g.MeasureString(text, font);
            g.FillRectangle(bgBrush, pt.X - 2, pt.Y - 1, size.Width + 4, size.Height + 2);
            g.DrawString(text, font, textBrush, pt);
        }

        private void SetPicture(Image newImage)
        {
            var old = picMain.Image;
            picMain.Image = newImage;
            old?.Dispose();
        }

        private void DgvData_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvData.Rows[e.RowIndex].DataBoundItem is not AssessmentRow row)
                return;

            e.CellStyle.BackColor = row.ScalarScore switch
            {
                < 50  => Color.LightPink,
                < 80  => Color.LightYellow,
                _     => dgvData.DefaultCellStyle.BackColor
            };
        }

        private static string TranslateMeasure(string name) =>
            _measureNames.TryGetValue(name, out string? ru) ? ru : name;

        private static readonly IReadOnlyDictionary<string, string> _measureNames =
            new Dictionary<string, string>
            {
                ["UnifiedQualityScore"]        = "Общий балл качества",
                ["BackgroundUniformity"]       = "Однородность фона",
                ["IlluminationUniformity"]     = "Равномерность освещения",
                ["LuminanceMean"]              = "Средняя яркость",
                ["LuminanceVariance"]          = "Дисперсия яркости",
                ["UnderExposurePrevention"]    = "Защита от недоэкспозиции",
                ["OverExposurePrevention"]     = "Защита от переэкспозиции",
                ["DynamicRange"]               = "Динамический диапазон",
                ["Sharpness"]                  = "Резкость",
                ["CompressionArtifacts"]       = "Артефакты сжатия",
                ["NaturalColour"]              = "Естественный цвет",
                ["SingleFacePresent"]          = "Одно лицо в кадре",
                ["EyesOpen"]                   = "Открытые глаза",
                ["MouthClosed"]                = "Закрытый рот",
                ["EyesVisible"]                = "Видимость глаз",
                ["MouthOcclusionPrevention"]   = "Рот не перекрыт",
                ["FaceOcclusionPrevention"]    = "Лицо не перекрыто",
                ["InterEyeDistance"]           = "Расстояние между глазами",
                ["HeadSize"]                   = "Размер головы",
                ["LeftwardCrop"]               = "Обрезка слева",
                ["RightwardCrop"]              = "Обрезка справа",
                ["MarginAbove"]                = "Поле сверху",
                ["MarginBelow"]                = "Поле снизу",
                ["HeadPoseYaw"]                = "Поворот головы (рыскание)",
                ["HeadPosePitch"]              = "Наклон головы (тангаж)",
                ["HeadPoseRoll"]               = "Крен головы",
                ["ExpressionNeutrality"]       = "Нейтральное выражение",
                ["NoHeadCoverings"]            = "Без головных уборов",
                ["Luminance"]                  = "Яркость",
                ["CropOfTheFaceImage"]         = "Кадрирование лица",
                ["HeadPose"]                   = "Поза головы",
            };
    }

    // API response models (mirrors OFIQ.RestApi.Models)
    record BoundingBoxDto(short X, short Y, short Width, short Height);
    record AssessmentResultDto(string MeasureName, double RawScore, double ScalarScore, int Code);
    record VectorQualityResponse(BoundingBoxDto BoundingBox, List<AssessmentResultDto> Assessments);

    // Grid row (Code excluded)
    record AssessmentRow(int Number, string MeasureName, double RawScore, double ScalarScore);
}
