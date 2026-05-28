using AgroDbApp.Metadata;

namespace AgroDbApp.Forms.Editors;

public sealed class PositionEditForm : BaseEditForm
{
    public PositionEditForm(EditMode mode, long? id = null) : base(Tables.Position, mode, id) { }
}

public sealed class SoilTypeEditForm : BaseEditForm
{
    public SoilTypeEditForm(EditMode mode, long? id = null) : base(Tables.SoilType, mode, id) { }
}

public sealed class TechniqueTypeEditForm : BaseEditForm
{
    public TechniqueTypeEditForm(EditMode mode, long? id = null) : base(Tables.TechniqueType, mode, id) { }
}

public sealed class CultureEditForm : BaseEditForm
{
    public CultureEditForm(EditMode mode, long? id = null) : base(Tables.Culture, mode, id) { }
}

public sealed class EmployeeEditForm : BaseEditForm
{
    public EmployeeEditForm(EditMode mode, long? id = null) : base(Tables.Employee, mode, id) { }
}

public sealed class FieldEditForm : BaseEditForm
{
    public FieldEditForm(EditMode mode, long? id = null) : base(Tables.Field, mode, id) { }
}

public sealed class ParkingEditForm : BaseEditForm
{
    public ParkingEditForm(EditMode mode, long? id = null) : base(Tables.Parking, mode, id) { }
}

public sealed class TechniqueEditForm : BaseEditForm
{
    public TechniqueEditForm(EditMode mode, long? id = null) : base(Tables.Technique, mode, id) { }
}

public sealed class RequestEditForm : BaseEditForm
{
    public RequestEditForm(EditMode mode, long? id = null) : base(Tables.Request, mode, id) { }
}

public sealed class OperationEditForm : BaseEditForm
{
    public OperationEditForm(EditMode mode, long? id = null) : base(Tables.Operation, mode, id) { }
}

public sealed class OperationTechniqueEditForm : BaseEditForm
{
    public OperationTechniqueEditForm(EditMode mode, long? id = null) : base(Tables.OperationTechnique, mode, id) { }
}

public sealed class SowingEditForm : BaseEditForm
{
    public SowingEditForm(EditMode mode, long? id = null) : base(Tables.Sowing, mode, id) { }
}

public sealed class JournalEntryEditForm : BaseEditForm
{
    public JournalEntryEditForm(EditMode mode, long? id = null) : base(Tables.JournalEntry, mode, id) { }
}