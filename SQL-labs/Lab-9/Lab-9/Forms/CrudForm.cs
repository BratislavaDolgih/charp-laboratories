using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AgroDbApp.Data;
using AgroDbApp.Metadata;
using Npgsql;

namespace AgroDbApp.Forms;

public sealed class CrudForm : Form
{
    private readonly TableMetadata _meta;
    private readonly bool _readOnlyMode;
    private readonly bool _selectionMode;
    private bool _isUpdatingPagination;

    public object? SelectedId { get; private set; }

    private readonly DataGridView _grid = new();
    private readonly Button _btnCreate = new();
    private readonly Button _btnUpdate = new();
    private readonly Button _btnDelete = new();
    private readonly Button _btnSelect = new();
    private readonly Button _btnPrev = new();
    private readonly Button _btnNext = new();
    private readonly Button _btnRefresh = new();
    private readonly Button _btnClose = new();
    private readonly NumericUpDown _pageNumber = new();
    private readonly NumericUpDown _pageSize = new();
    private readonly Label _lblTotal = new();

    public CrudForm(TableMetadata meta, bool readOnlyMode = false, bool selectionMode = false)
    {
        _meta = meta;
        _readOnlyMode = readOnlyMode;
        _selectionMode = selectionMode;

        InitializeComponent();
        Load += CrudForm_Load;
    }

    private void InitializeComponent()
    {
        Text = _meta.Title;
        Width = 1200;
        Height = 720;
        StartPosition = FormStartPosition.CenterScreen;

        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = true;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.CellDoubleClick += Grid_CellDoubleClick;

        _btnCreate.Text = "Create";
        _btnUpdate.Text = "Update";
        _btnDelete.Text = "Delete";
        _btnSelect.Text = "Выбрать";
        _btnPrev.Text = "<";
        _btnNext.Text = ">";
        _btnRefresh.Text = "Обновить";
        _btnClose.Text = "Close";

        _btnCreate.Click += BtnCreate_Click;
        _btnUpdate.Click += BtnUpdate_Click;
        _btnDelete.Click += BtnDelete_Click;
        _btnSelect.Click += BtnSelect_Click;
        _btnPrev.Click += BtnPrev_Click;
        _btnNext.Click += BtnNext_Click;
        _btnRefresh.Click += (_, _) => LoadPage();
        _btnClose.Click += (_, _) => Close();

        _pageNumber.Minimum = 1;
        _pageNumber.Maximum = 100000;
        _pageNumber.Value = 1;
        _pageNumber.Width = 80;
        _pageNumber.ValueChanged += (_, _) =>
        {
            if (!_isUpdatingPagination)
                LoadPage();
        };

        _pageSize.Minimum = 1;
        _pageSize.Maximum = 5000;
        _pageSize.Value = 1000;
        _pageSize.Width = 80;
        _pageSize.ValueChanged += (_, _) =>
        {
            if (_isUpdatingPagination)
                return;

            _isUpdatingPagination = true;
            _pageNumber.Value = 1;
            _isUpdatingPagination = false;
            LoadPage();
        };

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(8),
            AutoSize = false
        };

        topPanel.Controls.Add(_btnCreate);
        topPanel.Controls.Add(_btnUpdate);
        topPanel.Controls.Add(_btnDelete);
        topPanel.Controls.Add(_btnSelect);
        topPanel.Controls.Add(_btnRefresh);
        topPanel.Controls.Add(new Label { Text = "Страница:", AutoSize = true, Padding = new Padding(8, 8, 0, 0) });
        topPanel.Controls.Add(_pageNumber);
        topPanel.Controls.Add(_btnPrev);
        topPanel.Controls.Add(_btnNext);
        topPanel.Controls.Add(new Label { Text = "Размер:", AutoSize = true, Padding = new Padding(8, 8, 0, 0) });
        topPanel.Controls.Add(_pageSize);
        topPanel.Controls.Add(_lblTotal);
        topPanel.Controls.Add(_btnClose);

