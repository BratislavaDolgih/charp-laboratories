using AgroDbApp.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AgroDbApp.Forms
{
    public partial class ExportForm : Form
    {
        public ExportForm()
        {
            InitializeComponent();
            Load += ExportForm_Load;
        }

        private void ExportForm_Load(object? sender, EventArgs e)
        {
            radioButtonExcel.Checked = true;
        }

        private void ExportToExcel(string objectName)
        {
            try
            {
                List<List<string>> table = Pg.SelectAll(objectName);

                if (table == null || table.Count == 0)
                {
                    MessageBox.Show(
                        "Нет данных для экспорта.",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                saveFileDialog1.FileName = objectName;
                saveFileDialog1.DefaultExt = "xls";
                saveFileDialog1.Filter = "Excel files (*.xls)|*.xls";
                saveFileDialog1.Title = "Экспорт в Excel";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using FileStream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    using StreamWriter writer = new StreamWriter(stream, Encoding.Unicode);

                    foreach (string column in table[0])
                    {
                        writer.Write(column + "\t");
                    }
                    writer.WriteLine();

                    for (int i = 1; i < table.Count; i++)
                    {
                        foreach (string cell in table[i])
                        {
                            writer.Write(cell + "\t");
                        }
                        writer.WriteLine();
                    }

                    writer.Flush();

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = saveFileDialog1.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при экспорте в Excel:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ExportToHtml(string objectName)
        {
            try
            {
                List<List<string>> table = Pg.SelectAll(objectName);

                if (table == null || table.Count == 0)
                {
                    MessageBox.Show(
                        "Нет данных для экспорта.",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                saveFileDialog1.FileName = objectName;
                saveFileDialog1.DefaultExt = "html";
                saveFileDialog1.Filter = "HTML files (*.html)|*.html";
                saveFileDialog1.Title = "Экспорт в HTML";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using FileStream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

                    writer.WriteLine("<html>");
                    writer.WriteLine("<head>");
                    writer.WriteLine("<meta charset=\"utf-8\">");
                    writer.WriteLine($"<title>{objectName}</title>");
                    writer.WriteLine("</head>");
                    writer.WriteLine("<body bgcolor=\"#800000\">");
                    writer.WriteLine("<table align=\"center\" border=\"1\" cellspacing=\"0\" cellpadding=\"6\">");

                    writer.WriteLine("<tr>");
                    foreach (string header in table[0])
                    {
                        writer.WriteLine($"<th><font face=\"Verdana\" size=\"2\" color=\"#ffffff\">{System.Net.WebUtility.HtmlEncode(header)}</font></th>");
                    }
                    writer.WriteLine("</tr>");

                    for (int i = 1; i < table.Count; i++)
                    {
                        string rowColor = (i % 2 == 1) ? "#cccccc" : "#ffffff";

                        writer.WriteLine($"<tr bgcolor=\"{rowColor}\">");

                        foreach (string cell in table[i])
                        {
                            writer.WriteLine($"<td><font face=\"Verdana\" size=\"2\" color=\"#000000\">{System.Net.WebUtility.HtmlEncode(cell)}</font></td>");
                        }

                        writer.WriteLine("</tr>");
                    }

                    writer.WriteLine("</table>");
                    writer.WriteLine("</body>");
                    writer.WriteLine("</html>");

                    writer.Flush();

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = saveFileDialog1.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при экспорте в HTML:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void buttonExport_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!checkBoxOperationJournal.Checked &&
                    !checkBoxSowingSummary.Checked &&
                    !checkBoxTechniqueInfo.Checked)
                {
                    MessageBox.Show(
                        "Выберите хотя бы один отчет для экспорта.",
                        "Предупреждение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!radioButtonExcel.Checked && !radioButtonHtml.Checked)
                {
                    MessageBox.Show(
                        "Выберите формат экспорта.",
                        "Предупреждение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (radioButtonExcel.Checked)
                {
                    if (checkBoxOperationJournal.Checked)
                        ExportToExcel("v_operation_journal");

                    if (checkBoxSowingSummary.Checked)
                        ExportToExcel("v_sowing_summary");

                    if (checkBoxTechniqueInfo.Checked)
                        ExportToExcel("v_technique_info");
                }
                else if (radioButtonHtml.Checked)
                {
                    if (checkBoxOperationJournal.Checked)
                        ExportToHtml("v_operation_journal");

                    if (checkBoxSowingSummary.Checked)
                        ExportToHtml("v_sowing_summary");

                    if (checkBoxTechniqueInfo.Checked)
                        ExportToHtml("v_technique_info");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при выполнении экспорта:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}