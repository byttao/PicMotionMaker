using System.Diagnostics;
using System.Text;

namespace PicMotionMaker
{
    public partial class Form1 : Form
    {
        // �����Ƶ�ֱ���
        private const string OutputResolution = "1080x1440";

        // ÿ��ͼƬ�̶�����ʾʱ�����룩
        private const int ImageDurationSeconds = 5;

        // FFmpeg.exe ������·��
        private readonly string _ffmpegPath;

        // ���������ļ�·��
        private string _backgroundMusicPath = "";

        // ��ק������ͼƬ�ļ�·���б� (������ק����)
        private List<string> _droppedImagePaths = new List<string>();

        public Form1()
        {
            InitializeComponent();
            // �����ڳ�������Ŀ¼�²��� ffmpeg.exe
            _ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            _backgroundMusicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bgm.mp3");

            // ��� bgm.mp3 �Ƿ����
            if (!File.Exists(_backgroundMusicPath))
            {
                LogMessage("�������� δ�ڳ���Ŀ¼���ҵ����뽫�临�Ƶ�����ͬһĿ¼�²�������Ϊbgm.mp3��", LogLevel.Error);
                btnStartConversion.Enabled = false;
            }
            else
            {
                LogMessage("bgm.mp3 ���ҵ���������������С�", LogLevel.Info);
            }
            // ��� ffmpeg.exe �Ƿ����
            if (!File.Exists(_ffmpegPath))
            {
                LogMessage("FFmpeg.exe δ�ڳ���Ŀ¼���ҵ����뽫�临�Ƶ�����ͬһĿ¼�¡�", LogLevel.Error);
                btnStartConversion.Enabled = false;
            }
            else
            {
                LogMessage("FFmpeg.exe ���ҵ���������������С�", LogLevel.Info);
            }
        }

        // ��־����� RichTextBox �ķ���
        private void LogMessage(string message, LogLevel level)
        {
            // �� UI �߳��ϸ��� RichTextBox
            if (richTextBoxLog.InvokeRequired)
            {
                richTextBoxLog.Invoke(() => AppendLog(message, level));
            }
            else
            {
                AppendLog(message, level);
            }
        }

        // ʵ���� RichTextBox ׷����־�ı��ķ���
        private void AppendLog(string message, LogLevel level)
        {
            string prefix = "";
            Color color = Color.Black; // Ĭ����ɫ

            switch (level)
            {
                case LogLevel.Info:
                    prefix = "[��Ϣ] ";
                    color = Color.Blue;
                    break;

                case LogLevel.Warning:
                    prefix = "[����] ";
                    color = Color.Orange;
                    break;

                case LogLevel.Error:
                    prefix = "[����] ";
                    color = Color.Red;
                    break;

                case LogLevel.Success:
                    prefix = "[�ɹ�] ";
                    color = Color.Green;
                    break;

                case LogLevel.Processing:
                    prefix = "[����] ";
                    color = Color.Purple;
                    break;
            }

            // ��ʽ�����������ʱ�������־�������Ϣ
            string formattedMessage = $"{DateTime.Now:HH:mm:ss} {prefix}{message}\r\n";

            // ��¼��ʼ�ͽ���λ�ã��Ա�������ɫ
            int startIndex = richTextBoxLog.Text.Length;
            richTextBoxLog.AppendText(formattedMessage);
            int endIndex = richTextBoxLog.Text.Length;

            // ������־�ı�����ɫ
            richTextBoxLog.Select(startIndex, endIndex - startIndex);
            richTextBoxLog.SelectionColor = color;
            richTextBoxLog.SelectionCharOffset = 0; // �ָ�Ĭ���ַ�ƫ��
            richTextBoxLog.DeselectAll(); // ȡ��ѡ�񣬱���Ӱ���������

            // �Զ��������ı���ĵײ���ʼ����ʾ������־
            richTextBoxLog.ScrollToCaret();
        }