        Controls.Add(_grid);
        Controls.Add(topPanel);
    }

    private void CrudForm_Load(object? sender, EventArgs e)
    {
        if (_readOnlyMode)
        {
            _btnCreate.Hide();
            _btnUpdate.Hide();
            _btnDelete.Hide();
        }

        if (!_selectionMode)
        {
            _btnSelect.Hide();
        }

        if (_meta.IsView)
        {
            _btnCreate.Hide();
            _btnUpdate.Hide();
            _btnDelete.Hide();
        }

        LoadPage();
    }

    private void LoadPage()
    {
        try
        {
            int requestedPage = (int)_pageNumber.Value;
            int size = (int)_pageSize.Value;
            int total = Pg.CountRows(_meta.TableName);
            int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));
            int page = Math.Min(requestedPage, totalPages);

            _isUpdatingPagination = true;
            _pageNumber.Maximum = totalPages;
            _pageNumber.Value = page;
            _isUpdatingPagination = false;

            DataTable table = Pg.SelectPage(_meta.TableName, _meta.KeyColumn, size, page);
            _grid.DataSource = table;

            string diagnostics = $"   Всего: {total} | Страница {page} из {totalPages} | БД: {Pg.GetConnectionInfo()}";

            if (_meta.TableName == "field")
            {
                int autoFieldCount = Pg.CountRowsWhereLike("field", "name", "AUTO_FIELD_%");
                diagnostics += $" | AUTO_FIELD: {autoFieldCount}";
            }

            _lblTotal.Text = diagnostics;
            _btnPrev.Enabled = page > 1;
            _btnNext.Enabled = page < totalPages;
        }
        catch (Exception ex)
        {
            _isUpdatingPagination = false;
            MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private long? GetSelectedId()
    {
        if (_grid.SelectedRows.Count == 0)
            return null;

        object? value = _grid.SelectedRows[0].Cells[_meta.KeyColumn].Value;
        if (value == null || value == DBNull.Value)
            return null;

        return Convert.ToInt64(value);
    }

    private void BtnPrev_Click(object? sender, EventArgs e)
    {
        if (_pageNumber.Value > 1)
            _pageNumber.Value--;
    }

    private void BtnNext_Click(object? sender, EventArgs e)
    {
        if (_pageNumber.Value < _pageNumber.Maximum)
            _pageNumber.Value++;
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_meta.IsView)
            return;

        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выбери хотя бы одну строку.");
            return;
        }

        var confirm = MessageBox.Show(
            "Удалить выбранные записи?",
            "Подтверждение",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
            return;

        try
        {
            List<object> ids = new();

            foreach (DataGridViewRow row in _grid.SelectedRows)
            {
                object? value = row.Cells[_meta.KeyColumn].Value;
                if (value != null && value != DBNull.Value)
                    ids.Add(value);
            }

            int deleted = Pg.DeleteByIds(_meta.TableName, _meta.KeyColumn, ids);

            MessageBox.Show(
                $"Операция удаления выполнена.\nОбработано записей: {ids.Count}\nФизически удалено: {deleted}",
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadPage();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(
                "Ошибка PostgreSQL при удалении:\n" + ex.MessageText,
                "Ошибка триггера",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Ошибка удаления:\n" + ex.Message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void BtnCreate_Click(object? sender, EventArgs e)
    {
        if (_meta.EditorFactory == null)
            return;

        using var form = _meta.EditorFactory(EditMode.Create, null);
        if (form.ShowDialog(this) == DialogResult.OK)
            LoadPage();
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (_meta.EditorFactory == null)
            return;

        long? id = GetSelectedId();
        if (id == null)
        {
            MessageBox.Show("Сначала выбери запись.");
            return;
        }

        using var form = _meta.EditorFactory(EditMode.Update, id.Value);
        if (form.ShowDialog(this) == DialogResult.OK)
            LoadPage();
    }

    private void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (_selectionMode)
            SelectCurrentAndClose();
    }

    private void BtnSelect_Click(object? sender, EventArgs e)
    {
        SelectCurrentAndClose();
    }

    private void SelectCurrentAndClose()
    {
        long? id = GetSelectedId();
        if (id == null)
        {
            MessageBox.Show("Сначала выбери запись.");
            return;
        }

        SelectedId = id.Value;
        DialogResult = DialogResult.OK;
        Close();
    }
}
