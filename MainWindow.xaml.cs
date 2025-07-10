using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Diagnostics;

namespace LLMTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process apiProcess = null;
        private const string ApiUrl = "http://127.0.0.1:5000/translate";

        private readonly HttpClient _httpClient = new HttpClient();
        private string configPath = "config.json";

        private string sourceLang = "fa";
        private string targetLang = "ar";

        public MainWindow()
        {
            InitializeComponent();
            StartApiIfNeeded();
            LoadLangSettings();
            UpdateLangUI();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            try
            {
                if (apiProcess != null && !apiProcess.HasExited)
                {
                    apiProcess.Kill();
                }
            }
            catch { }
        }

        private void UpdateTextAlignments()
        {
            InputTextBox.TextAlignment = (sourceLang == "fa" || sourceLang == "ar") ? TextAlignment.Right : TextAlignment.Left;
            OutputTextBlock.TextAlignment = (targetLang == "fa" || targetLang == "ar") ? TextAlignment.Right : TextAlignment.Left;
        }

        private void LoadLangSettings()
        {
            if (File.Exists(configPath))
            {
                try
                {
                    var configJson = File.ReadAllText(configPath);
                    var json = JsonSerializer.Deserialize<JsonElement>(configJson);
                    sourceLang = json.GetProperty("source_lang").GetString();
                    targetLang = json.GetProperty("target_lang").GetString();
                }
                catch { }
            }
        }

        private void SaveLangSettings()
        {
            var obj = new { source_lang = sourceLang, target_lang = targetLang };
            File.WriteAllText(configPath, JsonSerializer.Serialize(obj));
        }

        private async void Translate_Click(object sender, RoutedEventArgs e)
        {
            var input = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            var requestData = new
            {
                source = input,
                source_lang = sourceLang,
                target_lang = targetLang
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:5000/translate", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);
                var translated = responseJson.GetProperty("translated_text").GetString();

                OutputTextBlock.Text = translated;
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = "Error: " + ex.Message;
            }
        }

        private void Reverse_Click(object sender, RoutedEventArgs e)
        {
            (sourceLang, targetLang) = (targetLang, sourceLang);
            SaveLangSettings();
            UpdateLangUI();
            InputTextBox.Text = OutputTextBlock.Text;
            Translate_Click(null, null);
        }

        private void SourceLang_Changed(object sender, RoutedEventArgs e)
        {
            sourceLang = SourceLangBox.Text;
            SaveLangSettings();
            UpdateTextAlignments();
        }

        private void TargetLang_Changed(object sender, RoutedEventArgs e)
        {
            targetLang = TargetLangBox.Text;
            SaveLangSettings();
            UpdateTextAlignments();
        }

        private void UpdateLangUI()
        {
            SourceLangBox.Text = sourceLang;
            TargetLangBox.Text = targetLang;

            UpdateTextAlignments();
        }

        private void CopyInput_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(InputTextBox.Text);
        }

        private void CopyOutput_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OutputTextBlock.Text);
        }

        private void StartApiIfNeeded()
        {
            try
            {
                using var client = new HttpClient();
                var test = new { source = "سلام", source_lang = "fa", target_lang = "en" };
                var content = new StringContent(JsonSerializer.Serialize(test), Encoding.UTF8, "application/json");
                var response = client.PostAsync(ApiUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // API already running
                    return;
                }
            }
            catch
            {
                // API is not running, so start it
            }

            // Run API hidden
            var psi = new ProcessStartInfo
            {
                FileName = "python\\pythonw",
                Arguments = "translate_api.py",
                WorkingDirectory = @"backend", // مسیر پوشه کد Flask
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                apiProcess = Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start API: " + ex.Message);
            }
        }

    }
}