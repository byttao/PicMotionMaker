using System.Diagnostics;
using System.Text;

namespace PicMotionMaker
{
    public partial class Form1 : Form
    {
        // 输出视频分辨率
        private const string OutputResolution = "1080x1440";

        // 每张图片固定的显示时长（秒）
        private const int ImageDurationSeconds = 5;

        // FFmpeg.exe 的完整路径
        private readonly string _ffmpegPath;

        // 背景音乐文件路径
        private string _backgroundMusicPath = "";

        // 拖拽进来的图片文件路径列表 (用于拖拽功能)
        private List<string> _droppedImagePaths = new List<string>();

        public Form1()
        {
            InitializeComponent();
            // 尝试在程序运行目录下查找 ffmpeg.exe
            _ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            _backgroundMusicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bgm.mp3");

            // 检查 bgm.mp3 是否存在
            if (!File.Exists(_backgroundMusicPath))
            {
                LogMessage("背景音乐 未在程序目录下找到。请将其复制到程序同一目录下并重命名为bgm.mp3。", LogLevel.Error);
                btnStartConversion.Enabled = false;
            }
            else
            {
                LogMessage("bgm.mp3 已找到，程序可正常运行。", LogLevel.Info);
            }
            // 检查 ffmpeg.exe 是否存在
            if (!File.Exists(_ffmpegPath))
            {
                LogMessage("FFmpeg.exe 未在程序目录下找到。请将其复制到程序同一目录下。", LogLevel.Error);
                btnStartConversion.Enabled = false;
            }
            else
            {
                LogMessage("FFmpeg.exe 已找到，程序可正常运行。", LogLevel.Info);
            }
        }

        // 日志输出到 RichTextBox 的方法
        private void LogMessage(string message, LogLevel level)
        {
            // 在 UI 线程上更新 RichTextBox
            if (richTextBoxLog.InvokeRequired)
            {
                richTextBoxLog.Invoke(() => AppendLog(message, level));
            }
            else
            {
                AppendLog(message, level);
            }
        }

        // 实际向 RichTextBox 追加日志文本的方法
        private void AppendLog(string message, LogLevel level)
        {
            string prefix = "";
            Color color = Color.Black; // 默认颜色

            switch (level)
            {
                case LogLevel.Info:
                    prefix = "[信息] ";
                    color = Color.Blue;
                    break;

                case LogLevel.Warning:
                    prefix = "[警告] ";
                    color = Color.Orange;
                    break;

                case LogLevel.Error:
                    prefix = "[错误] ";
                    color = Color.Red;
                    break;

                case LogLevel.Success:
                    prefix = "[成功] ";
                    color = Color.Green;
                    break;

                case LogLevel.Processing:
                    prefix = "[处理] ";
                    color = Color.Purple;
                    break;
            }

            // 格式化输出，包含时间戳、日志级别和消息
            string formattedMessage = $"{DateTime.Now:HH:mm:ss} {prefix}{message}\r\n";

            // 记录开始和结束位置，以便设置颜色
            int startIndex = richTextBoxLog.Text.Length;
            richTextBoxLog.AppendText(formattedMessage);
            int endIndex = richTextBoxLog.Text.Length;

            // 设置日志文本的颜色
            richTextBoxLog.Select(startIndex, endIndex - startIndex);
            richTextBoxLog.SelectionColor = color;
            richTextBoxLog.SelectionCharOffset = 0; // 恢复默认字符偏移
            richTextBoxLog.DeselectAll(); // 取消选择，避免影响后续输入

            // 自动滚动到文本框的底部，始终显示最新日志
            richTextBoxLog.ScrollToCaret();
        }

        // 拖拽进入时触发
        private void PanelDropTarget_DragEnter(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // 允许复制操作
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // 拖拽完成时触发
        private void PanelDropTarget_DragDrop(object sender, DragEventArgs e)
        {
            // 获取拖拽的文件路径
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // 过滤出图片文件（根据扩展名简单判断）
            var validImageFiles = files.Where(f => IsImageFile(f)).ToList();

            if (validImageFiles.Any())
            {
                // *** 修改点：移除 .Clear() ***
                // droppedFilePaths.Clear(); // 移除此行，实现追加效果

                // 将新的有效图片文件添加到现有列表中
                foreach (var filePath in validImageFiles)
                {
                    // 检查是否已存在，避免重复添加
                    if (!_droppedImagePaths.Contains(filePath))
                    {
                        _droppedImagePaths.Add(filePath);
                        listBoxFiles.Items.Add(filePath); // 只添加文件名
                        if (_droppedImagePaths.Count == 1)
                        {
                            txtOutputFolder.Text = Path.GetDirectoryName(_droppedImagePaths[0]);
                        }
                    }
                }

                LogMessage($"成功追加 {validImageFiles.Count} 张图片。", LogLevel.Info);
                // 打印追加的文件名，避免日志刷屏，可以只打印新添加的
                foreach (var filePath in validImageFiles)
                {
                    if (_droppedImagePaths.Contains(filePath) && _droppedImagePaths.Count(p => p == filePath) == 1) // 确保是这次添加的
                    {
                        LogMessage($"  + {Path.GetFileName(filePath)}", LogLevel.Info);
                    }
                }

                LogMessage($"当前列表共包含 {_droppedImagePaths.Count} 张图片。", LogLevel.Info);
                if (_droppedImagePaths.Count>5)
                {
                    LogMessage($"本次生成只会取前5张图片。", LogLevel.Warning);
                }
            }
            else
            {
                LogMessage("没有找到有效的图片文件。", LogLevel.Warning);
            }
        }

        // 判断是否是图片文件
        private bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath)?.ToLower();
            return extension != null && (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif");
        }

