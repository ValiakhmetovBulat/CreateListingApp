using System.Windows;
using System.Windows.Forms;
using System.IO;
using Xceed.Words.NET;
using Xceed.Document.NET;

namespace CreateListingApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedPath;
        private string filesContent;
        private Formatting fileNameFormatting;
        private Formatting fileContentFormating;
        private DocX document;

        public MainWindow()
        {
            InitializeComponent();
            filesContent = "";

            fileNameFormatting = new Formatting() 
            { 
                Size = 14D,
                FontFamily = new Font("Times New Roman"),                
            };

            

            fileContentFormating = new Formatting()
            {
                Size = 12D,
                FontFamily = new Font("Courier New")
            };
        }

        private void ButtonChooseFolder_Click(object sender, RoutedEventArgs e)
        {       
            filesContent = "";

            var folderBroserDialog = new FolderBrowserDialog();

            var dialogResult = folderBroserDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = folderBroserDialog.SelectedPath;

                if (selectedPath != null)
                {                    
                    TextBoxFolderPath.Text = selectedPath;

                    document = DocX.Create("listing.docx");                    

                    var files = Directory.GetFiles(selectedPath, "*", SearchOption.AllDirectories);

                    bool isObjSkipRequired = (bool)CheckBoxSkipObj.IsChecked;
                    bool isBinSkipRequired = (bool)CheckBoxSkipBin.IsChecked;
                    bool isCsProjSkipRequired = (bool)CheckBoxSkipCsProj.IsChecked;

                    var pathInter = Directory.GetCurrentDirectory();   

                    
                    foreach (var file in files)
                    {          
                        if (file.Contains("obj") && isObjSkipRequired)
                        {
                            continue;
                        }
                        if (file.Contains("bin") && isBinSkipRequired)
                        {
                            continue;
                        }
                        if (file.Contains(".csproj") && isCsProjSkipRequired)
                        {
                            continue;
                        }
                        if (file.Contains("wwwroot"))
                        {
                            continue;
                        }
                        if (file.Contains("Migrations"))
                        {
                            continue;
                        }
                        if (file.Contains(".sql"))
                        {
                            continue;
                        }

                        var fileName = System.IO.Path.GetFileName(file);
                        document.InsertParagraph(fileName, false, fileNameFormatting).SetLineSpacing(LineSpacingType.Line, 18f);                
                        filesContent += fileName;
                        filesContent += "\n";

                        var fileContent = File.ReadAllText(file);
                        document.InsertParagraph(fileContent, false, fileContentFormating).SetLineSpacing(LineSpacingType.Line, 12f);
                        filesContent += fileContent;
                        filesContent += "\n";
                    }

                    TextBoxFilesContent.Text = filesContent;
                }                
            }
        }

        private void ButtonConvertToWord_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Документ Word (*.docx)|*.docx";
            saveFileDialog.FileName = "listing.docx";
            saveFileDialog.Title = "Сохранить в Word";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
                var savePath = saveFileDialog.FileName;
                document.SaveAs(savePath);
                System.Windows.MessageBox.Show("Успешно!");
            }
        }
    }
}
