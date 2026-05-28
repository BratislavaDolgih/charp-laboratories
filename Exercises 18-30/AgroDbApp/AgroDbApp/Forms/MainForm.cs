using System;
using System.Windows.Forms;
using AgroDbApp.Data;

namespace AgroDbApp.Forms;

public partial class MainForm : Form
{
    private MenuStrip _menu = null!;

    public MainForm()
    {
        InitializeComponent();
        Load += MainForm_Load;
    }

    private void InitializeComponent()
    {
        Text = "Agro DB App";
        Width = 1200;
        Height = 800;
        StartPosition = FormStartPosition.CenterScreen;

        _menu = new MenuStrip();

        var dictionariesMenu = new ToolStripMenuItem("Справочники");
        var dataMenu = new ToolStripMenuItem("Данные");
        var reportsMenu = new ToolStripMenuItem("Отчеты");
        var aboutMenu = new ToolStripMenuItem("О программе");
        var exitMenu = new ToolStripMenuItem("Выход");

        // Справочники
        dictionariesMenu.DropDownItems.Add("Должности", null, (_, _) => OpenTable("position", "Справочник должностей"));
        dictionariesMenu.DropDownItems.Add("Типы почвы", null, (_, _) => OpenTable("soil_type", "Справочник типов почвы"));
        dictionariesMenu.DropDownItems.Add("Типы техники", null, (_, _) => OpenTable("technique_type", "Справочник типов техники"));
        dictionariesMenu.DropDownItems.Add("Культуры", null, (_, _) => OpenTable("culture", "Справочник культур"));

        // Данные
        dataMenu.DropDownItems.Add("Сотрудники", null, (_, _) => OpenTable("employee", "Сотрудники"));
        dataMenu.DropDownItems.Add("Угодья", null, (_, _) => OpenTable("field", "Угодья"));
        dataMenu.DropDownItems.Add("Парковки", null, (_, _) => OpenTable("parking", "Парковки"));
        dataMenu.DropDownItems.Add("Техника", null, (_, _) => OpenTable("technique", "Техника"));
        dataMenu.DropDownItems.Add("Заявки", null, (_, _) => OpenTable("request", "Заявки"));
        dataMenu.DropDownItems.Add("Операции", null, (_, _) => OpenTable("operation", "Операции"));
        dataMenu.DropDownItems.Add("Посевы", null, (_, _) => OpenTable("sowing", "Посевы"));
        dataMenu.DropDownItems.Add("Журнал", null, (_, _) => OpenTable("journal_entry", "Журнал"));
        dataMenu.DropDownItems.Add("Операция-Техника", null, (_, _) => OpenTable("operation_technique", "Операция-Техника"));

        // Отчеты — читаем VIEW из ЛР №6
        reportsMenu.DropDownItems.Add("Журнал операций", null, (_, _) => OpenTable("v_operation_journal", "Отчет: журнал операций"));
        reportsMenu.DropDownItems.Add("Состояние техники", null, (_, _) => OpenTable("v_technique_info", "Отчет: состояние техники"));
        reportsMenu.DropDownItems.Add("Сводка по посевам", null, (_, _) => OpenTable("v_sowing_summary", "Отчет: сводка по посевам"));

        aboutMenu.Click += (_, _) =>
        {
            using var form = new AboutForm();
            form.ShowDialog(this);
        };

        exitMenu.Click += (_, _) => Application.Exit();

        _menu.Items.Add(dictionariesMenu);
        _menu.Items.Add(dataMenu);
        _menu.Items.Add(reportsMenu);
        _menu.Items.Add(aboutMenu);
        _menu.Items.Add(exitMenu);

        Controls.Add(_menu);
        MainMenuStrip = _menu;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        try
        {
            Pg.GetOpenConnection();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не удалось подключиться к БД:\n{ex.Message}",
                "Ошибка подключения",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void OpenTable(string objectName, string title)
    {
        var form = new TableViewForm(objectName, title);
        form.Show();
    }
}