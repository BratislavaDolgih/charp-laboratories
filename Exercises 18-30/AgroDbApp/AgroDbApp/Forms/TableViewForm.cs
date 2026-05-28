using System;
using System.Data;
using System.Windows.Forms;
using AgroDbApp.Data;

namespace AgroDbApp.Forms;

public partial class TableViewForm : Form
{
    private readonly string _objectName;
    private readonly string _title;

    private DataGridView _grid = null!;
    private MenuStrip _menu = null!;
    private ToolStripMenuItem _backMenuItem = null!;
    private ToolStripMenuItem _exitMenuItem = null!;

    public TableViewForm(string objectName, string title)
    {
        _objectName = objectName;
        _title = title;

        InitializeComponent();
        Load += TableViewForm_Load;
    }

    private void InitializeComponent()
    {
        Text = _title;
        Width = 1100;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        _menu = new MenuStrip();
        _backMenuItem = new ToolStripMenuItem("Вернуться");
        _exitMenuItem = new ToolStripMenuItem("Выйти");

        _backMenuItem.Click += (_, _) => Close();
        _exitMenuItem.Click += (_, _) => Application.Exit();

        _menu.Items.Add(_backMenuItem);
        _menu.Items.Add(_exitMenuItem);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        Controls.Add(_grid);
        Controls.Add(_menu);
        MainMenuStrip = _menu;
    }

    private void TableViewForm_Load(object? sender, EventArgs e)
    {
        try
        {
            DataTable table = Pg.SelectAllFrom(_objectName);
            _grid.DataSource = table;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка загрузки данных из {_objectName}:\n{ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}