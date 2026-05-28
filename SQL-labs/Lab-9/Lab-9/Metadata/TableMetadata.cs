using System;
using System.Collections.Generic;
using AgroDbApp.Forms.Editors;

namespace AgroDbApp.Metadata;

public enum ColumnKind
{
    Text,
    Int,
    Long,
    Decimal,
    Date,
    DateTime,
    Bool,
    ForeignKey
}

public enum EditMode
{
    Create,
    Update
}

public sealed class ColumnMetadata
{
    public string Name { get; init; } = "";
    public string Label { get; init; } = "";
    public ColumnKind Kind { get; init; }
    public bool Required { get; init; } = false;

    public string? ForeignTable { get; init; }
    public string? ForeignKeyColumn { get; init; }
}

public sealed class TableMetadata
{
    public string TableName { get; init; } = "";
    public string Title { get; init; } = "";
    public string KeyColumn { get; init; } = "id";
    public bool IsView { get; init; } = false;
    public List<ColumnMetadata> Columns { get; init; } = new();
    public Func<EditMode, long?, BaseEditForm>? EditorFactory { get; init; }
}

public static class Tables
{
    public static readonly TableMetadata Position = new()
    {
        TableName = "position",
        Title = "Должности",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "name", Label = "Название", Kind = ColumnKind.Text, Required = true }
        },
        EditorFactory = (mode, id) => new PositionEditForm(mode, id)
    };

    public static readonly TableMetadata SoilType = new()
    {
        TableName = "soil_type",
        Title = "Типы почвы",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "name", Label = "Название", Kind = ColumnKind.Text, Required = true }
        },
        EditorFactory = (mode, id) => new SoilTypeEditForm(mode, id)
    };

    public static readonly TableMetadata TechniqueType = new()
    {
        TableName = "technique_type",
        Title = "Типы техники",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "name", Label = "Название", Kind = ColumnKind.Text, Required = true }
        },
        EditorFactory = (mode, id) => new TechniqueTypeEditForm(mode, id)
    };

    public static readonly TableMetadata Culture = new()
    {
        TableName = "culture",
        Title = "Культуры",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "name", Label = "Название", Kind = ColumnKind.Text, Required = true },
            new() { Name = "ground_expected", Label = "Ожидаемая почва", Kind = ColumnKind.Text }
        },
        EditorFactory = (mode, id) => new CultureEditForm(mode, id)
    };

    public static readonly TableMetadata Employee = new()
    {
        TableName = "employee",
        Title = "Сотрудники",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "last_name", Label = "Фамилия", Kind = ColumnKind.Text, Required = true },
            new() { Name = "first_name", Label = "Имя", Kind = ColumnKind.Text, Required = true },
            new() { Name = "middle_name", Label = "Отчество", Kind = ColumnKind.Text },
            new() { Name = "position_id", Label = "Должность", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "position", ForeignKeyColumn = "id" }
        },
        EditorFactory = (mode, id) => new EmployeeEditForm(mode, id)
    };

    public static readonly TableMetadata Field = new()
    {
        TableName = "field",
        Title = "Угодья",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "name", Label = "Название", Kind = ColumnKind.Text, Required = true },
            new() { Name = "area", Label = "Площадь", Kind = ColumnKind.Decimal, Required = true },
            new() { Name = "status", Label = "Статус", Kind = ColumnKind.Text, Required = true },
            new() { Name = "soil_type_id", Label = "Тип почвы", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "soil_type", ForeignKeyColumn = "id" }
        },
        EditorFactory = (mode, id) => new FieldEditForm(mode, id)
    };

    public static readonly TableMetadata Parking = new()
    {
        TableName = "parking",
        Title = "Парковки",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "name", Label = "Название", Kind = ColumnKind.Text, Required = true },
            new() { Name = "capacity", Label = "Вместимость", Kind = ColumnKind.Int, Required = true }
        },
        EditorFactory = (mode, id) => new ParkingEditForm(mode, id)
    };

    public static readonly TableMetadata Technique = new()
    {
        TableName = "technique",
        Title = "Техника",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "inventory_num", Label = "Инвентарный номер", Kind = ColumnKind.Text, Required = true },
            new() { Name = "parking_id", Label = "Парковка", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "parking", ForeignKeyColumn = "id" },
            new() { Name = "technique_type_id", Label = "Тип техники", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "technique_type", ForeignKeyColumn = "id" }
        },
        EditorFactory = (mode, id) => new TechniqueEditForm(mode, id)
    };

    public static readonly TableMetadata Request = new()
    {
        TableName = "request",
        Title = "Заявки",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "created_at", Label = "Создана", Kind = ColumnKind.DateTime, Required = true },
            new() { Name = "status", Label = "Статус", Kind = ColumnKind.Text, Required = true },
            new() { Name = "created_by_id", Label = "Создал", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "employee", ForeignKeyColumn = "id" },
            new() { Name = "field_id", Label = "Угодье", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "field", ForeignKeyColumn = "id" },
            new() { Name = "culture_id", Label = "Культура", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "culture", ForeignKeyColumn = "id" }
        },
        EditorFactory = (mode, id) => new RequestEditForm(mode, id)
    };

    public static readonly TableMetadata Operation = new()
    {
        TableName = "operation",
        Title = "Операции",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "started_at", Label = "Начало", Kind = ColumnKind.DateTime, Required = true },
            new() { Name = "finished_at", Label = "Завершение", Kind = ColumnKind.DateTime },
            new() { Name = "comment", Label = "Комментарий", Kind = ColumnKind.Text },
            new() { Name = "request_id", Label = "Заявка", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "request", ForeignKeyColumn = "id" },
            new() { Name = "performed_by_id", Label = "Исполнитель", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "employee", ForeignKeyColumn = "id" }
        },
        EditorFactory = (mode, id) => new OperationEditForm(mode, id)
    };

    public static readonly TableMetadata OperationTechnique = new()
    {
        TableName = "operation_technique",
        Title = "Операция-Техника",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "operation_id", Label = "Операция", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "operation", ForeignKeyColumn = "id" },
            new() { Name = "technique_id", Label = "Техника", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "technique", ForeignKeyColumn = "id" },
            new() { Name = "hours_used", Label = "Часы работы", Kind = ColumnKind.Decimal },
            new() { Name = "fuel_spent", Label = "Расход топлива", Kind = ColumnKind.Decimal }
        },
        EditorFactory = (mode, id) => new OperationTechniqueEditForm(mode, id)
    };

    public static readonly TableMetadata Sowing = new()
    {
        TableName = "sowing",
        Title = "Посевы",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "area_ha", Label = "Площадь посева", Kind = ColumnKind.Decimal, Required = true },
            new() { Name = "sowed_at", Label = "Дата посева", Kind = ColumnKind.Date, Required = true },
            new() { Name = "field_id", Label = "Угодье", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "field", ForeignKeyColumn = "id" },
            new() { Name = "culture_id", Label = "Культура", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "culture", ForeignKeyColumn = "id" }
        },
        EditorFactory = (mode, id) => new SowingEditForm(mode, id)
    };

    public static readonly TableMetadata JournalEntry = new()
    {
        TableName = "journal_entry",
        Title = "Журнал",
        KeyColumn = "id",
        Columns = new()
        {
            new() { Name = "operation_id", Label = "Операция", Kind = ColumnKind.ForeignKey, Required = true, ForeignTable = "operation", ForeignKeyColumn = "id" },
            new() { Name = "fixed_at", Label = "Дата фиксации", Kind = ColumnKind.DateTime, Required = true }
        },
        EditorFactory = (mode, id) => new JournalEntryEditForm(mode, id)
    };

    public static readonly TableMetadata VOperationJournal = new()
    {
        TableName = "v_operation_journal",
        Title = "Отчет: журнал операций",
        KeyColumn = "operation_id",
        IsView = true
    };

    public static readonly TableMetadata VTechniqueInfo = new()
    {
        TableName = "v_technique_info",
        Title = "Отчет: состояние техники",
        KeyColumn = "technique_id",
        IsView = true
    };

    public static readonly TableMetadata VSowingSummary = new()
    {
        TableName = "v_sowing_summary",
        Title = "Отчет: сводка по посевам",
        KeyColumn = "sowing_id",
        IsView = true
    };
}