        // ��ק����ʱ����
        private void PanelDropTarget_DragEnter(object sender, DragEventArgs e)
        {
            // �����ק�������Ƿ�����ļ�
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // �����Ʋ���
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // ��ק���ʱ����
        private void PanelDropTarget_DragDrop(object sender, DragEventArgs e)
        {
            // ��ȡ��ק���ļ�·��
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // ���˳�ͼƬ�ļ���������չ�����жϣ�
            var validImageFiles = files.Where(f => IsImageFile(f)).ToList();

            if (validImageFiles.Any())
            {
                // *** �޸ĵ㣺�Ƴ� .Clear() ***
                // droppedFilePaths.Clear(); // �Ƴ����У�ʵ��׷��Ч��

                // ���µ���ЧͼƬ�ļ���ӵ������б���
                foreach (var filePath in validImageFiles)
                {
                    // ����Ƿ��Ѵ��ڣ������ظ����
                    if (!_droppedImagePaths.Contains(filePath))
                    {
                        _droppedImagePaths.Add(filePath);
                        listBoxFiles.Items.Add(filePath); // ֻ����ļ���
                        if (_droppedImagePaths.Count == 1)
                        {
                            txtOutputFolder.Text = Path.GetDirectoryName(_droppedImagePaths[0]);
                        }
                    }
                }

                LogMessage($"�ɹ�׷�� {validImageFiles.Count} ��ͼƬ��", LogLevel.Info);
                // ��ӡ׷�ӵ��ļ�����������־ˢ��������ֻ��ӡ����ӵ�
                foreach (var filePath in validImageFiles)
                {
                    if (_droppedImagePaths.Contains(filePath) && _droppedImagePaths.Count(p => p == filePath) == 1) // ȷ���������ӵ�
                    {
                        LogMessage($"  + {Path.GetFileName(filePath)}", LogLevel.Info);
                    }
                }

                LogMessage($"��ǰ�б����� {_droppedImagePaths.Count} ��ͼƬ��", LogLevel.Info);
                if (_droppedImagePaths.Count>5)
                {
                    LogMessage($"��������ֻ��ȡǰ5��ͼƬ��", LogLevel.Warning);
                }
            }
            else
            {
                LogMessage("û���ҵ���Ч��ͼƬ�ļ���", LogLevel.Warning);
            }
        }

        // �ж��Ƿ���ͼƬ�ļ�
        private bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath)?.ToLower();
            return extension != null && (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif");
        }

        private async void btnStartConversion_Click(object sender, EventArgs e)
        {
            if (_droppedImagePaths.Count < 5)
            {
                LogMessage("��ѡ�� 5 ��ͼƬ��", LogLevel.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_backgroundMusicPath))
            {
                LogMessage("��ѡ�񱳾����֡�", LogLevel.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtOutputFolder.Text) || !Directory.Exists(txtOutputFolder.Text))
            {
                LogMessage("��ѡ��һ����Ч����Ƶ����ļ��С�", LogLevel.Warning);
                return;
            }

            // ��������������ļ�·��
            string outputFileName = $"�ϴ���Ƶ_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
            string outputPath = Path.Combine(txtOutputFolder.Text, outputFileName);

            LogMessage($"��ʼ�ϳ���Ƶ��: \"{outputPath}\"", LogLevel.Processing);
            btnStartConversion.Enabled = false;

            // �첽ִ����Ƶ�ϳ�����
            await ConvertImagesToVideoAsync(outputPath, _droppedImagePaths.Take(5).ToList());
        }

