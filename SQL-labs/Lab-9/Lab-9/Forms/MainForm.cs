using System;
using System.Windows.Forms;
using AgroDbApp.Data;
using AgroDbApp.Metadata;

using AgroDbApp.Forms;

namespace AgroDbApp.Forms;

public sealed class MainForm : Form
{
    private readonly MenuStrip _menu = new();

    public MainForm()
    {
        InitializeComponent();
        Load += MainForm_Load;
    }

    private void InitializeComponent()
    {
        Text = "Agro DB CRUD";
        Width = 1280;
        Height = 820;
        StartPosition = FormStartPosition.CenterScreen;

        var dictionaries = new ToolStripMenuItem("Справочники");
        var data = new ToolStripMenuItem("Данные");
        var reports = new ToolStripMenuItem("Отчеты");
        var about = new ToolStripMenuItem("О программе");
        var export = new ToolStripMenuItem("Экспорт базы");
        var exit = new ToolStripMenuItem("Выход");
        

        dictionaries.DropDownItems.Add("Должности", null, (_, _) => OpenCrud(Tables.Position));
        dictionaries.DropDownItems.Add("Типы почвы", null, (_, _) => OpenCrud(Tables.SoilType));
        dictionaries.DropDownItems.Add("Типы техники", null, (_, _) => OpenCrud(Tables.TechniqueType));
        dictionaries.DropDownItems.Add("Культуры", null, (_, _) => OpenCrud(Tables.Culture));

        data.DropDownItems.Add("Сотрудники", null, (_, _) => OpenCrud(Tables.Employee));
        data.DropDownItems.Add("Угодья", null, (_, _) => OpenCrud(Tables.Field));
        data.DropDownItems.Add("Парковки", null, (_, _) => OpenCrud(Tables.Parking));
        data.DropDownItems.Add("Техника", null, (_, _) => OpenCrud(Tables.Technique));
        data.DropDownItems.Add("Заявки", null, (_, _) => OpenCrud(Tables.Request));
        data.DropDownItems.Add("Операции", null, (_, _) => OpenCrud(Tables.Operation));
        data.DropDownItems.Add("Операция-Техника", null, (_, _) => OpenCrud(Tables.OperationTechnique));
        data.DropDownItems.Add("Посевы", null, (_, _) => OpenCrud(Tables.Sowing));
        data.DropDownItems.Add("Журнал", null, (_, _) => OpenCrud(Tables.JournalEntry));

        reports.DropDownItems.Add("Журнал операций", null, (_, _) => OpenReadOnly(Tables.VOperationJournal));
        reports.DropDownItems.Add("Состояние техники", null, (_, _) => OpenReadOnly(Tables.VTechniqueInfo));
        reports.DropDownItems.Add("Сводка по посевам", null, (_, _) => OpenReadOnly(Tables.VSowingSummary));

        about.Click += (_, _) =>
        {
            using var form = new AboutForm();
            form.ShowDialog(this);
        };

        exit.Click += (_, _) => Application.Exit();

        _menu.Items.Add(dictionaries);
        _menu.Items.Add(data);
        _menu.Items.Add(reports);
        _menu.Items.Add(export);
        _menu.Items.Add(about);
        _menu.Items.Add(exit);

        Controls.Add(_menu);
        MainMenuStrip = _menu;

        export.Click += (_, _) =>
        {
            using var form = new ExportForm();
            form.ShowDialog(this);
        };
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        try
        {
            Pg.GetOpenConnection();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось подключиться к БД:\n" + ex.Message);
        }
    }

    private void OpenCrud(TableMetadata meta)
    {
        var form = new CrudForm(meta, readOnlyMode: false, selectionMode: false);
        form.Show();
    }

    private void OpenReadOnly(TableMetadata meta)
    {
        var form = new CrudForm(meta, readOnlyMode: true, selectionMode: false);
        form.Show();
    }
}