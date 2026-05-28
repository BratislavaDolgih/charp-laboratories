using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using AgroDbApp.Data;
using AgroDbApp.Metadata;

using Npgsql;

namespace AgroDbApp.Forms.Editors;

public abstract class BaseEditForm : Form
{
    private readonly TableMetadata _meta;
    private readonly EditMode _mode;
    private readonly long? _editId;

    private readonly TableLayoutPanel _layout = new();
    private readonly Button _btnOk = new();
    private readonly Button _btnCancel = new();

    private readonly Dictionary<string, Control> _controls = new();
    private readonly Dictionary<string, long?> _fkValues = new();

    protected BaseEditForm(TableMetadata meta, EditMode mode, long? editId)
    {
        _meta = meta;
        _mode = mode;
        _editId = editId;

        InitializeComponent();
        Load += BaseEditForm_Load;
    }

    private void InitializeComponent()
    {
        Text = $"{(_mode == EditMode.Create ? "Добавление" : "Изменение")} - {_meta.Title}";
        Width = 760;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;

        _layout.Dock = DockStyle.Top;
        _layout.AutoSize = true;
        _layout.ColumnCount = 3;
        _layout.RowCount = 0;
        _layout.Padding = new Padding(10);
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));

        foreach (var col in _meta.Columns)
        {
            AddField(col);
        }

        _btnOk.Text = "OK";
        _btnCancel.Text = "Отмена";

        _btnOk.Click += BtnOk_Click;
        _btnCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        var bottom = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(10)
        };

        bottom.Controls.Add(_btnCancel);
        bottom.Controls.Add(_btnOk);

        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };
        scroll.Controls.Add(_layout);

        Controls.Add(scroll);
        Controls.Add(bottom);
    }

    private void AddField(ColumnMetadata col)
    {
        _layout.RowCount++;
        int row = _layout.RowCount - 1;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            Text = col.Label + (col.Required ? " *" : ""),
            AutoSize = true,
            Padding = new Padding(0, 8, 0, 0)
        };

        _layout.Controls.Add(label, 0, row);

        switch (col.Kind)
        {
            case ColumnKind.Text:
            case ColumnKind.Int:
            case ColumnKind.Long:
            case ColumnKind.Decimal:
                var tb = new TextBox { Width = 320 };
                _controls[col.Name] = tb;
                _layout.Controls.Add(tb, 1, row);
                _layout.Controls.Add(new Label(), 2, row);
                break;

            case ColumnKind.Date:
                var dtDate = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Width = 200
                };
                _controls[col.Name] = dtDate;
                _layout.Controls.Add(dtDate, 1, row);
                _layout.Controls.Add(new Label(), 2, row);
                break;

            case ColumnKind.DateTime:
                var dtDateTime = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "yyyy-MM-dd HH:mm:ss",
                    Width = 220
                };
                _controls[col.Name] = dtDateTime;
                _layout.Controls.Add(dtDateTime, 1, row);
                _layout.Controls.Add(new Label(), 2, row);
                break;

            case ColumnKind.Bool:
                var cb = new CheckBox();
                _controls[col.Name] = cb;
                _layout.Controls.Add(cb, 1, row);
                _layout.Controls.Add(new Label(), 2, row);
                break;

            case ColumnKind.ForeignKey:
                var panel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true
                };

                var fkText = new TextBox
                {
                    Width = 220,
                    ReadOnly = true
                };

                var fkBtn = new Button
                {
                    Text = "Выбрать",
                    Width = 90
                };

                _controls[col.Name] = fkText;
                _fkValues[col.Name] = null;

                fkBtn.Click += (_, _) => PickForeignKey(col, fkText);

                panel.Controls.Add(fkText);
                panel.Controls.Add(fkBtn);

                _layout.Controls.Add(panel, 1, row);
                _layout.Controls.Add(new Label(), 2, row);
                break;
        }
    }

    private void BaseEditForm_Load(object? sender, EventArgs e)
    {
        if (_mode == EditMode.Update && _editId.HasValue)
        {
            LoadData(_editId.Value);
        }
    }

    private void LoadData(long id)
    {
        DataRow? row = Pg.GetById(_meta.TableName, _meta.KeyColumn, id);
        if (row == null)
        {
            MessageBox.Show("Запись не найдена.");
            Close();
            return;
        }

        foreach (var col in _meta.Columns)
        {
            object value = row[col.Name];

            if (value == DBNull.Value)
                continue;

            switch (col.Kind)
            {
                case ColumnKind.Text:
                case ColumnKind.Int:
                case ColumnKind.Long:
                case ColumnKind.Decimal:
                    ((TextBox)_controls[col.Name]).Text = Convert.ToString(value);
                    break;

                case ColumnKind.Date:
                case ColumnKind.DateTime:
                    ((DateTimePicker)_controls[col.Name]).Value = Convert.ToDateTime(value);
                    break;

                case ColumnKind.Bool:
                    ((CheckBox)_controls[col.Name]).Checked = Convert.ToBoolean(value);
                    break;

                case ColumnKind.ForeignKey:
                    _fkValues[col.Name] = Convert.ToInt64(value);
                    ((TextBox)_controls[col.Name]).Text = Convert.ToString(value);
                    break;
            }
        }
    }

    private void PickForeignKey(ColumnMetadata col, TextBox textBox)
    {
        if (string.IsNullOrWhiteSpace(col.ForeignTable) || string.IsNullOrWhiteSpace(col.ForeignKeyColumn))
        {
            MessageBox.Show("Для внешнего ключа не настроена связанная таблица.");
            return;
        }

        var fkMeta = TableResolver.Resolve(col.ForeignTable);

        using var form = new CrudForm(fkMeta, readOnlyMode: true, selectionMode: true);
        if (form.ShowDialog(this) == DialogResult.OK && form.SelectedId != null)
        {
            long id = Convert.ToInt64(form.SelectedId);
            _fkValues[col.Name] = id;
            textBox.Text = id.ToString();
        }
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        try
        {
            var values = BuildValues();

            if (_mode == EditMode.Create)
            {
                Pg.Insert(_meta.TableName, values);

                MessageBox.Show(
                    "Запись успешно добавлена.",
                    "Готово",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                if (!_editId.HasValue)
                    throw new InvalidOperationException("Не передан id для обновления.");

                Pg.Update(_meta.TableName, _meta.KeyColumn, _editId.Value, values);

                MessageBox.Show(
                    "Запись успешно обновлена.",
                    "Готово",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(
                "Ошибка PostgreSQL:\n" + ex.MessageText,
                "Ошибка триггера",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Ошибка сохранения:\n" + ex.Message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private Dictionary<string, object?> BuildValues()
    {
        var values = new Dictionary<string, object?>();

        foreach (var col in _meta.Columns)
        {
            object? value = col.Kind switch
            {
                ColumnKind.Text => GetText(col),
                ColumnKind.Int => GetInt(col),
                ColumnKind.Long => GetLong(col),
                ColumnKind.Decimal => GetDecimal(col),
                ColumnKind.Date => ((DateTimePicker)_controls[col.Name]).Value.Date,
                ColumnKind.DateTime => ((DateTimePicker)_controls[col.Name]).Value,
                ColumnKind.Bool => ((CheckBox)_controls[col.Name]).Checked,
                ColumnKind.ForeignKey => GetForeignKey(col),
                _ => null
            };

            values[col.Name] = value;
        }

        return values;
    }

    private object? GetText(ColumnMetadata col)
    {
        string text = ((TextBox)_controls[col.Name]).Text.Trim();
        if (col.Required && string.IsNullOrWhiteSpace(text))
            throw new Exception($"Поле \"{col.Label}\" обязательно.");
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private object? GetInt(ColumnMetadata col)
    {
        string text = ((TextBox)_controls[col.Name]).Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            if (col.Required) throw new Exception($"Поле \"{col.Label}\" обязательно.");
            return null;
        }

        if (!int.TryParse(text, out int value))
            throw new Exception($"Поле \"{col.Label}\" должно быть целым числом.");

        return value;
    }

    private object? GetLong(ColumnMetadata col)
    {
        string text = ((TextBox)_controls[col.Name]).Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            if (col.Required) throw new Exception($"Поле \"{col.Label}\" обязательно.");
            return null;
        }

        if (!long.TryParse(text, out long value))
            throw new Exception($"Поле \"{col.Label}\" должно быть целым числом.");

        return value;
    }

    private object? GetDecimal(ColumnMetadata col)
    {
        string text = ((TextBox)_controls[col.Name]).Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            if (col.Required) throw new Exception($"Поле \"{col.Label}\" обязательно.");
            return null;
        }

        if (!decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value) &&
            !decimal.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out value))
        {
            throw new Exception($"Поле \"{col.Label}\" должно быть числом.");
        }

        return value;
    }

    private object? GetForeignKey(ColumnMetadata col)
    {
        if (!_fkValues.TryGetValue(col.Name, out long? value) || value == null)
        {
            if (col.Required)
                throw new Exception($"Поле \"{col.Label}\" обязательно.");
            return null;
        }

        return value.Value;
    }
}

internal static class TableResolver
{
    public static TableMetadata Resolve(string tableName)
    {
        return tableName switch
        {
            "position" => Tables.Position,
            "soil_type" => Tables.SoilType,
            "technique_type" => Tables.TechniqueType,
            "culture" => Tables.Culture,
            "employee" => Tables.Employee,
            "field" => Tables.Field,
            "parking" => Tables.Parking,
            "technique" => Tables.Technique,
            "request" => Tables.Request,
            "operation" => Tables.Operation,
            "operation_technique" => Tables.OperationTechnique,
            "sowing" => Tables.Sowing,
            "journal_entry" => Tables.JournalEntry,
            _ => throw new InvalidOperationException($"Неизвестная таблица: {tableName}")
        };
    }
}