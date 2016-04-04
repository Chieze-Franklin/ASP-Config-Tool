using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AspWebConfig
{
    public partial class Form1 : Form
    {
        TabPage pgExecute, pgSettings;
        TextBox txtProjPath, txtIISPath;
        Settings settings;
        FolderBrowserDialog folderDialog;
        Button btnBrowseProjPath;
        bool reloadIIS = true;

        public Form1()
        {
            InitializeComponent();

            InitThis();
            InitTabs();
        }

        void InitThis()
        {   
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "ASP Config Tool";

            try
            {
                Settings.CreateSettingsFile();
                settings = Settings.FromFile();
            }
            catch
            {
                settings = new Settings();
            }

            folderDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
            };
        }

        void InitTabs()
        {
            TabControl tabControl = new TabControl()
            {
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(tabControl);

            pgExecute = new TabPage()
            {
                Text = "Execute"
            };
            tabControl.TabPages.Add(pgExecute);

            pgSettings = new TabPage()
            {
                Text = "Settings"
            };
            tabControl.TabPages.Add(pgSettings);

            InitTabExecute();
            InitTabSettings();
        }

        void InitTabExecute()
        {
            Label lblProjPath = new Label()
            {
                AutoSize = true,
                Location = new Point(5, 5),
                Text = "Enter your project path (usually the folder that contains the web.config file):"
            };
            pgExecute.Controls.Add(lblProjPath);

            txtProjPath = new TextBox()
            {
                Location = new Point(30, lblProjPath.Bottom + 2),
                Text = settings.ProjectPath,
                Width = 400,
            };
            txtProjPath.TextChanged += delegate { settings.ProjectPath = txtProjPath.Text; };
            pgExecute.Controls.Add(txtProjPath);

            btnBrowseProjPath = new Button()
            {
                Font = new Font("Times New Roman", 12.0f, FontStyle.Regular),
                Text = char.ConvertFromUtf32(8226) + char.ConvertFromUtf32(8226) + char.ConvertFromUtf32(8226),
                TextAlign = ContentAlignment.TopCenter,
                Size = new Size(35, 20),
            };
            btnBrowseProjPath.Location = new Point(txtProjPath.Right + 2, txtProjPath.Top);
            btnBrowseProjPath.Click += delegate 
            {
                folderDialog.SelectedPath = txtProjPath.Text;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtProjPath.Text = folderDialog.SelectedPath;
                }
            };
            pgExecute.Controls.Add(btnBrowseProjPath);

            Button btnExecute = new Button()
            {
                Text = "Launch ASP.NET Configuration Website",
                Width = 300
            };
            btnExecute.Location = new Point(txtProjPath.Right - btnExecute.Width, txtProjPath.Bottom + 5);
            btnExecute.Click += BtnExecute_Click;
            pgExecute.Controls.Add(btnExecute);
        }

        void InitTabSettings()
        {
            Label lblIISPath = new Label()
            {
                AutoSize = true,
                Location = new Point(5, 5),
                Text = "Enter IIS Express Path:"
            };
            pgSettings.Controls.Add(lblIISPath);

            RadioButton radio64Bit = new RadioButton()
            {
                AutoSize = true,
                Checked = false,
                Location = new Point(lblIISPath.Left, lblIISPath.Bottom + 2),
                Text = "64 Bit IIS Express"
            };
            radio64Bit.CheckedChanged += delegate
            {
                if (radio64Bit.Checked) txtIISPath.Text = @"C:\Program Files\IIS Express\iisexpress.exe";
            };
            pgSettings.Controls.Add(radio64Bit);

            RadioButton radio32Bit = new RadioButton()
            {
                AutoSize = true,
                Checked = false,
                Location = new Point(lblIISPath.Left, radio64Bit.Bottom + 2),
                Text = "32 Bit IIS Express"
            };
            radio32Bit.CheckedChanged += delegate
            {
                if (radio32Bit.Checked) txtIISPath.Text = @"C:\Program Files (x86)\IIS Express\iisexpress.exe";
            };
            pgSettings.Controls.Add(radio32Bit);

            txtIISPath = new TextBox()
            {
                Location = new Point(30, radio32Bit.Bottom + 2),
                Text = settings.IISExpressPath,
                Width = txtProjPath.Width,
            };
            txtIISPath.TextChanged += delegate { settings.IISExpressPath = txtIISPath.Text; reloadIIS = true; };
            pgSettings.Controls.Add(txtIISPath);

            Button btnBrowseIISPath = new Button()
            {
                Font = btnBrowseProjPath.Font,
                Text = btnBrowseProjPath.Text,
                TextAlign = btnBrowseProjPath.TextAlign,
                Size = btnBrowseProjPath.Size,
            };
            btnBrowseIISPath.Location = new Point(txtIISPath.Right + 2, txtIISPath.Top);
            btnBrowseIISPath.Click += delegate
            {
                folderDialog.SelectedPath = txtIISPath.Text;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtIISPath.Text = folderDialog.SelectedPath;
                }
            };
            pgSettings.Controls.Add(btnBrowseIISPath);

            Label lblWebAdminFilesPath = new Label()
            {
                AutoSize = true,
                Location = new Point(lblIISPath.Left, txtIISPath.Bottom + 5),
                Text = "ASP.NET Web Admin Files Path:"
            };
            pgSettings.Controls.Add(lblWebAdminFilesPath);

            TextBox txtWebAdminFilesPath = new TextBox()
            {
                Location = new Point(30, lblWebAdminFilesPath.Bottom + 2),
                Text = settings.ASPWebAdminFilesPath,
                Width = txtProjPath.Width,
            };
            txtWebAdminFilesPath.TextChanged += delegate { settings.ASPWebAdminFilesPath = txtWebAdminFilesPath.Text; reloadIIS = true; };
            pgSettings.Controls.Add(txtWebAdminFilesPath);

            Button btnBrowseAdminFilesPath = new Button()
            {
                Font = btnBrowseProjPath.Font,
                Text = btnBrowseProjPath.Text,
                TextAlign = btnBrowseProjPath.TextAlign,
                Size = btnBrowseProjPath.Size,
            };
            btnBrowseAdminFilesPath.Location = new Point(txtWebAdminFilesPath.Right + 2, txtWebAdminFilesPath.Top);
            btnBrowseAdminFilesPath.Click += delegate
            {
                folderDialog.SelectedPath = txtWebAdminFilesPath.Text;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtWebAdminFilesPath.Text = folderDialog.SelectedPath;
                }
            };
            pgSettings.Controls.Add(btnBrowseAdminFilesPath);

            Label lblPort = new Label()
            {
                AutoSize = true,
                Location = new Point(lblIISPath.Left, txtWebAdminFilesPath.Bottom + 5),
                Text = "Port:"
            };
            pgSettings.Controls.Add(lblPort);

            NumericUpDown txtPort = new NumericUpDown()
            {
                Location = new Point(30, lblPort.Bottom + 2),
                Maximum = 65535,
                Minimum = 0,
                Value = settings.Port,
                Width = txtProjPath.Width / 7,
            };
            txtPort.ValueChanged += delegate { settings.Port = (int)txtPort.Value; reloadIIS = true; };
            pgSettings.Controls.Add(txtPort);

            Label lblClr = new Label()
            {
                AutoSize = true,
                Location = new Point(lblIISPath.Left, txtPort.Bottom + 5),
                Text = "CLR:"
            };
            pgSettings.Controls.Add(lblClr);

            TextBox txtClr = new TextBox()
            {
                Location = new Point(30, lblClr.Bottom + 2),
                Text = settings.CLR,
                Width = txtProjPath.Width / 7,
            };
            txtClr.TextChanged += delegate { settings.CLR = txtClr.Text; reloadIIS = true; };
            pgSettings.Controls.Add(txtClr);

            Button btnSave = new Button()
            {
                Text = "Save Settings",
                Width = 200
            };
            btnSave.Location = new Point(txtIISPath.Right - btnSave.Width, txtClr.Bottom + 5);
            btnSave.Click += delegate 
            {
                Settings.CreateSettingsFile();
                settings.Save();
            };
            pgSettings.Controls.Add(btnSave);
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(settings.ProjectPath))
                    throw new NullReferenceException("Invalid project path.");
                if (!Directory.Exists(settings.ProjectPath))
                    throw new DirectoryNotFoundException("Specified project path could not be found.");

                //run IIS Express
                if (reloadIIS)
                {
                    reloadIIS = false;

                    //IISBatchFile.CreateBatchFile(settings);
                    //IISBatchFile.RunBatchFile();
                    Process.Start($@"{settings.IISExpressPath}\iisexpress.exe"
                        , $@"/path:{settings.ASPWebAdminFilesPath} " +
                        "/vpath:\"/asp.netwebadminfiles\" " +
                        $@"/port:{settings.Port} " +
                        $@"/clr:{settings.CLR} " +
                        "/ntlm"
                        );

                    //wait for 5 seconds
                    var tempCursor = this.Cursor;
                    this.Cursor = Cursors.WaitCursor;
                    System.Threading.Thread.Sleep(5000);
                    this.Cursor = tempCursor;
                }

                //run the page on the browser
                Process.Start($@"http://localhost:{settings.Port}/asp.netwebadminfiles/default.aspx?applicationPhysicalPath={settings.ProjectPath}\&applicationUrl=/");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    [Serializable]
    public class Settings
    {
        public static void CreateSettingsFile(string filePath = SettingsFileNameWithExtension)
        {
            if (!File.Exists(filePath))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                    byte[] byteText = System.Text.Encoding.ASCII.GetBytes("");

                    fs.Write(byteText, 0, byteText.Length);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }
        }
        public static Settings FromFile(string filePath = SettingsFileNameWithExtension)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            Stream stream = null;

            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                return (Settings)serializer.Deserialize(stream);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        public void Save(string filePath = SettingsFileNameWithExtension)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            Stream stream = null;

            try
            {
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                serializer.Serialize(stream, this);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        [OptionalField]
        public string ASPWebAdminFilesPath = @"C:\windows\Microsoft.NET\Framework\v4.0.30319\ASP.NETWebAdminFiles";
        [OptionalField]
        public string CLR = @"4.0";
        [OptionalField]
        public string IISExpressPath = @"C:\Program Files\IIS Express";
        [OptionalField]
        public int Port = 8089;
        [OptionalField]
        public string ProjectPath;

        public const string SettingsFileNameWithExtension = "act-settings.xml";
    }

    //public class IISBatchFile
    //{
    //    public static void CreateBatchFile(Settings settings, string filePath = SettingsFileNameWithExtension)
    //    {
    //        //if (!File.Exists(filePath))
    //        {
    //            FileStream fs = null;
    //            try
    //            {
    //                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

    //                var fileContent = $"cd {settings.IISExpressPath}\n" +
    //                    $@"iisexpress " +
    //                    $@"/path:{settings.ASPWebAdminFilesPath} " +
    //                    "/vpath:\"/asp.netwebadminfiles\" " +
    //                    $@"/port:{settings.Port} " +
    //                    $@"/clr:{settings.CLR} " +
    //                    "/ntlm";
    //                byte[] byteText = System.Text.Encoding.ASCII.GetBytes(fileContent);

    //                fs.Write(byteText, 0, byteText.Length);
    //            }
    //            catch
    //            {
    //                throw;
    //            }
    //            finally
    //            {
    //                if (fs != null)
    //                    fs.Close();
    //            }
    //        }
    //    }

    //    public static void RunBatchFile(string filePath = SettingsFileNameWithExtension)
    //    {
    //        Process.Start(filePath);
    //    }

    //    public const string SettingsFileNameWithExtension = "iisexpress.bat";
    //}
}
