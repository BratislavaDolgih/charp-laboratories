using System.Drawing;
using System.Windows.Forms;

namespace AgroDbApp.Forms;

public sealed class AboutForm : Form
{
    public AboutForm()
    {
        Text = "О программе";
        StartPosition = FormStartPosition.CenterParent;
        Width = 520;
        Height = 320;

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11),
            Text =
                "Agro DB CRUD\n\n" +
                "Приложение для работы с базой данных PostgreSQL\n" +
                "по предметной области «Сельское хозяйство».\n\n" +
                "Лабораторная работа №9\n" +
                "Технологии: C#, WinForms, Npgsql, PostgreSQL"
        };

        Controls.Add(label);
    }
}