#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MaxBlocker   // ⚠️ При необходимости замените на имя вашего проекта
{
    public partial class Form1 : Form
    {
        // Элементы интерфейса
        private Button button1;
        private Button button2;
        private Button button3;
        private Label titleLabel;
        private Label subtitleLabel;
        private Panel mainPanel;

        // Данные для блокировки
        private readonly string[] maxDomains = new string[]
        {
            "max.ru",
            "www.max.ru",
            "web.max.ru",
            "download.max.ru",
            "api.max.ru",
            "oneme.ru",
            "api.oneme.ru"
        };

        private readonly string hostsPath = @"C:\Windows\System32\drivers\etc\hosts";
        private const string BlockIp = "127.0.0.1";
        private const string BlockComment = "# MAX Blocker";

        public Form1()
        {
            // Настройки формы
            this.Text = "Max-Blocker";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 40);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Панель для центрирования содержимого
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(mainPanel);

            // Заголовок
            titleLabel = new Label
            {
                Text = "Max-Blocker",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 128, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(500, 60),
                Location = new Point(50, 30)
            };
            mainPanel.Controls.Add(titleLabel);

            // Подзаголовок
            subtitleLabel = new Label
            {
                Text = "Блокировка MAX и его сайтов",
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(500, 30),
                Location = new Point(50, 95)
            };
            mainPanel.Controls.Add(subtitleLabel);

            // Создаём кнопки
            CreateStyledButtons();
        }

        // Создание стильных кнопок
        private void CreateStyledButtons()
        {
            Size buttonSize = new Size(160, 50);
            int startX = (this.ClientSize.Width - buttonSize.Width * 3 - 40) / 2; // центрирование
            int yPos = 160;

            // Кнопка "Заблокировать"
            button1 = new Button
            {
                Text = "Заблокировать",
                Size = buttonSize,
                Location = new Point(startX, yPos),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button1.Click += Button1_Click;
            button1.MouseEnter += (s, e) => button1.BackColor = Color.FromArgb(200, 35, 50);
            button1.MouseLeave += (s, e) => button1.BackColor = Color.FromArgb(220, 53, 69);

            // Кнопка "Узнать состояние"
            button2 = new Button
            {
                Text = "Узнать состояние",
                Size = buttonSize,
                Location = new Point(startX + buttonSize.Width + 20, yPos),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button2.Click += Button2_Click;
            button2.MouseEnter += (s, e) => button2.BackColor = Color.FromArgb(0, 105, 217);
            button2.MouseLeave += (s, e) => button2.BackColor = Color.FromArgb(0, 123, 255);

            // Кнопка "Разблокировать"
            button3 = new Button
            {
                Text = "Разблокировать",
                Size = buttonSize,
                Location = new Point(startX + (buttonSize.Width + 20) * 2, yPos),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button3.Click += Button3_Click;
            button3.MouseEnter += (s, e) => button3.BackColor = Color.FromArgb(30, 140, 50);
            button3.MouseLeave += (s, e) => button3.BackColor = Color.FromArgb(40, 167, 69);

            // Добавляем кнопки на панель
            mainPanel.Controls.Add(button1);
            mainPanel.Controls.Add(button2);
            mainPanel.Controls.Add(button3);
        }

        // Обработчики кнопок
        private void Button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Заблокировать MAX? Программа будет перезапущена от имени администратора.",
                                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                RunAsAdmin("/block");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            CheckStatus();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Разблокировать MAX? Программа будет перезапущена от имени администратора.",
                                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                RunAsAdmin("/unblock");
            }
        }

        // Перезапуск с правами администратора
        private void RunAsAdmin(string arguments)
        {
            try
            {
                ProcessStartInfo procInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = arguments,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(procInfo);
                Application.Exit();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Не удалось получить права администратора. Операция отменена.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Точка входа для действий с правами администратора
        public void PerformAction(string action)
        {
            switch (action)
            {
                case "/block":
                    BlockMax();
                    break;
                case "/unblock":
                    UnblockMax();
                    break;
            }
        }

        // Блокировка MAX
        private void BlockMax()
        {
            try
            {
                // 1. Блокировка доменов через hosts
                List<string> hostsLines = new List<string>(File.ReadAllLines(hostsPath));
                hostsLines.RemoveAll(line => line.Contains(BlockComment));
                foreach (string domain in maxDomains)
                {
                    hostsLines.Add($"{BlockIp} {domain} {BlockComment}");
                }
                File.WriteAllLines(hostsPath, hostsLines);

                // 2. Блокировка через брандмауэр (если найден exe)
                string maxExePath = FindMaxExecutable();
                if (!string.IsNullOrEmpty(maxExePath))
                {
                    ExecuteCommand($"netsh advfirewall firewall add rule name=\"MAX Blocker\" dir=out program=\"{maxExePath}\" action=block");
                    ExecuteCommand($"netsh advfirewall firewall add rule name=\"MAX Blocker\" dir=in program=\"{maxExePath}\" action=block");
                    MessageBox.Show("MAX заблокирован (Hosts + Брандмауэр).", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Домены MAX заблокированы в hosts. Исполняемый файл не найден, правило брандмауэра не создано.",
                                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Ошибка доступа. Запустите программу от имени администратора.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при блокировке: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Разблокировка MAX
        private void UnblockMax()
        {
            try
            {
                // Удаление записей из hosts
                List<string> hostsLines = new List<string>(File.ReadAllLines(hostsPath));
                hostsLines.RemoveAll(line => line.Contains(BlockComment));
                File.WriteAllLines(hostsPath, hostsLines);

                // Удаление правил брандмауэра
                ExecuteCommand("netsh advfirewall firewall delete rule name=\"MAX Blocker\"");
                MessageBox.Show("MAX разблокирован.", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при разблокировке: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Проверка статуса блокировки
        private void CheckStatus()
        {
            try
            {
                bool isBlocked = false;
                if (File.Exists(hostsPath))
                {
                    string[] lines = File.ReadAllLines(hostsPath);
                    foreach (string line in lines)
                    {
                        if (line.Contains(BlockComment))
                        {
                            isBlocked = true;
                            break;
                        }
                    }
                }

                bool firewallRuleExists = CheckFirewallRule();

                if (isBlocked && firewallRuleExists)
                    MessageBox.Show("Статус: MAX ЗАБЛОКИРОВАН (Hosts + Брандмауэр)", "Состояние",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (isBlocked)
                    MessageBox.Show("Статус: MAX ЗАБЛОКИРОВАН частично (только Hosts)", "Состояние",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Статус: MAX НЕ ЗАБЛОКИРОВАН", "Состояние",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке статуса: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Поиск исполняемого файла MAX в типичных местах
        private string FindMaxExecutable()
        {
            string[] possiblePaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "MAX", "MAX.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "MAX", "MAX.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MAX", "MAX.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MAX", "MAX.exe")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        // Проверка существования правила брандмауэра
        private bool CheckFirewallRule()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "netsh";
                process.StartInfo.Arguments = "advfirewall firewall show rule name=\"MAX Blocker\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output.Contains("MAX Blocker");
            }
            catch
            {
                return false;
            }
        }

        // Выполнение команды в командной строке
        private void ExecuteCommand(string command)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c " + command;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения команды: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Переопределение метода для обработки аргументов командной строки при запуске
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // Если есть аргументы, выполняем действие и закрываем форму
                PerformAction(args[1]);
                Application.Exit();
            }
        }
    }
}