        // �첽ִ��ͼƬ�ϳ���Ƶ������
        private async Task ConvertImagesToVideoAsync(string outputPath, List<string> imagePaths)
        {
            try
            {
                StringBuilder filterComplexBuilder = new StringBuilder();
                int numImages = imagePaths.Count;
                int transitionDuration = 1; // ת������ʱ�䣨�룩
                int totalDuration = imagePaths.Count * ImageDurationSeconds;
                Random random = new Random();
                //string[] transitionTypes = { "fade", "wipeleft", "wiperight", "wipeup", "wipedown", "dissolve", "pixelize", "radial", "hblur", "blur", "crossfade" }; // �Ƴ�һЩ��̫�����Ļ������Ҫ�������õ�
                string fixedTransitionType = "wipeleft"; // ֻ����һ��ת��Ч�������� "fade"

                for (int i = 0; i < numImages; i++)
                {
                    filterComplexBuilder.Append($"[{i}:v]scale={OutputResolution},format=yuv420p,trim=duration={(i + 1) * ImageDurationSeconds}[v{i}];");
                }

                string prevStreamTag = "[v0]";
                for (int i = 0; i < numImages - 1; i++)
                {
                    string nextStreamTag = $"[v{i + 1}]";
                    // string randomTransition = transitionTypes[random.Next(transitionTypes.Length)]; // �Ƴ����ѡ��
                    string currentTransitionType = fixedTransitionType; // ʹ�ù̶���ת������

                    int offset = i * ImageDurationSeconds + (ImageDurationSeconds - transitionDuration);

                    string outputTag;
                    if (i == numImages - 2)
                    {
                        outputTag = "[vout]";
                    }
                    else
                    {
                        outputTag = $"[intermediate_{i}]";
                    }

                    // ʹ�ù̶���ת������
                    filterComplexBuilder.Append(
                        $"{prevStreamTag}{nextStreamTag}xfade=transition={currentTransitionType}:duration={transitionDuration}:offset={offset}[{outputTag.Trim('[', ']')}];");
                    prevStreamTag = outputTag;
                }

                string filterComplex = filterComplexBuilder.ToString();
                LogMessage($"������ FFmpeg filter_complex: {filterComplex}", LogLevel.Info);

                var arguments = new StringBuilder();
                arguments.Append(" -y "); // ��������ļ�

                // ���ͼƬ�������
                foreach (var imagePath in imagePaths)
                {
                    arguments.Append($"-loop 1 -i \"{imagePath}\" ");
                }

                // ��ӱ��������������
                arguments.Append($"-i \"{_backgroundMusicPath}\" ");

                arguments.Append($"-filter_complex \"{filterComplex}\" ");
                arguments.Append($"-map \"[vout]\" -map {imagePaths.Count}:a "); // imagePaths.Count ����Ƶ��������

                arguments.Append($"-t {totalDuration} ");
                arguments.Append("-c:v libx264 ");
                arguments.Append("-pix_fmt yuv420p ");
                arguments.Append("-r 30 ");
                arguments.Append("-c:a aac ");
                arguments.Append("-b:a 192k ");

                arguments.Append($"\"{outputPath}\"");

                string command = arguments.ToString();
                LogMessage($"ִ�� FFmpeg ����: {command}", LogLevel.Info);

                using (Process ffmpegProcess = new Process())
                {
                    ffmpegProcess.StartInfo.FileName = _ffmpegPath;
                    ffmpegProcess.StartInfo.Arguments = command;
                    ffmpegProcess.StartInfo.UseShellExecute = true;
                    ffmpegProcess.StartInfo.RedirectStandardOutput = false;
                    ffmpegProcess.StartInfo.RedirectStandardError = false;
                    ffmpegProcess.StartInfo.CreateNoWindow = false;
                    LogMessage("�������� FFmpeg...", LogLevel.Info);
                    try
                    {
                        ffmpegProcess.Start();
                        ffmpegProcess.WaitForExit();

                        if (ffmpegProcess.ExitCode == 0)
                        {
                            LogMessage("��Ƶ�ϳɳɹ���", LogLevel.Success);
                            _droppedImagePaths.Clear();
                            listBoxFiles.Items.Clear();
                        }
                        else
                        {
                            LogMessage($"��Ƶ�ϳ�ʧ�ܡ�FFmpeg �˳���: {ffmpegProcess.ExitCode}", LogLevel.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"���� FFmpeg ʱ�����쳣: {ex.Message}", LogLevel.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"������Ƶ�ϳ�ʱ����δ֪����: {ex.Message}", LogLevel.Error);
                LogMessage($"��ջ����: {ex.StackTrace}", LogLevel.Error);
            }
            finally
            {
                btnStartConversion.Enabled = true;
            }
        }

        private void btnFloder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                // ���öԻ���ĳ�ʼ·��
                if (!string.IsNullOrEmpty(txtOutputFolder.Text) && Directory.Exists(txtOutputFolder.Text))
                {
                    fbd.SelectedPath = txtOutputFolder.Text;
                }
                else
                {
                    // ����ʹ���ҵ��ĵ���ΪĬ��
                    fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtOutputFolder.Text = fbd.SelectedPath;
                }
            }
        }

        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {
            LogMessage($"��Ƶ����ļ���������Ϊ: \"{txtOutputFolder.Text}\"", LogLevel.Info);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _droppedImagePaths.Clear();
            listBoxFiles.Items.Clear();
        }

        // ��־�������
        private enum LogLevel
        {
            Info,
            Warning,
            Error,
            Success,
            Processing
        }
    }
}
