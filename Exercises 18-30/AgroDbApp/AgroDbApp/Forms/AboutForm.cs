using System.Windows.Forms;

namespace AgroDbApp.Forms;

public class AboutForm : Form
{
    public AboutForm()
    {
        Text = "О программе";
        Width = 500;
        Height = 300;
        StartPosition = FormStartPosition.CenterParent;

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Text =
                "Agro DB App\n\n" +
                "Приложение для работы с PostgreSQL-базой данных\n" +
                "по предметной области «Сельское хозяйство».\n\n" +
                "Разработчик: студент\n" +
                "Технологии: C#, WinForms, Npgsql, PostgreSQL\n" +
                "Версия: 1.0"
        };

        Controls.Add(label);
    }
}