        private async void btnStartConversion_Click(object sender, EventArgs e)
        {
            if (_droppedImagePaths.Count < 5)
            {
                LogMessage("请选择 5 张图片。", LogLevel.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_backgroundMusicPath))
            {
                LogMessage("请选择背景音乐。", LogLevel.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtOutputFolder.Text) || !Directory.Exists(txtOutputFolder.Text))
            {
                LogMessage("请选择一个有效的视频输出文件夹。", LogLevel.Warning);
                return;
            }

            // 构建完整的输出文件路径
            string outputFileName = $"上传视频_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
            string outputPath = Path.Combine(txtOutputFolder.Text, outputFileName);

            LogMessage($"开始合成视频到: \"{outputPath}\"", LogLevel.Processing);
            btnStartConversion.Enabled = false;

            // 异步执行视频合成任务
            await ConvertImagesToVideoAsync(outputPath, _droppedImagePaths.Take(5).ToList());
        }

        // 异步执行图片合成视频的任务
        private async Task ConvertImagesToVideoAsync(string outputPath, List<string> imagePaths)
        {
            try
            {
                StringBuilder filterComplexBuilder = new StringBuilder();
                int numImages = imagePaths.Count;
                int transitionDuration = 1; // 转场持续时间（秒）
                int totalDuration = imagePaths.Count * ImageDurationSeconds;
                Random random = new Random();
                //string[] transitionTypes = { "fade", "wipeleft", "wiperight", "wipeup", "wipedown", "dissolve", "pixelize", "radial", "hblur", "blur", "crossfade" }; // 移除一些不太常见的或可能需要额外配置的
                string fixedTransitionType = "wipeleft"; // 只保留一种转场效果，例如 "fade"

                for (int i = 0; i < numImages; i++)
                {
                    filterComplexBuilder.Append($"[{i}:v]scale={OutputResolution},format=yuv420p,trim=duration={(i + 1) * ImageDurationSeconds}[v{i}];");
                }

                string prevStreamTag = "[v0]";
                for (int i = 0; i < numImages - 1; i++)
                {
                    string nextStreamTag = $"[v{i + 1}]";
                    // string randomTransition = transitionTypes[random.Next(transitionTypes.Length)]; // 移除随机选择
                    string currentTransitionType = fixedTransitionType; // 使用固定的转场类型

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

                    // 使用固定的转场类型
                    filterComplexBuilder.Append(
                        $"{prevStreamTag}{nextStreamTag}xfade=transition={currentTransitionType}:duration={transitionDuration}:offset={offset}[{outputTag.Trim('[', ']')}];");
                    prevStreamTag = outputTag;
                }

                string filterComplex = filterComplexBuilder.ToString();
                LogMessage($"构建的 FFmpeg filter_complex: {filterComplex}", LogLevel.Info);

                var arguments = new StringBuilder();
                arguments.Append(" -y "); // 覆盖输出文件

                // 添加图片输入参数
                foreach (var imagePath in imagePaths)
                {
                    arguments.Append($"-loop 1 -i \"{imagePath}\" ");
                }

                // 添加背景音乐输入参数
                arguments.Append($"-i \"{_backgroundMusicPath}\" ");

                arguments.Append($"-filter_complex \"{filterComplex}\" ");
                arguments.Append($"-map \"[vout]\" -map {imagePaths.Count}:a "); // imagePaths.Count 是音频流的索引

                arguments.Append($"-t {totalDuration} ");
                arguments.Append("-c:v libx264 ");
                arguments.Append("-pix_fmt yuv420p ");
                arguments.Append("-r 30 ");
                arguments.Append("-c:a aac ");
                arguments.Append("-b:a 192k ");

                arguments.Append($"\"{outputPath}\"");

                string command = arguments.ToString();
                LogMessage($"执行 FFmpeg 命令: {command}", LogLevel.Info);

                using (Process ffmpegProcess = new Process())
                {
                    ffmpegProcess.StartInfo.FileName = _ffmpegPath;
                    ffmpegProcess.StartInfo.Arguments = command;
                    ffmpegProcess.StartInfo.UseShellExecute = true;
                    ffmpegProcess.StartInfo.RedirectStandardOutput = false;
                    ffmpegProcess.StartInfo.RedirectStandardError = false;
                    ffmpegProcess.StartInfo.CreateNoWindow = false;
                    LogMessage("正在启动 FFmpeg...", LogLevel.Info);
                    try
                    {
                        ffmpegProcess.Start();
                        ffmpegProcess.WaitForExit();

                        if (ffmpegProcess.ExitCode == 0)
                        {
                            LogMessage("视频合成成功！", LogLevel.Success);
                            _droppedImagePaths.Clear();
                            listBoxFiles.Items.Clear();
                        }
                        else
                        {
                            LogMessage($"视频合成失败。FFmpeg 退出码: {ffmpegProcess.ExitCode}", LogLevel.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"启动 FFmpeg 时发生异常: {ex.Message}", LogLevel.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"处理视频合成时发生未知错误: {ex.Message}", LogLevel.Error);
                LogMessage($"堆栈跟踪: {ex.StackTrace}", LogLevel.Error);
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
                // 设置对话框的初始路径
                if (!string.IsNullOrEmpty(txtOutputFolder.Text) && Directory.Exists(txtOutputFolder.Text))
                {
                    fbd.SelectedPath = txtOutputFolder.Text;
                }
                else
                {
                    // 否则，使用我的文档作为默认
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
            LogMessage($"视频输出文件夹已设置为: \"{txtOutputFolder.Text}\"", LogLevel.Info);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _droppedImagePaths.Clear();
            listBoxFiles.Items.Clear();
        }

        // 日志输出函数
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
